using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Yohash.React.Editor
{
  public static class StateGenerator
  {
    [MenuItem("Yohash/React/Generate MainState", false, 301)]
    public static void GenerateMainState()
    {
      var stateContainers = GetDerivedTypesFromAllAssemblies();
      GenerateMainStateFiles(stateContainers);
      AssetDatabase.Refresh();
    }

    private static IEnumerable<Type> GetDerivedTypesFromAllAssemblies()
    {
      var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
      var derivedTypes = new List<Type>();

      foreach (var assembly in allAssemblies) {
        var types = assembly.GetTypes()
          .Where(type => type.IsClass
            && !type.IsAbstract
            && type.IsSubclassOf(typeof(StateContainer))
            && type.BaseType == typeof(StateContainer)
            && !type.IsSubclassOf(typeof(PropsContainer)))
          .ToList();
        derivedTypes.AddRange(types);
      }

      return derivedTypes;
    }

    private static void GenerateMainStateFiles(IEnumerable<Type> stateContainers)
    {
      var namespaceGroups = stateContainers.GroupBy(t => t.Namespace);

      foreach (var namespaceGroup in namespaceGroups) {
        var code = GenerateMainStateCode(namespaceGroup.Key, namespaceGroup);
        var name = namespaceGroup.Key?.Split('.').Last() ?? Application.productName;
        var path = $"Assets/_Generated/{name}";
        var filename = $"{name}State.cs";
        if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }

        var fullpath = Path.Combine(path, filename);

        using (var stream = new FileStream(fullpath, FileMode.Create, FileAccess.Write)) {
          using (var writer = new StreamWriter(stream)) {
            writer.Write(code);
          }
        }

        Debug.Log("Wrote file " + fullpath);
      }
    }

    private static string GenerateMainStateCode(string namespaceName, IEnumerable<Type> stateContainers)
    {
      var name = namespaceName?.Split('.').Last() ?? Application.productName;

      var sb = new StringBuilder();
      sb.AppendLine("using Yohash.React;");

      bool noNamespace = namespaceName == null;

      var b = noNamespace ? "" : "  ";
      if (!noNamespace) {
        sb.AppendLine($"namespace {namespaceName}");
        sb.AppendLine("{");
      }
      sb.AppendLine($"{b}public class {name}State : State");
      sb.AppendLine($"{b}{{");

      // **** Add fields
      foreach (var type in stateContainers) {
        sb.AppendLine($"{b}  public {type.Name} {type.Name} = new {type.Name}();");
      }

      // **** Add Reduce method
      sb.AppendLine();
      sb.AppendLine($"{b}  public override void Reduce(IAction action)");
      sb.AppendLine($"{b}  {{");
      foreach (var type in stateContainers) {
        sb.AppendLine($"{b}    {type.Name}.Reduce(action);");
      }
      sb.AppendLine($"{b}  }}");

      // **** Add Clone method
      sb.AppendLine();
      sb.AppendLine($"{b}  public override State Clone()");
      sb.AppendLine($"{b}  {{");
      sb.AppendLine($"{b}    var newState = new {name}State();");
      foreach (var type in stateContainers) {
        sb.AppendLine($"{b}    newState.{type.Name} = {type.Name}.Clone() as {type.Name};");
      }
      sb.AppendLine($"{b}    return newState;");
      sb.AppendLine($"{b}  }}");

      sb.AppendLine($"{b}}}");

      if (!noNamespace) {
        sb.AppendLine("}");
      }

      return sb.ToString();
    }
  }
}

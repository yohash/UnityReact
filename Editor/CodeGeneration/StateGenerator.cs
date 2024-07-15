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
        var name = namespaceGroup.Key.Split('.').Last();
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
      var name = namespaceName.Split('.').Last();

      var sb = new StringBuilder();

      sb.AppendLine($"namespace {namespaceName}");
      sb.AppendLine("{");
      sb.AppendLine($"  public class {name}State : State");
      sb.AppendLine("  {");

      // **** Add fields
      foreach (var type in stateContainers) {
        sb.AppendLine($"    public {type.Name} {type.Name} = new {type.Name}();");
      }

      // **** Add Reduce method
      sb.AppendLine();
      sb.AppendLine("    public override void Reduce(IAction action)");
      sb.AppendLine("    {");
      foreach (var type in stateContainers) {
        sb.AppendLine($"      {type.Name}.Reduce(action);");
      }
      sb.AppendLine("    }");

      // **** Add Clone method
      sb.AppendLine();
      sb.AppendLine("    public override State Clone()");
      sb.AppendLine("    {");
      sb.AppendLine($"      var newState = new {name}State();");
      foreach (var type in stateContainers) {
        sb.AppendLine($"      newState.{type.Name} = {type.Name}.Clone() as {type.Name};");
      }
      sb.AppendLine("      return newState;");
      sb.AppendLine("    }");

      sb.AppendLine("  }");
      sb.AppendLine("}");

      return sb.ToString();
    }
  }
}

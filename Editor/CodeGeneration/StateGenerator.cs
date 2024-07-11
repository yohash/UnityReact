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
    [MenuItem("Yohash/React/Generate MainState")]
    public static void GenerateMainState()
    {
      var stateContainers = GetDerivedTypesFromAllAssemblies();
      GenerateMainStateFiles(stateContainers);
      AssetDatabase.Refresh();
      Debug.Log("Done generating state");
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
        var path = $"Assets/_Generated/{namespaceGroup.Key}";
        var filename = "MainState.cs";
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
      var sb = new StringBuilder();

      sb.AppendLine($"namespace {namespaceName}");
      sb.AppendLine("{");
      sb.AppendLine("  public class MainState");
      sb.AppendLine("  {");

      // **** Add fields
      foreach (var type in stateContainers) {
        sb.AppendLine($"    public {type.Name} {type.Name} = new {type.Name}();");
      }

      // **** Add Reduce method
      sb.AppendLine();
      sb.AppendLine("    public void Reduce(IAction action)");
      sb.AppendLine("    {");
      foreach (var type in stateContainers) {
        sb.AppendLine($"      {type.Name}.Reduce(action);");
      }
      sb.AppendLine("    }");

      // **** Add Clone method
      sb.AppendLine();
      sb.AppendLine("    public MainState Clone()");
      sb.AppendLine("    {");
      sb.AppendLine("      var newState = new MainState();");
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

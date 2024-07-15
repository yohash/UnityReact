using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace Yohash.React.Editor
{
  public static class PropsGenerator
  {
    [MenuItem("Yohash/React/Generate Props Methods", false, 302)]
    public static void GeneratePropsMethods()
    {
      var propsTypes = GeneratePropsMethodsAssemblies();
      GeneratePropsMethodsFiles(propsTypes);
      AssetDatabase.Refresh();
    }

    private static IEnumerable<Type> GeneratePropsMethodsAssemblies()
    {
      var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
      var derivedTypes = new List<Type>();

      foreach (var assembly in allAssemblies) {
        var propsTypes = assembly.GetTypes()
          .Where(t => t.IsSubclassOf(typeof(Props)))
          .ToList();
        derivedTypes.AddRange(propsTypes);
      }

      return derivedTypes;
    }

    private static void GeneratePropsMethodsFiles(IEnumerable<Type> propsTypes)
    {
      var namespaceGroups = propsTypes.GroupBy(t => t.Namespace);

      foreach (var namespaceGroup in namespaceGroups) {
        var code = GeneratePropsMethodsCode(namespaceGroup.Key, namespaceGroup);
        var name = namespaceGroup.Key.Split('.').Last();
        var path = $"Assets/_Generated/{name}";
        var filename = $"{name}PropsMethods.cs";
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

    private static string GeneratePropsMethodsCode(string namespaceName, IEnumerable<Type> propsTypes)
    {
      var name = namespaceName.Split('.').Last();

      var sb = new StringBuilder();
      sb.AppendLine($"namespace {namespaceName}");
      sb.AppendLine("{");

      foreach (var propsType in propsTypes) {
        var propsName = propsType.Name;
        var stateContainerFields = propsType.GetFields(BindingFlags.Public | BindingFlags.Instance)
          .Where(f => f.FieldType.IsSubclassOf(typeof(StateContainer))
            && !f.FieldType.IsSubclassOf(typeof(PropsContainer)))
          .ToList();

        var propsContainerFields = propsType.GetFields(BindingFlags.Public | BindingFlags.Instance)
          .Where(f => f.FieldType.IsSubclassOf(typeof(PropsContainer)))
          .ToList();

        sb.AppendLine($"  public partial class {propsName} : Props");
        sb.AppendLine("  {");

        // **** Generate class constructor
        sb.AppendLine($"    public {propsName}()");
        sb.AppendLine("    {");
        foreach (var field in stateContainerFields) {
          var fieldName = field.Name;
          var fieldType = field.FieldType.Name;
          sb.AppendLine($"      {fieldName} = new {fieldType}();");
        }
        foreach (var container in propsContainerFields) {
          var fieldName = container.Name;
          var fieldType = container.FieldType.Name;
          sb.AppendLine($"      {fieldName} = new {fieldType}();");
        }
        sb.AppendLine("    }");
        sb.AppendLine();

        // **** Generate BuildProps method
        if (stateContainerFields.Count > 0) {
          sb.AppendLine($"    public override void BuildProps(State state)");
          sb.AppendLine("    {");
          sb.AppendLine($"      var _state = state as {name}State;");

          foreach (var field in stateContainerFields) {
            var fieldName = field.Name;
            var fieldType = field.FieldType.Name;
            sb.AppendLine($"      {fieldName} = _state.{fieldType};");
          }
          sb.AppendLine("    }");
          sb.AppendLine();
        }

        // **** Generate DidUpdate method
        sb.AppendLine($"    public override bool DidUpdate()");
        sb.AppendLine("    {");

        if (stateContainerFields.Count > 1) {
          sb.AppendLine($"      return {stateContainerFields[0].Name}.IsDirty ||");
          for (var i = 1; i < stateContainerFields.Count; i++) {
            var fieldName = stateContainerFields[i].Name;
            var line = $"        {fieldName}.IsDirty" + (i == stateContainerFields.Count - 1 ? ";" : " ||");
            sb.AppendLine(line);
          }
        } else if (stateContainerFields.Count == 1) {
          sb.AppendLine($"      return {stateContainerFields[0].Name}.IsDirty;");
        } else if (stateContainerFields.Count == 0) {
          sb.AppendLine($"      return true;");
        }
        sb.AppendLine("    }");

        // **** Generate BuildElement method
        if (propsContainerFields.Count > 0) {
          sb.AppendLine();
          sb.AppendLine($"    public override void BuildElement(PropsContainer propsContainer)");
          sb.AppendLine("    {");
          // currently, only one props container is supported
          var fieldName = propsContainerFields[0].Name;
          var fieldType = propsContainerFields[0].FieldType.Name;
          sb.AppendLine($"      {fieldName} = propsContainer as {fieldType};");
          sb.AppendLine("    }");
        }

        sb.AppendLine("  }");
        if (propsType != propsTypes.Last()) {
          sb.AppendLine();
        }
      }

      sb.AppendLine("}");

      return sb.ToString();
    }
  }
}

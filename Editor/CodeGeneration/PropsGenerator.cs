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
        if (namespaceGroup.Key == "Yohash.React") { continue; }
        var code = GeneratePropsMethodsCode(namespaceGroup.Key, namespaceGroup);
        var name = namespaceGroup.Key?.Split('.').Last() ?? Application.productName;
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
      var name = namespaceName?.Split('.').Last() ?? Application.productName;

      var sb = new StringBuilder();
      sb.AppendLine("using Yohash.React;");

      bool noNamespace = namespaceName == null;

      var b = noNamespace ? "" : "  ";
      if (!noNamespace) {
        sb.AppendLine($"namespace {namespaceName}");
        sb.AppendLine("{");
      }

      foreach (var propsType in propsTypes) {
        var propsName = propsType.Name;
        var stateContainerFields = propsType.GetFields(BindingFlags.Public | BindingFlags.Instance)
          .Where(f => f.FieldType.IsSubclassOf(typeof(StateContainer))
            && !f.FieldType.IsSubclassOf(typeof(PropsContainer)))
          .ToList();

        var propsContainerFields = propsType.GetFields(BindingFlags.Public | BindingFlags.Instance)
          .Where(f => f.FieldType.IsSubclassOf(typeof(PropsContainer)))
          .ToList();

        sb.AppendLine($"{b}public partial class {propsName} : Props");
        sb.AppendLine($"{b}{{");

        // **** Generate class constructor
        sb.AppendLine($"{b}  public {propsName}()");
        sb.AppendLine($"{b}  {{");
        foreach (var field in stateContainerFields) {
          var fieldName = field.Name;
          var fieldType = field.FieldType.Name;
          sb.AppendLine($"{b}    {fieldName} = new {fieldType}();");
        }
        foreach (var container in propsContainerFields) {
          var fieldName = container.Name;
          var fieldType = container.FieldType.Name;
          sb.AppendLine($"{b}    {fieldName} = new {fieldType}();");
        }
        sb.AppendLine($"{b}  }}");
        sb.AppendLine();

        // **** Generate BuildProps method
        if (stateContainerFields.Count > 0) {
          sb.AppendLine($"{b}  public override void BuildProps(State state)");
          sb.AppendLine($"{b}  {{");
          sb.AppendLine($"{b}    var _state = state as {name}State;");

          foreach (var field in stateContainerFields) {
            var fieldName = field.Name;
            var fieldType = field.FieldType.Name;
            sb.AppendLine($"{b}    {fieldName} = _state.{fieldType}.Clone() as {fieldType};");
          }
          sb.AppendLine($"{b}  }}");
          sb.AppendLine();
        }

        // **** Generate DidUpdate method
        sb.AppendLine($"{b}  public override bool DidUpdate(State state)");
        sb.AppendLine($"{b}  {{");

        if (stateContainerFields.Count > 1) {
          sb.AppendLine($"{b}    var _state = state as {name}State;");
          sb.AppendLine($"{b}    return _state.{stateContainerFields[0].FieldType.Name}.IsDirty ||");
          for (var i = 1; i < stateContainerFields.Count; i++) {
            var fieldType = stateContainerFields[i].FieldType.Name;
            var line = $"{b}      _state.{fieldType}.IsDirty" + (i == stateContainerFields.Count - 1 ? ";" : " ||");
            sb.AppendLine(line);
          }
        } else if (stateContainerFields.Count == 1) {
          sb.AppendLine($"{b}    var _state = state as {name}State;");
          sb.AppendLine($"{b}    return _state.{stateContainerFields[0].FieldType.Name}.IsDirty;");
        } else if (stateContainerFields.Count == 0) {
          sb.AppendLine($"{b}    return true;");
        }
        sb.AppendLine("    }");

        // **** Generate BuildElement method
        if (propsContainerFields.Count > 0) {
          sb.AppendLine();
          sb.AppendLine($"{b}  public override void BuildElement(PropsContainer propsContainer)");
          sb.AppendLine($"{b}  {{");
          // currently, only one props container is supported
          var fieldName = propsContainerFields[0].Name;
          var fieldType = propsContainerFields[0].FieldType.Name;
          sb.AppendLine($"{b}    {fieldName} = propsContainer as {fieldType};");
          sb.AppendLine($"{b}  }}");
          sb.AppendLine("");
          sb.AppendLine($"{b}  public override bool HasCustomProps => true; ");
        }

        sb.AppendLine($"{b}}}");
        if (propsType != propsTypes.Last()) {
          sb.AppendLine();
        }
      }

      if (!noNamespace) {
        sb.AppendLine("}");
      }

      return sb.ToString();
    }
  }
}

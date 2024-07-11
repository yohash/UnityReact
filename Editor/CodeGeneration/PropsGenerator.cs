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
    [MenuItem("Yohash/React/Generate Props Methods")]
    public static void GeneratePropsMethods()
    {
      var assemblies = AppDomain.CurrentDomain.GetAssemblies();
      foreach (var assembly in assemblies) {
        var propsTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Props))).ToList();
        if (propsTypes.Count > 0) {
          GeneratePropsMethods(assembly, propsTypes);
        }
      }
      AssetDatabase.Refresh();
    }

    private static void GeneratePropsMethods(Assembly assembly, List<Type> propsTypes)
    {
      var mainStateType = assembly.GetTypes()
        .FirstOrDefault(t => t.Name == "MainState");

      if (mainStateType == null) {
        Debug.LogWarning($"MainState not found in assembly: {assembly.FullName}");
        return;
      }

      var namespaceName = mainStateType.Namespace;
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
          sb.AppendLine("      var mainState = state as MainState;");

          foreach (var field in stateContainerFields) {
            var fieldName = field.Name;
            var fieldType = field.FieldType.Name;
            sb.AppendLine($"      {fieldName} = mainState.{fieldType};");
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

      var path = $"Assets/_Generated/{namespaceName}/";
      var filename = $"PropsMethods.cs";
      if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }

      var fullpath = Path.Combine(path, filename);

      using (var stream = new FileStream(fullpath, FileMode.Create, FileAccess.Write)) {
        using (var writer = new StreamWriter(stream)) {
          writer.Write(sb.ToString());
        }
      }

      AssetDatabase.Refresh();
    }
  }
}

using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Yohash.React.Editor
{
  public static class MiddlewaresGenerator
  {
    //  [MenuItem("Yohash/React/Generate Middlewares", false, 302)]
    //  public static void GenerateMiddlewares()
    //  {
    //    var middlewares = GetDerivedTypesFromAllAssemblies();
    //    GenerateMiddlewaresFiles(middlewares);
    //    AssetDatabase.Refresh();
    //  }

    //  private static IEnumerable<Type> GetDerivedTypesFromAllAssemblies()
    //  {
    //    var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
    //    var derivedTypes = new List<Type>();

    //    foreach (var assembly in allAssemblies) {
    //      var types = assembly.GetTypes()
    //        .Where(type => type.IsClass
    //          && !type.IsAbstract
    //          && type.IsSubclassOf(typeof(Middleware))
    //          && type.BaseType == typeof(Middleware))
    //        .ToList();
    //      derivedTypes.AddRange(types);
    //    }

    //    return derivedTypes;
    //  }

    //  private static void GenerateMiddlewaresFiles(IEnumerable<Type> middlewares)
    //  {
    //    var namespaceGroups = middlewares.GroupBy(t => t.Namespace);

    //    foreach (var namespaceGroup in namespaceGroups) {
    //      var code = GenerateMiddlewareCode(namespaceGroup.Key, namespaceGroup);
    //      var name = namespaceGroup.Key.Split('.').Last();
    //      var path = $"Assets/_Generated/{name}";
    //      var filename = $"{name}Middlewares.cs";
    //      if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }

    //      var fullpath = Path.Combine(path, filename);

    //      using (var stream = new FileStream(fullpath, FileMode.Create, FileAccess.Write)) {
    //        using (var writer = new StreamWriter(stream)) {
    //          writer.Write(code);
    //        }
    //      }

    //      Debug.Log("Wrote file " + fullpath);
    //    }
    //  }

    //  private static string GenerateMiddlewareCode(string namespaceName, IEnumerable<Type> middlewares)
    //  {
    //    var name = namespaceName.Split('.').Last();

    //    var sb = new StringBuilder();

    //    sb.AppendLine("using System;");
    //    sb.AppendLine();
    //    sb.AppendLine($"namespace {namespaceName}");
    //    sb.AppendLine("{");

    //    // **** Create the custom namespace-specific middlewares extension
    //    sb.AppendLine($"  public abstract class {name}Middleware : Middleware");
    //    sb.AppendLine("  {");
    //    sb.AppendLine($"    public abstract IAction HandleAction({name}State state, IAction action, Action<IAction> dispatch);");
    //    sb.AppendLine($"    public override IAction HandleAction(State state, IAction action, Action<IAction> dispatch)");
    //    sb.AppendLine("    {");
    //    sb.AppendLine("      return HandleAction((BasicUiState)state, action, dispatch);");
    //    sb.AppendLine("    }");
    //    sb.AppendLine("  }");
    //    sb.AppendLine();

    //    // **** Create the middlewares class
    //    sb.AppendLine($"  public class {name}Middlewares : Middlewares");
    //    sb.AppendLine("  {");

    //    // **** Add fields
    //    foreach (var type in middlewares) {
    //      sb.AppendLine($"    public {type.Name} {type.Name} = new {type.Name}();");
    //    }

    //    // **** Add HandleAction method
    //    sb.AppendLine();
    //    sb.AppendLine("    public IAction HandleAction(State state, IAction action, Action<IAction> dispatch)");
    //    sb.AppendLine("    {");
    //    foreach (var type in middlewares) {
    //      sb.AppendLine($"      action = {type.Name}.HandleAction(state, action, dispatch);");
    //      if (type != middlewares.Last()) {
    //        sb.AppendLine($"      if (action == null) {{ return action; }}");
    //      }
    //    }
    //    sb.AppendLine();
    //    sb.AppendLine("      return action;");
    //    sb.AppendLine("    }");
    //    sb.AppendLine("  }");
    //    sb.AppendLine("}");

    //    return sb.ToString();
    //  }
  }
}

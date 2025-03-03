using System.Reflection;
using System.Linq;
using System.Diagnostics;

namespace Yohash.React
{
  public static class ActionExtensions
  {
    public static string ToDetailedString(this IAction action)
    {
      var type = action.GetType();
      var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
      var fieldValues = string.Join("\n\t", fields.Select(f => $"{f.Name}\t = {f.GetValue(action)}"));
      return $"{type.Name}\n\t{fieldValues}";
    }

    public static string ToDetailedString(this IDebugAction debug)
    {
      var details = (debug.Action).ToDetailedString();
      var trace = debug.StackTrace.GetFrames();
      return $"{details}\n\tStackTrace\t = {debug.StackTrace.ToString()}\n***\n\n";
    }

    public static IDebugAction AsDebug(this IAction action)
    {
      return new DebugAction(action);
    }
  }
}

using System.Reflection;
using System.Linq;

namespace Yohash.React
{
  public static class ActionExtensions
  {
    public static string ToDetailedString(this IAction action)
    {
      var type = action.GetType();
      var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
      var fieldValues = string.Join(", ", fields.Select(f => $"{f.Name}={f.GetValue(action)}"));
      return $"{type.Name}({fieldValues})";
    }
  }
}

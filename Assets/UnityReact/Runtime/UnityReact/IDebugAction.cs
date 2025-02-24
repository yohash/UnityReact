using System.Diagnostics;

namespace Yohash.React
{
  public interface IDebugAction : IAction
  {
    public IAction Action { get; }
    public StackTrace StackTrace { get; }
  }
}

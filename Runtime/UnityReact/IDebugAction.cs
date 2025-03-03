using System.Diagnostics;

namespace Yohash.React
{
  public interface IDebugAction : IAction
  {
    public IAction Action { get; }
    public StackTrace StackTrace { get; }
  }

  public class DebugAction : IDebugAction
  {
    private StackTrace _stackTrace;
    private IAction _action;

    public DebugAction(IAction action)
    {
      _action = action;
      _stackTrace = new StackTrace(true);
    }

    public IAction Action => _action;
    public StackTrace StackTrace => _stackTrace;
  }
}

using System;
namespace Yohash.React
{
  public abstract class Middleware
  {
    public abstract IAction HandleAction(
      State state,
      IAction action,
      Action<IAction> dispatch
    );
  }
}

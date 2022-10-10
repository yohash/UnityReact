namespace Yohash.React
{
  public abstract class Middleware
  {
    public abstract Action HandleAction(
      State state,
      Action action,
      System.Action<Action> dispatch
    );
  }
}

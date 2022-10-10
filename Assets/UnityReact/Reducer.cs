namespace Yohash.React
{
  public abstract class Reducer
  {
    //public abstract T Reduce(T oldState, Action action);
  }

  public abstract class Reducer<T> : Reducer
  {
    public abstract T Reduce(T oldState, Action action);
  }
}

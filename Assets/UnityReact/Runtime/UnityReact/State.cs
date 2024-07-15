namespace Yohash.React
{
  [System.Serializable]
  public abstract class State
  {
    public State() { }
    public abstract State Clone();
    public abstract void Reduce(IAction action);
  }
}

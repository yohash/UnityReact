namespace Yohash.React
{
  [System.Serializable]
  public class State
  {
    public State() { }
    public virtual State Clone()
    {
      var copy = new State();
      return copy;
    }

    public virtual void Reduce(IAction action) { }
  }
}

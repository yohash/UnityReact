namespace Yohash.React
{
  [System.Serializable]
  public abstract class StateContainer
  {
    public bool IsDirty = false;

    public StateContainer Copy {
      get {
        return MemberwiseClone() as StateContainer;
      }
    }

    public void Reduce(Action action)
    {
      IsDirty = reduce(action);
    }

    /// <summary>
    /// This reduce function should return a bool
    /// that indicates DidStateUpdate
    /// </summary>
    protected abstract bool reduce(Action action);
  }

  public class EmptyStateContainer : StateContainer
  {
    protected override bool reduce(Action action)
    {
      return false;
    }
  }
}

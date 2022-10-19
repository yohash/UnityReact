namespace Yohash.React
{
  [System.Serializable]
  public abstract class StateContainer
  {
    public bool IsDirty = false;

    public StateContainer Copy {
      get {
        return (StateContainer)MemberwiseClone();
      }
    }

    public void Reduce(Action action)
    {
      IsDirty = true;
      var didReduce = reduce(action);
      if (!didReduce) { IsDirty = false; }
    }

    /// <summary>
    /// This reduce function should return a bool
    /// that indicates DidStateUpdate
    /// </summary>
    protected abstract bool reduce(Action action);
  }
}

namespace Yohash.React
{
  [System.Serializable]
  public abstract class StateContainer
  {
    // TBD
    //public bool IsDirty = false;

    public StateContainer Copy {
      get {
        //IsDirty = true;
        return (StateContainer)MemberwiseClone();
      }
    }

    public abstract StateContainer Reduce(Action action);
  }
}

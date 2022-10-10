namespace Yohash.React
{
  [System.Serializable]
  public struct StateContainer
  {
    // TBD
    //public bool IsDirty = false;

    public StateContainer Copy {
      get {
        //IsDirty = true;
        return (StateContainer)MemberwiseClone();
      }
    }

    // override this to reduce state
    public StateContainer Reduce(Action action)
    {
      return Copy;
    }
  }
}

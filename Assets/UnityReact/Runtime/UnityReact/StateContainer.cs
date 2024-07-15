namespace Yohash.React
{
  [System.Serializable]
  public abstract class StateContainer
  {
    public bool IsDirty = false;

    public StateContainer Clone()
    {
      return MemberwiseClone() as StateContainer;
    }

    public void Reduce(IAction action)
    {
      IsDirty = reduce(action);
    }

    /// <summary>
    /// This reduce function should return a bool
    /// that indicates DidStateUpdate
    /// </summary>
    protected abstract bool reduce(IAction action);
  }

  public class EmptyPropsContainer : PropsContainer { }

  public abstract class PropsContainer : StateContainer
  {
    public static PropsContainer Empty = new EmptyPropsContainer();

    protected override bool reduce(IAction action)
    {
      return false;
    }
  }
}


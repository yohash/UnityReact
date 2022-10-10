using System.Collections.Generic;

namespace Yohash.React
{
  [System.Serializable]
  public struct State
  {
    private List<StateContainer> containers;

    public void AddState(StateContainer container)
    {
      if (containers == null) {
        containers = new List<StateContainer>();
      }
      containers.Add(container);
    }

    public State Copy {
      get {
        return (State)MemberwiseClone();
      }
    }

    public State Reduce(Action action)
    {
      for (int i = 0; i < containers.Count; i++) {
        containers[i].Reduce(action);
      }
      return this;
    }

    // TBD best way to get individual state containers
    // to components that request them, vs. sending
    // all state
    public StateContainer GetState<T>()
    {
      foreach (var state in containers) {
        if (state is T) { return state; }
      }
      return new StateContainer();
    }
  }
}

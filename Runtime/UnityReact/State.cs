using System.Collections.Generic;

namespace Yohash.React
{
  [System.Serializable]
  public class State
  {
    private List<StateContainer> containers
      = new List<StateContainer>();

    public List<StateContainer> Containers {
      get { return containers; }
    }

    public int Count { get { return containers.Count; } }

    public State Copy {
      get {
        var copy = new State();
        foreach (var container in containers) {
          copy.AddState(container.Copy);
        }
        return copy;
      }
    }

    public bool TryGetState<T>(out T sc) where T : StateContainer
    {
      foreach (var container in containers) {
        if (container is T) {
          sc = container as T;
          return true;
        }
      }
      sc = default;
      return false;
    }

    public void AddState(StateContainer container)
    {
      containers.Add(container);
    }

    public State Reduce(Action action)
    {
      for (int i = 0; i < containers.Count; i++) {
        containers[i].Reduce(action);
      }
      return this;
    }
  }
}

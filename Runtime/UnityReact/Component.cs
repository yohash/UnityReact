using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Yohash.React
{
  public class NoProps : Props
  {
    public override List<StateContainer> state => _state;
    private List<StateContainer> _state = new List<StateContainer>();
    public override void SetState(List<StateContainer> containers) { }
  }

  public abstract class Props
  {
    public abstract List<StateContainer> state { get; }
    public abstract void SetState(List<StateContainer> containers);
    public Props Copy { get { return (Props)MemberwiseClone(); } }
  };

  public abstract class Component<T> : MonoBehaviour
    where T : Props
  {
    private bool initialized = false;
    private Queue<Action> _queue = new Queue<Action>();

    public abstract T props { get; }
    protected T oldProps;

    private void Update()
    {
      if (!initialized) {
        subscribe();
      }
    }

    private bool subscribe()
    {
      if (Store.Instance == null) { return false; }

      Store.Instance.Subscribe(onStoreUpdate, onStoreInitialize);

      while (_queue.Count > 0) {
        Action waiting = _queue.Dequeue();
        dispatch(waiting);
      }

      initialized = true;
      return true;
    }

    private void OnDisable()
    {
      Store.Instance.Unsubscribe(onStoreUpdate);
    }

    protected void dispatch(Action action)
    {
      if (!initialized) {
        _queue.Enqueue(action);
        return;
      }
      Store.Instance.Dispatch(action);
    }

    internal void onStoreInitialize(State state)
    {
      // first we build props from state, then assign
      // old props to the same set of props
      buildProps(state);
      oldProps = (T)props.Copy;
      InitializeComponent();
    }

    internal void onStoreUpdate(State oldState, State state)
    {
      oldProps = (T)props.Copy;
      buildProps(state);
      if (propsDidUpdate()) {
        UpdateComponent();
      }
    }

    internal void buildProps(State state)
    {
      var stateContainers = new List<StateContainer>();

      foreach (var container in state.Containers) {
        if (props.state.Any(t => container.GetType().Equals(t.GetType()))) {
          stateContainers.Add(container);
        }
      }
      props.SetState(stateContainers);
    }

    internal bool propsDidUpdate()
    {
      return props.state.Any(s => s.IsDirty);
    }

    public abstract void InitializeComponent();
    public abstract void UpdateComponent();
  }
}

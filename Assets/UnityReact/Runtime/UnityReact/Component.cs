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
    public Props Copy { get { return MemberwiseClone() as Props; } }
  };

  public abstract class Component<T> : MonoBehaviour
    where T : Props
  {
    private bool initialized = false;
    private Queue<IAction> _queue = new Queue<IAction>();

    public abstract T props { get; }
    protected T oldProps;

    private void Update()
    {
      if (!initialized) {
        subscribe();
      }
    }

    private void subscribe()
    {
      if (Store.Instance == null) { return; }
      initialized = true;

      Store.Instance.Subscribe(onStoreUpdate, onStoreInitialize);

      while (_queue.Count > 0) {
        IAction waiting = _queue.Dequeue();
        dispatch(waiting);
      }
    }

    private void OnDestroy()
    {
      Store.Instance?.Unsubscribe(onStoreUpdate);
    }

    protected void dispatch(IAction action)
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
      oldProps = props.Copy as T;
      InitializeComponent();
    }

    internal void onStoreUpdate(State oldState, State state)
    {
      oldProps = props.Copy as T;
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

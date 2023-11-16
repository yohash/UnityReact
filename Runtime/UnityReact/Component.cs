using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Analytics;

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

  public abstract class Component<T> : MonoBehaviour, IComponent
    where T : Props
  {
    private bool initialized = false;
    private Queue<IAction> _queue = new Queue<IAction>();

    private Dictionary<string, Element> children
      = new Dictionary<string, Element>();

    public abstract T props { get; }

    public Transform Transform => transform;

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
        var elements = UpdateComponent();
        updateChildren(elements);
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

    internal async void updateChildren(IEnumerable<Element> elements)
    {
      // check list of elements for
      // (1) new elements - to mount & add
      // (2) missing elements - to destroy & remove
      var newElements = elements.Where(e => !children.ContainsKey(e.Address)).ToList();
      var missing = children
        .Where(c => !elements.Any(e => e.Address == c.Key))
        .Select(kvp => kvp.Value)
        .ToList();

      // (1) mount & add - new elements
      foreach (var element in newElements) {
        // immediately add the child element so it is tracked while
        // the mounter is generating the component
        children.Add(element.Address, element);
        element.Component = await element.Mount;
        element.Component.Transform.SetParent(element.Parent);
        element.Component.Transform.localPosition = Vector3.zero;
        element.Component.Transform.localRotation = Quaternion.identity;
      }

      // (2) destroy & remove - missing elements
      foreach (var element in missing) {
        if (element.Component != null
          && element.Component.Transform != null) {
          Destroy(element.Component.Transform.gameObject);
        }
        children.Remove(element.Address);
      }
    }

    internal bool propsDidUpdate()
    {
      return props.state.Any(s => s.IsDirty);
    }

    public abstract void InitializeComponent();
    public abstract IEnumerable<Element> UpdateComponent();
  }
}

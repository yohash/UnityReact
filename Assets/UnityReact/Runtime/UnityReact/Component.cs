using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Yohash.React
{
  public abstract class Component<T> : MonoBehaviour, IComponent
    where T : Props
  {
    private Queue<IAction> _queue = new Queue<IAction>();
    private HashSet<Element> children = new HashSet<Element>();

    protected T oldProps;
    public abstract T props { get; }
    public Transform Transform => transform;
    public void Unmount() => unmount();

    // state-tracker bools
    private bool initialized = false;
    private bool isUpdating = false;

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
      unmount();
    }

    /// <summary>
    /// This unmount function is only called when this gameobject is destroyed,
    /// so it only applies to the root component.
    /// </summary>
    internal void unmount()
    {
      Store.Instance?.Unsubscribe(onStoreUpdate);
      // iterate backwards over the children, destroying each one
      for (int i = children.Count - 1; i >= 0; i--) {
        var child = children.ElementAt(i);
        children.Remove(child);
        child.Unmount();
      }
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
      // old props to the same set of props for initialization
      buildProps(state);
      oldProps = props.Copy as T;
      InitializeComponent();
      // add a post-initialize update to extract any child elements
      updateComponentAndChildren();
    }

    internal void onStoreUpdate(State oldState, State state)
    {
      oldProps = props.Copy as T;
      buildProps(state);
      if (propsDidUpdate()) {
        updateComponentAndChildren();
      }
    }

    internal void updateComponentAndChildren()
    {
      var elements = UpdateComponent();
      updateChildren(elements);
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
      // TODO - this is a hack to prevent children from updating while we're awaiting
      //        any given mounter to finish.
      //        If we keep this approach, replace the Task.Yield() with a
      //        prop Unity Async tool
      if (isUpdating) { await Task.Yield(); }
      isUpdating = true;

      // check list of elements for
      // (1) new elements - to mount & add
      var newElements = elements.Where(e => !children.Any(c => c.Address == e.Address)).ToList();
      foreach (var element in newElements) {
        // immediately add the child element so it is tracked while
        // the mounter is generating the component
        children.Add(element);
        // the mounting method is awaited so that we can perform
        // async file or web IO to download assets
        element.Component = await element.Mount();
      }

      // (2) missing elements - to destroy & remove
      var missing = children.Where(c => !elements.Any(e => e.Address == c.Address)).ToList();
      foreach (var child in missing) {
        // immediately remove the child from the tracked list before unmounting
        children.Remove(child);
        await child.Unmount();
      }

      // (3) existing elements - to update
      foreach (var child in children) {
        // place props in a list of state containers, then set the local props state
        // in order to update the component with props
        var newProps = elements.FirstOrDefault(e => e.Address == child.Address)?.Props;
        if (newProps == null) { continue; }
        // update the child component, so it can receive the props update
        child.Component.UpdateElementWithProps(newProps);
      }

      isUpdating = false;
    }

    public void UpdateElementWithProps(PropsContainer propsContainer)
    {
      oldProps = props.Copy as T;
      props.SetState(new List<StateContainer>() { propsContainer });
      // update the child component, so it can receive the props update
      // using the recursive method so elements can mount elements in turn
      // TBD - can we add a props-did-update check here?
      // if (propsDidUpdate(oldProps, props)) {
      updateComponentAndChildren();
    }

    internal bool propsDidUpdate()
    {
      return props.state.Any(s => s.IsDirty);
    }

    public abstract void InitializeComponent();
    public abstract IEnumerable<Element> UpdateComponent();
  }
}

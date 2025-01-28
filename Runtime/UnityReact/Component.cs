using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Yohash.React
{
  public abstract class Component<T> : MonoBehaviour, IComponent
    where T : Props, new()
  {
    private HashSet<Element> children = new HashSet<Element>();

    protected T oldProps = new();
    protected T props { get => _props; }
    private T _props = new();

    public Transform Transform => transform;
    public Object Object => this;
    public void Unmount() => unmount();

    private bool isUpdating = false;
    private Store _store;

    protected virtual void OnEnable()
    {
      _ = subscribe();
    }

    private async Task subscribe()
    {
      _store = await Store.Instance;
      if (Store.Instance == null) {
        throw new ComponentCreatedWithNoStore($"Component {name} await store, was created with no Store instance.");
      }

      try {
        _store.Subscribe(onStoreUpdate, onStoreInitialize);
      } catch (System.Exception ex) {
        Debug.LogError(ex);
      }
    }

    protected virtual void OnDisable()
    {
      unmount();
    }

    /// <summary>
    /// This unmount function is only called when this gameobject is destroyed,
    /// so it only applies to the root component.
    /// </summary>
    internal void unmount()
    {
      _store.Unsubscribe(onStoreUpdate);
      // iterate backwards over the children, destroying each one
      for (int i = children.Count - 1; i >= 0; i--) {
        var child = children.ElementAt(i);
        children.Remove(child);
        child.Unmount();
      }
    }

    protected void dispatch(IAction action)
    {
      _store.Dispatch(action);
    }

    internal void onStoreInitialize(State state)
    {
      // if this Component was mounted by a parent Component via Element,
      // we don't want to initialize from the store if the parent has to
      // assemble a custom PropsContainer for this component
      if (props.HasCustomProps) { return; }

      // first we build props from state, then assign
      // old props to the same set of props for initialization
      props.BuildProps(state);
      oldProps = props.Clone() as T;
      InitializeComponent();

      // see if the props have changed at all as a result of the
      // InitializeComponent() method. This can happen if dispatches to state
      // occur during initialization
      if (props.DidUpdate(state)) {
        oldProps = props.Clone() as T;
      }

      // finally, add a post-initialize update to extract any child elements
      updateComponentAndChildren(state);
    }

    internal void onStoreUpdate(State oldState, State state)
    {
      if (props.DidUpdate(state)) {
        oldProps = props.Clone() as T;
        props.BuildProps(state);
        updateComponentAndChildren(state);
      }
    }

    internal void updateComponentAndChildren(State state)
    {
      var elements = UpdateComponent();
      updateChildren(elements, state);
    }

    internal async void updateChildren(IEnumerable<Element> elements, State state)
    {
      // TODO - this is a hack to prevent children from updating while we're awaiting
      //        any given child update to finish.
      //        If we keep this approach, replace the Task.Yield() with a
      //        prop Unity Async tool
      while (isUpdating) { await Task.Yield(); }
      isUpdating = true;

      // check list of elements for
      // (1) new elements - to mount & add
      var newElements = elements.Where(e => !children.Any(c => c.Key == e.Key)).ToList();
      foreach (var element in newElements) {
        // immediately add the child element so it is tracked while
        // the mounter is generating the component
        children.Add(element);
        // the mounting method is awaited so that we can perform
        // async file or web IO to download assets
        element.Component = await element.Mount();
        // once the new child is mounted, initialize then run their update method
        var newProps = propsContainer(element);
        element.Component.InitializeElement(newProps, state);
        // set the index upon creation
        element.Component.Transform.SetSiblingIndex(element.Index);
      }

      // (2) missing elements - to destroy & remove
      var missing = children.Where(c => !elements.Any(e => e.Key == c.Key)).ToList();
      foreach (var child in missing) {
        // immediately remove the child from the tracked list before unmounting
        children.Remove(child);
        await child.Unmount();
      }

      // (3) existing elements - to update
      foreach (var child in children) {
        // if the component is null, it likely hasn't finished mounting yet
        // simply continue on, as each component will have its update method 
        // called when it's done mounting
        if (child.Component == null) { continue; }
        // update the child component, so it can receive the props update
        var newProps = propsContainer(child);
        child.Component.UpdateElement(newProps, state);
        // update the index in case it has changed
        child.Component.Transform.SetSiblingIndex(child.Index);
      }

      // this container can be null, if an element is mounted without a custom
      // set of props in the PropsContainer
      // TODO - is there a better way to directly reference the props?
      PropsContainer propsContainer(Element child)
        => elements.FirstOrDefault(e => e.Key == child.Key)?.Props
          ?? PropsContainer.Empty;

      isUpdating = false;
    }

    public void InitializeElement(PropsContainer propsContainer, State state)
    {
      // For initialization, build both props and element
      props.BuildProps(state);
      props.BuildElement(propsContainer);
      oldProps = props.Clone() as T;
      InitializeComponent();
    }

    public void UpdateElement(PropsContainer propsContainer, State state)
    {
      // TODO - is an "Element did change" style of method here worth while?
      //        Can we only update elements when needed?
      oldProps = props.Clone() as T;
      props.BuildElement(propsContainer);
      // only rebuild the props if they've updated
      if (props.DidUpdate(state)) {
        props.BuildProps(state);
      }
      updateComponentAndChildren(state);
    }

    public abstract void InitializeComponent();
    public abstract IEnumerable<Element> UpdateComponent();
  }
}

public class ComponentCreatedWithNoStore : System.Exception
{
  public ComponentCreatedWithNoStore(string message) : base(message) { }
}

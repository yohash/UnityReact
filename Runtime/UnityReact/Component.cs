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

    private void Start()
    {
      subscribe();
    }

    private void subscribe()
    {
      if (Store.Instance == null)
      {
        throw new ComponentCreatedWithNoStore($"Component {name} was created with no Store instance.");
      }

      Store.Instance.Subscribe(onStoreUpdate, onStoreInitialize);
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
      for (int i = children.Count - 1; i >= 0; i--)
      {
        var child = children.ElementAt(i);
        children.Remove(child);
        child.Unmount();
      }
    }

    protected void dispatch(IAction action)
    {
      Store.Instance.Dispatch(action);
    }

    internal void onStoreInitialize(State state)
    {
      // first we build props from state, then assign
      // old props to the same set of props for initialization
      props.BuildProps(state);
      oldProps = props.Clone() as T;
      InitializeComponent();

      // next, add a post-initialize update to extract any child elements
      // but first, see if the props have changed at all as a result of the
      // InitializeComponent(); method
      if (props.DidUpdate(state)) {
        oldProps = props.Clone() as T;
      }
      updateComponentAndChildren();
    }

    internal void onStoreUpdate(State oldState, State state)
    {
      if (props.DidUpdate(state))
      {
        oldProps = props.Clone() as T;
        props.BuildProps(state);
        updateComponentAndChildren();
      }
    }

    internal void updateComponentAndChildren()
    {
      var elements = UpdateComponent();
      updateChildren(elements);
    }

    internal async void updateChildren(IEnumerable<Element> elements)
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
      foreach (var element in newElements)
      {
        // immediately add the child element so it is tracked while
        // the mounter is generating the component
        children.Add(element);
        // the mounting method is awaited so that we can perform
        // async file or web IO to download assets
        element.Component = await element.Mount();
      }

      // (2) missing elements - to destroy & remove
      var missing = children.Where(c => !elements.Any(e => e.Key == c.Key)).ToList();
      foreach (var child in missing)
      {
        // immediately remove the child from the tracked list before unmounting
        children.Remove(child);
        await child.Unmount();
      }

      // (3) existing elements - to update
      foreach (var child in children) {
        // place props in a list of state containers, then set the local props state
        // in order to update the component with props
        var newProps = elements.FirstOrDefault(e => e.Key == child.Key)?.Props;
        if (newProps == null) { continue; }
        // TODO - if the component is null, we need to wait for it to be mounted
        //        Is there any way we can more accurately await the mounter? Rather
        //        than just waiting for a frame? This could result in locked logic too,
        //        if the mounter fails.
        while (child.Component == null) { await Task.Yield(); }
        // update the child component, so it can receive the props update
        child.Component.UpdateElementWithProps(newProps);
      }

      isUpdating = false;
    }

    public void UpdateElementWithProps(PropsContainer propsContainer)
    {
      // TODO - is an "Element did change" style of method here worth while?
      //        Can we only update elements when needed?
      oldProps = props.Clone() as T;
      props.BuildElement(propsContainer);
      // TBD - can we add a props-did-update check here?
      // if (propsDidUpdate(oldProps, props)) {
      updateComponentAndChildren();
    }

    public abstract void InitializeComponent();
    public abstract IEnumerable<Element> UpdateComponent();
  }
}

public class ComponentCreatedWithNoStore : System.Exception
{
  public ComponentCreatedWithNoStore(string message) : base(message) { }
}

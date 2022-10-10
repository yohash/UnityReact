using System.Collections.Generic;
using UnityEngine;

namespace Yohash.React
{
  public abstract class Component : MonoBehaviour
  {
    private bool initialized = false;
    private Queue<Action> _queue = new Queue<Action>();

    private void OnEnable()
    {
      Store.Instance.Subscribe(UpdateComponent, InitializeComponent);
    }

    private void OnDisable()
    {
      Store.Instance.Unsubscribe(UpdateComponent);
    }

    protected virtual void Start()
    {
      initialized = true;

      while (_queue.Count > 0) {
        Action waiting = _queue.Dequeue();
        dispatch(waiting);
      }
    }

    protected void dispatch(Action action)
    {
      if (!initialized) {
        _queue.Enqueue(action);
        return;
      }
      Store.Instance.Dispatch(action);
    }

    public abstract void InitializeComponent(State state);
    public abstract void UpdateComponent(State oldState, State newState);
  }

  public abstract class Behaviour : Component
  {

  }
}

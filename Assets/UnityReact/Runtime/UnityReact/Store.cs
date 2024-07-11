using System.Collections.Generic;
using UnityEngine;

namespace Yohash.React
{
  /// <summary>
  /// The Store will hold all state and middlewares, and is
  /// subscribed by all components. Components dispatch to the store
  /// and receive state updates.
  /// The Store is a singleton. Multiple stores are currently
  /// not supported.
  /// </summary>
  [System.Serializable]
  public class Store
  {
    public static Store Instance {
      get {
        return _instance;
      }
    }
    private static Store _instance;

    public bool Log = false;

    [SerializeField] private State state;

    private List<Middleware> middleware;

    public delegate void UpdateDelegate(State oldState, State newState);
    public UpdateDelegate OnStoreUpdate;

    // a public flag and local vars for queueing actions
    public bool ActionQueueing = true;
    private bool processing = false;
    private Queue<IAction> actionQueue = new Queue<IAction>();

    public Store(State state, List<Middleware> middlewares)
    {
      _instance = this;
      this.state = state;

      middleware = new List<Middleware>();
      for (int i = 0; i < middlewares.Count; i++) {
        middleware.Add(middlewares[i]);
      }
    }

    public void Subscribe(UpdateDelegate update, System.Action<State> initialize)
    {
      OnStoreUpdate += update;
      initialize(state);
    }

    public void Unsubscribe(UpdateDelegate update)
    {
      OnStoreUpdate -= update;
    }

    public void Dispatch(IAction action)
    {
      if (ActionQueueing && processing) {
        actionQueue.Enqueue(action);
        return;
      }

      if (Log) { Debug.Log(action.ToString()); }

      processing = true;

      // process action through middleware
      foreach (var middleware in middleware) {
        action = middleware.HandleAction(state, action, Dispatch);
        if (action == null) { break; }
      }

      // copy old state and reduce
      var oldState = state.Clone();
      state.Reduce(action);

      // update subscribed components
      OnStoreUpdate?.Invoke(oldState, state);

      processing = false;
      if (actionQueue.Count > 0) {
        Dispatch(actionQueue.Dequeue());
      }
    }
  }
}

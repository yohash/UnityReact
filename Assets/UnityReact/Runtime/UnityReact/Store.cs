using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    private static Store _instance;
    private static readonly TaskCompletionSource<Store> _initializationSource
      = new TaskCompletionSource<Store>();
    public static Task<Store> Instance => _initializationSource.Task;

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
      InitializeAsync(state, middlewares);
    }

    private async void InitializeAsync(State state, List<Middleware> middlewares)
    {
      try {
        // create the instance and set the result on our task completion source
        // so that any instance of `Component` that were created before this can
        // await the task and the associated singleton
        _instance = this;

        // perform actual initialization of the store
        this.state = state;
        middleware = new List<Middleware>();
        for (int i = 0; i < middlewares.Count; i++) {
          middleware.Add(middlewares[i]);
        }

        // finally, after full store initialization, set the result
        _initializationSource.TrySetResult(_instance);
      } catch (Exception ex) {
        _initializationSource.TrySetException(ex);
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
      if (Log && action is not IDebugAction) {
        action = action.AsDebug();
      }

      if (ActionQueueing && processing) {
        actionQueue.Enqueue(action);
        return;
      }

      if (Log) {
        var debug = (DebugAction)action;
        Debug.Log(debug.ToDetailedString());
        action = debug.Action;
      }

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

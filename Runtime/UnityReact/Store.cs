using System;
using System.Collections.Generic;
using UnityEngine;

namespace Yohash.React
{
  [System.Serializable]
  public class Store
  {
    public static Store Instance {
      get {
        return _instance;
      }
    }
    private static Store _instance;

    [SerializeField] private State state;

    //private List<Reducer> reducers;
    private List<Middleware> middleware;

    public delegate void UpdateDelegate(State oldState, State newState);
    public UpdateDelegate OnStoreUpdate;

    // a public flag and local vars for queueing actions
    public bool Queueing = true;
    private bool processing = false;
    private Queue<Action> actionQueue;

    public Store(
      List<StateContainer> containers,
      //List<Reducer> reducers,
      List<Middleware> middlewares
    )
    {
      _instance = this;

      state = new State();
      for (int i = 0; i < containers.Count; i++) {
        state.AddState(containers[i]);
      }

      //this.reducers = new List<Reducer>();
      //for (int i = 0; i < reducers.Count; i++) {
      //  this.reducers.Add(reducers[i]);
      //}

      middleware = new List<Middleware>();
      for (int i = 0; i < middlewares.Count; i++) {
        middleware.Add(middlewares[i]);
      }

      actionQueue = new Queue<Action>();
    }

    public void Subscribe(UpdateDelegate update, Action<State> initialize)
    {
      OnStoreUpdate += update;
      initialize(state);
    }

    public void Unsubscribe(UpdateDelegate update)
    {
      OnStoreUpdate -= update;
    }

    public void Dispatch(Action action)
    {
      if (processing) {
        actionQueue.Enqueue(action);
        return;
      }

      processing = true;

      // process action through middleware
      foreach (var middleware in middleware) {
        action = middleware.HandleAction(state, action, Dispatch);
        if (action == null) { break; }
      }

      // copy old state and reduce
      var oldState = state.Copy;
      state.Reduce(action);

      // update subscribed components
      OnStoreUpdate(oldState, state);

      processing = false;
      if (actionQueue.Count > 0) {
        Dispatch(actionQueue.Dequeue());
      }
    }
  }
}

﻿using System;
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
    public int Subscribed = 0;
    private bool processing = false;
    private Queue<Action> actionQueue;

    public Store(
      List<StateContainer> containers,
      List<Middleware> middlewares
    )
    {
      _instance = this;

      state = new State();
      for (int i = 0; i < containers.Count; i++) {
        state.AddState(containers[i]);
      }

      middleware = new List<Middleware>();
      for (int i = 0; i < middlewares.Count; i++) {
        middleware.Add(middlewares[i]);
      }

      actionQueue = new Queue<Action>();
    }

    public void Subscribe(UpdateDelegate update, Action<State> initialize)
    {
      OnStoreUpdate += update;
      Subscribed++;

      initialize(state);
    }

    public void Unsubscribe(UpdateDelegate update)
    {
      OnStoreUpdate -= update;
      Subscribed--;
    }

    public void Dispatch(Action action)
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
using System.Collections.Generic;
using UnityEngine;
using Yohash.React;

public class BasicUiStore : MonoBehaviour
{
  [SerializeField] private Store store;


  void Start()
  {
    var state = new List<StateContainer>() {
      new MenuState(),
      new SubmenuState()
    };
    var middleware = new List<Middleware>() {
      new SubmenuMiddleware()
    };

    store = new Store(state, middleware);
    store.Log = true;
  }
}

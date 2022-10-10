using System.Collections.Generic;
using UnityEngine;
using Yohash.React;

public class MenuStore : MonoBehaviour
{
  [SerializeField] private Store store;

  void Start()
  {
    var state = new List<StateContainer>() {
      new MenuState()
    };
    var middleware = new List<Middleware>() {

    };

    store = new Store(state, middleware);
  }
}

public class MenuState : StateContainer
{
  public enum WhichButton { A, B, C }
  public WhichButton CurrentButton;

  public float SliderValue;

  public override StateContainer Reduce(Action action)
  {
    var newState = (MenuState)Copy;

    switch (action) {
      case SliderAction sa:
        newState.SliderValue = sa.Value;
        break;
      case PressButtonAction pba:
        newState.CurrentButton = (WhichButton)pba.Index;
        break;
    }

    return newState;
  }
}
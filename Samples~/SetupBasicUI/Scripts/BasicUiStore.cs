using System.Collections.Generic;
using UnityEngine;

namespace Yohash.React.Samples.BasicUi
{
  public class BasicUiStore : MonoBehaviour
  {
    [SerializeField] private Store store;


    private void Awake()
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

}
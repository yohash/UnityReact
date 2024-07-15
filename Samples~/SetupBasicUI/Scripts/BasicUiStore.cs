using System.Collections.Generic;
using UnityEngine;

namespace Yohash.React.Samples.BasicUi
{
  public class BasicUiStore : MonoBehaviour
  {
    [SerializeField] private Store store;


    private void Awake()
    {
      var middleware = new List<Middleware>() {
        new SubmenuMiddleware()
      };

      store = new Store(new BasicUiState(), middleware);
      store.Log = true;
    }
  }
}

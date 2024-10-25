using UnityEngine;

namespace Yohash.React
{
  public interface IComponent
  {
    Transform Transform { get; }
    UnityEngine.Object Object { get; }
    void UpdateElementWithProps(PropsContainer props, State state);
    void Unmount();
  }
}


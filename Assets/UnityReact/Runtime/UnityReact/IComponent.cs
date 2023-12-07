using UnityEngine;

namespace Yohash.React
{
  public interface IComponent
  {
    Transform Transform { get; }
    UnityEngine.Object Object { get; }
    void UpdateElementWithProps(PropsContainer props);
    void Unmount();
  }
}

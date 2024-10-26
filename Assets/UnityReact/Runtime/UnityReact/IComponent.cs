using UnityEngine;

namespace Yohash.React
{
  public interface IComponent
  {
    Transform Transform { get; }
    UnityEngine.Object Object { get; }
    void InitializeElement(PropsContainer props, State state);
    void UpdateElementWithProps(PropsContainer props, State state);
    void Unmount();
  }
}

using UnityEngine;

namespace Yohash.React
{
  public interface IComponent
  {
    Transform Transform { get; }
    UnityEngine.Object Object { get; }
    void InitializeElement(PropsContainer props, State state);
    void UpdateElement(PropsContainer props, State state);
    void Unmount();
  }
}

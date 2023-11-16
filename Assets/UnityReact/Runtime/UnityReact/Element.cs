using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Yohash.React
{
  public class Element
  {
    // provide a unique address
    public string Address;
    public Transform Parent;
    public IComponent Component;
    public Func<Task<IComponent>> Mount;

    public Element(Func<Task<IComponent>> mounter, string address, Transform parent)
    {
      Mount = mounter;
      Address = address;
      Parent = parent;
    }
  }

  public interface IComponent
  {
    Transform Transform { get; }
  }
}


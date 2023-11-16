using System.Threading.Tasks;
using UnityEngine;

namespace Yohash.React
{
  public class Element
  {
    public string Address;
    public Transform Parent;
    public IComponent Component;
    public Task<IComponent> Mount;

    public Element(Task<IComponent> mounter, string address, Transform parent)
    {
      Mount = mounter;
      Address = address;
      Parent = parent;
    }
  }

  //public class Element<T> : Element
  //  where T : IComponent
  //{


  //  public Element(Task<IComponent> mounter, string address, Transform parent)
  //    : base(mounter, address, parent)
  //  {

  //  }

  //  public Task<T> Mount()
  //  {
  //    return await mount();
  //  }

  //  public abstract Task<T> mount();
  //}

  public interface IComponent
  {
    Transform Transform { get; }
  }
}


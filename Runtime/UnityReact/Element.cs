using System;
using System.Threading.Tasks;

namespace Yohash.React
{
  public class Element
  {
    // provide a unique address
    public string Address;
    private Func<Task<IComponent>> _mount;
    public Task<IComponent> Mount()
    {
      return _mount();
    }

    private Func<IComponent, Task> _unmount;
    public Task Unmount()
    {
      // first, call the internal unmount function
      // to disconnect Store internals
      Component.Unmount();
      // then, call the external unmount function
      // provided by the user
      return _unmount(Component);
    }

    // these custom props are optional
    public PropsContainer Props;

    // the component reference after it's mounted
    public IComponent Component;

    public Element(
      Func<Task<IComponent>> mounter,
      Func<IComponent, Task> unmounter,
      string address,
      PropsContainer props
    )
    {
      _mount = mounter;
      _unmount = unmounter;
      Address = address;
      Props = props;
    }
  }
}

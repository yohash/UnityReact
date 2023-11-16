using System.Collections.Generic;

namespace Yohash.React
{
  public class NoProps : Props
  {
    public override List<StateContainer> state => _state;
    private List<StateContainer> _state = new List<StateContainer>();
    public override void SetState(List<StateContainer> containers) { }
  }

  public abstract class Props
  {
    public abstract List<StateContainer> state { get; }
    public abstract void SetState(List<StateContainer> containers);
    public Props Copy { get { return MemberwiseClone() as Props; } }
  };
}

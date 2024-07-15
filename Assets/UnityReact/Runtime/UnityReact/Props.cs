namespace Yohash.React
{
  public class NoProps : Props { }

  public abstract class Props
  {
    public Props Copy { get { return MemberwiseClone() as Props; } }
    // these methods are defined by code generation
    public virtual void BuildProps(State state) { }
    public virtual bool DidUpdate() { return false; }
    public virtual void BuildElement(PropsContainer propsContainer) { }
  };
}

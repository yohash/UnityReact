namespace Yohash.React
{
  public class NoProps : Props { }

  public abstract class Props
  {
    public virtual Props Clone() { return MemberwiseClone() as Props; }
    // these methods are defined by code generation
    public virtual void BuildProps(State state) { }
    public virtual bool DidUpdate(State state) { return false; }
    public virtual void BuildElement(PropsContainer propsContainer) { }
  };
}

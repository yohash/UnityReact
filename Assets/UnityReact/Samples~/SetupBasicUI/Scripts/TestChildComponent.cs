using System.Collections.Generic;
using Yohash.React;

public class TestChildComponent : Yohash.React.Component<NoProps>
{
  public override NoProps props => _props;
  private NoProps _props = new NoProps() { };

  public override void InitializeComponent() { }
  public override IEnumerable<Element> UpdateComponent()
  {
    return new List<Element>();
  }
}


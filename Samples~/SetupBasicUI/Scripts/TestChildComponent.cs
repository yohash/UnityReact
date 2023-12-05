using System.Collections.Generic;

namespace Yohash.React.Samples.BasicUi
{
  public class TestChildComponent : Component<NoProps>
  {
    public override NoProps props => _props;
    private NoProps _props = new NoProps() { };

    public override void InitializeComponent() { }
    public override IEnumerable<Element> UpdateComponent()
    {
      return new List<Element>();
    }
  }
}

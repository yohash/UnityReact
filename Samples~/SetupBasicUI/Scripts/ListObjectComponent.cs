using System.Collections.Generic;
using UnityEngine.UI;
using Yohash.React;


public class ListProps : Props
{
  public MenuState Menu = new MenuState();

  public override List<StateContainer> state =>
    new List<StateContainer>() {
      Menu
    };

  public override void SetState(List<StateContainer> containers)
  {
    foreach (var container in containers) {
      if (container is MenuState) {
        Menu = container.Copy as MenuState;
      }
    }
  }
}

public class ListObjectComponent : Yohash.React.Component<ListProps>
{
  private ListProps _props = new ListProps();
  public override ListProps props => _props;

  public int Index;

  public Button Increment;
  public Button Decrement;

  public override void InitializeComponent()
  {
    Index = transform.GetSiblingIndex();
    Increment.onClick.AddListener(() => dispatch(new ListUpdateObject() {
      Index = Index,
      ValueBy = 1
    }));
    Decrement.onClick.AddListener(() => dispatch(new ListUpdateObject() {
      Index = Index,
      ValueBy = -1
    }));
  }

  public override IEnumerable<Element> UpdateComponent()
  {
    return new List<Element>();
  }
}

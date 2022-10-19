using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yohash.React;

public class SubMenuProps : Props
{
  public SubmenuState Submenu = new SubmenuState();

  public override List<StateContainer> state =>
    new List<StateContainer>() {
      Submenu
    };

  public override void SetState(List<StateContainer> containers)
  {
    foreach (var container in containers) {
      if (container is SubmenuState) {
        Submenu = (SubmenuState)container.Copy;
      }
    }
  }
}

public class SubmenuComponent : Yohash.React.Component<SubMenuProps>
{
  public GameObject Submenu;

  public Button ChangeColorButton;
  public Button Close;

  public Image Background;

  public override SubMenuProps props => _props;
  private SubMenuProps _props = new SubMenuProps() { };

  public override void InitializeComponent()
  {
    ChangeColorButton.onClick.AddListener(() => dispatch(new CycleSubmenuColorAction()));
    Close.onClick.AddListener(() => dispatch(new CloseSubmenuAction()));

    Submenu.SetActive(props.Submenu.IsOpen);
  }

  public override void UpdateComponent()
  {
    Debug.LogWarning("Submenu update");
    Submenu.SetActive(props.Submenu.IsOpen);

    if (!oldProps.Submenu.SubmenuColor.Equals(props.Submenu.SubmenuColor)) {
      Background.color = props.Submenu.SubmenuColor;
    }
  }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Yohash.React.Samples
{
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
          Submenu = container.Copy as SubmenuState;
        }
      }
    }
  }

  public class SubmenuComponent : Yohash.React.Component<SubMenuProps>
  {
    public GameObject Submenu;

    public Button ChangeColorButton;
    public Button Close;
    public Button Reset;
    public Button ShowSlider;

    public Text Subtext;

    public Image Background;

    public override SubMenuProps props => _props;
    private SubMenuProps _props = new SubMenuProps() { };

    public override void InitializeComponent()
    {
      ChangeColorButton.onClick.AddListener(() => dispatch(new CycleSubmenuColorAction()));
      Close.onClick.AddListener(() => dispatch(new CloseSubmenuAction()));
      Reset.onClick.AddListener(() => dispatch(new ResetButtonPressedAction()));
      ShowSlider.onClick.AddListener(() => dispatch(new SubMenuShowSliderValueAction()));

      Submenu.SetActive(props.Submenu.IsOpen);
    }

    public override IEnumerable<Element> UpdateComponent()
    {
      Submenu.SetActive(props.Submenu.IsOpen);

      Subtext.text = props.Submenu.Subtext;

      if (!oldProps.Submenu.SubmenuColor.Equals(props.Submenu.SubmenuColor)) {
        Background.color = props.Submenu.SubmenuColor;
      }

      return new List<Element>();
    }
  }
}

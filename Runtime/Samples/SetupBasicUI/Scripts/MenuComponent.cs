using UnityEngine.UI;
using System.Collections.Generic;
using Yohash.React;

public class MenuProps : Props
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
        Menu = (MenuState)container.Copy;
      }
    }
  }
}

public class MenuComponent : Yohash.React.Component<MenuProps>
{
  public Text WhichButtonPressed;
  public Text SliderValue;

  public Button[] WhichButtons;

  public Button Lock;
  public Button OpenSubmenu;

  public Slider ValueSlider;

  public override MenuProps props => _props;
  private MenuProps _props = new MenuProps() { };

  public override void InitializeComponent()
  {
    OpenSubmenu.onClick.AddListener(() => dispatch(new OpenSubmenuAction()));
    Lock.onClick.AddListener(() => dispatch(new MenuLockAction()));

    ValueSlider.onValueChanged.AddListener(
      value => dispatch(new SliderAction() { Value = value })
    );

    for (int i = 0; i < WhichButtons.Length; i++) {
      var action = new PressButtonAction() { Index = i };
      WhichButtons[i].onClick.AddListener(() => dispatch(action));
    }

    updateView();
  }

  public override void UpdateComponent()
  {
    UnityEngine.Debug.LogWarning("Menu update");
    updateView();
  }

  private void updateView()
  {
    WhichButtonPressed.text = props.Menu.CurrentButton.ToString();
    SliderValue.text = props.Menu.SliderValue.ToString();

    OpenSubmenu.interactable = !props.Menu.Locked;
    for (int i = 0; i < WhichButtons.Length; i++) {
      var action = new PressButtonAction() { Index = i };
      WhichButtons[i].interactable = !props.Menu.Locked;
    }
    ValueSlider.interactable = !props.Menu.Locked;
  }
}

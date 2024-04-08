using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Yohash.React.Samples.BasicUi
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
    public Toggle PsychedlicToggle;

    public Text Subtext;

    public Image Background;

    public override void InitializeComponent()
    {
      ChangeColorButton.onClick.AddListener(() => dispatch(new CycleSubmenuColorAction()));
      Close.onClick.AddListener(() => dispatch(new CloseSubmenuAction()));
      Reset.onClick.AddListener(() => dispatch(new ResetButtonPressedAction()));
      ShowSlider.onClick.AddListener(() => dispatch(new SubMenuShowSliderValueAction()));
      // pre-set the toggle before we setup the event listener
      PsychedlicToggle.isOn = props.Submenu.Psychedlic;
      PsychedlicToggle.onValueChanged.AddListener((value) => dispatch(new SetPsychedliaAction() { Value = value }));
      Submenu.SetActive(props.Submenu.IsOpen);
    }

    public override IEnumerable<Element> UpdateComponent()
    {
      Submenu.SetActive(props.Submenu.IsOpen);

      Subtext.text = props.Submenu.Subtext;

      if (!oldProps.Submenu.SubmenuColor.Equals(props.Submenu.SubmenuColor)) {
        Background.color = props.Submenu.SubmenuColor;
      }

      if (props.Submenu.Psychedlic) {
        return new List<Element>() {
          new Element(
            () => AttachComponent<PsychedeliaComponent>(Submenu.transform),
            c => DestroyComponent(c),
            "PsychedlicComponent",
            new PsychedlicState() {
              CycleLength = 1.2f,
              OnColorChange = (color) => Background.color = color
            }
          )
        };
      }

      return new List<Element>();
    }

    public static async Task<IComponent> AttachComponent<C>(Transform parent)
      where C : MonoBehaviour, IComponent
    {
      var component = parent.gameObject.AddComponent<C>();
      return component.GetComponent<IComponent>();
    }

    public static async Task DestroyComponent(IComponent component)
    {
      MonoBehaviour.Destroy(component.Object);
    }
  }
}

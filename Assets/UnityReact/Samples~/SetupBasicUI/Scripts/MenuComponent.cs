using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Yohash.React.Samples.BasicUi
{
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
          Menu = container.Copy as MenuState;
        }
      }
    }
  }

  public class MenuComponent : Yohash.React.Component<MenuProps>
  {
    [Header("Assign Prefab")]
    public GameObject ListObjectPrefab;

    [Header("Assign UI components in hierarchy")]
    public Text WhichButtonPressed;
    public Text SliderValue;

    public Button[] WhichButtons;

    public Button Lock;
    public Button OpenSubmenu;

    public RectTransform ScrollviewContent;
    public Button AddListObject;
    public Button RemoveListObject;

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

      AddListObject.onClick.AddListener(() => dispatch(new ListAddObject()));
      RemoveListObject.onClick.AddListener(() => dispatch(new ListRemoveObject()));

      updateView();
    }

    public override IEnumerable<Element> UpdateComponent()
    {
      updateView();

      return props.Menu.ListValues
        .Select((dat, index) => new Element(
            mountListItem,
            unmountListItem,
            "ListObject" + index.ToString(),
            new ListObjectProps() {
              Data = dat,
              Index = index
            }
          )
        );
    }

    /// <summary>
    /// Thought this mount method is async, we are performing
    /// a simple, sychronous prefab instantiation.
    /// A better implementation would be to load the prefab from
    /// file asynchronously, using for example, the AssetBundle API,
    /// and hooking into other asset tools, like object pools
    /// </summary>
    private async Task<IComponent> mountListItem()
    {
      var child = Instantiate(ListObjectPrefab);
      child.transform.SetParent(ScrollviewContent);
      child.transform.localPosition = Vector3.zero;
      child.transform.rotation = Quaternion.identity;
      return child.GetComponent<IComponent>();
    }

    private async Task unmountListItem(IComponent component)
    {
      Destroy(component.Transform.gameObject);
    }

    private void updateView()
    {
      WhichButtonPressed.text = props.Menu.CurrentButton.ToString();
      SliderValue.text = props.Menu.SliderValue.ToString();

      OpenSubmenu.interactable = !props.Menu.Locked;
      for (int i = 0; i < WhichButtons.Length; i++) {
        WhichButtons[i].interactable = !props.Menu.Locked;
      }
      ValueSlider.interactable = !props.Menu.Locked;
    }
  }
}

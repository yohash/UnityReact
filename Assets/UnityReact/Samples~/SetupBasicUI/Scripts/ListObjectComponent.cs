using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Yohash.React;

public class ListObjectProps : PropsContainer
{
  public ListObjectData Data;
  public int Index;
}

public class ListProps : Props
{
  public ListObjectProps ListObject = new ListObjectProps();

  public override List<StateContainer> state =>
    new List<StateContainer>() {
      ListObject
    };

  public override void SetState(List<StateContainer> containers)
  {
    foreach (var container in containers) {
      if (container is ListObjectProps) {
        ListObject = container.Copy as ListObjectProps;
      }
    }
  }
}

public class ListObjectComponent : Yohash.React.Component<ListProps>
{
  private ListProps _props = new ListProps();
  public override ListProps props => _props;

  public Button Increment;
  public Button Decrement;

  public Text Value;
  public Text Title;

  public Toggle AddChild;

  public Transform ChildContainer;
  public GameObject TestChildPrefab;

  public override void InitializeComponent()
  {
    Increment.onClick.AddListener(() => dispatch(new ListUpdateObject() {
      Index = props.ListObject.Index,
      ValueBy = 1
    }));
    Decrement.onClick.AddListener(() => dispatch(new ListUpdateObject() {
      Index = props.ListObject.Index,
      ValueBy = -1
    }));
    AddChild.onValueChanged.AddListener(value => dispatch(new ListObjectMountChild() {
      Index = props.ListObject.Index,
      Add = value
    }));
    Value.text = props.ListObject.Data.Value.ToString();
    Title.text = $"Item[{props.ListObject.Index}]";
  }

  public override IEnumerable<Element> UpdateComponent()
  {
    updateView();

    var elements = new List<Element>();
    if (props.ListObject.Data.MountChild) {
      elements.Add(new Element(
        mountTestChild,
        unmountTestChild,
        "childOf" + props.ListObject.Index,
        PropsContainer.Empty
      ));
    }

    return elements;
  }

  private async Task<IComponent> mountTestChild()
  {
    var child = Instantiate(TestChildPrefab);
    child.transform.SetParent(ChildContainer);
    child.transform.localPosition = Vector3.zero;
    child.transform.rotation = Quaternion.identity;
    var rect = child.GetComponent<RectTransform>();
    rect.anchorMax = Vector2.one;
    rect.anchorMin = Vector2.zero;
    rect.offsetMax = Vector2.zero;
    rect.offsetMin = Vector2.zero;
    return child.GetComponent<IComponent>();
  }

  private async Task unmountTestChild(IComponent component)
  {
    Destroy(component.Transform.gameObject);
  }

  private void updateView()
  {
    var old = oldProps.ListObject.Data.Value;
    var current = props.ListObject.Data.Value;
    // only update the view if the value has changed
    if (old == current) { return; }
    Value.text = props.ListObject.Data.Value.ToString();
  }
}

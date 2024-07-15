namespace Yohash.React.Samples.BasicUi
{
  public partial class ListProps : Props
  {
    public ListProps()
    {
      ListObject = new ListObjectProps();
    }

    public override bool DidUpdate()
    {
      return true;
    }

    public override void BuildElement(PropsContainer propsContainer)
    {
      ListObject = propsContainer as ListObjectProps;
    }
  }

  public partial class MenuProps : Props
  {
    public MenuProps()
    {
      Menu = new MenuState();
    }

    public override void BuildProps(State state)
    {
      var _state = state as BasicUiState;
      Menu = _state.MenuState;
    }

    public override bool DidUpdate()
    {
      return Menu.IsDirty;
    }
  }

  public partial class PsychedlicProps : Props
  {
    public PsychedlicProps()
    {
      Psychedlic = new PsychedlicState();
    }

    public override bool DidUpdate()
    {
      return true;
    }

    public override void BuildElement(PropsContainer propsContainer)
    {
      Psychedlic = propsContainer as PsychedlicState;
    }
  }

  public partial class SubMenuProps : Props
  {
    public SubMenuProps()
    {
      Submenu = new SubmenuState();
    }

    public override void BuildProps(State state)
    {
      var _state = state as BasicUiState;
      Submenu = _state.SubmenuState;
    }

    public override bool DidUpdate()
    {
      return Submenu.IsDirty;
    }
  }
}

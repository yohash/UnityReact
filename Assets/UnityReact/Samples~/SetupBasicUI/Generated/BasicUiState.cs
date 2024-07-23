using Yohash.React;
namespace Yohash.React.Samples.BasicUi
{
  public class BasicUiState : State
  {
    public MenuState MenuState = new MenuState();
    public SubmenuState SubmenuState = new SubmenuState();

    public override void Reduce(IAction action)
    {
      MenuState.Reduce(action);
      SubmenuState.Reduce(action);
    }

    public override State Clone()
    {
      var newState = new BasicUiState();
      newState.MenuState = MenuState.Clone() as MenuState;
      newState.SubmenuState = SubmenuState.Clone() as SubmenuState;
      return newState;
    }
  }
}

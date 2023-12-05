namespace Yohash.React.Samples
{
  public class SubmenuMiddleware : Middleware
  {
    public override IAction HandleAction(State state, IAction action, System.Action<IAction> dispatch)
    {
      switch (action) {
        case ResetButtonPressedAction rbpa:
          // dispatch the reset action as a "PressButtonAction"
          dispatch(new PressButtonAction() { Index = 0 });
          // return null, so the action won't process
          return null;
        // Other options:
        //  - return the new PressButtonAction()
        //  - return the ResetButtonPressedAction() and allow state to react to it

        case SubMenuShowSliderValueAction sssva:
          // this action grabs state to set the subtext value
          if (state.TryGetState(out MenuState menu)) {
            dispatch(new SetSubTextAction() { Text = menu.SliderValue.ToString() });
          }
          break;
      }

      return action;
    }
  }
}

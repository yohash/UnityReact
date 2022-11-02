using Yohash.React;

public class SubmenuMiddleware : Middleware
{
  public override Action HandleAction(State state, Action action, System.Action<Action> dispatch)
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
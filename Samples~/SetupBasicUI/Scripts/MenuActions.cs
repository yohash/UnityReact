using Yohash.React;

public struct PressButtonAction : IAction
{
  public int Index;
}

public struct SliderAction : IAction
{
  public float Value;
}

public struct SetSubTextAction : IAction
{
  public string Text;
}

public struct SubMenuShowSliderValueAction : IAction { }

public struct ResetButtonPressedAction : IAction { }
public struct OpenSubmenuAction : IAction { }
public struct CloseSubmenuAction : IAction { }
public struct CycleSubmenuColorAction : IAction { }
public struct MenuLockAction : IAction { }

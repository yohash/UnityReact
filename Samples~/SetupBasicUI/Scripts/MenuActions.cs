using Yohash.React;

public class PressButtonAction : IAction
{
  public int Index;
}

public class SliderAction : IAction
{
  public float Value;
}

public class SetSubTextAction : IAction
{
  public string Text;
}

public class SubMenuShowSliderValueAction : IAction { }

public class ResetButtonPressedAction : IAction { }
public class OpenSubmenuAction : IAction { }
public class CloseSubmenuAction : IAction { }
public class CycleSubmenuColorAction : IAction { }
public class MenuLockAction : IAction { }

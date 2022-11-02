using Yohash.React;

public class PressButtonAction : Action
{
  public int Index;
}

public class SliderAction : Action
{
  public float Value;
}

public class SetSubTextAction : Action
{
  public string Text;
}

public class SubMenuShowSliderValueAction : Action { }

public class ResetButtonPressedAction : Action { }
public class OpenSubmenuAction : Action { }
public class CloseSubmenuAction : Action { }
public class CycleSubmenuColorAction : Action { }
public class MenuLockAction : Action { }

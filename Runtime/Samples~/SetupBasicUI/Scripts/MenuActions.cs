using Yohash.React;

public class PressButtonAction : Action
{
  public int Index;
  public override string ToString()
  {
    return GetType() + ": " + Index;
  }
}

public class SliderAction : Action
{
  public float Value;
  public override string ToString()
  {
    return GetType() + ": " + Value;
  }
}

public class OpenSubmenuAction : Action { }
public class CloseSubmenuAction : Action { }
public class CycleSubmenuColorAction : Action { }
public class MenuLockAction : Action { }

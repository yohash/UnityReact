using Yohash.React;

public class MenuState : StateContainer
{
  public enum WhichButton { A, B, C }
  public WhichButton CurrentButton;

  public float SliderValue;

  public bool Locked = false;

  protected override bool reduce(Action action)
  {
    switch (action) {
      case SliderAction sa: {
          SliderValue = sa.Value;
          return true;
        }
      case PressButtonAction pba: {
          CurrentButton = (WhichButton)pba.Index;
          return true;
        }
      case MenuLockAction _: {
          Locked = !Locked;
          return true;
        }
    }
    return false;
  }
}

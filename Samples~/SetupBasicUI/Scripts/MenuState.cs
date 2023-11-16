using System;
using Yohash.React;

public class MenuState : StateContainer
{
  public enum WhichButton { A, B, C }
  public WhichButton CurrentButton;

  public float SliderValue;

  public bool Locked = false;

  public int[] ListValues = new int[0];

  protected override bool reduce(IAction action)
  {
    switch (action) {
      case ListAddObject _: {
          Array.Resize(ref ListValues, ListValues.Length + 1);
          return true;
        }
      case ListRemoveObject _: {
          Array.Resize(ref ListValues, ListValues.Length - 1);
          return true;
        }
      case ListUpdateObject luo: {
          ListValues[luo.Index] += luo.ValueBy;
          return true;
        }

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

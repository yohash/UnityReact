using System;

namespace Yohash.React.Samples.BasicUi
{
  public struct ListObjectData
  {
    public int Value;
    public bool MountChild;
  }

  public class MenuState : StateContainer
  {
    public enum WhichButton { A, B, C }
    public WhichButton CurrentButton;

    public float SliderValue;

    public bool Locked = false;

    public int lockCount = 0;

    public ListObjectData[] ListValues = new ListObjectData[0];

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
            ListValues[luo.Index].Value += luo.ValueBy;
            return true;
          }
        case ListObjectMountChild child: {
            ListValues[child.Index].MountChild = child.Add;
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
            lockCount++;
            return true;
          }
      }
      return false;
    }
  }
}

using UnityEngine;

namespace Yohash.React.Samples.BasicUi
{
  public class SubmenuState : StateContainer
  {
    public Color32 SubmenuColor;

    public bool IsOpen;
    public bool Psychedlic;

    public string Subtext;

    protected override bool reduce(IAction action)
    {
      switch (action) {
        case OpenSubmenuAction _: {
            IsOpen = true;
            return true;
          }
        case CloseSubmenuAction _: {
            IsOpen = false;
            return true;
          }
        case CycleSubmenuColorAction _: {
            SubmenuColor = new Color32(
              (byte)Random.Range(0, 255),
              (byte)Random.Range(0, 255),
              (byte)Random.Range(0, 255),
              255
            );
            return true;
          }
        case SetPsychedliaAction spsa: {
            Psychedlic = spsa.Value;
            return true;
          }
        case MenuLockAction _: {
            // close the submenu if we lock the interface
            IsOpen = false;
            return true;
          }
        case SetSubTextAction ssta: {
            Subtext = ssta.Text;
            return true;
          }
      }
      return false;
    }
  }
}

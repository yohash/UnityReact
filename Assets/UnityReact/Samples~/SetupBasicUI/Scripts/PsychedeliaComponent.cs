using System;
using System.Collections.Generic;
using UnityEngine;

namespace Yohash.React.Samples.BasicUi
{
  public class PsychedlicState : PropsContainer
  {
    public float CycleLength = 1.5f;
    public Action<Color> OnColorChange;
  }

  public partial class PsychedlicProps : Props
  {
    public PsychedlicState Psychedlic;
  }

  public class PsychedeliaComponent : Yohash.React.Component<PsychedlicProps>
  {
    private Color fromColor = Color.black;
    private Color toColor = Color.white;
    private float timer;

    public override void InitializeComponent() { }

    public override IEnumerable<Element> UpdateComponent()
    {
      return new List<Element>();
    }

    private void Update()
    {
      var color = Color.Lerp(fromColor, toColor, timer / props.Psychedlic.CycleLength);
      props.Psychedlic.OnColorChange?.Invoke(color);
      if (timer > props.Psychedlic.CycleLength) {
        timer = 0;
        fromColor = toColor;
        toColor = new Color(
          UnityEngine.Random.Range(0f, 1f),
          UnityEngine.Random.Range(0f, 1f),
          UnityEngine.Random.Range(0f, 1f)
        );
      }
      timer += Time.deltaTime;
    }
  }
}

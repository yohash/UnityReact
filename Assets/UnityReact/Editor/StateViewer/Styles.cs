using System.Linq;
using UnityEngine;
using System;

namespace Yohash.React.Editor
{
  public static class Styles
  {
    // ********************************************************************
    // These public handles are used by the StateViewerWindow
    // ********************************************************************
    public static Lazy<GUIStyle> OpenBox = new Lazy<GUIStyle>(() =>
      new GUIStyle() {
        fontSize = 12,
        normal = openBox.Value,
        stretchWidth = true,
        alignment = TextAnchor.MiddleLeft,
        wordWrap = true,
        richText = true,
        padding = new RectOffset(8, 8, 8, 8),
        margin = new RectOffset(8, 8, 0, 8)
      }
    );
    public static Lazy<GUIStyle> OpenBox_Contents = new Lazy<GUIStyle>(() =>
      new GUIStyle() {
        fontSize = 12,
        normal = openBox.Value,
        stretchWidth = true,
        alignment = TextAnchor.MiddleLeft,
        wordWrap = true,
        richText = true,
        padding = new RectOffset(8, 8, 4, 4),
        margin = new RectOffset(4, 4, 4, 4)
      }
    );
    public static Lazy<GUIStyle> OpenHeader_Container = new Lazy<GUIStyle>(() =>
      new GUIStyle() {
        fontSize = 12,
        normal = openHeader_Container.Value,
        onHover = openHoverHeader_Container.Value,
        stretchWidth = true,
        padding = new RectOffset(8, 8, 4, 4),
        margin = new RectOffset(4, 4, 4, 4)
      }
    );
    public static Lazy<GUIStyle> OpenHeader = new Lazy<GUIStyle>(() =>
      new GUIStyle() {
        fontSize = 14,
        fontStyle = FontStyle.Bold,
        normal = openHeader.Value,
        onHover = openHoverHeader.Value,
        stretchWidth = true,
        padding = new RectOffset(4, 4, 4, 4),
        margin = new RectOffset(8, 8, 8, 0)
      }
    );
    public static Lazy<GUIStyle> ClosedHeader_Container = new Lazy<GUIStyle>(() =>
      new GUIStyle() {
        fontSize = 12,
        normal = closedHeader_Container.Value,
        hover = closedHoverHeader_Container.Value,
        stretchWidth = true,
        padding = new RectOffset(8, 8, 4, 4),
        margin = new RectOffset(4, 4, 4, 4)
      }
    );
    public static Lazy<GUIStyle> ClosedHeader = new Lazy<GUIStyle>(() =>
      new GUIStyle() {
        fontSize = 14,
        normal = closedHeader.Value,
        hover = closedHoverHeader.Value,
        stretchWidth = true,
        padding = new RectOffset(4, 4, 4, 4),
        margin = new RectOffset(8, 8, 8, 8)
      }
    );
    public static Lazy<GUIStyle> ContainerBackground = new Lazy<GUIStyle>(() =>
      new GUIStyle() {
        fontSize = 12,
        normal = closedHeader_Container.Value,
        stretchWidth = true,
        padding = new RectOffset(4, 4, 4, 4),
        margin = new RectOffset(4, 4, 4, 4)
      }
    );
    public static Lazy<GUIStyle> ContainerContents = new Lazy<GUIStyle>(() =>
      new GUIStyle() {
        fontSize = 12,
        normal = closedHeader_Container.Value,
        stretchWidth = true,
        padding = new RectOffset(8, 8, 0, 0),
        margin = new RectOffset(0, 0, 0, 0)
      }
    );
    public static Lazy<GUIStyle> Index = new Lazy<GUIStyle>(() =>
      new GUIStyle() {
        fontSize = 12,
        normal = openHeader_Container.Value,
        hover = openHoverHeader_Container.Value,
        alignment = TextAnchor.MiddleCenter,
        stretchWidth = true,
        margin = new RectOffset(4, 4, 4, 4)
      }
    );

    // ********************************************************************
    // These private handles are used to create the public handles
    // ********************************************************************
    private static Lazy<GUIStyleState> openHoverHeader = new Lazy<GUIStyleState>(() =>
      new GUIStyleState {
        textColor = Color.white,
        background = openHoverTexture.Value
      }
    );
    private static Lazy<GUIStyleState> openHeader = new Lazy<GUIStyleState>(() =>
      new GUIStyleState {
        textColor = Color.white,
        background = openHeaderTexture.Value
      }
    );
    private static Lazy<GUIStyleState> closedHoverHeader = new Lazy<GUIStyleState>(() =>
      new GUIStyleState {
        textColor = Color.white,
        background = closedHoverTexture.Value
      }
    );
    private static Lazy<GUIStyleState> closedHeader = new Lazy<GUIStyleState>(() =>
      new GUIStyleState {
        textColor = Color.white,
        background = closedTexture.Value
      }
    );
    private static Lazy<GUIStyleState> openBox = new Lazy<GUIStyleState>(() =>
      new GUIStyleState {
        textColor = Color.white,
        background = openBoxTexture.Value
      }
    );
    private static Lazy<GUIStyleState> openHeader_Container = new Lazy<GUIStyleState>(() =>
      new GUIStyleState {
        textColor = Color.white,
        background = openHeader_ContainerTexture.Value
      }
    );
    private static Lazy<GUIStyleState> openHoverHeader_Container = new Lazy<GUIStyleState>(() =>
      new GUIStyleState {
        textColor = Color.white,
        background = openHoverTexture_Container.Value
      }
    );
    private static Lazy<GUIStyleState> closedHoverHeader_Container = new Lazy<GUIStyleState>(() =>
      new GUIStyleState {
        textColor = Color.white,
        background = closedHoverTexture_Container.Value
      }
    );
    private static Lazy<GUIStyleState> closedHeader_Container = new Lazy<GUIStyleState>(() =>
      new GUIStyleState {
        textColor = Color.white,
        background = closedTexture_Container.Value
      }
    );

    // ********************************************************************
    // These textures are used to create the private handles
    // ********************************************************************
    // a toggler (header) that is open/expanded
    private static Texture2D _openHover;
    private static Lazy<Texture2D> openHoverTexture = new Lazy<Texture2D>(() => {
      if (_openHover == null) {
        _openHover = new Texture2D(1, 1);
        Color[] pixels = Enumerable.Repeat(Color.black, 1).ToArray();
        _openHover.SetPixels(pixels);
        _openHover.Apply();
      }
      return _openHover;
    });
    private static Texture2D _openHeaderTexture;
    private static Lazy<Texture2D> openHeaderTexture = new Lazy<Texture2D>(() => {
      if (_openHeaderTexture == null) {
        _openHeaderTexture = new Texture2D(1, 1);
        Color[] pixels = Enumerable.Repeat(Color.grey, 1).ToArray();
        _openHeaderTexture.SetPixels(pixels);
        _openHeaderTexture.Apply();
      }
      return _openHeaderTexture;
    });

    // a toggler (header) that is closed/collapsed
    private static Texture2D _closedHover;
    private static Lazy<Texture2D> closedHoverTexture = new Lazy<Texture2D>(() => {
      if (_closedHover == null) {
        _closedHover = new Texture2D(1, 1);
        Color[] pixels = Enumerable.Repeat(new Color(.4f, .4f, .4f), 1).ToArray();
        _closedHover.SetPixels(pixels);
        _closedHover.Apply();
      }
      return _closedHover;
    });
    private static Texture2D _closed;
    private static Lazy<Texture2D> closedTexture = new Lazy<Texture2D>(() => {
      if (_closed == null) {
        _closed = new Texture2D(1, 1);
        Color[] pixels = Enumerable.Repeat(new Color(.35f, .35f, .35f), 1).ToArray();
        _closed.SetPixels(pixels);
        _closed.Apply();
      }
      return _closed;
    });

    // Backing box for the state container data
    private static Texture2D _openBoxTexture;
    private static Lazy<Texture2D> openBoxTexture = new Lazy<Texture2D>(() => {
      if (_openBoxTexture == null) {
        _openBoxTexture = new Texture2D(1, 1);
        Color[] pixels = Enumerable.Repeat(new Color(.45f, .45f, .45f), 1).ToArray();
        _openBoxTexture.SetPixels(pixels);
        _openBoxTexture.Apply();
      }
      return _openBoxTexture;
    });
    // a toggler (container) that is open/expanded
    private static Texture2D _openHover_Container;
    private static Lazy<Texture2D> openHoverTexture_Container = new Lazy<Texture2D>(() => {
      if (_openHover_Container == null) {
        _openHover_Container = new Texture2D(1, 1);
        Color[] pixels = Enumerable.Repeat(new Color(.5f, .5f, .5f), 1).ToArray();
        _openHover_Container.SetPixels(pixels);
        _openHover_Container.Apply();
      }
      return _openHover_Container;
    });
    private static Texture2D _openHeader_Container;
    private static Lazy<Texture2D> openHeader_ContainerTexture = new Lazy<Texture2D>(() => {
      if (_openHeader_Container == null) {
        _openHeader_Container = new Texture2D(1, 1);
        Color[] pixels = Enumerable.Repeat(new Color(.425f, .425f, .425f), 1).ToArray();
        _openHeader_Container.SetPixels(pixels);
        _openHeader_Container.Apply();
      }
      return _openHeader_Container;
    });

    // a toggler (container) that is closed/collapsed
    private static Texture2D _closed_ContainerHover_Container;
    private static Lazy<Texture2D> closedHoverTexture_Container = new Lazy<Texture2D>(() => {
      if (_closed_ContainerHover_Container == null) {
        _closed_ContainerHover_Container = new Texture2D(1, 1);
        Color[] pixels = Enumerable.Repeat(new Color(.4f, .4f, .4f), 1).ToArray();
        _closed_ContainerHover_Container.SetPixels(pixels);
        _closed_ContainerHover_Container.Apply();
      }
      return _closed_ContainerHover_Container;
    });
    private static Texture2D _closed_Container;
    private static Lazy<Texture2D> closedTexture_Container = new Lazy<Texture2D>(() => {
      if (_closed_Container == null) {
        _closed_Container = new Texture2D(1, 1);
        Color[] pixels = Enumerable.Repeat(new Color(.35f, .35f, .35f), 1).ToArray();
        _closed_Container.SetPixels(pixels);
        _closed_Container.Apply();
      }
      return _closed_Container;
    });
  }
}

using System.Collections.Generic;
using System.Collections;
using System;
using UnityEditor;
using UnityEngine;
using Yohash.React;
using System.Linq;
using System.Threading.Tasks;

public class ComponentViewer : EditorWindow
{
  private static ComponentViewer window;
  private static Vector2 scroll;
  private static List<Yohash.React.IComponent> components = new List<Yohash.React.IComponent>();
  private static bool showDebug = false;

  private static string[] titles;
  private static bool[] toggles;

  private Dictionary<string, managedToggle> embeddedToggles = new Dictionary<string, managedToggle>();

  private class managedToggle
  {
    public bool IsOn;
    public bool[] Contents;
    public void Resize(int size)
    {
      Array.Resize(ref Contents, size);
    }
  }

  [MenuItem("Yohash/React/Component Viewer", false, 810)]
  private static void Init()
  {
    window = (ComponentViewer)GetWindow(typeof(ComponentViewer));
    window.Show();
  }

  private void OnGUI()
  {
    scroll = EditorGUILayout.BeginScrollView(scroll);

    showDebug = GUILayout.Toggle(showDebug, "Show Debug Info");
    for (int i = 0; i < components.Count; i++) {
      toggles[i] = GUILayout.Toggle(toggles[i], titles[i], toggles[i] ? Styles.OpenHeader.Value : Styles.ClosedHeader.Value);
      if (toggles[i]) {
        drawContainer(components[i]);
      }
    }
    EditorGUILayout.EndScrollView();
  }

  private void Update()
  {
    if (Store.Instance != null
      && Store.Instance.OnStoreUpdate != null
      && !Array.Exists(Store.Instance.OnStoreUpdate.GetInvocationList(), x => x.Method.Name == "updateComponentView")
    ) {
      Store.Instance.OnStoreUpdate += updateComponentView;
    }
    if (!Application.isPlaying) {
      embeddedToggles = new Dictionary<string, managedToggle>();
    }
    Repaint();
  }


  private async void updateComponentView(State oldState, State newState)
  {
    // wait a frame to allow all components to update
    await Task.Yield();
    populateComponents();
  }

  private void populateComponents()
  {
    components = FindObjectsOfType<GameObject>()
      .Where(go => go.GetComponent<IComponent>() != null)
      .Select(go => go.GetComponent<IComponent>())
      .ToList();

    titles = new string[components.Count];
    toggles = new bool[components.Count];

    for (int i = 0; i < components.Count; i++) {
      titles[i] = components[i].GetType().Name;
    }
  }

  private void drawContainer(IComponent component)
  {

  }
}

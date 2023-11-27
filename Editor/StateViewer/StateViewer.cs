using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Yohash.React;

public partial class StateViewer : EditorWindow
{
  private static StateViewer window;
  private static Vector2 scroll;
  private static List<StateContainer> baseState = new List<StateContainer>();

  private static string[] titles;
  private static bool[] toggles;

  private static bool showDebug = false;

  private Dictionary<string, managedToggle> embeddedToggles = new Dictionary<string, managedToggle>();

  [MenuItem("Yohash/React/State Viewer", false, 800)]
  private static void Init()
  {
    window = (StateViewer)GetWindow(typeof(StateViewer));
    window.Show();
  }

  private void OnGUI()
  {
    scroll = EditorGUILayout.BeginScrollView(scroll);

    showDebug = GUILayout.Toggle(showDebug, "Show Debug Info");
    for (int i = 0; i < baseState.Count; i++) {
      toggles[i] = GUILayout.Toggle(toggles[i], titles[i], toggles[i] ? Styles.OpenHeader.Value : Styles.ClosedHeader.Value);
      if (toggles[i]) {
        drawContainer(baseState[i]);
      }
    }
    EditorGUILayout.EndScrollView();
  }

  private void Update()
  {
    if (Store.Instance != null
      && Store.Instance.OnStoreUpdate != null
      && !Array.Exists(Store.Instance.OnStoreUpdate.GetInvocationList(), x => x.Method.Name == "updateStateView")
    ) {
      Store.Instance.OnStoreUpdate += updateStateView;
    }
    if (!Application.isPlaying) {
      embeddedToggles = new Dictionary<string, managedToggle>();
    }
    Repaint();
  }

  // ******************************************************************
  //  PRIVATE METHODS
  // ******************************************************************
  private void updateStateView(State oldState, State newState)
  {
    populateState();
  }

  private void populateState()
  {
    var allStateContainers = Store.Instance.State.Containers
      .OrderBy(sc => sc.GetType().Name)
      .ToList();

    if (baseState.Count != allStateContainers.Count) {
      baseState.Clear();
      baseState.AddRange(allStateContainers);
      titles = new string[baseState.Count];
      toggles = new bool[baseState.Count];
    } else {
      baseState.Clear();
      baseState.AddRange(allStateContainers);
    }

    for (int i = 0; i < baseState.Count; i++) {
      titles[i] = baseState[i].GetType().ToString();
    }
  }

  private class managedToggle
  {
    public bool IsOn;
    //public string Key;

    public bool[] Contents;

    public void Resize(int size)
    {
      Array.Resize(ref Contents, size);
    }
  }

  private void drawContainer(StateContainer container)
  {
    EditorGUILayout.BeginVertical(Styles.OpenBox.Value);
    displayStateRecursively(container);
    EditorGUILayout.EndVertical();
  }

  private void displayStateRecursively(object parent, int nested = 0)
  {
    // don't over-render
    if (nested > 12) {
      GUILayout.Label("...");
      return;
    }

    var fields = parent.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    foreach (FieldInfo info in fields) {
      var type = info.FieldType;
      var name = info.Name;
      var value = info.GetValue(parent);
      testTypeInfoAndDrawRecursively(value, type, name, nested);
    }
  }

  private void testTypeInfoAndDrawRecursively(object value, Type type, string name, int nested = 0)
  {
    // TESTS -
    // (1) check for a primitive type using isPrimitive
    //     && check for a string using f.FieldType.Name == "String"
    //     && check for an enum by using isEnum
    //    these will all "prety print" with the same method

    // (2) if IsGenericType, check for IEnumberable using IsAssignableFrom
    //    collects lists, dicts, queues, etc.
    // (3) check for arrays using IsArray
    //    quickest way to find them and display just like IEnumberables
    //
    // (4) check for Nullable types and Tuples by using
    //      the special combination of IsGenericType and IsValueType
    //
    // *** everything remaining here is a user-defined class or struct ***
    //    (all collections that might be caught again here under IsClass are done)
    //    (all primitives that might be caught again here under IsValueType are done)
    //    (all arrays that might be caught again here under IsClass are done)
    //
    // (5) check for classes using IsClass
    // (6) check for structs using IsValueType (???)

    // store the generic type to avoid testing in conditionals
    var genericType = type.IsGenericType
      ? type.GetGenericTypeDefinition()
      : null;

    if (showDebug) { fieldInfoProperties(value, type); }

    // (1) check for "simple" displays, primitives, enums, and strings
    if (type.IsPrimitive || type.IsEnum || (type.IsClass && type.Name == "String")) {
      //object value = info.GetValue(parent);
      var printType = type.ToString().Split('.').Last().Split('`').First().Replace('+', '.');

      string typeText = $"<b>{printType}</b>: {name}";
      string valueText = $"{(value == null ? "NULL" : limit(value.ToString()))}";

      GUILayout.BeginHorizontal();
      GUILayout.Box(typeText, Styles.OpenBox_Contents.Value, GUILayout.ExpandWidth(true));
      GUILayout.Box(valueText, Styles.OpenBox_Contents.Value, GUILayout.ExpandWidth(true));
      GUILayout.EndHorizontal();
    }
    // (2) if IsGenericType, check for IEnumberable using IsAssignableFrom (lists, dicts, queues, etc.)
    else if (type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(genericType)) {
      // pretty-print the types of the enumerable and the contents
      var args = type.GetGenericArguments();

      var printType = genericType.ToString().Split('.').Last().Split('`').First().Replace('+', '.');
      var printArg = string.Join(",", args.Select(t => t.ToString().Split('.').Last())).Replace('+', '.');

      string title = $"<b>{printType}<{printArg}></b>: {name}";

      // call the helper method to render the contents
      var enumerable = (IEnumerable)value;
      var key = string.Concat(type.ToString(), name);
      //fieldInfoProperties(parent, info);
      renderIEnumerable(title, type, enumerable, key, nested);
    }
    // (3) check for arrays using IsArray
    else if (type.IsArray) {
      // pretty-print the types of the enumerable and the contents
      var printType = type.Name.ToString().Split('.').Last();
      string title = $"<b>{printType}</b>: {name}";

      // call the helper method to render the contents
      var array = (IEnumerable)value;
      var key = string.Concat(type.ToString(), name);
      //fieldInfoProperties(parent, info);
      renderIEnumerable(title, type, array, key, nested);
    }
    // (4) try to use the "Special" combo of IsGenericType and IsValueType to detect
    // Tuples and Nullable types
    else if (type.IsGenericType && type.IsValueType && type.GetGenericArguments().Count() > 0) {
      //object value = info.GetValue(parent);
      var args = type.GetGenericArguments();
      var printType = genericType == typeof(Nullable<>)
        // a nullable type
        ? $"{args[0].ToString().Split('.').Last().Replace('+', '.')}?"
        // may be a Tuple, we'll concat the args together in parentheses
        : $"({string.Join(", ", args.Select(t => t.ToString().Split('.').Last().Replace('+', '.')))})";

      string typeText = $"<b>{printType}</b>: {name}";
      // TODO - finish this formating for either of these types that has a complex
      // expandable value (like a nullable struct, for example)
      string valueText = $"{(value == null ? "NULL" : limit(value.ToString()))}";

      GUILayout.BeginHorizontal();
      GUILayout.Box(typeText, Styles.OpenBox_Contents.Value, GUILayout.ExpandWidth(true));
      GUILayout.Box(valueText, Styles.OpenBox_Contents.Value, GUILayout.ExpandWidth(true));
      GUILayout.EndHorizontal();
    }
    // (5) capture classes -- only non-primitive, non-IList, non-string classes should remain
    // (6) capture structs -- they will render the same way
    else if (type.IsClass || type.IsValueType) {
      var key = string.Concat(type.ToString(), name);
      renderStructOrClass(type, value, name, key, nested);
    }
    // (etc...) any other generic object
    else {
      //object value = info.GetValue(parent);
      var args = type.GetGenericArguments();
      var printType = type.ToString().Split('.').Last().Split('`').First().Replace('+', '.');
      string typeText = $"<b>{printType}</b>: {name}";
      string valueText = $"{(value == null ? "NULL" : limit(value.ToString()))}";
      GUILayout.BeginHorizontal();
      GUILayout.Box(typeText, Styles.OpenBox_Contents.Value, GUILayout.ExpandWidth(true));
      GUILayout.Box(valueText, Styles.OpenBox_Contents.Value, GUILayout.ExpandWidth(true));
      GUILayout.EndHorizontal();
    }
  }

  private void renderStructOrClass(Type type, object value, string name, string key, int nested)
  {
    var length = type.GetFields().Count();

    // create/maintain toggle tracker
    if (!embeddedToggles.ContainsKey(key)) {
      embeddedToggles[key] = new managedToggle() {
        IsOn = false,
        Contents = new bool[length]
      };
    }
    if (embeddedToggles[key].Contents.Length != length) {
      embeddedToggles[key].Resize(length);
    }

    // begin the vertical container that will display the expandable elements
    EditorGUILayout.BeginVertical(Styles.ContainerBackground.Value);

    var printType = type.ToString().Split('.').Last().Split('`').First().Replace('+', '.');
    var typeText = $"<b>{printType}</b>: {name}";

    embeddedToggles[key].IsOn = GUILayout.Toggle(
      embeddedToggles[key].IsOn,
      typeText,
      embeddedToggles[key].IsOn
        ? Styles.OpenHeader_Container.Value
        : Styles.ClosedHeader_Container.Value
    );

    if (embeddedToggles[key].IsOn) {
      displayStateRecursively(value, nested + 1);
    }
    EditorGUILayout.EndVertical();
  }

  private void renderIEnumerable(string title, Type type, IEnumerable enumerable, string key, int nested)
  {
    var length = count(enumerable);

    // create/maintain toggle tracker
    if (!embeddedToggles.ContainsKey(key)) {
      embeddedToggles[key] = new managedToggle() {
        IsOn = false,
        Contents = new bool[length]
      };
    }
    if (embeddedToggles[key].Contents.Length != length) {
      embeddedToggles[key].Resize(length);
    }

    // begin the vertical container that will display the expandable elements
    EditorGUILayout.BeginVertical(Styles.ContainerBackground.Value);

    embeddedToggles[key].IsOn = GUILayout.Toggle(
      embeddedToggles[key].IsOn,
      title,
      embeddedToggles[key].IsOn
        ? Styles.OpenHeader_Container.Value
        : Styles.ClosedHeader_Container.Value
    );

    if (embeddedToggles[key].IsOn) {
      try {
        if (length == 0) {
          GUILayout.Box("\tNO CONTENTS", Styles.ContainerBackground.Value);
        } else {
          var args = type.GetGenericArguments();

          // for 1-length generic arguments, we should show the contents of each line
          // this is the most common behaviour
          if (type.IsArray || args.Count() == 1) {
            int index = 0;
            foreach (var item in enumerable) {
              string print = $"<b>[{index}]</b>";
              GUILayout.BeginHorizontal(Styles.ContainerContents.Value);
              GUILayout.Box(print, Styles.Index.Value, GUILayout.Height(30), GUILayout.Width(40));

              // layer the box and contents to the right of the index, allowing us to layer containers
              // progressively lower within each other
              GUILayout.BeginVertical(Styles.OpenHeader_Container.Value, GUILayout.ExpandWidth(true));
              if (item == null) {
                GUILayout.Box("NULL", Styles.OpenHeader_Container.Value);
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                break;
              }
              var itemType = item.GetType();
              testTypeInfoAndDrawRecursively(item, itemType, $"Item[{index}]", nested + 1);
              GUILayout.EndVertical();

              GUILayout.EndHorizontal();

              index++;
            }
          }
          // for 2-length generic arguments, we should show the key/value pairs
          // these are most commonly dictionaries
          else if (args.Count() == 2) {
            int index = 0;
            foreach (var kvp in enumerable) {
              // fetch this entries relevant values
              var pair = kvp.GetType().GetProperties();
              var entryKey = pair[0].GetValue(kvp);
              var entryValue = pair[1].GetValue(kvp);

              string printIndex = $"<b>[{index}]</b>";
              GUILayout.BeginHorizontal(Styles.ContainerContents.Value);

              embeddedToggles[key].Contents[index] = GUILayout.Toggle(
                embeddedToggles[key].Contents[index],
                printIndex,
                Styles.Index.Value,
                GUILayout.Height(30),
                GUILayout.Width(40)
              );

              // use this outer vertical block to stack the key/value pair
              GUILayout.BeginVertical();

              // build the key in its own expandable box, in case it's complex
              GUILayout.BeginVertical(Styles.OpenHeader_Container.Value, GUILayout.ExpandWidth(true));
              var keyType = entryKey.GetType();
              testTypeInfoAndDrawRecursively(entryKey, keyType, $"Key[{index}]", nested + 1);
              GUILayout.EndVertical();


              if (embeddedToggles[key].Contents[index]) {
                // if toggled open
                // similarily for the value, build in the next vertical block
                GUILayout.BeginVertical(Styles.OpenHeader_Container.Value, GUILayout.ExpandWidth(true));
                var valueType = entryValue.GetType();
                testTypeInfoAndDrawRecursively(entryValue, valueType, $"Value[{index}]", nested + 1);
                GUILayout.EndVertical();
              }

              // close the outer verticale block of key/value stack
              GUILayout.EndVertical();

              // this ends the dictionary entry
              GUILayout.EndHorizontal();

              index++;
            }
          }
        }
      } catch (Exception e) {
        Debug.LogError("Exception while rendering IENumberable: " + e.Message);
        throw;
      }
    }
    EditorGUILayout.EndVertical();
  }

  private void fieldInfoProperties(object parent, Type type)
  {
    GUILayout.Label("Name: " + type.Name);
    GUILayout.Label("isGenericType: " + type.IsGenericType);
    GUILayout.Label("IsPrimitive: " + type.IsPrimitive);
    GUILayout.Label("IsArray: " + type.IsArray);
    GUILayout.Label("isClass: " + type.IsClass);
    GUILayout.Label("isValueType: " + type.IsValueType);
    GUILayout.Label("isEnum: " + type.IsEnum);
    GUILayout.Label("IsNested: " + type.IsNested);
    GUILayout.Label("f.FieldType.Name == \"String\" : " + (type.Name == "String"));
    GUILayout.Label("args: " + type.GetGenericArguments().Count());
    GUILayout.Label("Number of Fields: " + type.GetFields().Count());
    GUILayout.Label("Properties: " + string.Join("\n\t", type.GetProperties().Select(i => i.Name.ToString())));

    //GUILayout.Label("Value: " + info.GetValue(parent));
    //object value = info.GetValue(parent);
    //foreach (var field in info.FieldType.GetFields()) {
    //  object value2 = field.GetValue(value);
    //  GUILayout.Label($"\t{field} = {value2}\n");
    //}
  }

  private string limit(string s)
  {
    if (s.Length > 1000) {
      return s.Substring(0, 1000);
    }
    return s;
  }

  private int count(IEnumerable objects)
  {
    int count = 0;
    foreach (var o in objects) {
      count++;
    }
    return count;
  }
}

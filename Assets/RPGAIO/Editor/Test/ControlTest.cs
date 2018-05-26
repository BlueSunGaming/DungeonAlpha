//using System;
//using System.Collections.Generic;
//using System.Linq;
//using LogicSpawn.RPGMaker;
//using UnityEditor;
//using UnityEngine;
//
//public class ControlTest : EditorWindow
//{
//    public ControlDefinition ControlDefinition = null;
//    public List<ControlDefinition> MyDefinitions = new List<ControlDefinition>();
//    [MenuItem("Window/CONTROL")]
//    static void Init()
//    {
//        var window = EditorWindow.GetWindow(typeof(ControlTest));
//        window.minSize = new Vector2(600, 600);
//    }
//
//    public string GotShift;
//    public string LastShift;
//    private bool GotShiftSolo;
//
//    public ControlTest()
//    {
//        MyDefinitions.Add(new ControlDefinition("MoveForward", false));
//        MyDefinitions.Add(new ControlDefinition("MoveBackward", false));
//        MyDefinitions.Add(new ControlDefinition("Skill 1", false));
//        MyDefinitions.Add(new ControlDefinition("Inventory", false));
//        MyDefinitions.Add(new ControlDefinition("Begin Event", false));
//    }
//
//    void OnGUI()
//    {
//        GUI.skin = Resources.Load("RPGMakerAssets/EditorSkinRPGMaker") as GUISkin;
//
//
//        EditorGUILayout.HelpBox("Press Shift+Escape to assign shift due to bug etc",MessageType.Info);
//
//        foreach(var control in MyDefinitions)
//        {
//            GUILayout.BeginHorizontal();
//            EditorGUILayout.PrefixLabel(control.ID);
//
//            var selected = ControlDefinition == control ? " Press Key " : control.KeyString;
//
//            if (GUILayout.Button(selected, GUILayout.MaxHeight(30)))
//            {
//                if (ControlDefinition == null)
//                {
//                    SetCurrentKey(control);
//                }
//                else if (ControlDefinition == control)
//                {
//                    RestoreCurrentKey();
//                }
//            }
//
//            GUILayout.EndHorizontal();
//        }
//
//
//        #region KeyCheck
//
//        var evt = Event.current;
//
//        if (ControlDefinition != null)
//        {
//            if (evt.type == EventType.repaint)
//            {
//                if (Input.GetKey(KeyCode.LeftShift))
//                {
//                    GotShift = "Left";
//                    LastShift = "Left";
//                }
//                else if (Input.GetKey(KeyCode.RightShift))
//                {
//                    GotShift = "Right";
//                    LastShift = "Right";
//                }
//                else
//                {
//                    GotShift = "";
//                }
//            }
//
//
//
//            if (evt.type == EventType.KeyDown)
//            {
//                if (Event.current.modifiers > 0)
//                {
//
//                    GotShiftSolo = false;
//                    if ((Event.current.modifiers & EventModifiers.Shift) == EventModifiers.Shift)
//                    {
//                        ControlDefinition.HasModifier = true;
//                        ControlDefinition.ModifierKey = EventModifiers.Shift;
//                    }
//                    else if ((Event.current.modifiers & EventModifiers.Control) == EventModifiers.Control)
//                    {
//                        ControlDefinition.HasModifier = true;
//                        ControlDefinition.ModifierKey = EventModifiers.Control;
//                    }
//                    else if ((Event.current.modifiers & EventModifiers.Alt) == EventModifiers.Alt)
//                    {
//                        ControlDefinition.HasModifier = true;
//                        ControlDefinition.ModifierKey = EventModifiers.Alt;
//                    }
//                    else if ((Event.current.modifiers & EventModifiers.Command) == EventModifiers.Command)
//                    {
//                        ControlDefinition.HasModifier = true;
//                        ControlDefinition.ModifierKey = EventModifiers.Command;
//                    }
//                    else
//                    {
//                        ControlDefinition.HasModifier = false;
//                    }
//
//
//
//                }
//
//                KeyCode[] keyCode = new KeyCode[] {KeyCode.Escape, KeyCode.None};
//
//                if (keyCode.All(k => evt.keyCode != k))
//                {
//                    ControlDefinition.Key = Event.current.keyCode;
//                    GotShift = "";
//                    GotShiftSolo = false;
//                    evt.Use();
//                }
//
//                else if (evt.keyCode == KeyCode.Escape)
//                {
//                    if (Event.current.modifiers > 0)
//                    {
//                        if ((Event.current.modifiers & EventModifiers.Shift) == EventModifiers.Shift)
//                        {
//                            ControlDefinition.HasModifier = false;
//                            ControlDefinition.Key = KeyCode.LeftShift;
//                            ControlDefinition.IsShift = true;
//                            ControlDefinition = null;
//                            GotShift = "";
//                            GotShiftSolo = false;
//                            evt.Use();
//                        }
//                    }
//                    else
//                    {
//                        RestoreCurrentKey();
//                        evt.Use();
//                    }
//                }
//
//                GUI.FocusControl("");
//            }
//            else if ((string.IsNullOrEmpty(GotShift) && GotShiftSolo))
//            {
//                ControlDefinition.Key = LastShift == "Left" ? KeyCode.LeftShift : KeyCode.RightShift;
//                ControlDefinition.HasModifier = false;
//                ControlDefinition.IsShift = true;
//                ControlDefinition = null;
//
//                GotShift = "";
//                GotShiftSolo = false;
//                evt.Use();
//            }
//            else if (evt.type == EventType.KeyUp)
//            {
//
//                KeyCode[] modKeyCodes = new KeyCode[]
//                                            {
//                                                KeyCode.LeftShift, KeyCode.RightShift, KeyCode.RightControl,
//                                                KeyCode.LeftControl, KeyCode.RightAlt, KeyCode.LeftAlt, KeyCode.AltGr
//                                            };
//                if (modKeyCodes.Any(k => k == ControlDefinition.Key))
//                {
//                    ControlDefinition.HasModifier = false;
//                }
//
//                GotShift = "";
//                GotShiftSolo = false;
//                ControlDefinition = null;
//                evt.Use();
//            }
//
//            if (GotShift != "")
//            {
//                GotShiftSolo = true;
//            }
//            else
//            {
//                GotShiftSolo = false;
//            }
//        }
//
//        #endregion
//
//        
//    }
//
//    void SetCurrentKey(ControlDefinition c)
//    {
//        ControlDefinition = c;
//        c.OldKeyCode = c.Key;
//        c.OldModifierKey = c.ModifierKey;
//        c.OldHasModifier = c.HasModifier;
//
//        c.Key = KeyCode.None;
//        c.HasModifier = false;
//        c.IsShift = false;
//    }
//
//    void RestoreCurrentKey()
//    {
//        ControlDefinition.Key = ControlDefinition.OldKeyCode;
//        ControlDefinition.HasModifier = ControlDefinition.OldHasModifier;
//        ControlDefinition.ModifierKey = ControlDefinition.OldModifierKey;
//        ControlDefinition.IsShift = ControlDefinition.OldIsShift;
//        ControlDefinition = null;
//    }
//
//
//}
//

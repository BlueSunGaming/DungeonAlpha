using System.Linq;
using LogicSpawn.RPGMaker;
//using UnityEditor;
using UnityEngine;

public class InGameKeyPress : MonoBehaviour
{
    //todo: MyDefinitionB => List<Definition> from RM_RPGHANDLER.CONTROLS
    //redesign view

    public ControlDefinition ControlDefinition = null;
    public ControlDefinition MyDefinitionB = new ControlDefinition("MyControl");
    public string GotShift = "";
    public string LastShift;
    private bool GotShiftSolo = false;

    void OnGUI()
    {
        GUI.skin = Resources.Load("RPGMakerAssets/EditorSkinRPGMaker") as GUISkin;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Move Forward");

        var selectedB = ControlDefinition == MyDefinitionB ? " PRESS KEY " : MyDefinitionB.KeyString;

        if (GUILayout.Button(selectedB, GUILayout.MaxHeight(30)))
        {
            if (ControlDefinition == null)
            {
                SetCurrentKey(MyDefinitionB);
            }
            else if (ControlDefinition == MyDefinitionB)
            {
                RestoreCurrentKey();
            }
        }

        var evt = Event.current;


        if (ControlDefinition != null)
        {
            if (evt.type == EventType.Repaint)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    GotShift = "Left";
                    LastShift = "Left";
                }
                else if (Input.GetKey(KeyCode.RightShift))
                {
                    GotShift = "Right";
                    LastShift = "Right";
                }
                else
                {
                    GotShift = "";
                }
            }

            

            if (evt.type == EventType.KeyDown)
            {
                if (Event.current.modifiers > 0)
                {

                    GotShiftSolo = false;
                    if ((Event.current.modifiers & EventModifiers.Shift) == EventModifiers.Shift)
                    {
                        ControlDefinition.HasModifier = true;
                        ControlDefinition.ModifierKey = EventModifiers.Shift;
                    }
                    else if ((Event.current.modifiers & EventModifiers.Control) == EventModifiers.Control)
                    {
                        ControlDefinition.HasModifier = true;
                        ControlDefinition.ModifierKey = EventModifiers.Control;
                    }
                    else if ((Event.current.modifiers & EventModifiers.Alt) == EventModifiers.Alt)
                    {
                        ControlDefinition.HasModifier = true;
                        ControlDefinition.ModifierKey = EventModifiers.Alt;
                    }
                    else if ((Event.current.modifiers & EventModifiers.Command) == EventModifiers.Command)
                    {
                        ControlDefinition.HasModifier = true;
                        ControlDefinition.ModifierKey = EventModifiers.Command;
                    }
                    else
                    {
                        ControlDefinition.HasModifier = false;
                    }



                }

                KeyCode[] keyCode = new KeyCode[] { KeyCode.Escape, KeyCode.None };

                if (keyCode.All(k => evt.keyCode != k))
                {
                    ControlDefinition.Key = Event.current.keyCode;
                    GotShift = "";
                    GotShiftSolo = false;
                    evt.Use();
                }
                
                else if (evt.keyCode == KeyCode.Escape)
                {
                    RestoreCurrentKey();
                    evt.Use();
                }

                GUI.FocusControl("");
            }
            else if((string.IsNullOrEmpty(GotShift) && GotShiftSolo))
            {
                ControlDefinition.Key = LastShift == "Left" ? KeyCode.LeftShift : KeyCode.RightShift;
                ControlDefinition.HasModifier = false;

//                var sameKey = MyDefinitions.FirstOrDefault(c => c.HasModifier == ControlDefinition.HasModifier &&
//                                                                               c.ModifierKey == ControlDefinition.ModifierKey &&
//                                                                               c.Key == ControlDefinition.Key &&
//                                                                               c.IsShift == ControlDefinition.IsShift &&
//                                                                               c.ID != ControlDefinition.ID
//                                                                               );
//
//                if (sameKey != null)
//                {
//                    Debug.Log("Unassigned key for [" + sameKey.VisibleName + "]");
//                    sameKey.Key = KeyCode.None;
//                    sameKey.HasModifier = false;
//                    sameKey.ModifierKey = EventModifiers.CapsLock;
//                    sameKey.IsShift = false;
//                }

                ControlDefinition = null;

                GotShift = "";
                GotShiftSolo = false;
                evt.Use();
            }
            else if (evt.type == EventType.KeyUp) 
            {

                KeyCode[] modKeyCodes = new KeyCode[] { KeyCode.LeftShift, KeyCode.RightShift, KeyCode.RightControl, KeyCode.LeftControl, KeyCode.RightAlt, KeyCode.LeftAlt, KeyCode.AltGr };
                if (modKeyCodes.Any(k => k == ControlDefinition.Key))
                {
                    ControlDefinition.HasModifier = false;
                }

//                var sameKey = MyDefinitions.FirstOrDefault(c => c.HasModifier == ControlDefinition.HasModifier &&
//                                                                               c.ModifierKey == ControlDefinition.ModifierKey &&
//                                                                               c.Key == ControlDefinition.Key &&
//                                                                               c.IsShift == ControlDefinition.IsShift &&
//                                                                               c.ID != ControlDefinition.ID
//                                                                               );
//
//                if (sameKey != null)
//                {
//                    Debug.Log("Unassigned key for [" + sameKey.VisibleName + "]");
//                    sameKey.Key = KeyCode.None;
//                    sameKey.HasModifier = false;
//                    sameKey.ModifierKey = EventModifiers.CapsLock;
//                    sameKey.IsShift = false;
//                }

                GotShift = "";
                GotShiftSolo = false;
                ControlDefinition = null;
                evt.Use();
            }

            if (GotShift != "")
            {
                GotShiftSolo = true;
            }
            else
            {
                GotShiftSolo = false;
            }
        }



        GUILayout.EndHorizontal();
    }

    void SetCurrentKey(ControlDefinition c)
    {
        ControlDefinition = c;
        c.OldKeyCode = c.Key;
        c.OldModifierKey = c.ModifierKey;
        c.OldHasModifier = c.HasModifier;

        c.Key = KeyCode.None;
        c.HasModifier = false;
    }

    void RestoreCurrentKey()
    {
        ControlDefinition.Key = ControlDefinition.OldKeyCode;
        ControlDefinition.HasModifier = ControlDefinition.OldHasModifier;
        ControlDefinition.ModifierKey = ControlDefinition.OldModifierKey;
        ControlDefinition.IsShift = ControlDefinition.OldIsShift;
        ControlDefinition = null;
    }


}
using System;
using System.Linq;
using UnityEngine;

namespace LogicSpawn.RPGMaker
{
    public class ControlDefinition
    {
        public string ID;
        public string VisibleName;
        public ControlAction Action;
        public string StringParameter;
        public int IntParameter;
        public bool Enabled = true;

        public ControlType ControlType;


        public bool OldIsShift;
        public bool IsShift;
        public bool OldHasModifier;
        public bool HasModifier;
        public EventModifiers OldModifierKey;
        public EventModifiers ModifierKey;
        public KeyCode OldKeyCode;
        public KeyCode Key;
        public int OldMouseButton;
        public int MouseButton;


        public bool OldIsShiftAlt;
        public bool IsShiftAlt;
        public bool OldHasModifierAlt;
        public bool HasModifierAlt;
        public EventModifiers OldModifierKeyAlt;
        public EventModifiers ModifierKeyAlt;
        public KeyCode OldKeyCodeAlt;
        public KeyCode KeyAlt;
        public int OldMouseButtonAlt;
        public int MouseButtonAlt;

        public bool IsRequiredControl;

        public string KeyString
        {
            get
            {
                if(MouseButton != -1)
                {
                    switch(MouseButton)
                    {
                        case 0:
                            return "Left Click";
                        case 1:
                            return "Right Click";
                        case 2:
                            return "Middle Click";
                    }
                }

                var B = "";
                if (HasModifier) B += ModifierKey.ToString() + " + ";

                KeyCode[] numberCodes = new KeyCode[]
                                            {
                                                KeyCode.Alpha0, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3,
                                                KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7,
                                                KeyCode.Alpha8, KeyCode.Alpha9
                                            };
                if (numberCodes.All(n => Key != n))
                    B += (Key == KeyCode.LeftShift || Key == KeyCode.RightShift) ? "Shift" : Key.ToString();
                else
                {
                    B += Array.IndexOf(numberCodes, Key).ToString();
                }
                return B;
            }
        }

        public string AltKeyString
        {
            get
            {
                if (MouseButtonAlt != -1)
                {
                    switch (MouseButtonAlt)
                    {
                        case 0:
                            return "Left Click";
                        case 1:
                            return "Right Click";
                        case 2:
                            return "Middle Click";
                    }
                }

                var B = "";
                if (HasModifierAlt) B += ModifierKeyAlt.ToString() + " + ";

                KeyCode[] numberCodes = new KeyCode[]
                                            {
                                                KeyCode.Alpha0, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3,
                                                KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7,
                                                KeyCode.Alpha8, KeyCode.Alpha9
                                            };
                if (numberCodes.All(n => KeyAlt != n))
                    B += (KeyAlt == KeyCode.LeftShift || KeyAlt == KeyCode.RightShift) ? "Shift" : KeyAlt.ToString();
                else
                {
                    B += Array.IndexOf(numberCodes, KeyAlt).ToString();
                }
                return B;
            }
        }


        public string InputAxisName;
        public bool IsAxis;
        public bool IsPositiveAxis;
        public CustomControlAction CustomAction;

        public ControlDefinition()
        {
            ControlType = ControlType.KeysOnly;
            MouseButton = -1;
            CustomAction = CustomControlAction.Begin_Event;
            MouseButtonAlt = -1;
            ModifierKeyAlt = EventModifiers.None; 
            KeyAlt = KeyCode.None;
            HasModifierAlt = false;
            InputAxisName = "";
            IsAxis = false;
            IsPositiveAxis = false;
        }

        public ControlDefinition(string id)
        {
            ID = id;
            Action = ControlAction.Custom;
            StringParameter = "";
            IntParameter = -1;
            CustomAction = CustomControlAction.Begin_Event;

            VisibleName = id;
            HasModifier = false;
            Key = KeyCode.None;
            MouseButton = -1;

            HasModifierAlt = false;
            KeyAlt = KeyCode.None;
            MouseButtonAlt = -1;
            ModifierKeyAlt = EventModifiers.None;

            IsRequiredControl = true;
        }

        public ControlDefinition(string id, ControlAction action, KeyCode defaultKey, bool modifier = true, EventModifiers defaultModifier = EventModifiers.CapsLock)
        {
            ID = id;
            StringParameter = "";
            IntParameter = -1;
            Action = action;
            VisibleName = id.Replace("_", " ");
            HasModifier = defaultModifier != EventModifiers.CapsLock;
            ModifierKey = defaultModifier;
            Key = defaultKey;
            MouseButton = -1;
            CustomAction = CustomControlAction.Begin_Event;

            HasModifierAlt = false;
            KeyAlt = KeyCode.None;
            MouseButtonAlt = -1;
            ModifierKeyAlt = EventModifiers.None;

            IsRequiredControl = modifier;
        }

        public ControlDefinition(string id, ControlAction action, MouseButton defaultMouseButton, bool modifier = true, EventModifiers defaultModifier = EventModifiers.CapsLock)
        {
            ID = id;
            StringParameter = "";
            IntParameter = -1;
            Action = action;
            VisibleName = id.Replace("_", " ");
            HasModifier = defaultModifier != EventModifiers.CapsLock;
            ModifierKey = defaultModifier;
            Key = KeyCode.None;
            MouseButton = (int)defaultMouseButton;
            CustomAction = CustomControlAction.Begin_Event;

            HasModifierAlt = false;
            KeyAlt = KeyCode.None;
            MouseButtonAlt = -1;
            ModifierKeyAlt = EventModifiers.None;

            IsRequiredControl = modifier;
        }
    }

    public enum MouseButton : int
    {
        Left = 0,
        Right = 1,
        Middle = 2
    }

    public enum ControlType
    {
        KeysOnly,
        MouseOnly,
        Any
    }
    public enum CustomControlAction
    {
        Begin_Event,
        Custom
    }

    public enum ControlAction
    {
        Move_Forward,
        Move_Backward,
        Rotate_Left,
        Rotate_Right,
        Strafe_Left,
        Strafe_Right,
        Jump,

        Hide_Ui,
        Inventory,
        Character_Sheet,
        Quest_Book,
        Skill_Book,
        Crafting,
        Achievement,
        Talents,

        Use_Skill,
        Begin_Event,

        Save_Game,
        Load_Game,

        Take_Screenshot,

        Custom,
        WorldMap,
        InteractKey,
        Rotate_Up,
        Rotate_Down
    }
}

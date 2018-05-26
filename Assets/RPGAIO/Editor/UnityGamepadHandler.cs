using LogicSpawn.RPGMaker.API;
using UnityEditor;

namespace LogicSpawn.RPGMaker.Editor
{
    public class UnityGamepadHandler
    {
        public static void AddGamepadControls()
        {

/*                          new ControlDefinition("Select_Target",ControlAction.Custom, MouseButton.Left),
                            new ControlDefinition("Confirm_Cast",ControlAction.Custom, MouseButton.Left),
                            new ControlDefinition("Attack",ControlAction.Custom, MouseButton.Right),
                            new ControlDefinition("Interact",ControlAction.Custom, MouseButton.Right)
    *
                                new ControlDefinition("Move_Forward",ControlAction.Move_Forward, KeyCode.W),
                                new ControlDefinition("Move_Backward",ControlAction.Move_Backward, KeyCode.S),
                                new ControlDefinition("Rotate_Left",ControlAction.Rotate_Left, KeyCode.A),
                                new ControlDefinition("Rotate_Right",ControlAction.Rotate_Right, KeyCode.D),
                                new ControlDefinition("Strafe_Left",ControlAction.Strafe_Left, KeyCode.Q),
                                new ControlDefinition("Strafe_Right",ControlAction.Strafe_Right, KeyCode.E),
                                new ControlDefinition("Jump",ControlAction.Jump, KeyCode.Space),

                                new ControlDefinition("Interact_Key",ControlAction.InteractKey, KeyCode.F),

                                new ControlDefinition("Inventory",ControlAction.Inventory, KeyCode.I),
                                new ControlDefinition("Character_Sheet",ControlAction.Character_Sheet, KeyCode.C),
                                new ControlDefinition("Quest_Book",ControlAction.Quest_Book, KeyCode.J),
                                new ControlDefinition("Skill_Book",ControlAction.Skill_Book, KeyCode.K),
                                new ControlDefinition("Crafting",ControlAction.Crafting, KeyCode.L),
                                new ControlDefinition("Achievement",ControlAction.Achievement, KeyCode.Y),
                                new ControlDefinition("Talents",ControlAction.Talents, KeyCode.T),
                                new ControlDefinition("WorldMap",ControlAction.WorldMap, KeyCode.M),

                                new ControlDefinition("Hide_Ui",ControlAction.Hide_Ui, KeyCode.Z),
                                new ControlDefinition("Save_Game",ControlAction.Save_Game, KeyCode.F7),
                                new ControlDefinition("Load_Game",ControlAction.Load_Game, KeyCode.F8),
                                new ControlDefinition("Take_Screenshot",ControlAction.Take_Screenshot, KeyCode.Print)
             
                                        Skills 1 - 12*/

            AddAxis(new InputAxis(RPG.Gamepad.LeftStickVert, 2){invert = true});
            AddAxis(new InputAxis(RPG.Gamepad.LeftStickHori, 1));
            AddAxis(new InputAxis(RPG.Gamepad.RightStickVert, 5));
            AddAxis(new InputAxis(RPG.Gamepad.RightStickHori, 4));
            AddAxis(new InputAxis(RPG.Gamepad.DpadHori, 6));
            AddAxis(new InputAxis(RPG.Gamepad.DpadVert, 7));
            AddAxis(new InputAxis(RPG.Gamepad.GamepadA, "joystick button 0"));
            AddAxis(new InputAxis(RPG.Gamepad.GamepadB, "joystick button 1"));
            AddAxis(new InputAxis(RPG.Gamepad.GamepadX, "joystick button 2"));
            AddAxis(new InputAxis(RPG.Gamepad.GamepadY, "joystick button 3"));
            AddAxis(new InputAxis(RPG.Gamepad.GamepadLB, "joystick button 4"));
            AddAxis(new InputAxis(RPG.Gamepad.GamepadRB, "joystick button 5"));
            AddAxis(new InputAxis(RPG.Gamepad.GamepadBack, "joystick button 6"));
            AddAxis(new InputAxis(RPG.Gamepad.GamepadStart, "joystick button 7"));
            AddAxis(new InputAxis(RPG.Gamepad.GamepadTriggers, 3));
            AddAxis(new InputAxis(RPG.Gamepad.GamepadLeftStickClick, "joystick button 8"));
            AddAxis(new InputAxis(RPG.Gamepad.GamepadRightStickClick, "joystick button 9"));
        }

        private static SerializedProperty GetChildProperty(SerializedProperty parent, string name)
        {
            SerializedProperty child = parent.Copy();
            child.Next(true);
            do
            {
                if (child.name == name) return child;
            }
            while (child.Next(false));
            return null;
        }

        public static bool AxisDefined(string axisName)
        {
            SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

            axesProperty.Next(true);
            axesProperty.Next(true);
            while (axesProperty.Next(false))
            {
                SerializedProperty axis = axesProperty.Copy();
                var joyNum = GetChildProperty(axis, "joyNum").intValue;
                axis.Next(true);
                if (axis.stringValue == axisName) return true;
            }
            return false;
        }

        public enum AxisType
        {
            KeyOrMouseButton = 0,
            MouseMovement = 1,
            JoystickAxis = 2
        };

        public class InputAxis
        {
            public string name;
            public string descriptiveName;
            public string descriptiveNegativeName;
            public string negativeButton;
            public string positiveButton;
            public string altNegativeButton;
            public string altPositiveButton;

            public float gravity;
            public float dead;
            public float sensitivity;

            public bool snap = false;
            public bool invert = false;

            public AxisType type;

            public int axis;
            public int joyNum;



            /// <summary>
            /// Construct an axis
            /// </summary>
            public InputAxis(string axisName, int axisNum)
            {
                name = axisName;
                dead = 0.2f;
                sensitivity = 1f;
                type = AxisType.JoystickAxis;
                axis = axisNum;
                joyNum = 0;
            }

            /// <summary>
            /// Construct a button
            /// </summary>
            public InputAxis(string axisName, string button)
            {
                name = axisName;
                positiveButton = button;
                dead = 0.001f;
                sensitivity = 1000f;
                gravity = 1000f;
                type = AxisType.KeyOrMouseButton;
                axis = 0;
                joyNum = 0;
            }
        }

        private static void AddAxis(InputAxis axis)
        {
            if (AxisDefined(axis.name)) return;

            SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

            axesProperty.arraySize++;
            serializedObject.ApplyModifiedProperties();

            SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

            GetChildProperty(axisProperty, "m_Name").stringValue = axis.name;
            GetChildProperty(axisProperty, "descriptiveName").stringValue = axis.descriptiveName;
            GetChildProperty(axisProperty, "descriptiveNegativeName").stringValue = axis.descriptiveNegativeName;
            GetChildProperty(axisProperty, "negativeButton").stringValue = axis.negativeButton;
            GetChildProperty(axisProperty, "positiveButton").stringValue = axis.positiveButton;
            GetChildProperty(axisProperty, "altNegativeButton").stringValue = axis.altNegativeButton;
            GetChildProperty(axisProperty, "altPositiveButton").stringValue = axis.altPositiveButton;
            GetChildProperty(axisProperty, "gravity").floatValue = axis.gravity;
            GetChildProperty(axisProperty, "dead").floatValue = axis.dead;
            GetChildProperty(axisProperty, "sensitivity").floatValue = axis.sensitivity;
            GetChildProperty(axisProperty, "snap").boolValue = axis.snap;
            GetChildProperty(axisProperty, "invert").boolValue = axis.invert;
            GetChildProperty(axisProperty, "type").intValue = (int)axis.type;
            GetChildProperty(axisProperty, "axis").intValue = axis.axis - 1;
            GetChildProperty(axisProperty, "joyNum").intValue = axis.joyNum;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
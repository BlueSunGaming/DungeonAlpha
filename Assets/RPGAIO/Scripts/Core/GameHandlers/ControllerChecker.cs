using LogicSpawn.RPGMaker.API;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class ControllerChecker : MonoBehaviour
    {
        public static bool UsingKeyboardMouse = true;
        void Update()
        {
            if(UsingKeyboardMouse)
            {
                //Check gamepad keys
                if (Input.GetButton(RPG.Gamepad.GamepadA) ||
                   Input.GetButton(RPG.Gamepad.GamepadB) ||
                   Input.GetButton(RPG.Gamepad.GamepadX) ||
                   Input.GetButton(RPG.Gamepad.GamepadY) ||
                   Input.GetButton(RPG.Gamepad.GamepadLB) ||
                   Input.GetButton(RPG.Gamepad.GamepadRB) ||
                   Input.GetButton(RPG.Gamepad.GamepadLeftStickClick) ||
                   Input.GetButton(RPG.Gamepad.GamepadRightStickClick) ||
                   Input.GetButton(RPG.Gamepad.GamepadBack) ||
                   Input.GetButton(RPG.Gamepad.GamepadStart) ||

                //Check axes
                    (Input.GetAxis(RPG.Gamepad.LeftStickHori) != 0) ||
                    (Input.GetAxis(RPG.Gamepad.LeftStickVert) != 0) ||
                    (Input.GetAxis(RPG.Gamepad.RightStickHori) != 0) ||
                    (Input.GetAxis(RPG.Gamepad.RightStickVert) != 0) ||
                    (Input.GetAxis(RPG.Gamepad.GamepadTriggers) != 0) ||
                    (Input.GetAxis(RPG.Gamepad.DpadHori) != 0) ||
                    (Input.GetAxis(RPG.Gamepad.DpadVert) != 0)
                    )
                {
                    UsingKeyboardMouse = false;
                }
            }
            

            if(!UsingKeyboardMouse)
            {
                //Check keyboard and mouse keys
                for (int i = 1; i < 330; i++)
                {
                    if (Input.GetKey((KeyCode)i))
                    {
                        UsingKeyboardMouse = true;
                    }
                }
            }
        }
    }
}
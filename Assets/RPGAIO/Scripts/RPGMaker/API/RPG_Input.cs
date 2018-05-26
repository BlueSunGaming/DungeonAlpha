using System.Linq;
using LogicSpawn.RPGMaker.Core;
using UnityEngine;

namespace LogicSpawn.RPGMaker.API
{
    public static partial class RPG
    {
        public static class Input
        {

            //Input Axes
            public const string HorizontalAxis = "Rotate_Right|Rotate_Left";
            public const string VerticalAxis = "Move_Forward|Move_Backward";
            public const string StrafeAxis = "Strafe_Right|Strafe_Left";
            public const string CameraHorizontalAxis = "RotateCam_Right|RotateCam_Left";
            public const string CameraVerticalAxis = "RotateCam_Down|RotateCam_Up|";

            //Input Buttons
            public const string SelectTarget = "Select_Target";
            public const string ConfirmCast = "Confirm_Cast";
            public const string Attack = "Attack";
            public const string AttackInPlaceKey = "Attack_In_Place";
            public const string InteractMouse = "Interact";
            public const string TargetNearest = "Target_Nearest/Lock";
            public const string Sprint = "Sprint";

            public const string MoveForward = "Move_Forward";
            public const string MoveBackward = "Move_Backward";
            public const string RotateLeft = "Rotate_Left";
            public const string RotateRight = "Rotate_Right";
            public const string RotateCamLeft = "RotateCam_Left";
            public const string RotateCamRight = "RotateCam_Right";
            public const string RotateCamUp = "RotateCam_Up";
            public const string RotateCamDown = "RotateCam_Down";
            public const string StrafeLeft = "Strafe_Left";
            public const string StrafeRight = "Strafe_Right";
            public const string Jump = "Jump";
            public const string InteractKey = "Interact_Key";
            public const string Inventory = "Inventory";
            public const string CharacterSheet = "Character_Sheet";
            public const string QuestBook = "Quest_Book";
            public const string SkillBook = "Skill_Book";
            public const string Crafting = "Crafting";
            public const string Achievement = "Achievement";
            public const string Talents = "Talents";
            public const string WorldMap = "WorldMap";
            public const string HideUi = "Hide_Ui";
            public const string SaveGame = "Save_Game";
            public const string LoadGame = "Load_Game";
            public const string TakeScreenshot = "Take_Screenshot";
            public const string Skill1 = "Skill_1";
            public const string Skill2 = "Skill_2";
            public const string Skill3 = "Skill_3";
            public const string Skill4 = "Skill_4";
            public const string Skill5 = "Skill_5";
            public const string Skill6 = "Skill_6";
            public const string Skill7 = "Skill_7";
            public const string Skill8 = "Skill_8";
            public const string Skill9 = "Skill_9";
            public const string Skill10 = "Skill_10";
            public const string Skill11 = "Skill_11";
            public const string Skill12 = "Skill_12";

            //todo: def should not pull from DefaultControlsList, but current controls list
            // i.e. Rm_GameConfig.Instance

            public static float GetAxis(string axisName)
            {
                if (!axisName.Contains("|"))
                {
                    Debug.LogError("Called GetAxis with a button. Use one of the axes in RPG.Input to use this method.");
                    return 0;
                }

                var axes = axisName.Split('|');
                var negativeAxis = axes[0];
                var positiveAxis = axes[1];

                var negDef = Rm_GameConfig.Instance.Controls.ControlDefinitions.Concat(Rm_RPGHandler.Instance.DefaultSettings.ControlSettings).FirstOrDefault(d => d.ID == negativeAxis || d.VisibleName == negativeAxis);
                var posDef = Rm_GameConfig.Instance.Controls.ControlDefinitions.Concat(Rm_RPGHandler.Instance.DefaultSettings.ControlSettings).FirstOrDefault(d => d.ID == positiveAxis || d.VisibleName == positiveAxis);

                if (negDef != null && posDef != null)
                {
                    if (GetKeyCheck(negDef.IsShift, negDef.HasModifier, negDef.ModifierKey, negDef.Key, negDef.MouseButton) ||
                        GetKeyCheck(negDef.IsShiftAlt, negDef.HasModifierAlt, negDef.ModifierKeyAlt, negDef.KeyAlt, negDef.MouseButtonAlt))
                    {
                        return 1f;
                    }

                    if (GetKeyCheck(posDef.IsShift, posDef.HasModifier, posDef.ModifierKey, posDef.Key, posDef.MouseButton) ||
                        GetKeyCheck(posDef.IsShiftAlt, posDef.HasModifierAlt, posDef.ModifierKeyAlt, posDef.KeyAlt, posDef.MouseButtonAlt))
                    {
                        return -1f;
                    }

                    return string.IsNullOrEmpty(posDef.InputAxisName) ? 0 : UnityEngine.Input.GetAxis(posDef.InputAxisName);

                }

                Debug.Log("Invalid axis specified");
                return 0;
            }

            public static bool GetKey(string name)
            {
                var def = Rm_GameConfig.Instance.Controls.ControlDefinitions.Concat(Rm_RPGHandler.Instance.DefaultSettings.ControlSettings).FirstOrDefault(d => d.ID == name || d.VisibleName == name);
                if (def != null)
                {
                    if (GetKeyCheck(def.IsShift, def.HasModifier, def.ModifierKey, def.Key, def.MouseButton) ||
                        GetKeyCheck(def.IsShiftAlt, def.HasModifierAlt, def.ModifierKeyAlt, def.KeyAlt, def.MouseButtonAlt) ||
                        GetGamePad(def.InputAxisName, def.IsAxis, def.IsPositiveAxis))
                    {
                        return true;
                    }
                }
                else
                {
                    Debug.Log("Did not find that control(" + name + "). You may need to define it in the RPG Maker");
                    return false;
                }

                return false;
            }

            public static bool GetKeyDown(string name)
            {
                var def = Rm_GameConfig.Instance.Controls.ControlDefinitions.Concat(Rm_RPGHandler.Instance.DefaultSettings.ControlSettings).FirstOrDefault(d => d.ID == name || d.VisibleName == name);
                if (def != null)
                {
                    if (GetKeyDownCheck(def.IsShift, def.HasModifier, def.ModifierKey, def.Key, def.MouseButton) ||
                        GetKeyDownCheck(def.IsShiftAlt, def.HasModifierAlt, def.ModifierKeyAlt, def.KeyAlt, def.MouseButtonAlt) ||
                        GetGamePadDown(def.InputAxisName, def.IsAxis, def.IsPositiveAxis))
                    {
                        return true;
                    }
                }
                else
                {
                    Debug.Log("Did not find that control(" + name + "). You may need to define it in the RPG Maker");
                    return false;
                }

                return false;
            }

            public static bool GetKeyUp(string name)
            {
                var def = Rm_GameConfig.Instance.Controls.ControlDefinitions.Concat(Rm_RPGHandler.Instance.DefaultSettings.ControlSettings).FirstOrDefault(d => d.ID == name || d.VisibleName == name);
                if (def != null)
                {
                    if (GetKeyUpCheck(def.IsShift, def.HasModifier, def.ModifierKey, def.Key, def.MouseButton) ||
                        GetKeyUpCheck(def.IsShiftAlt, def.HasModifierAlt, def.ModifierKeyAlt, def.KeyAlt, def.MouseButtonAlt) ||
                        GetGamePadUp(def.InputAxisName, def.IsAxis, def.IsPositiveAxis))
                    {
                        return true;
                    }
                }
                else
                {
                    Debug.Log("Did not find that control(" + name + "). You may need to define it in the RPG Maker");
                    return false;
                }

                return false;
            }

            public static bool GetGamePadInput(string name)
            {
                var def = Rm_GameConfig.Instance.Controls.ControlDefinitions.Concat(Rm_RPGHandler.Instance.DefaultSettings.ControlSettings).FirstOrDefault(d => d.ID == name || d.VisibleName == name);
                if (def != null)
                {
                    if (GetGamePad(def.InputAxisName, def.IsAxis, def.IsPositiveAxis))
                    {
                        return true;
                    }
                }
                else
                {
                    Debug.Log("Did not find that control(" + name + "). You may need to define it in the RPG Maker");
                    return false;
                }

                return false;
            }

            public static bool GetGamePadInputDown(string name)
            {
                var def = Rm_GameConfig.Instance.Controls.ControlDefinitions.Concat(Rm_RPGHandler.Instance.DefaultSettings.ControlSettings).FirstOrDefault(d => d.ID == name || d.VisibleName == name);
                if (def != null)
                {
                    if (GetGamePadDown(def.InputAxisName, def.IsAxis, def.IsPositiveAxis))
                    {
                        return true;
                    }
                }
                else
                {
                    Debug.Log("Did not find that control(" + name + "). You may need to define it in the RPG Maker");
                    return false;
                }

                return false;
            }

            public static bool GetGamePadInputUp(string name)
            {
                var def = Rm_GameConfig.Instance.Controls.ControlDefinitions.Concat(Rm_RPGHandler.Instance.DefaultSettings.ControlSettings).FirstOrDefault(d => d.ID == name || d.VisibleName == name);
                if (def != null)
                {
                    if (GetGamePadUp(def.InputAxisName, def.IsAxis, def.IsPositiveAxis))
                    {
                        return true;
                    }
                }
                else
                {
                    Debug.Log("Did not find that control(" + name + "). You may need to define it in the RPG Maker");
                    return false;
                }

                return false;
            }


            private static bool GetGamePad(string inputAxisName, bool isAxis, bool isPositiveAxis)
            {
                if (string.IsNullOrEmpty(inputAxisName)) return false;

                if (isAxis)
                {
                    return isPositiveAxis ? UnityEngine.Input.GetAxis(inputAxisName) > 0 : UnityEngine.Input.GetAxis(inputAxisName) < 0;
                }
                else
                {
                    return UnityEngine.Input.GetButton(inputAxisName);
                }
            }

            private static bool GetGamePadDown(string inputAxisName, bool isAxis, bool isPositiveAxis)
            {
                if (string.IsNullOrEmpty(inputAxisName)) return false;

                if (!isAxis)
                {
                    return UnityEngine.Input.GetButtonDown(inputAxisName);
                }

                return false;
            }

            private static bool GetGamePadUp(string inputAxisName, bool isAxis, bool isPositiveAxis)
            {
                if (string.IsNullOrEmpty(inputAxisName)) return false;

                if (!isAxis)
                {
                    return UnityEngine.Input.GetButtonUp(inputAxisName);
                }

                return false;
            }

            private static bool GetKeyCheck(bool isShift, bool hasModifier, EventModifiers modifierKey, KeyCode key, int mouseButton)
            {
                if (isShift)
                {
                    if (UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift))
                    {
                        return true;
                    }
                }
                else if (mouseButton != -1)
                {
                    if (UnityEngine.Input.GetMouseButton(mouseButton))
                    {
                        return true;
                    }
                }
                else if (UnityEngine.Input.GetKey(key))
                {
                    if (hasModifier)
                    {
                        if (modifierKey == EventModifiers.Control)
                        {
                            if (UnityEngine.Input.GetKey(KeyCode.LeftControl) || UnityEngine.Input.GetKey(KeyCode.RightControl))
                            {
                                return true;
                            }
                        }
                        else if (modifierKey == EventModifiers.Shift)
                        {
                            if (UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift))
                            {
                                return true;
                            }
                        }
                        else if (modifierKey == EventModifiers.Alt)
                        {
                            if (UnityEngine.Input.GetKey(KeyCode.LeftAlt) || UnityEngine.Input.GetKey(KeyCode.RightAlt) || UnityEngine.Input.GetKey(KeyCode.AltGr))
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }
                }


                return false;
            }

            private static bool GetKeyDownCheck(bool isShift, bool hasModifier, EventModifiers modifierKey, KeyCode key, int mouseButton)
            {

                if (isShift)
                {
                    if (UnityEngine.Input.GetKeyDown(KeyCode.LeftShift) || UnityEngine.Input.GetKeyDown(KeyCode.RightShift))
                    {
                        return true;
                    }
                }
                else if (mouseButton != -1)
                {
                    if (UnityEngine.Input.GetMouseButtonDown(mouseButton))
                    {
                        return true;
                    }
                }
                else if (UnityEngine.Input.GetKeyDown(key))
                {
                    if (hasModifier)
                    {
                        if (modifierKey == EventModifiers.Control)
                        {
                            if (UnityEngine.Input.GetKey(KeyCode.LeftControl) || UnityEngine.Input.GetKey(KeyCode.RightControl))
                            {
                                return true;
                            }
                        }
                        else if (modifierKey == EventModifiers.Shift)
                        {
                            if (UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift))
                            {
                                return true;
                            }
                        }
                        else if (modifierKey == EventModifiers.Alt)
                        {
                            if (UnityEngine.Input.GetKey(KeyCode.LeftAlt) || UnityEngine.Input.GetKey(KeyCode.RightAlt) || UnityEngine.Input.GetKey(KeyCode.AltGr))
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                return false;
            }

            private static bool GetKeyUpCheck(bool isShift, bool hasModifier, EventModifiers modifierKey, KeyCode key, int mouseButton)
            {

                if (isShift)
                {
                    if (UnityEngine.Input.GetKeyUp(KeyCode.LeftShift) ||
                        UnityEngine.Input.GetKeyUp(KeyCode.RightShift))
                    {
                        return true;
                    }
                }
                else if (mouseButton != -1)
                {
                    if (UnityEngine.Input.GetMouseButtonUp(mouseButton))
                    {
                        return true;
                    }
                }
                else if (UnityEngine.Input.GetKeyUp(key))
                {
                    if (hasModifier)
                    {
                        if (modifierKey == EventModifiers.Control)
                        {
                            if (UnityEngine.Input.GetKey(KeyCode.LeftControl) || UnityEngine.Input.GetKey(KeyCode.RightControl))
                            {
                                return true;
                            }
                        }
                        else if (modifierKey == EventModifiers.Shift)
                        {
                            if (UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift))
                            {
                                return true;
                            }
                        }
                        else if (modifierKey == EventModifiers.Alt)
                        {
                            if (UnityEngine.Input.GetKey(KeyCode.LeftAlt) || UnityEngine.Input.GetKey(KeyCode.RightAlt) || UnityEngine.Input.GetKey(KeyCode.AltGr))
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
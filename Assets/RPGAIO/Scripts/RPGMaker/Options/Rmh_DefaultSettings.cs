using System.Collections.Generic;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Core;
using UnityEngine;

namespace LogicSpawn.RPGMaker
{
    public class Rmh_DefaultSettings
    {
        public List<ControlDefinition> ControlSettings;

        public Rm_ControlSetup DefaultControls;
        public Rm_AudioSetup DefaultAudio;
        public Rm_GraphicsSetup DefaultGraphics;

        public bool CanFastTravelOnMap;
        public bool CanAttackOnSpot;
        public bool EnableClickToMove;
        public bool EnableInteractWithKey;
        public bool HoldRunKeyToRun;
        public bool EnableTargetLock;
        public CameraMode DefaultCameraMode;

        public Rmh_DefaultSettings()
        {
            CanFastTravelOnMap = false;
            CanAttackOnSpot = true;
            EnableClickToMove = false;
            EnableInteractWithKey = false;
            EnableTargetLock = false;
            HoldRunKeyToRun = true;
            DefaultCameraMode = CameraMode.Standard;

            ControlSettings = new List<ControlDefinition>()
                                  {
                                        new ControlDefinition("Select_Target",ControlAction.Custom, MouseButton.Left),
                                        new ControlDefinition("Confirm_Cast",ControlAction.Custom, MouseButton.Left),
                                        new ControlDefinition("Attack",ControlAction.Custom, MouseButton.Right){InputAxisName = RPG.Gamepad.GamepadTriggers, IsAxis = true, IsPositiveAxis = true},
                                        new ControlDefinition("Attack_In_Place",ControlAction.Custom, KeyCode.LeftShift),
                                        new ControlDefinition("Interact",ControlAction.Custom, MouseButton.Right),
                                        new ControlDefinition("Target_Nearest/Lock",ControlAction.Custom, KeyCode.Tab){InputAxisName = RPG.Gamepad.GamepadLB, IsAxis = false},
                                        new ControlDefinition("Sprint",ControlAction.Custom, KeyCode.LeftShift){KeyAlt = KeyCode.RightShift, InputAxisName = RPG.Gamepad.GamepadLeftStickClick, IsAxis = false},
                                        new ControlDefinition("Alternate_Skills",ControlAction.Custom, KeyCode.None){InputAxisName = RPG.Gamepad.GamepadTriggers, IsAxis = true, IsPositiveAxis = false},
                                  };

            var defaultControlsList = new List<ControlDefinition>()
                                      {
                                          new ControlDefinition("Move_Forward",ControlAction.Move_Forward, KeyCode.W){InputAxisName = RPG.Gamepad.LeftStickVert, IsAxis = true, IsPositiveAxis = false},
                                          new ControlDefinition("Move_Backward",ControlAction.Move_Backward, KeyCode.S){InputAxisName = RPG.Gamepad.LeftStickVert, IsAxis = true, IsPositiveAxis = true},
                                          new ControlDefinition("Rotate_Left",ControlAction.Rotate_Left, KeyCode.A){InputAxisName = RPG.Gamepad.LeftStickHori, IsAxis = true, IsPositiveAxis = false},
                                          new ControlDefinition("Rotate_Right",ControlAction.Rotate_Right, KeyCode.D){InputAxisName = RPG.Gamepad.LeftStickHori, IsAxis = true, IsPositiveAxis = true},
                                          new ControlDefinition("RotateCam_Left",ControlAction.Rotate_Up, KeyCode.None){InputAxisName = RPG.Gamepad.RightStickHori, IsAxis = true, IsPositiveAxis = false},
                                          new ControlDefinition("RotateCam_Right",ControlAction.Rotate_Down, KeyCode.None){InputAxisName = RPG.Gamepad.RightStickHori, IsAxis = true, IsPositiveAxis = true},
                                          new ControlDefinition("RotateCam_Up",ControlAction.Rotate_Up, KeyCode.None){InputAxisName = RPG.Gamepad.RightStickVert, IsAxis = true, IsPositiveAxis = false},
                                          new ControlDefinition("RotateCam_Down",ControlAction.Rotate_Down, KeyCode.None){InputAxisName = RPG.Gamepad.RightStickVert, IsAxis = true, IsPositiveAxis = true},
                                          new ControlDefinition("Reset_Camera",ControlAction.Rotate_Down, KeyCode.None){InputAxisName = RPG.Gamepad.GamepadRightStickClick, IsAxis = false},
                                          new ControlDefinition("Strafe_Left",ControlAction.Strafe_Left, KeyCode.Q),
                                          new ControlDefinition("Strafe_Right",ControlAction.Strafe_Right, KeyCode.E),
                                          new ControlDefinition("Jump",ControlAction.Jump, KeyCode.Space){InputAxisName = RPG.Gamepad.GamepadA, IsAxis = false},

                                          new ControlDefinition("Interact_Key",ControlAction.InteractKey, KeyCode.F){InputAxisName = RPG.Gamepad.GamepadX, IsAxis = false},

                                          new ControlDefinition("Inventory",ControlAction.Inventory, KeyCode.I){InputAxisName = RPG.Gamepad.GamepadBack, IsAxis = false},
                                          new ControlDefinition("Character_Sheet",ControlAction.Character_Sheet, KeyCode.C){InputAxisName = RPG.Gamepad.DpadHori, IsAxis = true, IsPositiveAxis = false},
                                          new ControlDefinition("Quest_Book",ControlAction.Quest_Book, KeyCode.J){InputAxisName = RPG.Gamepad.DpadVert, IsAxis = true, IsPositiveAxis = true},
                                          new ControlDefinition("Skill_Book",ControlAction.Skill_Book, KeyCode.K){InputAxisName = RPG.Gamepad.DpadHori, IsAxis = true, IsPositiveAxis = true},
                                          new ControlDefinition("Crafting",ControlAction.Crafting, KeyCode.L),
                                          new ControlDefinition("Achievement",ControlAction.Achievement, KeyCode.Y),
                                          new ControlDefinition("Talents",ControlAction.Talents, KeyCode.T),
                                          new ControlDefinition("WorldMap",ControlAction.WorldMap, KeyCode.M){InputAxisName = RPG.Gamepad.DpadVert, IsAxis = true, IsPositiveAxis = false},

                                          new ControlDefinition("Hide_Ui",ControlAction.Hide_Ui, KeyCode.Z),
                                          new ControlDefinition("Save_Game",ControlAction.Save_Game, KeyCode.F7),
                                          new ControlDefinition("Load_Game",ControlAction.Load_Game, KeyCode.F8),
                                          new ControlDefinition("Take_Screenshot",ControlAction.Take_Screenshot, KeyCode.Print)
                                      };

            var skillKeyCodes = new[] {KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5,
                                        KeyCode.Alpha6,KeyCode.Alpha7,KeyCode.Alpha8,KeyCode.Alpha9,KeyCode.Alpha0, KeyCode.Minus,KeyCode.Plus};

            for (int i = 0; i < 12; i++)
            {
                var skillControl = new ControlDefinition("Skill_" + (i+1))
                {
                    VisibleName = "Skill " + (i+1),
                    Action = ControlAction.Use_Skill,
                    IntParameter = i,
                    Key = skillKeyCodes[i],
                    IsRequiredControl = true
                };

                //Skill 1-3 controller controls
                if(i == 0)
                {
                    skillControl.InputAxisName = RPG.Gamepad.GamepadRB;
                    skillControl.IsAxis = false;
                }

                if(i == 1)
                {
                    skillControl.InputAxisName = RPG.Gamepad.GamepadY;
                    skillControl.IsAxis = false;
                }

                if(i == 2)
                {
                    skillControl.InputAxisName = RPG.Gamepad.GamepadB;
                    skillControl.IsAxis = false;
                }

                defaultControlsList.Add(skillControl);
            }

            DefaultControls = new Rm_ControlSetup()
                                  {
                                      ControlDefinitions = defaultControlsList
                                  };

            DefaultAudio = new Rm_AudioSetup()
            {
                MasterVolume = 30,
                MusicVolume = 30,
                VoiceVolume = 30,
                SoundEffectVolume = 30,
                AmbientVolume = 30,
                SpeakerMode = AudioSettings.driverCapabilities
            };

			var r = new Resolution();
            r.width = 1280;
            r.height = 720;
            r.refreshRate = Screen.currentResolution.refreshRate;

            DefaultGraphics = new Rm_GraphicsSetup()
            {
                FullScreen = false,
                Resolution = r,
                QualityLevel = QualitySettings.names.Length - 1,
                AnisotropicFiltering = AnisotropicFiltering.Enable,
                AntialiasingLevel = (int)Rm_AliasingLevels.Eight,
                TextureQuality = (int)Rm_TextureLevels.Max,
                VSyncCount = (int)Rm_VSyncLevels.Off,
                ShadowType = LightShadows.Hard
            };
        }
    }
}
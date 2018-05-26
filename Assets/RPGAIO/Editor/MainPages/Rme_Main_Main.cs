using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;
using UnityEditor;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Editor
{
    public static class Rme_Main_Main
    {
        #region Home

        private static Texture2D PictureA = PictureA ?? Resources.Load("RPGMakerAssets/sampleImageA") as Texture2D;
        private static Texture2D PictureB = PictureB ?? Resources.Load("RPGMakerAssets/forumsquare") as Texture2D;
        private static Texture2D PictureC = PictureC ?? Resources.Load("RPGMakerAssets/sampleImageC") as Texture2D;

        public static void Home(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            GUI.Box(fullArea, "","backgroundBox");

            GUILayout.BeginArea(fullArea);
            GUILayout.BeginHorizontal("mainTitleCenter");
            //GUILayout.Box("", "mainTitleCenter");
            GUILayout.FlexibleSpace();
            if(GUILayout.Button(Rme_Main.PatchInfo,"Label"))
            {
                Application.OpenURL(Rme_Main.PatchNoteUrl);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.BeginVertical(GUILayout.MaxWidth(250));
            GUILayout.Label("Video Tutorials", "homePageText", GUILayout.Width(250));
            GUILayout.Box(PictureA, "genericButton", GUILayout.Height(250), GUILayout.Width(250));

            GUILayout.Label("Visit the website to browse and learn from our handy video tutorials.",
                            "homePageText", GUILayout.Width(250), GUILayout.Height(75));
            if(GUILayout.Button("Open Videos", "genericButton", GUILayout.Width(250)))
            {
                Application.OpenURL("http://rpgaio.logicspawn.co.uk/tutorials.html");
            }
            GUILayout.FlexibleSpace();

            GUILayout.EndVertical();

            GUILayout.Space(5);

            GUILayout.BeginVertical(GUILayout.MaxWidth(250));
            GUILayout.Label("Browse Forums", "homePageText", GUILayout.Width(250));
            GUILayout.Box(PictureB, "genericButton", GUILayout.Height(250), GUILayout.Width(250));

            GUILayout.Label("Get help, report bugs, and discuss RPGAIO with other users",
                            "homePageText", GUILayout.Width(250), GUILayout.Height(75));
            if (GUILayout.Button("Visit Forums", "genericButton", GUILayout.Width(250)))
            {
                Application.OpenURL("http://rpgaio.logicspawn.co.uk/forums/");
            }
            GUILayout.FlexibleSpace();

            GUILayout.EndVertical();


            GUILayout.Space(5);


            GUILayout.BeginVertical(GUILayout.MaxWidth(250));
            GUILayout.Label("Liking RPGAIO?", "homePageText", GUILayout.Width(250));
            GUILayout.Box(PictureC, "genericButton", GUILayout.Height(250), GUILayout.Width(250));

            GUILayout.Label("Don't forget to give RPGAIO a 5 star rating to support development.",
                            "homePageText", GUILayout.Width(250), GUILayout.Height(75));
            if (GUILayout.Button("Rate now", "genericButton", GUILayout.Width(250)))
            {
                Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/53542");
            } 
            GUILayout.FlexibleSpace();

            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        #endregion

        #region GameInfo

        private static Texture2D normalCursor = null;
        private static string cursorPath = "";
        private static Cursors Cursors { get { return Rm_RPGHandler.Instance.GameInfo.Cursors; } }
        public static void GameInfo(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            GUI.Box(fullArea, "","backgroundBox");

            GUILayout.BeginArea(fullArea);
            RPGMakerGUI.Title("Game Info");
            Rm_RPGHandler.Instance.GameInfo.GameTitle = RPGMakerGUI.TextField("Game Name: ",
                                                                                  Rm_RPGHandler.Instance.GameInfo.
                                                                                      GameTitle);
            Rm_RPGHandler.Instance.GameInfo.GameCompany = RPGMakerGUI.TextField("Company Name: ",
                                                                                    Rm_RPGHandler.Instance.GameInfo.
                                                                                        GameCompany);

            GUILayout.Space(5);
            RPGMakerGUI.Title("Game Cursors");
            GUILayout.BeginHorizontal();

            Cursors.Default = RPGMakerGUI.ImageSelector("Normal", Cursors.Default, ref Cursors.DefaultCursorPath);
            Cursors.NPC = RPGMakerGUI.ImageSelector("NPC", Cursors.NPC, ref Cursors.NpcCursorPath);
            Cursors.Interact = RPGMakerGUI.ImageSelector("Interact", Cursors.Interact, ref Cursors.InteractCursorPath);
            Cursors.Enemy = RPGMakerGUI.ImageSelector("Enemy", Cursors.Enemy, ref Cursors.EnemyCursorPath);
            Cursors.Item = RPGMakerGUI.ImageSelector("Item", Cursors.Item, ref Cursors.ItemCursorPath);
            Cursors.Harvest = RPGMakerGUI.ImageSelector("Harvest", Cursors.Harvest, ref Cursors.HarvestCursorPath);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            Cursors.Draggable = RPGMakerGUI.ImageSelector("Draggable", Cursors.Draggable, ref Cursors.DraggableCursorPath);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.EndArea();

        }

        #endregion

        #region "Minimap"
        private static MinimapOptions MinimapOptions { get { return Rm_RPGHandler.Instance.GameInfo.MinimapOptions; } }
        public static void Minimap(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            GUI.Box(fullArea, "", "backgroundBox");

            GUILayout.BeginArea(fullArea);
            RPGMakerGUI.Title("Minimap Options");
            RPGMakerGUI.Toggle("Show Minimap?", ref MinimapOptions.ShowMinimap);
            RPGMakerGUI.Toggle("Rotate Minimap with Player?", ref MinimapOptions.RotateMinimapWithPlayer);

            GUILayout.Space(5);
            RPGMakerGUI.Title("Default Minimap Icons");
            RPGMakerGUI.Help("Ensure that your icons are imported as a \"Sprite (2D and UI)\"", 0);
            GUILayout.BeginHorizontal();

            MinimapOptions.Player = RPGMakerGUI.ImageSelector("Player", MinimapOptions.Player, ref MinimapOptions.PlayerIconPath);
            if (!string.IsNullOrEmpty(MinimapOptions.PlayerIconPath) && MinimapOptions.PlayerSprite == null)
            {
                MinimapOptions.PlayerIconPath = "";
                Debug.LogError("[RPGAIO] Assigned Minimap Icon is not a sprite. Change the image's import options to \"Sprite (2D and UI)\"");
            }

            MinimapOptions.NPC = RPGMakerGUI.ImageSelector("NPC", MinimapOptions.NPC, ref MinimapOptions.NpcIconPath);
            if (!string.IsNullOrEmpty(MinimapOptions.NpcIconPath) && MinimapOptions.NpcSprite == null)
            {
                MinimapOptions.NpcIconPath = "";
                Debug.LogError("[RPGAIO] Assigned Minimap Icon is not a sprite. Change the image's import options to \"Sprite (2D and UI)\"");
            }

            MinimapOptions.Interact = RPGMakerGUI.ImageSelector("Interact", MinimapOptions.Interact, ref MinimapOptions.InteractIconPath);
            if (!string.IsNullOrEmpty(MinimapOptions.InteractIconPath) && MinimapOptions.InteractSprite == null)
            {
                MinimapOptions.InteractIconPath = "";
                Debug.LogError("[RPGAIO] Assigned Minimap Icon is not a sprite. Change the image's import options to \"Sprite (2D and UI)\"");
            }

            MinimapOptions.Enemy = RPGMakerGUI.ImageSelector("Enemy", MinimapOptions.Enemy, ref MinimapOptions.EnemyIconPath);
            if (!string.IsNullOrEmpty(MinimapOptions.EnemyIconPath) && MinimapOptions.EnemySprite == null)
            {
                MinimapOptions.EnemyIconPath = "";
                Debug.LogError("[RPGAIO] Assigned Minimap Icon is not a sprite. Change the image's import options to \"Sprite (2D and UI)\"");
            }

            MinimapOptions.Harvest = RPGMakerGUI.ImageSelector("Harvest", MinimapOptions.Harvest, ref MinimapOptions.HarvestIconPath);
            if (!string.IsNullOrEmpty(MinimapOptions.HarvestIconPath) && MinimapOptions.HarvestSprite == null)
            {
                MinimapOptions.HarvestIconPath = "";
                Debug.LogError("[RPGAIO] Assigned Minimap Icon is not a sprite. Change the image's import options to \"Sprite (2D and UI)\"");
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.EndArea();

        }

        #endregion

        #region Game Settings

        private static Vector2 controlScrollPos = Vector2.zero;
        private static Vector2 gameOptionsScrollPos = Vector2.zero;

        private static Rmh_DefaultSettings Settings
        {
            get { return Rm_RPGHandler.Instance.DefaultSettings; }
        }

        public static ControlDefinition ControlDefinition = null;
        public static bool ChangingAlt = false;
        public static List<ControlDefinition> MyDefinitions
        {
            get { return Rm_RPGHandler.Instance.DefaultSettings.DefaultControls.ControlDefinitions; }
        }
        public static List<ControlDefinition> CoreDefinitions
        {
            get { return Rm_RPGHandler.Instance.DefaultSettings.ControlSettings; }
        }
        public static Rmh_Customise Customise
        {
            get { return Rm_RPGHandler.Instance.Customise; }
        }
        public static string GotShift = "";
        public static string LastShift;
        private static bool GotShiftSolo = false;

        private static int selectedEventIndex;
        public static void GameSettings(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            var e = Event.current;
            if(e.type == EventType.KeyDown)
            {
                if(e.keyCode == KeyCode.Escape && e.modifiers == 0)
                {
                    ClearCurrentKey(ChangingAlt);
                }
            }

            GUI.Box(fullArea, "","backgroundBox");
            GUILayout.BeginArea(fullArea);

            gameOptionsScrollPos = GUILayout.BeginScrollView(gameOptionsScrollPos);
            RPGMakerGUI.Title("Game Options");
            RPGMakerGUI.SubTitle("General Settings");

            RPGMakerGUI.Toggle("Can fast travel on map?", ref Rm_RPGHandler.Instance.DefaultSettings.CanFastTravelOnMap);
            Settings.DefaultCameraMode = (CameraMode)RPGMakerGUI.EnumPopup("Camera Mode:", Settings.DefaultCameraMode);
            if(Settings.DefaultCameraMode == CameraMode.TopDown)
            {
                Customise.TopDownHeight = RPGMakerGUI.FloatField("Top Down Height", Customise.TopDownHeight, 1);
                Customise.TopDownDistance = RPGMakerGUI.FloatField("Top Down Distance", Customise.TopDownDistance, 1);

                Customise.CameraXOffset = RPGMakerGUI.FloatField("Camera X Offset", Customise.CameraXOffset, 1);
                Customise.CameraYOffset = RPGMakerGUI.FloatField("Camera Y Offset", Customise.CameraYOffset, 1);
                Customise.CameraZOffset = RPGMakerGUI.FloatField("Camera Z Offset", Customise.CameraZOffset, 1);
            }

#if (!UNITY_IOS && !UNITY_ANDROID)
            if(RPGMakerGUI.Toggle("Enable Orbit Around Player:", ref Customise.EnableOrbitPlayer))
            {
                Customise.OrbitPlayerOption = (ClickOption)RPGMakerGUI.EnumPopup("- Mouse Button:", Customise.OrbitPlayerOption);
            }
            if (RPGMakerGUI.Toggle("Enable Click Rotate Player:", ref Customise.EnableClickToRotate))
            {
                Customise.ClickToRotateOption = (ClickOption)RPGMakerGUI.EnumPopup("- Mouse Button:", Customise.ClickToRotateOption);
            }

            RPGMakerGUI.Toggle("Both Mouse Buttons to Move Forward:", ref Customise.PressBothMouseButtonsToMove);
            RPGMakerGUI.Toggle("Rotate Camera With Player?", ref Customise.RotateCameraWithPlayer);
            RPGMakerGUI.Toggle("Enable Camera Target Lock?", ref Rm_RPGHandler.Instance.DefaultSettings.EnableTargetLock);
#endif



            RPGMakerGUI.Toggle("Enable 2D Audio?", ref Rm_RPGHandler.Instance.Audio.Enable2DAudio);

            RPGMakerGUI.SubTitle("Default Graphics");

            Settings.DefaultGraphics.FullScreen = RPGMakerGUI.Toggle("Fullscreen:", Settings.DefaultGraphics.FullScreen);
            Settings.DefaultGraphics.QualityLevel = RPGMakerGUI.Popup("Quality Level:", Settings.DefaultGraphics.QualityLevel, QualitySettings.names);
            Settings.DefaultGraphics.AnisotropicFiltering =
                (AnisotropicFiltering)
                RPGMakerGUI.EnumPopup("Anisotropic Filtering:", Settings.DefaultGraphics.AnisotropicFiltering);
            Settings.DefaultGraphics.TextureQuality = RPGMakerGUI.Popup("Texture Quality:",
                                                                            Settings.DefaultGraphics.TextureQuality,
                                                                            Enum.GetNames(typeof (Rm_TextureLevels)));
            Settings.DefaultGraphics.VSyncCount = RPGMakerGUI.Popup("Vertical Sync:",
                                                                        Settings.DefaultGraphics.VSyncCount,
                                                                        Enum.GetNames(typeof (Rm_VSyncLevels)));
            Settings.DefaultGraphics.ShadowType =
                (LightShadows)
                RPGMakerGUI.EnumPopup("Shadows:", Settings.DefaultGraphics.ShadowType);

            RPGMakerGUI.SubTitle("Default Audio");
            Settings.DefaultAudio.MasterVolume = EditorGUILayout.IntSlider("Master Volume:",
                                                                           Settings.DefaultAudio.MasterVolume, 0, 100);
            Settings.DefaultAudio.MusicVolume = EditorGUILayout.IntSlider("Music Volume:",
                                                                          Settings.DefaultAudio.MusicVolume, 0, 100);
            Settings.DefaultAudio.SoundEffectVolume = EditorGUILayout.IntSlider("Sound Effect Volume:",
                                                                                Settings.DefaultAudio.SoundEffectVolume,
                                                                                0, 100);
            Settings.DefaultAudio.VoiceVolume = EditorGUILayout.IntSlider("Voice/Dialogue Volume:",
                                                                                Settings.DefaultAudio.VoiceVolume,
                                                                                0, 100);
            Settings.DefaultAudio.AmbientVolume = EditorGUILayout.IntSlider("Ambient Volume:",
                                                                            Settings.DefaultAudio.AmbientVolume, 0, 100);
            RPGMakerGUI.SubTitle("Controls:");
            EditorGUILayout.HelpBox("To assign \"Shift\" press Shift+Escape. This is due to a UnityUI issue. Press ESC to clear key.", MessageType.Info);



            RPGMakerGUI.Toggle("Shift + Attack attacks on spot?", ref Rm_RPGHandler.Instance.DefaultSettings.CanAttackOnSpot);
            RPGMakerGUI.Toggle("Hold run key to run?", ref Rm_RPGHandler.Instance.DefaultSettings.HoldRunKeyToRun);
            RPGMakerGUI.Toggle("Enable Click To Move?", ref Rm_RPGHandler.Instance.DefaultSettings.EnableClickToMove);
            RPGMakerGUI.Toggle("Enable Interaction with key?", ref Rm_RPGHandler.Instance.DefaultSettings.EnableInteractWithKey);

            #region CoreControl
            for (int index = 0; index < CoreDefinitions.Count; index++)
            {
                var control = CoreDefinitions[index];
                GUILayout.Space(5);

                GUILayout.BeginHorizontal("backgroundBoxControl", GUILayout.Height(30));
                GUILayout.Space(5);
                var selected = ControlDefinition == control ? " Press Key " : control.KeyString;
                GUILayout.Label(control.VisibleName, GUILayout.Width(120), GUILayout.Height(30));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(selected, "genericButton", GUILayout.Width(100), GUILayout.MaxHeight(30)))
                {
                    GUI.FocusControl("");
                    if (ControlDefinition == null)
                    {
                        SetCurrentKey(control, false);
                    }
                    else
                    {
                        if (Event.current.button == 0)
                        {
                            ControlDefinition.Key = KeyCode.None;
                            ControlDefinition.MouseButton = 0;
                            ControlDefinition.HasModifier = false;
                            ControlDefinition.IsShift = false;
                            ControlDefinition.ModifierKey = EventModifiers.None;
                            ControlDefinition = null;
                        }
                        else if (Event.current.button == 1)
                        {
                            ControlDefinition.Key = KeyCode.None;
                            ControlDefinition.MouseButton = 1;
                            ControlDefinition.HasModifier = false;
                            ControlDefinition.IsShift = false;
                            ControlDefinition.ModifierKey = EventModifiers.None;
                            ControlDefinition = null;
                        }
                        else if (Event.current.button == 2)
                        {
                            ControlDefinition.Key = KeyCode.None;
                            ControlDefinition.MouseButton = 2;
                            ControlDefinition.HasModifier = false;
                            ControlDefinition.IsShift = false;
                            ControlDefinition.ModifierKey = EventModifiers.None;
                            ControlDefinition = null;
                        }
                        else
                        {
                            RestoreCurrentKey(ChangingAlt);
                        }
                    }

                }
                GUILayout.EndHorizontal();
            }
            #endregion


            RPGMakerGUI.SubTitle("Default Controls");
            EditorGUILayout.HelpBox("To assign \"Shift\" press Shift+Escape. This is due to a UnityUI issue. Press ESC to clear key.", MessageType.Info);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("Add New Control Definition","genericButton"))
            {
                var control = new ControlDefinition("MyControl") {IsRequiredControl = false};
                control.IsRequiredControl = false;
                MyDefinitions.Add(control);
            }
            //if(GUILayout.Button("Add Gamepad Controls","genericButton"))
            //{
            //    UnityGamepadHandler.AddGamepadControls();
            //}
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("backgroundBoxControl", GUILayout.Height(30));
            GUILayout.Space(20);
            GUILayout.Label("Control ID:", GUILayout.Width(120), GUILayout.Height(30));
            GUILayout.Label("Visible Name In-Game:", GUILayout.Width(200), GUILayout.Height(30));
            GUILayout.Label("Action (and Parameter):", GUILayout.Width(200), GUILayout.Height(30));
            GUILayout.FlexibleSpace();
            GUILayout.Label("Key Combo", GUILayout.Width(100), GUILayout.Height(30));
            GUILayout.EndHorizontal();

            for (int index = 0; index < MyDefinitions.Count; index++)
            {
                GUILayout.BeginVertical("foldoutBox");
                var control = MyDefinitions[index];
                GUILayout.Space(5);

                control.Enabled = EditorGUILayout.BeginToggleGroup("Enable [" + control.ID + "]", control.Enabled);
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal("backgroundBoxControl", GUILayout.Height(30));
                GUILayout.Space(5);
                var selected = ControlDefinition == control && !ChangingAlt ? " Press Key " : control.KeyString;
                var selectedAlt = ControlDefinition == control && ChangingAlt ? " Press Key " : control.AltKeyString;
                GUILayout.Label(control.ID, GUILayout.Width(120), GUILayout.Height(30));
                control.VisibleName = GUILayout.TextField(control.VisibleName, GUILayout.Width(200));

                GUILayout.FlexibleSpace();
                if (!control.IsRequiredControl)
                {
                    GUILayout.BeginHorizontal();
                    control.CustomAction = (CustomControlAction)RPGMakerGUI.EnumPopup(control.CustomAction);
                    control.ID = control.VisibleName;

                    if (control.CustomAction == CustomControlAction.Begin_Event)
                    {
                        RPGMakerGUI.PopupID<NodeChain>("", ref control.StringParameter);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(selected, "genericButton", GUILayout.Width(100), GUILayout.MaxHeight(30)))
                {
                    GUI.FocusControl("");
                    if (ControlDefinition == null)
                    {
                        SetCurrentKey(control,false);
                    }
                    else
                    {
                        if(Event.current.button == 0)
                        {
                            ControlDefinition.Key = KeyCode.None;
                            ControlDefinition.MouseButton = 0;
                            ControlDefinition.HasModifier = false;
                            ControlDefinition.IsShift = false;
                            ControlDefinition.ModifierKey = EventModifiers.None;
                            ControlDefinition = null;
                        }
                        else if (Event.current.button == 1)
                        {
                            ControlDefinition.Key = KeyCode.None;
                            ControlDefinition.MouseButton = 1;
                            ControlDefinition.HasModifier = false;
                            ControlDefinition.IsShift = false;
                            ControlDefinition.ModifierKey = EventModifiers.None;
                            ControlDefinition = null;
                        }
                        else if(Event.current.button == 2)
                        {
                            ControlDefinition.Key = KeyCode.None;
                            ControlDefinition.MouseButton = 2;
                            ControlDefinition.HasModifier = false;
                            ControlDefinition.IsShift = false;
                            ControlDefinition.ModifierKey = EventModifiers.None;
                            ControlDefinition = null;
                        }
                        else
                        {
                            RestoreCurrentKey(ChangingAlt);
                        }
                    }
                    
                }
                if (GUILayout.Button(selectedAlt, "genericButton", GUILayout.Width(100), GUILayout.MaxHeight(30)))
                {
                    GUI.FocusControl("");
                    if (ControlDefinition == null)
                    {
                        SetCurrentKey(control,true);
                    }
                    else
                    {
                        if(Event.current.button == 0)
                        {
                            ControlDefinition.KeyAlt = KeyCode.None;
                            ControlDefinition.MouseButtonAlt = 0;
                            ControlDefinition.HasModifierAlt = false;
                            ControlDefinition.IsShiftAlt = false;
                            ControlDefinition.ModifierKeyAlt = EventModifiers.None;
                            ControlDefinition = null;
                        }
                        else if (Event.current.button == 1)
                        {
                            ControlDefinition.KeyAlt = KeyCode.None;
                            ControlDefinition.MouseButtonAlt = 1;
                            ControlDefinition.HasModifierAlt = false;
                            ControlDefinition.IsShiftAlt = false;
                            ControlDefinition.ModifierKeyAlt = EventModifiers.None;
                            ControlDefinition = null;
                        }
                        else if(Event.current.button == 2)
                        {
                            ControlDefinition.KeyAlt = KeyCode.None;
                            ControlDefinition.MouseButtonAlt = 2;
                            ControlDefinition.HasModifierAlt = false;
                            ControlDefinition.IsShiftAlt = false;
                            ControlDefinition.ModifierKeyAlt = EventModifiers.None;
                            ControlDefinition = null;
                        }
                        else
                        {
                            RestoreCurrentKey(ChangingAlt);
                        }
                    }
                    
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (!control.IsRequiredControl)
                {
                    if (GUILayout.Button("Delete", "genericButton", GUILayout.Height(20), GUILayout.Width(70)))
                    {
                        MyDefinitions.Remove(control);
                        index--;
                        return;
                    }
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
                EditorGUILayout.EndToggleGroup();
                GUILayout.EndVertical();
            }
            GUILayout.Space(5);
            GUILayout.BeginHorizontal("backgroundBoxControl", GUILayout.Height(30));
            GUILayout.Space(20);
            GUILayout.Label("Control ID", GUILayout.Width(120), GUILayout.Height(30));
            GUILayout.Label("Visible Name In-Game", GUILayout.Width(200), GUILayout.Height(30));
            GUILayout.Label("Action (and Parameter)", GUILayout.Width(200), GUILayout.Height(30));
            GUILayout.FlexibleSpace();
            GUILayout.Label("Key Combo", GUILayout.Width(100), GUILayout.Height(30));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.EndScrollView();
            GUILayout.EndArea();


            #region KeyCheck

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
                        if(ChangingAlt)
                        {
                            if ((Event.current.modifiers & EventModifiers.Shift) == EventModifiers.Shift)
                            {
                                ControlDefinition.HasModifierAlt = true;
                                ControlDefinition.ModifierKeyAlt = EventModifiers.Shift;
                            }
                            else if ((Event.current.modifiers & EventModifiers.Control) == EventModifiers.Control)
                            {
                                ControlDefinition.HasModifierAlt = true;
                                ControlDefinition.ModifierKeyAlt = EventModifiers.Control;
                            }
                            else if ((Event.current.modifiers & EventModifiers.Alt) == EventModifiers.Alt)
                            {
                                ControlDefinition.HasModifierAlt = true;
                                ControlDefinition.ModifierKeyAlt = EventModifiers.Alt;
                            }
                            else if ((Event.current.modifiers & EventModifiers.Command) == EventModifiers.Command)
                            {
                                ControlDefinition.HasModifierAlt = true;
                                ControlDefinition.ModifierKeyAlt = EventModifiers.Command;
                            }
                            else
                            {
                                ControlDefinition.HasModifierAlt = false;
                            }
                        }
                        else
                        {
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
                        



                    }

                    KeyCode[] keyCode = new KeyCode[] { KeyCode.Escape, KeyCode.None };

                    if (keyCode.All(k => evt.keyCode != k))
                    {
                        if(ChangingAlt)
                        {
                            ControlDefinition.KeyAlt = Event.current.keyCode;
                        }
                        else
                        {
                            ControlDefinition.Key = Event.current.keyCode;    
                        }
                        
                        GotShift = "";
                        GotShiftSolo = false;
                        evt.Use();
                    }

                    else if (evt.keyCode == KeyCode.Escape)
                    {
                        if (Event.current.modifiers > 0)
                        {
                            if ((Event.current.modifiers & EventModifiers.Shift) == EventModifiers.Shift)
                            {
                                if (ChangingAlt)
                                {
                                    ControlDefinition.KeyAlt = KeyCode.LeftShift;
                                    ControlDefinition.HasModifierAlt = false;
                                    ControlDefinition.IsShiftAlt = true;
                                    ControlDefinition.ModifierKeyAlt = EventModifiers.CapsLock;
                                }
                                else
                                {
                                    ControlDefinition.Key = KeyCode.LeftShift;
                                    ControlDefinition.HasModifier = false;
                                    ControlDefinition.IsShift = true;
                                    ControlDefinition.ModifierKey = EventModifiers.CapsLock;
                                }



                                ControlDefinition sameKey = null;
                                var clearAlt = false;

                                if (ChangingAlt)
                                {
                                    sameKey = MyDefinitions.FirstOrDefault(c => c.HasModifierAlt == ControlDefinition.HasModifierAlt &&
                                                                                c.ModifierKeyAlt == ControlDefinition.ModifierKeyAlt &&
                                                                                c.KeyAlt == ControlDefinition.KeyAlt &&
                                                                                c.IsShiftAlt == ControlDefinition.IsShiftAlt &&
                                                                                c.ID != ControlDefinition.ID);
                                    if (sameKey != null)
                                    {
                                        clearAlt = true;
                                    }

                                    if (sameKey == null)
                                    {

                                        sameKey = MyDefinitions.FirstOrDefault(c => c.HasModifier == ControlDefinition.HasModifierAlt &&
                                                                                   c.ModifierKey == ControlDefinition.ModifierKeyAlt &&
                                                                                   c.Key == ControlDefinition.KeyAlt &&
                                                                                   c.IsShift == ControlDefinition.IsShiftAlt);

                                        if (sameKey != null)
                                        {
                                            clearAlt = false;
                                        }
                                    }
                                }
                                else
                                {
                                    sameKey = MyDefinitions.FirstOrDefault(c => c.HasModifierAlt == ControlDefinition.HasModifier &&
                                                                                c.ModifierKeyAlt == ControlDefinition.ModifierKey &&
                                                                                c.KeyAlt == ControlDefinition.Key &&
                                                                                c.IsShiftAlt == ControlDefinition.IsShift);
                                    if (sameKey != null)
                                    {
                                        clearAlt = true;
                                    }

                                    if (sameKey == null)
                                    {

                                        sameKey = MyDefinitions.FirstOrDefault(c => c.HasModifier == ControlDefinition.HasModifier &&
                                                                                   c.ModifierKey == ControlDefinition.ModifierKey &&
                                                                                   c.Key == ControlDefinition.Key &&
                                                                                   c.IsShift == ControlDefinition.IsShift &&
                                                                                    c.ID != ControlDefinition.ID);

                                        if (sameKey != null)
                                        {
                                            clearAlt = false;
                                        }
                                    }
                                }
                                

                                if (sameKey != null)
                                {
                                    if (clearAlt)
                                    {
                                        sameKey.KeyAlt = KeyCode.None;
                                        sameKey.HasModifierAlt = false;
                                        sameKey.ModifierKeyAlt = EventModifiers.CapsLock;
                                        sameKey.IsShiftAlt = false;
                                    }
                                    else
                                    {
                                        sameKey.Key = KeyCode.None;
                                        sameKey.HasModifier = false;
                                        sameKey.ModifierKey = EventModifiers.CapsLock;
                                        sameKey.IsShift = false;
                                    }
                                    Debug.Log("Unassigned key for [" + sameKey.VisibleName + "]");
                                    
                                }

                                ControlDefinition = null;

                                GotShift = "";
                                GotShiftSolo = false;


                                evt.Use();
                            }
                        }
                        else
                        {
                            RestoreCurrentKey(ChangingAlt);
                            evt.Use();
                        }
                    }

                    GUI.FocusControl("");
                }
                else if ((string.IsNullOrEmpty(GotShift) && GotShiftSolo))
                {
                    //this doesn't work here
                }
                else if (evt.type == EventType.KeyUp)
                {

                    KeyCode[] modKeyCodes = new KeyCode[]
                                            {
                                                KeyCode.LeftShift, KeyCode.RightShift, KeyCode.RightControl,
                                                KeyCode.LeftControl, KeyCode.RightAlt, KeyCode.LeftAlt, KeyCode.AltGr
                                            };
                    if (modKeyCodes.Any(k => k == (ChangingAlt ? ControlDefinition.KeyAlt : ControlDefinition.Key)))
                    {
                        ControlDefinition.HasModifier = false;
                    }

                    GotShift = "";
                    GotShiftSolo = false;

                    ControlDefinition sameKey = null;
                    var clearAlt = false;

                    if (ChangingAlt)
                    {
                        sameKey = MyDefinitions.FirstOrDefault(c => c.HasModifierAlt == ControlDefinition.HasModifierAlt &&
                                                                    c.ModifierKeyAlt == ControlDefinition.ModifierKeyAlt &&
                                                                    c.KeyAlt == ControlDefinition.KeyAlt &&
                                                                    c.IsShiftAlt == ControlDefinition.IsShiftAlt &&
                                                                    c.ID != ControlDefinition.ID);
                        if (sameKey != null)
                        {
                            clearAlt = true;
                        }

                        if (sameKey == null)
                        {

                            sameKey = MyDefinitions.FirstOrDefault(c => c.HasModifier == ControlDefinition.HasModifierAlt &&
                                                                       c.ModifierKey == ControlDefinition.ModifierKeyAlt &&
                                                                       c.Key == ControlDefinition.KeyAlt &&
                                                                       c.IsShift == ControlDefinition.IsShiftAlt);

                            if (sameKey != null)
                            {
                                clearAlt = false;
                            }
                        }
                    }
                    else
                    {
                        sameKey = MyDefinitions.FirstOrDefault(c => c.HasModifierAlt == ControlDefinition.HasModifier &&
                                                                    c.ModifierKeyAlt == ControlDefinition.ModifierKey &&
                                                                    c.KeyAlt == ControlDefinition.Key &&
                                                                    c.IsShiftAlt == ControlDefinition.IsShift);
                        if (sameKey != null)
                        {
                            clearAlt = true;
                        }

                        if (sameKey == null)
                        {

                            sameKey = MyDefinitions.FirstOrDefault(c => c.HasModifier == ControlDefinition.HasModifier &&
                                                                       c.ModifierKey == ControlDefinition.ModifierKey &&
                                                                       c.Key == ControlDefinition.Key &&
                                                                       c.IsShift == ControlDefinition.IsShift &&
                                                                        c.ID != ControlDefinition.ID);

                            if (sameKey != null)
                            {
                                clearAlt = false;
                            }
                        }
                    }


                    if (sameKey != null)
                    {
                        if (clearAlt)
                        {
                            sameKey.KeyAlt = KeyCode.None;
                            sameKey.HasModifierAlt = false;
                            sameKey.ModifierKeyAlt = EventModifiers.CapsLock;
                            sameKey.IsShiftAlt = false;
                        }
                        else
                        {
                            sameKey.Key = KeyCode.None;
                            sameKey.HasModifier = false;
                            sameKey.ModifierKey = EventModifiers.CapsLock;
                            sameKey.IsShift = false;
                        }
                        Debug.Log("Unassigned key for [" + sameKey.VisibleName + "]");

                    }


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

            #endregion

        }

        private static void SetCurrentKey(ControlDefinition c, bool alt)
        {
            if(!alt)
            {
                ControlDefinition = c;
                c.OldKeyCode = c.Key;
                c.OldModifierKey = c.ModifierKey;
                c.OldHasModifier = c.HasModifier;
                c.OldMouseButton = c.MouseButton;

                c.Key = KeyCode.None;
                c.MouseButton = -1;
                c.HasModifier = false;
                c.IsShift = false;
                c.ModifierKey = EventModifiers.CapsLock;
                ChangingAlt = false;
            }
            else
            {
                ControlDefinition = c;
                c.OldKeyCodeAlt = c.KeyAlt;
                c.OldModifierKeyAlt = c.ModifierKeyAlt;
                c.OldHasModifierAlt = c.HasModifierAlt;
                c.OldMouseButtonAlt = c.MouseButtonAlt;

                c.KeyAlt = KeyCode.None;
                c.MouseButtonAlt = -1;
                c.HasModifierAlt = false;
                c.IsShiftAlt = false;
                c.ModifierKeyAlt = EventModifiers.CapsLock;
                ChangingAlt = true;
            }
        }

        private static void RestoreCurrentKey(bool alt)
        {
            if (ControlDefinition == null) return;

            if(!alt)
            {
                ControlDefinition.Key = ControlDefinition.OldKeyCode;
                ControlDefinition.HasModifier = ControlDefinition.OldHasModifier;
                ControlDefinition.ModifierKey = ControlDefinition.OldModifierKey;
                ControlDefinition.IsShift = ControlDefinition.OldIsShift;
                ControlDefinition.MouseButton = ControlDefinition.OldMouseButton;
                ControlDefinition = null;
            }
            else
            {
                ControlDefinition.KeyAlt = ControlDefinition.OldKeyCodeAlt;
                ControlDefinition.HasModifierAlt = ControlDefinition.OldHasModifierAlt;
                ControlDefinition.ModifierKeyAlt = ControlDefinition.OldModifierKeyAlt;
                ControlDefinition.IsShiftAlt = ControlDefinition.OldIsShiftAlt;
                ControlDefinition.MouseButtonAlt = ControlDefinition.OldMouseButtonAlt;
                ControlDefinition = null;
            }
        }
        private static void ClearCurrentKey(bool alt)
        {
            if (ControlDefinition == null) return;

            if (alt)
            {
                ControlDefinition.KeyAlt = KeyCode.None;
                ControlDefinition.HasModifierAlt = false;
                ControlDefinition.ModifierKeyAlt = EventModifiers.CapsLock;
                ControlDefinition.IsShiftAlt = false;
            }
            else
            {
                ControlDefinition.Key = KeyCode.None;
                ControlDefinition.HasModifier = false;
                ControlDefinition.ModifierKey = EventModifiers.CapsLock;
                ControlDefinition.IsShift = false;
            }

            ControlDefinition = null;
        }

        #endregion

        #region Save Options

        private static Rmh_Customise CustomiseSave
        {
            get { return Rm_RPGHandler.Instance.Customise; }
        }

        private static bool toggleSavableOptions;

        public static void SaveOptions(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            GUI.Box(fullArea, "","backgroundBox");

            GUILayout.BeginArea(fullArea);
            RPGMakerGUI.Title("Main - Save Options");

            CustomiseSave.SaveInMenu = RPGMakerGUI.Toggle("Save option in menu?", CustomiseSave.SaveInMenu);
            if (!CustomiseSave.SaveInMenu)
            {
                EditorGUILayout.HelpBox(
                    "You will need to add checkpoints in your game or an interactable save object in order to save player's progress.",
                    MessageType.Info);
            }

            CustomiseSave.SaveOnSceneSwitch = RPGMakerGUI.Toggle("Save on scene switch?",
                                                                     CustomiseSave.SaveOnSceneSwitch,
                                                                     GUILayout.Width(600));
#if (!UNITY_WEBPLAYER)
            var checkIfFalse = CustomiseSave.PersistGameObjectInfo;
            CustomiseSave.PersistGameObjectInfo = RPGMakerGUI.Toggle("Savable GameObjects?",
                                                                         CustomiseSave.PersistGameObjectInfo,
                                                                         GUILayout.Width(600));

            if (!checkIfFalse && CustomiseSave.PersistGameObjectInfo)
            {

                CustomiseSave.SaveEnemyStatus = true;
                CustomiseSave.SaveGameObjectPosition = true;
                CustomiseSave.SaveGameObjectRotation = true;
                CustomiseSave.SaveGameObjectDestroyed = true;
                CustomiseSave.SaveGameObjectEnabled = true;
            }

            if (CustomiseSave.PersistGameObjectInfo)
            {
                toggleSavableOptions = EditorGUILayout.Foldout(toggleSavableOptions, "Savable Object Options:");
                if (toggleSavableOptions)
                {
                    CustomiseSave.SaveEnemyStatus = RPGMakerGUI.Toggle("Save Enemy Status?",
                                                                           CustomiseSave.SaveEnemyStatus,
                                                                           GUILayout.Width(600));
                    EditorGUILayout.HelpBox(
                        "This will save stats such as Enemy HP, Skill Cooldowns, Buffs/Debuffs etc.", MessageType.Info);

                    CustomiseSave.SaveGameObjectPosition = RPGMakerGUI.Toggle("Save Position?",
                                                                                  CustomiseSave.SaveGameObjectPosition,
                                                                                  GUILayout.Width(600));
                    CustomiseSave.SaveGameObjectRotation = RPGMakerGUI.Toggle("Save Rotation?",
                                                                                  CustomiseSave.SaveGameObjectRotation,
                                                                                  GUILayout.Width(600));
                    CustomiseSave.SaveGameObjectDestroyed = RPGMakerGUI.Toggle("Save Is Destroyed?",
                                                                                   CustomiseSave.SaveGameObjectDestroyed,
                                                                                   GUILayout.Width(600));
                    CustomiseSave.SaveGameObjectEnabled = RPGMakerGUI.Toggle("Save Is Enabled?",
                                                                                 CustomiseSave.SaveGameObjectEnabled,
                                                                                 GUILayout.Width(600));
                }
            }
            else
            {
                CustomiseSave.SaveEnemyStatus = false;
                CustomiseSave.SaveGameObjectPosition = false;
                CustomiseSave.SaveGameObjectRotation = false;
                CustomiseSave.SaveGameObjectDestroyed = false;
                CustomiseSave.SaveGameObjectEnabled = false;
            }
#else
            CustomiseSave.PersistGameObjectInfo = false;
#endif


            GUILayout.EndArea();
        }

        #endregion

        #region CreditsInfo

        private static Rm_CreditsInfo selectedInfo = null;

        public static void Credits(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            var list = Rm_RPGHandler.Instance.GameInfo.CreditsInfo;
            GUI.Box(leftArea, "","backgroundBox");
            GUI.Box(mainArea, "","backgroundBox");

            GUILayout.BeginArea(PadRect(leftArea, 0, 0));
                RPGMakerGUI.ListArea(list, ref selectedInfo,Rm_ListAreaType.CreditsInfo,false,false);
            GUILayout.EndArea();


            GUILayout.BeginArea(mainArea);
            RPGMakerGUI.Title("Credits");
            if (selectedInfo != null)
            {
                var oldtype = selectedInfo.Type;
                selectedInfo.Type = (CreditsType) RPGMakerGUI.EnumPopup("Type:", selectedInfo.Type);
                if(selectedInfo.Type != oldtype) GUI.FocusControl("");

                if (selectedInfo.Type == CreditsType.Logo)
                {
                    selectedInfo.Image = RPGMakerGUI.ImageSelector("Image:", selectedInfo.Image, ref selectedInfo.ImagePath);
                }
                else if (selectedInfo.Type == CreditsType.Name)
                {
                    selectedInfo.Name = RPGMakerGUI.TextField("Name: ", selectedInfo.Name);
                }
                else if (selectedInfo.Type == CreditsType.RoleTitle)
                {
                    selectedInfo.Role = RPGMakerGUI.TextField("Role:", selectedInfo.Role);
                }
                else if (selectedInfo.Type == CreditsType.Text)
                {
                    selectedInfo.Text = RPGMakerGUI.TextArea("Text:",selectedInfo.Text);
                }
                else if (selectedInfo.Type == CreditsType.Title)
                {
                    selectedInfo.Title = RPGMakerGUI.TextField("Title" ,selectedInfo.Title);
                }
                else if (selectedInfo.Type == CreditsType.Space)
                {
                    selectedInfo.Space = RPGMakerGUI.IntField("Space (pixels):", selectedInfo.Space);
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Add or select a new field to customise credits.", MessageType.Info);
            }
            GUILayout.EndArea();
        }

        #endregion

        #region Preferences

        public static Rmh_Preferences Prefs
        {
            get { return Rm_RPGHandler.Instance.Preferences; }
        }

        public static void Preferences(Rect fullArea, Rect leftBar, Rect mainArea)
        {
            GUI.Box(fullArea, "","backgroundBox");

            GUILayout.BeginArea(fullArea);
            RPGMakerGUI.Title("Main - Preferences");
            if(RPGMakerGUI.Toggle("Enable Auto Save?", ref Prefs.EnableAutoSave))
            {
                Prefs.AutoSaveFrequency = RPGMakerGUI.FloatField("Time Between Autosaves (s)", Prefs.AutoSaveFrequency);
                Prefs.AutoSaveFrequency = Mathf.Max(Prefs.AutoSaveFrequency, 30);
            }
            if (RPGMakerGUI.Toggle("Backup Game Data on Save?", ref Prefs.EnableBackupOnSave))
            {
                RPGMakerGUI.Toggle("One Backup Per Day?", ref Prefs.OneBackupPerDay);
            }
            GUILayout.EndArea();
        }

        #endregion

        public static Rect PadRect(Rect rect, int left, int top)
        {
            return new Rect(rect.x + left, rect.y + top, rect.width - (left * 2), rect.height - (top * 2)); 
        }
    }
}
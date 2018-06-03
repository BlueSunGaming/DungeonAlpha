using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Editor;
using LogicSpawn.RPGMaker.Editor.New;
using UnityEngine;
using UnityEditor;


    public class Rme_Main : EditorWindow
    {
        public const string RpgVersion = "RPG All-In-One v1.5.2";
        public const string PatchNoteUrl = "http://rpgaio.logicspawn.co.uk/forums/showthread.php?tid=410";

        public const string PatchInfo = "New in v1.5.2: Subtitles and NPC Improvements!";

        public const string PatchNotes = RpgVersion + "\n" + "\n" +
                                                "Version 1.5.2" + "\n" +
                                                "- On Screen Text and Subtitle System:" + "\n" + 
                                                "  - You can now show on screen text during dialog, events and cutscenes" + "\n" + 
                                                "  - Subtitles can be accompanied with audio" + "\n" + 
                                                "  - Text duration can be auto-set based on audio length" + "\n" + 
                                                "  - Added node: Wait for End of Subtitles, to delay events/dialog until subtitles finish" + "\n" + 
                                                "- NPC Improvements:" + "\n" + 
                                                "  - Can now trigger a dialog with events and event triggers (e.g. when you get close to an NPC)" + "\n" + 
                                                "  - NPC can now walk to a position through an event" + "\n" + 
                                                "  Draggable Items:" + "\n" + 
                                                "  - Added a basic 'Draggable Object' script which will allow you to drag an object around" + "\n" + 
                                                "  - Added a new cursor to highlight draggable items (set your draggable items to have the tag 'Draggable')" + "\n" + 
                                                "  - Use this with the new option to trigger events with gameObjects to create new interactions/puzzles (e.g. move a key to a trigger to open a door)" + "\n" + 
                                                " - Misc New Features:" + "\n" + 
                                                "  - Visual customisations can now have a default option" + "\n" + 
                                                "  - When books drop, you can now view/listen to them before picking them up" + "\n" + 
                                                "  - Harvesting timer now syncs up correctly" + "\n" + 
                                                "  - Added nodes: Give Player Item (to inventory), Player Has Item, Remove Player Item" + "\n" + 
                                                "  - When books drop, you can now view/listen to them before picking them up" + "\n" +
                                                "  - Event triggers can now be triggered by gameObjects" + "\n" + 
                                                " - Bug Fixes and Changes" + "\n" +
                                                "  - Fixed a bug that allowed you to harvest quest items when quest was not active" + "\n" + 
                                                "  - Spawning items via events will now allow you to specify a quantity" + "\n" + 
                                                "  - Using vitals like 'hunger' which reduce health when below 0 will no longer reduce health below 0" + "\n" +
                                                "  - Can now allow event triggers to be triggered again when the player is further than a specified distance from the trigger" + "\n" + 
                                                
                                         "Check the full patch notes at rpgaio.logicspawn.co.uk/forums/ for more info" + "\n" + 
                                         "" + "\n"; 



        public static Rme_Main Window;
        public static CurrentPage[] Pages = Enum.GetValues(typeof(CurrentPage)) as CurrentPage[];
        public CurrentPage CurrentPage = CurrentPage.Main;
        public CurrentPage CurrentSubMenu = CurrentPage.Main;
        public int CurrentPageIndex = 0;
        
        public string[] MainPages = new[] { "Home", "Preferences" };
        public string[] GamePages = new[] { "Options", "Game Info", "Settings and Controls", "Save Options", "Credits", "Global Playlist", "Minimap", "World Map" };
        public string[] StatsPages = new[] { "Options", "Attributes", "Vitals", "Statistics", "Traits", "Experience" };
        public string[] PlayerPages = new[] {"Options","Class-Names","Races","Sub-Races", "Meta-Datas", "Characters","Genders","Pets"};
        public string[] EnemiesPages = new [] {"Options","Enemies"};
        public string[] NPCPages = new[] {"NPCs","Vendor Shops", "Reputations"};
        public string[] ItemsPages = new[] {"Options","Item DB","Craftable Items", "Tiers","Craft Lists","Dismantling","Quest Items", "Loot-Tables", "Costume Designer"};
        public string[] CombatPages = new[] {"Options","Skills","Talents", "Talent Groups", "Status Effects"};
        public string[] ObjectivesPages = new[] {"Options","Quest Chains","Quests"};
        public string[] InteractablesPages = new[] { "Options", "Interactable Objects", "Harvestable Objects" };
        public string[] CustomPages = new[] {"Custom Variables"};

        public static bool ShowSubMenu = false;
        private static bool _loadedLatestData;  
        //todo: last saved at label

        // Add menu named "My Window" to the Window menu
        [MenuItem("Tools/LogicSpawn RPG All In One/RPGAIO", false, 0)]
        public static void Init()
        {
            // Get existing open window or if none, make a new one:
            var window = (Rme_Main) GetWindow(typeof (Rme_Main));
            //window.maxSize = new Vector2(1050, 500);
            window.titleContent = new GUIContent("RPG AIO");
            window.minSize = new Vector2(950.1F, 530.1F);
            window.position = new Rect(200, 200, 950.1F, 530.1F);
            Window = window;
            Pages = Enum.GetValues(typeof (CurrentPage)) as CurrentPage[];
            EditorGameDataSaveLoad.LoadIfNotLoadedFromEditor();
        }

        // Add menu named "My Window" to the Window menu
        [MenuItem("Tools/LogicSpawn RPG All In One/Tools/Data/New Game Data", false, 555)]
        public static void NewGameData()
        {
            EditorGameDataSaveLoad.NewData();
            GameSettingsSaveLoadManager.Instance.NewData();
            if (CombatNodeWindow.Window != null)
            {
                CombatNodeWindow.Window.Close();
            }
            if (DialogNodeWindow.Window != null)
            {
                DialogNodeWindow.Window.Close();
            }
            if (EventNodeWindow.Window != null)
            {
                EventNodeWindow.Window.Close();
            }
        }

        void OnGUI()
        {
            try
            {
                OnGUIx();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public string newShaderName;
        private void OnSelectedShaderPopup(string command, Shader shader)
        {
            if (shader != null)
            {
                var shaderInfo = RPGMakerGUI.LastSelectedShaderInfo;
                if(shaderInfo != null)
                {
                    var oldShader = shaderInfo.ShaderName;
                    if (oldShader != shader.name)
                    {
                        InitShader(shaderInfo, shader.name);
                    }
                }
            }
        }



        private void InitShader(ShaderLerpInfo shaderInfo, string shaderName)
        {
            shaderInfo.ShaderName = shaderName;
            shaderInfo.PropsToLerp = new List<ShaderPropLerpInfo>();
            var shader = Shader.Find(shaderName);
            var props = ShaderUtil.GetPropertyCount(shader);

            for (int i = 0; i < props; i++)
            {
                var propName = ShaderUtil.GetPropertyName(shader, i);
                var propDesc = ShaderUtil.GetPropertyDescription(shader, i);
                var type = ShaderUtil.GetPropertyType(shader, i);

                if (!new[] { ShaderUtil.ShaderPropertyType.Color, ShaderUtil.ShaderPropertyType.Float, ShaderUtil.ShaderPropertyType.Range }.Any(s => type == s))
                {
                    continue;
                }

                ShaderType shaderType;
                switch (type)
                {
                    case ShaderUtil.ShaderPropertyType.Color:
                        shaderType = ShaderType.Color;
                        break;
                    case ShaderUtil.ShaderPropertyType.Float:
                        shaderType = ShaderType.Float;
                        break;
                    case ShaderUtil.ShaderPropertyType.Range:
                        shaderType = ShaderType.Range;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                shaderInfo.PropsToLerp.Add(new ShaderPropLerpInfo()
                                    {
                                        PropName = propName,
                                        PropDesc = propDesc,
                                        PropType = shaderType,
                                        OnlyLerpAlpha = true,
                                        LerpTo = 0
                                    });
            }
        }

        private void OnGUIx()
        {
            GUI.skin = null;
            GUI.skin = Resources.Load("RPGMakerAssets/EditorSkinRPGMaker") as GUISkin;

            if (PlayerSettings.colorSpace == ColorSpace.Linear)
            {
                GUI.backgroundColor = new Color(1, 1, 1, 0.3f);
            }
            else
            {
                GUI.backgroundColor = new Color(1, 1, 1, 1);
            }   

            var subMenuHeight = 30;
            var workingArea = new Rect(5, 5, Window.position.width - 10, Window.position.height - 10);
            var topbarArea = new Rect(10, 10, workingArea.width - 10, 30);
            var topbarAreaTwo = new Rect(10, topbarArea.yMax + 5, topbarArea.width, 25);
            if (ShowSubMenu) topbarAreaTwo = new Rect(10, topbarArea.yMax + 5, topbarArea.width, 25 + subMenuHeight);
            var subMenu = new Rect(10, topbarAreaTwo.yMax - subMenuHeight, topbarArea.width, subMenuHeight);

            var bottomAreaHeight = 35;
            var leftBar = new Rect(10, topbarAreaTwo.yMax + 5,
                                   180, Window.position.height - 10 - (topbarAreaTwo.yMax + bottomAreaHeight + 5 + 5));
            var bottomArea = new Rect(10, leftBar.yMax + 5, topbarArea.width, bottomAreaHeight);
            var mainArea = new Rect(leftBar.xMax + 5, leftBar.y, topbarArea.width - (leftBar.width + 5), leftBar.height);
            var fullArea = new Rect(leftBar.x, leftBar.y, topbarArea.width, leftBar.height);

            GUI.Box(workingArea, "","workingAreaBox");
            GUI.Box(bottomArea, "", "backgroundBox");

            GUILayout.BeginArea(PadRect(topbarArea, 0, 0),"","topAreaBackground");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("", "LogicSpawnIcon"))
            {
                Application.OpenURL("http://rpgaio.logicspawn.co.uk/");
            }
            //GUILayout.Label(RpgVersion);
            var isMobile = "";
#if (UNITY_IOS || UNITY_ANDROID)
            isMobile = " - Mobile Mode";
#endif
            if(GUILayout.Button(RpgVersion + isMobile,"Label"))
            {
                Application.OpenURL("http://rpgaio.logicspawn.co.uk/");

            }
            GUILayout.FlexibleSpace();
            GUILayout.Label(Rm_RPGHandler.Instance.GameInfo.GameTitle + " by " + Rm_RPGHandler.Instance.GameInfo.GameCompany);
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndArea();

            GUILayout.BeginArea(PadRect(topbarAreaTwo, 0, 0), "", "backgroundBox");
            GUILayout.BeginHorizontal();
            for (int i = 0; i < Pages.Length; i++)
            {
                if(GUILayout.Button(Pages[i].ToString(),"menuButton"))
                {
                    if (ShowSubMenu && CurrentSubMenu == Pages[i])
                    {
                        ShowSubMenu = false;
                    }
                    else
                    {
                        CurrentSubMenu = Pages[i];
                        ShowSubMenu = true;
                    }
                }
            }
            
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            switch(CurrentPage)
            {
                case CurrentPage.Main:
                    if (CurrentPageIndex == Array.IndexOf(MainPages, "Home"))
                    {
                        Rme_Main_Main.Home(fullArea,leftBar,mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(MainPages, "Preferences"))
                    {
                        Rme_Main_Main.Preferences(fullArea, leftBar, mainArea);
                    }
                    break;
                case CurrentPage.Game:
                    if(CurrentPageIndex == Array.IndexOf(GamePages,"Options"))
                    {
                        Rme_Main_Game.Options(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(GamePages, "Game Info"))
                    {
                        Rme_Main_Main.GameInfo(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(GamePages, "Settings and Controls"))
                    {
                        Rme_Main_Main.GameSettings(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(GamePages, "Save Options"))
                    {
                        Rme_Main_Main.SaveOptions(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(GamePages, "Credits"))
                    {
                        Rme_Main_Main.Credits(fullArea, leftBar, mainArea);
                    }
                    else if(CurrentPageIndex == Array.IndexOf(GamePages,"Global Playlist"))
                    {
                        Rme_Main_Game.GlobalPlaylist(fullArea, leftBar, mainArea);
                    }
                    else if(CurrentPageIndex == Array.IndexOf(GamePages,"Minimap"))
                    {
                        Rme_Main_Main.Minimap(fullArea, leftBar, mainArea);
                    }
                    else if(CurrentPageIndex == Array.IndexOf(GamePages,"World Map"))
                    {
                        Rme_Main_Game.WorldMap(fullArea, leftBar, mainArea);
                    }
                    break;
                case CurrentPage.Stats:
                    if (CurrentPageIndex == Array.IndexOf(StatsPages, "Options"))
                    {
                        Rme_Main_Stats.Options(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(StatsPages, "Attributes"))
                    {
                        Rme_Main_Stats.Attributes(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(StatsPages, "Vitals"))
                    {
                        Rme_Main_Stats.Vitals(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(StatsPages, "Statistics"))
                    {
                        Rme_Main_Stats.Statistics(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(StatsPages, "Traits"))
                    {
                        Rme_Main_Stats.Traits(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(StatsPages, "Experience"))
                    {
                        Rme_Main_Player.Experience(fullArea, leftBar, mainArea);
                    }
                    break;
                case CurrentPage.Player:
                    if (CurrentPageIndex == Array.IndexOf(PlayerPages, "Options"))
                    {
                        Rme_Main_Player.Options(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(PlayerPages, "Class-Names"))
                    {
                        Rme_Main_Player.ClassNames(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(PlayerPages, "Races"))
                    {
                        Rme_Main_Player.Races(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(PlayerPages, "Sub-Races"))
                    {
                        Rme_Main_Player.SubRaces(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(PlayerPages, "Meta-Datas"))
                    {
                        Rme_Main_Player.MetaDatas(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(PlayerPages, "Characters"))
                    {
                        Rme_Main_Player.Classes(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(PlayerPages, "Genders"))
                    {
                        Rme_Main_Player.Genders(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(PlayerPages, "Pets"))
                    {
                        Rme_Main_Player.Pets(fullArea, leftBar, mainArea);
                    }
                    break;
                case CurrentPage.Enemies:
                    if (CurrentPageIndex == Array.IndexOf(EnemiesPages, "Options"))
                    {
                        Rme_Main_Enemies.Options(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(EnemiesPages, "Enemies"))
                    {
                        Rme_Main_Enemies.Enemies(fullArea, leftBar, mainArea);
                    }
                    break;
                case CurrentPage.NPC:
                    if (CurrentPageIndex == Array.IndexOf(NPCPages, "NPCs"))
                    {
                        Rme_Main_NPC.NPCs(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(NPCPages, "Vendor Shops"))
                    {
                        Rme_Main_NPC.VendorShops(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(NPCPages, "Reputations"))
                    {
                        Rme_Main_NPC.Reputations(fullArea, leftBar, mainArea);
                    }
                    break;
                case CurrentPage.Items:
                    if (CurrentPageIndex == Array.IndexOf(ItemsPages, "Options"))
                    {
                        Rme_Main_Items.Options(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(ItemsPages, "Item DB"))
                    {
                        Rme_Main_Items.ItemDB(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(ItemsPages, "Craftable Items"))
                    {
                        Rme_Main_Items.CraftableItems(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(ItemsPages, "Craft Lists"))
                    {
                        Rme_Main_Items.CraftLists(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(ItemsPages, "Dismantling"))
                    {
                        Rme_Main_Items.Dismantling(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(ItemsPages, "Quest Items"))
                    {
                        Rme_Main_Items.QuestItems(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(ItemsPages, "Loot-Tables"))
                    {
                        Rme_Main_Items.LootTables(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(ItemsPages, "Costume Designer"))
                    {
                        Rme_Main_Items_CostumeDesigner.Main(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(ItemsPages, "Tiers"))
                    {
                        Rme_Main_Items.Tiers(fullArea, leftBar, mainArea);
                    }
                    break;
                case CurrentPage.Combat:
                    if (CurrentPageIndex == Array.IndexOf(CombatPages, "Options"))
                    {
                        Rme_Main_Combat.Options(fullArea, leftBar, mainArea, Window);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(CombatPages, "Skills"))
                    {
                        Rme_Main_Combat.Skills(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(CombatPages, "Talents"))
                    {
                        Rme_Main_Combat.Talents(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(CombatPages, "Talent Groups"))
                    {
                        Rme_Main_Combat.TalentGroups(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(CombatPages, "Status Effects"))
                    {
                        Rme_Main_Combat.StatusEffects(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(CombatPages, "Visualiser"))
                    {
                        Rme_Main_Combat_SkillVisualiser.Main(fullArea, leftBar, mainArea, Window);
                    }
                    break;
                case CurrentPage.Objectives:
                    if (CurrentPageIndex == Array.IndexOf(ObjectivesPages, "Quest Chains"))
                    {
                        Rme_Main_Questing.QuestChains(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(ObjectivesPages, "Options"))
                    {
                        Rme_Main_Questing.Options(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(ObjectivesPages, "Quests"))
                    {
                        Rme_Main_Questing.Quests(fullArea, leftBar, mainArea);
                    }
                    break;
                case CurrentPage.Interactables:
                    if (CurrentPageIndex == Array.IndexOf(InteractablesPages, "Options"))
                    {
                        Rme_Main_Interactables.Options(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(InteractablesPages, "Interactable Objects"))
                    {
                        Rme_Main_Interactables.InteractableObjects(fullArea, leftBar, mainArea);
                    }
                    else if (CurrentPageIndex == Array.IndexOf(InteractablesPages, "Harvestable Objects"))
                    {
                        Rme_Main_Interactables.HarvestableObjects(fullArea, leftBar, mainArea);
                    }
                    break;
                case CurrentPage.Custom:
                    if (CurrentPageIndex == Array.IndexOf(CustomPages, "Custom Variables"))
                    {
                        Rme_Main_Custom.CustomVariables(fullArea, leftBar, mainArea);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (ShowSubMenu)
            {
                GUI.Box(subMenu, "");
                GUILayout.BeginArea(PadRect(subMenu, 0, 0),"","subMenuBackground");

                String[] arrayToUse;
                switch (CurrentSubMenu)
                {
                    case CurrentPage.Main:
                        arrayToUse = MainPages;
                        break;
                    case CurrentPage.Game:
                        arrayToUse = GamePages;
                        break;
                    case CurrentPage.Stats:
                        arrayToUse = StatsPages;
                        break;
                    case CurrentPage.Player:
                        arrayToUse = PlayerPages;
                        break;
                    case CurrentPage.Enemies:
                        arrayToUse = EnemiesPages;
                        break;
                    case CurrentPage.NPC:
                        arrayToUse = NPCPages;
                        break;
                    case CurrentPage.Items:
                        arrayToUse = ItemsPages;
                        break;
                    case CurrentPage.Combat:
                        arrayToUse = CombatPages;
                        break;
                    case CurrentPage.Objectives:
                        arrayToUse = ObjectivesPages;
                        break;
                    case CurrentPage.Interactables:
                        arrayToUse = InteractablesPages;
                        break;
                    case CurrentPage.Custom:
                        arrayToUse = CustomPages;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                for (int i = 0; i < arrayToUse.Length; i++)
                {
                    if (GUILayout.Button(arrayToUse[i],"subMenuButton"))
                    {
                        CurrentPageIndex = Array.IndexOf(arrayToUse, arrayToUse[i]);
                        CurrentPage = CurrentSubMenu;
                        ShowSubMenu = false;
                        GUI.FocusControl("");
                        RPGMakerGUI.ResetScrollPositions();
                    }
                }
                GUILayout.FlexibleSpace();  
                GUILayout.EndHorizontal();

                GUILayout.EndArea();
            }

            GUILayout.BeginArea(PadRect(bottomArea, 5, 5));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if(GUILayout.Button("Save Data","genericButton"))
            {
                EditorGameDataSaveLoad.SaveGameData();
                GameSettingsSaveLoadManager.Instance.SaveSettings();
            }
            if (GUILayout.Button("Reload Data", "genericButton"))
            {
                Rm_RPGHandler.Instance = null;
                EditorGameDataSaveLoad.LoadGameDataFromEditor();
                GameSettingsSaveLoadManager.Instance.LoadSettings();
                AssetDatabase.Refresh();
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();


        }

        public void Update()
        {
            //note: updates editor when new gameobject selections are made etc
            this.Repaint();
        }

        

        public static bool ValidateImagePath(Texture2D textureToValidate, ref string pathToSet )
        {
            if (textureToValidate)
            {
                var fullPath = AssetDatabase.GetAssetPath(textureToValidate);
                if (!fullPath.StartsWith("Assets/Resources/"))
                {
                    Debug.LogError("Icons must be in Assets/Resources/*");
                    pathToSet = "";
                    return false;
                }
                else
                {
                    pathToSet = fullPath.Replace("Assets/Resources/", "").Replace(Path.GetExtension(fullPath), "");
                    return true;
                }
            }

            return false;
        }

        public Rect PadRect(Rect rect, int left, int top)
        {
            return new Rect(rect.x + left, rect.y + top, rect.width - (left*2), rect.height - (top*2));
        }

        public void OnEnable()
        {
            Window = this;
            EditorGameDataSaveLoad.LoadIfNotLoadedFromEditor();
        }

        void OnDisable()
        {
            var option = EditorUtility.DisplayDialogComplex("Save Changes?", "Would you like to save changes to the Game Data? \n\n Note: Discarding will reload the previous Game Data",
                                                "Save", "Close without Saving", "Discard");

            if (option == 0)
            {
                EditorGameDataSaveLoad.SaveGameData();
            }
            else if (option == 2)
            {
                EditorGameDataSaveLoad.LoadGameDataFromEditor();
            }
        }


        public static bool ValidateAudioPath(AudioClip music, ref string musicPath)
        {
            if (music)
            {
                var fullPath = AssetDatabase.GetAssetPath(music);
                if (!fullPath.StartsWith("Assets/Resources/"))
                {
                    Debug.LogError("Music file must be in Assets/Resources/*");
                    musicPath = "";
                    return false;
                }
                else
                {
                    musicPath = fullPath.Replace("Assets/Resources/", "").Replace(Path.GetExtension(fullPath), "");
                    return true;
                }
            }

            return false;

        }

        public static bool ValidatePath(UnityEngine.Object music, ref string musicPath)
        {
            if (music != null)
            {
                var fullPath = AssetDatabase.GetAssetPath(music);
                if (!fullPath.StartsWith("Assets/Resources/"))
                {
                    Debug.LogError("File must be in Assets/Resources/*");
                    musicPath = "";
                    return false;
                }
                else
                {
                    musicPath = fullPath.Replace("Assets/Resources/", "").Replace(Path.GetExtension(fullPath), "");
                    return true;
                }
            }

            return false;

        }
    }
    public enum CurrentPage
    {
        Main, //GameInfo, GameOptions,
        Game, //world map,
        Stats, //asvt
        Player, //exp curves
        Enemies, //exp curves
        NPC, //NPC repo, Vendor repo,
        Items, //Item repositories, Crafting, loot tables
        Combat, //Skills, ranges , vitalHandler
        Objectives, //Quests, Achievements
        Interactables, //Harvesting, Repo interactables
        Custom //custom data and events
    }
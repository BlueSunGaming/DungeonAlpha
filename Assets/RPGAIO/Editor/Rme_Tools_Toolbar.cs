using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Assets.Scripts.Beta.NewImplementation;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Editor.New;
using LogicSpawn.RPGMaker.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace LogicSpawn.RPGMaker.Editor
{
    public class Rme_Tools_Toolbar : EditorWindow
    {

        private static Rme_Tools_Toolbar Window;

        private Texture2D PrefabBrowserIcon;
        private Texture2D SaveIcon;
        private Texture2D LoadIcon;
        private Texture2D NewSceneIcon;
        private Texture2D EventTriggerIcon;
        private Texture2D LevelSwitchIcon;
        private Texture2D PopupTextIcon;
        private Texture2D CombatIcon;
        private Texture2D DialogIcon;
        private Texture2D EventIcon;
        private Texture2D AchievementIcon;
        private Texture2D MapIcon;

        [MenuItem("Tools/LogicSpawn RPG All In One/Tools/Toolbar", false, 3)]
        private static void Init()
        {
            // Get existing open window or if none, make a new one:
            Window = (Rme_Tools_Toolbar)GetWindow(typeof(Rme_Tools_Toolbar));
            Window.maxSize = new Vector2(2000, 2000);
            Window.titleContent = new GUIContent("RPGAIOToolbar");
            Window.minSize = new Vector2(80.1F, 80.1F);
            Window.position = new Rect(100, 100, 1100, 80);
        }

        [MenuItem("Tools/LogicSpawn RPG All In One/Tools/Add World Loot Item", false, 2)]
        private static void WorldLoot()
        {
            AddWorldLoot();
        }

        //[MenuItem("Tools/LogicSpawn RPG All In One/Fix Issue (Dev)", false, 2)]
        //private static void FixIssue()
        //{
        //    Debug.Log(Path.Combine(Application.persistentDataPath,
        //                           @"Saves\"));
        //}

        [MenuItem("Tools/LogicSpawn RPG All In One/Tools/Create New Scene", false, 3)]
        private static void CreateNewScene()
        {
            AddScene();
        }

        [MenuItem("Tools/LogicSpawn RPG All In One/Tools/Add Scene Essentials", false, 4)]
        private static void AddSceneEssentialsMenu()
        {
            AddSceneEssentials();
        }

        [MenuItem("Tools/LogicSpawn RPG All In One/Play Intro Scene", false,4)]
        private static void OpenMainScene()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();    
            EditorSceneManager.OpenScene("Assets/Scenes/CoreScenes/Intro.unity");
            EditorApplication.isPlaying = true;
        }

        [MenuItem("Tools/LogicSpawn RPG All In One/Tools/Cheats/Add 10 Exp", false, 6)]
        private static void Add10Exp()
        {
            GetObject.PlayerCharacter.AddExp(10);
        }

        [MenuItem("Tools/LogicSpawn RPG All In One/Tools/Cheats/Add 100 Exp", false, 6)]
        private static void Add100Exp()
        {
            GetObject.PlayerCharacter.AddExp(100);
        }

        [MenuItem("Tools/LogicSpawn RPG All In One/Tools/Cheats/Add 1000 Exp", false, 6)]
        private static void Add1000Exp()
        {
            GetObject.PlayerCharacter.AddExp(1000);
        }

        [MenuItem("Tools/LogicSpawn RPG All In One/Tools/Cheats/Add 10000 Exp", false, 6)]
        private static void Add10000Exp()
        {
            GetObject.PlayerCharacter.AddExp(10000);
        }

        [MenuItem("Tools/LogicSpawn RPG All In One/Tools/Cheats/Add 1000 Skill Points", false, 6)]
        private static void Add1000SkillExp()
        {
            GetObject.PlayerCharacter.AddSkillPoints(1000);
        }

        [MenuItem("Tools/LogicSpawn RPG All In One/Tools/Cheats/Add 1000 Gold", false, 6)]
        private static void Add1000Gold()
        {
            GetObject.PlayerCharacter.Inventory.AddGold(1000);
        }
        [MenuItem("Tools/LogicSpawn RPG All In One/Tools/Cheats/Add 100000 Gold", false, 6)]
        private static void Add100000Gold()
        {
            GetObject.PlayerCharacter.Inventory.AddGold(100000);
        }

        [MenuItem("Tools/LogicSpawn RPG All In One/Tools/Cheats/SubtitleText", false, 6)]
        private static void SubtitleText()
        {
            GetObject.UIHandler.SubtitleUI.ShowSubtitle("Hello and welcome to RPGAIO", 10);
        }
        [MenuItem("Tools/LogicSpawn RPG All In One/Tools/Cheats/SubtitleText2", false, 6)]
        private static void SubtitleText2()
        {
            GetObject.UIHandler.SubtitleUI.ShowSubtitles(
                SubtitleTemplate.Line("Hello, and welcome to RPGAIO", 2),
                //SubtitleTemplate.Line("Developed by LogicSpawn (Dextero Solutions)", 5),
                SubtitleTemplate.Line("Soon, we will be releasing\n 2.0.0", 2),
                SubtitleTemplate.Line("New combat, new controls, new gametypes", 1),
                SubtitleTemplate.Line("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.!", 5),
                SubtitleTemplate.Line("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.\n Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi\n ut aliquip ex ea\n commodo consequat.!", 3),
                SubtitleTemplate.Line("You!", 1),
                SubtitleTemplate.Line("Ready?!", 1),
                SubtitleTemplate.Line("--- RPGAIO ---", 5)
                );
        }


        [MenuItem("Tools/LogicSpawn RPG All In One/Tools/Cheats/Add Item 0", false, 6)]
        private static void AddItem0()
        {
            GetObject.PlayerCharacter.Inventory.AddItem(Rm_RPGHandler.Instance.Repositories.Items.AllItems[0]);
        }

        [MenuItem("Tools/LogicSpawn RPG All In One/Tools/Data/Clear Player Prefs", false, 5)]
        private static void ClearPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("RPGAIO: Cleared all Player Prefs for this project");
        }

        [MenuItem("Tools/LogicSpawn RPG All In One/Contact LogicSpawn", false, 15)]
        public static void HelpAndSupport()
        {
            Process.Start(@"http://rpgaio.logicspawn.co.uk/RPGAIO/forums/");
        }

        [MenuItem("Tools/LogicSpawn RPG All In One/Tools/Update/Update UI", false, 5)]
        public static void UpdateUiOnScene()
        {
            var essentialsUI = GameObject.Find("[Essentials_UI]");
            if(essentialsUI == null)
            {
                Debug.LogError("[RPGAIO] Could not find \"[Essentials_UI]\" on this scene. You either already have \"[Essentials_UI]\" and \"[Essentials_UI_Mobile]\" setup on this scene, \"[Essentials_UI]\" misnamed as \"Essentials_UI\" or this scene is not setup correctly. " +
                               "Please run this only on a scene with Essentials_UI active that also does not have Essentials_UI_Mobile or UIToggleHandler");

                return;
            }

            //Add Mobile UI
            var mobileUI = Resources.Load("RPGMakerAssets/Essentials_UI_Mobile");
            var mobileUIObj = (GameObject)Instantiate(mobileUI);
            mobileUIObj.name = "[Essentials_UI_Mobile]";

            //Find Essentials and add UIToggleHandler with references
            var essentials = GameObject.Find("[Essentials]");
            var uiToggler = essentials.GetComponentInChildren<UIToggleHandler>();
            if(uiToggler == null)
            {
                for (int i = 0; i < essentials.transform.childCount; i++)
                {
                    if(essentials.transform.GetChild(i).name == "Handlers")
                    {
                        uiToggler = essentials.transform.GetChild(i).gameObject.AddComponent<UIToggleHandler>();
                    }
                }
            }

            uiToggler.Essentials_UI = essentialsUI;
            uiToggler.Essentials_UI_Mobile = mobileUIObj;

            essentialsUI.SetActive(false);
            mobileUIObj.SetActive(false);

            Debug.Log("[RPGAIO] Update done, please save the scene to keep the changes.");
        }

        [MenuItem("Tools/LogicSpawn RPG All In One/Tools/Update/Update Prefabs", false, 5)]
        public static void UpdatePrefabs()
        {

            if(EditorUtility.DisplayDialog("Update Prefabs",
                "Updating Prefabs will open a new scene, load all relevant prefabs and apply any missing objects to them from previous/current updates.", "Update Prefabs"))
            {
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);

                var s = Resources.LoadAll<Rm_PrefabIdentifier>("");
                var minimapObjects = s.Where(ss => ss.PrefabType == Rmh_PrefabType.NPC || ss.PrefabType == Rmh_PrefabType.Enemy ||
                                         ss.PrefabType == Rmh_PrefabType.Player_Class || ss.PrefabType == Rmh_PrefabType.Interactable ||
                                         ss.PrefabType == Rmh_PrefabType.Harvest);

                var minimapIcon = Resources.Load("RPGMakerAssets/PrefabGen/MinimapIconUI");
                var updatedObjects = new List<string>();
                foreach (var obj in minimapObjects)
                {
                    if (obj == null) continue;
                    
                    var updated = false;
                    var prefab = (GameObject)PrefabUtility.InstantiatePrefab(obj.gameObject);
                    
                    //Update NPC Objects without NpcQuestStatus
                    if(obj.PrefabType == Rmh_PrefabType.NPC)
                    {
                        if(!prefab.GetComponentInChildren<NpcQuestStatusModel>())
                        {
                            var npcQuestStatus = Resources.Load("RPGMakerAssets/PrefabGen/NpcQuestStatus");
                            var npcQuestStatusObj = (GameObject)PrefabUtility.InstantiatePrefab(npcQuestStatus);
                            npcQuestStatusObj.transform.SetParent(prefab.transform, false);
                            npcQuestStatusObj.transform.localPosition = new Vector3(0, 2.6f, 0);

                            prefab.GetComponent<NpcCharacterMono>().QuestStatusModel = npcQuestStatusObj.GetComponent<NpcQuestStatusModel>();
                            updated = true;
                        }
                    }

                    //Update Player Objects without CharacterCamera
                    if(obj.PrefabType == Rmh_PrefabType.Player_Class)
                    {
                        var model = obj.GetComponent<RPGController>().characterModel;
                        model.SetLayerRecursively(LayerMask.NameToLayer("PlayerModel"));

                        if(!prefab.FindInChildren("CharacterUICamera"))
                        {
                            var characterUICamera = Resources.Load("RPGMakerAssets/PrefabGen/CharacterUICamera");
                            var cameraUICameraObj = (GameObject)PrefabUtility.InstantiatePrefab(characterUICamera);
                            cameraUICameraObj.transform.SetParent(prefab.transform, false);
                            cameraUICameraObj.transform.localPosition = new Vector3(0, 1.0f, 1.8f);
                            updated = true;
                        }
                    }

                    //Update objects without MinimapIcon
                    if(!prefab.GetComponentInChildren<MinimapIconModel>())
                    {
                        var minimapUI = (GameObject)PrefabUtility.InstantiatePrefab(minimapIcon);
                        minimapUI.transform.SetParent(prefab.transform, false);
                        minimapUI.transform.localPosition = new Vector3(0, -0.5f, 0);

                        var minimapIconModel = minimapUI.GetComponent<MinimapIconModel>();
                        switch(obj.PrefabType)
                        {
                            case Rmh_PrefabType.Enemy:
                                minimapIconModel.Type = MinimapIconType.Enemy;
                                break;
                            case Rmh_PrefabType.NPC:
                                minimapIconModel.Type = MinimapIconType.NPC;
                                break;
                            case Rmh_PrefabType.Player_Class:
                                minimapIconModel.Type = MinimapIconType.Player;
                                break;
                            case Rmh_PrefabType.Interactable:
                                minimapIconModel.Type = MinimapIconType.Interactable;
                                break;
                            case Rmh_PrefabType.Harvest:
                                minimapIconModel.Type = MinimapIconType.Harvestable;
                                break;
                            default:
                                minimapIconModel.Type = MinimapIconType.Custom;
                                break;
                        }
                        updated = true;
                    }

                    if(updated)
                    {
                        updatedObjects.Add(obj.name);
                    }
                    PrefabUtility.ReplacePrefab(prefab, PrefabUtility.GetPrefabParent(prefab), ReplacePrefabOptions.ConnectToPrefab);
                }

                Debug.Log("[RPGAIO] Updated Prefabs [Check console for more details] " + "\n\n" + updatedObjects.Count + " Objects Updated:" + "\n" + String.Join("\n",updatedObjects.ToArray()));
                EditorUtility.DisplayDialog("Update Prefabs", "Patching done, close this scene without saving and return to your previous scene.", "Done");
            }
        }

        void OnEnable()
        {
            Window = this;
            PrefabBrowserIcon = PrefabBrowserIcon ?? Resources.Load("RPGMakerAssets/ToolbarIcons/prefabBrowser") as Texture2D;
            SaveIcon = SaveIcon ?? Resources.Load("RPGMakerAssets/ToolbarIcons/saveIcon") as Texture2D;
            LoadIcon = LoadIcon ?? Resources.Load("RPGMakerAssets/ToolbarIcons/loadIcon") as Texture2D;
            NewSceneIcon = NewSceneIcon ?? Resources.Load("RPGMakerAssets/ToolbarIcons/newSceneIcon") as Texture2D;
            CombatIcon = CombatIcon ?? Resources.Load("RPGMakerAssets/ToolbarIcons/combatIcon") as Texture2D;
            DialogIcon = DialogIcon ?? Resources.Load("RPGMakerAssets/ToolbarIcons/dialogIcon") as Texture2D;
            EventIcon = EventIcon ?? Resources.Load("RPGMakerAssets/ToolbarIcons/eventsIcon") as Texture2D;
            AchievementIcon = AchievementIcon ?? Resources.Load("RPGMakerAssets/ToolbarIcons/achievementIcon") as Texture2D;
            MapIcon = MapIcon ?? Resources.Load("RPGMakerAssets/ToolbarIcons/mapIcon") as Texture2D;
            EventTriggerIcon = EventTriggerIcon ?? Resources.Load("RPGMakerAssets/ToolbarIcons/eventTrigger") as Texture2D;
            LevelSwitchIcon = LevelSwitchIcon ?? Resources.Load("RPGMakerAssets/ToolbarIcons/levelSwitch") as Texture2D;
            PopupTextIcon = PopupTextIcon ?? Resources.Load("RPGMakerAssets/ToolbarIcons/popupText") as Texture2D;
        }

        void OnGUI()
        {
            try
            {
                OnGUIx();
            }
            catch (Exception e)
            {
                Debug.Log("Editor Error: " + e.Message + "@" + e.Source);
            }
        }

        void Update()
        {
            this.Repaint();
        }

        private void OnGUIx()
        {
            GUI.skin = Resources.Load("RPGMakerAssets/EditorSkinRPGMaker") as GUISkin;
            if(Window.position.width > 600)
            {
                GUILayout.BeginHorizontal();
            }
            else
            {
                GUILayout.BeginVertical();
            }

            if (GUILayout.Button(new GUIContent("RPGAIO", RPGMakerGUI.RPGMakerIcon), "rpgToolBarButton"))
            {
                Rme_Main.Init();
            }
            if (GUILayout.Button(new GUIContent("Prefab Window", PrefabBrowserIcon), "rpgToolBarButton"))
            {
                Rme_Tools_PrefabRepository.Init();
            }
            if (GUILayout.Button(new GUIContent("Save Data", SaveIcon), "rpgToolBarButton"))
            {
                EditorGameDataSaveLoad.SaveGameData();
            }
            if (GUILayout.Button(new GUIContent("Reload Data", LoadIcon), "rpgToolBarButton"))
            {
                EditorGameDataSaveLoad.LoadGameDataFromEditor();
            }
            if (GUILayout.Button(new GUIContent("Combat", CombatIcon), "rpgToolBarButton"))
            {
                CombatNodeWindow.Init();
            }
            if (GUILayout.Button(new GUIContent("Dialog", DialogIcon), "rpgToolBarButton"))
            {
                DialogNodeWindow.Init();
            }
            if (GUILayout.Button(new GUIContent("Events", EventIcon), "rpgToolBarButton"))
            {
                EventNodeWindow.Init();
            }
            if (GUILayout.Button(new GUIContent("Achievements", AchievementIcon), "rpgToolBarButton"))
            {
                AchievementNodeWindow.Init();
            }
            if (GUILayout.Button(new GUIContent("Map", MapIcon), "rpgToolBarButton"))
            {
                WorldMapNodeWindow.Init();
            }

            if (GUILayout.Button(new GUIContent("New Scene", NewSceneIcon), "rpgToolBarButton"))
            {
                AddScene();
            }
            if (GUILayout.Button(new GUIContent("Event Trigger", EventTriggerIcon), "rpgToolBarButton"))
            {
                EventTrigger();
            }
            if (GUILayout.Button(new GUIContent("Level Switch", LevelSwitchIcon), "rpgToolBarButton"))
            {
                LevelSwitch();
            }
            if (GUILayout.Button(new GUIContent("Popup Text", PopupTextIcon), "rpgToolBarButton"))
            {
                PopupText();
            }


            GUILayout.EndHorizontal();
        }

        private static void AddWorldLoot()
        {
            var worldLoot = (GameObject)Instantiate(Resources.Load("RPGMakerAssets/WorldLootItem"), Vector3.zero, Quaternion.identity);
            Selection.activeObject = SceneView.currentDrawingSceneView;
            if (Selection.activeObject != null)
            {
                var sceneCam = SceneView.currentDrawingSceneView.camera;
                var spawnPos = sceneCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 3f));
                worldLoot.transform.position = spawnPos;    
            }
        }

        private static void EventTrigger()
        {
            var eventTrigger = (GameObject)Instantiate(Resources.Load("RPGMakerAssets/PrefabGen/EventTrigger"), Vector3.zero, Quaternion.identity);
            if (eventTrigger != null)
            {
                eventTrigger.transform.name = "New Event Trigger";
                eventTrigger.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 5;
            }
        }

        private static void PopupText()
        {
            var eventTrigger = (GameObject)Instantiate(Resources.Load("RPGMakerAssets/PrefabGen/PopupText"), Vector3.zero, Quaternion.identity);
            if (eventTrigger != null)
            {
                eventTrigger.transform.name = "New Popup Text";
                eventTrigger.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 5;
            }
        }

        private static void LevelSwitch()
        {
            var levelSwitch = (GameObject)Instantiate(Resources.Load("RPGMakerAssets/PrefabGen/LevelSwitch"), Vector3.zero, Quaternion.identity);
            if (levelSwitch != null)
            {
                levelSwitch.transform.name = "New Level Switch Trigger";
                levelSwitch.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 5;
            }
        }

        private static void AddScene()
        {
            var original = EditorBuildSettings.scenes; 
            var newSettings = new EditorBuildSettingsScene[original.Length + 1];
            System.Array.Copy(original, newSettings, original.Length);

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);

            var e1 = (GameObject)Instantiate(Resources.Load("RPGMakerAssets/Essentials"), Vector3.zero, Quaternion.identity);
            var e2 = (GameObject)Instantiate(Resources.Load("RPGMakerAssets/Essentials_UI"), Vector3.zero, Quaternion.identity);
            var e3 = (GameObject)Instantiate(Resources.Load("RPGMakerAssets/SpawnPosition"), Vector3.zero, Quaternion.identity);
            var e4 = (GameObject)Instantiate(Resources.Load("RPGMakerAssets/Essentials_UI_Mobile"), Vector3.zero, Quaternion.identity);

            e1.name = "[Essentials]";
            e2.name = "[Essentials_UI]";
            e3.name = "[Essentials_SpawnPosition]";
            e4.name = "[Essentials_UI_Mobile]";

            var uiToggleHandler = e1.GetComponentInChildren<UIToggleHandler>();
            uiToggleHandler.Essentials_UI = e2;
            uiToggleHandler.Essentials_UI_Mobile = e4;

            e2.SetActive(false);
            e4.SetActive(false);

            var path = EditorUtility.SaveFilePanel("Scene Save Location","", "","unity");

            var dataPath = Application.dataPath;
            var shortenedPath = path.Replace(dataPath, "");
            shortenedPath = shortenedPath.Substring(1, shortenedPath.Length - 1);

            Debug.Log("[RPGAIO] New scene saved at: " + shortenedPath);
            EditorSceneManager.SaveScene(scene, "Assets/" + shortenedPath, false);
            //EditorSceneManager.OpenScene("Assets/" + shortenedPath);

            Debug.Log(path);
            if (path.Length != 0)
            {
                var sceneToAdd = new EditorBuildSettingsScene("Assets/" + shortenedPath, true);
                newSettings[newSettings.Length - 1] = sceneToAdd;
                EditorBuildSettings.scenes = newSettings;
            }
        }

        private static void AddSceneEssentials()
        {
            var e1 = (GameObject)Instantiate(Resources.Load("RPGMakerAssets/Essentials"), Vector3.zero, Quaternion.identity);
            var e2 = (GameObject)Instantiate(Resources.Load("RPGMakerAssets/Essentials_UI"), Vector3.zero, Quaternion.identity);
            var e3 = (GameObject)Instantiate(Resources.Load("RPGMakerAssets/SpawnPosition"), Vector3.zero, Quaternion.identity);
            var e4 = (GameObject)Instantiate(Resources.Load("RPGMakerAssets/Essentials_UI_Mobile"), Vector3.zero, Quaternion.identity);

            e1.name = "[Essentials]";
            e2.name = "[Essentials_UI]";
            e3.name = "[Essentials_SpawnPosition]";
            e4.name = "[Essentials_UI_Mobile]";

            var uiToggleHandler = e1.GetComponentInChildren<UIToggleHandler>();
            uiToggleHandler.Essentials_UI = e2;
            uiToggleHandler.Essentials_UI_Mobile = e4;

            e2.SetActive(false);
            e4.SetActive(false);

            EditorUtility.DisplayDialog("Essentials Added.", "You will need to manually add the scene to the build scenes.", "OK");
        }

        public Rect PadRect(Rect rect, int left, int top)
        {
            return new Rect(rect.x + left, rect.y + top, rect.width - (left*2), rect.height - (top*2));
        }

    
    }
}
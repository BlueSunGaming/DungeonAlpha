using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace LogicSpawn.RPGMaker.Editor
{
    public class Rme_Tools_PrefabRepository : EditorWindow
    {
        private Rmh_PrefabType selectedCategory = Rmh_PrefabType.Skill;
        private SkillType selectedSkillType = SkillType.Aura;
        private bool showAll = true;
        private bool showTriangles;
        public const string BasePrefabPath = "RPGAIOPrefabRepository";
        private int selectedObjectIndex = 0;
        private PrefabInfo selectedObject = null;
        private Vector2 scrollPos = Vector2.zero;
        private List<PrefabInfo> objects = new List<PrefabInfo>();
        private GUIContent[] content = null;
        private static string _searchTerm = "";
        private static string _searchTermPlaceholder = "<color=gray>Type here to filter: </color>";
        private static Rme_Tools_PrefabRepository Window;

        [MenuItem("Tools/LogicSpawn RPG All In One/Prefab Repository", false, 3)]
        public static void Init()
        {
            // Get existing open window or if none, make a new one:
            Window = (Rme_Tools_PrefabRepository)GetWindow(typeof(Rme_Tools_PrefabRepository));
            Window.maxSize = new Vector2(3000, 3000);
            Window.titleContent = new GUIContent("RPGPrefabs");
            Window.minSize = new Vector2(500.1f, 100.1f);
            Window.position = new Rect(100, 100, 900.1f, 800.1f);
        }

        void OnGUI()
        {
            try
            {
                OnGUIx();
            }
            catch (Exception e) 
            {
                //todo: figure cause of GUI control bug
                if (!e.Message.Contains("Getting control"))
                {
                    Debug.LogException(e);    
                }
            }
        }

        void OnEnable()
        {
            Window = this;
            showAll = true;
        }

        void Update()
        {
            if (!objects.Any())
            {
                UpdatePrefabs();    
                return;
            }

            this.Repaint();
        }

        void UpdatePrefabs()
        {
            selectedObject = null;
            var files = Directory.GetFiles(Application.dataPath + "/Resources/" + BasePrefabPath,"*.prefab",SearchOption.AllDirectories);
            var filesFixed = files.Select(s => s.Replace(Application.dataPath, "Assets"));
            foreach (var file in filesFixed)
            {
                var fileUnityFormat = file.Replace("\\","/");

                if(objects.FirstOrDefault(o => o.Path == file) == null)
                {
                    var obj = AssetDatabase.LoadAssetAtPath<GameObject>(fileUnityFormat);
                    objects.Add(new PrefabInfo(file, obj));
                }
            }
        }

        private void OnGUIx()
        {
            GUI.skin = Resources.Load("RPGMakerAssets/EditorSkinRPGMaker") as GUISkin;
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Space(1);
            GUILayout.Label("Prefab Browser","mainTitle", GUILayout.Height(40));
            GUILayout.Space(1);
            GUILayout.EndHorizontal();

            GUILayout.Space(2);

            GUILayout.BeginHorizontal();

            GUILayout.Space(2);

            #region "Categories"
            GUILayout.BeginVertical("backgroundBox", GUILayout.Width(180), GUILayout.ExpandHeight(true));
            GUILayout.Label("Categories");
            var categories = Enum.GetNames(typeof (Rmh_PrefabType));
            
            var ifShowAll = showAll ? "listItemSelected" : "listItem";
            if (GUILayout.Button("All", ifShowAll, GUILayout.Height(25)))
            {
                showAll = true;
                return;
            }
            foreach(var cat in categories)
            {
                var ifSelected = selectedCategory.ToString() == cat && !showAll ? "listItemSelected" : "listItem";
                if (GUILayout.Button(cat.Replace('_',' '), ifSelected, GUILayout.Height(25)))
                {
                    selectedCategory = (Rmh_PrefabType)Enum.Parse(typeof(Rmh_PrefabType), cat);
                    showAll = false;
                    return;
                }
            }
            GUILayout.EndVertical();
            #endregion

            GUILayout.Space(2);

            #region "MainArea"
            GUILayout.BeginVertical("backgroundBoxMain");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Filter:", GUILayout.Width(35));
            GUI.SetNextControlName("searchTerm");
            _searchTerm = GUILayout.TextField(_searchTerm, "nodeTextField", GUILayout.Width(200));
            GUILayout.Space(1);
            if(!showAll && !new []{Rmh_PrefabType.Enemy, Rmh_PrefabType.NPC, Rmh_PrefabType.Harvest}.Any(c => c == selectedCategory))
            {
                if (GUILayout.Button("+ Create New", "genericButton", GUILayout.Height(25)))
                {
                    if(selectedCategory == Rmh_PrefabType.Skill)
                    {
                        Rme_PrefabGenerator.GeneratePrefab(selectedCategory, selectedSkillType);
                    }
                    else
                    {
                        Rme_PrefabGenerator.GeneratePrefab(selectedCategory);
                    } 
                    UpdatePrefabs();
                }

                if(selectedCategory == Rmh_PrefabType.Skill)
                {
                    selectedSkillType = (SkillType) RPGMakerGUI.EnumPopup("Skill Type:",selectedSkillType);    
                }
            }
            else if(!showAll)
            {
                if (GUILayout.Button("+ Create New In RPGMaker", "genericButton", GUILayout.Height(25)))
                {
                    Rme_Main.Init();
                }
            }
     
            GUILayout.EndHorizontal();

            if (Event.current.type == EventType.Repaint)
            {
                if (GUI.GetNameOfFocusedControl() == "searchTerm" && _searchTerm == _searchTermPlaceholder)
                {
                    _searchTerm = "";
                }
            }
            List<PrefabInfo> objectsFiltered = objects;
            if(_searchTerm != _searchTermPlaceholder)
            {
                if (showAll)
                {
                    objectsFiltered = objects.Where(o => o.Identifier != null
                                                         && ((o.Identifier.SearchName.ToLower().Contains(_searchTerm.ToLower())
                                                              || o.Identifier.PrefabType.ToString().ToLower().Contains(_searchTerm.ToLower())))
                    ).ToList();
                }
                else
                {
                    objectsFiltered = objects.Where(o => o.Identifier != null
                                                     && ((o.Identifier.SearchName.ToLower().Contains(_searchTerm.ToLower())
                                                          || o.Identifier.PrefabType.ToString().ToLower().Contains(_searchTerm.ToLower()))
                                                         && o.Identifier.PrefabType == selectedCategory)
                    ).ToList();
                }
                
            }


            content = objectsFiltered.Select(s => new GUIContent(s.Identifier.SearchName + (showTriangles ? s.Details : ""), s.Preview)).ToArray();    
            

            scrollPos = GUILayout.BeginScrollView(scrollPos);
            selectedObjectIndex = GUILayout.SelectionGrid(selectedObjectIndex, content, Window.position.width > 520 ? (int)((Window.position.width - 520) / 120).RoundToNearest(1) : 1, "prefabWindowGrid");
            if(selectedObjectIndex > content.Length - 1)
            {
                selectedObjectIndex = 0;
            }
            var oldSelectedObject = selectedObject;
            selectedObject = selectedObjectIndex != -1 && objectsFiltered.Count > 0 ? objectsFiltered[selectedObjectIndex] : null;
            if(selectedObject != oldSelectedObject)
            {
                GUI.FocusControl("");
            }
            GUILayout.EndScrollView();

            GUILayout.EndVertical();
            #endregion

            GUILayout.Space(2);

            #region "Properties"
            GUILayout.BeginVertical("backgroundBox",GUILayout.Width(200),GUILayout.ExpandHeight(false));
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginVertical();
            GUILayout.Label("Properties");
            if(selectedObject != null)
            {
                if (selectedObject.Identifier != null)
                {
                    EditorGUILayout.PrefixLabel("Name:");
                    selectedObject.Identifier.SearchName = EditorGUILayout.TextField(selectedObject.Identifier.SearchName);
                    EditorGUILayout.PrefixLabel("Type:");
                    selectedObject.Identifier.PrefabType = (Rmh_PrefabType)EditorGUILayout.EnumPopup(selectedObject.Identifier.PrefabType);
                    EditorGUILayout.LabelField(selectedObject.Identifier.PrefabType.ToString());
                    if (GUILayout.Button("Spawn To Scene", "genericButton", GUILayout.Height(25)))
                    {
                        var obj = AssetDatabase.LoadAssetAtPath<GameObject>(selectedObject.Path);
                        GeneralMethodsEditor.InstantiateInView(obj);
                    }
                    if (GUILayout.Button("Select in Project Folder", "genericButton", GUILayout.Height(25)))
                    {
                        var obj = AssetDatabase.LoadAssetAtPath<GameObject>(selectedObject.Path);
                        Selection.activeObject = obj;
                        Selection.activeGameObject = obj;
                    }
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            #endregion

            GUILayout.Space(2);

            GUILayout.EndHorizontal();

            GUILayout.Space(2);

            GUILayout.BeginHorizontal("backgroundBox",GUILayout.Height(30));
            GUILayout.Space(5);
            if (GUILayout.Button("Reload Prefabs", "genericButton", GUILayout.Height(25)))
            {
                objects = new List<PrefabInfo>();
                UpdatePrefabs();
                return;
            }
            if (GUILayout.Button("Toggle Tri. Count", "genericButton", GUILayout.Height(25)))
            {
                showTriangles = !showTriangles;
            }
            GUILayout.FlexibleSpace();
            
            GUILayout.Label(objectsFiltered.Count + " Items", GUILayout.Width(120));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

        }

        public Rect PadRect(Rect rect, int left, int top)
        {
            return new Rect(rect.x + left, rect.y + top, rect.width - (left*2), rect.height - (top*2));
        }
    }

    public class PrefabInfo
    {
        public string Path;
        public Texture2D Preview;
        public Rm_PrefabIdentifier Identifier;
        public int TriCount;
        public string Details
        {
            get { return TriCount > 0 ? "\n Tris:" + TriCount : ""; }
        }

        public PrefabInfo(string path, GameObject go)
        {
            if(go != null)
            {
                Identifier = go.GetComponent<Rm_PrefabIdentifier>();

                var allMeshes = go.GetAllChildren<MeshFilter>();
                var totalMeshTriCount = allMeshes.Sum(m => m.sharedMesh.triangles.Length / 3);
                TriCount = totalMeshTriCount;
            }

            Path = path;
            Preview = AssetPreview.GetAssetPreview(go);
        }
    }
}
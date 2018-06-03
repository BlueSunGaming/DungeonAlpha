using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Assets.Scripts.RPGMaker.Nodes.Core;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Editor.New
{
    public abstract class NodeWindow : EditorWindow
    {
        private const float DefaultNodeWidth = 5;
        private const float DefaultNodeHeight = 5;
        public const float ScrollDimensions = 10000;
        public const float VariablePaneWidth = 400;

        private bool _initialised;
        
        private Node _selectedNode;
        private int _selectedWindow;
        private Vector2 _nodeScrollPos = Vector2.zero;

        private Vector2 _nodeAddPosition = Vector2.zero;
        private bool _showAddNodeWindow;
        private bool _focusedSearch;
        private Rect _addNodeRect = new Rect(0, 0, 200, 450);
        private Vector2 _addNodeScrollPos = Vector2.zero;
        private const int AddNodeWindowID = 999999;
        private string _searchTerm = "";
        private string _searchTermPlaceholder = "<color=gray>Search or choose a node: </color>";
        private OrganisedNodeTree OrganisedNodeTree;
        private GUIStyle FoldOutStyle;
        private bool _showStandard = true;
        private bool _showProps = true;
        private bool _showRoutines = true;
        private bool _showConditions = true;

        protected List<Type> NodeTypes;

        private bool _snapWindows;
        private bool _showVars;
        private bool _showGrid = true;
        private bool _showHelp;
        private const string GeneralHelpText = "Node Trees:\r\nNode trees can be added at the bottom of the window.\r\n\r\nAdding Nodes:\r\nOnce you have selected a node tree, right click the scroll-view area to add a new node.";

        private Node _nodeToLink;
        private int _nodeToLinkIndex;
        private List<object[]> _allLinks;
        private int _nodeLinkQuality = 10;

        private string _savedData;
        private string _popupIDValue = "";
        private int _selectedIndex = 0;

        protected internal abstract NodeBank NodeBank { get; }
        protected internal int SelectedNodeTreeIndex;
        private NodeTree _selectedNodeTree;
        private Rect _scrollViewRect;
        private Vector2 _scrollTextArea = new Vector2(0, 0);

        private GameObject _gameObjectHolder = null;
        private Sprite _spriteHolder = null;
        private AudioClip _audioClipHolder = null;
        private Texture2D _texture2DHolder = null;

        protected internal NodeTree SelectedNodeTree
        {
            get { return _selectedNodeTree; }
            set
            {

                var old = _selectedNodeTree;
                _selectedNodeTree = value;

                if (old != _selectedNodeTree)
                {
                    RefreshWindow();
                }
            }
        }

        private void RefreshWindow()
        {
            GUI.FocusControl("");
            _selectedWindow = -1;
            _selectedNode = null;
            _nodeToLink = null;
            _showAddNodeWindow = false;
            UpdateLinks();
        }

        private List<Node> Nodes { get { return SelectedNodeTree != null ? SelectedNodeTree.Nodes : null; } }

        //todo: abstract this method for diff types, e.g. start with StartDialogNode
        protected virtual void OnEnable()
        {
            EditorGameDataSaveLoad.LoadIfNotLoadedFromEditor();
            wantsMouseMove = false;
            var assemblies = GetAssemblies();
            NodeTypes = assemblies.SelectMany(t => t.GetTypes()).Where(t => typeof(Node).IsAssignableFrom(t) && !t.IsAbstract).ToList();
            FilterNodeTypes(ref NodeTypes);
            OrganisedNodeTree = new OrganisedNodeTree(NodeTypes.Select(n => (Node)Activator.CreateInstance(n)).ToList(),GetNodeType());
            RefreshWindow();
            _initialised = true;
        }

        

        private static List<Assembly> GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies().ToList();
        }

        protected abstract NodeTreeType GetNodeType(); 
        protected abstract void FilterNodeTypes(ref List<Type> nodeTypes);
        private void OnGUI()
        {
            if (!_initialised) return;

            var oldColor = GUI.color;
            GUI.color = new Color(12f / 255, 12f / 255, 12f / 255, 0.7f);
            GUI.DrawTexture(new Rect(0,0,position.width,position.height), EditorGUIUtility.whiteTexture);
            GUI.color = oldColor;
            try
            {
                OnGuIx();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void OnGuIx()
        {
            GUI.skin = Resources.Load("RPGMakerAssets/EditorSkinRPGMaker") as GUISkin;
            if(FoldOutStyle == null)
            {
                GUIStyle style = EditorStyles.foldout;
                FoldOutStyle = new GUIStyle(style) {fontStyle = FontStyle.Bold, 
                    normal = {textColor = Color.white},
                    active = {textColor = Color.white},
                    focused = { textColor = Color.white},
                    hover = { textColor = Color.white},
                    onActive = { textColor = Color.white},
                    onNormal = { textColor = Color.white},
                    onFocused = { textColor = Color.white},
                    onHover = { textColor = Color.white},
                };
            }

            if (_showVars)
            {
                _scrollViewRect = new Rect(0, 0, position.width - VariablePaneWidth, position.height - 25);
            }
            else
            {
                _scrollViewRect = new Rect(0, 0, position.width, position.height - 25);
            }
            if (NodeBank.NodeTrees.Count > 0)
            {
                SelectedNodeTree =
                    NodeBank.NodeTrees[SelectedNodeTreeIndex];
            }


            if(_showGrid)
            {
                Rme_NodeWindowDraw.DrawNodeGrid(_scrollViewRect, 12.5f, Color.gray);
                Rme_NodeWindowDraw.DrawNodeGrid(_scrollViewRect, 100f, Color.black, 0.3f, 2.0f);
            }

            

            if (SelectedNodeTree != null)
            {
                Rme_NodeWindowDraw.DrawLinesNew(Nodes, _nodeScrollPos, position, _nodeLinkQuality, _allLinks);
            }

            if (SelectedNodeTree != null && _showVars)
            {
                GUI.Box(new Rect(position.width - VariablePaneWidth, 0, VariablePaneWidth, position.height - 25), "", "backgroundBoxMain");
                GUILayout.BeginArea(new Rect(position.width - VariablePaneWidth, 0, VariablePaneWidth, position.height - 25));
                GUILayout.BeginVertical();

                GUILayout.BeginHorizontal("backgroundBox");
                GUILayout.Label("Variables", "variablePaneTitle");
                GUILayout.FlexibleSpace();
                GUILayout.Box("+", "genericButton", GUILayout.Height(25), GUILayout.Width(25));
                var rect = GUILayoutUtility.GetLastRect();
                var evtC = Event.current;
                if (evtC.type == EventType.MouseDown)
                {
                    var mousePos = evtC.mousePosition;

                    if (rect.Contains(mousePos))
                    {
                        // Now create the menu, add items and show it
                        var menu = new GenericMenu();
                        var primitives = new[] { PropertyType.String, PropertyType.Int, PropertyType.Float, PropertyType.Bool };
                        foreach (var itemType in primitives)
                        {
                            menu.AddItem(new GUIContent(itemType.ToString()), false, AddTreeVar(), itemType);
                        }

                        menu.AddSeparator("");
                        var others = new[] { PropertyType.GameObject, PropertyType.Vector3 };

                        foreach (var itemType in others)
                        {
                            menu.AddItem(new GUIContent(itemType.ToString()), false, AddTreeVar(), itemType);
                        }

                        menu.ShowAsContext();
                        evtC.Use();
                    }
                }
                GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                GUILayout.Label("Name", GUILayout.Width(120));
                GUILayout.Label("Value", GUILayout.Width(200));
                GUILayout.Label("IsList?", GUILayout.Width(30));
                GUILayout.EndHorizontal();
                for (int index = 0; index < SelectedNodeTree.Variables.Count; index++)
                {
                    var treeVar = SelectedNodeTree.Variables[index];
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(RPGMakerGUI.AddIcon, "genericButton", GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        AddPropertyNode(treeVar);
                    }
                    treeVar.Name = RPGMakerGUI.TextField("", treeVar.Name, 0, GUILayout.Width(100));
                    if (treeVar.PropType != PropertyType.CombatCharacter && !treeVar.ID.StartsWith("DamageDealtVar_"))
                    {
                        if (treeVar.PropType == PropertyType.String || treeVar.PropType == PropertyType.TextArea)
                        {
                            GUILayout.Label("S", GUILayout.Width(5));   
                            if(!treeVar.IsList)
                                treeVar.DefaultValue = RPGMakerGUI.TextField("", Convert.ToString(treeVar.DefaultValue), 0, GUILayout.Width(195));
                        }
                        else if (treeVar.PropType == PropertyType.Float)
                        {
                            GUILayout.Label("F", GUILayout.Width(5));
                            if (!treeVar.IsList)
                                treeVar.DefaultValue = RPGMakerGUI.FloatField("", Convert.ToSingle(treeVar.DefaultValue), 0, GUILayout.Width(195));
                        }
                        else if (treeVar.PropType == PropertyType.Int)
                        {
                            GUILayout.Label("I", GUILayout.Width(5));
                            if (!treeVar.IsList)
                                treeVar.DefaultValue = RPGMakerGUI.IntField("", Convert.ToInt32(treeVar.DefaultValue), 0, GUILayout.Width(195));
                        }
                        else if (treeVar.PropType == PropertyType.Bool)
                        {
                            GUILayout.Label("B", GUILayout.Width(5));
                            if (!treeVar.IsList)
                                treeVar.DefaultValue = RPGMakerGUI.Toggle("", Convert.ToBoolean(treeVar.DefaultValue), GUILayout.Width(195));
                        }
                        else if(treeVar.PropType == PropertyType.Vector3)
                        {
                            GUILayout.Label("V", GUILayout.Width(5));
                            if (!treeVar.IsList)
                                treeVar.DefaultValue = RPGMakerGUI.Vector3Field("", (RPGVector3)(treeVar.DefaultValue), 0, GUILayout.Width(55));
                        }
                        else if(treeVar.PropType == PropertyType.GameObject)
                        {
                            GUILayout.Label("G", GUILayout.Width(5));
                            GUILayout.Space(195);
                        }


                        treeVar.IsList = EditorGUILayout.Toggle(treeVar.IsList, GUILayout.Width(30));

                        if (RPGMakerGUI.DeleteButton(18))
                        {
                            SelectedNodeTree.Variables.RemoveAt(index);
                            index--;
                        }    
                    }
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();
                GUILayout.EndArea();
            }

            _nodeScrollPos = GUI.BeginScrollView(_scrollViewRect,
                                     _nodeScrollPos,
                                     new Rect(0, 0, ScrollDimensions, ScrollDimensions), false, false);
            if (SelectedNodeTree != null)
            {
                DrawNodeChains(Nodes);



                BeginWindows();
                for (int index = 0; index < Nodes.Count; index++)
                {
                    var node = Nodes[index];
                    var windowStyle = "";

                    var propNode = node as PropertyNode;
                    var isPropertyNode = propNode != null && propNode.NodeType == NodeType.Property;

                    if(node is NoteNode)
                    {
                        windowStyle = (_selectedWindow == node.WindowID ? "nodeSelectedWindowNote" : "nodeStartWindowNote");   
                    }
                    else if (!isPropertyNode)
                    {
                        if(propNode != null)
                        {
                            windowStyle = (_selectedWindow == node.WindowID ? "nodeSelectedWindowAdv" : "nodeStartWindowAdv");   
                        }
                        else
                        {
                            windowStyle = (_selectedWindow == node.WindowID ? "nodeSelectedWindow" : "nodeStartWindow");    
                        }
                    }
                    else
                    {
                        windowStyle = (_selectedWindow == node.WindowID ? "nodeSelectedWindowProp" : "nodeStartWindowProp");
                    }

                    node.Rect = GUILayout.Window(node.WindowID, node.Rect, NodeGUIWindow, "", windowStyle);
                }

                if (_showAddNodeWindow)
                {
                    _addNodeRect = GUILayout.Window(AddNodeWindowID, _addNodeRect, AddNodeWindow, "", "addNodeBackground");
                }
                else
                {
                    _focusedSearch = false;
                }

                EndWindows();
            }
            GUI.EndScrollView();



            #region footer-Toolbar

            GUILayout.BeginArea(new Rect(0, _scrollViewRect.yMax, position.width, position.height - _scrollViewRect.yMax));
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);



            if (SelectedNodeTree != null)
            {
                GUI.enabled = _selectedNode != null && _selectedNode.CanBeDeleted;
                if (GUILayout.Button("Delete", "nodeToolbarButton", GUILayout.MinWidth(60)) || (Event.current.isKey && Event.current.keyCode == KeyCode.Delete))
                {
                    TryDeleteNode();
                    Event.current.Use();
                    return;
                }
                GUI.enabled = true;

                if (GUILayout.Button("Save", "nodeToolbarButton", GUILayout.MinWidth(60)))
                {
                    EditorGameDataSaveLoad.SaveGameData();
                    GameSettingsSaveLoadManager.Instance.SaveSettings();
                }
                if (GUILayout.Button("Reload", "nodeToolbarButton", GUILayout.MinWidth(60)))
                {
                    Rm_RPGHandler.Instance = null;
                    EditorGameDataSaveLoad.LoadGameDataFromEditor();
                    GameSettingsSaveLoadManager.Instance.LoadSettings();
                    UpdateLinks();
                    Nodes.ForEach(n => n.Rect = new Rect(n.Rect.x, n.Rect.y, 50, 50));
                    return;

                }
                if (GUILayout.Button("Help", "nodeToolbarButton", GUILayout.MinWidth(60)))
                {
                    _showHelp = !_showHelp;

                }
                if (GUILayout.Button("Grid" + (_showGrid ? "[x]" : "[ ]"), "nodeToolbarButton", GUILayout.MinWidth(60)))
                {
                    _showGrid = !_showGrid;
                }
                if (GUILayout.Button("Snap" + (_snapWindows ? "[x]" : "[ ]"), "nodeToolbarButton", GUILayout.MinWidth(60)))
                {
                    _snapWindows = !_snapWindows;
                }
                if (GUILayout.Button("Variables" + (_showVars ? "[x]" : "[ ]"), "nodeToolbarButton", GUILayout.MinWidth(60)))
                {
                    _showVars = !_showVars;
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.Label("Node Tree:");
            SelectedNodeTreeIndex = EditorGUILayout.Popup(SelectedNodeTreeIndex,
                                                          NodeBank.NodeTrees.Select((q, indexOf) => indexOf + ". " + q.Name).ToArray(),GUILayout.Width(200));





            if (SelectedNodeTree != null)
            {
                SelectedNodeTree.Name = GUILayout.TextField(SelectedNodeTree.Name, GUILayout.Width(200));
            }


            if (GUILayout.Button("Add Tree", "nodeToolbarButton", GUILayout.MinWidth(60)))
            {
                var newTree = GetNewTree(NodeBank.Type);
                NodeBank.NodeTrees.Add(newTree);
                SelectedNodeTreeIndex = NodeBank.NodeTrees.IndexOf(newTree);
                SelectedNodeTree = newTree;
                RefreshWindow();
                return;
            }
            GUI.enabled = SelectedNodeTree != null && !SelectedNodeTree.Required;
            if (GUILayout.Button("Delete Tree", "nodeToolbarButton", GUILayout.MinWidth(60)))
            {
                if (SelectedNodeTree != null && !SelectedNodeTree.Required)
                {
                    if(SelectedNodeTree.Type == NodeTreeType.Combat)
                    {
                        var skill = Rm_RPGHandler.Instance.Repositories.Skills.AllSkills.FirstOrDefault(s => s.ID == SelectedNodeTree.ID);
                        if(skill != null)
                        {
                            skill.DamageScalingTreeID = "";
                        }
                    }

                    NodeBank.NodeTrees.Remove(SelectedNodeTree);

                    SelectedNodeTree = null;
                    SelectedNodeTreeIndex = 0;

                    RefreshWindow();
                    return;
                }
            } 
            if (GUILayout.Button("Duplicate Tree", "nodeToolbarButton", GUILayout.MinWidth(60)))
            {
                var treeCopy = GeneralMethods.CopyObject(SelectedNodeTree);
                treeCopy.ID = Guid.NewGuid().ToString();
                treeCopy.Name = treeCopy.Name + " copy";
                NodeBank.NodeTrees.Add(treeCopy);
                RefreshWindow();
                return;
            }
            GUI.enabled = true;
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            #endregion


            GUILayout.BeginArea(new Rect(0, 0, position.width - 15 - (_showVars ? VariablePaneWidth : 0), 40));
            GUILayout.BeginHorizontal("nodeWindowTitle");
            GUILayout.Space(5);
            GUILayout.Label(SelectedNodeTree == null ? "Nodes" : NodeBank.Type + " > " + SelectedNodeTree.Name, "nodeWindowTitle");
            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();
            GUILayout.EndArea();


            if(_showHelp)
            {
                GUILayout.BeginArea(new Rect(position.width - 305, 0,300,position.height));
                GUILayout.FlexibleSpace();
                GUILayout.Box(_selectedNode != null ? _selectedNode.Description : GeneralHelpText,"helpBox");
                GUILayout.Space(30);
                GUILayout.EndArea();    
            }

            var evt = Event.current;
            if (evt != null)
            {
                if (evt.type == EventType.MouseDown && evt.button == 0)
                {
                    var mousePos = evt.mousePosition;

                    if (SelectedNodeTree != null)
                    {
                        if (_showAddNodeWindow && !_addNodeRect.Contains(mousePos)) //&& SelectedNodeTree != null
                        {
                            _showAddNodeWindow = false;
                        }
                        else if (Nodes.All(n => !n.Rect.Contains(Event.current.mousePosition - _nodeScrollPos)))
                        {
                            RefreshWindow();
                        }
                    }

                    evt.Use();
                    return;
                }
                if (evt.type == EventType.ContextClick)
                {
                    var mousePos = evt.mousePosition;
                    if (_scrollViewRect.Contains(mousePos)) //&& SelectedNodeTree != null
                    {
                        ShowContextMenu(mousePos + _nodeScrollPos);
                    }
                    evt.Use();
                    return;
                }
            }

            if(_nodeToLink != null)
            {            
                wantsMouseMove = true;
                Rme_NodeWindowDraw.DrawLineFromTo(_nodeToLink, _nodeToLinkIndex, Event.current.mousePosition, _nodeScrollPos, position, _nodeLinkQuality);
            }

        }

        private void TryDeleteNode()
        {
            if (_selectedNode != null && _selectedNode.CanBeDeleted)
            {
                DeleteNode(_selectedNode, SelectedNodeTree);
            }
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

        private GenericMenu.MenuFunction2 AddTreeVar()
        {
            return AddTreeVar;
        }

        private void AddTreeVar(object userData)
        {
            var propType = (PropertyType) userData;
            SelectedNodeTree.Variables.Add(new NodeTreeVar("New " + propType + " Variable", propType));
        }

        protected void DeleteNode(Node nodeToDelete, NodeTree nodeTree)
        {
            ClearPrevLinks(nodeToDelete, nodeTree);
            for (int i = 0; i < nodeToDelete.NextNodeLinks.Count; i++)
            {
                RemoveLink(nodeToDelete, i, nodeTree);
            }

            Nodes.Remove(nodeToDelete); 
            UpdateLinks();
        }

        private void DrawNodeChains(List<Node> nodes)
        {
            foreach(var n in nodes.Where(n => n.IsStartNode))
            {
                var nodesInChain = new List<Node>(){n};
                NodeHelper.GetAllLinkedNodes(n, SelectedNodeTree, ref nodesInChain);

                var rects = nodesInChain.Select(x => new Rect(x.Rect)).ToList();
                if(rects.Count > 0)
                {
                    var top = rects.Min(r => r.yMin);
                    var left = rects.Min(r => r.xMin);

                    var bottom = rects.Max(r => r.yMax);
                    var right = rects.Max(r => r.xMax);
                    var pos = new Vector2((left - 10), (top - 10));
                    var rect = new Rect(pos.x, pos.y, right - left + 20, bottom - top + 20);

                    n.NodeChainName = GUI.TextField(ClampWindow(new Rect(rect.x, rect.y - 30, 240, 25)), n.NodeChainName, "nodeChainTitle");
                    GUI.Box(ClampWindow(rect), "");
                }
                
            }
        }

        protected virtual void Update()
        {

            //if(_nodeToLink != null)
            //{
                Repaint();
            //}

            if(_nodeToLink != null && _nodeToLink.NextNodeLinks.Count - 1 < _nodeToLinkIndex)
            {
                _nodeToLink = null;
            }
        }

        private void AddNodeWindow(int id)
        {
            GUI.SetNextControlName("searchTerm");
            _searchTerm = GUILayout.TextField(_searchTerm, "nodeTextField");

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.Space(7);
            _showStandard = EditorGUILayout.Toggle(_showStandard, GUILayout.Width(15));
            GUILayout.EndVertical();
            GUILayout.Label("Standard");

            GUILayout.BeginVertical();
            GUILayout.Space(7);
            _showProps = EditorGUILayout.Toggle(_showProps, GUILayout.Width(15));
            GUILayout.EndVertical();
            GUILayout.Label("Property");

            GUILayout.BeginVertical();
            GUILayout.Space(7);
            _showConditions = EditorGUILayout.Toggle(_showConditions, GUILayout.Width(15));
            GUILayout.EndVertical();
            GUILayout.Label("Conditions");

            GUILayout.BeginVertical();
            GUILayout.Space(7);
            _showRoutines = EditorGUILayout.Toggle(_showRoutines, GUILayout.Width(15));
            GUILayout.EndVertical();
            GUILayout.Label("Routines");
            GUILayout.EndHorizontal();
    
            if(!_focusedSearch)
            {
                EditorGUI.FocusTextInControl("searchTerm");
                _focusedSearch = true;
            }

             if (Event.current.type == EventType.Repaint)
             {
                 if (GUI.GetNameOfFocusedControl () == "searchTerm" && _searchTerm == _searchTermPlaceholder)
                 {
                     _searchTerm = "";
                 }
             }


            var filteredNodes = new List<Node>();
            NodeTypes.ForEach(t => filteredNodes.Add((Node)Activator.CreateInstance(t)));

            filteredNodes = filteredNodes.Where(n => n.ShowInSearch).ToList();

            if(_searchTerm != _searchTermPlaceholder)
            {
                filteredNodes = filteredNodes.Where(n => n.Name.ToLower().Contains(_searchTerm.ToLower()) ||
                    n.SubText.ToLower().Contains(_searchTerm.ToLower()) ||
                    n.Description.ToLower().Contains(_searchTerm.ToLower())).ToList();

                if(!_showStandard)
                {
                    filteredNodes = filteredNodes.Where(n => n.IsRoutine || n is IntComparison || n.NodeType == NodeType.Property).ToList();
                }
                if(!_showRoutines)
                {
                    filteredNodes = filteredNodes.Where(n => !n.IsRoutine).ToList();
                }
                if(!_showConditions)
                {
                    filteredNodes = filteredNodes.Where(n =>!(n is IntComparison)).ToList();
                }
                if(!_showProps)
                {
                    filteredNodes = filteredNodes.Where(n => n.NodeType != NodeType.Property).ToList();
                }
            }

            GUILayout.Space(5);

            _addNodeScrollPos = GUILayout.BeginScrollView(_addNodeScrollPos,false,true, GUILayout.Height(300), GUILayout.Width(400));

            var organisedNodeTree = new OrganisedNodeTree(filteredNodes, GetNodeType());

            foreach(var mainCat in organisedNodeTree.MainCategories)
            {
                var storedMainCat = OrganisedNodeTree.GetMainCat(mainCat.CatergoryName);
                storedMainCat.Show = EditorGUILayout.Foldout(storedMainCat.Show, mainCat.CatergoryName, FoldOutStyle);
                if (storedMainCat.Show)
                {
                    foreach(var subCat in mainCat.SubCategories)
                    {
                        var storedSubCat = OrganisedNodeTree.GetSubCat(mainCat.CatergoryName, subCat.SubCatergoryName);
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(30);



                        storedSubCat.Show = EditorGUILayout.Foldout(storedSubCat.Show, subCat.SubCatergoryName, FoldOutStyle);
                        GUILayout.EndHorizontal();
                        if (storedSubCat.Show)
                        {
                            foreach (var node in subCat.Nodes)
                            {
                                GUILayout.BeginHorizontal();
                                GUILayout.Space(60);
                                if (GUILayout.Button(node.Tag + node.Name, "addNodeType"))
                                {
                                    AddNode(node.GetType(), _nodeAddPosition);
                                    _showAddNodeWindow = false;
                                }
                                GUILayout.EndHorizontal();
                            }
                        }
                    }

                    foreach (var node in mainCat.Nodes)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(30);
                        if (GUILayout.Button(node.Tag + node.Name, "addNodeType"))
                        {
                            AddNode(node.GetType(), _nodeAddPosition);
                            _showAddNodeWindow = false;
                        }
                        GUILayout.EndHorizontal();
                    }
                }
            }

            GUILayout.EndScrollView();
        }

        private void ShowContextMenu(Vector2 nodeAddPosition)
        {
            _nodeAddPosition = nodeAddPosition;
            _addNodeRect = new Rect(_nodeAddPosition.x +5, _nodeAddPosition.y +5, 400, 300);
            _addNodeRect = ClampWindowToViewArea(_addNodeRect);
            _showAddNodeWindow = true;
            _searchTerm = _searchTermPlaceholder;
        }

        private void NodeGUIWindow(int id)
        {
            var node = Nodes.First(n => n.WindowID == id);
            var propNode = node as PropertyNode;
            var isPropertyNode = propNode != null && propNode.NodeType == NodeType.Property;

            if ((Event.current.button == 0) && (Event.current.type == EventType.MouseUp))
            {
                _selectedNode = node;
                _selectedWindow = id;
            }

            GUILayout.BeginVertical();

            //Title
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Box(node.Name, "nodeTitle");
            GUILayout.Box(node.SubText, "nodeSubTitle");
            GUILayout.EndVertical();

            if (isPropertyNode)
            {
                NodeNextLinks(node);
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            //Incoming link + properties            
            if (node.CanBeLinkedTo)
            {
                GUILayout.BeginHorizontal(GUILayout.Height(25), GUILayout.MaxHeight(25));
                if (GUILayout.Button("", "nodeLinkImageIncoming", GUILayout.Width(20), GUILayout.Height(20)))
                {

                    if (Event.current.button == 0)
                    {
                        //Try AddLink or set link to add
                        if (_nodeToLink != null )
                        {
                            if (_nodeToLink.NodeType != NodeType.Property)
                            {
                                LinkNodes(_nodeToLink, node);    
                            }
                            else
                            {
                                Debug.LogWarning("[RPGAIO] Cannot link properties to node execution path.");
                                _nodeToLink = null;
                            }
                            
                        }
                    }
                    else if (Event.current.button == 1)
                    {
                        ClearPrevLinks(node, SelectedNodeTree);
                    }
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            else
            {
                if(!isPropertyNode)
                {
                    GUILayout.Space(20);
                }
                else
                {
                    GUILayout.Space(10);
                }
            }
            GUILayout.FlexibleSpace();

            ShowNodeParams(node);

            GUILayout.EndVertical();

            GUILayout.Space(20);

            //outgoing links
            GUILayout.BeginVertical();
            if (!isPropertyNode)
            {
                NodeNextLinks(node);
            }

            if(!node.HasMaxNextLinks || node.MaxNextLinks > node.NextNodeLinks.Count)
            {
                GUILayout.BeginHorizontal(GUILayout.Height(25), GUILayout.MaxHeight(25));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("", "nodeAddNewSlot", GUILayout.Width(20), GUILayout.Height(20)))
                {
                    if (Event.current.button == 0)
                    {
                        node.AddNodeLinkSlot();
                    }
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            if (!isPropertyNode)
            {
                GUILayout.Space(5);
            }
            GUILayout.EndVertical();
            GUI.DragWindow();
            
            if (_snapWindows)
                node.Rect = RoundWindow(node.Rect);

            node.Rect = ClampWindow(node.Rect);
        }

        private void NodeNextLinks(Node node)
        {
            for (int index = 0; index < node.NextNodeLinks.Count; index++)
            {
                GUILayout.BeginHorizontal(GUILayout.Height(25), GUILayout.MaxHeight(25));
                GUILayout.FlexibleSpace();
                GUILayout.Box(node.NextNodeLinkLabel(index), "nodeLink");
                if (GUILayout.Button(" ", "nodeLinkImage", GUILayout.Width(20), GUILayout.Height(20)))
                {
                    if (Event.current.button == 0)
                    {
                        //Try AddLink or set link to add
                        if (_nodeToLink == null)
                        {
                            _nodeToLink = node;
                            _nodeToLinkIndex = index;
                        }
                        else
                        {
                            Debug.LogWarning("[RPGAIO] You cannot link from outgoing to outgoing.");
                            _nodeToLink = null;
                        }

                    }
                    else if (Event.current.button == 1)
                    {
                        if (!string.IsNullOrEmpty(node.NextNodeLinks[index].ID))
                        {
                            RemoveLink(node, index, SelectedNodeTree);
                        }
                        else
                        {
                            if (node.NextNodeLinks.Count > 1 && node.CanRemoveLinks)
                            {
                                node.NextNodeLinks.RemoveAt(index);
                                node.Rect = new Rect(node.Rect.x, node.Rect.y, node.Rect.width, 50);
                            }
                        }

                    }
                }
                GUILayout.EndHorizontal();
            }
        }

        private void LinkNodes(Node from, Node to)
        {
            if (from == to) return;
            if (!string.IsNullOrEmpty(from.NextNodeLinks[_nodeToLinkIndex].ID))
            {
                RemoveLink(from, _nodeToLinkIndex, SelectedNodeTree);
            }
            from.NextNodeLinks[_nodeToLinkIndex].ID = to.ID;
            to.PrevNodeLinks.Add(new StringField { ID = from.ID });
            _nodeToLink = null;
            UpdateLinks();
        }

        private void LinkNodes(Node from, NodeParameter to)
        {
            var validProp = ValidateNode(from, to);

            if(!validProp)
            {
                Debug.LogWarning("[RPGAIO] Invalid object for this property.");
                _nodeToLink = null;
                return;
            }

            if (!string.IsNullOrEmpty(from.NextNodeLinks[_nodeToLinkIndex].ID))
            {
                RemoveLink(from, _nodeToLinkIndex, SelectedNodeTree);
            }
            from.NextNodeLinks[_nodeToLinkIndex].ID = null;
            to.InputNodeId.ID = from.ID;
            _nodeToLink = null;
            UpdateLinks();
        }

        private bool ValidateNode(Node from, NodeParameter to)
        {
            var propNode = from as PropertyNode;
            if(propNode != null)
            {

                if (propNode.PropertyFamily != PropertyFamily.Any && propNode.PropertyFamily != to.PropertyFamily)
                {
                    return false;
                }

                return propNode.PropertyType == PropertyType.Any || propNode.PropertyType == to.PropertyType ||
                       propNode.PropertyType == PropertyType.Float && to.PropertyType == PropertyType.Int ||
                       propNode.PropertyType == PropertyType.Int && to.PropertyType == PropertyType.Float ||
                       propNode.PropertyType == PropertyType.GameObject &&
                       to.PropertyType == PropertyType.CombatCharacter;
            }
            else
            {
                Debug.LogWarning("[RPGAIO] Cannot link non-property node to node execution path.");
                return false;
            }
        }

        private void ClearPrevLinks(Node node, NodeTree nodeTree)
        {
            foreach (var link in node.PrevNodeLinks)
            {
                var foundNode = nodeTree.Nodes.FirstOrDefault(n => n.ID == link.ID);
                if(foundNode != null)
                {
                    foundNode.NextNodeLinks.Where(n => n.ID == node.ID).ToList().ForEach(s => s.ID = "");
                }
            }

            foreach (var n in nodeTree.Nodes)
            {
                foreach (var v in n.Parameters.Values.Where(v => v.InputNodeId.ID == node.ID))
                {
                    v.InputNodeId.ID = "";
                }
            }

            node.PrevNodeLinks = new List<StringField>();
            UpdateLinks();
        }

        private void UpdateLinks()
        {
            if (SelectedNodeTree == null) return;

            _allLinks = new List<object[]>();
            foreach (var node in Nodes)
            {
                for (int index = 0; index < node.NextNodeLinks.Count; index++)
                {
                    var link = node.NextNodeLinks[index];
                    if (!string.IsNullOrEmpty(link.ID))
                    {
                        var foundNode = Nodes.FirstOrDefault(n => n.ID == link.ID);
                        if (foundNode != null)
                        {
                            _allLinks.Add(new object[] { node, foundNode, index });
                        }
                    }
                }

                var parameterNumber = 0;
                var vals = node.Parameters.Values.ToArray();
                foreach (var v in vals)
                {
                    var i = parameterNumber;
                    var linkId = v.InputNodeId.ID;
                    if(!string.IsNullOrEmpty(linkId))
                    {
                        var foundNode = Nodes.FirstOrDefault(n => n.ID == linkId);
                        _allLinks.Add(new object[] { foundNode, node, i, null });    
                    }
                    parameterNumber++;

                    foreach(var subParam in v.SubParams.Values)
                    {
                        var parameter = subParam;
                        AddSubParamLinks(v,parameter, node, vals, ref parameterNumber);
                    }
                }
            }
        }


        private void AddSubParamLinks(NodeParameter parentParam, SubNodeParameter subNodeParam, Node node, NodeParameter[] vals, ref int parameterNumber)
        {
            var isShowing = subNodeParam.Condition(parentParam);
            if(!isShowing) return;

            var i = parameterNumber; //Needs to be the index inside of all parameters including subparams
            var parameter = subNodeParam.Parameter;
            var linkId = parameter.InputNodeId.ID;
            if (!string.IsNullOrEmpty(linkId))
            {
                var foundNode = Nodes.FirstOrDefault(n => n.ID == linkId);
                _allLinks.Add(new object[] { foundNode, node, i, null });
            }
            parameterNumber++;

            if (parameter.SubParams.Any())
            {
                foreach (var subParam in parameter.SubParams.Values)
                {
                    var p = subParam;
                    AddSubParamLinks(parameter, p, node, vals, ref parameterNumber);
                }    
            }
        }

        private void RemoveLink(Node node, int linkIndex, NodeTree nodeTree)
        {
            var linkedNodeID = node.NextNodeLinks[linkIndex];
            var foundNode = nodeTree.Nodes.FirstOrDefault(n => n.ID == linkedNodeID.ID);
            if (foundNode != null)
            {
                var link = foundNode.PrevNodeLinks.FirstOrDefault(s => s.ID == node.ID);
                if(link != null)
                {
                    foundNode.PrevNodeLinks.Remove(link);
                }
            }
            linkedNodeID.ID = "";
            UpdateLinks();
        }

        private void ShowNodeParams(Node node)
        {
            foreach(var param in node.Parameters.Values)
            {
                ShowNodeParam(param);
            }
        }

        private void ShowNodeParam(NodeParameter parameter, int nestedLevel = 0)
        {

            GUILayout.BeginHorizontal();

            if((parameter.Source == PropertySource.InputOnly || parameter.Source == PropertySource.EnteredOrInput))
            {
                var style = string.IsNullOrEmpty(parameter.InputNodeId.ID) ? "nodeLinkIncomingPropAlt" : "nodeLinkIncomingProp";
                if (GUILayout.Button("", style, GUILayout.Width(20), GUILayout.Height(20)))
                {

                    if (Event.current.button == 0)
                    {
                        //Try AddLink or set link to add
                        if (_nodeToLink != null)
                        {
                            LinkNodes(_nodeToLink, parameter);
                        }
                    }
                    else if (Event.current.button == 1)
                    {
                        parameter.InputNodeId.ID = "";
                        UpdateLinks();
                    }
                }
            }

            var prefix = ("").PadLeft(nestedLevel * 3) + (nestedLevel > 0 ? "- " : "");
            GUILayout.Label(prefix + parameter.Name);


            if (parameter.Source == PropertySource.EnteredOnly || (parameter.Source == PropertySource.EnteredOrInput && string.IsNullOrEmpty(parameter.InputNodeId.ID)))
            {
                switch (parameter.PropertyType)
                {
                    case PropertyType.String:
                        if (parameter.Value == null) parameter.Value = "";
                        parameter.Value = GUILayout.TextField((string)parameter.Value, "nodeTextField", GUILayout.MinWidth(80));
                        break;
                    case PropertyType.TextArea:
                        if (parameter.Property == null || parameter.Property.GetType() != typeof(RPGVector2))
                        {
                            parameter.Property = new RPGVector2(0, 0);
                        }

                        var paramText = (string)parameter.Value;
                        var rpgVector2 = (RPGVector2)parameter.Property;
                        Vector2 vector = rpgVector2;
                        vector = ScrollableTextArea(vector, ref paramText, GUI.skin.GetStyle("nodeTextArea"));
                        parameter.Property = new RPGVector2(vector.x, vector.y);
                        parameter.Value = paramText;
                        break;
                    case PropertyType.Int:
                        parameter.Value = EditorGUILayout.IntField(Convert.ToInt32(parameter.Value), GUILayout.MinWidth(80));
                        break;
                    case PropertyType.Float:
                        parameter.Value = EditorGUILayout.FloatField(Convert.ToSingle(parameter.Value), GUILayout.MinWidth(80));
                        break;
                    case PropertyType.Bool:
                        parameter.Value = GUILayout.Toggle((bool)parameter.Value, "");
                        break;
                    case PropertyType.StringArray:
                        {
                            var z = (string[])parameter.Property;

                            parameter.Value = EditorGUILayout.Popup(Convert.ToInt32(parameter.Value),
                                                                                    z, GUILayout.MinWidth(120));
                        }
                        break;
                    case PropertyType.IntArray:
                        {
                            var z = (int[])parameter.Property;  

                            parameter.Value = EditorGUILayout.Popup(Convert.ToInt32(parameter.Value),
                                                                                    z.Select(i => i.ToString(CultureInfo.InvariantCulture)).ToArray(), GUILayout.MinWidth(120));
                        }
                        break;
                    case PropertyType.Enum:
                        parameter.Value = EditorGUILayout.EnumPopup((Enum)parameter.Value, GUILayout.MinWidth(120));
                        break;
                    case PropertyType.Attribute:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<Rm_AttributeDefintion>("", ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.Statistic:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<Rm_StatisticDefintion>("", ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.Vital:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<Rm_VitalDefinition>("", ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.Trait:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<Rm_TraitDefintion>("", ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.StatusEffect:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<StatusEffect>("", ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.Skill:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<Skill>("", ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.Talent:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<Talent>("", ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.TraitExpDefinition:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<ExpDefinition>("", ref _popupIDValue, "ID", "Name", "Trait");
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.ExpGainedDefinition:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<ExpDefinition>("", ref _popupIDValue, "ID", "Name", "ExpGained");
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.PlayerLevellingDefintion:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<ExpDefinition>("", ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.TalentGroup:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<TalentGroup>("", ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.Item:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<Item>("", ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.CraftableItem:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<Item>("", ref _popupIDValue, "ID", "Name", "Craft");
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.QuestItem:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<Item>("", ref _popupIDValue, "ID", "Name", "Quest");
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.Rm_ClassDefinition:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<Rm_ClassDefinition>("", ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.Rmh_CustomVariable:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<Rmh_CustomVariable>("", ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.ReputationDefinition:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<ReputationDefinition>("", ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.SkillMetaDefinition:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<SkillMetaDefinition>("", ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.Quest:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<Quest>("", ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.NonPlayerCharacter:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<NonPlayerCharacter>("", ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.Interactable:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<Interactable>("", ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.SlotDefinition:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<SlotDefinition>("", ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.WeaponTypeDefinition:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<WeaponTypeDefinition>("", ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.MetaData:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<Rm_MetaDataDefinition>("", ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.RarityDefinition:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<RarityDefinition>("", ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.CombatCharacter:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<CombatCharacter>("", ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.Rm_LootTable:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<Rm_LootTable>("", ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.Achievement:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<Achievement>("", ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.MonsterTypeDefinition:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<MonsterTypeDefinition>("", ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.PetDefinition:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<Rm_PetDefinition>("", ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.Event:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<NodeChain>("", ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.Dialog:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<NodeTree>("", ref _popupIDValue, "ID","Name","Dialog");
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.VendorShop:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<VendorShop>("", ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.Sound:
                        _popupIDValue = (string)parameter.Value;
                        _audioClipHolder = RPGMakerGUI.AudioClipSelector("", _audioClipHolder, ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.GameObject:
                        _popupIDValue = (string)parameter.Value;
                        _gameObjectHolder = RPGMakerGUI.PrefabSelector("", _gameObjectHolder, ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.Sprite:
                        _popupIDValue = (string)parameter.Value;
                        _spriteHolder = RPGMakerGUI.SpriteSelector("", _spriteHolder, ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.Vector2:
                        parameter.Value = RPGMakerGUI.Vector2Field("", (RPGVector2)parameter.Value,0, GUILayout.MinWidth(40));
                        break;
                    case PropertyType.Vector3:
                        parameter.Value = RPGMakerGUI.Vector3Field("", (RPGVector3)parameter.Value,0, GUILayout.MinWidth(40));
                        break;
                    case PropertyType.Texture2D:
                        _popupIDValue = (string)parameter.Value;
                        _texture2DHolder = RPGMakerGUI.ImageSelector("", _texture2DHolder, ref _popupIDValue, true);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.NodeChain:
                        _popupIDValue = (string)parameter.Value;
                        RPGMakerGUI.PopupID<NodeChain>("", ref _popupIDValue);
                        parameter.Value = _popupIDValue;
                        break;
                    case PropertyType.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal(); 

            foreach(var subParam in parameter.SubParams.Values)
            {
                if(subParam.Condition(parameter))
                {
                    ShowNodeParam(subParam.Parameter, nestedLevel + 1);
                }
            }
        }

        private void GenericParam(NodeParameter parameter, int nestedLevel)
        {
            _selectedIndex = ((object[])parameter.Value)[0] != null ? (int)((object[])parameter.Value)[0] : 0;
            ((object[])parameter.Value)[0] = _selectedIndex;
            var type = (GenericValue)Enum.Parse(typeof(GenericValue), GetGenericOptions((VarType)parameter.Property)[_selectedIndex]);

            if(type == GenericValue.Auto)
            {
                return;
            }
            
            GUILayout.BeginHorizontal();
            var prefix = ("").PadLeft(nestedLevel * 3) + (nestedLevel > 0 ? "- " : "");
            GUILayout.Label(prefix + parameter.Name);
            
//            if (parameter.Value == null)
//            {
//                parameter.Value = new object[] {null, null, null, null};
//            }
//            else if (parameter.Value.GetType() != typeof(object[]))
//            {
//                var s = parameter.Value as JArray;
//                parameter.Value = (s == null ? null : s.ToObject<object[]>()) ?? new object[] { null, null, null, null };
//            }
//
//            if (parameter.Property.GetType() != typeof(VarType))
//            {
//                parameter.Property = Enum.ToObject(typeof(VarType), Convert.ToInt32(parameter.Property));
//            }
//
//            var ad = (object[]) parameter.Value;
//            for (int i = 0; i < ad.Length; i++)
//            {
//                var obj = ad[i];
//                if (obj is long)
//                {
//                    ad[i] = Convert.ToInt32(ad[i]);
//                }
//            }

            _selectedIndex = ((object[])parameter.Value)[0] != null ? (int)((object[])parameter.Value)[0] : 0;
            _selectedIndex = EditorGUILayout.Popup(_selectedIndex, GetGenericOptions((VarType)parameter.Property));
            ((object[])parameter.Value)[0] = _selectedIndex;

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            //back in vertical
            type = (GenericValue)Enum.Parse(typeof(GenericValue), GetGenericOptions((VarType)parameter.Property)[_selectedIndex]);
            switch (type)
            {
                case GenericValue.Whole_Number:
                    break;
                case GenericValue.Number:
                    break;
                case GenericValue.Boolean:
                    break;
                case GenericValue.Text:
                    break;
                case GenericValue.Physical_Damage:
                    break;
                case GenericValue.Total_Elemental_Damage:
                    break;
                case GenericValue.Elemental_Damage:
                    break;
                case GenericValue.Total_Damage:
                    break;
                case GenericValue.Node_Chain_Result:
                    break;
                case GenericValue.Attribute_Value:
                    _popupIDValue = ((object[])parameter.Value)[1] != null ? (string)((object[])parameter.Value)[1] : "";

                    RPGMakerGUI.PopupID<Rm_AttributeDefintion>(("").PadLeft(nestedLevel * 3) + "- Attribute", ref _popupIDValue);
                    ((object[]) parameter.Value)[1] = _popupIDValue;

                    _selectedIndex = ((object[]) parameter.Value)[2] != null ? (int)((object[])parameter.Value)[2] : 0;
                    _selectedIndex = EditorGUILayout.Popup(("").PadLeft(nestedLevel*3) + "- From", _selectedIndex, GetGenericTargets());
                    ((object[]) parameter.Value)[2] = _selectedIndex;

                    _selectedIndex = ((object[]) parameter.Value)[3] != null ? (int)((object[])parameter.Value)[3] : 0;
                    _selectedIndex = EditorGUILayout.Popup(("").PadLeft(nestedLevel*3) + "- Value", _selectedIndex, new string[] {"Base", "Total", "Skill"});
                    ((object[]) parameter.Value)[3] = _selectedIndex;

                    break;
                case GenericValue.Statistic_Value:
                    break;
                case GenericValue.Trait_Level:
                    break;
                case GenericValue.Vital_Value:
                    break;
                case GenericValue.Level:
                    break;
                case GenericValue.Enemy_Type:
                    break;
                case GenericValue.Position:
                    break;
                case GenericValue.Custom_Variable:
                    break;
                case GenericValue.Random:
                    break;
                case GenericValue.Auto:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string[] GetGenericOptions(VarType varType)
        {
            var options = new List<GenericValue>();

            switch(varType)
            {
                case VarType.Whole_Number:
                    options.AddRange(new []
                                         {
                                            GenericValue.Whole_Number,
                                            GenericValue.Node_Chain_Result,
                                            GenericValue.Attribute_Value,
                                            GenericValue.Statistic_Value,
                                            GenericValue.Trait_Level,
                                            GenericValue.Vital_Value,
                                            GenericValue.Level,
                                            GenericValue.Custom_Variable,
                                            GenericValue.Random
                                         });
                    break;
                case VarType.Number:
                    options.AddRange(new[]
                                         {                                            
                                            GenericValue.Whole_Number,
                                            GenericValue.Number,
                                            GenericValue.Node_Chain_Result,
                                            GenericValue.Attribute_Value,
                                            GenericValue.Statistic_Value,
                                            GenericValue.Trait_Level,
                                            GenericValue.Vital_Value,
                                            GenericValue.Level,
                                            GenericValue.Enemy_Type,
                                            GenericValue.Custom_Variable,
                                            GenericValue.Random
                                         });
                    break;
                case VarType.String:
                    options.AddRange(new[]
                                         {
                                            GenericValue.Text,
                                            GenericValue.Node_Chain_Result,
                                            GenericValue.Custom_Variable
                                         });
                    break;
                case VarType.Boolean: options.AddRange(new[]
                                         {
                                            GenericValue.Boolean,
                                            GenericValue.Node_Chain_Result,
                                            GenericValue.Custom_Variable
                                         });
                    break;
                case VarType.Position: options.AddRange(new[]
                                         {
                                            GenericValue.Position,
                                            GenericValue.Node_Chain_Result
                                         });
                    break;
            }

            switch(SelectedNodeTree.Type)
            {
                case NodeTreeType.Combat:
                    if(SelectedNodeTree.ID == Rm_RPGHandler.Instance.Nodes.DamageTakenTree.ID)
                    {
                        options.AddRange(new []
                                             {
                                                  GenericValue.Physical_Damage,
                                                  GenericValue.Elemental_Damage,
                                                  GenericValue.Total_Elemental_Damage,
                                                  GenericValue.Total_Damage,
                                             });
                    }
                    break;
                case NodeTreeType.Dialog:
                    break;
                case NodeTreeType.Event:
                    break;
                case NodeTreeType.WorldMap:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if(options.Count == 0)
            {
                options.Add(GenericValue.Auto);
            }

            return options.Select(x => x.ToString()).ToArray();
        }

        private string[] GetGenericTargets()
        {
            return new[] { "Attacker", "Defender" };
        }

        private void AddNode(Type nodeType, Vector2 pos)
        {
            var n = (Node)Activator.CreateInstance(nodeType);
            //var n = GeneralMethods.CopyObject(nodeType);
            n.OnCreate();

            n.ID = Guid.NewGuid().ToString();
            n.WindowID = GetNextID(NodeBank);
            n.Rect = new Rect(pos.x, pos.y, DefaultNodeWidth, DefaultNodeHeight);
            Nodes.Add(n); 
        }

        private void AddPropertyNode(NodeTreeVar treeVar)
        {
            if (treeVar.Name == "Attacker")
            {
                var n = new AttackerCombatNode();
                n.OnCreate();
                n.ID = Guid.NewGuid().ToString();
                n.WindowID = GetNextID(NodeBank);
                n.Rect = new Rect(100 + _nodeScrollPos.x, 100 + _nodeScrollPos.y, DefaultNodeWidth, DefaultNodeHeight);
                Nodes.Add(n); 
            }
            else if(treeVar.Name == "Defender")
            {
                var n = new DefenderCombatNode();
                n.OnCreate();
                n.ID = Guid.NewGuid().ToString();
                n.WindowID = GetNextID(NodeBank);
                n.Rect = new Rect(100 + _nodeScrollPos.x, 100 + _nodeScrollPos.y, DefaultNodeWidth, DefaultNodeHeight);
                Nodes.Add(n); 
            }
            else if (treeVar.ID == "DamageDealtVar_Physical")
            {
                var n = new PhysicalDamageCombatNode();
                n.OnCreate();
                n.ID = Guid.NewGuid().ToString();
                n.WindowID = GetNextID(NodeBank);
                n.Rect = new Rect(100 + _nodeScrollPos.x, 100 + _nodeScrollPos.y, DefaultNodeWidth, DefaultNodeHeight);
                Nodes.Add(n); 
            }
            else if (treeVar.ID.StartsWith("DamageDealtVar"))
            {
                var n = new CustomDamageCombatNode();
                n.OnCreate();
                n.ID = Guid.NewGuid().ToString();
                n.DamageId = treeVar.ID.Replace("DamageDealtVar_", "");
                n.NewName = RPG.Combat.GetElementalNameById(n.DamageId) + " Damage";
                n.WindowID = GetNextID(NodeBank);
                n.Rect = new Rect(100 + _nodeScrollPos.x, 100 + _nodeScrollPos.y, DefaultNodeWidth, DefaultNodeHeight);
                Nodes.Add(n); 
            }
            else
            {
                var n = new NodeTreeVarNode();
                if (treeVar.IsList)
                {
                    n.NodePropertyFamily = PropertyFamily.List;
                }
                else
                {
                    if (new[] { PropertyType.String, PropertyType.Float, PropertyType.Int, PropertyType.Bool }.Any(x => treeVar.PropType == x))
                    {
                        n.NodePropertyFamily = PropertyFamily.Primitive;
                    }
                    else
                    {
                        n.NodePropertyFamily = PropertyFamily.Object;
                    }
                }

                n.NodePropertyType = treeVar.PropType;
                n.NewName = treeVar.Name;
                n.VariableId = treeVar.ID;
                n.OnCreate();

                n.ID = Guid.NewGuid().ToString();
                n.WindowID = GetNextID(NodeBank);
                n.Rect = new Rect(100 + _nodeScrollPos.x, 100 + _nodeScrollPos.y, DefaultNodeWidth, DefaultNodeHeight);
                Nodes.Add(n); 
            }
        }

        protected static int GetNextID(NodeBank nodeBank)
        {
            //todo: nodetree
            return nodeBank.NodeTrees.SelectMany(n => n.Nodes).Any()
                       ? (nodeBank.NodeTrees.SelectMany(n => n.Nodes).Max(n => n.WindowID) + 1)
                       : 0;
        }

        private Rect RoundWindow(Rect rect)
        {
            rect.x = rect.x.RoundToNearest(12.5f);
            rect.y = rect.y.RoundToNearest(12.5f);
            rect.width = rect.width.RoundToNearest(12.5f);
            rect.height = rect.height.RoundToNearest(12.5f);
            return rect;
        }

        private Rect ClampWindow(Rect rect)
        {
//            rect.x = Mathf.Clamp(rect.x, 0, ScrollDimensions - rect.width);
//            rect.y = Mathf.Clamp(rect.y, 0, ScrollDimensions - rect.height);
            rect.x = Mathf.Clamp(rect.xMax, rect.width, ScrollDimensions) - rect.width;
            rect.y = Mathf.Clamp(rect.yMax, rect.height, ScrollDimensions) - rect.height;
            return rect;
        }
        private Rect ClampWindowToViewArea(Rect rect)
        {
            rect.x = Mathf.Clamp(rect.xMax, rect.width + _nodeScrollPos.x, position.width + _nodeScrollPos.x - 25) - rect.width;
            rect.y = Mathf.Clamp(rect.yMax, rect.height + _nodeScrollPos.y, position.height + _nodeScrollPos.y - 25) - rect.height;
            return rect;
        }

        protected Node GetNodeObject(Type type)
        {
            return (Node)Activator.CreateInstance(type);
        }

        public static NodeTree GetNewTree(NodeTreeType nodeType)
        {
            int newCount;
            NodeBank nodeBank;
            switch (nodeType)
            {
                case NodeTreeType.Combat:
                    newCount = Rm_RPGHandler.Instance.Nodes.CombatNodeBank.NodeTrees.Count;
                    nodeBank = Rm_RPGHandler.Instance.Nodes.CombatNodeBank;
                    break;
                case NodeTreeType.Dialog:
                    newCount = Rm_RPGHandler.Instance.Nodes.DialogNodeBank.NodeTrees.Count;
                    nodeBank = Rm_RPGHandler.Instance.Nodes.DialogNodeBank;
                    break;
                case NodeTreeType.Event:
                    newCount = Rm_RPGHandler.Instance.Nodes.EventNodeBank.NodeTrees.Count;
                    nodeBank = Rm_RPGHandler.Instance.Nodes.EventNodeBank;
                    break;
                case NodeTreeType.Achievements:
                    newCount = Rm_RPGHandler.Instance.Nodes.AchievementsNodeBank.NodeTrees.Count;
                    nodeBank = Rm_RPGHandler.Instance.Nodes.AchievementsNodeBank;
                    break;
                case NodeTreeType.WorldMap:
                    newCount = Rm_RPGHandler.Instance.Nodes.WorldMapNodeBank.NodeTrees.Count;
                    nodeBank = Rm_RPGHandler.Instance.Nodes.WorldMapNodeBank;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var tree = new NodeTree { Name = "New Tree" + newCount , Type = nodeType};
            switch (nodeType)
            {
                case NodeTreeType.Combat:
                    var node = new CombatStartNode("Min Physical Damage", "Start node for dealt physical",
                                        "Physical Damage represents weapon damage, or base combatant damage. You can then add additional nodes to add onto this value. Example: Add 10x Strength") {Identifier = "MIN_Physical"};
                    node.OnCreate();
                    tree.AddNode(node, new Vector2(25, 87.5f), 0);
                    var nodeMax = new CombatStartNode("Max Physical Damage", "Start node for dealt physical",
                                                    "Physical Damage represents weapon damage, or base combatant damage. You can then add additional nodes to add onto this value. Example: Add 10x Strength") {Identifier = "MAX_Physical"};
                    nodeMax.OnCreate();
                    tree.AddNode(nodeMax, new Vector2(25, 287.5f), 1);
                    tree.Variables.Add(new NodeTreeVar("Attacker", PropertyType.CombatCharacter));
                    break;
                case NodeTreeType.Dialog:
                    tree.AddNode(new DialogStartNode(), Vector2.zero, GetNextID(nodeBank));
                    break;
                case NodeTreeType.Event:
                    tree.AddNode(new EventStartNode(), Vector2.zero, GetNextID(nodeBank));
                    break;
                case NodeTreeType.Achievements:
                    tree.AddNode(new AchievementStartNode(), Vector2.zero, GetNextID(nodeBank));
                    tree.AddNode(new AchievementMinProgress(), new Vector2(0, 300), (GetNextID(nodeBank) + 1));
                    tree.AddNode(new AchievementMaxProgress(), new Vector2(0, 600), (GetNextID(nodeBank) + 2));
                    tree.AddNode(new AchievementEndNode(), new Vector2(300,0), (GetNextID(nodeBank) + 3));
                    break;
                case NodeTreeType.WorldMap:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return tree;
        }

        private Vector2 ScrollableTextArea(Vector2 scrollPos, ref string text, GUIStyle style)
        {
            const float width = 200;

            var pixelHeight = style.CalcHeight(new GUIContent(text), width);

            if(pixelHeight > 200)
                scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(Mathf.Min(pixelHeight + 10,200)), GUILayout.Width(width));

            text = EditorGUILayout.TextArea(text, style, GUILayout.MinHeight(pixelHeight));
            
            if (pixelHeight > 200)
                GUILayout.EndScrollView();

            return scrollPos;
        }
    }
}
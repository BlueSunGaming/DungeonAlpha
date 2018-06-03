//using System.Linq;
//using LogicSpawn.RPGMaker;
//using UnityEditor;
//using UnityEngine;
//
//namespace LogicSpawn.RPGMaker.Editor
//{
//    public class Rme_Node_Dialog : Rme_NodeWindow
//    {
//        [MenuItem("LogicSpawn RPG Maker/Node Editors/Dialog Manager", false,2)]
//        public static void Init()
//        {
//            Window = (Rme_Node_Dialog)GetWindow(typeof(Rme_Node_Dialog));
//            Window.WindowType = NodeTreeType.Dialog;
//            Window.PrimaryTrees = new string[0];
//
//            Window.title = "DialogManager";
//            Window.minSize = new Vector2(1000.0F, 600.0F);
//            Window.position = new Rect(100, 100, 1000, 600);
//        }
//
//        
//
//        public static void ShowWindow(string id)
//        {
//            Init();
//            Window.SelectedNodeTree = Window.NodeBank.NodeTrees.FirstOrDefault(n => n.ID == id);
//            Window.SelectedNodeTreeIndex = Window.NodeBank.NodeTrees.IndexOf(Window.SelectedNodeTree);
//        }
//
//        public override sealed void InitNodes()
//        {
//            if (Nodes.Count == 0)
//            {
//                var startingNode = new Rm_Node { NodeType = Rm_NodeType.Start_Dialog };
//                startingNode.NextPoints.Add(new Rm_NodeLink(startingNode));
//                startingNode.Rect.x = startingNode.Rect.y = 30;
//                Nodes.Add(startingNode);
//
//                var endNode = new Rm_Node { NodeType = Rm_NodeType.End_Dialog };
//                endNode.NextPoints.Add(new Rm_NodeLink(endNode));
//                endNode.Rect.x = 300;
//                endNode.Rect.y = 30;
//                Nodes.Add(endNode);
//            }
//        }
//
//        public override sealed void AddContextMenu(Event evt, Vector2 mousePos)
//        {
//            var menu = new GenericMenu();
//
//            menu.AddItem(new GUIContent("Add NPC Response"), false, AddNode(), new object[] { Rm_NodeType.NPC_Response, mousePos });
//            menu.AddItem(new GUIContent("Add Player Response"), false, AddNode(), new object[] { Rm_NodeType.Player_Response, mousePos });
//            menu.AddItem(new GUIContent("Add Begin Quest"), false, AddNode(), new object[] { Rm_NodeType.Begin_Quest, mousePos });
//            menu.AddItem(new GUIContent("Add Complete Quest"), false, AddNode(), new object[] { Rm_NodeType.Complete_Quest, mousePos });
//            menu.AddItem(new GUIContent("Add End - Vendor"), false, AddNode(), new object[] { Rm_NodeType.End_Dialog_Vendor, mousePos });
//            menu.AddItem(new GUIContent("Add End - Crafting"), false, AddNode(), new object[] { Rm_NodeType.End_Dialog_Crafting, mousePos });
//
//            menu.AddSeparator("");
//            menu.AddItem(new GUIContent("Add Event Node"), false, AddNode(), new object[] { Rm_NodeType.Run_Event, mousePos });
//            menu.AddItem(new GUIContent("ddAdd Variable Setter Node"), false, AddNode(), new object[] { Rm_NodeType.VarSetter, mousePos });
//            menu.AddItem(new GUIContent("Add Condition Node"), false, AddNode(), new object[] { Rm_NodeType.Condition, mousePos });
//
//            menu.ShowAsContext();
//            evt.Use();
//        }
//    }
//}
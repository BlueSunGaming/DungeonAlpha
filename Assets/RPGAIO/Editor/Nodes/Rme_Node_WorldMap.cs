//using LogicSpawn.RPGMaker;
//using UnityEditor;
//using UnityEngine;
//
//namespace LogicSpawn.RPGMaker.Editor
//{
//    public class Rme_Node_WorldMap : Rme_NodeWindow
//    {
//        [MenuItem("LogicSpawn RPG Maker/Node Editors/World Map", false,4)]
//        public static void Init()
//        {
//            Window = (Rme_Node_WorldMap)GetWindow(typeof(Rme_Node_WorldMap));
//            Window.WindowType = NodeTreeType.WorldMap;
//            Window.PrimaryTrees = new string[]{"Core_StartingMap"};
//
//            Window.title = "WorldMap";
//            Window.minSize = new Vector2(1000.0F, 600.0F);
//            Window.position = new Rect(100, 100, 1000, 600);
//        }
//
//        public override sealed void InitNodes()
//        {
//            if (Nodes.Count == 0)
//            {
//                var startingNode = new Rm_Node { NodeType = Rm_NodeType.Start_Combat };
//                startingNode.NextPoints.Add(new Rm_NodeLink(startingNode));
//                startingNode.Rect.x = startingNode.Rect.y = 30;
//                Nodes.Add(startingNode);
//            }
//        }
//
//        public override sealed void AddContextMenu(Event evt, Vector2 mousePos)
//        {
//            var menu = new GenericMenu();
//
//            menu.AddItem(new GUIContent("ddAdd Calculation Node"), false, AddNode(), new object[] { Rm_NodeType.Calculate, mousePos });
//            menu.AddItem(new GUIContent("ddAdd Random Node"), false, AddNode(), new object[] { Rm_NodeType.Random, mousePos });
//            menu.AddItem(new GUIContent("ddAdd Comparison Node"), false, AddNode(), new object[] { Rm_NodeType.Comparison, mousePos });
//            menu.AddItem(new GUIContent("ddAdd Condition Node"), false, AddNode(), new object[] { Rm_NodeType.Condition, mousePos });
//            menu.AddItem(new GUIContent("ddAdd Boolean Node"), false, AddNode(), new object[] { Rm_NodeType.Boolean, mousePos });
//            menu.AddItem(new GUIContent("ddAdd Variable Setter Node"), false, AddNode(), new object[] { Rm_NodeType.VarSetter, mousePos });
//            menu.AddItem(new GUIContent("Add Event Node"), false, AddNode(), new object[] { Rm_NodeType.Run_Event, mousePos });
//            menu.AddItem(new GUIContent("ddAdd Variable Node"), false, AddNode(), new object[] { Rm_NodeType.Start_Combat_Var, mousePos });
//            menu.AddItem(new GUIContent("ddAdd Result Node"), false, AddNode(), new object[] { Rm_NodeType.Result_Node, mousePos });
//
//            menu.ShowAsContext();
//            evt.Use();
//        }
//    }
//}
//using LogicSpawn.RPGMaker;
//using UnityEditor;
//using UnityEngine;
//
//namespace LogicSpawn.RPGMaker.Editor
//{
//    public class Rme_Node_Events : Rme_NodeWindow
//    {
//        [MenuItem("LogicSpawn RPG Maker/Node Editors/Event Manager", false,2)]
//        public static void Init()
//        {
//            Window = (Rme_Node_Events)GetWindow(typeof(Rme_Node_Events));
//            Window.WindowType = NodeTreeType.Event;
//            Window.PrimaryTrees = new string[0];
//
//            Window.title = "EventManager";
//            Window.minSize = new Vector2(1000.0F, 600.0F);
//            Window.position = new Rect(100, 100, 1000, 600);
//        }
//
//        public override sealed void InitNodes()
//        {
//            if (Nodes.Count == 0)
//            {
//                var startingNode = new Rm_Node { NodeType = Rm_NodeType.Start_Event };
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
//            menu.AddItem(new GUIContent("eeAdd Calculation Node"), false, AddNode(), new object[] { Rm_NodeType.Calculate, mousePos });
//            menu.AddItem(new GUIContent("eeAdd Random Node"), false, AddNode(), new object[] { Rm_NodeType.Random, mousePos });
//            menu.AddItem(new GUIContent("eeAdd Comparison Node"), false, AddNode(), new object[] { Rm_NodeType.Comparison, mousePos });
//            menu.AddItem(new GUIContent("eeAdd Condition Node"), false, AddNode(), new object[] { Rm_NodeType.Condition, mousePos });
//            menu.AddItem(new GUIContent("eeAdd Boolean Node"), false, AddNode(), new object[] { Rm_NodeType.Boolean, mousePos });
//            menu.AddItem(new GUIContent("eeAdd Variable Setter Node"), false, AddNode(), new object[] { Rm_NodeType.VarSetter, mousePos });
//            menu.AddItem(new GUIContent("Add Event Node"), false, AddNode(), new object[] { Rm_NodeType.Run_Event, mousePos });
//            menu.AddItem(new GUIContent("eeAdd Variable Node"), false, AddNode(), new object[] { Rm_NodeType.Start_Combat_Var, mousePos });
//            menu.AddItem(new GUIContent("eeAdd Result Node"), false, AddNode(), new object[] { Rm_NodeType.Result_Node, mousePos });
//
//            menu.ShowAsContext();
//            evt.Use();
//        }
//    }
//}
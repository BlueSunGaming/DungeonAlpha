//using System.Linq;
//using LogicSpawn.RPGMaker;
//using UnityEditor;
//using UnityEngine;
//
//namespace LogicSpawn.RPGMaker.Editor
//{
//    public class Rme_Node_CombatCalc : Rme_NodeWindow
//    {
//        [MenuItem("LogicSpawn RPG Maker/Node Editors/Damage Calculator", false, 1)]
//        public static void Init()
//        {
//            // Get existing open window or if none, make a new one:
//
//            Window = (Rme_Node_CombatCalc)GetWindow(typeof(Rme_Node_CombatCalc));
//            Window.WindowType = NodeTreeType.Combat;
//            Window.PrimaryTrees =new []{ "Core_DamageTaken", "Core_DamageDealt"};
//
//            Window.title = "CombatCalc";
//            Window.minSize = new Vector2(1000.0F, 600.0F);
//            Window.position = new Rect(100, 100, 1000, 600);
//        }
//                
//        public override sealed void AddContextMenu(Event evt, Vector2 mousePos)
//        {
//            var menu = new GenericMenu();
//
//            menu.AddItem(new GUIContent("Add Calculation Node"), false, AddNode(), new object[]{Rm_NodeType.Calculate,mousePos});
//            menu.AddItem(new GUIContent("Add Random Node"), false, AddNode(), new object[] { Rm_NodeType.Random, mousePos });
//            menu.AddItem(new GUIContent("Add Comparison Node"), false, AddNode(), new object[] { Rm_NodeType.Comparison, mousePos });
//            menu.AddItem(new GUIContent("Add Condition Node"), false, AddNode(), new object[] { Rm_NodeType.Condition, mousePos });
//            menu.AddItem(new GUIContent("Add IsPlayerNode"), false, AddNode(), new object[] { Rm_NodeType.IsPlayerNode, mousePos });
//            
//            if (SelectedNodeTree.Name == "Core_DamageDealt")
//                menu.AddItem(new GUIContent("Add Min-Max Node"), false, AddNode(), new object[] { Rm_NodeType.MinMaxNode, mousePos });
//
//            menu.AddItem(new GUIContent("Add Boolean Node"), false, AddNode(), new object[] { Rm_NodeType.Boolean, mousePos });
//            menu.AddItem(new GUIContent("Add Variable Setter Node"), false, AddNode(), new object[] { Rm_NodeType.VarSetter, mousePos });
//            menu.AddItem(new GUIContent("Add Event Node"), false, AddNode(), new object[] { Rm_NodeType.Run_Event, mousePos });
//            menu.AddItem(new GUIContent("Add Variable Node"), false, AddNode(), new object[] { Rm_NodeType.Start_Combat_Var, mousePos });
//            menu.AddItem(new GUIContent("Add Result Node"), false, AddNode(), new object[] { Rm_NodeType.Result_Node, mousePos });
//
//            menu.ShowAsContext();
//            evt.Use();
//        }
//
//        public override sealed void InitNodes()
//        {
//            Debug.Log("I was here!");
//            var damageTakenTree = Rm_RPGHandler.Instance.Nodes.DamageTakenTree;
//            var damageDealtTree = Rm_RPGHandler.Instance.Nodes.DamageDealtTree;
//
//            if (damageTakenTree.Nodes.Count == 0)
//            {
//                var startingNode = new Rm_Node {NodeType = Rm_NodeType.Start_Combat};
//                startingNode.WindowID = GetNextID(Rm_RPGHandler.Instance.Nodes.CombatNodeBank);
//                startingNode.NextPoints.Add(new Rm_NodeLink(startingNode));
//                startingNode.Rect.x = startingNode.Rect.y = 30;
//                damageTakenTree.Nodes.Add(startingNode);
//        
//                var endNode = new Rm_Node {NodeType = Rm_NodeType.End_Combat};
//                endNode.WindowID = GetNextID(Rm_RPGHandler.Instance.Nodes.CombatNodeBank);
//                endNode.Rect.x = 400;
//                endNode.Rect.y = 30;
//                damageTakenTree.Nodes.Add(endNode);
//        
//                var evadeNode = new Rm_Node {NodeType = Rm_NodeType.End_Combat_Evade};
//                evadeNode.WindowID = GetNextID(Rm_RPGHandler.Instance.Nodes.CombatNodeBank);
//                evadeNode.Rect.x = 400;
//                evadeNode.Rect.y = 250;
//                damageTakenTree.Nodes.Add(evadeNode);
//
//                var missNode = new Rm_Node {NodeType = Rm_NodeType.End_Combat_Miss};
//                missNode.WindowID = GetNextID(Rm_RPGHandler.Instance.Nodes.CombatNodeBank);
//                missNode.Rect.x = 400;
//                missNode.Rect.y = 450;
//                damageTakenTree.Nodes.Add(missNode);
//            }
//
//            if (damageDealtTree.Nodes.Count == 0)
//            {
//                var startingNode = new Rm_Node {NodeType = Rm_NodeType.Start_Dealt_Damage};
//                startingNode.WindowID = GetNextID(Rm_RPGHandler.Instance.Nodes.CombatNodeBank);
//                startingNode.Identifier = "Physical";
//                startingNode.NextPoints.Add(new Rm_NodeLink(startingNode));
//                startingNode.Rect.x = startingNode.Rect.y = 30;
//                damageDealtTree.Nodes.Add(startingNode);
//
//                var endNode = new Rm_Node { NodeType = Rm_NodeType.End_Dealt_Damage };
//                endNode.Identifier = "Physical";
//                endNode.WindowID = GetNextID(Rm_RPGHandler.Instance.Nodes.CombatNodeBank);
//                endNode.Rect.x = 400;
//                endNode.Rect.y = 30;
//                damageDealtTree.Nodes.Add(endNode);
//        
//                var evadeNode = new Rm_Node {NodeType = Rm_NodeType.Start_Crit_Chance};
//                evadeNode.WindowID = GetNextID(Rm_RPGHandler.Instance.Nodes.CombatNodeBank);
//                evadeNode.NextPoints.Add(new Rm_NodeLink(evadeNode));
//                evadeNode.Rect.x = 30;
//                evadeNode.Rect.y = 250;
//                damageDealtTree.Nodes.Add(evadeNode);
//
//                var missNode = new Rm_Node { NodeType = Rm_NodeType.End_Crit_Chance };
//                missNode.WindowID = GetNextID(Rm_RPGHandler.Instance.Nodes.CombatNodeBank);
//                missNode.Rect.x = 400;
//                missNode.Rect.y = 250;
//                damageDealtTree.Nodes.Add(missNode);
//            }
//
//            //remove old
//            damageDealtTree.Nodes.RemoveAll(
//                n => n.Identifier != "Physical" && (n.NodeType == Rm_NodeType.Start_Dealt_Damage || n.NodeType == Rm_NodeType.End_Dealt_Damage) &&
//                     Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions.All(e => e.ID != n.Identifier));
//            //add non-existing
//
//            var notAdded = Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions.Where(c => damageDealtTree.Nodes.All(n => n.Identifier != c.ID)).ToList();
//            for (int index = 0; index < notAdded.Count; index++)
//            {
//                var element = notAdded[index];
//                var newNode = new Rm_Node {NodeType = Rm_NodeType.Start_Dealt_Damage};
//                newNode.Identifier = element.ID;
//                newNode.NextPoints.Add(new Rm_NodeLink(newNode));
//                newNode.WindowID = GetNextID(Rm_RPGHandler.Instance.Nodes.CombatNodeBank);
//                newNode.Rect.x = 30;
//                newNode.Rect.y = 500 + (index * 250);
//                damageDealtTree.Nodes.Add(newNode);
//
//                var newEndNode = new Rm_Node {NodeType = Rm_NodeType.End_Dealt_Damage};
//                newEndNode.Identifier = element.ID;
//                newEndNode.WindowID = GetNextID(Rm_RPGHandler.Instance.Nodes.CombatNodeBank);
//                newEndNode.Rect.x = 400;
//                newEndNode.Rect.y = 500 + (index * 250);
//                damageDealtTree.Nodes.Add(newEndNode);
//            }
//        }
//    }
//}
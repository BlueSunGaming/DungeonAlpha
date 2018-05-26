using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker
{
    public class Rmh_Nodes
    {
        public NodeBank CombatNodeBank;
        public NodeBank EventNodeBank;
        public NodeBank DialogNodeBank;
        public NodeBank WorldMapNodeBank;
        public NodeBank AchievementsNodeBank;
        private const string DamageTakenTreeName = "Core_DamageTaken";
        private const string DamageDealtTreeName = "Core_DamageDealt";
        private const string StatScalingTreeName = "Core_StatScaling";
        private const string VitalScalingTreeName = "Core_VitalScaling";
        private const string CoreInitialMap = "Core_InitialMap";

        [JsonIgnore]
        public List<NodeTree> EventNodeTrees
        {
            get{return EventNodeBank.NodeTrees;}
        }

        [JsonIgnore]
        public List<NodeChain> EventNodeChains
        {
            get
            {
                var nodeChains = new List<NodeChain>();
                foreach (var tree in EventNodeTrees)
                {
                    var startNodes = tree.Nodes.Where(n => n.IsStartNode).ToList();
                    foreach (var startNode in startNodes)
                    {
                        nodeChains.Add(new NodeChain(tree, startNode));
                    }
                }

                // Combat node chains are not events
                //foreach (var tree in CombatNodeBank.NodeTrees.Where(t => !t.Required))
                //{
                //    var startNodes = tree.Nodes.Where(n => n.IsStartNode).ToList();
                //    foreach (var startNode in startNodes)
                //    {
                //        nodeChains.Add(new NodeChain(tree, startNode));
                //    }
                //}

                return nodeChains;
            }
        }

        [JsonIgnore]
        public NodeTree DamageTakenTree
        {
            get
            {
                var tree = CombatNodeBank.NodeTrees.First(t => t.ID == DamageTakenTreeName);
                return tree;
            }
        }

        [JsonIgnore]
        public NodeTree DamageDealtTree
        {
            get
            {
                var tree = CombatNodeBank.NodeTrees.First(t => t.ID == DamageDealtTreeName);
                return tree;
            }
        }

        [JsonIgnore]
        public NodeTree StatScalingTree
        {
            get
            {
                var tree = CombatNodeBank.NodeTrees.First(t => t.ID == StatScalingTreeName);
                return tree;
            }
        }

        [JsonIgnore]
        public NodeTree VitalScalingTree
        {
            get
            {
                var tree = CombatNodeBank.NodeTrees.First(t => t.ID == VitalScalingTreeName);
                return tree;
            }
        }

        [JsonIgnore]
        public List<NodeTree> AllTrees
        {
            get { return CombatNodeBank.NodeTrees.Concat(EventNodeTrees).Concat(DialogNodeBank.NodeTrees).Concat(AchievementsNodeBank.NodeTrees).ToList(); }
        }


        public Rmh_Nodes()
        {
            var x = new NodeTree {ID = DamageTakenTreeName, Name = DamageTakenTreeName, Required = true, Type = NodeTreeType.Combat};
            var n = new NodeTree { ID = DamageDealtTreeName, Name = DamageDealtTreeName, Required = true, Type = NodeTreeType.Combat };
            var y = new NodeTree { ID = StatScalingTreeName, Name = StatScalingTreeName, Required = true, Type = NodeTreeType.Combat };
            var p = new NodeTree { ID = VitalScalingTreeName, Name = VitalScalingTreeName, Required = true, Type = NodeTreeType.Combat };


            var node = new CombatStartNode("Min Physical Damage", "Start node for dealt physical",
                                           "Physical Damage represents weapon damage, or base combatant damage. You can then add additional nodes to add onto this value. Example: Add 10x Strength") {Identifier = "MIN_Physical"};
            node.OnCreate();
            n.AddNode(node, new Vector2(25, 87.5f), 0);
            var nodeMax = new CombatStartNode("Max Physical Damage", "Start node for dealt physical",
                                           "Physical Damage represents weapon damage, or base combatant damage. You can then add additional nodes to add onto this value. Example: Add 10x Strength") {Identifier = "MAX_Physical"};
            nodeMax.OnCreate();
            n.AddNode(nodeMax, new Vector2(25, 287.5f), 1);

            var damageTakenNode = new CombatStartNode("Damage To Be Dealt", "Represents damage about to be taken",
                                           "This is the incoming damage. It can be modified to take into account other stats. E.g. Damage - Defender's Armor value.") { Identifier = "DamageTaken" };
            var missedNode = new MissedHitNode();
            var evadedNode = new EvadedHitNode();
            var successHitNode = new SuccessHitEndNode();
            damageTakenNode.OnCreate();
            missedNode.OnCreate();
            successHitNode.OnCreate();
            evadedNode.OnCreate();
            x.AddNode(damageTakenNode, new Vector2(25, 87.5f), 0);
            x.AddNode(successHitNode, new Vector2(700, 87.5f), 1);
            x.AddNode(missedNode, new Vector2(700, 237.5f), 2);
            x.AddNode(evadedNode, new Vector2(700, 387.5f), 3);

            n.Variables.Add(new NodeTreeVar("Attacker",PropertyType.CombatCharacter));

            y.Variables.Add(new NodeTreeVar("Attacker",PropertyType.CombatCharacter));
            p.Variables.Add(new NodeTreeVar("Attacker",PropertyType.CombatCharacter));

            x.Variables.Add(new NodeTreeVar("Attacker",PropertyType.CombatCharacter));
            x.Variables.Add(new NodeTreeVar("Defender",PropertyType.CombatCharacter));
            x.Variables.Add(new NodeTreeVar("Physical Damage", PropertyType.Int) { ID = "DamageDealtVar_Physical" });

            AchievementsNodeBank = new NodeBank()
            {
                Type = NodeTreeType.Achievements,
                NodeTrees = new List<NodeTree>()
            };

            CombatNodeBank = new NodeBank
            {
                Type = NodeTreeType.Combat,
                NodeTrees = new List<NodeTree>(){ x,n,y,p }
            };

            EventNodeBank = new NodeBank
            {
                Type = NodeTreeType.Event,
                NodeTrees = new List<NodeTree>()
            };

            DialogNodeBank = new NodeBank
            {
                Type = NodeTreeType.Dialog,
                NodeTrees = new List<NodeTree>()
            };

            WorldMapNodeBank = new NodeBank
            {
                Type = NodeTreeType.WorldMap,
                NodeTrees = new List<NodeTree>()
                                {
                                    new NodeTree() {ID = CoreInitialMap , Name = CoreInitialMap, Required = true, Type = NodeTreeType.Combat}
                                }
            };
        }

    }
}
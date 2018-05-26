using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.RPGMaker.Nodes.Core;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class NodeChain
    {
        public List<Node> Nodes;
        [JsonIgnore]
        public Node CurrentNode;
        public string Name;
        public string NodeTreeName;
        public bool Done;

        [JsonIgnore]
        public BaseCharacter CombatantA;
        [JsonIgnore]
        public BaseCharacter CombatantB;

        public ReturnType ReturnType;

        public int IntValue { get; set; }
        public string StringValue { get; set; }
        public float FloatValue { get; set; }
        public bool BoolValue { get; set; }

        public object ObjectValue { get; set; }

        public List<NodeTreeVar> Variables { get; set; }
        public Damage Damage { get; set; }
        public DamageDealt DamageDealt { get; set; }

        [JsonIgnore]
        public BaseCharacter Combatant
        {
            get { return CombatantA; }
            set { CombatantA = value; }
        }

        [JsonIgnore]
        public BaseCharacter Attacker
        {
            get { return CombatantA; }
            set { CombatantA = value; }
        }

        [JsonIgnore]
        public BaseCharacter Defender
        {
            get { return CombatantB; }
            set { CombatantB = value; }
        }

        public NodeChain(NodeTree nodeTree, Type startNodeType, string identifier = "")
        {
            Variables = nodeTree.Variables;

            nodeTree = GeneralMethods.CopyObject(nodeTree);

            NodeTreeName = nodeTree.Name;
            CurrentNode = nodeTree.Nodes.FirstOrDefault(n => n.GetType() == startNodeType && 
                (string.IsNullOrEmpty(identifier) || n.Identifier == identifier)
                );
            if (CurrentNode == null) throw new Exception("Error: Node chain could not find starting node.");

            Name = CurrentNode.NodeChainName;
            Nodes = NodeHelper.GetChainAsNodes(CurrentNode,nodeTree);
            ReturnType = ((StartNode) CurrentNode).ReturnType;
            IntValue = 0;
            ObjectValue = null;
            Done = false;
            InitRefs();
            Variables.ForEach(v => v.ResetValue());

        }

        private void InitRefs()
        {
            Nodes.ForEach(n => n.SetNodeChain(this));
        }


        public NodeChain(NodeTree nodeTree, Node startingNode)
        {
            Variables = nodeTree.Variables;

            nodeTree = GeneralMethods.CopyObject(nodeTree);

            NodeTreeName = nodeTree.Name;
            CurrentNode = nodeTree.Nodes.FirstOrDefault(n => n.ID == startingNode.ID);
            if (CurrentNode == null) throw new Exception("Error: Node chain could not find starting node.");

            Name = CurrentNode.NodeChainName;
            Nodes = NodeHelper.GetChainAsNodes(CurrentNode,nodeTree);
            ReturnType = ((StartNode) CurrentNode).ReturnType;
            IntValue = 0;
            ObjectValue = null;
            Done = false;
            InitRefs();
            Variables.ForEach(v => v.ResetValue());

        }

        public NodeChain(NodeChain dialogNodeChain, string startNodeType)
        {
            Variables = dialogNodeChain.Variables;

            var b = GeneralMethods.CopyObject(dialogNodeChain);

            CurrentNode = b.Nodes.First(n => n.ID == startNodeType);
            Nodes = NodeHelper.GetChainAsNodes(CurrentNode, b.Nodes);
            IntValue = 0;
            ObjectValue = null;
            Done = false;
            InitRefs();
            Variables.ForEach(v => v.ResetValue());

        }

        public NodeChain()
        {

        }


        public NodeChain(NodeChain nodeChain, Node startNode)
        {
            Variables = nodeChain.Variables;
            CurrentNode = nodeChain.Nodes.FirstOrDefault(n => n.ID == startNode.ID);
            if (CurrentNode == null) throw new Exception("Error: Node chain could not find starting node.");
            Nodes = NodeHelper.GetChainAsNodes(CurrentNode, nodeChain.Nodes);
            ObjectValue = null;
            Done = false;
            InitRefs();
            Variables.ForEach(v => v.ResetValue());
        }

        public void Evaluate()
        {
            if (CurrentNode == null)
            {
                Done = true;
                return;
            }

            var nextNode = CurrentNode.Evaluate(this);
            if(nextNode != null)
            {
                CurrentNode = Nodes.FirstOrDefault(n => n.ID == nextNode);
            }
            else
            {
                Done = true;
            }
        }

        public Node GetNode(string nodeId)
        {
            return Nodes.FirstOrDefault(n => n.ID == nodeId);
        }

        public void Goto(string id)
        {
            CurrentNode = Nodes.First(n => n.ID == id);
        }

        public NodeTreeVar GetVariable(string variableId)
        {
            if (Variables == null || !Variables.Any()) return null;
            return Variables.FirstOrDefault(v => v.ID == variableId);
        }
    }
}
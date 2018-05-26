using LogicSpawn.RPGMaker.API;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class CombatStartNode : StartNode
    {
        public string NewName;
        public string NewDescription;
        public string NewSubText;

        [JsonIgnore]
        public override string Name
        {
            get { return NewName; }
        }

        [JsonIgnore]
        public override string Description
        {
            get { return NewDescription; }
        }

        [JsonIgnore]
        public override string SubText
        {
            get { return NewSubText; }
        }

        public override bool CanBeLinkedTo
        {
            get { return false; }
        }

        public override bool IsStartNode
        {
            get { return true; }
        }

        public override bool ShowInSearch
        {
            get { return false; }
        }

        public override bool ShowTarget
        {
            get { return false; }
        }

        public override bool CanBeDeleted
        {
            get { return false; }
        }
        public override string NextNodeLinkLabel(int index)
        {
            return "Next";
        }

        public CombatStartNode(string name, string subtext, string description)
        {
            NodeChainName = name;
            NewName = name;
            NewDescription = description;
            NewSubText = subtext;
        }

        public CombatStartNode()
        {
            
        }

        protected override void SetupParameters()
        {

        }

        protected override void Eval(NodeChain nodeChain)
        {

        }
    }
}
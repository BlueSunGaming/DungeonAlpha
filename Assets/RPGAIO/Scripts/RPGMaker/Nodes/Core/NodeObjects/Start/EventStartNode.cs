using LogicSpawn.RPGMaker.API;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Entry Points","")]
    [NodeCategoryTree(NodeTreeType.Event)]
    public class EventStartNode : StartNode
    {
        [JsonIgnore]
        public override string Name
        {
            get { return "Event Start"; }
        }

        [JsonIgnore]
        public override string Description
        {
            get { return "This acts as the entry point for starting an event."; }
        }

        [JsonIgnore]
        public override string SubText
        {
            get { return ""; }
        }

        public override bool CanBeLinkedTo
        {
            get { return false; }
        }

        public override bool IsStartNode
        {
            get { return true; }
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

        protected override void SetupParameters()
        {

        }

        protected override void Eval(NodeChain nodeChain)
        {

        }
    }
}
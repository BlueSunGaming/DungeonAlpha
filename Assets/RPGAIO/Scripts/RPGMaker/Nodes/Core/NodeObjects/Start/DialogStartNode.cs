using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class DialogStartNode : StartNode
    {
        [JsonIgnore]
        public override string Name
        {
            get { return "Enter Dialog"; }
        }

        [JsonIgnore]
        public override string Description
        {
            get { return "This acts as the entry point for starting a dialog.\n\nYou can link it straight away to an NPC dialog node or, for example, run an event before then adding an NPC dialog node."; }
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

        protected override void SetupParameters()
        {

        }

        protected override void Eval(NodeChain nodeChain)
        {

        }
    }
}
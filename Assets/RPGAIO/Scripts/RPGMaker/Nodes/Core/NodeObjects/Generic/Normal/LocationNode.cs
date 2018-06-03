using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class LocationNode : OptionsNode
    {
        public string LocName;

        public LocationNode()
        {
            LocName = "Location Node";
        }

        public override string Name
        {
            get { return LocName; }
        }

        public override string Description
        {
            get { return "Represents a location"; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        public override bool CanBeLinkedTo
        {
            get
            {
                return true;
            }
        }

        public override string NextNodeLinkLabel(int index)
        {
            return "Path " + index;
        }

        protected override void SetupParameters()
        {
        }

        protected override int Eval(NodeChain nodeChain)
        {
            return -1;
        }
    }
}
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class AchievementMaxProgress : StartNode
    {
        [JsonIgnore]
        public override string Name
        {
            get { return "Achievement Max Progress"; }
        }

        [JsonIgnore]
        public override string Description
        {
            get { return "Achievement max progress value."; }
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
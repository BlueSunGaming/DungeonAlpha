using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class AchievementStartNode : StartNode
    {
        [JsonIgnore]
        public override string Name
        {
            get { return "Achievement Start"; }
        }

        [JsonIgnore]
        public override string Description
        {
            get { return "Achievement."; }
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
            Add("Name", PropertyType.String, null, "My Achievement");
            Add("Image", PropertyType.Texture2D, null, null);
            Add("Has Progress?", PropertyType.Bool, null, false);
            Add("Description", PropertyType.TextArea, null, "My Achievement");
        }

        protected override void Eval(NodeChain nodeChain)
        {

        }
    }
}
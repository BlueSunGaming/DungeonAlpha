using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class AchievementEndNode : EndNode
    {

        [JsonIgnore]
        public override string Name
        {
            get { return "Achievement Unlock"; }
        }

        [JsonIgnore]
        public override string Description
        {
            get { return "End point for unlocking an achievement."; }
        }

        [JsonIgnore]
        public override string SubText
        {
            get { return "Link here to unlock."; }
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

        public AchievementEndNode()
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
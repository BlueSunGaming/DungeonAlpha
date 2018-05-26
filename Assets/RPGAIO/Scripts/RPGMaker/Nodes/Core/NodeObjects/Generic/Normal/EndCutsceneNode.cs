using LogicSpawn.RPGMaker.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    public class EndCutsceneNode : SimpleNode
    {
        public override string Name
        {
            get { return "End Cutscene"; }
        }

        public override string Description
        {
            get { return "Resumes control of the player and shows the GUI."; }
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
            return "Next";
        }

        protected override void SetupParameters()
        {
        }

        protected override void Eval(NodeChain nodeChain)
        {
            GameMaster.CutsceneActive = false;
            GetObject.PlayerController.Interacting = false;
            GameMaster.ShowUI = true;
        }
    }
}
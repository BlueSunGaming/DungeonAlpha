using LogicSpawn.RPGMaker.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Game", "")]
    public class StartCutsceneNode : SimpleNode
    {
        public override string Name
        {
            get { return "Start Cutscene"; }
        }

        public override string Description
        {
            get { return "Pauses control of the player and hides the GUI."; }
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
            GameMaster.CutsceneActive = true;
            GetObject.PlayerController.Interacting = true;
            GameMaster.ShowUI = false;
        }
    }
}
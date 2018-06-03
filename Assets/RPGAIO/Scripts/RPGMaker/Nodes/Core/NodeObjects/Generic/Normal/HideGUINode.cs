namespace LogicSpawn.RPGMaker.Core
{
    public class HideGUINode : SimpleNode
    {
        public override string Name
        {
            get { return "Hide GUI"; }
        }

        public override string Description
        {
            get { return "Hides the GUI."; }
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
            GameMaster.ShowUI = false;
        }
    }
}
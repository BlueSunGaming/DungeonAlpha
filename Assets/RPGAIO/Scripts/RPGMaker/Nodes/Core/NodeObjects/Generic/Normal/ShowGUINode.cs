namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Game","")]
    public class ShowGUINode : SimpleNode
    {
        public override string Name
        {
            get { return "Show GUI"; }
        }

        public override string Description
        {
            get { return "Show the GUI."; }
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
            GameMaster.ShowUI = true;
        }
    }
}
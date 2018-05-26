namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Dialog", "")]
    public class EndDialogNode : SimpleNode
    {
        public override string Name
        {
            get { return "End Dialog"; }
        }

        public override string Description
        {
            get { return "Closes the current open dialog."; }
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
            DialogHandler.Instance.EndDialog();
        }
    }
}
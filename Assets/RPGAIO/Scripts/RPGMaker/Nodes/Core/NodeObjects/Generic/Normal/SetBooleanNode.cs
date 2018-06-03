namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Boolean", "")]
    public class SetBooleanNode : SimpleNode
    {
        public override string Name
        {
            get { return "Set Boolean"; }
        }

        public override string Description
        {
            get { return "Sets a boolean to a value."; }
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
            Add("Boolean", PropertyType.Bool, null, null, PropertySource.InputOnly);
            Add("Value", PropertyType.Bool, null, true, PropertySource.EnteredOrInput);
        }

        protected override void Eval(NodeChain nodeChain)
        {
            var value = (bool)ValueOf("Value");
            ApplyFunctionTo("Boolean", o =>
                                           {    
                                               o = value;
                                               return o;
                                           });
        }
    }
}
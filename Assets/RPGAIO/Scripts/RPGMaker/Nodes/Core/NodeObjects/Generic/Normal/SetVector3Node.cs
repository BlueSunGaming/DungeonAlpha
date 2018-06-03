namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Vector3", "")]
    public class SetVector3Node : SimpleNode
    {
        public override string Name
        {
            get { return "Set Vector3"; }
        }

        public override string Description
        {
            get { return "Sets a vector3 to a value."; }
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
            Add("Vector3", PropertyType.Vector3, null, null, PropertySource.InputOnly);
            Add("Value", PropertyType.Vector3, null, RPGVector3.Zero, PropertySource.EnteredOrInput);
        }

        protected override void Eval(NodeChain nodeChain)
        {
            var value = (RPGVector3)ValueOf("Value");
            ApplyFunctionTo("Vector3", o =>
                                           {    
                                               o = value;
                                               return o;
                                           });
        }
    }
}
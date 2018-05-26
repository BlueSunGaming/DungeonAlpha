namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Text","")]
    public class SetStringNode : SimpleNode
    {
        public override string Name
        {
            get { return "Set String"; }
        }

        public override string Description
        {
            get { return "Sets an string node to a value."; }
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
            Add("String", PropertyType.String, null, null, PropertySource.InputOnly);
            Add("Value", PropertyType.String, null, "", PropertySource.EnteredOrInput);
        }

        protected override void Eval(NodeChain nodeChain)
        {
            var value = (string)ValueOf("Value");
            ApplyFunctionTo("String", o =>
                                          {    
                                              o = value;
                                              return o;
                                          });
        }
    }
}
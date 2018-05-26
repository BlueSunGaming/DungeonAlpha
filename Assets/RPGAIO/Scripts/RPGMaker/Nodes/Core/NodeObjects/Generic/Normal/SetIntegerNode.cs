using System;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Calculations","Integer")]
    public class SetIntegerNode : SimpleNode
    {
        public override string Name
        {
            get { return "Set Integer"; }
        }

        public override string Description
        {
            get { return "Sets an integer node to a value."; }
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
            Add("Integer", PropertyType.Int, null, null, PropertySource.InputOnly);
            Add("Value", PropertyType.Int, null, 0, PropertySource.EnteredOrInput);
        }

        protected override void Eval(NodeChain nodeChain)
        {
            var value = Convert.ToInt32(ValueOf("Value"));
            ApplyFunctionTo("Integer", o =>
                                           {    
                                               o = value;
                                               return o;
                                           });
        }
    }
}
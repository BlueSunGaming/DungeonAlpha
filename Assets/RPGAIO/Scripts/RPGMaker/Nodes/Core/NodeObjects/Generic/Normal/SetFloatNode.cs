using System;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Calculations","Float")]
    public class SetFloatNode : SimpleNode
    {
        public override string Name
        {
            get { return "Set Float"; }
        }

        public override string Description
        {
            get { return "Sets an float node to a value."; }
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
            Add("Float", PropertyType.Float, null, null, PropertySource.InputOnly);
            Add("Value", PropertyType.Float, null, 0.0f, PropertySource.EnteredOrInput);
        }

        protected override void Eval(NodeChain nodeChain)
        {
            var value = Convert.ToSingle(ValueOf("Value"));
            ApplyFunctionTo("Float", o =>
                                         {    
                                             o = value;
                                             return o;
                                         });
        }
    }
}
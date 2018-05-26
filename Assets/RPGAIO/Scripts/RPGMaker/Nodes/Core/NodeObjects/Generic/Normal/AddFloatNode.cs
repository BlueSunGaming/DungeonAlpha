using System;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Calculations", "Float")]
    public class AddFloatNode : SimpleNode
    {
        public override string Name
        {
            get { return "Add Float Value"; }
        }

        public override string Description
        {
            get { return "Adds X to the float value."; }
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
            Add("Value", PropertyType.Float, null, 0, PropertySource.EnteredOrInput); //param value format: (index, index/string/value), e.g. 0,abc-123 => 0 represents attribute, abc-123 represents attribute
            Add("Target", PropertyType.Float, null, 0, PropertySource.EnteredOrInput);
        }

        protected override void Eval(NodeChain nodeChain)
        {
            var valueToAdd = Convert.ToSingle(ValueOf("Value"));

            var valueOfTarget = Convert.ToSingle(ValueOf("Target"));
            valueOfTarget += valueToAdd;
            ApplyFunctionTo("Target", o => { 
                o = valueOfTarget;
                return o;
            });
        }
    }
}
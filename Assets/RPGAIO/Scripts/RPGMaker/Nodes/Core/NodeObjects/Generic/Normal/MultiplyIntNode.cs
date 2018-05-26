using System;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Calculations", "Int")]
    public class MultiplyIntNode : SimpleNode
    {
        public override string Name
        {
            get { return "Multiply Int Value"; }
        }

        public override string Description
        {
            get { return "Multiplies int value by X."; }
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
            Add("Value", PropertyType.Int, null, 0, PropertySource.EnteredOrInput); //param value format: (index, index/string/value), e.g. 0,abc-123 => 0 represents attribute, abc-123 represents attribute
            Add("Target", PropertyType.Int, null, 0, PropertySource.EnteredOrInput);
        }

        protected override void Eval(NodeChain nodeChain)
        {
            var multiplyBy = Convert.ToInt32(ValueOf("Value"));

            var valueOfTarget = Convert.ToInt32(ValueOf("Target"));
            valueOfTarget *= multiplyBy;
            ApplyFunctionTo("Target", o =>
            {
                o = valueOfTarget;
                return o;
            });
        }
    }
}
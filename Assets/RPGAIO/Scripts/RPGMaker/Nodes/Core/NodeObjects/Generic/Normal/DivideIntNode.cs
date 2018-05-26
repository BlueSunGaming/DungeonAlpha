using System;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Calculations", "Int")]
    public class DivideIntNode : SimpleNode
    {
        public override string Name
        {
            get { return "Divide Int Value"; }
        }

        public override string Description
        {
            get { return "Divides int value by X."; }
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
            Add("Value", PropertyType.Int, null, 0, PropertySource.EnteredOrInput);
            Add("Target", PropertyType.Int, null, 0, PropertySource.EnteredOrInput);
        }

        protected override void Eval(NodeChain nodeChain)
        {
            var divideBy = Convert.ToInt32(ValueOf("Value"));

            var valueOfTarget = Convert.ToInt32(ValueOf("Target"));
            valueOfTarget /= divideBy;
            ApplyFunctionTo("Target", o =>
            {
                o = valueOfTarget;
                return o;
            });
        }
    }
}
using System;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Calculations", "Int")]
    public class SubtractIntNode : SimpleNode
    {
        public override string Name
        {
            get { return "Subtract Int Value"; }
        }

        public override string Description
        {
            get { return "Subtracts X from the int value."; }
        }

        public override string SubText
        {
            get { return "Target - Value"; }
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
            Add("Value", PropertyType.Int, null, 0 , PropertySource.EnteredOrInput); 
            Add("Target", PropertyType.Int,null, 0, PropertySource.EnteredOrInput);
        }

        protected override void Eval(NodeChain nodeChain)
        {
            var valueToAdd = Convert.ToInt32(ValueOf("Value"));

            var valueOfTarget = Convert.ToInt32(ValueOf("Target"));
            valueOfTarget -= valueToAdd;
            ApplyFunctionTo("Target", o => { 
                o = valueOfTarget;
                return o;
            });
        }
    }
}
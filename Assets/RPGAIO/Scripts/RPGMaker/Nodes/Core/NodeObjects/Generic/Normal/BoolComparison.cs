using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Comparison", "")]
    public class BoolComparison : BooleanNode
    {
        public override string Name
        {
            get { return "Bool Comparison"; }
        }

        public override string Description
        {
            get { return "Returns true if the condition is met."; }
        }

        public override string SubText
        {
            get { return "True if A == B"; }
        }

        protected override void SetupParameters()
        {
            Add("A", PropertyType.Bool, null, false,PropertySource.EnteredOrInput);
            Add("B", PropertyType.Bool, null, false, PropertySource.EnteredOrInput);
        }

        protected override bool Eval(NodeChain nodeChain)
        {
            return (bool)ValueOf("A") == (bool)ValueOf("B");
        }
    }
}
using System;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Comparison", "")]
    public class IntComparison : BooleanNode
    {
        public override string Name
        {
            get { return "Int Comparison"; }
        }

        public override string Description
        {
            get { return "Returns true if the condition is met."; }
        }

        public override string SubText
        {
            get { return "True if A > B"; }
        }

        protected override void SetupParameters()
        {
            Add("A", PropertyType.Int, null, 0, PropertySource.EnteredOrInput);
            Add("B", PropertyType.Int, null, 1, PropertySource.EnteredOrInput);
        }

        protected override bool Eval(NodeChain nodeChain)
        {
            return Convert.ToInt32(ValueOf("A")) > Convert.ToInt32(ValueOf("B"));
        }
    }
}
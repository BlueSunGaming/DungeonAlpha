using System;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Comparison", "")]
    public class FloatEqualsComparison : BooleanNode
    {
        public override string Name
        {
            get { return "Float Equals Comparison"; }
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
            Add("A", PropertyType.Float, null, 0, PropertySource.EnteredOrInput);
            Add("B", PropertyType.Float, null, 1, PropertySource.EnteredOrInput);
        }

        protected override bool Eval(NodeChain nodeChain)
        {
            return Convert.ToSingle(ValueOf("A")) == Convert.ToSingle(ValueOf("B"));
        }
    }
}
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Calculations", "")]
    public class RandomNode : BooleanNode
    {
        public override string Name
        {
            get { return "Random Node"; }
        }

        public override string Description
        {
            get { return "Returns a random value between MIN and MAX and returns true if the value is greater than another value."; }
        }

        public override string SubText
        {
            get { return "Basic randomiser."; }
        }

        protected override void SetupParameters()
        {
            Add("Whole Numbers?", PropertyType.Bool, null, true).WithSubParams(
                SubParam("Min", PropertyType.Int, null, 0 ).If(p => (bool)p.Value),
                SubParam("Max", PropertyType.Int, null, 100).If(p => (bool)p.Value),
                SubParam("True when > than", PropertyType.Int, null, 75).If(p => (bool)p.Value),

                SubParam("Min ", PropertyType.Float, null, 0.0f).If(p => !(bool)p.Value),
                SubParam("Max ", PropertyType.Float, null, 100.0f).If(p => !(bool)p.Value),
                SubParam("True when > than ", PropertyType.Float, null, 75.0f).If(p => !(bool)p.Value)
                );
        }

        protected override bool Eval(NodeChain nodeChain)
        {
            if((bool)ValueOf("Whole Numbers?"))
            {
                var randomNumber = Random.Range((int) Parameter("Whole Numbers?").ValueOf("Min"), (int) Parameter("Whole Numbers?").ValueOf("Max") + 1);
                return randomNumber > (int)Parameter("Whole Numbers?").ValueOf("True when > than");
            }
            else
            {
                var randomNumber = Random.Range((float)Parameter("Whole Numbers?").ValueOf("Min "), (float)Parameter("Whole Numbers?").ValueOf("Max ") + 1);
                return randomNumber > (float)Parameter("Whole Numbers?").ValueOf("True when > than ");
            }
        }
    }
}
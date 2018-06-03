using System;

namespace LogicSpawn.RPGMaker.Core
{
    public class VectorNode : PropertyNode
    {
        public override string Name
        {
            get { return "Vector Node"; }
        }

        public override string Description
        {
            get { return "Gets the player's gameObject."; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        public override PropertyFamily PropertyFamily
        {
            get { return PropertyFamily.Object; }
        }

        protected override PropertyType PropertyNodeType
        {
            get { return PropertyType.Vector3; }
        }

        protected override bool InheritsPropertyType
        {
            get { return false; }
        }

        protected override void SetupParameters()
        {
            Add("Vector", PropertyType.Vector3, null, RPGVector3.Zero, PropertySource.EnteredOnly, PropertyFamily.Object);
        }

        public override object EvaluateInput(NodeChain nodeChain, Func<object, object> func)
        {
            var vector = (RPGVector3) ValueOf("Vector");
            return vector;
        }
    }
}
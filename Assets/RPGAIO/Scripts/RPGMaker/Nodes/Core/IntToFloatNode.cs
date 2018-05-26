using System;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Property", "Conversion")]
    public class IntToFloatNode : PropertyNode
    {
        [JsonIgnore]
        protected override bool InheritsPropertyType { get { return false; } }
        [JsonIgnore]
        protected override PropertyType PropertyNodeType { get { return PropertyType.Float; } }
        [JsonIgnore]
        public override PropertyFamily PropertyFamily { get { return PropertyFamily.Primitive; } }
        [JsonIgnore]
        public override NodeType NodeType { get { return NodeType.Property; } }

        public override string Name
        {
            get { return "Int to Float"; }
        }

        public override string Description
        {
            get { return "Converts an int value to a float value"; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        public override bool HasMaxNextLinks
        {
            get
            {
                return true;
            }
        }

        public override int MaxNextLinks
        {
            get
            {
                return 1;
            }
        }
        public override bool CanRemoveLinks
        {
            get
            {
                return false;
            }
        }

        public override bool CanBeLinkedTo
        {
            get
            {
                return false;
            }
        }

        protected override object EvaluateNode(NodeChain nodeChain)
        {
            return true;
        }

        protected override void SetupParameters()
        {
            Add("Int", PropertyType.Int, null, null, PropertySource.EnteredOrInput, PropertyFamily.Primitive);
        }

        public override object EvaluateInput(NodeChain nodeChain, Func<object, object> func)
        {
            var intValue = (int) ValueOf("Int");

            var result = Convert.ToSingle(intValue);
            if(func != null)
            {
                var xx = Convert.ToSingle(func(result));
                result = xx;
            }
            return result;
        }


    }
}
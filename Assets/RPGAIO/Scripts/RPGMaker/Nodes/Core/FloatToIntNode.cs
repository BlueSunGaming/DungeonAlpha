using System;
using Assets.Scripts.Exceptions;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Property", "Conversion")]
    public class FloatToIntNode : PropertyNode
    {
        [JsonIgnore]
        protected override bool InheritsPropertyType { get { return false; } }
        [JsonIgnore]
        protected override PropertyType PropertyNodeType { get { return PropertyType.Int; } }
        [JsonIgnore]
        public override PropertyFamily PropertyFamily { get { return PropertyFamily.Primitive; } }
        [JsonIgnore]
        public override NodeType NodeType { get { return NodeType.Property; } }

        public override string Name
        {
            get { return "Float To Int"; }
        }

        public override string Description
        {
            get { return "Converts a float value to an int value"; }
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
            Add("Float", PropertyType.Float, null, null, PropertySource.EnteredOrInput, PropertyFamily.Primitive);
        }

        public override object EvaluateInput(NodeChain nodeChain, Func<object, object> func)
        {
            var floatValue = (float) ValueOf("Float");

            var result = Convert.ToInt32(floatValue);
            if(func != null)
            {
                var xx = Convert.ToInt32(func(result));
                result = xx;
            }
            return result;
        }


    }
}
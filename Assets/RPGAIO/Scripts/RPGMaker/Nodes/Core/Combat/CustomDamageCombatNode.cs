using System;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class CustomDamageCombatNode : PropertyNode
    {
        [JsonIgnore]
        public override PropertyFamily PropertyFamily { get { return PropertyFamily.Primitive; } }

        [JsonIgnore]
        protected override bool InheritsPropertyType { get { return false; } }

        [JsonIgnore]
        protected override PropertyType PropertyNodeType { get { return PropertyType.Int; } }

        [JsonIgnore]
        public override NodeType NodeType { get { return NodeType.Property; } }

        [JsonIgnore]
        public override string Name { get { return NewName; } }

        public string NewName;
        public string DamageId;

        public override string Description
        {
            get { return "Represents the custom damage dealt/to deal"; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        public override bool ShowInSearch
        {
            get { return false; }
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

        public override object EvaluateInput(NodeChain nodeChain, Func<object, object> func)
        {
            var variable = nodeChain.DamageDealt.Elementals[DamageId];
            var result = Convert.ToInt32(func(variable));
            nodeChain.DamageDealt.Elementals[DamageId] = result;
            return result;
        }

        protected override void SetupParameters()
        {
        }

        public CustomDamageCombatNode()
        {
            ID = "DamageDealtVar_" + DamageId;
            InheritedPropertyType = PropertyType.Int;
        }

    }
}
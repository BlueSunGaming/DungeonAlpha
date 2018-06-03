using System;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class PhysicalDamageCombatNode : PropertyNode
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
        public override string Name { get { return "Physical Damage"; } }

        public override string Description
        {
            get { return "Represents the physical damage dealt/to deal"; }
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
            var variable = nodeChain.DamageDealt.Physical;
            var result = Convert.ToInt32(func(variable));
            nodeChain.DamageDealt.Physical = result;
            return result;
        }

        protected override void SetupParameters()
        {
        }

        public PhysicalDamageCombatNode()
        {
            ID = "DamageDealtVar_" + ID;
            InheritedPropertyType = PropertyType.Int;
        }

    }
}
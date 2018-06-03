using System;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class DefenderCombatNode : PropertyNode
    {
        [JsonIgnore]
        public override PropertyFamily PropertyFamily { get { return PropertyFamily.Object; } }

        [JsonIgnore]
        protected override bool InheritsPropertyType { get { return false; } }

        [JsonIgnore]
        protected override PropertyType PropertyNodeType { get { return PropertyType.CombatCharacter; } }

        [JsonIgnore]
        public override NodeType NodeType { get { return NodeType.Property; } }

        [JsonIgnore]
        public override string Name { get { return "Defender"; } }

        public override string Description
        {
            get { return "Represents the defending combat character"; }
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
            var variable = nodeChain.CombatantB;
            if (variable != null)
            {
                var result = func(variable);
                return result;
            }

            return null;
        }

        protected override void SetupParameters()
        {
        }

        public DefenderCombatNode()
        {
            InheritedPropertyType = PropertyType.CombatCharacter;
        }

    }
}
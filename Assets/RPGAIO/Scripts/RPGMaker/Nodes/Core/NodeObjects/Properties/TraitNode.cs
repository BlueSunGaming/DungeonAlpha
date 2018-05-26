using System;
using LogicSpawn.RPGMaker.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Property", "Player Stats")]
    public class TraitNode : PropertyNode
    {
        public override string Name
        {
            get { return "Player Trait Level"; }
        }

        public override string Description
        {
            get { return "Gets the player's traits's level."; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        public override PropertyFamily PropertyFamily
        {
            get { return PropertyFamily.Primitive; }
        }

        protected override PropertyType PropertyNodeType
        {
            get { return PropertyType.Int; }
        }

        protected override bool InheritsPropertyType
        {
            get { return false; }
        }

        protected override void SetupParameters()
        {
            Add("Name", PropertyType.String, null, "", PropertySource.EnteredOrInput, PropertyFamily.Object);
        }

        public override object EvaluateInput(NodeChain nodeChain, Func<object,object> func)
        {
            var statName = (string) ValueOf("Name");
            var attr = GetObject.PlayerCharacter.GetTrait(statName).Level;
            if (func != null)
            {
                var result = Convert.ToInt32(func(attr));
                GetObject.PlayerCharacter.GetTrait(statName).Level = result;
                GetObject.PlayerCharacter.FullUpdateStats();
            }

            return GetObject.PlayerCharacter.GetTrait(statName).Level;
        }
    }
}
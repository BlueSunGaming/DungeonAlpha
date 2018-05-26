using System;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Property", "Combat Stats")]
    public class GetTraitLevel : PropertyNode
    {
        public override string Name
        {
            get { return "Get Trait Level"; }
        }

        public override string Description
        {
            get { return "Gets the player's trait total."; }
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
            Add("Trait", PropertyType.Trait, null, "", PropertySource.EnteredOrInput, PropertyFamily.Object);
        }

        public override object EvaluateInput(NodeChain nodeChain, Func<object,object> func)
        {
            var traitID = (string)ValueOf("Trait");
            var attr = GetObject.PlayerCharacter.GetTraitByID(traitID).Level;
            if (func != null)
            {
                var result = Convert.ToInt32(func(attr));
                GetObject.PlayerCharacter.GetTraitByID(traitID).Level = result;
            }

            return GetObject.PlayerCharacter.GetTraitByID(traitID).Level;
        }
    }
}
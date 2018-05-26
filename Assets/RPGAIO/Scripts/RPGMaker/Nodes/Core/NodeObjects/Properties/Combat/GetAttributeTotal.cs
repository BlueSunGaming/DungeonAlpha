using System;
using LogicSpawn.RPGMaker.Beta;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Property", "Combat Stats")]
    public class GetAttributeTotal : PropertyNode
    {
        public override string Name
        {
            get { return "Attribute Total"; }
        }

        public override string Description
        {
            get { return "Gets the combatants's attribute total."; }
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
            Add("Combatant", PropertyType.CombatCharacter, null, "", PropertySource.InputOnly, PropertyFamily.Object);
            Add("Attribute", PropertyType.Attribute, null, "", PropertySource.EnteredOrInput, PropertyFamily.Object);
        }

        public override object EvaluateInput(NodeChain nodeChain, Func<object,object> func)
        {
            var attrID = (string) ValueOf("Attribute");
            var combatant = ValueOf("Combatant") as BaseCharacter ?? ((GameObject)ValueOf("Combatant")).GetComponent<BaseCharacterMono>().Character;

            var attr = combatant.GetAttributeByID(attrID).BaseValue;
            if (func != null)
            {
                var result = Convert.ToInt32(func(attr));
                combatant.GetAttributeByID(attrID).BaseValue = result;
            }

            return combatant.GetAttributeByID(attrID).TotalValue;
        }
    }
}
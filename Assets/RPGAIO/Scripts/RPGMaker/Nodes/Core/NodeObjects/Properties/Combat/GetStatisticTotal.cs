using System;
using LogicSpawn.RPGMaker.Beta;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Property", "Combat Stats")]
    public class GetStatisticTotal : PropertyNode
    {
        public override string Name
        {
            get { return "Statistic Total"; }
        }

        public override string Description
        {
            get { return "Gets the combatants's statistic total."; }
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
            get { return PropertyType.Float; }
        }

        protected override bool InheritsPropertyType
        {
            get { return false; }
        }

        protected override void SetupParameters()
        {
            Add("Combatant", PropertyType.CombatCharacter, null, null, PropertySource.InputOnly, PropertyFamily.Object);
            Add("Statistic", PropertyType.Statistic, null, "", PropertySource.EnteredOrInput, PropertyFamily.Object);
        }

        public override object EvaluateInput(NodeChain nodeChain, Func<object,object> func)
        {
            var statID = (string)ValueOf("Statistic");
            var combatant = ValueOf("Combatant") as BaseCharacter ?? ((GameObject)ValueOf("Combatant")).GetComponent<BaseCharacterMono>().Character;
            var attr = combatant.GetStatByID(statID).BaseValue;
            if (func != null)
            {
                var result = Convert.ToSingle(func(attr));
                combatant.GetStatByID(statID).BaseValue = result;
            }

            return combatant.GetStatByID(statID).TotalValue;
        }
    }
}
using System;
using LogicSpawn.RPGMaker.Beta;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Comparison", "")]
    public class IsAttackStyle : BooleanNode
    {
        public override string Name
        {
            get { return "Attack Style Check"; }
        }

        public override string Description
        {
            get { return "Returns true if the combatant uses the attack style."; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        protected override void SetupParameters()
        {
            Add("Combatant", PropertyType.CombatCharacter, null, "", PropertySource.InputOnly, PropertyFamily.Object);
            Add("Melee?", PropertyType.Bool, null, false, PropertySource.InputOnly);
        }

        protected override bool Eval(NodeChain nodeChain)
        {
                        var combatant = ValueOf("Combatant") as BaseCharacter ?? ((GameObject)ValueOf("Combatant")).GetComponent<BaseCharacterMono>().Character;
            
            var isMelee = (bool) ValueOf("Melee?");
            return (isMelee && combatant.AttackStyle == AttackStyle.Melee) ||
                   (!isMelee && combatant.AttackStyle == AttackStyle.Ranged);
        }
    }
}
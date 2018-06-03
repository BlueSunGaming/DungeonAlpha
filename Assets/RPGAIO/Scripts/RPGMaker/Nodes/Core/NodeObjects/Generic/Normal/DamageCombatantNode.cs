using System;
using LogicSpawn.RPGMaker.Beta;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Combat", "")]
    public class DamageCombatantNode : SimpleNode
    {
        public override string Name
        {
            get { return "Physical Damage Combatant"; }
        }

        public override string Description
        {
            get { return "Deals physical damage to a combatant."; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        public override bool CanBeLinkedTo
        {
            get
            {
                return true;
            }
        }

        public override string NextNodeLinkLabel(int index) 
        {
            return "Next";
        }

        protected override void SetupParameters()
        {
            Add("Combatant", PropertyType.CombatCharacter, null, "", PropertySource.InputOnly, PropertyFamily.Object);
            Add("Physical Damage", PropertyType.Int, null, 1, PropertySource.EnteredOrInput, PropertyFamily.Primitive); 
        }

        protected override void Eval(NodeChain nodeChain)
        {
            var damage = (int)ValueOf("Physical Damage");
                        var combatant = ValueOf("Combatant") as BaseCharacter ?? ((GameObject)ValueOf("Combatant")).GetComponent<BaseCharacterMono>().Character;
            if (combatant != null)
            {
                combatant.VitalHandler.TakeDamage(null, new Damage()
                                                            {
                                                                MinDamage = damage,
                                                                MaxDamage = damage
                                                            },false);
            }
        }
    }
}
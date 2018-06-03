using System;
using System.Linq;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Beta;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Combat", "")]
    public class ElementalDamageCombatantNode : SimpleNode
    {
        public override string Name
        {
            get { return "Elemental Damage Combatant"; }
        }

        public override string Description
        {
            get { return "Deals elemental damage to a combatant."; }
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
            Add("Elemental Name", PropertyType.String, null, "", PropertySource.EnteredOrInput, PropertyFamily.Primitive); 
            Add("Damage", PropertyType.Int, null, 1, PropertySource.EnteredOrInput, PropertyFamily.Primitive); 
        }

        protected override void Eval(NodeChain nodeChain)
        {
            var damage = (int)ValueOf("Damage");
            var element = (string)ValueOf("Elemental Name");
                        var combatant = ValueOf("Combatant") as BaseCharacter ?? ((GameObject)ValueOf("Combatant")).GetComponent<BaseCharacterMono>().Character;
  

            var elementalId = RPG.Combat.GetElementalIdByName(element);
            if (!string.IsNullOrEmpty(elementalId))
            {
                if (combatant != null)
                {
                    var damageToDeal = new Damage()
                                           {
                                               MinDamage = damage,
                                               MaxDamage = damage
                                           };

                    damageToDeal.ElementalDamages.First(e => e.ElementID == elementalId).MinDamage = damage;
                    damageToDeal.ElementalDamages.First(e => e.ElementID == elementalId).MaxDamage = damage;

                    combatant.VitalHandler.TakeDamage(null, damageToDeal, false);
                }
            }
            else
            {
                Debug.LogError("[RPGAIO] Deal elemental damage node did not find element named: " + element);
            }
            
        }
    }
}
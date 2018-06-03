using System.Linq;
using LogicSpawn.RPGMaker.Beta;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Comparison", "")]
    public class IsStatusEffectActive : BooleanNode
    {
        public override string Name
        {
            get { return "Is Status Effect Active"; }
        }

        public override string Description
        {
            get { return "Returns true if the combatant has the specified status effect active."; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        protected override void SetupParameters()
        {
            Add("Combatant", PropertyType.CombatCharacter, null, "", PropertySource.InputOnly, PropertyFamily.Object);
            Add("Status Effect", PropertyType.StatusEffect, null, "", PropertySource.InputOnly, PropertyFamily.Object);
        }

        protected override bool Eval(NodeChain nodeChain)
        {
            var combatant = ValueOf("Combatant") as BaseCharacter ?? ((GameObject)ValueOf("Combatant")).GetComponent<BaseCharacterMono>().Character;
            var statusEffectID = (string)ValueOf("Status Effect");
            return combatant.StatusEffects.FirstOrDefault(s => s.ID == statusEffectID) != null;
        }
    }
}
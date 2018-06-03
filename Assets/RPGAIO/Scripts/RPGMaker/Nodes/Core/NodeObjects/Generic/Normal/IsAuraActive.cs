using System.Linq;
using LogicSpawn.RPGMaker.Beta;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Comparison", "")]
    public class IsAuraActive : BooleanNode
    {
        public override string Name
        {
            get { return "Is Aura Active"; }
        }

        public override string Description
        {
            get { return "Returns true if the combatant has the specified aura active."; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        protected override void SetupParameters()
        {
            Add("Combatant", PropertyType.CombatCharacter, null, "", PropertySource.InputOnly, PropertyFamily.Object);
            Add("Aura", PropertyType.Skill, null, "", PropertySource.InputOnly, PropertyFamily.Object);
        }

        protected override bool Eval(NodeChain nodeChain)
        {
            var combatant = ValueOf("Combatant") as BaseCharacter ?? ((GameObject)ValueOf("Combatant")).GetComponent<BaseCharacterMono>().Character;
            var auraEffectID = (string)ValueOf("Aura");
            return combatant.AuraEffects.FirstOrDefault(s => s.SkillId == auraEffectID) != null;
        }
    }
}
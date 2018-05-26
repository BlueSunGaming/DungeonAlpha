using LogicSpawn.RPGMaker.Beta;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Comparison", "")]
    public class IsEnemyCheck : BooleanNode
    {
        public override string Name
        {
            get { return "Is Enemy Check"; }
        }

        public override string Description
        {
            get { return "Returns true if the combatant is an enemy type."; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        protected override void SetupParameters()
        {
            Add("Combatant", PropertyType.CombatCharacter, null, "", PropertySource.InputOnly, PropertyFamily.Object);
        }

        protected override bool Eval(NodeChain nodeChain)
        {
            var combatant = ValueOf("Combatant") as BaseCharacter ?? ((GameObject)ValueOf("Combatant")).GetComponent<BaseCharacterMono>().Character;
            return !(combatant is PlayerCharacter);
        }
    }
}
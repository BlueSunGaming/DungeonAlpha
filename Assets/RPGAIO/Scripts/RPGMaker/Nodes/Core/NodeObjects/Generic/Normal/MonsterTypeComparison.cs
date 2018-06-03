using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Comparison", "")]
    public class MonsterTypeComparison : BooleanNode
    {
        public override string Name
        {
            get { return "Monster Type Check"; }
        }

        public override string Description
        {
            get { return "Returns true if the combatant is of type."; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        protected override void SetupParameters()
        {
            Add("Enemy", PropertyType.CombatCharacter, null, "", PropertySource.InputOnly, PropertyFamily.Object);
            Add("Type", PropertyType.MonsterTypeDefinition,null,"");
        }

        protected override bool Eval(NodeChain nodeChain)
        {
            var combatant = (CombatCharacter)ValueOf("Enemy");
            var monsterId = combatant.MonsterTypeID;
            var compareId = (string)ValueOf("Type");
            return monsterId == compareId;
        }
    }
}
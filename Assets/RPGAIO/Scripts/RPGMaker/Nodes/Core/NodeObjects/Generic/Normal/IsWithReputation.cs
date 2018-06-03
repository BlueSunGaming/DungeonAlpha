using System.Linq;
using LogicSpawn.RPGMaker.Beta;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Reputation", "")]
    public class IsWithReputation : BooleanNode
    {
        public override string Name
        {
            get { return "Is With Reputation"; }
        }

        public override string Description
        {
            get { return "Returns true if the player is positive with the reputation or the enemy is part of it."; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        protected override void SetupParameters()
        {
            Add("Combatant", PropertyType.CombatCharacter, null, "", PropertySource.InputOnly, PropertyFamily.Object);
            Add("Reputation", PropertyType.ReputationDefinition, null, "", PropertySource.EnteredOrInput, PropertyFamily.Object);
        }

        protected override bool Eval(NodeChain nodeChain)
        {
            var combatant = ValueOf("Combatant") as BaseCharacter ?? ((GameObject)ValueOf("Combatant")).GetComponent<BaseCharacterMono>().Character;
            var enemyRep = combatant as CombatCharacter;
            var player = combatant as PlayerCharacter;
            var reputation = (string)ValueOf("Reputation");

            if(player != null)
            {
                var rep = GetObject.PlayerSave.QuestLog.AllReputations.FirstOrDefault(r => r.ReputationID == reputation);
                if(rep != null)
                {
                    return rep.IsPositive;
                }
            }

            return enemyRep != null && enemyRep.ReputationId == reputation;
        }
    }
}
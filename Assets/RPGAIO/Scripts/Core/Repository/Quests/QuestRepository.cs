using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class QuestRepository
    {
        public List<Quest> AllQuests
        {
            get { return AllSingleQuests.Concat(AllQuestChains.SelectMany(q => q.QuestsInChain)).ToList(); }
        }

        public List<Quest> AllSingleQuests;
        public List<QuestChain> AllQuestChains;
        public List<ReputationDefinition> AllReputations;

        public QuestRepository()
        {
            AllSingleQuests = new List<Quest>();
            AllQuestChains = new List<QuestChain>();
            AllReputations = new List<ReputationDefinition>();
            var npcRep = new ReputationDefinition()
                             {
                                 ID = "Core_NPCReputation", Name = "NonPlayerCharacters", IsTrackable = false, IsDefault = true
                             };
            //npcRep.AlliedFactions.Add(new FactionStatus() { ID = "Core_NPCReputation" , IsTrue = true });
            npcRep.EnemyFactions.Add(new FactionStatus() { ID = "Core_EnemyReputation", IsTrue = true });

            var enemyRep = new ReputationDefinition()
                               {
                                   ID = "Core_EnemyReputation",
                                   Name = "EnemyCharacters",
                                   IsTrackable = false,
                                   IsDefault = true,
                                   AttackIfBelowReputation = true,
                                   StartingValue = -1000,
                                   BelowReputationValue = 0
                               };

            enemyRep.EnemyFactions.Add(new FactionStatus() { ID = "Core_NPCReputation", IsTrue = true });
            //enemyRep.AlliedFactions.Add(new FactionStatus() { ID = "Core_EnemyReputation", IsTrue = true });

            AllReputations.Add(npcRep);
            AllReputations.Add(enemyRep);
        }

        public Quest GetQuest(string questId)
        {
            return AllSingleQuests.First(i => i.ID == questId);
        }

        public QuestChain GetQuestChain(string id)
        {
            return AllQuestChains.First(i => i.ID == id);
        }
    }
}
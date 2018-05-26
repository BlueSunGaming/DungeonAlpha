using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class QuestLog
    {
        public List<Reputation> AllReputations ;
        public List<Quest> AllObjectives ;

        public QuestLog()
        {
            AllObjectives = new List<Quest>();
            AllReputations = new List<Reputation>();
        }

        public void Init()
        {
            //todo: where the quest is my class
            var copyQuests = GeneralMethods.CopyObject(Rm_RPGHandler.Instance.Repositories.Quests.AllSingleQuests);
            var copyQuestchains = GeneralMethods.CopyObject(Rm_RPGHandler.Instance.Repositories.Quests.AllQuestChains.SelectMany(q => q.QuestsInChain)).ToList();

            AllObjectives.AddRange(copyQuests);
            AllObjectives.AddRange(copyQuestchains);

            foreach(var rep in Rm_RPGHandler.Instance.Repositories.Quests.AllReputations)
            {
                AllReputations.Add(new Reputation(){ReputationID = rep.ID, Value = rep.StartingValue});
            }
        }


        public void AddReputation(Reputation repToAdd)
        {
            var rep = AllReputations.First(r => r.ReputationID == repToAdd.ReputationID);
            rep.Value += repToAdd.Value;
        }

        [JsonIgnore]
        public List<Quest> ActiveObjectives
        {
            get { return AllObjectives.Where(o => o.IsAccepted && !o.TurnedIn).ToList(); }
        }

        [JsonIgnore]
        public List<Quest> NotStartedObjectives
        {
            get { return AllObjectives.Where(o => !o.IsAccepted && !o.TurnedIn).ToList(); }
        
        }
        [JsonIgnore]
        public List<Quest> CompletedObjectives
        {
            get { return AllObjectives.Where(o => o.TurnedIn).ToList(); }
        }
        
        public Quest GetObjective(string objectiveID)
        {
            return AllObjectives.First(o => o.ID == objectiveID);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class TalentRepository
    {
        public List<Talent> AllTalents;
        public List<TalentGroup> AllTalentGroups;

        public TalentRepository()
        {
            AllTalents = new List<Talent>();
            AllTalentGroups = new List<TalentGroup>();
        }

        public Talent Get(string talentID)
        {
            return AllTalents.First(i => i.ID == talentID);
        }
        public TalentGroup GetGroup(string groupId)
        {
            return AllTalentGroups.First(i => i.ID == groupId);
        }

        public List<Talent> GetTalents(string classID)
        {
            return AllTalents.Where(i => i.ClassIDs.Contains(classID)).ToList();
        }
    }
}
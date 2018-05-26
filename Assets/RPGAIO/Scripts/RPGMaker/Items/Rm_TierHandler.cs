using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;

namespace LogicSpawn.RPGMaker
{
    public class Rm_TierHandler
    {
        public int MaxLevel
        {
            get { return Rm_RPGHandler.Instance.Experience.MaxPlayerLevel; }
        }

        public List<Rm_Tier> Tiers;

        public Rm_TierHandler()
        {
            Tiers = new List<Rm_Tier>();
        }

        public string GetTierID(Item item)
        {
            var requiredLevel = item as IRequireLevel;
            if(requiredLevel != null)
            {
                var tier = Tiers.First(t => requiredLevel.RequiredLevel <= t.MaxLevel);
                return tier.TierID;
            }

            return Tiers.First().TierID;
        }

        public string GetTierName(string tier)
        {
            //todo : check this
            if (tier == "") return "Tier";
            var found = Tiers.FirstOrDefault(t => t.TierID == tier);
            return found != null ? found.Name : "Tier";
        }
    }
}
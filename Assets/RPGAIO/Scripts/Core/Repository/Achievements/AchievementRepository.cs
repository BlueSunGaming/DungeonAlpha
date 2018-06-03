using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class AchievementRepository
    {
        public List<Achievement> AllAchievements;

        public AchievementRepository()
        {
            AllAchievements = new List<Achievement>();
        }

        public Achievement Get(string achievementID)
        {
            return AllAchievements.First(i => i.ID == achievementID);
        }
    }
}
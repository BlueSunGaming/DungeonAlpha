using System;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker
{
    public class Rm_Tier
    {
        public string TierID = "";
        public string Name = "";
        public float minLevel;
        public float maxLevel;

        public Rm_Tier()
        {
            TierID = Guid.NewGuid().ToString();
            Name = "New Tier";
        }

        [JsonIgnore]
        public int MinLevel
        {
            get { return (int) minLevel; }
        }
        [JsonIgnore]
        public int MaxLevel
        {
            get { return (int)maxLevel; }
        }
    }
}
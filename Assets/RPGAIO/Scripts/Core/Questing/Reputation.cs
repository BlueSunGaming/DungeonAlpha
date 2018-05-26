using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class Reputation
    {
        public string ReputationID;
        public int Value;

        [JsonIgnore]
        public bool IsPositive
        {
            get { return Value > 0; }
        }
    }
}
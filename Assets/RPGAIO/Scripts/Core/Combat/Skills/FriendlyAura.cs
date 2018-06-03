using LogicSpawn.RPGMaker.Beta;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class FriendlyAura
    {
        [JsonIgnore]
        public BaseCharacterMono SourceCharacter;
        public AuraEffect AuraEffect;

        public FriendlyAura()
        {
            SourceCharacter = null;
            AuraEffect = null;
        }
    }
}
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class AuraEffect
    {
        public string SkillId;
        public bool TakeResourceAmountPerSec ;
        public float ResourcePerSecTimer;
        public bool ApplyToAllies ;
        public float Radius ;
        public SkillStatistics AuraEffectStats;

        [JsonIgnore]
        public TimedPassiveEffect PassiveEffect
        {
            get { return AuraEffectStats.Effect; }
        }

        [JsonIgnore]
        public bool HasDuration
        {
            get { return AuraEffectStats.Effect.HasDuration; }
        }
        [JsonIgnore]
        public float Duration
        {
            get { return AuraEffectStats.Effect.Duration; }
        }

        public AuraEffect()
        {
            TakeResourceAmountPerSec = false;
            ApplyToAllies = true;
            Radius = 5;
        }
    }
}
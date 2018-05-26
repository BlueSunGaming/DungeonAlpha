using System.Collections.Generic;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class AuraSkill : Skill
    {
        [JsonIgnore]
        public AuraEffect AuraEffect { 
            get
            {
                var auraEff = AuraEffectStatistics[CurrentRank];
                auraEff.AuraEffectStats = SkillStatistics[CurrentRank];
                return GeneralMethods.CopyObject(auraEff);
            } 
        }

        public List<AuraEffect> AuraEffectStatistics ;

        public AuraSkill()
        {
            Name = "New Aura Skill";
            SkillType = SkillType.Aura;
            AuraEffectStatistics = new List<AuraEffect>();
        }
    }
}
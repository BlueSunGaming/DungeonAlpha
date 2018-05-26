using System.Collections.Generic;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class RestorationSkill : Skill
    {
        [JsonIgnore]
        public Restoration Restoration { get { return RestorationEffects[CurrentRank]; } }
        public List<Restoration> RestorationEffects;

        public RestorationSkill()
        {
            RestorationEffects = new List<Restoration>();
            Name = "New Restoration Skill";
            SkillType = SkillType.Restoration;
        }
    }
}
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class AreaOfEffectSkill : Skill
    {
        public bool HitMultipleTimes ;
        public float DelayBetweenHits ;
        public AOEShape Shape;

        public List<float> TimeTillDestroy ;
        public List<float> WidthStatistics ;
        public List<float> LengthStatistics ;
        public List<float> HeightStatistics ;

        [JsonIgnore]
        public float Duration { get { return TimeTillDestroy[CurrentRank]; } }

        [JsonIgnore]
        public float Width { get { return WidthStatistics[CurrentRank]; } }

        [JsonIgnore]
        public float Diameter { get { return WidthStatistics[CurrentRank]; } }

        [JsonIgnore]
        public float Length { get { return LengthStatistics[CurrentRank]; } }

        [JsonIgnore]
        public float Height { get { return Shape == AOEShape.Sphere ? WidthStatistics[CurrentRank] : HeightStatistics[CurrentRank]; } }


        public AreaOfEffectSkill()
        {
            Name = "New AOE Skill";
            Shape = AOEShape.Cuboid;
            SkillType = SkillType.Area_Of_Effect;
            WidthStatistics = new List<float>();
            LengthStatistics = new List<float>();
            HeightStatistics = new List<float>();
            TimeTillDestroy = new List<float>();
            TargetType = TargetType.Enemy;
        }
    }

    public enum AOEShape
    {
        Cuboid,
        Sphere
    }
}
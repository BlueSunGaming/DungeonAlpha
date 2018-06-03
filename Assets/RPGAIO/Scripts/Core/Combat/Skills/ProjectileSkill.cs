using System.Collections.Generic;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class ProjectileSkill : Skill
    {
        [JsonIgnore]
        public bool IsPiercing { get { return ProjectileSkillStatistics[CurrentRank].IsPiercing; } }
        [JsonIgnore]
        public float NumberOfPierces { get { return ProjectileSkillStatistics[CurrentRank].NumberOfPierces; } }
        [JsonIgnore]
        public float[] PiercingScaling { get { return ProjectileSkillStatistics[CurrentRank].PiercingScaling; } }
        [JsonIgnore]
        public float Speed { get { return ProjectileSkillStatistics[CurrentRank].Speed; } }
        [JsonIgnore]
        public float TimeTillDestroy { get { return ProjectileSkillStatistics[CurrentRank].TimeTillDestroy; } }
        [JsonIgnore]
        public bool AlwaysLockOn { get { return ProjectileSkillStatistics[CurrentRank].AlwaysLockOn; } }

        public AudioContainer TravelSound;
        public List<ProjectileSkillStatistics> ProjectileSkillStatistics;

        public ProjectileSkill()
        {
            Name = "New Projectile Skill";
            ProjectileSkillStatistics = new List<ProjectileSkillStatistics>();
            TravelSound = new AudioContainer();
            SkillType = SkillType.Projectile;
            TargetType = TargetType.Enemy;
        }
    }

    public class ProjectileSkillStatistics
    {
        public bool IsPiercing ;
        public int NumberOfPierces ;
        public float[] PiercingScaling ;
        public float Speed ;
        public float TimeTillDestroy ;
        public bool AlwaysLockOn;

        public ProjectileSkillStatistics()
        {
            IsPiercing = false;
            NumberOfPierces = 1;
            PiercingScaling = new float[0];
            Speed = 1;
            TimeTillDestroy = 3.0f;
            AlwaysLockOn = false;
        }

    }
}
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class MeleeSkill : Skill
    {
        public bool SeperateCastPerAttack;

        [JsonIgnore]
        public int Attacks { get { return MeleeSkillDetails[CurrentRank].Attacks; } }

        [JsonIgnore]
        public MeleeSkillDetail Details { get { return MeleeSkillDetails[CurrentRank]; } }

        [JsonIgnore]
        public float MaxTimeBetweenCasts { get { return MeleeSkillDetails[CurrentRank].MaxTimeBetweenCasts; } }

        public List<StringField> ImpactPrefabPaths;
        public List<MeleeSkillDetail> MeleeSkillDetails;
        public List<AudioContainer> MeleeSkillSounds;
        public List<AudioContainer> MeleeCastingSounds;
        public List<AudioContainer> MeleeImpactSounds;
        public List<MeleeSkillAnimation> MeleeAnimations;
        public int CurrentAttack;

        public MeleeSkill()
        {
            MeleeSkillDetails = new List<MeleeSkillDetail>();
            MeleeSkillSounds = new List<AudioContainer>();
            MeleeCastingSounds = new List<AudioContainer>();
            MeleeImpactSounds = new List<AudioContainer>();
            ImpactPrefabPaths = new List<StringField>();
            Name = "New Melee Skill";
            SkillType = SkillType.Melee;
            SeperateCastPerAttack = false;
            MeleeAnimations = new List<MeleeSkillAnimation>();
            CurrentAttack = 0;
        }
    }

    public class MeleeSkillAnimation
    {
        public string ClassID;
        public List<SkillAnimationDefinition> Definitions;

        public MeleeSkillAnimation()
        {
            Definitions = new List<SkillAnimationDefinition>();
        }
    }

    public class MeleeSkillDetail
    {
        public List<string> ImpactPrefabPaths; 
        public List<float> MeleeSkillScalings;
        public List<MeleeMoveDefinition> MeleeMoveDefinitions;

        public int Attacks;
        public float MaxTimeBetweenCasts;

        public MeleeSkillDetail()
        {
            MeleeSkillScalings = new List<float>();
            MeleeMoveDefinitions = new List<MeleeMoveDefinition>();
            ImpactPrefabPaths = new List<string>();
            Attacks = 1;
            MaxTimeBetweenCasts = 2.0f;
        }
    }

    public class MeleeMoveDefinition
    {
        public SkillMovementType MovementType;
        public float MoveToSpeed;
        public float JumpToHeight;
        public float LandTime;
        public string MovingToPrefab;
        public string LandPrefab;

        public MeleeMoveDefinition()
        {
            MovementType = SkillMovementType.StayInPlace;
            MoveToSpeed = 3.0f;
            JumpToHeight = 5.0f;
            LandTime = 0.1f;
        }
    }
}
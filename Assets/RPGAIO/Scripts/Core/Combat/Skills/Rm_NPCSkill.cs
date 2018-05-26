using System.Collections.Generic;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class Rm_NPCSkill
    {
        public string SkillID;
        public Rm_EnemySkillCastType CastType;
        public float Paramater;
        public bool RequireResource;
        public SkillAnimationDefinition AnimationToUse;
        //todo: this
        public MeleeSkillAnimation MeleeSkillAnimations;
        public int Rank;
        public float NthSecondsTimer;
        public int Priority;

        [JsonIgnore] 
        public Skill SkillRef;


        public Rm_NPCSkill()
        {
            SkillID = "";
            AnimationToUse = new SkillAnimationDefinition();
            MeleeSkillAnimations = new MeleeSkillAnimation();
            RequireResource = true;
            CastType = Rm_EnemySkillCastType.WhenOffCooldown;
            Paramater = 0;
            Rank = 0;
            SkillRef = null;
            NthSecondsTimer = 0;
            Priority = 10;
        }
    }
}
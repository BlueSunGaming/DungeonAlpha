namespace LogicSpawn.RPGMaker.Core
{
    public class TalentEffect
    {
        public PassiveEffect Effect;
        public bool IsAlsoSkill;
        public string SKillID;
        public string Description;

        public int ReqTraitLevelToLevel;
        public int SkillPointsToLevel;
        public int LevelReqToLevel;
        public int ReqTimesUsed;


        public TalentEffect()
        {
            Effect = new PassiveEffect();
            Description = "";
        }
    }
}
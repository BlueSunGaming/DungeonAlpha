namespace LogicSpawn.RPGMaker.Core
{
    public class SkillAnimationDefinition
    {
        public string ClassID;


        public string LegacyAnim;

        public string CastingLegacyAnim;


        public string ApproachLegacyAnim;


        public string LandLegacyAnim;

        //Mecanim:

        public int CastSkillAnimSet;
        public int CastAnimNumber;

        public int CastingSkillAnimSet;
        public int CastingAnimNumber;

        public int ApproachSkillAnimSet;
        public int ApproachAnimNumber;

        public int LandSkillAnimSet;
        public int LandAnimNumber;
    }

    public class ClassAnimationDefinition
    {
        public string ClassID;
        public string LegacyAnim;

        public int AnimNumber;
    }
}
namespace LogicSpawn.RPGMaker.Core
{
    public class SkillMetaSusceptibility
    {
        public string ID;
        public float AdditionalDamage; 
        
        public bool HasDuration;
        public float Duration;

        public SkillMetaSusceptibility()
        {
            HasDuration = false;
            Duration = 0;
        }
    }
}
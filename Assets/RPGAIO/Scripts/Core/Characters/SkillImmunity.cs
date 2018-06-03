namespace LogicSpawn.RPGMaker.Core
{
    public class SkillImmunity
    {
        public string ID;

        public bool HasDuration;
        public float Duration;

        public SkillImmunity()
        {
            HasDuration = false;
            Duration = 0;
        }
    }
}
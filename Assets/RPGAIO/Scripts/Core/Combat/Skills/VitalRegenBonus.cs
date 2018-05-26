namespace LogicSpawn.RPGMaker.Core
{
    public class VitalRegenBonus
    {
        public string VitalID ;
        public float RegenBonus;

        public bool HasDuration;
        public float Duration;

        public VitalRegenBonus()
        {
            VitalID = "";
            RegenBonus = 0.0F;
            HasDuration = false;
            Duration = 0;
        }
    }
}
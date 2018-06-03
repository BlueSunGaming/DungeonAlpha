namespace LogicSpawn.RPGMaker.Core
{
    public class Restoration
    {
        public RestorationType RestorationType ;
        
        public string VitalToRestoreID;
        
        public bool FixedRestore;
        public int AmountToRestore ;
        public float PercentToRestore ;
        
        public float SecBetweenRestore;
        public float Duration;

        public float IntervalTimer;

        public string SkillMetaId;

        public Restoration()
        {
            SkillMetaId = null;
            RestorationType = RestorationType.Instant;
            VitalToRestoreID = "";
            FixedRestore = true;
            AmountToRestore = 100;
            Duration = 1.0f;
        }
    }
}
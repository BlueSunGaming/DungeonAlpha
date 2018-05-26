namespace LogicSpawn.RPGMaker
{
    public class Rm_AttributeScalings
    {
        public string AttributeID;
        public Rm_ScalingType ScalingType;
        public string VitalID;
        public string StatisticID; //or statisticname
        public float Amount;

        public Rm_AttributeScalings(Rm_ScalingType scalingType)
        {
            ScalingType = scalingType;
            AttributeID = "";
            VitalID = "";
            StatisticID = "";
            Amount = 0;
        }
    }
}
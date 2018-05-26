namespace LogicSpawn.RPGMaker.Core
{
    public class AchievementProgress
    {
        public string Label ;
        public int CurrentValue ;
        public int TargetValue ;

        public AchievementProgress()
        {
            Label = "Progress";
            CurrentValue = TargetValue = 0;
        }

        public bool Done
        {
            get { return CurrentValue >= TargetValue; }
        }
    }
}
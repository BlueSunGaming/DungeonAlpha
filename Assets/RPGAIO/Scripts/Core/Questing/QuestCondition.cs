using LogicSpawn.RPGMaker;

namespace LogicSpawn.RPGMaker.Core
{
    public class QuestCondition
    {
		public ConditionType ConditionType ;
        public bool IsDone ;

        public bool UseCustomText;
        public string CustomText;
        public string CustomCompletedText;

        public QuestCondition()
        {
            ConditionType = ConditionType.Kill;
            CustomText = "Complete the Objective.";
            CustomCompletedText = "Objective Complete.";
        }
    }
}
namespace LogicSpawn.RPGMaker.Core
{
    public class KillCondition : QuestCondition
    {
        public bool IsNPC;
        public string CombatantID ;
        public int NumberToKill ;
        public int NumberKilled ;

        public KillCondition()
        {
            ConditionType = ConditionType.Kill;
            NumberToKill = 1;
        }
    }
}
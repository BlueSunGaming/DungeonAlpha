namespace LogicSpawn.RPGMaker.Core
{
    public class CustomCondition : QuestCondition
    {
        public Rm_CustomVariableGetSet CustomVariableRequirement;

        public CustomCondition()
        {
            ConditionType = ConditionType.Custom;
            CustomVariableRequirement = new Rm_CustomVariableGetSet();
        }
    }
}
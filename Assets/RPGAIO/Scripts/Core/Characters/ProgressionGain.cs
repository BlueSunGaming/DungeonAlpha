namespace LogicSpawn.RPGMaker.Core
{
    public class ProgressionGain
    {
        public bool GainExp;
        public bool GainSkillPoints;
        public bool GainTraitExp;
        public int ExpGained;
        public int SkillPointsGained;
        public string TraitID;
        public int TraitExpGained;
        public bool GainExpWithDefinition;
        public bool GainSkillWithDefinition;
        public bool GainTraitWithDefinition;
        public string GainExpWithDefinitionID;
        public string GainSkillWithDefinitionID;
        public string GainTraitWithDefinitionID;
        public int CombatantLevel;

        public ProgressionGain()
        {
            GainExp = true;
            GainSkillPoints = false;
            GainTraitExp = false;
            ExpGained = 1;
            SkillPointsGained = 1;
            TraitExpGained = 1;
            CombatantLevel = -1;
        }
    }
}
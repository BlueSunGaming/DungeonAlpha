using System.Collections.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    public class PassiveEffect
    {
        public List<AttributeBuff> AttributeBuffs ;
        public List<VitalBuff> VitalBuffs ;
        public List<StatisticBuff> StatisticBuffs ;

        public List<VitalRegenBonus> VitalRegenBonuses ;
        public List<Rm_CustomVariableGetSet> CustomVariableSetters ;
        public List<Rm_CustomVariableGetSet> CustomVariableSettersOnDisable;
        public List<ReduceStatusDuration> StatusDurationReduction;

        public bool RemoveStatusEffect;
        public string RemoveStatusEffectID ;

        public bool AddSkillImmunity;
        public string SkillImmunityID;

        public bool AddSkillSusceptibility;
        public string SkillSusceptibilityID;
        public float SkillSusceptibilityAmount;

        public bool RunsEvent ;
        public string RunEventID ;
        
        public bool HasProcEffect;
        public Rm_ProcEffect ProcEffect ;

        public PassiveEffect()
        {

            CustomVariableSetters = new List<Rm_CustomVariableGetSet>();
            CustomVariableSettersOnDisable = new List<Rm_CustomVariableGetSet>();
            StatusDurationReduction = new List<ReduceStatusDuration>();
            AttributeBuffs = new List<AttributeBuff>();
            VitalBuffs = new List<VitalBuff>();
            StatisticBuffs = new List<StatisticBuff>();
            VitalRegenBonuses = new List<VitalRegenBonus>();
            ProcEffect = new Rm_ProcEffect();
        }
    }
}
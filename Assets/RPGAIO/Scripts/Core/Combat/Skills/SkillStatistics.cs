namespace LogicSpawn.RPGMaker.Core
{
    public class SkillStatistics
    {
        public string Description;

        public string ResourceRequiredId;

        public TimedPassiveEffect Effect;
        public float CoolDownTime ;
        public float TotalCastTime ;
        public float CastingTime ;
        public Damage Damage ;
        public int BonusTaunt;
        public int ResourceRequirement ;

        public int ReqTraitLevelToLevel;
        public int SkillPointsToLevel;
        public int LevelReqToLevel;

        public bool ApplyDOTOnHit ;
        public float ChanceToApplyDOT;
        public DamageOverTime DamageOverTime ;

        public int ResourceAddedAmount ;

        public bool RemoveStatusEffect ;
        public float ChanceToRemoveStatusEffect ;
        public string RemoveStatusEffectID ;

        public bool ApplyStatusEffect ;
        public bool ApplyStatusEffectWithDuration ;
        public float ApplyStatusEffectDuration ;
        public float ChanceToApplyStatusEffect;
        public string StatusEffectID;

        public bool HasProcEffect;
        public Rm_ProcEffect ProcEffect;

        public bool HasDuration;
        public float SpawnForTime;
        public bool LimitSpawnInstances;
        public int MaxInstances;

        public bool RunEventOnHit;
        public string EventOnHitID;

        public bool GivePlayerItem;
        public string ItemToGiveID;
        public int ItemToGiveAmount;

        public float VitalConditionParamater;
        


        public SkillStatistics()
        {
            Effect = new TimedPassiveEffect();
            DamageOverTime = new DamageOverTime();
            Damage = new Damage();
            ProcEffect = new Rm_ProcEffect();

            SpawnForTime = 10;

            HasDuration = true;
            LimitSpawnInstances = true;
            MaxInstances = 1;

            SkillPointsToLevel = 1;
            CoolDownTime = 1.0f;
            LevelReqToLevel = 1;

            Description = "";

            BonusTaunt = 0;

            ChanceToRemoveStatusEffect = 1.0f;
            ChanceToApplyStatusEffect = 1.0f;

            ApplyStatusEffectWithDuration = true;
            ApplyStatusEffectDuration = 5.0f;

            RemoveStatusEffectID = "";
            StatusEffectID = "";
            EventOnHitID = "";
            ItemToGiveID = "";
        }
    }
}
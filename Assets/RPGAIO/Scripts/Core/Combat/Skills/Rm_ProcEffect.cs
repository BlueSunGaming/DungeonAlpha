namespace LogicSpawn.RPGMaker.Core
{
    public class Rm_ProcEffect
    {
        public Rm_ProcCondition ProcCondition;
        public float Parameter;
        public int ParameterCounter;
        public Rm_ProcEffectType ProcEffectType;

        public Rm_PullTowardsType PullType;
        public bool PullAllTheWay;

        public float EffectParameter;
        public string EffectParameterString;

        public bool HasDuration;
        public float Duration;

        public Rm_ProcEffect()
        {
            ProcCondition = Rm_ProcCondition.On_Hit;
            Parameter = 1.0f;
            ProcEffectType = Rm_ProcEffectType.KnockBack;
            PullType = Rm_PullTowardsType.CasterOrSkill;
            EffectParameter = 1.0f;
            HasDuration = false;
            PullAllTheWay = true;
        }
    }

    public enum Rm_PullTowardsType
    {
        CasterOrSkill,
        TargetedPosition
    }
}
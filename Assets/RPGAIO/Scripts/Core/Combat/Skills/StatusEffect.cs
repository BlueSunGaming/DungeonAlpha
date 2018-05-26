using System;

namespace LogicSpawn.RPGMaker.Core
{
    public class StatusEffect
    {
        public string ID;

        public string Name ;
        public TimedPassiveEffect Effect;

        public bool CausesDOT;
        public DamageOverTime DamageOverTime ;

        public bool CauseStun;
        public bool CauseAnimationFreeze;
        public bool CauseRetreat;
        public bool CauseSilence;

        public ImageContainer Image;

        public bool ApplyToNearbyAllies;
        public float WithinRadius;
        
        public bool HasSkillMeta;
        public string SkillMetaID;

        public StatusEffect()
        {
            ID = Guid.NewGuid().ToString();
            Effect = new TimedPassiveEffect();
            Name = "New Status Effect";
            DamageOverTime = new DamageOverTime();
            CauseSilence = CauseStun = CauseAnimationFreeze = false;
            Image = new ImageContainer();
            HasSkillMeta = false;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
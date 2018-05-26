using System;

namespace LogicSpawn.RPGMaker.Core
{
    public class TimedPassiveEffect : PassiveEffect
    {
        public string ID;

        public string ActivePrefab;
        public string OnActivatePrefab;
        public string OnExpiredPrefab;

        public bool CanBeCancelled;
        public string CancellingStatusEffectID;

        public bool HasDuration;
        public float Duration;
        

        public TimedPassiveEffect()
        {
            ID = Guid.NewGuid().ToString();
            HasDuration = true;
            Duration = 10;
        }
    }
}
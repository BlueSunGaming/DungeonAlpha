using System;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class DamageOverTime
    {
        public string ID;
        public string InstanceID;

        public string DoTName ;
        public bool HasDuration ;
        public float Duration ;
        public float TimeBetweenTick ;
        public Damage DamagePerTick ;
        public float IntervalTimer;
        
        public string SkillMetaID;

        public string ActivePrefab;
        public string DamageTickPrefab;
        public string OnExpiredPrefab;
        public string OnActivatePrefab;

        [JsonIgnore]
        public BaseCharacter Attacker;

        

        public DamageOverTime()
        {
            ID = Guid.NewGuid().ToString();
            Attacker = null;
            IntervalTimer = 0;
            DoTName = "";
            HasDuration = true;
            Duration = 1;
            TimeBetweenTick = 1;
            DamagePerTick = new Damage();
        }
    }
}
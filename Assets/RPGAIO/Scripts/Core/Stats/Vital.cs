using LogicSpawn.RPGMaker.API;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class Vital
    {
        public string ID ;
        public int BaseValue ;
        public int ScaledBaseValue;
        public int AttributeValue ;
        public int SkillValue ;
        public int EquipValue ;
        
        public float RegenPercentValue ;
        public bool IsHealth ;

        public bool AlwaysStartsAtZero;
        public bool RegenWhileInCombat;

        public bool HasUpperLimit;
        public int UpperLimit;

        public bool ReduceHealthIfZero;
        public float ReductionIntervalSeconds;
        public bool ReduceByFixedAmount;
        public int ReductionFixedAmount;
        public float ReductionPercentageAmount;

        public float ReductionIntervalTimer;

        public Rm_UnityColors Color;

        public int _currentValue;

        [JsonIgnore]
        public int CurrentValue
        {
            get
            {
                if ( _currentValue <= MaxValue) return _currentValue;

                _currentValue = MaxValue;
                return _currentValue;
            }
            set
            {
                _currentValue = value;
                if (_currentValue < 0) _currentValue = 0;
            }
        }

        public Vital()
        {
            ReductionIntervalTimer = 0.0f;
        }

        [JsonIgnore]
        public int MaxValue
        {
            get
            {
                var totalValue = ScaledBaseValue + AttributeValue + EquipValue + SkillValue;
                if(!HasUpperLimit)
                    return totalValue;

                return Mathf.Min(UpperLimit, totalValue);
            }
        }

        

        public void Reset()
        {
            AttributeValue = SkillValue = EquipValue = ScaledBaseValue = 0;
        }

        public string GetName()
        {
            return RPG.Stats.GetVitalName(ID);
        }

        public string GetDescription()
        {
            return RPG.Stats.GetVitalDesc(ID);
        }
    }
}

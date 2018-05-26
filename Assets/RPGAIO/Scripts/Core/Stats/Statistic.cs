using LogicSpawn.RPGMaker.API;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class Statistic
    {
        public string ID ;
        public StatisticType StatisticType ;
        public float BaseValue ;
        public float ScaledBaseValue;
        public float EquipValue ;
        public float AttributeValue ; //todo: remove this as we use scaledbasevalue
        public float SkillValue ;

        public bool HasMaxValue;
        public float MaxValue;
        public Rm_UnityColors Color;

        public float TotalValue
        {
            get
            {
                var totalValue = ScaledBaseValue + EquipValue + AttributeValue + SkillValue;
                if (!HasMaxValue)
                    return totalValue;

                return Mathf.Min(totalValue, MaxValue);
            }
        }

        

        public void Reset()
        {
            AttributeValue = SkillValue = EquipValue = ScaledBaseValue = 0;
        }

        public string GetName()
        {
            return RPG.Stats.GetStatisticName(ID);
        }
        
        public string GetDescription()
        {
            return RPG.Stats.GetStatisticDesc(ID);
        }
    }
}

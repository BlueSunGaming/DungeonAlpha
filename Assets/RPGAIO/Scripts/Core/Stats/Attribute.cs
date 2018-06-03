using LogicSpawn.RPGMaker.API;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class Attribute
    {
        public string ID ;
        public int BaseValue ;
        public int EquipValue ;
        public int SkillValue ;
        public int TempValue ;
        public bool HasMaxValue;
        public int MaxValue;
        public Rm_UnityColors Color;

        public int TotalValue
        {
            get
            {
                var totalValue = BaseValue + EquipValue + SkillValue;
                if (!HasMaxValue)
                    return totalValue;

                return Mathf.Min(MaxValue, totalValue);
            }
        }

        public string GetName()
        {
            return RPG.Stats.GetAttributeName(ID);
        }

        public string GetDescription()
        {
            return RPG.Stats.GetAttributeDesc(ID);
        }

        public void Reset()
        {
            SkillValue = EquipValue = 0;
        }
    }
}

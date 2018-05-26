using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;

namespace LogicSpawn.RPGMaker.Core
{
    public class Socket : Item
    {
        public List<AttributeBuff> AttributeBuffs ;
        public List<VitalBuff> VitalBuffs ;
        public List<StatisticBuff> StatisticBuffs ;



        public List<ReduceStatusDuration> StatusDurationReduction;
        public List<VitalRegenBonus> VitalRegenBonuses;
        public List<SkillImmunity> SkillMetaImmunitiesID;
        public List<SkillMetaSusceptibility> SkillMetaSusceptibilities;

        public Socket()
        {
            AttributeBuffs = new List<AttributeBuff>();
            VitalBuffs = new List<VitalBuff>();
            StatisticBuffs = new List<StatisticBuff>();
            ItemType = ItemType.Socket;

            StatusDurationReduction = new List<ReduceStatusDuration>();
            VitalRegenBonuses = new List<VitalRegenBonus>();
            SkillMetaImmunitiesID = new List<SkillImmunity>();
            SkillMetaSusceptibilities = new List<SkillMetaSusceptibility>();
        }

        public void AddAttributeBuff(string attributeID, int value)
        {
            var alreadyExists = AttributeBuffs.FirstOrDefault(a => a.AttributeID == attributeID);
            if (alreadyExists != null)
            {
                alreadyExists.Amount += value;
            }
            else
            {
                AttributeBuffs.Add(new AttributeBuff(attributeID, value));
            }
        }

        public void AddVitalBuff(string vitalID, int value)
        {
            var alreadyExists = VitalBuffs.FirstOrDefault(v => v.VitalID == vitalID);
            if (alreadyExists != null)
            {
                alreadyExists.Amount += value;
            }
            else
            {
                VitalBuffs.Add(new VitalBuff(vitalID, value));
            }
        }

        public void AddStatisticBuff(string statID, float value)
        {
            var alreadyExists = StatisticBuffs.FirstOrDefault(s => s.StatisticID == statID);
            if (alreadyExists != null)
            {
                alreadyExists.Amount += value;
            }
            else
            {
                StatisticBuffs.Add(new StatisticBuff(statID, value));
            }
        } 
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    public class BuffItem : Item, IRequireLevel
    {
        public bool AllClasses ;
        public string ClassID ; //todo: remove this
        public List<string> ClassIDs;
        public int RequiredLevel { get; set; }

        public List<AttributeBuff> AttributeBuffs ;
        public List<VitalBuff> VitalBuffs ;
        public List<StatisticBuff> StatisticBuffs ;
        public SocketHolder SocketHolder ;

        public bool SetCVarOnEquip;
        public bool SetCVarOnUnEquip;

        public Rm_CustomVariableGetSet CustomVariableOnEquip;
        public Rm_CustomVariableGetSet CustomVariableOnUnEquip;

        public List<ReduceStatusDuration> StatusDurationReduction;
        public List<VitalRegenBonus> VitalRegenBonuses;
        public List<SkillImmunity> SkillMetaImmunitiesID;
        public List<SkillMetaSusceptibility> SkillMetaSusceptibilities;

        public BuffItem()
        {
            AttributeBuffs = new List<AttributeBuff>();
            VitalBuffs = new List<VitalBuff>();
            StatisticBuffs = new List<StatisticBuff>();
            SocketHolder = new SocketHolder();
            RequiredLevel = 1;
            ClassID = "???";
            ClassIDs = new List<string>();
            AllClasses = true;
            CustomVariableOnEquip = new Rm_CustomVariableGetSet();
            CustomVariableOnUnEquip = new Rm_CustomVariableGetSet();

            StatusDurationReduction = new List<ReduceStatusDuration>();
            VitalRegenBonuses = new List<VitalRegenBonus>();
            SkillMetaImmunitiesID = new List<SkillImmunity>();
            SkillMetaSusceptibilities = new List<SkillMetaSusceptibility>();
        }

        public void AddAttributeBuff(string attributeID, int value)
        {
            var alreadyExists = AttributeBuffs.FirstOrDefault(a => a.AttributeID == attributeID);
            if(alreadyExists != null)
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

        public bool AddSocket(Socket socket)
        {
            for (int i = 0; i < SocketHolder.Count; i++)
            {
                if(SocketHolder.Sockets[i] == null)
                {
                    SocketHolder.Sockets[i] = socket;
                    return true;
                }
            }

            return false;
        }

        public void RemoveSocket(Socket socket)
        {
            var foundSocket = Array.IndexOf(SocketHolder.Sockets, socket);
            SocketHolder.Sockets[foundSocket] = null;
        }

        public override string GetTooltipDescription()
        {
            var tooltip = "";
            foreach(var attributeBuff in AttributeBuffs)
            {
                var color = RPG.Stats.GetAttributeColorById(attributeBuff.AttributeID);
                var name = RPG.Stats.GetAttributeName(attributeBuff.AttributeID);
                tooltip += RPG.UI.FormatLine(color, string.Format("+{0} {1}", attributeBuff.Amount, name));
            }

            foreach(var statisticBuff in StatisticBuffs)
            {
                var color = RPG.Stats.GetStatisticColorById(statisticBuff.StatisticID);
                var name = RPG.Stats.GetStatisticName(statisticBuff.StatisticID);
                var isPercentage = RPG.Stats.IsStatisticPercentageInUI(statisticBuff.StatisticID);

                if(isPercentage)
                {
                    tooltip += RPG.UI.FormatLine(color, string.Format("+{0} {1}", (statisticBuff.Amount * 100).ToString("N2") + "%", name));
                }
                else
                {
                    tooltip += RPG.UI.FormatLine(color, string.Format("+{0} {1}", statisticBuff.Amount, name)); 
                }
            }
            foreach(var vitalBuff in VitalBuffs)
            {
                var color = RPG.Stats.GetVitalColorById(vitalBuff.VitalID);
                var name = RPG.Stats.GetVitalName(vitalBuff.VitalID);
                tooltip += RPG.UI.FormatLine(color, string.Format("+{0} {1}", vitalBuff.Amount, name));
            }
            foreach(var immun in SkillMetaImmunitiesID)
            {
                var name = RPG.Stats.GetMetaName(immun.ID);
                tooltip += string.Format("+ {0} Immunity", name) + "\n";
            }
            foreach(var sus in SkillMetaSusceptibilities)
            {
                var name = RPG.Stats.GetMetaName(sus.ID);
                tooltip += string.Format("+ {0} {1} Susceptibility", (sus.AdditionalDamage * 100).ToString("N2") + "%", name) + "\n";
            }
            foreach(var statReduc in StatusDurationReduction)
            {
                var isPercentage = statReduc.IsPercentageDecrease;
                var name = RPG.Stats.GetStatusEffectName(statReduc.StatusEffectID);

                if (isPercentage)
                {
                    tooltip += string.Format("- {0} on duration of {1}", (statReduc.DecreaseAmount * 100).ToString("N2") + "%", name) + "\n";
                }
                else
                {
                    tooltip += string.Format("- {0} seconds of {1}", statReduc.DecreaseAmount, name) + "\n";
                }

            }
            foreach(var vitalRegen in VitalRegenBonuses)
            {
                var name = RPG.Stats.GetVitalName(vitalRegen.VitalID);
                tooltip += string.Format("+ {0} {1} Regeneration", (vitalRegen.RegenBonus * 100).ToString("N2") + "%", name) + "\n";
            }


            var requirements = "";
            if(Rm_RPGHandler.Instance.Items.ItemsHaveRequiredLevel)
            {
                var color = GetObject.PlayerCharacter.Level >= RequiredLevel ? Rm_UnityColors.White : Rm_UnityColors.Red;
                requirements += RPG.UI.FormatLine(color, string.Format("Requires Level {0}", RequiredLevel));
            }
            if(Rm_RPGHandler.Instance.Items.LimitItemsToClass && !AllClasses)
            {
                var color = ClassIDs.FirstOrDefault(c => c == GetObject.PlayerCharacter.PlayerCharacterID) != null ? Rm_UnityColors.White : Rm_UnityColors.Red;
                var classes = ClassIDs.Select(c => RPG.Player.GetClassName(c)).OrderBy(c => c).OxbridgeAnd(", ", " or ");
                requirements += RPG.UI.FormatLine(color, string.Format("Requires {0}", classes));
            }

            if (!string.IsNullOrEmpty(requirements))
            {
                requirements = "\n\n" + requirements;
            }

            var baseDescription =  base.GetTooltipDescription();
            return tooltip + baseDescription + requirements;
        }
    }
}
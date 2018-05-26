using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class Talent
    {
        public string Name ;
        public int CurrentRank ;
        public int MaxRank ;
        public bool AllClasses ;
        public List<string> ClassIDs ;
        public bool Learnt ;
        public ImageContainer Image;

        public bool AutomaticallyUnlockAtLevel;
        public int LevelToAutomaticallyUnlock;

        public bool OnlyRequireOneTalent;   
        public List<StringField> RequiredTalents;
 
        //Should have the same COUNT as MaxRank
        public List<TalentEffect> talentEffects;
        public string ID;

        //SKILL DESCRIPTION
        [JsonIgnore]
        public string Description
        {
            get { return TalentEffect.Description; }
            set { TalentEffect.Description = value; }
        }

        [JsonIgnore]
        public string DescriptionFormatted
        {
            get { return GetFormattedTalentDescription(); }
        }

        [JsonIgnore]
        public bool IsMaxxedOut
        {
            get
            {
                return Learnt && CurrentRank == MaxRank - 1;
            }
        }

        [JsonIgnore]
        public TalentEffect TalentEffect
        {
            get { return talentEffects[CurrentRank]; }
        }


        [JsonIgnore]
        public int ReqTraitLevelToLevel { get { return TalentEffect.ReqTraitLevelToLevel; } }

        [JsonIgnore]
        public int SkillPointsToLevel { get { return TalentEffect.SkillPointsToLevel; } }

        [JsonIgnore]
        public int LevelRequiredToLevel { get { return TalentEffect.LevelReqToLevel; } }


        public SkillUpgradeType UpgradeType;
        public string TraitIDToLevel;


        public bool CanToggle;

        public bool InTalentGroup;
        public string TalentGroupID;

        public bool IsActive;
        public string PreviousTalentID;

        public bool RequirementsMet(PlayerCharacter player)
        {
            var firstOrDefault = player.TalentHandler.Talents.FirstOrDefault(t => t.ID == PreviousTalentID);
            if (firstOrDefault == null) return false;

            var hasPreviousSkill = firstOrDefault.Learnt;
            return hasPreviousSkill;
        }

        public Talent()
        {
            ID = Guid.NewGuid().ToString();
            Name = "New Talent";
            CurrentRank = 0;
            MaxRank = 1;
            Image = new ImageContainer();
            AllClasses = true;
            ClassIDs = new List<string>();
            RequiredTalents = new List<StringField>();
            talentEffects = new List<TalentEffect>();
            UpgradeType = SkillUpgradeType.SkillPoints;
            TraitIDToLevel = "";


            AutomaticallyUnlockAtLevel = false;
            LevelToAutomaticallyUnlock = 1;
        }

        public bool CanUnlock (PlayerCharacter player)
        {
            if (Learnt) return false;

            if (!string.IsNullOrEmpty(PreviousTalentID))
            {
                var firstOrDefault = player.TalentHandler.Talents.FirstOrDefault(s => s.ID == PreviousTalentID);
                var hasPreviousTalent = firstOrDefault != null && firstOrDefault.Learnt;
                if (!hasPreviousTalent) return false;
            }

            if (UpgradeType == SkillUpgradeType.SkillPoints && player.CurrentSkillPoints < SkillPointsToLevel)
            {
                return false;
            }

            if (UpgradeType == SkillUpgradeType.PlayerLevel && player.Level < LevelRequiredToLevel)
            {
                return false;
            }

            if (UpgradeType == SkillUpgradeType.TraitLevel && player.GetTraitByID(TraitIDToLevel).Level < ReqTraitLevelToLevel)
            {
                return false;
            }

            return true;
        }

        public bool CanUpgrade(PlayerCharacter player)
        {
            if (CurrentRank >= MaxRank - 1) return false;
            if (!Learnt) return false;

            if (UpgradeType == SkillUpgradeType.SkillPoints)
            {
                return player.CurrentSkillPoints >= SkillPointsToLevel;
            }
            else if (UpgradeType == SkillUpgradeType.PlayerLevel)
            {
                return player.Level >= LevelRequiredToLevel;
            }

            else if (UpgradeType == SkillUpgradeType.TraitLevel)
            {
                return player.GetTraitByID(TraitIDToLevel).Level >= ReqTraitLevelToLevel;
            }

            return true;
        }


        private string GetFormattedTalentDescription()
        {
            var playerSave = GetObject.PlayerSave;

            var text = Description;
            for (int i = 0; i < Rm_RPGHandler.Instance.ASVT.VitalDefinitions.Count; i++)
            {
                var attr = Rm_RPGHandler.Instance.ASVT.VitalDefinitions[i];
                text = text.Replace("{Vital_" + attr.Name + "_Current" + "}", playerSave == null ? "VAL" : playerSave.Character.GetVital(attr.Name).CurrentValue.ToString());
                text = text.Replace("{Vital_" + attr.Name + "_Max" + "}", playerSave == null ? "VAL" : playerSave.Character.GetVital(attr.Name).MaxValue.ToString());
                text = text.Replace("{Vital_" + attr.Name + "_Base" + "}", playerSave == null ? "VAL" : playerSave.Character.GetVital(attr.Name).BaseValue.ToString());
                text = text.Replace("{Vital_" + attr.Name + "_Skill" + "}", playerSave == null ? "VAL" : playerSave.Character.GetVital(attr.Name).SkillValue.ToString());
                text = text.Replace("{Vital_" + attr.Name + "_Equip" + "}", playerSave == null ? "VAL" : playerSave.Character.GetVital(attr.Name).EquipValue.ToString());
                text = text.Replace("{Vital_" + attr.Name + "_Attr" + "}", playerSave == null ? "VAL" : playerSave.Character.GetVital(attr.Name).AttributeValue.ToString());
            }

            for (int i = 0; i < Rm_RPGHandler.Instance.ASVT.AttributesDefinitions.Count; i++)
            {
                var attr = Rm_RPGHandler.Instance.ASVT.AttributesDefinitions[i];
                text = text.Replace("{Attr_" + attr.Name + "}", playerSave == null ? "VAL" : playerSave.Character.GetAttribute(attr.Name).TotalValue.ToString());
                text = text.Replace("{Attr_" + attr.Name + "_Base" + "}", playerSave == null ? "VAL" : playerSave.Character.GetAttribute(attr.Name).BaseValue.ToString());
                text = text.Replace("{Attr_" + attr.Name + "_Skill" + "}", playerSave == null ? "VAL" : playerSave.Character.GetAttribute(attr.Name).SkillValue.ToString());
                text = text.Replace("{Attr_" + attr.Name + "_Equip" + "}", playerSave == null ? "VAL" : playerSave.Character.GetAttribute(attr.Name).EquipValue.ToString());
            }

            for (int i = 0; i < Rm_RPGHandler.Instance.ASVT.StatisticDefinitions.Count; i++)
            {
                var stat = Rm_RPGHandler.Instance.ASVT.StatisticDefinitions[i];
                text = text.Replace("{Stat_" + stat.Name + "}", playerSave == null ? "VAL" : playerSave.Character.GetStat(stat.Name).TotalValue.ToString());
                text = text.Replace("{Stat_" + stat.Name + "_Base" + "}", playerSave == null ? "VAL" : playerSave.Character.GetStat(stat.Name).BaseValue.ToString());
                text = text.Replace("{Stat_" + stat.Name + "_Skill" + "}", playerSave == null ? "VAL" : playerSave.Character.GetStat(stat.Name).SkillValue.ToString());
                text = text.Replace("{Stat_" + stat.Name + "_Equip" + "}", playerSave == null ? "VAL" : playerSave.Character.GetStat(stat.Name).EquipValue.ToString());
                text = text.Replace("{Stat_" + stat.Name + "_Attr" + "}", playerSave == null ? "VAL" : playerSave.Character.GetStat(stat.Name).AttributeValue.ToString());
            }

            for (int i = 0; i < Rm_RPGHandler.Instance.ASVT.TraitDefinitions.Count; i++)
            {
                var trait = Rm_RPGHandler.Instance.ASVT.TraitDefinitions[i];
                text = text.Replace("{Trait_" + trait.Name + "}", playerSave == null ? "VAL" : playerSave.Character.GetTrait(trait.Name).Level.ToString());
            }

            return text;
        }

        public void UnlockForFree(PlayerCharacter playerCharacter)
        {
            Learnt = true;
        }

        public void Unlock(PlayerCharacter playerCharacter)
        {
            Learnt = true;
            if (UpgradeType == SkillUpgradeType.SkillPoints)
            {
                playerCharacter.AddSkillPoints(-SkillPointsToLevel);
            }
        }

        public void Upgrade(PlayerCharacter playerCharacter)
        {
            if (UpgradeType == SkillUpgradeType.SkillPoints)
            {
                playerCharacter.AddSkillPoints(-SkillPointsToLevel);
            }

            CurrentRank++;
            if (CurrentRank > MaxRank)
            {
                CurrentRank = MaxRank;
            }
        }


        public override string ToString()
        {
            return Name;
        }
    }
}
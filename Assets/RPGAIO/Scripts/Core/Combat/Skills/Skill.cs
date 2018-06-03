using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Beta;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    //TODO: Skillcaster should handle cast ranges/projectile speeds (variables) etc -> can be improved by talents
    public class Skill
    {
        //All Skills
        [JsonIgnore]
        public BaseCharacterMono CasterMono;
        [JsonIgnore] 
        public BaseCharacter Caster;

        public string Name ;
        public int CurrentRank ;
        public int MaxRank ;
        public float MinCastRange ;
        public float CastRange ;

        public bool AutomaticallyUnlockAtLevel;
        public int LevelToAutomaticallyUnlock;

        public bool EnemyOnlySkill;

        //Skill Stats
        public bool HasSkillMeta;
        public string SkillMetaID;
        public bool Learnt ; //not used, can just use unlocked
        public bool Unlocked; //Unlocked means cannot learn.
        public bool AllClasses ;
        public List<string> ClassIDs ;
        public SkillType SkillType ;
        public string PreviousSkillID ;

        public ImageContainer Image;
        public AudioContainer CastingSound;
        public AudioContainer Sound;
        public AudioContainer ImpactSound;

        public List<SkillStatistics> SkillStatistics;   
        public bool OnlyRequireOneSkill;
        public List<StringField> RequiredSkills;
        public bool AppliesBuffDebuff;

        public bool IsMaxxedOut
        {
            get {
                return Unlocked && CurrentRank == MaxRank - 1;
            }
        }

        //SKILL DESCRIPTION
        [JsonIgnore]
        public string Description
        {
            get { return SkillStatistics[CurrentRank].Description; }
            set { SkillStatistics[CurrentRank].Description = value; }
        }

        [JsonIgnore]
        public string DescriptionFormatted
        {
            get { return GetFormattedSkillDescription(); }
        }

        //All
        [JsonIgnore]
        public float CoolDownTime { get { return SkillStatistics[CurrentRank].CoolDownTime; } }

        [JsonIgnore]
        public TimedPassiveEffect Effect { get { return SkillStatistics[CurrentRank].Effect; } }

        [JsonIgnore]
        public float TotalCastTime { get { return SkillStatistics[CurrentRank].TotalCastTime; } }

        [JsonIgnore]
        public float CastingTime { get { return SkillStatistics[CurrentRank].CastingTime; } }

        //Damaging Skills

        [JsonIgnore]
        public Damage Damage { 
            get
            {
                if(string.IsNullOrEmpty(DamageScalingTreeID))
                    return SkillStatistics[CurrentRank].Damage;

                var damageCopy = GeneralMethods.CopyObject(SkillStatistics[CurrentRank].Damage);
                var damage = CombatCalcEvaluator.EvaluateSkillDamage(DamageScalingTreeID, damageCopy, Caster);
                return damage;
            } 
        }

        public string DamageScalingTreeID;

        //Spawn type only:
        [JsonIgnore]
        public float SpawnForTime { get { return SkillStatistics[CurrentRank].SpawnForTime; } }
        [JsonIgnore]
        public bool LimitSpawnInstances { get { return SkillStatistics[CurrentRank].LimitSpawnInstances; } }
        [JsonIgnore]
        public int MaxInstances { get { return SkillStatistics[CurrentRank].MaxInstances; } }

        [JsonIgnore]
        public bool HasDuration { get { return SkillStatistics[CurrentRank].HasDuration; } }

        //To Self
        public bool UseResourceOnCast ;
        public string ResourceIDUsed ;
        [JsonIgnore]
        public int ResourceRequirement { get { return SkillStatistics[CurrentRank].ResourceRequirement; } }
        public bool AddResourceOnCast ;
        public string ResourceAddedID ;

        [JsonIgnore]
        public int ResourceAddedAmount { get { return SkillStatistics[CurrentRank].ResourceAddedAmount; } }
        public bool UseEventOnCast ;
        public string EventOnCastID ;

        //On Hit/CastedOn Target

        [JsonIgnore]
        public int BonusTaunt { get { return SkillStatistics[CurrentRank].BonusTaunt; } }

        [JsonIgnore]
        public bool ApplyStatusEffect { get { return SkillStatistics[CurrentRank].ApplyStatusEffect; } }

        [JsonIgnore]
        public float ChanceToApplyStatusEffect { get { return SkillStatistics[CurrentRank].ChanceToApplyStatusEffect; } }

        [JsonIgnore]
        public bool ApplyStatusEffectWithDuration { get { return SkillStatistics[CurrentRank].ApplyStatusEffectWithDuration; } }

        [JsonIgnore]
        public float ApplyStatusEffectDuration { get { return SkillStatistics[CurrentRank].ApplyStatusEffectDuration; } }

        [JsonIgnore]
        public string StatusEffectID { get { return SkillStatistics[CurrentRank].StatusEffectID; } }

        [JsonIgnore]
        public bool HasProcEffect { get { return SkillStatistics[CurrentRank].HasProcEffect; } }

        [JsonIgnore]
        public Rm_ProcEffect ProcEffect { get { return SkillStatistics[CurrentRank].ProcEffect; } }

        [JsonIgnore]
        public bool RemoveStatusEffect { get { return SkillStatistics[CurrentRank].RemoveStatusEffect; } }

        [JsonIgnore]
        public float ChanceToRemoveStatusEffect { get { return SkillStatistics[CurrentRank].ChanceToRemoveStatusEffect; } }

        [JsonIgnore]
        public string RemoveStatusEffectID { get { return SkillStatistics[CurrentRank].RemoveStatusEffectID; } }


        [JsonIgnore]
        public bool ApplyDOTOnHit { get { return SkillStatistics[CurrentRank].ApplyDOTOnHit; } }
        [JsonIgnore]
        public float ChanceToApplyDOT { get { return SkillStatistics[CurrentRank].ChanceToApplyDOT; } }
        [JsonIgnore]
        public DamageOverTime DamageOverTime { get { return SkillStatistics[CurrentRank].DamageOverTime; } }

        [JsonIgnore]
        public bool RunEventOnHit { get { return SkillStatistics[CurrentRank].RunEventOnHit; } }
        [JsonIgnore]
        public string EventOnHitID { get { return SkillStatistics[CurrentRank].EventOnHitID; } }

        [JsonIgnore]
        public bool GivePlayerItem { get { return SkillStatistics[CurrentRank].GivePlayerItem; } }

        [JsonIgnore]
        public string ItemToGiveID { get { return SkillStatistics[CurrentRank].ItemToGiveID; } }

        [JsonIgnore]
        public int ItemToGiveAmount { get { return SkillStatistics[CurrentRank].ItemToGiveAmount; } }

        public TargetType TargetType ;

        //Unlock/Upgrade Conditions
        public SkillUpgradeType UpgradeType ;
        public string TraitIDToLevel;

        [JsonIgnore]
        public int ReqTraitLevelToLevel { get { return SkillStatistics[CurrentRank].ReqTraitLevelToLevel; } }

        [JsonIgnore]
        public int SkillPointsToLevel { get { return SkillStatistics[CurrentRank].SkillPointsToLevel; } }

        [JsonIgnore]
        public int LevelRequiredToLevel { get { return SkillStatistics[CurrentRank].LevelReqToLevel; } }

        //Prefabs
        public string CastPrefabPath;
        public string CastingPrefabPath;
        public string LandPrefab;
        public string MovingToPrefab;
        public string PrefabPath;
        public string ImpactPrefabPath;

        //Animations
        public List<SkillAnimationDefinition> AnimationsToUse ;

        //Usage Tracking
        public float CurrentCoolDownTime;
        public int TimesUsed;

        //Can Cast Requirements
        public bool RequireVitalBelowX = false;
        public bool RequireVitalAboveX = false;
        public string RequiredVitalId;
        [JsonIgnore]
        public float VitalConditionParameter { get { return SkillStatistics[CurrentRank].VitalConditionParamater; } }

        public bool OffCooldown
        {
            get { return CurrentCoolDownTime <= 0; }
        }

        public bool RequireEquippedWep = false;
        public string RequiredEquippedWepTypeID = "";

        //melee/ability/restoration/aoe
        public bool AlwaysRequireTarget;
        public SkillMovementType MovementType;
        public float MoveToSpeed;
        public float JumpToHeight;
        public float LandTime;

        //Combo Skills
        public float MaxComboTime;

        [JsonIgnore]
        public bool Targetable
        {
            get
            {
                return new[] {SkillType.Aura, SkillType.Ability, SkillType.Restoration, SkillType.Projectile}.Any(s => s == SkillType);
            }
        }

        public bool CanCastSkill(bool isPlayer)
        {
            if (!Unlocked) return false;
            if (!OffCooldown) return false;

            if (UseResourceOnCast && isPlayer)
            {
                var playerResource = GetObject.PlayerCharacter.GetVitalByID(ResourceIDUsed);
                if(playerResource.CurrentValue - ResourceRequirement < 0)
                {
                    RPG.Popup.ShowInfo("Not enough " + RPG.Stats.GetVitalName(playerResource.ID));
                    return false;
                }
            }
            
            return true;
        }

        public bool ReqPrevSkillInCombo;
        public float CurrentComboTime;
        public string PrevSkillForComboID;

        public string ID;

        public Skill()
        {
            ID = Guid.NewGuid().ToString();
            Name = "New Skill";
            AllClasses = true;
            Unlocked = true;
            CurrentRank = 0;
            MaxRank = 1;
            AnimationsToUse = new List<SkillAnimationDefinition>();
            SkillStatistics = new List<SkillStatistics>();
            MinCastRange = 0.0f;
            CastRange = 5.0f;
            LandTime = 0.1F;
            UseResourceOnCast = true;
            MovementType = SkillMovementType.StayInPlace;
            MoveToSpeed = 25f;
            JumpToHeight = 5.0f;
            ClassIDs = new List<string>();
            SkillMetaID = "";
            UpgradeType = Rm_RPGHandler.Instance.Combat.DefaultUpgradeType;
            Image = new ImageContainer();
            CastingSound = new AudioContainer();
            Sound = new AudioContainer();
            ImpactSound = new AudioContainer();
            RequiredSkills = new List<StringField>();

            AutomaticallyUnlockAtLevel = false;
            LevelToAutomaticallyUnlock = 1;
        }


        private string GetFormattedSkillDescription()
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


        public bool CanUnlock(PlayerCharacter player)
        {
            if (Unlocked) return false;

            if(!string.IsNullOrEmpty(PreviousSkillID))
            {
                var firstOrDefault = player.SkillHandler.AvailableSkills.FirstOrDefault(s => s.ID == PreviousSkillID);
                var hasPreviousSkill = firstOrDefault != null && firstOrDefault.Unlocked;
                if (!hasPreviousSkill) return false;
            }
            
            if(UpgradeType == SkillUpgradeType.SkillPoints && player.CurrentSkillPoints < SkillPointsToLevel)
            {
                return false;
            }

            if(UpgradeType == SkillUpgradeType.PlayerLevel && player.Level < LevelRequiredToLevel)
            {
                return false;
            }

            if(UpgradeType == SkillUpgradeType.TraitLevel && player.GetTraitByID(TraitIDToLevel).Level < ReqTraitLevelToLevel)
            {
                return false;
            }

            return true;
        }

        public bool CanUpgrade(PlayerCharacter player)
        {
            if (!Unlocked) return false;
            if (CurrentRank >= MaxRank - 1) return false;

            if(UpgradeType == SkillUpgradeType.SkillPoints)
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

        public bool CanCast(PlayerCharacter player)
        {
            var canCast = true;
         

            if (RequireVitalBelowX)
            {
                canCast = player.GetVitalByID(RequiredVitalId).CurrentValue <=
                          player.GetVitalByID(RequiredVitalId).MaxValue * VitalConditionParameter;
            }
            if (canCast == false) return false;

            if (ReqPrevSkillInCombo)
            {
                var stillExists =
                    Rm_RPGHandler.Instance.Repositories.Skills.AllSkills.FirstOrDefault(s => s.ID == PrevSkillForComboID);

                if(stillExists != null)
                {
                    canCast = stillExists.CurrentComboTime < MaxComboTime;
                }

            }
            if (canCast == false) return false;

            if(RequireVitalAboveX)
            {
                canCast = player.GetVitalByID(RequiredVitalId).CurrentValue >=
                          player.GetVitalByID(RequiredVitalId).MaxValue * VitalConditionParameter;
            }
            if (canCast == false) return false;

            if (!string.IsNullOrEmpty(RequiredEquippedWepTypeID))
            {
                var weapon = player.Equipment.GetSlot("Weapon").Item as Weapon;
                if (weapon != null)
                    canCast = weapon.WeaponTypeID == RequiredEquippedWepTypeID;
            }
            if (canCast == false) return false;

            if(ResourceRequirement > 0 && UseResourceOnCast)
            {
                canCast = player.GetVitalByID(ResourceIDUsed).CurrentValue - ResourceRequirement >= 0;
                
                if (player.GetVitalByID(ResourceIDUsed).IsHealth)
                    canCast = player.GetVitalByID(ResourceIDUsed).CurrentValue - ResourceRequirement >= 1;
            }
            if (canCast == false) return false;

            return canCast;
        }

        public SkillAnimationDefinition LegacyAnimationToUse(string classID)
        {
            var skillAnimationDefinition = AnimationsToUse.FirstOrDefault(a => a.ClassID == classID);
            if (skillAnimationDefinition != null)
                return skillAnimationDefinition;
            
            return null;
        }

        public override string ToString()
        {
            return Name;
        }

        public void UnlockForFree(PlayerCharacter playerCharacter)
        {
            Unlocked = true;
        }

        public void Unlock(PlayerCharacter playerCharacter)
        {
            Unlocked = true;
            if(UpgradeType == SkillUpgradeType.SkillPoints)
            {
                playerCharacter.AddSkillPoints(-SkillPointsToLevel);
            }
        }

        public void Upgrade(PlayerCharacter playerCharacter)    
        {
            if(UpgradeType == SkillUpgradeType.SkillPoints)
            {
                playerCharacter.AddSkillPoints(-SkillPointsToLevel);
            }

            CurrentRank++;
            if (CurrentRank > MaxRank)
            {
                CurrentRank = MaxRank;
            }
        }
    }

    public enum SkillUpgradeType
    {
        PlayerLevel,
        SkillPoints,
        TraitLevel
    }
}
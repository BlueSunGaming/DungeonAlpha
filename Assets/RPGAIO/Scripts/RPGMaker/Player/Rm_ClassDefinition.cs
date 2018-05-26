using System;
using System.Collections.Generic;
using LogicSpawn.RPGMaker.Core;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker
{
    public class Rm_ClassDefinition
    {
        public string ID;

        public string ApplicableRaceID;
        public string ApplicableSubRaceID;
        public string ApplicableGenderID;
        public List<StringField> ApplicableClassIDs;


        public string ClassPrefabPath;
        public string Name;
        public string Description;
        public string ExpDefinitionID;

        public AttackStyle AttackStyle;
        public float UnarmedAttackRange;
        public float UnarmedAttackSpeed;
        public int UnarmedAttackDamage;

        public int StartingGold;
        public string StartingPet;
        public bool HasStartingPet;

        public bool StartAtWorldLocation;
        public string StartingScene;
        public string StartingWorldArea;
        public string StartingLocation;

        public string AutoAttackPrefabPath;
        public string AutoAttackImpactPrefabPath;
        public AudioContainer ProjectileTravelSound;
        public AudioContainer AutoAttackImpactSound;
        public float ProjectileSpeed;

        public string ImagePath;
        [JsonIgnore]
        public Texture2D _image ;
        [JsonIgnore]
        public Texture2D Image
        {
            get { return _image ?? (_image = Resources.Load(ImagePath) as Texture2D); }
            set { _image = value; }
        }
        public RPGAnimationType AnimationType;
        public LegacyAnimation LegacyAnimations;

        public List<Rm_AsvtAmount> StartingAttributes;
        public List<Rm_AsvtAmountFloat> StartingStats;
        public List<Rm_AsvtAmount> StartingVitals;
        public List<Rm_AsvtAmount> StartingTraitLevels;

        public List<Rm_AsvtAmount> AttributePerLevel;


        public List<string> SkillMetaImmunitiesID ;
        public List<SkillMetaSusceptibility> SkillMetaSusceptibilities ;

        public List<string> StartingSkillIds;
        public List<string> StartingTalentIds;
        public List<LootDefinition> StartingItems;
        public StartEquipDefinition StartingEquippedWeapon;
        public List<StartEquipDefinition> StartingEquipped;

        public bool HasStartingQuest;
        public string StartingQuestID;

        //Costume Designer
        public EquipmentInfo EquipmentInfo;

        //Character Customisation
        public List<VisualCustomisation> VisualCustomisations;


        public Rm_ClassDefinition()
        {
            ID = Guid.NewGuid().ToString();

            ApplicableRaceID = "";
            ApplicableSubRaceID = "";
            ApplicableGenderID = "";
            ApplicableClassIDs = new List<StringField>();

            ExpDefinitionID = Rmh_Experience.PlayerExpDefinitionID;
            Name = "Unnamed Character";
            Description = "A strong combatant.";
            ClassPrefabPath = "";
            UnarmedAttackRange = 1.5f;
            UnarmedAttackSpeed = 2.0f;
            UnarmedAttackDamage = 1;
            AutoAttackPrefabPath = "";
            AttackStyle = AttackStyle.Melee;
            StartingScene = "";
            StartingGold = 0;
            ImagePath = "";

            AnimationType = RPGAnimationType.Legacy;
            LegacyAnimations = new LegacyAnimation();

            ProjectileTravelSound = new AudioContainer();
            AutoAttackImpactSound = new AudioContainer();
            ProjectileSpeed = 10f;

            HasStartingPet = false;
            StartingPet = "";

            StartingAttributes = new List<Rm_AsvtAmount>();
            StartingStats = new List<Rm_AsvtAmountFloat>();
            StartingVitals = new List<Rm_AsvtAmount>();
            StartingTraitLevels = new List<Rm_AsvtAmount>();
            StartingItems = new List<LootDefinition>();
            StartingEquippedWeapon = new StartEquipDefinition();
            StartingEquipped = new List<StartEquipDefinition>();

            AttributePerLevel = new List<Rm_AsvtAmount>();
            StartingSkillIds = new List<string>();
            StartingTalentIds = new List<string>();
            SkillMetaImmunitiesID = new List<string>();
            SkillMetaSusceptibilities = new List<SkillMetaSusceptibility>();

            EquipmentInfo = new EquipmentInfo();
            VisualCustomisations = new List<VisualCustomisation>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
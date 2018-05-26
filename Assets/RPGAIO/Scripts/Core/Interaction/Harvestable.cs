using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class Harvestable
    {
        public bool IsQuestItem;
        public string HarvestedObjectID;
        public float TimeToHarvest;
        public float CurrentHarvestTime;
        public int MinObtainable;
        public int MaxObtainable;

        public int MaxAtOnce;

        public int MinAmountGained;
        public int MaxAmountGained;

        public string Name;
        public string ID;

        public bool RequireLevel;
        public int LevelRequired;

        public bool RequireTraitLevel;
        public string RequiredTraitID;
        public int TraitLevelRequired;

        public string HarvestingSoundPath;
        [JsonIgnore]
        public AudioClip _harvestingSound ;
        [JsonIgnore]
        public AudioClip HarvestSound
        {
            get { return _harvestingSound ?? (_harvestingSound = Resources.Load(HarvestingSoundPath) as AudioClip); }
            set { _harvestingSound = value; }
        }
        public string QuestAcceptedID;


        public ProgressionGain ProgressionGain;

        public bool RegensResources;
        public float TimeInSecToRegen;
        public int AmountRegenerated;


        //Tracking:
        public int MaterialsRemaining;
        public string PrefabPath;


        //Animation
        public List<ClassAnimationDefinition> ClassHarvestingAnims;

        public Harvestable()
        {
            ID = Guid.NewGuid().ToString();
            Name = "New Harvestable";
            QuestAcceptedID = "";
            HarvestedObjectID = null;
            TimeToHarvest = 5;
            MinObtainable = 5;
            MaxObtainable = 10;

            ClassHarvestingAnims = new List<ClassAnimationDefinition>();

            ProgressionGain = new ProgressionGain();
            ProgressionGain.GainExp = false;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
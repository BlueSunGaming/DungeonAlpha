using System;
using System.Collections.Generic;
using LogicSpawn.RPGMaker.Core;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker
{
    public class Rmh_Player
    {
        //Player
        public List<Rm_ClassNameDefinition> ClassNameDefinitions;
        public List<Rm_RaceDefinition> RaceDefinitions;
        public List<Rm_SubRaceDefinition> SubRaceDefinitions;
        public List<StringDefinition> GenderDefinitions;
        public List<Rm_MetaDataDefinition> MetaDataDefinitions;

        //Character Creation
        public bool SkipCharacterCreation;
        public bool SkipRaceSelection;
        public bool SkipSubRaceSelection;
        public bool RemoveSubRaceDescription;
        public bool CustomisationsEnabled;
        public DescriptionShown ShownDescription;

        [JsonProperty("ClassDefinitions")]
        public List<Rm_ClassDefinition> CharacterDefinitions;
        public List<DifficultyDefinition> Difficulties;
        public string DefaultDifficultyID;
        public bool UseCustomExperienceFormula;
        //Pets
        public List<Rm_PetDefinition> PetDefinitions;


        public bool ManualAssignAttrPerLevel;
        public int AttributePointsEarnedPerLevel;

        public bool GiveSkillPointsPerLevel;
        public int SkillPointsEarnedPerLevel;

        public AudioContainer LevelUpSound;


        public Rmh_Player()
        {
            ClassNameDefinitions = new List<Rm_ClassNameDefinition>()
                                       {
                                           new Rm_ClassNameDefinition()
                                               {ID = "Default_Builtin_Class_10101010", Name = "Class"}
                                       };

            var raceDef = new Rm_RaceDefinition() {ID = "Default_Builtin_Race_10101010", Name = "Race"};
            RaceDefinitions = new List<Rm_RaceDefinition>()
                                       {
                                           raceDef
                                       };

            SubRaceDefinitions = new List<Rm_SubRaceDefinition>()
                                       {
                                           new Rm_SubRaceDefinition()
                                               {ID = "Default_Builtin_SubRace_10101010", Name = "Sub-Race", ApplicableRaceID = "Default_Builtin_Race_10101010"}
                                       };

            var builtInGender = new StringDefinition() {ID = "Default_Builtin_Gender_10101010", Name = "Male"};
            GenderDefinitions = new List<StringDefinition>()
                                    {
                                        builtInGender,
                                        new StringDefinition() {ID = Guid.NewGuid().ToString(), Name = "Female"}
                                    };

            MetaDataDefinitions = new List<Rm_MetaDataDefinition>();

            SkipCharacterCreation = false;
            SkipRaceSelection = false;
            SkipSubRaceSelection = false;
            RemoveSubRaceDescription = true;
            CustomisationsEnabled = true;

            ShownDescription = DescriptionShown.CharacterDescription;

            CharacterDefinitions = new List<Rm_ClassDefinition>();
            PetDefinitions = new List<Rm_PetDefinition>();
            LevelUpSound = new AudioContainer();
            ManualAssignAttrPerLevel = false;
            AttributePointsEarnedPerLevel = 5;
            GiveSkillPointsPerLevel = false;
            SkillPointsEarnedPerLevel = 100;

            Difficulties = new List<DifficultyDefinition>()
                               {
                                   new DifficultyDefinition("Easy", 0.5f),
                                   new DifficultyDefinition("Normal", 1.0f),
                                   new DifficultyDefinition("Hard", 2.0f),
                                   new DifficultyDefinition("Expert", 3.0f)
                               };

            DefaultDifficultyID = Difficulties[1].ID;   
        }
    }

    public enum DescriptionShown
    {
        ClassDescription,
        CharacterDescription,
        Both,
        None
    }
}
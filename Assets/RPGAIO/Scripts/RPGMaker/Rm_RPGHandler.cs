using System.Collections.Generic;
using LogicSpawn.RPGMaker.Objectives;
using Newtonsoft.Json;
using UnityEngine;

namespace  LogicSpawn.RPGMaker
{
    [ExecuteInEditMode]
    public class Rm_RPGHandler
    {
        //Modules

        public string Version;

        public Rmh_GameInfo GameInfo;
        public Rmh_Audio Audio;
        public Rmh_ASVT ASVT; //Attr, Stats, Vitals, Traits
        public Rmh_Item Items; //Items, Crafting
        public Rmh_Player Player; //Player
        public Rmh_Experience Experience;
        public Rmh_Enemy Enemy; //Enemies
        public Rmh_Questing Questing;
        public Rmh_Customise Customise; //Options
        public Rmh_Interactables Interactables; //Options
        public Rmh_GUI GUI; //Options
        public Rmh_Combat Combat; //Skills
        public Rmh_Harvesting Harvesting; //Harvesting
        public Rmh_Repositories Repositories; //Repositories and databases
        public Rmh_Editor Editor;
        public Rmh_Nodes Nodes;
        public Rmh_DefinedVariables DefinedVariables;
        public Rmh_DefaultSettings DefaultSettings;
        public Rmh_Preferences Preferences;

        //Instance
        [JsonIgnore] public static Rm_RPGHandler Instance;

        public Rm_RPGHandler()
        {
            Instance = this;

            Version = "";

            GameInfo = new Rmh_GameInfo();
            Audio = new Rmh_Audio();
            ASVT = new Rmh_ASVT();
            Items = new Rmh_Item();
            Player = new Rmh_Player();
            Experience = new Rmh_Experience();
            Enemy = new Rmh_Enemy();
            Questing = new Rmh_Questing();
            Customise = new Rmh_Customise();
            Interactables = new Rmh_Interactables();
            GUI= new Rmh_GUI();
            Combat = new Rmh_Combat();
            Harvesting = new Rmh_Harvesting();
            Repositories = new Rmh_Repositories();
            Editor = new Rmh_Editor();
            Nodes = new Rmh_Nodes();
            DefinedVariables = new Rmh_DefinedVariables();
            DefaultSettings = new Rmh_DefaultSettings();
            Preferences = new Rmh_Preferences();
        }
    }
}

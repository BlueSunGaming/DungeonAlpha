using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker
{
    public class ReputationDefinition
    {
        public string ID;
        public string Name;
        public int StartingValue;

        public int ValueLossForNPCAttack;
        public bool AttackIfBelowReputation;
        public int BelowReputationValue;

        public bool IsTrackable;

        public string ImagePath;
        [JsonIgnore]
        public Texture2D _image ;
        [JsonIgnore]
        public Texture2D Image
        {
            get { return _image ?? (_image = Resources.Load(ImagePath) as Texture2D); }
            set { _image = value; }
        }

        public List<FactionStatus> EnemyFactions;
        //public List<FactionStatus> AlliedFactions;
        public List<FactionRank> Ranks;
        public bool IsDefault;

        public ReputationDefinition()
        {
            Name = "New Reputation";
            ID = Guid.NewGuid().ToString();
            EnemyFactions = new List<FactionStatus>();
            //AlliedFactions = new List<FactionStatus>();
            Ranks = new List<FactionRank>()
                        {
                            new FactionRank() {Name = "Neutral", Requirement = 0},
                            new FactionRank() {Name = "Friendly", Requirement = 250},
                            new FactionRank() {Name = "Respected", Requirement = 1000}
                        };
            IsTrackable = true;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class FactionRank
    {
        public string ID;
        public string Name;
        public int Requirement;

        public FactionRank()
        {
            ID = Guid.NewGuid().ToString();
            Name = "New Faction Rank";
            Requirement = 0;
        }
    }

    public class FactionStatus
    {
        public string ID;
        public bool IsTrue;
    }
}
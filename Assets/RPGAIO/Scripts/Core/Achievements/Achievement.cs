using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class Achievement
    {
        public string ID;
        public string Name ;
        public ImageContainer ImageContainer;
        
        public string Description ;
        public int Score ;

        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime DateAchieved ;

        public bool HasProgress ;
        public AchievementProgress Progress ;
        public bool IsAchieved ;

        public Achievement(string id, string name, string imagePath, string desc, int score)
        {
            ImageContainer = new ImageContainer();
            
            ID = id;
            Name = name;
            ImageContainer.ImagePath = imagePath;
            Progress = new AchievementProgress();
            Description = desc;
            Score = score;
            IsAchieved = false;
            DateAchieved = new DateTime();
        }
    }
}
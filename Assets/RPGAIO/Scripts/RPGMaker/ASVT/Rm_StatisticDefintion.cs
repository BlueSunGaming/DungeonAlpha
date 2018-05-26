using System;
using LogicSpawn.RPGMaker.Core;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker
{
    public class Rm_StatisticDefintion
    {
        public string Name = "Unnamed Statistic";
        public string Description = "";
        public StatisticType StatisticType;
        public float DefaultValue = 1.00f;
        public string ID;
        public Rm_UnityColors Color = Rm_UnityColors.None;

        public bool IsPercentageInUI;
        public bool HasMaxValue;
        public float MaxValue;
        public bool IsDefault;

        [JsonIgnore]
        public Texture2D _image ;
        [JsonIgnore]
        public Texture2D Image
        {
            get { return _image ?? (_image = Resources.Load(ImagePath) as Texture2D); }
            set { _image = value; }
        }
        public string ImagePath;

        public Rm_StatisticDefintion()
        {
            ID = Guid.NewGuid().ToString();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
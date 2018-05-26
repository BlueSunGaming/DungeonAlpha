using System;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker
{
    public class Rm_TraitDefintion
    {
        public string ID;
        public string Name;
        public string Description;
        public string ExpDefinitionID;
        public int StartingLevel;
        public Rm_UnityColors Color = Rm_UnityColors.None;

        [JsonIgnore]
        public Texture2D _image ;
        [JsonIgnore]
        public Texture2D Image
        {
            get { return _image ?? (_image = Resources.Load(ImagePath) as Texture2D); }
            set { _image = value; }
        }
        public string ImagePath;

        public Rm_TraitDefintion()
        {
            ID = Guid.NewGuid().ToString();
            Name = "New Trait";
            StartingLevel = 1;
            Description = "";
            ExpDefinitionID = Rmh_Experience.TraitExpDefinitionID;

        }

        public override string ToString()
        {
            return Name;
        }
    }
}
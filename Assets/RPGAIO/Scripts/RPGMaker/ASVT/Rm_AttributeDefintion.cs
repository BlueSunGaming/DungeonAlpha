using System;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker
{
    public class Rm_AttributeDefintion
    {
        public string Name = "Unnamed Attribute";
        public string Description = "";
        public int DefaultValue = 1;
        public string ID;
        public Rm_UnityColors Color = Rm_UnityColors.None;

        public bool HasMaxValue;
        public int MaxValue;

        [JsonIgnore]
        public Texture2D _image ;
        [JsonIgnore]
        public Texture2D Image
        {
            get { return _image ?? (_image = Resources.Load(ImagePath) as Texture2D); }
            set { _image = value; }
        }
        public string ImagePath;

        public Rm_AttributeDefintion()
        {
            ID = Guid.NewGuid().ToString();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
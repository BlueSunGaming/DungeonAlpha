using System;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class Interactable
    {
        public string Name;
        public string ConversationNodeId;
        public string ID;
        [JsonIgnore]
        private Texture2D _image ;
        [JsonIgnore]
        public Texture2D Image
        {
            get { return _image ?? (_image = Resources.Load(ImagePath) as Texture2D); }
            set { _image = value; }
        }
        public string ImagePath;
        public string PrefabPath;

        public Interactable()
        {
            ID = Guid.NewGuid().ToString();
            Name = "New Interactable";
            ConversationNodeId = "";
            PrefabPath = "";
            ImagePath = "";
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class ImageContainer
    {
        public string ImagePath;
        [JsonIgnore]
        public Texture2D _image ;
        [JsonIgnore]
        public Texture2D Image
        {
            get { return _image ?? (_image = Resources.Load(ImagePath) as Texture2D); }
            set { _image = value; }
        }

        public ImageContainer()
        {
            ImagePath = "";
        }
    }    
    
    public class SpriteContainer
    {
        public string ImagePath;
        [JsonIgnore]
        public Sprite _image ;
        [JsonIgnore]
        public Sprite Image
        {
            get { return _image ?? (_image = Resources.Load(ImagePath) as Sprite); }
            set { _image = value; }
        }

        public SpriteContainer()
        {
            ImagePath = "";
        }
    }
}
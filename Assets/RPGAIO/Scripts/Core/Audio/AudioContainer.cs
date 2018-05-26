using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class AudioContainer
    {
        public string AudioPath;
        [JsonIgnore]
        public AudioClip _audio ;
        [JsonIgnore]
        public AudioClip Audio
        {
            get { return _audio ?? (_audio = Resources.Load(AudioPath) as AudioClip); }
            set { _audio = value; }
        }

        public AudioContainer()
        {
            AudioPath = "";
        }
    }
}
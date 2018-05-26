using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class CharacterResponse
    {
        public string Response ;

        public string SoundPath;
        [JsonIgnore]
        public AudioClip _sound ;
        [JsonIgnore]
        public AudioClip Sound
        {
            get { return _sound ?? (_sound = Resources.Load(SoundPath) as AudioClip); }
            set { _sound = value; }
        }

        public ActionType Action ;
        public int Parameter ;

        public CharacterResponse()
        {
            SoundPath = "";
            Response = "";
            Action = ActionType.End;
            Parameter = -1;
        }
    }
}
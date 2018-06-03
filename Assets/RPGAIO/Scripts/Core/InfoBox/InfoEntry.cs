using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class InfoEntry
    {
        public InfoEntryType Type ;
        public string Message ;
        public AudioClip Audio ;

        public InfoEntry(string message, InfoEntryType type, AudioClip audio = null)
        {
            Message = message;
            Type = type;
        }
    }
}
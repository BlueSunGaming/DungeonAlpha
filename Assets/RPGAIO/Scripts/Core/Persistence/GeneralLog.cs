using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class GeneralLog
    {
        public List<GeneralLogEntry> AllEntries ;
        private const string GeneralLogImagePath = "Icons/GeneralLog/";

        public GeneralLog()
        {
            AllEntries = new List<GeneralLogEntry>();
        }

        [JsonIgnore]
        public List<GeneralLogEntry> GetAllLogEntries
        {
            get { return AllEntries; }
        }

        public void AddEntry(GeneralLogEntry entry)
        {
            entry.ImagePath = GeneralLogImagePath + entry.EntryType;
            entry.Image = Resources.Load(entry.ImagePath) as Texture2D;
            entry.DateOfEntry = DateTime.Now;
        }
    }

    public class GeneralLogEntry
    {
        public string LogContent ;


        [JsonIgnore]
        public Texture2D _image ;

        [JsonIgnore]
        public Texture2D Image
        {
            get { return _image ?? (_image = Resources.Load(ImagePath) as Texture2D); }
            set { _image = value; }
        }
        public string ImagePath;

        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime DateOfEntry ;
        public EntryType EntryType ;

        public GeneralLogEntry()
        {
            DateOfEntry = new DateTime();
        }
    }

    public enum EntryType
    {
        LevelUp,
        RareLoot,
        NewLocation,
        BossKill,
        QuestComplete,
        Miscellaneous
    }
}
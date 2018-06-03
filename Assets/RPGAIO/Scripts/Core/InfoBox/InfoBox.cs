using System.Collections.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    public static class InfoBox
    {
        private static List<InfoEntry> Entries;
        public static List<InfoEntry> Logs
        {
            get { return Entries; }
        }

        static InfoBox()
        {
            Entries = new List<InfoEntry>();
        }

        public static void Log(string details)
        {
            Entries.Add(new InfoEntry(details, InfoEntryType.Info));
        }

        public static void LogWarning(string details)
        {
            Entries.Add(new InfoEntry(details, InfoEntryType.Warning));
        }
    }
}
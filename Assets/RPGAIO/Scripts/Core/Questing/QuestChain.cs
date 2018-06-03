using System;
using System.Collections.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    public class QuestChain
    {
        public string ID;
        public string Name;
        public List<Quest> QuestsInChain;

        public QuestChain()
        {
            ID = Guid.NewGuid().ToString();
            Name = "New Quest Chain";
            QuestsInChain = new List<Quest>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class QuestItemRepository
    {
        public List<Item> AllItems;

        public QuestItemRepository()
        {
            AllItems = new List<Item>();
        }

        public Item Get(string itemID)
        {
            return GeneralMethods.CopyObject(AllItems.FirstOrDefault(i => i.ID == itemID));
        }
    }
}
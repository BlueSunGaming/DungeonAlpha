using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class ItemRepository
    {
        public List<Item> AllItems;

        public ItemRepository()
        {
            AllItems = new List<Item>();
        }

        public Item Get(string itemID)
        {
            var exists = AllItems.FirstOrDefault(i => i.ID == itemID) != null;
            return exists ? GeneralMethods.CopyObject(AllItems.First(i => i.ID == itemID)) : null;
        }
    }
}
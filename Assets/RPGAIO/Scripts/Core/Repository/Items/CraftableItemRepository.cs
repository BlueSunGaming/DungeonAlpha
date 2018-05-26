using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class CraftableItemRepository
    {

        public List<Item> AllItems;

        public CraftableItemRepository()
        {
            AllItems = new List<Item>();
        }
        public Item Get(string itemID)
        {
            var foundItem = AllItems.FirstOrDefault(i => i.ID == itemID);
            return foundItem != null ? GeneralMethods.CopyObject(foundItem) : null;
        }
    }
}
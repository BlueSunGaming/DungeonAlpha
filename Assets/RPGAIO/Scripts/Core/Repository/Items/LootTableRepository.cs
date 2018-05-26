using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class LootTableRepository
    {
        public List<Rm_LootTable> AllTables;

        public LootTableRepository()
	    {
            AllTables = new List<Rm_LootTable>();
	    } 

        public Rm_LootTable GetLootTable(string tableID)
        {
            return AllTables.First(t => t.ID == tableID);
        }
    }

    public class Rm_LootTable
    {
        public string Name;
        public int ChanceForItem; //0-1 , if RNG > val then we choose a random loottableItem based on loottableItem %
        //loot table item %'s should add to 1 (100%)
        public List<Rm_LootTableItem> LootTableItems;
        public string ID;

        public Rm_LootTable()
        {
            ID = Guid.NewGuid().ToString();
            Name = "New Loot Table";
            ChanceForItem = 100;
            LootTableItems = new List<Rm_LootTableItem>();
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class Rm_LootTableItem
    {
        public string ItemID;
        
        public bool IsNormalItem;
        public bool IsQuestItem;
        public bool IsCraftableItem;

        public bool IsGold;
        public bool IsEmpty;
        public float Chance;
        public float sliderMin;
        public float sliderMax;
        public int MinQuantity;
        public int MaxQuantity;

        public Rm_LootTableItem()
        {
            ItemID = "";
            IsGold = false;
            IsEmpty = false;
            Chance = 0;
            MinQuantity = 1;
            MaxQuantity = 1;
        }

        public override string ToString()
        {
            Item referencedItem = null;
            if(IsNormalItem)
            {
                referencedItem = Rm_RPGHandler.Instance.Repositories.Items.Get(ItemID);
            }
            else if(IsQuestItem)
            {
                referencedItem = Rm_RPGHandler.Instance.Repositories.QuestItems.Get(ItemID);
            }
            else if (IsCraftableItem)
            {
                referencedItem = Rm_RPGHandler.Instance.Repositories.CraftableItems.Get(ItemID);
            }

            if (referencedItem != null)
                return referencedItem.Name;

            return "[Item not found!]";
        }
    }
}
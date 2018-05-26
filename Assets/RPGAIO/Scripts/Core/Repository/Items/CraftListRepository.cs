using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class CraftListRepository
    {


        public List<Rm_TierCraftList> TierCraftLists;
        public List<Rm_CustomCraftList> CustomCraftLists;

        public CraftListRepository ()
	    {
            TierCraftLists = new List<Rm_TierCraftList>()
                                 {
                                     new Rm_TierCraftList(),
                                     new Rm_TierCraftList(),
                                     new Rm_TierCraftList(),
                                     new Rm_TierCraftList(),
                                     new Rm_TierCraftList(),
                                     new Rm_TierCraftList()
                                 };
            CustomCraftLists = new List<Rm_CustomCraftList>();
	    } 

        public List<Item> GetCraftList(Item item)
        {
            if (CustomCraftLists.FirstOrDefault(c => c.ItemID == item.ID) != null)
            {
                var itemsNeededIDs = CustomCraftLists.First(c => c.ItemID == item.ID).ItemsNeededIDs;
                var listOfItem = new List<Item>();
                foreach(var itemNeeded in itemsNeededIDs)
                {
                    var craftItem = Rm_RPGHandler.Instance.Repositories.Items.Get(itemNeeded.ItemID) 
                        ?? Rm_RPGHandler.Instance.Repositories.CraftableItems.Get(itemNeeded.ItemID);
                    var stackable = craftItem as IStackable;
                    if(stackable != null)
                    {
                        stackable.CurrentStacks = itemNeeded.Quantity;
                    }
                     
                    listOfItem.Add(craftItem);
                }

                return listOfItem;
            }

            var itemNeededIds = TierCraftLists.First(t => t.TierID == Rm_RPGHandler.Instance.Items.RmTierHandler.GetTierID(item)).ItemsNeededIDs;
            var listOfItems = new List<Item>();
            foreach (var itemNeeded in itemNeededIds)
            {
                var craftItem = Rm_RPGHandler.Instance.Repositories.Items.Get(itemNeeded.ItemID)
                        ?? Rm_RPGHandler.Instance.Repositories.CraftableItems.Get(itemNeeded.ItemID);
                var stackable = craftItem as IStackable;
                if (stackable != null)
                {
                    stackable.CurrentStacks = itemNeeded.Quantity;
                }

                listOfItems.Add(craftItem);
            }

            return listOfItems; 
        }
    }

    public class Rm_TierCraftList : Rm_CraftList
    {
        public string TierID = "";

        public override string ToString()
        {
            return "Craft List for ["+ Rm_RPGHandler.Instance.Items.RmTierHandler.GetTierName(TierID) + "]";
        }
    }

    public class Rm_CraftList
    {
        public List<CraftListItem> ItemsNeededIDs;

        public Rm_CraftList()
        {
            ItemsNeededIDs = new List<CraftListItem>();
        }

    }

    public class CraftListItem
    {
        public string ItemID;
        public int Quantity;

        public CraftListItem()
        {
            Quantity = 1;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(ItemID)) return "Item";

            var item = Rm_RPGHandler.Instance.Repositories.Items.Get(ItemID);
            if(item != null)
            {
                return (Quantity + "x " + item.Name);
            }
               
            var craftItem = Rm_RPGHandler.Instance.Repositories.CraftableItems.Get(ItemID);
            if (craftItem != null)
            {
                return (Quantity + "x " + craftItem.Name);
            }    

            var questItem = Rm_RPGHandler.Instance.Repositories.QuestItems.Get(ItemID);
            if (questItem != null)
            {
                return (Quantity + "x " + questItem.Name);
            }
                
            //todo:remove
            return Quantity + "x A Item";
        }
    }

    public class Rm_CustomCraftList : Rm_CraftList
    {
        public string ItemID = "";

        public override string ToString()
        {
            var stillExists = Rm_RPGHandler.Instance.Repositories.CraftableItems.Get(ItemID) != null
                                  ? Rm_RPGHandler.Instance.Repositories.CraftableItems.Get(ItemID).Name
                                  : "Null";
            return "Craft List for [" + stillExists + "]";
        }
    }
}
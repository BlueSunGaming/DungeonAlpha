using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class DismantleRepository //TODO: Dismantle system
    {
        public List<DismantleDefintion> TierToDismantleItems;

        public DismantleRepository()
        {
            TierToDismantleItems = new List<DismantleDefintion>();
        }

        public List<Item> GetDismantle(BuffItem item)
        {
            var tierList = TierToDismantleItems.FirstOrDefault(t => t.TierID == Rm_RPGHandler.Instance.Items.RmTierHandler.GetTierID(item));
            if(tierList == null)
            {
                Debug.LogWarning("Dismantling but no tier lists set up.");
                return new List<Item>();
            }

            var list = tierList.DismantleItems;
            //todo:test this
            var listOfItems = list.Select(i => Rm_RPGHandler.Instance.Repositories.Items.Get(i.ItemID)).ToList();

            foreach(var dismantleItem in list)
            {
                var itemToAdd = Rm_RPGHandler.Instance.Repositories.Items.Get(dismantleItem.ItemID);
                var stack = itemToAdd as IStackable;
                if(stack != null)
                {
                    stack.CurrentStacks = Random.Range(dismantleItem.MinQuantity, dismantleItem.MaxQuantity +1);
                }
                listOfItems.Add(itemToAdd);
            }

            foreach (var stack in listOfItems.OfType<IStackable>())
            {
                if (Rm_RPGHandler.Instance.Items.ScaleDismantleList)
                {
                
                    if (item.ItemType == ItemType.Weapon)
                    {
                        stack.CurrentStacks *= Rm_RPGHandler.Instance.Items.CraftSlotScalings.First(s => s.SlotIdentifier == "Weapon").Multiplier;
                    }
                    else
                    {
                        var apparel = item as Apparel;
                        if (apparel != null)
                            stack.CurrentStacks *= Rm_RPGHandler.Instance.Items.CraftSlotScalings.First(s => s.SlotIdentifier == apparel.apparelSlotID).Multiplier;
                    }
                }
            }
            return listOfItems;
        }

        
    }
    public class DismantleDefintion
    {
        public string TierID;
        public List<DismantleItem> DismantleItems;

        public DismantleDefintion()
        {
            TierID = "";
            DismantleItems = new List<DismantleItem>();
        }

        public override string ToString()
        {
            return "Tier: " + Rm_RPGHandler.Instance.Items.RmTierHandler.GetTierName(TierID);
        }
    }
    public class DismantleItem
    {
        public string ItemID;
        public int MinQuantity;
        public int MaxQuantity;

        public DismantleItem()
        {
            ItemID = "";
            MinQuantity = 1;
            MaxQuantity = 1;
        }
        public override string ToString()
        {
            var item = Rm_RPGHandler.Instance.Repositories.Items.Get(ItemID);
            if (item != null)
            {

                return MinQuantity == MaxQuantity ? "1x " + item.Name : (MinQuantity + " - " + MaxQuantity + " " + item.Name);
            }
            //todo:remove
            return ItemID;
        }
    }
}
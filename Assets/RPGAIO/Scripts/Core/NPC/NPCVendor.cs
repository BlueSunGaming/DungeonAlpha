using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class NPCVendor
    {
        private static readonly NPCVendor MyInstance = new NPCVendor();
        public static NPCVendor Instance
        {
            get { return MyInstance; }
        }

        public List<VendorShopItem> VendorShopitems (string vendorID)
        {
            return GetObject.PlayerMono.PlayerSave.GamePersistence.VendorInventories.First(v => v.ID == vendorID).VendorShopItems;
        }

        public List<Item> BuyBackItems
        {
            get { return GetObject.PlayerMono.PlayerSave.GamePersistence.BuyBackItems; }
        }

        public void BuyItemFromPlayer(PlayerCharacter player, Item item)
        {
            //todo: we take 1 stack instead of whole, so we need code to add that 1 stack to exisitng buyback item
            if (!item.CanBeDropped) return;
            BuyBackItems.Add(item);
            player.Inventory.RemoveItem(item);
            player.Inventory.Gold += item.SellValue;
        }

        public void SellItemToPlayer(string vendorID, PlayerCharacter player, string itemID)
        {
            var item = Rm_RPGHandler.Instance.Repositories.Items.Get(itemID);
            item = GeneralMethods.CopyObject(item);

            if (!(player.Inventory.Gold - item.BuyValue >= 0)) return;

            var sold = player.Inventory.AddItem(item);
            if(sold)
            {
                player.Inventory.Gold -= item.BuyValue;
                var vendorShopItem = VendorShopitems(vendorID).First(v => v.ItemID == itemID);

                if(!vendorShopItem.InfiniteStock)
                    vendorShopItem.QuantityRemaining -= 1;

                if(vendorShopItem.QuantityRemaining == 0)
                {
                    VendorShopitems(vendorID).Remove(vendorShopItem);
                }
            }
        }

        public void PlayerBuyBack(PlayerCharacter player, Item item)
        {
            if (!(player.Inventory.Gold - item.SellValue >= 0)) return;
            var sold = player.Inventory.AddItem(item);
            if(sold)
            {
                player.Inventory.Gold -= item.SellValue;
                BuyBackItems.Remove(item);
            }
        }
    }
}


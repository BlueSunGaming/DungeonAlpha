using System;
using System.Collections.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    public class VendorShop
    {
        public string Name = "";
        public List<VendorShopItem> VendorShopItems;
        public string ID;

        public VendorShop()
        {
            ID = Guid.NewGuid().ToString();
            Name = "New Vendor Shop";
            VendorShopItems = new List<VendorShopItem>();
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class VendorShopItem
    {
        public string VendorItemRef;
        public string ItemID;
        public int QuantityRemaining;
        public bool InfiniteStock;

        public VendorShopItem()
        {
            VendorItemRef = Guid.NewGuid().ToString();
            QuantityRemaining = 1;
        }

        public override string ToString()
        {
            if (Rm_RPGHandler.Instance.Repositories.Items.Get(ItemID) != null)
                return Rm_RPGHandler.Instance.Repositories.Items.Get(ItemID).Name;

            //todo: return null
            return ItemID;
        }
    }
}
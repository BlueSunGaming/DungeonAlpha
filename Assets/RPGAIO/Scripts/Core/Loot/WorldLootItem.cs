using System;

namespace LogicSpawn.RPGMaker.Core
{
    [Serializable]
    public class WorldLootItem
    {
        private Item _item = null;
        public Item Item
        {
            get { return _item ?? (_item = GetItem()); }
        }

        public int Type; // 0 = item, 1 = craftable, 2 = quest
        public string ItemId ;
        public int Quantity = 1;
        public int Gold ;
        public bool LootInWorldOnce = true;
        public string LootId = Guid.NewGuid().ToString();

        public Item GetItem()
        {
            if(Type == 0)
            {
                var item = Rm_RPGHandler.Instance.Repositories.Items.Get(ItemId);
                if (item != null)
                {
                    var stackable = item as IStackable;
                    if (stackable != null)
                    {
                        stackable.CurrentStacks = Quantity;
                    }
                    return item;
                }
            }
            
            if(Type == 1)
            {
                var item = Rm_RPGHandler.Instance.Repositories.CraftableItems.Get(ItemId);
                if (item != null)
                {
                    var stackable = item as IStackable;
                    if (stackable != null)
                    {
                        stackable.CurrentStacks = Quantity;
                    }
                    return item;
                }
            }

            if(Type == 2)
            {
                var item = Rm_RPGHandler.Instance.Repositories.QuestItems.Get(ItemId);
                if (item != null)
                {
                    var stackable = item as IStackable;
                    if (stackable != null)
                    {
                        stackable.CurrentStacks = Quantity;
                    }
                    return item;
                }
            }

            return null;
        }
    }
}
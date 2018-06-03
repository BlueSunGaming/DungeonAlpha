using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Beta;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    //TODO: Finish implement of RPGHandler maxweight/size bools in addItem methods
    public class Inventory
    {
        public InventoryType InventoryType;
        public List<Item> AllItems ;
        public int MaxItems ;

        [JsonIgnore] public PlayerCharacter Player;

        [JsonIgnore]
        public float CurrentWeight
        {
            get { return GetCurrentWeight(); }
        }

        public float _maxWeight ;

        public float MaxWeight
        {
            get
            {
                if(Rm_RPGHandler.Instance.Items.BoostCarryWeightByAttr)
                {
                    var player = GetObject.PlayerCharacter;
                    var carryWeightAttr = Rm_RPGHandler.Instance.Items.AttrForCarryWeightID;
                    var attrVal = player != null ? player.GetAttributeByID(carryWeightAttr).TotalValue : 0;
                    var multiplier = Rm_RPGHandler.Instance.Items.CarryWeightBoostMultiplier;

                    return _maxWeight + (attrVal * multiplier);
                }

                return _maxWeight;
            }
            set
            {
                _maxWeight = value;
            }
        }
        public int Gold ;

        [JsonIgnore]
        public bool OverMaxWeight
        {
            get { return GetCurrentWeight() > MaxWeight; }
        }

        [JsonIgnore]
        //TODO: This should take into account weight as well, also be generic for list<items> + int weight + int numOfItems
        public bool HasSpace
        {
            get
            {
                if (Rm_RPGHandler.Instance.Items.InventoryHasMaxItems) return AllItems.Count < MaxItems;

                return true;
            }
        }

        public Inventory()
        {
            AllItems = new List<Item>();
            _maxWeight = Rm_RPGHandler.Instance.Items.MaxWeight;
            MaxItems = Rm_RPGHandler.Instance.Items.MaxItems;
            InventoryType = InventoryType.BasicInventory;
            Gold = 0;  
        }

        public void DropItemByRef(string refId)
        {
            var itemToRemove = AllItems.FirstOrDefault(i => i.InventoryRefID == refId);

            if (itemToRemove.RunEventOnDrop && !itemToRemove.DropEventHasRun)
            {
                GetObject.EventHandler.RunEvent(itemToRemove.EventTreeIdToRunOnDrop);
                itemToRemove.DropEventHasRun = true;
            }

            if(itemToRemove.CanBeDropped)
            {
                LootSpawner.Instance.SpawnItem(GetObject.PlayerMonoGameObject.transform.position, itemToRemove);
                Player.Inventory.RemoveItem(itemToRemove);    
            }
            
            //Debug.Log("Dropped item!");
        }
        public void DropItemByID(string itemID)
        {
            var itemToRemove = AllItems.FirstOrDefault(i => i.ID == itemID);
            if (itemToRemove.CanBeDropped)
            {
                LootSpawner.Instance.SpawnItem(GetObject.PlayerMonoGameObject.transform.position, itemToRemove);
                Player.Inventory.RemoveItem(itemToRemove);
            }
        }
        public void DropItem(Item item)
        {
            if(AllItems.Contains(item))
            {
                if (item.CanBeDropped)
                {
                    LootSpawner.Instance.SpawnItem(GetObject.PlayerMonoGameObject.transform.position, item);
                    Player.Inventory.RemoveItem(item);
                }
            }
            else
            {
                Debug.Log("Inventory does not contain this item.");
            }
        }

        public void UseItemByRef(string refId)
        {
            //ItemType.Apparel, ItemType.Weapon, ItemType.Socket, ItemType.Book, ItemType.Consumable
            var itemToUse = AllItems.FirstOrDefault(i => i.InventoryRefID == refId);
            if (itemToUse == null) return;


            if(itemToUse.RunEventOnUse && !itemToUse.EventHasRun)
            {
                GetObject.EventHandler.RunEvent(itemToUse.EventTreeIdToRunOnUse);
                itemToUse.EventHasRun = true;
            }

            if(itemToUse.ItemType == ItemType.Apparel || itemToUse.ItemType == ItemType.Weapon)
            {
                GetObject.PlayerCharacter.Equipment.EquipItem(itemToUse);    
            }
            else if(itemToUse.ItemType == ItemType.Consumable)
            {
                var consumable = (Consumable)itemToUse;
                if(consumable.CurrentCooldown <= 0)
                {
                    consumable.CurrentCooldown = consumable.Cooldown;
                    GetObject.PlayerCharacter.AddRestoration(GeneralMethods.CopyObject(consumable.Restoration));
                    var stack = (IStackable)consumable;
                    stack.CurrentStacks--;
                    if (stack.CurrentStacks == 0)
                    {
                        GetObject.PlayerCharacter.Inventory.RemoveItemByRef(refId);
                    }
                }
            }
            else if(itemToUse.ItemType == ItemType.Book )
            {
                var book = (Book)itemToUse;
                BookHandler.Instance.ReadBook(book);
            }
            else if(itemToUse.ItemType == ItemType.Socket )
            {
                Debug.Log("not implemeted: use socket");
            }

            RPG.Events.OnInventoryUpdate(new RPGEvents.InventoryUpdateEventArgs());

            //Debug.Log("Used an inventory item!");
        }

        public bool AddItem(Item item)
        {
            if(!CanAddItem(item))
            {
                return false;
            }
            var stackableItem = item as IStackable;
            var isStackable = stackableItem != null;
            var addToStack = false;
            var existingItem = AllItems.FirstOrDefault(i => i.ID == item.ID);
            if (isStackable && existingItem != null)
            {
                var stack = (IStackable)existingItem;
                addToStack = true;
                stack.CurrentStacks += stackableItem.CurrentStacks;
                if (existingItem.PickupEventHasRun || item.PickupEventHasRun)
                {
                    existingItem.PickupEventHasRun = true;
                    item.PickupEventHasRun = true;
                }
            }

            if (addToStack)
            {
                RPG.Events.OnInventoryUpdate(new RPGEvents.InventoryUpdateEventArgs());
                return true;
            }

            if(item.InventoryRefID == null)
            {
                item.InventoryRefID = Guid.NewGuid().ToString();    
            }
            AllItems.Add(item);

            if (item.RunEventOnPickup && !item.PickupEventHasRun)
            {
                GetObject.EventHandler.RunEvent(item.EventTreeIdToRunOnPickup);
                item.PickupEventHasRun = true;
            }

            RPG.Events.OnInventoryUpdate(new RPGEvents.InventoryUpdateEventArgs());
            
            return true;
        }

        public bool CanAddItem(Item item)
        {
            var x = new List<Item>();
            x.Add(item);
            return CanAddItems(x);
        }

        public bool CanAddItems(List<Item> items)
        {
            bool hasMaxItems;
            bool usesWeightSystem;

            switch (InventoryType)
            {
                case InventoryType.BasicInventory:
                    hasMaxItems = Rm_RPGHandler.Instance.Items.InventoryHasMaxItems;
                    usesWeightSystem = Rm_RPGHandler.Instance.Items.InventoryUsesWeightSystem;
                    break;
                case InventoryType.GlobalStash:
                    hasMaxItems = Rm_RPGHandler.Instance.Items.GlobalStashHasMaxItems;
                    usesWeightSystem = Rm_RPGHandler.Instance.Items.GlobalStashUsesWeightSystem;
                    break;
                case InventoryType.CharacterStash:
                    hasMaxItems = Rm_RPGHandler.Instance.Items.CharacterStashHasMaxItems;
                    usesWeightSystem = Rm_RPGHandler.Instance.Items.CharacterStashUsesWeightSystem;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            if(!hasMaxItems && (!usesWeightSystem || 
                (InventoryType == InventoryType.BasicInventory && Rm_RPGHandler.Instance.Items.AllowOverMax)))
            {
                return true;
            }

            bool canAdd;
            var totalWeight = 0f;
            var totalCount = 0;

            foreach(var item in items)
            {
                var stackableItem = item as IStackable;
                var isStackable = stackableItem != null;
                
                if (item == null) continue;

                var existingItem = AllItems.FirstOrDefault(i => i.ID == item.ID);
                totalCount  += isStackable && existingItem != null ? 0 : 1;

                if (isStackable)
                {
                    var weightToAdd = item.Weight * stackableItem.CurrentStacks;
                    totalWeight += weightToAdd;
                }
                else
                {
                    totalWeight += item.Weight;
                }
            }
            if (!hasMaxItems)
            {
                canAdd = GetCurrentWeight() + totalWeight <= MaxWeight || Rm_RPGHandler.Instance.Items.AllowOverMax;
            }
            else if (!usesWeightSystem)
            {
                canAdd = AllItems.Count + totalCount <= MaxItems;
            }
            else
            {
                canAdd = (AllItems.Count + totalCount <= MaxItems) && (GetCurrentWeight() + totalWeight <= MaxWeight) 
                            || Rm_RPGHandler.Instance.Items.AllowOverMax;
            }

            return canAdd;
        }

        public float GetCurrentWeight()
        {
            var currentWeight = 0f;
            var unstackables = AllItems.Where(a => (a as IStackable) == null).ToList();
            var stackables = AllItems.Where(a => (a as IStackable) != null).ToList();

            if(unstackables.Count > 0)
                currentWeight += unstackables.Sum(u => u.Weight);
            
            if(stackables.Count > 0)
                currentWeight += stackables.Sum(u => u.Weight * ((IStackable) u).CurrentStacks);

            var player = Player;
            if(player != null)
            {
                var isPlayer = player.Inventory == this;
                if (isPlayer)
                {
                    currentWeight += player.Equipment.AllEquippedItems.Where(i => i != null).Sum(a => a.Weight);
                }
            }
            
            return currentWeight;

        }

        public void RemoveItem(int index)
        {
            AllItems.RemoveAt(index);
            RPG.Events.OnInventoryUpdate(new RPGEvents.InventoryUpdateEventArgs());

        }

        public void RemoveItem(string itemID)
        {
            var itemToRemove = AllItems.FirstOrDefault(i => i.ID == itemID);
            AllItems.Remove(itemToRemove);
            RPG.Events.OnInventoryUpdate(new RPGEvents.InventoryUpdateEventArgs());

        }
        public void RemoveItemByRef(string itemRefId)
        {
            var itemToRemove = AllItems.FirstOrDefault(i => i.InventoryRefID == itemRefId);
            AllItems.Remove(itemToRemove);
            RPG.Events.OnInventoryUpdate(new RPGEvents.InventoryUpdateEventArgs());

        }

        public void RemoveItem(Item item)
        {
            //Debug.Log("Trying to remove item with name: " + item.Name);
            AllItems.Remove(item);
            RPG.Events.OnInventoryUpdate(new RPGEvents.InventoryUpdateEventArgs());
        }

        public void RemoveStack(Item item, int amountToRemove)
        {
            var stackableItem = (IStackable)item;
            stackableItem.CurrentStacks -= amountToRemove;
            if(stackableItem.CurrentStacks == 0) RemoveItem(item);
            RPG.Events.OnInventoryUpdate(new RPGEvents.InventoryUpdateEventArgs());
        }

        public List<Item> GetAllItems(bool ordered = false)
        {
            return ordered ?  AllItems.OrderBy(i => i.Name).ToList() : AllItems;
        }

        public List<Item> GetItemsByType(ItemType itemType)
        {
            return AllItems.Where(i => i.ItemType == itemType).OrderBy(i => i.Name).ToList();
        }

        public void AddGold(int amount)
        {
            Gold += amount;
        }

        public bool RemoveGold(int amount)
        {
            if(Gold - amount < 0)
            {
                return false;
            }

            Gold -= amount;
            return true;
        }

        public Item GetReferencedItem(string itemInventoryRefId)
        {
            return AllItems.FirstOrDefault(i => i.InventoryRefID == itemInventoryRefId);
        }
    }

    public enum InventoryType
    {
        BasicInventory,
        GlobalStash,
        CharacterStash
    }
}

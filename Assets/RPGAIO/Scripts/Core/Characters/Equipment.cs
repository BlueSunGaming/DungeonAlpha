using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class Equipment
    {
        [JsonIgnore]
        public PlayerCharacter Player ;

        //todo: rename to slots
        public List<ItemSlot> EquippedItems ;

        public Equipment(PlayerCharacter player)
        {
            Player = player;
            EquippedItems = new List<ItemSlot>();

            EquippedItems.Add(new ItemSlot() { SlotID = "Weapon", SlotName = "Weapon"});

            for (var i = 0; i < Rm_RPGHandler.Instance.Items.ApparelSlots.Count; i++)
            {
                var slotName = Rm_RPGHandler.Instance.Items.ApparelSlots[i];
                EquippedItems.Add(new ItemSlot() { SlotID = slotName.ID, SlotName = slotName.Name});
            }
        }

        //todo:test
        public ItemSlot GetSlot(string slotId)
        {
            return EquippedItems.FirstOrDefault(e => e.SlotID == slotId);
        }

        [JsonIgnore]
        public List<Item> AllEquippedItems
        {
            get
            {
                if(EquippedItems.Any(e => e.Item != null))
                {
                    var items = EquippedItems.Where(i => Rm_RPGHandler.Instance.Items.EnableOffHandSlot ? i != null : i.SlotID != "OffHand")
                        .Select(e => e.Item).Where(e => e != null).ToList();
                    return items;    
                }
                else
                {
                    return new List<Item>();
                }
            }
        }
        [JsonIgnore]
        public Item EquippedWeapon
        {
            get
            {
                return GetSlot("Weapon").Item;
            }
        }

        [JsonIgnore]
        public Item EquippedOffHand
        {
            get
            {
                var slot = GetSlot("OffHand") ;
                return  slot != null ? slot.Item : null;
            }
        }

        public bool Unarmed
        {
            get
            {
                return EquippedWeapon == null && ((EquippedOffHand != null && (EquippedOffHand as Weapon) == null) || EquippedOffHand == null);
            }
        }

        //TODO: EquipToSlot, e.g. for drag and drop
        public EquipResult EquipToSlot(Item itemToEquip)
        {
            return EquipResult.Success;
        }

        public EquipResult EquipItem(Item equippable)
        {
            var result = EquipResult.WrongItem;
            switch (equippable.ItemType)
            {
                case ItemType.Apparel:
                    result = EquipItem(equippable as Apparel);
                    break;
                case ItemType.Weapon:
                    result = EquipItem(equippable as Weapon);
                    break;
            }

            if (result == EquipResult.Success)
            {
                UpdateDynamicItems();
                RPG.Events.OnEquippedItem(new RPGEvents.EquippedItemEventArgs()
                {
                    Item = equippable
                });
                Player.FullUpdateStats();
            }

            return result;
        }

        public void UpdateDynamicItems()
        {
            var player = GetObject.PlayerMono;
            if (player == null) return;

            var classDef = Rm_RPGHandler.Instance.Player.CharacterDefinitions.First(c => c.ID == Player.PlayerCharacterID);
            var definitions = classDef.EquipmentInfo.Definitions;

            var childrenTransforms = player.transform.GetAllChildren<Transform>();
            var dynamicChildren = childrenTransforms.Where(c => definitions.FirstOrDefault(d => d.NameOfTransform == c.name) != null).ToList();

            foreach(var dynamicChild in dynamicChildren)
            {
                var defsForChild = definitions.Where(d => d.NameOfTransform == dynamicChild.name).ToList();
                var display = false;
                foreach(var def in defsForChild)
                {
                    if(def.OnlyWeaponSlot)
                    {
                        var isWeapon = EquippedWeapon != null && EquippedWeapon.ID == def.RequiredEquippedItemId;
                        if (isWeapon) display = true;
                    }
                    else if(def.SpecificSlot)
                    {
                        var isInSlot = GetSlot(def.SlotId).Item != null && GetSlot(def.SlotId).Item.ID == def.RequiredEquippedItemId;
                        if (isInSlot) display = true;
                    }
                    else
                    {
                        var equipped = AllEquippedItems.FirstOrDefault(i => i.ID == def.RequiredEquippedItemId);
                        if (equipped != null) display = true;
                    }
                }
                dynamicChild.gameObject.SetActive(display);
            }
        }

        private EquipResult EquipItem(Weapon weapon)
        {
            if (Player.Level < weapon.RequiredLevel && Rm_RPGHandler.Instance.Items.ItemsHaveRequiredLevel) return EquipResult.LowLevel;
            //TODO : should be direct check etc
            if (Rm_RPGHandler.Instance.Items.LimitItemsToClass && !weapon.AllClasses && weapon.ClassIDs.FirstOrDefault(c => c == Player.PlayerCharacterID) == null ) return EquipResult.WrongClass;

            var equippedWeaponSlot = GetSlot("Weapon");
            var equippedOffHandSlot = GetSlot("OffHand");

            if (equippedWeaponSlot.Item == null)
            {
                if (equippedOffHandSlot != null && equippedOffHandSlot.Item == null)
                {
                    equippedWeaponSlot.Item = weapon;
                    Player.Inventory.RemoveItem(weapon);
                }
                else if (!weapon.TwoHanded)
                {
                    equippedWeaponSlot.Item = weapon;
                    Player.Inventory.RemoveItem(weapon);
                }
                else
                {
                    equippedWeaponSlot.Item = weapon;
                    Player.Inventory.RemoveItem(weapon);
                    Player.Inventory.AddItem(equippedOffHandSlot.Item);
                    equippedOffHandSlot.Item = null;
                }
            }
            else
            {
                if (!weapon.TwoHanded)
                {
                    if (Rm_RPGHandler.Instance.Items.EnableOffHandSlot && weapon.AllowDualWield && Rm_RPGHandler.Instance.Items.AllowDualWield)
                    {
                        if((equippedWeaponSlot.Item as Weapon).TwoHanded)
                        {
                            var temp = equippedWeaponSlot.Item;
                            equippedWeaponSlot.Item = weapon;

                            Player.Inventory.RemoveItem(weapon);
                            Player.Inventory.AddItem(temp);   
                        }
                        else
                        {
                            if (equippedOffHandSlot.Item == null)
                            {
                                equippedOffHandSlot.Item = weapon;
                                Player.Inventory.RemoveItem(weapon);
                            }
                            else
                            {
                                Player.Inventory.AddItem(equippedOffHandSlot.Item);
                                equippedOffHandSlot.Item = weapon;
                                Player.Inventory.RemoveItem(weapon);
                            }
                        }
                    }
                    else
                    {
                        var temp = equippedWeaponSlot.Item;
                        equippedWeaponSlot.Item = weapon;

                        Player.Inventory.RemoveItem(weapon);
                        Player.Inventory.AddItem(temp);    
                    }
                }
                else if (equippedOffHandSlot != null && equippedOffHandSlot.Item != null)
                {
                    if (Player.Inventory.HasSpace)
                    {
                        Player.Inventory.AddItem(equippedWeaponSlot.Item);
                        Player.Inventory.AddItem(equippedOffHandSlot.Item);
                        equippedWeaponSlot.Item = equippedOffHandSlot.Item = null;
                        equippedWeaponSlot.Item = weapon;
                        Player.Inventory.RemoveItem(weapon);
                    }
                    else
                    {
                        return EquipResult.NoSpace;
                    }
                }
                else
                {
                    var temp = equippedWeaponSlot.Item;
                    equippedWeaponSlot.Item = weapon;

                    Player.Inventory.RemoveItem(weapon);
                    Player.Inventory.AddItem(temp);
                }
            }

            return EquipResult.Success;
        }

        private EquipResult EquipItem(Apparel apparel)
        {
            if (Player.Level < apparel.RequiredLevel && Rm_RPGHandler.Instance.Items.ItemsHaveRequiredLevel) return EquipResult.LowLevel;
            if (Rm_RPGHandler.Instance.Items.LimitItemsToClass && !apparel.AllClasses && apparel.ClassIDs.FirstOrDefault(c => c == Player.PlayerCharacterID) == null) return EquipResult.WrongClass;

            var EquippedWeaponSlot = GetSlot("Weapon");
            var EquippedOffHandSlot = GetSlot("OffHand");
            var EquippedItemSlot = GetSlot(apparel.apparelSlotID);

            if (Rm_RPGHandler.Instance.Items.EnableOffHandSlot && apparel.ApparelSlotName == "OffHand")
            {
                if (EquippedOffHandSlot.Item == null)
                {
                    if (EquippedWeaponSlot.Item == null)
                    {
                        EquippedOffHandSlot.Item = apparel;
                        Player.Inventory.RemoveItem(apparel);
                    }
                    else
                    {
                        if (!(EquippedWeaponSlot.Item as Weapon).TwoHanded)
                        {

                            EquippedOffHandSlot.Item = apparel;
                            Player.Inventory.RemoveItem(apparel);
                        }
                        else
                        {
                            Player.Inventory.AddItem(EquippedWeaponSlot.Item);
                            EquippedWeaponSlot.Item = null;
                            EquippedOffHandSlot.Item = apparel;
                            Player.Inventory.RemoveItem(apparel);
                        }
                    }
                }
                else
                {
                    var temp = EquippedOffHandSlot.Item;
                    EquippedOffHandSlot.Item = apparel;

                    Player.Inventory.RemoveItem(apparel);
                    Player.Inventory.AddItem(temp);
                }
            }
            else
            {
                if(EquippedItemSlot != null)
                {
                    if (EquippedItemSlot.Item == null)
                    {
                        EquippedItemSlot.Item = apparel;
                        Player.Inventory.RemoveItem(apparel);
                    }
                    else
                    {
                        var temp = EquippedItemSlot.Item;
                        EquippedItemSlot.Item = apparel;

                        Player.Inventory.RemoveItem(apparel);
                        Player.Inventory.AddItem(temp);
                    }
                }
                else
                {
                    Debug.Log("Trying to equip to a slot that does not exist, it's likely the offhand with EnableOffHand disabled.");
                }
            }
            return EquipResult.Success;
        }

        public bool UnEquipItem(ItemSlot slot, Item item)
        {
            if(Player.Inventory.AddItem(item))
            {
                slot.Item = null;

                UpdateDynamicItems();
                Player.FullUpdateStats();
                RPG.Events.OnUnEquippedItem(new RPGEvents.UnEquippedItemEventArgs()
                {
                    Item = item
                });
                return true;
            }

            return false;
        }
        public bool UnEquipItem(Item item)
        {
            var equippedSlot = EquippedItems.FirstOrDefault(i => i.Item != null && i.Item.ID == item.ID);
            
            if(equippedSlot != null)
            {
                if (Player.Inventory.AddItem(item))
                {
                    equippedSlot.Item = null;

                    UpdateDynamicItems();
                    Player.FullUpdateStats();
                    RPG.Events.OnUnEquippedItem(new RPGEvents.UnEquippedItemEventArgs()
                    {
                        Item = item
                    });
                    return true;
                }
            }
            return false;
        }
        public bool UnEquipItem(string itemID)
        {
            var equippedSlot = EquippedItems.FirstOrDefault(i => i.Item != null && i.Item.ID == itemID);
            
            if(equippedSlot != null)
            {
                var item = equippedSlot.Item;
                if (Player.Inventory.AddItem(item))
                {
                    equippedSlot.Item = null;

                    UpdateDynamicItems();
                    Player.FullUpdateStats();
                    RPG.Events.OnUnEquippedItem(new RPGEvents.UnEquippedItemEventArgs()
                    {
                        Item = item
                    });
                    return true;
                }
            }
            return false;
        }

        public Item GetReferencedItem(string itemInventoryRefId)
        {
            var allequips = AllEquippedItems;
            if(allequips.Count > 0)
            {
                var found = allequips.FirstOrDefault(i => i.InventoryRefID == itemInventoryRefId);
                return found;    
            }

            return null;
        }
    }

    public class ItemSlot
    {
        public string SlotID;
        public string SlotName = "";
        public Item Item = null;

        public ItemSlot()
        {
            SlotID = Guid.NewGuid().ToString();
        }
    }
}

public enum EquipResult
{
    Success,
    LowLevel,
    WrongClass,
    NoSpace,
    WrongItem
}
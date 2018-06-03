using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class CraftHandler
    {
        private static readonly CraftHandler MyInstance = new CraftHandler();
        public static CraftHandler Instance
        {
            get { return MyInstance; }
        }

        public bool Crafting;

        private List<CraftSlotScaling> _slotScaling
        {
            get { return Rm_RPGHandler.Instance.Items.CraftSlotScalings; }
        }

        public void CraftItem(Item item)
        {

            if (Crafting) return;

            Crafting = true;

            var player = GetObject.PlayerCharacter;
            var buffItem = item as BuffItem;

            //TODO: may not need this if GUI already handles whether the button is enabled by this boolean:
            if(buffItem != null)
            {
                if(Rm_RPGHandler.Instance.Items.UseTraitLvlAsReqForCrafting)
                    if (!(player.Traits.First(t => t.ID == Rm_RPGHandler.Instance.Items.TraitIDForCrafting).Level >= buffItem.RequiredLevel))
                    {

                        Debug.Log("Trait level not high enough");
                        return;
                    }

                if(Rm_RPGHandler.Instance.Items.UsePlayerLvlAsReqForCrafting)
                    if (!(player.Level >= buffItem.RequiredLevel))
                    {

                        Debug.Log("Required player level not met to craft");
                        return;
                    }
            }

            //Get List
            var materialList = Rm_RPGHandler.Instance.Repositories.CraftLists.GetCraftList(item);

            var scaling = 1;
            if(Rm_RPGHandler.Instance.Items.ScaleCraftList)
            {
                if (item.ItemType == ItemType.Apparel)
                {
                    var apparel = item as Apparel;
                    scaling = _slotScaling.First(s => s.SlotIdentifier == apparel.apparelSlotID).Multiplier;
                }
                else if (item.ItemType == ItemType.Weapon)
                {
                    scaling = _slotScaling.First(s => s.SlotIdentifier == "Weapon").Multiplier;
                }
                else
                {
                    scaling = 1;
                }
    
            }

            Debug.Log(item.Description);
            if (player.Inventory.CanAddItem(item))
            {
                if (!CheckAndCraftItem(item, player.Inventory, scaling, materialList)) return;
                var craftedItem = GeneralMethods.CopyObject(item);

                GetObject.EventHandler.StartCoroutine(CraftItemRoutine(craftedItem));

            }
            else
            {
                Debug.Log("[RPGAIO] Player has no space to craft that item.");
            }
                
            //Debug.Log("Successful craft");
        }

        IEnumerator CraftItemRoutine(Item craftedItem)
        {
            AudioPlayer.Instance.Play(Rm_RPGHandler.Instance.Items.CraftSound.Audio, AudioType.SoundFX, Vector3.zero);
            yield return new WaitForSeconds(Rm_RPGHandler.Instance.Items.CraftTime);
            var player = GetObject.PlayerCharacter;
            player.Inventory.AddItem(craftedItem);
            Crafting = false;
        }

        private bool CheckAndCraftItem(Item craftItem,Inventory inventory, int scaling, List<Item> materialList )
        {
            var hasItems = CanCraft(craftItem);
            

            if(hasItems)
            {
                foreach (var requiredMat in materialList)
                {
                    var requiredMatStack = requiredMat as IStackable;

                    var material = inventory.GetAllItems().FirstOrDefault(i => i.Name == requiredMat.Name);

                    if(requiredMatStack != null)
                    {
                        inventory.RemoveStack(material, requiredMatStack.CurrentStacks * scaling);    
                    }
                    else
                    {
                        inventory.RemoveItem(material);
                    }
                    
                }

                return true;
            }

            return false;
        }
        public bool CanCraft(Item craftItem)
        {
            
            if(Rm_RPGHandler.Instance.Items.UsePlayerLvlAsReqForCrafting)
            {
                var buffItem = craftItem as BuffItem;
                if(buffItem != null && GetObject.PlayerCharacter.Level < buffItem.RequiredLevel)
                {
                    Debug.Log("Player level is not high enough to craft this.");
                    return false;
                }
            }

            if(Rm_RPGHandler.Instance.Items.UseTraitLvlAsReqForCrafting)
            {
                var trait = GetObject.PlayerCharacter.GetTraitByID(Rm_RPGHandler.Instance.Items.TraitIDForCrafting);
                var buffItem = craftItem as BuffItem;
                if(buffItem != null && trait.Level < buffItem.RequiredLevel)
                {
                    Debug.Log("Trait [" + RPG.Stats.GetTraitName(trait.ID) +"] level is not high enough to craft this.");
                    return false;
                }
            }

            var materialList = Rm_RPGHandler.Instance.Repositories.CraftLists.GetCraftList(craftItem);

            var scaling = 1;
            if (Rm_RPGHandler.Instance.Items.ScaleCraftList)
            {
                if (craftItem.ItemType == ItemType.Apparel)
                {
                    var apparel = craftItem as Apparel;
                    scaling = _slotScaling.First(s => s.SlotIdentifier == apparel.apparelSlotID).Multiplier;
                }
                else if (craftItem.ItemType == ItemType.Weapon)
                {
                    scaling = _slotScaling.First(s => s.SlotIdentifier == "Weapon").Multiplier;
                }
            }

            var inventory = GetObject.PlayerCharacter.Inventory;
            var hasItems = true;
            foreach (var requiredMat in materialList)
            {
                var requiredMatStack = requiredMat as IStackable;

                var material = inventory.GetAllItems().FirstOrDefault(i => i.ID == requiredMat.ID);
                if (material == null)
                {
                    hasItems = false;
                    break;
                }

                var materialStack = material as IStackable;
                if (requiredMatStack != null)
                {
                    if (materialStack.CurrentStacks >= requiredMatStack.CurrentStacks * scaling) continue;
                }
                else
                {
                    continue;
                }

                hasItems = false;
                break;
            }

            return hasItems;
        }

        public void DismantleItem(Item item)
        {
            var buffItem = item as BuffItem;
            if (buffItem != null)
            {
                var items = Rm_RPGHandler.Instance.Repositories.Dismantle.GetDismantle(buffItem);

                //todo: Implement this way of checking if inventory has space for list of items into Inventory.cs
                      //also, itemCount is wrong, as the item might already be in the inventory (in the case of stackables)
                var totalWeight = items.Sum(i => i.Weight);
                var itemCount = items.Count;

                var canAdd = true;
                if(Rm_RPGHandler.Instance.Items.InventoryUsesWeightSystem && !Rm_RPGHandler.Instance.Items.AllowOverMax)
                {
                    canAdd = GetObject.PlayerCharacter.Inventory.CurrentWeight + totalWeight <=
                             GetObject.PlayerCharacter.Inventory.MaxWeight;
                }

                if(Rm_RPGHandler.Instance.Items.InventoryHasMaxItems)
                {
                    canAdd = GetObject.PlayerCharacter.Inventory.AllItems.Count + itemCount <=
                             GetObject.PlayerCharacter.Inventory.MaxItems;
                }

                if(canAdd)
                {
                    foreach (var shards in items)
                    {
                        GetObject.PlayerCharacter.Inventory.AddItem(shards);
                    }
                    GetObject.PlayerCharacter.Inventory.RemoveItem(item);
                }
                else
                {
                    Debug.Log("Not enough space!");
                }
                
            }
        }
    }

    
}
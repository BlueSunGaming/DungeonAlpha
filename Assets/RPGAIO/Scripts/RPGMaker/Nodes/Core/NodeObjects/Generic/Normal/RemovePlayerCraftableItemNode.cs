using System.Linq;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Player", "Items")]
    public class RemovePlayerCraftableItemNode : SimpleNode
    {
        public override string Name
        {
            get { return "Remove / Take Craftable Item From Player"; }
        }

        public override string Description
        {
            get { return "Takes a craftable item from the player's inventory"; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        protected override void SetupParameters()
        {
            Add("Item", PropertyType.CraftableItem, null, null); 
            Add("Quantity", PropertyType.Int, null, 1); 
        }

        protected override void Eval(NodeChain nodeChain)
        {
            var itemId = (string)ValueOf("Item");
            var quantity = (int)ValueOf("Quantity");

            var playerInventory = GetObject.PlayerCharacter.Inventory;
            if(quantity == 1)
            {
                var item = playerInventory.GetAllItems().FirstOrDefault(i => i.ID == itemId);
                if (item != null)
                {
                    var stackable = item as IStackable;
                    if (stackable != null)
                    {
                        playerInventory.RemoveStack(item, quantity);
                    }
                    else
                    {
                        playerInventory.RemoveItem(itemId);
                    }
                }
                else
                {
                    Debug.LogWarning("[RPGAIO] Node event tried to remove item from player inventory but it was not found.");
                }
            }
            else
            {
                var allItems = playerInventory.GetAllItems();
                var item = allItems.FirstOrDefault(i => i.ID == itemId);

                if (item != null)
                {
                    var stackable = item as IStackable;
                    if (stackable != null)
                    {
                        playerInventory.RemoveStack(item, quantity);
                    }
                    else
                    {
                        var amountFound = allItems.Count(i => i.ID == itemId);
                        if(amountFound < quantity)
                        {
                            Debug.LogWarning("[RPGAIO] Node event did not find enough quantity of an item to remove, but will remove as many as possible.");
                        }
                        var amountToRemove = Mathf.Min(quantity, amountFound);
                        for (int i = 0; i < amountToRemove; i++)
                        {
                            playerInventory.RemoveItem(itemId);
                        }
                    }
                    
                }
                else
                {
                    Debug.LogWarning("[RPGAIO] Node event tried to remove item from player inventory but it was not found.");
                }
            }
        }
    }
}
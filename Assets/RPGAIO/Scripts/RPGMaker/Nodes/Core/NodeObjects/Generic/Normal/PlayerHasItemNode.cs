using System.Linq;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Player", "Items")]
    public class PlayerHasItemNode : BooleanNode
    {
        public override string Name
        {
            get { return "Player Has Item"; }
        }

        public override string Description
        {
            get { return "Returns true if the player has the item or required quantity."; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        protected override void SetupParameters()
        {
            Add("Item", PropertyType.Item, null, null); 
            Add("Quantity", PropertyType.Int, null, 1); 
        }

        protected override bool Eval(NodeChain nodeChain)
        {
            var itemId = (string)ValueOf("Item");
            var quantityNeeded = (int)ValueOf("Quantity");

            var foundItem = GetObject.PlayerCharacter.Inventory.AllItems.FirstOrDefault(i => i.ID == itemId);
            var foundItemCount = GetObject.PlayerCharacter.Inventory.AllItems.Count(i => i.ID == itemId);
            if(foundItem != null && quantityNeeded > 1)
            {
                var stackable = foundItem as IStackable;

                if(stackable == null)
                {
                    return foundItemCount >= quantityNeeded;
                }

                return stackable.CurrentStacks >= quantityNeeded;
            }

            return foundItem != null;
        }
    }
}
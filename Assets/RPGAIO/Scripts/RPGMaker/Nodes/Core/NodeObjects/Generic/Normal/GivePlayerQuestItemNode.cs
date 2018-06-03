using System.Collections.Generic;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Player", "Items")]
    public class GivePlayerQuestItemNode : BooleanNode
    {
        public override string Name
        {
            get { return "Give Player Quest Item"; }
        }

        public override string Description
        {
            get { return "Returns true if the item is successfully given to the player."; }
        }

        public override string SubText
        {
            get { return "Returns false if item is not given"; }
        }

        protected override void SetupParameters()
        {
            Add("Item", PropertyType.QuestItem, null, null);
            Add("Quantity", PropertyType.Int, null, 1); 
        }

        protected override bool Eval(NodeChain nodeChain)
        {
            var itemId = (string)ValueOf("Item");
            var quantity = (int)ValueOf("Quantity");

            var item = Rm_RPGHandler.Instance.Repositories.QuestItems.Get(itemId);
            var stackable = item as IStackable;


            if (item != null)
            {
                if (stackable != null)
                {
                    stackable.CurrentStacks = quantity;
                    return GetObject.PlayerCharacter.Inventory.AddItem(item);
                }

                //else

                var itemsToAdd = new List<Item>();
                for (int i = 0; i < quantity; i++)
                {
                    itemsToAdd.Add(Rm_RPGHandler.Instance.Repositories.QuestItems.Get(itemId));
                }

                var canAddItems = GetObject.PlayerCharacter.Inventory.CanAddItems(itemsToAdd);
                if (canAddItems)
                {
                    for (int i = 0; i < quantity; i++)
                    {
                        GetObject.PlayerCharacter.Inventory.AddItem(itemsToAdd[i]);
                    }
                }

                return canAddItems;
            }

            Debug.LogError("[RPGAIO] Could not find item in Item Db to give player.");
            return false;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _core
{
    public class IInventory : MonoBehaviour
    {
        public enum InventoryType
        {
            PLAYER,
            CHEST,
            NPC,
            MONSTER
        }

        private List<Item> currentItems = new List<Item>();

        // Should be overridden by inherited class
        protected InventoryType invType = InventoryType.CHEST;

        //void Open()
        void Start()
        {
            RandomPopulate();
            Item i = GameManager.instance.IsValidItem(1);
            currentItems.Add(i);
        }

        // Will be protected to only trigger when the dungeon is generated
        void RandomPopulate()
        {

        }

        protected List<Item> GetCurrentItems()
        {
            return currentItems;
        }

        protected bool AddItem(Item i)
        {
            bool returnVal = false;
            // If the item doesn't exist, add it.
            if (!currentItems.Exists(x => x.nItemID == i.nItemID))
            {
                currentItems.Add(i);
                returnVal = true;
            }
            // If the item does exist, increment the count of the item.
            //else if (currentItems.Exists(x => x.nItemID == i.nItemID))
            //{
            //    currentItems.Add(i);
            //}

            return returnVal;
        }

    }

}
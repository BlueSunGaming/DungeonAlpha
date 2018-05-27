using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonRPG;

namespace _core
{
    public enum InventoryType
    {
        PLAYER,
        CHEST,
        NPC,
        MONSTER
    }

    public interface IInventory 
    {
        List<Item> currentItems { get; set; }
        //= new List<Item>();

        // Should be overridden by inherited class
        InventoryType invType { get; set; }

        void RandomPopulate();

        List<Item> GetCurrentItems();
        bool AddItem(Item i);
        bool RemoveItem(Item i);
        bool DoesItemExist(int itemID);

        //= InventoryType.CHEST;

        //void Open()
        //void Start()
        //{
        //    RandomPopulate();
        //    Item i = GameManager.instance.IsValidItem(1);
        //    currentItems.Add(i);
        //}

        // Will be protected to only trigger when the dungeon is generated


    }

}
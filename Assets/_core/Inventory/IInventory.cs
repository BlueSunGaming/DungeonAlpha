using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

}

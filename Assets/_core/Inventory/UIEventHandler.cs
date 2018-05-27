using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonRPG;

public class UIEventHandler : MonoBehaviour
{
    public delegate void ItemEventHandler(Item item);
    public static event ItemEventHandler OnItemAddedToInventory;
    public static event ItemEventHandler OnItemAddedToPlayerInventory;
    public static event ItemEventHandler OnItemRemovedFromInventory;
    public static event ItemEventHandler OnItemEquipped;

    // Dialogue Or Menu Display Event Handlers
    public delegate void DisplayEventHandler(bool shouldShow);
    public static event DisplayEventHandler OnInventoryDisplayed;


    public static void ItemAddedToInventory(Item item)
    {
        if (OnItemAddedToInventory != null)
        {
            OnItemAddedToInventory(item);
        }
    }

    public static void ItemAddedToPlayerInventory(Item item)
    {
        if (OnItemAddedToPlayerInventory != null)
        {
            OnItemAddedToPlayerInventory(item);
        }
    }

    public static void ItemAddedToInventory(List<Item> items)
    {
        if (OnItemAddedToInventory != null)
        {
            foreach (Item i in items)
            {
                OnItemAddedToInventory(i);
            }
        }
    }

    public static void ItemRemovedToInventory(Item item)
    {
        if (OnItemRemovedFromInventory != null)
        {
            OnItemRemovedFromInventory(item);
        }
    }

    public static void ItemEquipped(Item item)
    {
        if (OnItemEquipped != null)
        {
            OnItemEquipped(item);
        }
    }

    public static void InventoryDisplayed(bool shouldShow)
    {
        if (OnInventoryDisplayed != null)
        {
            OnInventoryDisplayed(shouldShow);
        }
    }
}

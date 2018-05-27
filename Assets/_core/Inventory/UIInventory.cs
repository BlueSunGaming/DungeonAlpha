using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DungeonRPG;

public class UIInventory : MonoBehaviour {
    public GameObject playerInventoryPanel;
    public GameObject itemDetailsInventoryPanel;
    public RectTransform scrollViewContent;

    InventoryUIItem itemContainer { get; set; }

    public RectTransform transferContent;
    public GameObject transferInventoryPanel;
    
    private Text ListOfItems;
    List<InventoryUIItem> allItemUis = new List<InventoryUIItem>();
    List<InventoryUIItem> allPlayerItemUis = new List<InventoryUIItem>();
    //public InventoryItemUI itemSlot { get; set; }
    bool menuIsVisible { get; set; }
    Animator animator;

    Item currentSelection { get; set; }

    // Use this for initialization
    void Start ()
    {
        menuIsVisible = true;
        
        itemContainer = Resources.Load<InventoryUIItem>("UI/Item_Container");
        animator = GameObject.Find("TransferPanel").transform.GetComponent<Animator>();

        // Connect up our delegate function to the Event Handler that will distribute occurrences
        UIEventHandler.OnItemAddedToInventory += ItemAdded;
        UIEventHandler.OnItemAddedToPlayerInventory += ItemAddedPlayer;
        UIEventHandler.OnItemRemovedFromInventory += ItemRemoved;
        UIEventHandler.OnInventoryDisplayed += InventoryDisplay;

        InventoryDisplay(menuIsVisible);

        GameManager.instance.GiveItem(201);
        GameManager.instance.GiveItem(101);
    }

    private void InventoryDisplay(bool shouldShow)
    {
        menuIsVisible = shouldShow;
        playerInventoryPanel.SetActive(menuIsVisible);
        transferInventoryPanel.SetActive(menuIsVisible);
        itemDetailsInventoryPanel.SetActive(menuIsVisible);
    }

    // Update is called once per frame
    void Update ()
	{
        if (Input.GetKeyDown(KeyCode.I))
        {
            InventoryDisplay(!menuIsVisible);
        }

        GameObject uiTriggerGo = GameManager.instance.GetUITriggeringGO();
        var objectToTest = uiTriggerGo != null ? uiTriggerGo : GameObject.FindGameObjectWithTag("Player");
	    if (!IsTargetInRange(objectToTest))
	    {
            InventoryDisplay(false);
            //   menuIsVisible = false;

            //   animator.SetBool("TransferPanelOpen", menuIsVisible);
	        //transferContent.gameObject.SetActive(menuIsVisible);
            GameManager.instance.SetUITriggeringGO(null);
	    }
    }

    public void AddAllItems()
    {
        //Debug.Log("Attempting to perform add all items with count of allItemUis at " + allItemUis.Count);
        foreach (InventoryUIItem i in allItemUis)
        {
            if (i != null)
            {
                UIEventHandler.ItemAddedToPlayerInventory(i.GetItem());
            }
        }
    }
    private bool IsTargetInRange(GameObject target)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        float distanceToTarget = (target.transform.position - player.transform.position).magnitude;

        return distanceToTarget <= 2.0f;
    }

    // process an item being added to the transferPanel inventory
    void ItemAdded(Item item)
    {
        Debug.Log("Item Added to transfer Inv with id =" + item.nItemID);

        InventoryUIItem tempItem = CreateTemporaryUIItem(item);
        allItemUis.Add(tempItem);
        tempItem.transform.SetParent(transferContent);
    }

    // process an item being added to the player inventory
    void ItemAddedPlayer(Item item)
    {
        Debug.Log("Item Added to Player Inv with id =" + item.nItemID);

        InventoryUIItem tempItem = CreateTemporaryUIItem(item);
        allPlayerItemUis.Add(tempItem);
        tempItem.transform.SetParent(scrollViewContent);
        // TODO: make sure that the correct Item container is being cleared out.
        //InventoryUIItem uiItem = allItemUis.Find(x => x.GetItemID() == item.nItemID);
        //if (uiItem != null)
        //{
        //    allItemUis.Remove(uiItem);
        //    Destroy(uiItem);
        //}
    }

    void ItemRemoved(Item item)
    {
        // Can we find the item
        InventoryUIItem itemFound = allItemUis.Find(x => x.GetItemID() == itemContainer.GetItemID());
        if (itemFound != null)
        {
            if (allItemUis.Remove(itemFound))
            {
                Destroy(itemFound);
            }
            else
            {
                // TODO: "System failure error within 3 .. 2 .. 1 ..; Just Kidding, your system is fine."
                Debug.Log("Item was found but not able to be destroyed");
            }
        }
    }

    InventoryUIItem CreateTemporaryUIItem(Item item)
    {
        itemContainer.SetItem(item);
        InventoryUIItem emptyItem = Instantiate(itemContainer);
        emptyItem.SetItem(item);
        
        return emptyItem;
    }
}

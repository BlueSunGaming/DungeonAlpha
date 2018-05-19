using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour {
    public RectTransform panelInventory;
    public RectTransform scrollViewContent;

    InventoryUIItem itemContainer { get; set; }

    public RectTransform transferContent;
    public RectTransform viewportTransform;
    
    private Text ListOfItems;
    private GameManager gm;
    // TODO: Validate this is unnecessary private GameObject iw;
    List<InventoryUIItem> allItemUis = new List<InventoryUIItem>();
    //public InventoryItemUI itemSlot { get; set; }
    bool menuIsVisible { get; set; }

    Item currentSelection { get; set; }

    // Use this for initialization
    void Start ()
    {
        menuIsVisible = false;
        
        gm = GameManager.instance;
        itemContainer = Resources.Load<InventoryUIItem>("UI/Item_Container");

        // Connect up our delegate function to the Event Handler that will distribute occurrences
        UIEventHandler.OnItemAddedToInventory += ItemAdded;
        UIEventHandler.OnItemRemovedFromInventory += ItemRemoved;
        UIEventHandler.OnInventoryDisplayed += InventoryDisplay;

        scrollViewContent.gameObject.SetActive(menuIsVisible);
    }

    private void InventoryDisplay(bool shouldShow)
    {
        menuIsVisible = shouldShow;
        scrollViewContent.gameObject.SetActive(menuIsVisible);
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

    private bool IsTargetInRange(GameObject target)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        float distanceToTarget = (target.transform.position - player.transform.position).magnitude;

        return distanceToTarget <= 2.0f;
    }

    // process an item being added to the player inventory
    void ItemAdded(Item item)
    {
        Debug.Log("Everything is connected and we are instantiating an item with id =" + item.nItemID);
        itemContainer.SetItem(item);
        InventoryUIItem emptyItem = Instantiate(itemContainer);
        emptyItem.SetItem(item);
        allItemUis.Add(emptyItem);
        emptyItem.transform.SetParent(scrollViewContent);
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
}

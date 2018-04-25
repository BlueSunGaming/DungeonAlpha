using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour {
    public RectTransform inventoryContent;
    public RectTransform viewportTransform;

    private Text ListOfItems;
    private GameManager gm;
    // TODO: Validate this is unnecessary private GameObject iw;
    List<InventoryItemUI> allItemUis = new List<InventoryItemUI>();
    public InventoryItemUI itemSlot { get; set; }
    
    // Use this for initialization
    void Start ()
    {
        gm = Object.FindObjectOfType<GameManager>();
        itemSlot = Resources.Load<InventoryItemUI>("UI/ItemSlot");

        // Connect up our delegate function to the Event Handler that will distribute occurrences
        UIEventHandler.OnItemAddedToInventory += ItemAdded;
        UIEventHandler.OnItemRemovedFromInventory += ItemRemoved;

        if (gm && itemSlot)
        {
            foreach (Item i in gm.GetItems())
            {
                //ListOfItems.text += i.sName + " ";
                //InventoryItemUI emptyItem = Instantiate(itemSlot);
                //emptyItem.SetItem(i);
                //allItemUis.Add(emptyItem);
                //emptyItem.transform.SetParent(inventoryContent);
            }
        }
        else
        {
            Debug.Log("Could not locate Game ManagerByType or itemSlotByTag");
        }
    }
	
	// Update is called once per frame
	void Update ()
	{
		
	}

    // process an item being added to the player inventory
    void ItemAdded(Item item)
    {
        Debug.Log("Everything is connected and we are instantiating an item with id =" + item.nItemID);
        itemSlot.SetItem(item);
        InventoryItemUI emptyItem = Instantiate(itemSlot);
        emptyItem.SetItem(item);
        allItemUis.Add(emptyItem);
        emptyItem.transform.SetParent(inventoryContent);
    }

    void ItemRemoved(Item item)
    {
        // Can we find the item
        InventoryItemUI itemFound = allItemUis.Find(x => x.GetItemID() == itemSlot.GetItemID());
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

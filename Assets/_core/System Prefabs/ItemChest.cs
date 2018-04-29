using System;
using System.Collections;
using System.Collections.Generic;
using RPG.CameraUI;
using UnityEngine;
using _core;

public class ItemChest : MonoBehaviour, IInventory
{
    // Default Items to randomly generate in chest
    private int numItemsInChest = 4;

    [SerializeField] private int itemLayer;
    //_core.IInventory.invType; //=  InventoryType.CHEST;

    public RectTransform inventoryContent;
    public RectTransform viewportTransform;
    //RectTransform transferPanelTransform;

    CameraRaycaster cameraRaycaster;

    public InventoryItemUI itemSlot { get; set; }
    
    public List<Item> currentItems { get; set; }

    public InventoryType invType { get; set; }

    [SerializeField]
    [TextArea(3, 10)]
    public string[] Contents;

    // Use this for initialization
    void Start () {
        currentItems = new List<Item>();
        invType = InventoryType.CHEST;

        foreach (string itemID in Contents)
        {
            int x = 0;
            Int32.TryParse(itemID, out x);
            Item potentialItem = GameManager.instance.IsValidItem(x);
            if (potentialItem != null)
            {
                bool ItemWasAdded = AddItem(potentialItem);
                Debug.Log("Value of ItemWasAdded is " + ItemWasAdded + " for the id = " + x);
            }
            else
            {
                Debug.Log("No attempted add for the id = " + x);
            }
        }
        RegisterForMouseClick();
    }

    
    void OnMouseClick(RaycastHit raycastHit, int layerHit)
    {
        if (layerHit == itemLayer)
        {
            var itemChest = raycastHit.collider.gameObject;
            bool bIsInRange = IsTargetInRange(itemChest);
            if (IsTargetInRange(itemChest))
            {
                Debug.Log("Item Chest was in range");
                AddAllItems();
                //TriggerDialogue();
            }
        }
    }

    void AddAllItems()
    {

        Animator animator = GameObject.Find("TransferPanel").transform.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("TransferPanelOpen", true);
            Debug.Log("animator was found");
        }
        else
        {
            Debug.Log("animator was not found");
        }

        foreach (Item i in GetCurrentItems())
        {
            if (i != null)
            {
                UIEventHandler.ItemAddedToInventory(i);
            }
        }

        //itemSlot = Resources.Load<InventoryItemUI>("UI/ItemSlot");

        //    Debug.Log("We are instantiating an item with id =" + i.nItemID);
        //    itemSlot.SetItem(i);
        //    InventoryItemUI emptyItem = Instantiate(itemSlot);
        //    emptyItem.SetItem(i);
        //    //allItemUis.Add(emptyItem);
        //    emptyItem.transform.SetParent(inventoryContent);
        
    }

    private bool IsTargetInRange(GameObject target)
    {
        float distanceToTarget = (target.transform.position - transform.position).magnitude;

        return distanceToTarget <= 2.0f;
    }

    private void RegisterForMouseClick()
    {
        cameraRaycaster = FindObjectOfType<CameraRaycaster>();

        cameraRaycaster.notifyMouseClickObservers += OnMouseClick;
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void RandomPopulate()
    {
        throw new NotImplementedException();
    }

    public List<Item> GetCurrentItems()
    {
        return currentItems;
    }

    public bool AddItem(Item i)
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

    public bool RemoveItem(Item i)
    {
        bool removalSuccessful = false;
        if (DoesItemExist(i.nItemID))
        {
            currentItems.Remove(i);
        }
        return removalSuccessful;
    }

    public bool DoesItemExist(int itemID)
    {
        bool returnVal = false;
        // If the item is in the current inventory return true
        if (currentItems.Exists(x => x.nItemID == itemID))
        {
            returnVal = true;
        }

        return returnVal;
    }
}

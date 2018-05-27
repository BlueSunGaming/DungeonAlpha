using System;
using System.Collections;
using System.Collections.Generic;
using DungeonRPG.CameraUI;
using UnityEngine;
using _core;
using DungeonRPG;

public class ItemChest : Interactable //MonoBehaviour, IInventory
{
    // Default Items to randomly generate in chest
    private int numItemsInChest = 4;

    [SerializeField]
    private GameObject chestObject;
    [SerializeField] private int itemLayer;

    //CameraRaycaster cameraRaycaster;
    //private Animator mTransferPanelAnimator;

    public InventoryUIItem itemSlot { get; set; }
    
    public List<Item> currentItems { get; set; }

    public InventoryType invType { get; set; }

    [SerializeField]
    [TextArea(3, 10)]
    public string[] Contents;

    // Use this for initialization
    void Start () {
        currentItems = new List<Item>();
        invType = InventoryType.CHEST;

        //mTransferPanelAnimator = GameObject.Find("TransferPanel").transform.GetComponent<Animator>();
        //inventoryContent = (RectTransform)GameObject.FindGameObjectWithTag("TransferContent").transform;

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
        //RegisterForMouseClick();
    }

    public Item ItemDrop { get; set; }
    public override void Interact()
    {
        GameManager.instance.SetUITriggeringGO(chestObject);
        AddAllItems();
        //InventoryController.Instance.GiveItem(ItemDrop);
        Destroy(gameObject);
    }



    //void OnMouseClick(RaycastHit raycastHit, int layerHit)
    //{
    //    if (layerHit == itemLayer)
    //    {
    //        var itemChest = raycastHit.collider.gameObject;
    //        if (IsTargetInRange(itemChest))
    //        {
    //            GameManager.instance.SetUITriggeringGO(itemChest);
    //            Debug.Log("Item Chest was in range");
    //            AddAllItems();
    //        }
    //    }
    //}

    public void AddAllItems()
    {
        foreach (Item i in GetCurrentItems())
        {
            if (i != null)
            {
                UIEventHandler.ItemAddedToInventory(i);
            }
        }
    }

    private bool IsTargetInRange(GameObject target)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        float distanceToTarget = (target.transform.position - player.transform.position).magnitude;

        return distanceToTarget <= 2.0f;
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

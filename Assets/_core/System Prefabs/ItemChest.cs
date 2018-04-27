using System;
using System.Collections;
using System.Collections.Generic;
using RPG.CameraUI;
using UnityEngine;


public class ItemChest : _core.IInventory
{
    // Default Items to randomly generate in chest
    private int numItemsInChest = 4;
    //_core.IInventory.invType; //=  InventoryType.CHEST;

    public RectTransform inventoryContent;
    public RectTransform viewportTransform;
    //RectTransform transferPanelTransform;

    CameraRaycaster cameraRaycaster;

    public InventoryItemUI itemSlot { get; set; }

    [SerializeField]
    [TextArea(3, 10)]
    public string[] Contents;

    // Use this for initialization
    void Start () {
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
        if (layerHit == CursorAffordance.nItemLayer)
        {
            var itemChest = raycastHit.collider.gameObject;

            if (IsTargetInRange(itemChest))
            {
                // 
                AddAllItems();
                //TriggerDialogue();
            }
        }
    }

    void AddAllItems()
    {
        itemSlot = Resources.Load<InventoryItemUI>("UI/ItemSlot");
        foreach (Item i in GetCurrentItems())
        {
            Debug.Log("We are instantiating an item with id =" + i.nItemID);
            itemSlot.SetItem(i);
            InventoryItemUI emptyItem = Instantiate(itemSlot);
            emptyItem.SetItem(i);
            //allItemUis.Add(emptyItem);
            emptyItem.transform.SetParent(inventoryContent);
        }
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour {
    public RectTransform transferContent;
    public RectTransform viewportTransform;

    public Animator animator;

    private Text ListOfItems;
    private GameManager gm;
    // TODO: Validate this is unnecessary private GameObject iw;
    List<InventoryItemUI> allItemUis = new List<InventoryItemUI>();
    public InventoryItemUI itemSlot { get; set; }
    bool menuIsVisible { get; set; }

    Item currentSelection { get; set; }

    // Use this for initialization
    void Start ()
    {
        menuIsVisible = false;
        animator = GameObject.Find("TransferPanel").transform.GetComponent<Animator>();
        if (animator)
        {
            Debug.Log("animator was found");
        }
        animator.SetBool("TransferPanelOpen", menuIsVisible);
        gm = GameManager.instance;
        itemSlot = Resources.Load<InventoryItemUI>("UI/ItemSlot");

        // Connect up our delegate function to the Event Handler that will distribute occurrences
        UIEventHandler.OnItemAddedToInventory += ItemAdded;
        UIEventHandler.OnItemRemovedFromInventory += ItemRemoved;

        transferContent.gameObject.SetActive(menuIsVisible);
    }
	
	// Update is called once per frame
	void Update ()
	{
        if (Input.GetKeyDown(KeyCode.I))
        {
            menuIsVisible = !menuIsVisible;
            animator.SetBool("TransferPanelOpen", menuIsVisible);
            transferContent.gameObject.SetActive(menuIsVisible);
        }

	    if (animator.GetBool("TransferPanelOpen"))
	    {
	        GameObject uiTriggerGo = GameManager.instance.GetUITriggeringGO();
            var objectToTest = uiTriggerGo != null ? uiTriggerGo : GameObject.FindGameObjectWithTag("Player");
	        if (!IsTargetInRange(objectToTest))
	        {
	            menuIsVisible = false;

                animator.SetBool("TransferPanelOpen", menuIsVisible);
	            transferContent.gameObject.SetActive(menuIsVisible);
                GameManager.instance.SetUITriggeringGO(null);
	        }
	    }
    }

    private bool IsTargetInRange(GameObject target)
    {
        //   this.transform.position;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        float distanceToTarget = (target.transform.position - player.transform.position).magnitude;

        return distanceToTarget <= 2.0f;
    }

    // process an item being added to the player inventory
    void ItemAdded(Item item)
    {
        Debug.Log("Everything is connected and we are instantiating an item with id =" + item.nItemID);
        itemSlot.SetItem(item);
        InventoryItemUI emptyItem = Instantiate(itemSlot);
        emptyItem.SetItem(item);
        allItemUis.Add(emptyItem);
        emptyItem.transform.SetParent(transferContent);
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

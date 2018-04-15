using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour {
    public RectTransform inventoryContent;
    public RectTransform viewportTransform;

    private Text ListOfItems;
    private GameManager gm;
    private GameObject iw;
    List<InventoryItemUI> allItemUis = new List<InventoryItemUI>();
    // 
    public InventoryItemUI itemSlot { get; set; }
    

    // Use this for initialization
    void Start ()
    {
        gm = Object.FindObjectOfType<GameManager>();
        //iw = GameObject.FindGameObjectWithTag("ItemWindow");
        itemSlot = Resources.Load<InventoryItemUI>("UI/ItemSlot");
        if (gm) // iw)
        {
            foreach (Item i in gm.GetItems())
            {
                //ListOfItems.text += i.sName + " ";
                InventoryItemUI emptyItem = Instantiate(itemSlot);
                emptyItem.SetItem(i);
                allItemUis.Add(emptyItem);
                emptyItem.transform.SetParent(inventoryContent);
            }
        }
        else
        {
            Debug.Log("Could not locate Game ManagerByType or ItemWindowByTag");
        }
    }
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}

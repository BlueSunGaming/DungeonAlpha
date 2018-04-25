using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    private Item item;

    public Text nameText;
    public Text descText;
    public Sprite itemImg;

    public int GetItemID()
    {
        return item.nItemID;
    }

    public void SetItem(Item inItem)
    {
        nameText.text = inItem.sName;
        descText.text = inItem.sDescription;

        if (inItem.sItemIcon != "")
        {
            itemImg = Resources.Load<Sprite>("UI/Inventory/Inventory Icons/" + inItem.sItemIcon);
        }
        
        if (itemImg != null)
        {
            Debug.Log("Value of itemImg name is " + itemImg.name);
        }
        else
        {
            Debug.Log("itemImg is null");
        }
        item = inItem;
    }
}

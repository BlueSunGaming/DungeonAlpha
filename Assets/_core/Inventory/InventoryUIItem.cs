﻿using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIItem : MonoBehaviour {

    private Item item;

    //public Text nameText;
    //public Text descText;
    //public Sprite itemImgSprite;

    public int GetItemID()
    {
        return item.nItemID;
    }

    public void SetItem(Item inItem)
    {
        this.transform.Find("Item_Name").GetComponent<Text>().text = inItem.sName;
        //this.transform.Find("Item_Description").GetComponent<Text>().text = inItem.sDescription;

        if (inItem.sItemIcon != "")
        {
            // Utilize System.IO.Path method to strip the file type extension
            string newPath = Path.ChangeExtension(inItem.sItemIcon, null);
            //itemImgSprite = Resources.Load<Sprite>("UI/Inventory/InventoryIcons/" + newPath);
            //this.transform.Find("Item_Icon").
        }

        item = inItem;
    }

    public void OnSelectItemButton()
    {
        Debug.Log("Hey It worked");
    }
}

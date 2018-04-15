using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    private Item item;

    public Text nameText;
    public Text descText;
    public Image itemImg;

    public void SetItem(Item inItem)
    {
        nameText.text = inItem.sName;
        descText.text = inItem.sDescription;
        item = inItem;
    }

}

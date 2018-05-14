using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemDetails : MonoBehaviour {

    Item item;
    Button selectedItemButton, itemInteractButton;
    Text itemNameText, itemDescriptionText, itemInteractButtonText, statText;
    void Start()
    {
        itemNameText = transform.Find("Item_Name").GetComponent<Text>();
        itemDescriptionText = transform.Find("Item_Description").GetComponent<Text>();
        itemInteractButton = transform.GetComponentInChildren<Button>();
        itemInteractButtonText = itemInteractButton.GetComponentInChildren<Text>();
        gameObject.SetActive(false);
    }

    public void SetItem(Item item, Button selectedButton)
    {
        gameObject.SetActive(true);
        //statText.text = "";
        // TODO: Setup Stats on item
        //if (item.Stats != null)
        //{
        //    foreach (BaseStat stat in item.Stats)
        //    {
        //        statText.text += stat.StatName + ": " + stat.BaseValue + "\n";
        //    }
        //}
        itemInteractButton.onClick.RemoveAllListeners();
        this.item = item;
        selectedItemButton = selectedButton;
        itemNameText.text = item.sName;
        itemDescriptionText.text = item.sDescription;
        itemInteractButtonText.text = item.ActionName;
        itemInteractButton.onClick.AddListener(OnItemInteract);
    }

    public void OnItemInteract()
    {
        if (item.ItemType == Item.ItemTypes.Consumable)
        {
            GameManager.instance.ConsumeItem(item);
            Destroy(selectedItemButton.gameObject);
        }
        else if (item.ItemType == Item.ItemTypes.Weapon)
        {
            GameManager.instance.EquipItem(item);
            Destroy(selectedItemButton.gameObject);
        }
        item = null;
        gameObject.SetActive(false);
    }
}


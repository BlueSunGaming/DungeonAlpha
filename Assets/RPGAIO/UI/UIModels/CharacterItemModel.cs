using System;
using System.Linq;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterItemModel : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image ItemImage;
    public string SlotName;
    public string ItemID;
    public bool EmptySlot;

    public void Init(Item item)
    {
        if(item == null)
        {
            EmptySlot = true; 
            ItemImage.sprite = null;
            ItemImage.color = Color.clear;
            return;
        }

        var sprite = GeneralMethods.CreateSprite(item.Image);

        ItemImage.sprite = sprite;
        ItemID = item.ID;
        ItemImage.color = Color.white;
        EmptySlot = false;
    }

    public void EmptyThisSlot()
    {
        EmptySlot = true;
        ItemImage.sprite = null;
        ItemImage.color = Color.clear;
    }

    public void UnequipItem()
    {
        if (EmptySlot) return;

        if(GetObject.PlayerCharacter.Equipment.UnEquipItem(ItemID))
        {
            UIHandler.Instance.InventoryUI.UpdateItemContainer();
            Debug.Log("Unequipped item!");
        }     
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            UnequipItem();
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Clear();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EmptySlot) return;

        TooltipUI.Set(GetObject.PlayerCharacter.Equipment.AllEquippedItems.First(i => i.ID == ItemID));
    }
}

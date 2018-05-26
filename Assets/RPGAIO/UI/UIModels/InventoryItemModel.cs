using System.Linq;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemModel : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image ItemImage;
    public Image ItemImageForDrag;
    public SkillDragHandler DragHandler;
    public Text ItemQuantity;
    public string ItemID;
    public bool EmptySlot;

    public void Init(Item item)
    {
        if(item == null)
        {
            EmptySlot = true; 
            ItemImage.sprite = null;
            ItemImageForDrag.sprite = null;
            ItemImage.color = Color.clear;
            DragHandler.IsSkill = false;
            DragHandler.RefId = "";
            ItemQuantity.text = "";
            return;
        }

        var sprite = GeneralMethods.CreateSprite(item.Image);
        var stackable = item as IStackable;

        ItemImage.sprite = sprite;
        ItemImageForDrag.sprite = sprite;
        DragHandler.IsSkill = false;
        DragHandler.RefId = item.InventoryRefID;
        ItemQuantity.text = stackable != null ? stackable.CurrentStacks.ToString() : "";
        ItemID = item.InventoryRefID;
        ItemImage.color = Color.white;
        EmptySlot = false;
    }

    public void UseItem()
    {
        if (EmptySlot) return;

        if(!UIHandler.Instance.VendorUI.Show)
            GetObject.PlayerCharacter.Inventory.UseItemByRef(ItemID);
        else
            UIHandler.Instance.VendorUI.VendorBuyFromPlayer(ItemID);
        //Debug.Log("Used item!");
    }

    public void DropItem()
    {
        if (EmptySlot) return;

        GetObject.PlayerCharacter.Inventory.DropItemByRef(ItemID);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            UseItem();
        else if (eventData.button == PointerEventData.InputButton.Right)
            DropItem();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Clear();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipUI.Set(GetObject.PlayerCharacter.Inventory.GetReferencedItem(ItemID));
    }
}

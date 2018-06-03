using System.Linq;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VendorItemModel : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image ItemImage;
    public Text ItemName;
    public Text ItemQuantity;
    public Text BuyBack;
    public Text Cost;
    public string VendorItemRef;
    private Item referencedItem;
    private VendorShopItem vendorShopItem;

    public void Init(VendorShopItem item)
    {
        vendorShopItem = item;
        if(item == null)
        {
            ItemImage.sprite = null;
            ItemImage.color = Color.clear;
            ItemQuantity.text = "";
            ItemName.text = "";
            return;
        }

        referencedItem = GetReferencedItem(vendorShopItem.ItemID);
        var sprite = GeneralMethods.CreateSprite(referencedItem.Image);
        BuyBack.gameObject.SetActive(false);
        ItemImage.sprite = sprite;
        Cost.text = referencedItem.BuyValue.ToString();
        ItemQuantity.text = "x" + item.QuantityRemaining.ToString();
        ItemName.text = referencedItem.Name;
        VendorItemRef = item.VendorItemRef;
        if (vendorShopItem.InfiniteStock)
        {
            ItemQuantity.text = VendorUI.Infinity;
        }
        ItemImage.color = Color.white;
    }

    public void InitBuyBack(Item item)
    {
        BuyBack.gameObject.SetActive(true);

        referencedItem = item;
        var sprite = GeneralMethods.CreateSprite(referencedItem.Image);

        ItemImage.sprite = sprite;
        var stackable = item as IStackable;
        Cost.text = referencedItem.BuyValue.ToString();
        ItemQuantity.text = stackable != null ? "x" + stackable.CurrentStacks.ToString() : "x1";
        ItemImage.color = Color.white;
    }

    private Item GetReferencedItem(string itemId)
    {
        return Rm_RPGHandler.Instance.Repositories.Items.Get(itemId) ??
            Rm_RPGHandler.Instance.Repositories.CraftableItems.Get(itemId) ??
            Rm_RPGHandler.Instance.Repositories.QuestItems.Get(itemId);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            SelectItem();
    }

    private void SelectItem()
    {
        VendorUI.Instance.SelectItem(vendorShopItem,referencedItem);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Clear();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipUI.Set(referencedItem);
    }
}
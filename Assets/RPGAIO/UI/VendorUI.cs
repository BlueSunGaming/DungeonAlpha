using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class VendorUI : MonoBehaviour
{
    public static VendorUI Instance;
    public GameObject ItemContainer;
    public Text GoldAmount;
    public Text VendorShopTitle;
    public GameObject VendorItemUIPrefab;
    public GameObject BuyBackSepeatorPrefab;

    public Text PurchaseText;
    public InputField PurchaseQuantity;
    public Image ItemIcon;
    public Button QuantityPlusButton;
    public Button QuantityMinusButton;
    private int curQuantity;

    private VendorShop selectedVendorShop;
    private VendorShopItem selectedVendorItem;
    private Item selectedRefItem;

    private Vector3 PosOnOpen;
    private Transform playerTransform;
    public bool Show;
    public const string Infinity = "∞";

    private EventSystem EventSystem
    {
        get { return UIHandler.Instance.EventSystem; }
    }

	// Use this for initialization
	void Awake () {
	    Instance = this;
        //check null playmonogameobj
	    var p = GetObject.PlayerMonoGameObject;
        if(p != null)
	        playerTransform = p.transform;
        PurchaseQuantity.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        ItemIcon.CrossFadeAlpha(0, 0, true);
    }

    private void ValueChangeCheck()
    {
        if (selectedVendorItem == null)
        {
            PurchaseQuantity.text = "";
            return;
        }

        var value = PurchaseQuantity.text;
        var result = -1;
        bool invalid = !Int32.TryParse(value, out result);

        if(result <= 0)
        {
            invalid = true;
        }

        if (selectedVendorItem.QuantityRemaining < result && !selectedVendorItem.InfiniteStock)
        {
            invalid = true;
        }

        if(invalid)
        {
            PurchaseQuantity.text = "1";
            curQuantity = 1;
            PurchaseText.text = string.Format("Purchase {0}x {1}", curQuantity, selectedRefItem.Name);
        }
        else
        {
            curQuantity = result;
            PurchaseQuantity.text = curQuantity.ToString();
            PurchaseText.text = string.Format("Purchase {0}x {1}", curQuantity, selectedRefItem.Name);
        }
    }

    public VendorShop GetShop(string id)
    {
        return GetObject.PlayerSave.GamePersistence.VendorInventories.FirstOrDefault(s => s.ID == id);
    }

    void Update()
    {
        var playerChar = GetObject.PlayerCharacter;
        GoldAmount.text = playerChar.Inventory.Gold.ToString("N0");
        var dist = Vector3.Distance(PosOnOpen, playerTransform.position);
        if(dist > 3) CloseVendorWindow();
    }

    public void ShowVendorWindow(VendorShop shop)
    {
        Show = !Show;
        SelectItem(null,null);
        if(Show)
        {
            PosOnOpen = GetObject.PlayerMonoGameObject.transform.position;
            UpdateItemContainer(shop);
        }
    }

    public void CloseVendorWindow()
    {
        Show = false;
    }

    public void QuantityPlus()
    {
        curQuantity++;
        QuantityMinusButton.interactable = true;
        if (selectedVendorItem.QuantityRemaining <= curQuantity && !selectedVendorItem.InfiniteStock)
        {
            QuantityPlusButton.interactable = false;
        }
        UpdatePurchaseText();
    }

    public void QuantityMinus()
    {
        curQuantity--;
        QuantityPlusButton.interactable = true;
        if (curQuantity == 1)
        {
            QuantityMinusButton.interactable = false;
        }
        UpdatePurchaseText();
    }

    private void UpdatePurchaseText()
    {
        if(selectedRefItem != null)
            PurchaseText.text = string.Format("Purchase {0}x {1}", curQuantity, selectedRefItem.Name);
        PurchaseQuantity.text = curQuantity.ToString();
    }
    public void SelectItem(VendorShopItem item, Item refItem)
    {
        ItemIcon.CrossFadeAlpha(1,0,true);
        if(item == null)
        {
            PurchaseText.text = "Select an item to purchase";
            PurchaseQuantity.text = "0";
            ItemIcon.sprite = null;
            ItemIcon.CrossFadeAlpha(0, 0, true);
            return;
        }

        selectedVendorItem = item;
        selectedRefItem = refItem;
        QuantityPlusButton.interactable = (refItem as IStackable) != null && item.QuantityRemaining > 1;
        if(selectedVendorItem.InfiniteStock)
        {
            QuantityPlusButton.interactable = true;
        }
        QuantityMinusButton.interactable = false;

        curQuantity = 1;
        PurchaseText.text = string.Format("Purchase {0}x {1}","1",refItem.Name);
        PurchaseQuantity.text = "1";
        ItemIcon.sprite = GeneralMethods.CreateSprite(refItem.Image);
    }

    public void PlayerBuyItem()
    {
        var player = GetObject.PlayerCharacter;
        var isBuyBack = GetObject.PlayerSave.GamePersistence.BuyBackItems.Contains(selectedRefItem);
        var itemId = isBuyBack ? selectedRefItem.ID : selectedVendorItem.ItemID;
        var itemBeingBought = Rm_RPGHandler.Instance.Repositories.Items.Get(itemId);
        itemBeingBought = GeneralMethods.CopyObject(itemBeingBought);
        var singleItemValue = isBuyBack ? itemBeingBought.SellValue : itemBeingBought.BuyValue;
        var cost = singleItemValue * curQuantity;

        if (!(player.Inventory.Gold - cost >= 0)) return;

        var stackable = itemBeingBought as IStackable;
        if(stackable != null)
        {
            stackable.CurrentStacks = curQuantity;
        }

        var sold = player.Inventory.AddItem(itemBeingBought);
        if (sold)
        {
            player.Inventory.Gold -= cost;
            var vendorShopItem = selectedVendorShop.VendorShopItems.First(v => v.ItemID == selectedRefItem.ID);

            if (!vendorShopItem.InfiniteStock)
                vendorShopItem.QuantityRemaining -= curQuantity;

            if (vendorShopItem.QuantityRemaining == 0)
            {
                selectedVendorShop.VendorShopItems.Remove(vendorShopItem);
                SelectItem(null,null);
            }
        }

        UpdateItemContainer(selectedVendorShop);
    }

    public void VendorBuyFromPlayer(string itemRefID)
    {
        var player = GetObject.PlayerCharacter;
        var item = player.Inventory.GetReferencedItem(itemRefID);
        var stackable = item as IStackable;
        if (!item.CanBeDropped) return;
        selectedVendorShop.VendorShopItems.Add(new VendorShopItem()
        {
            ItemID = item.ID,
            InfiniteStock = false,
            QuantityRemaining = stackable != null ? stackable.CurrentStacks : 1
        });
        player.Inventory.RemoveItem(item);
        var profit = stackable != null ? item.SellValue*stackable.CurrentStacks : item.SellValue;
        player.Inventory.Gold += profit;

        UpdateItemContainer(selectedVendorShop);
    }

    public void UpdateItemContainer(VendorShop shop)
    {
        selectedVendorShop = shop;
        curQuantity = 1;
        VendorShopTitle.text = shop.Name;
        QuantityPlusButton.interactable = (selectedRefItem as IStackable) != null && selectedVendorItem.QuantityRemaining > 1;
        QuantityMinusButton.interactable = false;
        UpdatePurchaseText();
        if (!Show) return;
        ItemContainer.transform.DestroyChildren();
        //GameObject firstButton = null;

        for (int i = 0; i < shop.VendorShopItems.Count; i++)
        {
            var vendorItem = shop.VendorShopItems[i];
            var go = Instantiate(VendorItemUIPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            go.transform.SetParent(ItemContainer.transform, false);
            var itemModel = go.GetComponent<VendorItemModel>();
            itemModel.Init(vendorItem);
            //firstButton = firstButton ?? go;

        }

        //var buyback = Instantiate(BuyBackSepeatorPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        //buyback.transform.SetParent(ItemContainer.transform, false);
        //
        //for (int i = 0; i < GetObject.PlayerSave.GamePersistence.BuyBackItems.Count; i++)
        //{
        //    var buyBackItem = GetObject.PlayerSave.GamePersistence.BuyBackItems[i];
        //    var go = Instantiate(VendorItemUIPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        //    go.transform.SetParent(ItemContainer.transform, false);
        //    var itemModel = go.GetComponent<VendorItemModel>();
        //    itemModel.InitBuyBack(buyBackItem);
        //    //firstButton = firstButton ?? go;
        //
        //}
        //EventSystem.SetSelectedGameObject(firstButton);
    }
}
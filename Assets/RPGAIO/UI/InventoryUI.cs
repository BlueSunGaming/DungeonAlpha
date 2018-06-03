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

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;
    public GameObject ItemContainer;
    public Text GoldAmount;
    public Text InventoryStatus;
    public GameObject ItemUIPrefab;
    public bool Show;

    private EventSystem EventSystem
    {
        get { return UIHandler.Instance.EventSystem; }
    }

	// Use this for initialization
	void Awake () {
	    Instance = this;
	}

    void OnEnable()
    {
        RPG.Events.InventoryUpdated += UpdateItemContainer;
    }

    void OnDisable()
    {
        RPG.Events.InventoryUpdated -= UpdateItemContainer;
    }

    void Update()
    {
        var playerChar = GetObject.PlayerCharacter;
        GoldAmount.text = playerChar.Inventory.Gold.ToString("N0");
        InventoryStatus.text = "";
        InventoryStatus.text += Rm_RPGHandler.Instance.Items.InventoryHasMaxItems ? string.Format("Slots: {0}/{1} ", playerChar.Inventory.AllItems.Count, playerChar.Inventory.MaxItems) : "";
        InventoryStatus.text += Rm_RPGHandler.Instance.Items.InventoryUsesWeightSystem ? string.Format("Weight: {0}/{1}", playerChar.Inventory.CurrentWeight, playerChar.Inventory.MaxWeight) : "";
    }

    public void ToggleInventory()
    {
        Show = !Show;
        if(Show)
        {
            UpdateItemContainer();
        }
    }

    public void CloseInventory()
    {
        Show = false;
    }

    private void UpdateItemContainer(object sender, RPGEvents.InventoryUpdateEventArgs e)
    {
        UpdateItemContainer();
    }

    public void UpdateItemContainer()
    {
        if (!Show) return;

        //DialogModel.NpcImage.sprite = GeneralMethods.CreateSprite(DialogHandler.DialogNpc.GetImage());
        var playerChar = GetObject.PlayerCharacter;
        ItemContainer.transform.DestroyChildren();
        GameObject firstButton = null;

        for (int i = 0; i < playerChar.Inventory.AllItems.Count; i++)
        {
            var item = playerChar.Inventory.AllItems[i];
            

            var go = Instantiate(ItemUIPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            go.transform.SetParent(ItemContainer.transform, false);
            var itemModel = go.GetComponent<InventoryItemModel>();
            itemModel.Init(item);
            firstButton = firstButton ?? go;

        }
        EventSystem.SetSelectedGameObject(firstButton);

        if (Rm_RPGHandler.Instance.Items.InventoryHasMaxItems && (playerChar.Inventory.AllItems.Count < playerChar.Inventory.MaxItems))
        {
            //Spawn extra empty slots
            var slotsToAdd = playerChar.Inventory.MaxItems - playerChar.Inventory.AllItems.Count;
            for (int i = 0; i < slotsToAdd; i++)
            {
                var go = Instantiate(ItemUIPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                go.transform.SetParent(ItemContainer.transform, false);
                var itemModel = go.GetComponent<InventoryItemModel>();
                itemModel.Init(null);
            }
        }
        else if (!Rm_RPGHandler.Instance.Items.InventoryHasMaxItems)
        {
            if (playerChar.Inventory.AllItems.Count < 25)
            {
                var slotsToAdd = 25 - playerChar.Inventory.AllItems.Count;
                for (int i = 0; i < slotsToAdd; i++)
                {
                    var go = Instantiate(ItemUIPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    go.transform.SetParent(ItemContainer.transform, false);
                    var itemModel = go.GetComponent<InventoryItemModel>();
                    itemModel.Init(null);
                }
            }
            else
            {
                var slots = playerChar.Inventory.AllItems.Count.RoundToNearest(5);
                var slotsToAdd = slots - playerChar.Inventory.AllItems.Count;
                for (int i = 0; i < slotsToAdd; i++)
                {
                    var go = Instantiate(ItemUIPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    go.transform.SetParent(ItemContainer.transform, false);
                    var itemModel = go.GetComponent<InventoryItemModel>();
                    itemModel.Init(null);
                }
            }
        }

        //Debug.Log("Update inventory!");
    }
}
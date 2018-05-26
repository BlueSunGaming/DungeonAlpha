using System;
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

public class CraftingUI : MonoBehaviour
{
    public static CraftingUI Instance;
    public GameObject CraftableItemContainer;
    public GameObject CraftingMaterialsContainer;
    public GameObject CraftingMaterialPrefab;
    public GameObject CraftableItemPrefab;
    public GameObject CraftableCategoryPrefab;
    public Button CraftItemButton;

    public Item SelectedCraftableItem;
    public Image SelectedCraftItemImage;
    public Text SelectedCraftItemName;
    public Image CraftingProgressBar;
    public bool Show;
    public bool CurrentlyCrafting;
    public bool LoadedCraftableItems;
    public bool OpenedByDialog;

    private Vector3 PosOnOpen;
    private Transform playerTransform;

    private EventSystem EventSystem
    {
        get { return UIHandler.Instance.EventSystem; }
    }

    private List<CraftSlotScaling> _slotScaling
    {
        get { return Rm_RPGHandler.Instance.Items.CraftSlotScalings; }
    }

	// Use this for initialization
	void Awake () {
	    Instance = this;
        CraftingMaterialsContainer.transform.DestroyChildren();
	    SelectedCraftItemImage.color = Color.clear;

        var p = GetObject.PlayerMonoGameObject;
        if (p != null)
            playerTransform = p.transform;
	}

    void OnEnable()
    {
        RPG.Events.InventoryUpdated += UpdateCraftLists;
    }
    void OnDisable()
    {
        RPG.Events.InventoryUpdated -= UpdateCraftLists;
    }

    private void UpdateCraftLists(object sender, RPGEvents.InventoryUpdateEventArgs e)
    {
        if(SelectedCraftableItem != null)
            UpdateCraftLists(SelectedCraftableItem.ID);
    }

    private void UpdateCraftLists(string craftableItemId)
    {
        SelectedCraftItemImage.color = Color.white;

        var craftableItem = Rm_RPGHandler.Instance.Repositories.CraftableItems.Get(craftableItemId);
        SelectedCraftableItem = craftableItem;
        var sprite = GeneralMethods.CreateSprite(craftableItem.Image);
        SelectedCraftItemImage.sprite = sprite;
        SelectedCraftItemName.text = craftableItem.Name;

        //materials required
        CraftingMaterialsContainer.transform.DestroyChildren();
        var materialList = Rm_RPGHandler.Instance.Repositories.CraftLists.GetCraftList(craftableItem);

        var scaling = 1;
        if (Rm_RPGHandler.Instance.Items.ScaleCraftList)
        {
            if (craftableItem.ItemType == ItemType.Apparel)
            {
                var apparel = craftableItem as Apparel;
                scaling = _slotScaling.First(s => s.SlotIdentifier == apparel.apparelSlotID).Multiplier;
            }
            else if (craftableItem.ItemType == ItemType.Weapon)
            {
                scaling = _slotScaling.First(s => s.SlotIdentifier == "Weapon").Multiplier;
            }
        }

        foreach (var item in materialList)
        {
            var stackable = item as IStackable;
            var numberRequired = 1;
            if (stackable != null)
            {
                stackable.CurrentStacks *= scaling;
                numberRequired = stackable.CurrentStacks;
            }

            var go = Instantiate(CraftingMaterialPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            go.transform.SetParent(CraftingMaterialsContainer.transform, false);
            var itemModel = go.GetComponent<CraftingMaterialModel>();
            itemModel.Init(item, numberRequired);
        }

        CraftItemButton.interactable = CraftHandler.Instance.CanCraft(craftableItem);
    }

    void Update()
    {
        if(CurrentlyCrafting && CraftingProgressBar.fillAmount < 0.99f)
        {
            var craftTime = Rm_RPGHandler.Instance.Items.CraftTime;
            CraftingProgressBar.fillAmount += 1 * (Time.deltaTime / craftTime);

            if(CraftingProgressBar.fillAmount >= 0.99f)
            {
                CurrentlyCrafting = false;
                UpdateCraftLists(SelectedCraftableItem.ID);
                CraftingProgressBar.fillAmount = 1.0f;  
            }
        }


        var dist = Vector3.Distance(PosOnOpen, playerTransform.position);
        if (dist > 3 && OpenedByDialog) CloseCraftingWindow();
    }

    public void SelectCraftableItem(string craftableItemId)
    {
        UpdateCraftLists(craftableItemId);
    }

    public void CraftSelectedItem()
    {
        if (SelectedCraftableItem == null || CurrentlyCrafting) return;

        if (CraftHandler.Instance.CanCraft(SelectedCraftableItem))
        {
            StartCraftingProgressBar();
            CraftHandler.Instance.CraftItem(SelectedCraftableItem);
        }
    }

    private void StartCraftingProgressBar()
    {
        CurrentlyCrafting = true;
        CraftingProgressBar.fillAmount = 0;
    }

    public void ToggleCraftingUI()
    {
        Show = !Show;
        if(Show)
        {
            PosOnOpen = GetObject.PlayerMonoGameObject.transform.position;

            if(!LoadedCraftableItems)
            {
                LoadCraftableItems();
            }

            SelectedCraftableItem = null;
            SelectedCraftItemName.text = "- Select an Item to Craft -";
            SelectedCraftItemImage.color = Color.clear;
            CraftingMaterialsContainer.transform.DestroyChildren();

        }
        else
        {
            OpenedByDialog = false;
        }
    }

    private void LoadCraftableItems()
    {
        var allCraftableItems = Rm_RPGHandler.Instance.Repositories.CraftableItems.AllItems;
        CraftableItemContainer.transform.DestroyChildren();

        //Weapons
        var craftableWeapons = allCraftableItems.Where(c => c.ItemType == ItemType.Weapon).ToList();
        if(craftableWeapons.Any())
        {
            var go = Instantiate(CraftableCategoryPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            go.transform.SetParent(CraftableItemContainer.transform, false);
            var categoryName = go.GetComponent<Text>();
            categoryName.text = "- Weapons -";

            foreach (var item in craftableWeapons)
            {
                var go1 = Instantiate(CraftableItemPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                go1.transform.SetParent(CraftableItemContainer.transform, false);
                var itemModel = go1.GetComponent<CraftableItemModel>();
                itemModel.Init(item);
            }
        }
        //Offhand

        var craftableOffhands = allCraftableItems.Where(c => c.ItemType == ItemType.Apparel && ((Apparel)c).ApparelSlotName == "OffHand").ToList();
        if (craftableOffhands.Any())
        {
            var go = Instantiate(CraftableCategoryPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            go.transform.SetParent(CraftableItemContainer.transform, false);
            var categoryName = go.GetComponent<Text>();
            categoryName.text = "- OffHands -";

            foreach (var item in craftableOffhands)
            {
                var go1 = Instantiate(CraftableItemPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                go1.transform.SetParent(CraftableItemContainer.transform, false);
                var itemModel = go1.GetComponent<CraftableItemModel>();
                itemModel.Init(item);
            }
        }
        //Consumable

        var craftableConsumables = allCraftableItems.Where(c => c.ItemType == ItemType.Consumable).ToList();
        if (craftableConsumables.Any())
        {
            var go = Instantiate(CraftableCategoryPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            go.transform.SetParent(CraftableItemContainer.transform, false);
            var categoryName = go.GetComponent<Text>();
            categoryName.text = "- Consumables -";

            foreach (var item in craftableConsumables)
            {
                var go1 = Instantiate(CraftableItemPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                go1.transform.SetParent(CraftableItemContainer.transform, false);
                var itemModel = go1.GetComponent<CraftableItemModel>();
                itemModel.Init(item);
            }
        }
        
        //Material
        var craftableMaterials = allCraftableItems.Where(c => c.ItemType == ItemType.Material).ToList();
        if (craftableMaterials.Any())
        {
            var go = Instantiate(CraftableCategoryPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            go.transform.SetParent(CraftableItemContainer.transform, false);
            var categoryName = go.GetComponent<Text>();
            categoryName.text = "- Consumables -";

            foreach (var item in craftableMaterials)
            {
                var go1 = Instantiate(CraftableItemPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                go1.transform.SetParent(CraftableItemContainer.transform, false);
                var itemModel = go1.GetComponent<CraftableItemModel>();
                itemModel.Init(item);
            }
        }

        //Book
        var craftableBooks = allCraftableItems.Where(c => c.ItemType == ItemType.Book).ToList();
        if (craftableBooks.Any())
        {
            var go = Instantiate(CraftableCategoryPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            go.transform.SetParent(CraftableItemContainer.transform, false);
            var categoryName = go.GetComponent<Text>();
            categoryName.text = "- Books -";

            foreach (var item in craftableBooks)
            {
                var go1 = Instantiate(CraftableItemPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                go1.transform.SetParent(CraftableItemContainer.transform, false);
                var itemModel = go1.GetComponent<CraftableItemModel>();
                itemModel.Init(item);
            }
        }

        //Miscellaneous
        var craftableMisc = allCraftableItems.Where(c => c.ItemType == ItemType.Miscellaneous).ToList();
        if (craftableMisc.Any())
        {
            var go = Instantiate(CraftableCategoryPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            go.transform.SetParent(CraftableItemContainer.transform, false);
            var categoryName = go.GetComponent<Text>();
            categoryName.text = "- Miscellaneous -";

            foreach (var item in craftableMisc)
            {
                var go1 = Instantiate(CraftableItemPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                go1.transform.SetParent(CraftableItemContainer.transform, false);
                var itemModel = go1.GetComponent<CraftableItemModel>();
                itemModel.Init(item);
            }
        }

        //Quest Items
        var craftableQuest = allCraftableItems.Where(c => c.ItemType == ItemType.Quest_Item).ToList();
        if (craftableQuest.Any())
        {
            var go = Instantiate(CraftableCategoryPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            go.transform.SetParent(CraftableItemContainer.transform, false);
            var categoryName = go.GetComponent<Text>();
            categoryName.text = "- Quest -";

            foreach (var item in craftableQuest)
            {
                var go1 = Instantiate(CraftableItemPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                go1.transform.SetParent(CraftableItemContainer.transform, false);
                var itemModel = go1.GetComponent<CraftableItemModel>();
                itemModel.Init(item);
            }
        }

        //Apparel by Slot
        foreach(var apparelSlot in Rm_RPGHandler.Instance.Items.ApparelSlots)
        {
            if (apparelSlot.Name == "OffHand") continue;

            var craftableOfType = allCraftableItems.Where(c => c.ItemType == ItemType.Apparel && ((Apparel)c).ApparelSlotName == apparelSlot.Name).ToList();
            if (craftableOfType.Any())
            {
                var go = Instantiate(CraftableCategoryPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                go.transform.SetParent(CraftableItemContainer.transform, false);
                var categoryName = go.GetComponent<Text>();
                categoryName.text = "- " + apparelSlot.Name + " -";

                foreach (var item in craftableOfType)
                {
                    var go1 = Instantiate(CraftableItemPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                    go1.transform.SetParent(CraftableItemContainer.transform, false);
                    var itemModel = go1.GetComponent<CraftableItemModel>();
                    itemModel.Init(item);
                }
            }
        }
    }

    public void CloseCraftingWindow()
    {
        Show = false;
        OpenedByDialog = false;
    }
}

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

public class UIHandler : MonoBehaviour
{

    public static UIHandler Instance;
    public static bool MouseOnUI = true;
    public static bool MouseOnPlayer = true;
    public static bool ShowFPSCounter = true;
    public bool DebugShowFPS = true;
    public bool DebugShowUI = true;

    public ShowFps FPSCounter;
    public AchievementUI AchievementsUI;
    public InventoryUI InventoryUI;
    public QuestTrackerUI QuestTrackerUI;
    public CharacterUI CharacterUI;
    public CraftingUI CraftingUI;
    public WorldMapUI WorldMapUI;
    public CoreUI CoreUI;
    public DialogUI DialogUI;
    public BookUI BookUI;
    public QuestLogUI QuestLogUI;
    public AbilityLogUI AbilityLogUI;
    public SkillBarUI SkillBarUI;
    public TooltipUI TooltipUI;
    public HarvestingUI HarvestingUI;
    public VendorUI VendorUI;
    public PopupUI PopupUI;
    public PetUI PetUI;
    public PauseMenuUI PauseMenuUI;
    public MinimapUI MinimapUI;
    public MobileTouchUI MobileTouchUI;
    public SubtitleUI SubtitleUI;
    public EventSystem EventSystem;

    public bool CanOpenWindows
    {
        get { return !GameMaster.CutsceneActive && !GameMaster.GamePaused; }
    }

    void Awake()
    {
        Instance = this;
        MouseOnPlayer = false;
        PauseMenuUI.gameObject.SetActive(false);
        InventoryUI.Instance = InventoryUI;
        TooltipUI.Instance = TooltipUI;
        

        //Hide UI to stop flickering
        DialogUI.gameObject.SetActive(false);
        SubtitleUI.SubtitleTextObject.SetActive(false);
    }

    void OnEnable()
    {
        RPG.Events.MenuOpened += ToggleMenu;
        RPG.Events.OpenCrafting += OpenCrafting;
        RPG.Events.OpenVendor += OpenVendor;
    }

    void OnDisable()
    {
        RPG.Events.MenuOpened -= ToggleMenu;
        RPG.Events.OpenCrafting -= OpenCrafting;
        RPG.Events.OpenVendor -= OpenVendor;
    }

    public void Mobile_TryJump()
    {
        GetObject.PlayerController.TryJump();
    }
    public void Mobile_TogglePauseGame()
    {
        ToggleMenu(null,null);
    }
    public void Mobile_ToggleInventory()
    {
        InventoryUI.ToggleInventory();
    }
    public void Mobile_ToggleSkills()
    {
        AbilityLogUI.ToggleAbilityLogUI();
    }
    public void Mobile_ToggleQuests()
    {
        QuestLogUI.ToggleQuestLogUI();
    }
    public void Mobile_ToggleCharacter()
    {
        CharacterUI.ToggleCharacterSheet();
    }
    public void Mobile_ToggleCrafting()
    {
        CraftingUI.ToggleCraftingUI();
    }

    private void OpenCrafting(object sender, RPGEvents.OpenCraftingEventArgs e)
    {
        CraftingUI.Show = false;
        CraftingUI.ToggleCraftingUI();
    }
    
    private void OpenVendor(object sender, RPGEvents.OpenVendorEventArgs e)
    {
        var vendorShop = e.VendorShop;
        VendorUI.Show = false;
        VendorUI.ShowVendorWindow(VendorUI.GetShop(vendorShop));
    }


    public void Init()
    {
        CoreUI.Init();
        SkillBarUI.Init();
    }

    void Update()
    {
        var curPet = GetObject.PlayerCharacter.CurrentPet;
        PetUI.Show = curPet != null && curPet.PetData.CurrentBehaviour != PetBehaviour.PetOnly;

        MouseOnUI = EventSystem.current.IsPointerOverGameObject() || (SkillDragHandler.itemBeingDragged != null);
        if (CanOpenWindows && Input.GetKeyDown(KeyCode.G))
        {
            GameMaster.ShowUI = !GameMaster.ShowUI;
            if(GameMaster.ShowUI)
            {
                InventoryUI.UpdateItemContainer();
            }
        }

        if (CanOpenWindows && RPG.Input.GetKeyDown(RPG.Input.WorldMap))
        {
            WorldMapUI.ToggleMap();
        }

        if (CanOpenWindows && RPG.Input.GetKeyDown(RPG.Input.Inventory))
        {
            InventoryUI.ToggleInventory();
            TooltipUI.Clear();
        }

        if (CanOpenWindows && RPG.Input.GetKeyDown(RPG.Input.CharacterSheet))
        {
            CharacterUI.ToggleCharacterSheet();
            TooltipUI.Clear();
        }
        if (CanOpenWindows && RPG.Input.GetKeyDown(RPG.Input.Crafting))
        {
            CraftingUI.ToggleCraftingUI();
            TooltipUI.Clear();
        }
        if (CanOpenWindows && RPG.Input.GetKeyDown(RPG.Input.QuestBook))
        {
            QuestLogUI.ToggleQuestLogUI();
            TooltipUI.Clear();
        }
        if (CanOpenWindows && RPG.Input.GetKeyDown(RPG.Input.SkillBook))
        {
            AbilityLogUI.ToggleAbilityLogUI();
            TooltipUI.Clear();
        }

        FPSCounter.gameObject.SetActive(DebugShowUI && DebugShowFPS && ShowFPSCounter);

        AchievementsUI.gameObject.SetActive(DebugShowUI && GameMaster.ShowUI);
        InventoryUI.gameObject.SetActive(DebugShowUI && GameMaster.ShowUI && InventoryUI.Show);
        CharacterUI.gameObject.SetActive(DebugShowUI && GameMaster.ShowUI && CharacterUI.Show);
        CraftingUI.gameObject.SetActive(DebugShowUI && GameMaster.ShowUI && CraftingUI.Show);
        QuestLogUI.gameObject.SetActive(DebugShowUI && GameMaster.ShowUI && QuestLogUI.Show);
        VendorUI.gameObject.SetActive(DebugShowUI && GameMaster.ShowUI && VendorUI.Show);
        AbilityLogUI.gameObject.SetActive(DebugShowUI && GameMaster.ShowUI && AbilityLogUI.Show);
        PetUI.gameObject.SetActive(DebugShowUI && GameMaster.ShowUI && PetUI.Show);

        var minimapOptionShow = Rm_RPGHandler.Instance.GameInfo.MinimapOptions.ShowMinimap;
        MinimapUI.gameObject.SetActive(DebugShowUI && GameMaster.ShowUI && MinimapUI.Show && minimapOptionShow);

        WorldMapUI.gameObject.SetActive(DebugShowUI && GameMaster.ShowUI);
        HarvestingUI.gameObject.SetActive(DebugShowUI && GameMaster.ShowUI);
        CoreUI.gameObject.SetActive(DebugShowUI && GameMaster.ShowUI);
        DialogUI.gameObject.SetActive(DebugShowUI && DialogUI.DialogModel != null && GameMaster.ShowUI);
        BookUI.gameObject.SetActive(DebugShowUI && GameMaster.ShowUI);
        QuestTrackerUI.gameObject.SetActive(DebugShowUI && GameMaster.ShowUI);
        SkillBarUI.gameObject.SetActive(DebugShowUI && GameMaster.ShowUI);
        PopupUI.gameObject.SetActive(DebugShowUI && GameMaster.ShowUI);

        if(MobileTouchUI != null)
            MobileTouchUI.gameObject.SetActive(DebugShowUI && GameMaster.ShowUI);
    }

    public void ToggleMenu(object sender, RPGEvents.OpenMenuEventArgs e)
    {
        if(!PauseMenuUI.Showing)
        {
            GameMaster.GamePaused = true;
            PauseMenuUI.gameObject.SetActive(true);
            PauseMenuUI.Showing = true;
        }
        else
        {
            if (PauseMenuUI.LoadMenu.activeInHierarchy || PauseMenuUI.OptionsMenu.activeInHierarchy || PauseMenuUI.ConfirmExit.activeInHierarchy)
            {
                PauseMenuUI.BackToDefaultMenu();
            }
            else
            {
                GameMaster.GamePaused = false;
                PauseMenuUI.gameObject.SetActive(false);
                PauseMenuUI.Showing = false;
            }
        }
    }
}

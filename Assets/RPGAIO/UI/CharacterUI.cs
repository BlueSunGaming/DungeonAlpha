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

public class CharacterUI : MonoBehaviour
{
    public static CharacterUI Instance;
    public CharacterTooltipModel Tooltip;
    public Scrollbar ScrollbarPos;
    public RectTransform CachedTooltipRect;
    public List<CharacterItemModel> EquipmentSlots;
    public GameObject CharacterInfoContainer;
    public GameObject CharacterInfoPrefab;
    public GameObject CharacterAttributeLinePrefab;
    public GameObject CharacterAttributeApplyPrefab;
    public GameObject CharacterInfoTitlePrefab;
    public bool Show;
    public bool Initialised;

    private EventSystem EventSystem
    {
        get { return UIHandler.Instance.EventSystem; }
    }

	// Use this for initialization
	void Awake () {
	    Instance = this;


        HideTooltip();
	    CachedTooltipRect = Tooltip.GetComponent<RectTransform>();
	}

    void OnEnable()
    {
        RPG.Events.EquippedItem += UpdateFromEquip;
        RPG.Events.UnEquippedItem += UpdateFromUnequip;
        RPG.Events.GainedExp += UpdateFromGainedExp;
        RPG.Events.UpdatedPlayerStats += UpdateAllStats;
    }

    void OnDisable()
    {
        RPG.Events.EquippedItem -= UpdateFromEquip;
        RPG.Events.UnEquippedItem -= UpdateFromUnequip;
        RPG.Events.GainedExp -= UpdateFromGainedExp;
        RPG.Events.UpdatedPlayerStats -= UpdateAllStats;
    }

    private void UpdateAllStats(object sender, RPGEvents.UpdatedPlayerStatsArgs e)
    {
        UpdateStats();
    }

    private void UpdateFromGainedExp(object sender, RPGEvents.GainedExpEventArgs e)
    {
        UpdateStats();
    }

    void Update()
    {
        if(GameMaster.CutsceneActive || GameMaster.GamePaused)
        {
            HideTooltip();
        }

        if(Tooltip.isActiveAndEnabled)
            Tooltip.transform.position = Input.mousePosition + new Vector3(CachedTooltipRect.rect.width / 2f + 40 , -(CachedTooltipRect.rect.height / 2f) - 20, 0);

    }

    public void ShowTooltip(string description)
    {
        if (string.IsNullOrEmpty(description))
        {
            HideTooltip();
            return;
        }

        Tooltip.TooltipText.text = description;
        Tooltip.gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        Tooltip.gameObject.SetActive(false);
    }

    private void UpdateStats()
    {
        var currentSliderPos = ScrollbarPos.value;
        if (CharacterInfoContainer == null) return;
        var Player = GetObject.PlayerCharacter;
        var PlayerSave = GetObject.PlayerSave;

        var showAttributePoints = Player.CurrentAttributePoints > 0 || Player.Attributes.Any(a => a.TempValue > 0);

        CharacterInfoContainer.transform.DestroyChildren();




        if (Player == null || PlayerSave == null || Player.DamageDealable == null) return;

        var damageDealable = Player.DamageDealable.MaxTotal;

        if(showAttributePoints)
        {
            var obj = Instantiate(CharacterInfoTitlePrefab, Vector3.zero, Quaternion.identity) as GameObject;
            obj.transform.SetParent(CharacterInfoContainer.transform, false);
            var titleModel = obj.GetComponent<CharacterLineModel>();
            titleModel.TextLeft.text = string.Format("Attributes (Points Remaining: {0})", Player.CurrentAttributePoints);
            titleModel.HideTooltip = true;

            //Show attribute point applying first
            foreach (var attr in Player.Attributes)
            {
                var go = Instantiate(CharacterAttributeLinePrefab, Vector3.zero, Quaternion.identity) as GameObject;
                go.transform.SetParent(CharacterInfoContainer.transform, false);
                var textSelectModel = go.GetComponent<CharacterAttributeLineModel>();
                textSelectModel.Init(attr, titleModel);
            }

            var applyObj = Instantiate(CharacterAttributeApplyPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            applyObj.transform.SetParent(CharacterInfoContainer.transform, false);
        }

        var infoSet = new Dictionary<string, ThreeDataHolder>();

        var lines = new ThreeDataHolder();

        var raceInfo = Rm_RPGHandler.Instance.Player.SkipRaceSelection ? "" : RPG.Player.GetRaceDefinition(Player.PlayerRaceID).Name + " ";
        var subRaceInfo = Rm_RPGHandler.Instance.Player.SkipSubRaceSelection ? "" : RPG.Player.GetSubRaceDefinition(Player.PlayerSubRaceID).Name + " ";

        lines.Add(raceInfo + subRaceInfo + RPG.Player.GetClassName(Player.PlayerClassNameID), "Lv." + Player.Level, "Your character's basic info");
        lines.Add("Damage", damageDealable.ToString().Colorfy(Rm_UnityColors.Red), "This is how much damage you deal with a basic attack");
        lines.Add("Exp", (Player.Exp + "/" + Player.ExpToLevel.ToString()), "This is how much experience you have");

        foreach(var metaData in Player.MetaData)
        {
            var meta = RPG.Stats.GetMetaDataByID(metaData.ID);
            lines.Add(RPG.Stats.GetMetaDataName(metaData.ID), meta.GetValueName(metaData.ValueID), meta.TooltipDescription);

        }

        infoSet.Add("", lines);

        var vitalLines = new ThreeDataHolder();
        foreach (var vital in Player.Vitals)
        {
            vitalLines.Add(vital.GetName(), vital.CurrentValue.ToString() + " / " + vital.MaxValue.ToString(), vital.GetDescription());
        }
        infoSet.Add("Vitals", vitalLines);

        //Show attributes normally if we don't have points to apply
        if(!showAttributePoints)
        {
            var attributeLines = new ThreeDataHolder();
            foreach (var attribute in Player.Attributes)
            {
                attributeLines.Add(attribute.GetName(), attribute.TotalValue.ToString(), attribute.GetDescription());
            }
            infoSet.Add("Attributes", attributeLines);
        }
        

        var statisticLines = new ThreeDataHolder();
        foreach (var statistics in Player.Stats)
        {
            statisticLines.Add(statistics.GetName(), statistics.TotalValue.ToString(), statistics.GetDescription());
        }
        infoSet.Add("Statistics", statisticLines);

        if(Player.Traits.Count > 0)
        {
            var traitLines = new ThreeDataHolder();
            foreach (var trait in Player.Traits)
            {
                traitLines.Add(trait.GetName(), trait.Level.ToString(), RPG.Stats.GetTraitDescription(trait.ID));
            }
            infoSet.Add("Traits", traitLines);
        }
        
        //Accounting for 2 built-in reputations
        if(PlayerSave.QuestLog.AllReputations.Count > 2)
        {
            var repLines = new ThreeDataHolder();
            foreach (var rep in GetObject.PlayerSave.QuestLog.AllReputations)
            {
                var repName = RPG.Stats.GetReputationName(rep.ReputationID);
                if (repName == "NonPlayerCharacters" || repName == "EnemyCharacters") continue;
                repLines.Add(repName, rep.Value.ToString(), "");
            }
            infoSet.Add("Reputations", repLines);
        }
        
        foreach(var dataSet in infoSet)
        {
            if(!string.IsNullOrEmpty(dataSet.Key))
            {
                var obj = Instantiate(CharacterInfoTitlePrefab, Vector3.zero, Quaternion.identity) as GameObject;
                obj.transform.SetParent(CharacterInfoContainer.transform, false);
                var infoModel = obj.GetComponent<CharacterLineModel>();
                infoModel.TextLeft.text = dataSet.Key;
                infoModel.HideTooltip = true;
            }

            foreach (var kvp in dataSet.Value.Data)
            {
                var go = Instantiate(CharacterInfoPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                go.transform.SetParent(CharacterInfoContainer.transform, false);
                var textSelectModel = go.GetComponent<CharacterLineModel>();
                textSelectModel.TextLeft.text = kvp.valueB;
                textSelectModel.TextRight.text = kvp.valueA;

                textSelectModel.Description = kvp.valueC;
            }
        }

        ScrollbarPos.value = currentSliderPos;
    }

    private void UpdateFromUnequip(object sender, RPGEvents.UnEquippedItemEventArgs e)  
    {
        var item = e.Item;
        var slot = EquipmentSlots.FirstOrDefault(i => i.ItemID == item.ID);
        if(slot != null)
        {
            slot.EmptyThisSlot();
        }
    }

    private void UpdateFromEquip(object sender, RPGEvents.EquippedItemEventArgs e)
    {
        UpdateEquippedItems();
    }


    public void ToggleCharacterSheet()
    {
        Show = !Show;
        if(Show)
        {
            UpdateStats();
            UpdateEquippedItems();
            ScrollbarPos.value = 0;
        }
    }

    public void CloseCharacterSheet()
    {
        Show = false;
    }

    public void UpdateEquippedItems()
    {
        if (!Show) return;

        var player = GetObject.PlayerCharacter;

        foreach(var slot in EquipmentSlots)
        {
            slot.EmptyThisSlot();
        }

        foreach(var equippedItem in player.Equipment.EquippedItems.Where(i => i.Item != null))
        {
            var item = equippedItem.Item;
            if (item.ItemType == ItemType.Apparel)
            {
                var apparel = (Apparel)item;
                if (apparel.ApparelSlotName == "OffHand")
                {
                    var slot = EquipmentSlots.First(eq => eq.SlotName == "OffHand");
                    slot.Init(item);
                }
                else
                {
                    var slot = EquipmentSlots.First(eq => eq.SlotName == apparel.ApparelSlotName);
                    slot.Init(item);
                }
            }
            else
            {
                if(equippedItem.SlotName == "Weapon")
                {
                    var slot = EquipmentSlots.First(eq => eq.SlotName == "Weapon");
                    slot.Init(item);
                }
                else
                {
                    var slot = EquipmentSlots.First(eq => eq.SlotName == "OffHand");
                    slot.Init(item);
                }
            }
        }
    }

    public void ApplyPoints()
    {
        var player = GetObject.PlayerCharacter;
        player.Attributes.ForEach(a =>
                                      {
                                          a.BaseValue += a.TempValue;
                                          a.TempValue = 0;
                                      });
        UpdateStats();
    }
}

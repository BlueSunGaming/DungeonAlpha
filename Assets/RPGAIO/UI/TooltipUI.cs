using System;
using System.Linq;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance;
    public static object CurrentTooltipObject;
    public GameObject TooltipPanel;
    public TooltipModel TooltipModel;

    private Transform _tooltipTransform;
    private Camera _tooltipCamera;

    // Update is called once per frame
    void Awake()
    {
        Instance = this;
        _tooltipTransform = TooltipPanel.transform;
    }

    void Update()
    {
        if (GameMaster.CutsceneActive) Clear();
        TooltipPanel.SetActive(CurrentTooltipObject != null);

        _tooltipTransform.position = Input.mousePosition;
    }

    public static void Set(Item item)
    {
        if(item == null)
        {
            Clear();
            return;
        }

        CurrentTooltipObject = item;
        Instance.TooltipModel.ObjectName.text = item.Name;

        var weapon = item as Weapon;
        var extraInfo = "";

        if(weapon != null)
        {
            extraInfo = RPG.Items.GetWeaponTypeName(weapon.WeaponTypeID) + " ";
        }
        else
        {
            var itemTypeName = Rm_RPGHandler.Instance.Items.ItemTypeNames.First(i => i.ItemType == item.ItemType).Name;
            extraInfo = itemTypeName;
        }

        Instance.TooltipModel.ObjectType.text = RPG.Items.GetRarityName(item.RarityID) + " " + extraInfo;
        Instance.TooltipModel.ObjectImage.sprite = GeneralMethods.CreateSprite(item.Image);
        Instance.TooltipModel.ObjectDescription.text = item.GetTooltipDescription();
        Instance.TooltipModel.ObjectFooter.text = item.ItemType == ItemType.Quest_Item ? "Cannot be sold" : "Sells for " + item.SellValue.ToString("N0") + "g";
        
        if(item.ItemType == ItemType.Weapon)
        {
            if(Rm_RPGHandler.Instance.Items.DamageHasVariance)
            {
                Instance.TooltipModel.ObjectMainInfo.text = weapon.Damage.MinTotal + "-" + weapon.Damage.MaxTotal + " Damage";
            }
            else
            {
                Instance.TooltipModel.ObjectMainInfo.text = weapon.Damage.MaxTotal + " Damage";
            }
        }
        else if(item.ItemType == ItemType.Apparel)
        {
            var apparel = (Apparel)item;
            var firstStat = apparel.StatisticBuffs.FirstOrDefault();    
            if(firstStat != null)
            {
                var statName = RPG.Stats.GetStatisticName(firstStat.StatisticID);
                var isPercentage = RPG.Stats.IsStatisticPercentageInUI(firstStat.StatisticID);
                var statInfo = isPercentage ? (firstStat.Amount*100).ToString("N2") + "%" : firstStat.Amount.ToString();
                Instance.TooltipModel.ObjectMainInfo.text = statInfo + " " + statName;  
            }
            else
            {
                Instance.TooltipModel.ObjectMainInfo.text = "";
            }
        }
        else if(item.ItemType == ItemType.Consumable)
        {
            var consumable = (Consumable)item;
            var restorationTypeName = Rm_RPGHandler.Instance.Items.ConsumableTypeNames.First(i => i.Type == consumable.Restoration.RestorationType).Name;
            Instance.TooltipModel.ObjectMainInfo.text = restorationTypeName;
        }
        else
        {
            Instance.TooltipModel.ObjectMainInfo.text = "";
        }
    }

    public static void Set(Skill skill)
    {
        throw new NotImplementedException();
    }


    public static void Clear()
    {
        CurrentTooltipObject = null;
    }

	
}
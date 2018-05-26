using System.Globalization;
using System.Linq;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestRewardModel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image ItemImage;
    public Text Amount;
    public string ItemID;

    public void Init(Item item, int amount)
    {
        var sprite = GeneralMethods.CreateSprite(item.Image);
        ItemImage.sprite = sprite;
        Amount.text = amount.ToString(CultureInfo.InvariantCulture);
        ItemImage.color = Color.white;
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Clear();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var item = Rm_RPGHandler.Instance.Repositories.Items.Get(ItemID)
            ?? Rm_RPGHandler.Instance.Repositories.CraftableItems.Get(ItemID)
            ?? Rm_RPGHandler.Instance.Repositories.QuestItems.Get(ItemID);

        TooltipUI.Set(item);
    }
}

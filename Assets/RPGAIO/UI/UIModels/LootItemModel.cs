using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LootItemModel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Item Item;

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Clear();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(Item != null)
            TooltipUI.Set(Item);
    }
}
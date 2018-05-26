using System.Linq;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftableItemModel : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Text ItemName;
    public string CraftableItemId;

    public void Init(Item item)
    {
        ItemName.text = item.Name;
        CraftableItemId = item.ID;
    }

    public void SelectItem()
    {
        UIHandler.Instance.CraftingUI.SelectCraftableItem(CraftableItemId); 
        //Debug.Log("Selected craftable item!");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //if (eventData.button == PointerEventData.InputButton.Left)
        //    SelectItem();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Clear();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var item = Rm_RPGHandler.Instance.Repositories.CraftableItems.Get(CraftableItemId);
        TooltipUI.Set(item);
    }
}

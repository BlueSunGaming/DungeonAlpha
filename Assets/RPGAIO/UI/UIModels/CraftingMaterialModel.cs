using System.Linq;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CraftingMaterialModel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image ItemImage;
    public Text CurrentOwned;
    public string ItemID;
    public int NumberRequired;

    public void Init(Item item, int numberRequired)
    {
        var sprite = GeneralMethods.CreateSprite(item.Image);
        var stackable = item as IStackable;
        var player = GetObject.PlayerCharacter;
        ItemImage.sprite = sprite;
        ItemID = item.ID;
        NumberRequired = numberRequired;

        var itemNeeded = player.Inventory.AllItems.FirstOrDefault(i => i.ID == ItemID);
        var amtOwned = 0;

        if(stackable != null)
        {
            if (itemNeeded != null)
            {
                var itemStacks = (IStackable) itemNeeded;
                amtOwned = itemStacks.CurrentStacks;
            }
        }
        else
        {
            if(itemNeeded != null)
            {
                amtOwned = player.Inventory.AllItems.Count(i => i.ID == ItemID);
            }
        }

        CurrentOwned.text = amtOwned >= numberRequired 
            ? string.Format("{0}/{1}",amtOwned,numberRequired)
            : string.Format("<color=red>{0}/{1}</color>", amtOwned, numberRequired);    
        ItemImage.color = Color.white;
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Clear();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var item = Rm_RPGHandler.Instance.Repositories.Items.Get(ItemID);
        TooltipUI.Set(item);
    }
}

public class MobilePhone
{
    private readonly string _brand;
    private readonly int _cost;

    public MobilePhone(string brand, int cost)
    {
        _brand = brand;
        _cost = cost;
    }

    public void DoSomething()
    {
        var str = SomethingA();
        var a = str + "";
        if(SomethingB(a))
        {
            return;
        }
    }

    private bool SomethingB(string someStr)
    {
        return true;
    }

    private string SomethingA()
    {
        return "abc";
    }
}

using System.Linq;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillBarSlotModel : MonoBehaviour, IDropHandler
{
    public GameObject item
    {
        get
        {
            if(transform.childCount>0)
            {
                return transform.GetChild(0).gameObject;
            }
            return null;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        var skillBarButtonModel = GetComponent<SkillBarButtonModel>();
        
        var skillDragHandler = SkillDragHandler.itemBeingDragged != null ? SkillDragHandler.itemBeingDragged.GetComponent<SkillDragHandler>() : null;
        if (skillDragHandler != null && skillDragHandler.IsSkill)
        {
            var skill = RPG.GetPlayerCharacter.SkillHandler.AvailableSkills.First(s => s.ID == skillDragHandler.RefId);
            RPG.GetPlayerCharacter.SkillHandler.Slots[skillBarButtonModel.SkillSlot].ChangeSlotTo(skill);

            skillBarButtonModel.SkillImage.sprite = GeneralMethods.CreateSprite(skill.Image.Image);
            skillBarButtonModel.SkillImage.color = Color.white;

        }
        else
        {
            var inventoryItem = RPG.GetPlayerCharacter.Inventory.GetReferencedItem(skillDragHandler.RefId);
            RPG.GetPlayerCharacter.SkillHandler.Slots[skillBarButtonModel.SkillSlot].ChangeSlotTo(inventoryItem);

            skillBarButtonModel.SkillImage.sprite = GeneralMethods.CreateSprite(inventoryItem.Image);
            skillBarButtonModel.SkillImage.color = Color.white;
        }
    }
}
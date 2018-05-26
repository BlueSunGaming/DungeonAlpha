using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AbilityEntryModel : MonoBehaviour, IPointerClickHandler
{
    public Image AbilityImage;
    public Image AbilityImageForDrag;
    public SkillDragHandler DragHandler;
    public Text AbilityText;
    public string RefId;

    public void Init(Texture2D image, string text, string refId, bool enableDrag)
    {
        var sprite = GeneralMethods.CreateSprite(image);
        AbilityImage.sprite = sprite;
        AbilityImageForDrag.sprite = sprite;
        AbilityText.text = text;
        RefId = refId;

        //For skills only:
        DragHandler.IsSkill = true;
        DragHandler.RefId = refId;

        //if(!enableDrag)
        //{
        //    Destroy(DragHandler.gameObject);
        //}
    }

    public void SelectItem()
    {
        UIHandler.Instance.AbilityLogUI.SelectAbility(RefId); 
        //Debug.Log("Selected ability item!");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            SelectItem();
    }
}

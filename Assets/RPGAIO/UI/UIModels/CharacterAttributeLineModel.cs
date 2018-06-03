using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterAttributeLineModel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool HideTooltip;
    public Text TextLeft;
    public Text TextRight;
    public string Description;

    //Attributes
    public Attribute AttributeRef;
    public CharacterLineModel TitleModelRef;
    public Button AddPointButton;
    public Button MinusPointButton;


    public void Init(Attribute attribute, CharacterLineModel titleModel)
    {
        var player = GetObject.PlayerCharacter;
        AttributeRef = attribute;
        TitleModelRef = titleModel;
        TextRight.text = RPG.Stats.GetAttributeName(attribute.ID);
        Description = RPG.Stats.GetAttributeDesc(attribute.ID);
        AddPointButton.interactable = player.CurrentAttributePoints > 0;
        MinusPointButton.interactable = attribute.TempValue > 0;
        UpdateText();
    }

    public void AddPoint()
    {
        var player = GetObject.PlayerCharacter;
        AttributeRef.TempValue += 1;
        player.CurrentAttributePoints -= 1;
        if(player.CurrentAttributePoints == 0)
        {
            var attributes = FindObjectsOfType<CharacterAttributeLineModel>();
            foreach(var attr in attributes)
            {
                attr.AddPointButton.interactable = false;
            }
        }

        UpdateText();
        MinusPointButton.interactable = true;
    }
    public void MinusPoint()
    {
        var player = GetObject.PlayerCharacter;
        AttributeRef.TempValue -= 1;
        player.CurrentAttributePoints += 1;
        if (AttributeRef.TempValue == 0)
        {
            MinusPointButton.interactable = false;
        }

        UpdateText();
        var attributes = FindObjectsOfType<CharacterAttributeLineModel>();
        foreach (var attr in attributes)
        {
            attr.AddPointButton.interactable = true;
        }
    }

    private void UpdateText()
    {
        TitleModelRef.TextLeft.text = string.Format("Attributes (Points Remaining: {0})", GetObject.PlayerCharacter.CurrentAttributePoints);
        TextLeft.text = AttributeRef.TotalValue.ToString();
        if (AttributeRef.TempValue > 0)
        {
            TextLeft.text += "+" + AttributeRef.TempValue.ToString().Colorfy(Rm_UnityColors.Green);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!HideTooltip)
            CharacterUI.Instance.ShowTooltip(Description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CharacterUI.Instance.HideTooltip();
    }
}
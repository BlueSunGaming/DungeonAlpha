using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VisualImageSelectModel : MonoBehaviour, IPointerClickHandler
{
    public Image Image;
    public VisualCustomisationImagesModel ModelRef;
    public string OptionRef;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        ModelRef.SetTextOption(OptionRef);
    }

    public void Init(VisualCustomisationImagesModel modelRef, string option, ImageContainer imageOption)
    {
        OptionRef = option;
        ModelRef = modelRef;
        Image.overrideSprite = GeneralMethods.CreateSprite(imageOption.Image);
    }
}
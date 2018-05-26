using System.Collections.Generic;
using LogicSpawn.RPGMaker;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VisualTextSelectModel : MonoBehaviour, IPointerClickHandler
{
    public Text Text;
    public VisualCustomisationTextsModel ModelRef;
    public string OptionRef;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        ModelRef.SetTextOption(OptionRef);
    }

    public void Init(VisualCustomisationTextsModel modelRef, string option, string labelName)
    {
        OptionRef = option;
        ModelRef = modelRef;
        Text.text = labelName;
    }
}
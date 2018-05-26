using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterLineModel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool HideTooltip;
    public Text TextLeft;
    public Text TextRight;
    public string Description;
    
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
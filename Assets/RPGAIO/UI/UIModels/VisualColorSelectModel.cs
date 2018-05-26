using LogicSpawn.RPGMaker;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VisualColorSelectModel : MonoBehaviour, IPointerClickHandler
{
    public Image ColorImage;
    public VisualCustomisationColorsModel ModelRef;
    public RPG_Color Color;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        ModelRef.SetColorOption(Color);
    }

    public void Init(VisualCustomisationColorsModel modelRef, RPG_Color colorOption)
    {
        Color = colorOption;
        ModelRef = modelRef;
        ColorImage.color = Color.ToUnityColor();
    }
}
using LogicSpawn.RPGMaker.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RaceSelectModel : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Text ButtonText;
    public Image ButtonImage;
    public string RaceID;

    public void OnPointerClick(PointerEventData eventData)
    {
        CharacterCreationMono.Instance.SetRace(RaceID);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CharacterCreationMono.Instance.ShowRace(RaceID);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CharacterCreationMono.Instance.ShowRace(null);
    }
}
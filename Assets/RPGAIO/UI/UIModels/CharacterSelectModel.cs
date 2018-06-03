using LogicSpawn.RPGMaker.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSelectModel : MonoBehaviour, IPointerClickHandler
{
    public Text ButtonText;
    public Image ButtonImage;
    public string ClassNameID;
    public string CharacterID;

    public void OnPointerClick(PointerEventData eventData)
    {
        CharacterCreationMono.Instance.SetCharacter(ClassNameID, CharacterID);
    }
}
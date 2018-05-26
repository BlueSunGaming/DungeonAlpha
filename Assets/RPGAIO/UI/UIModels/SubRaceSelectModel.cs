using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SubRaceSelectModel : MonoBehaviour, IPointerClickHandler
{
    public Image ButtonImage;
    public string SubRaceID;

    public void OnPointerClick(PointerEventData eventData)
    {
        CharacterCreationMono.Instance.SetSubRace(SubRaceID);
    }
}
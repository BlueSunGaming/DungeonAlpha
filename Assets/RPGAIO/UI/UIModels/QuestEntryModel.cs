using System.Linq;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestEntryModel : MonoBehaviour, IPointerClickHandler
{
    public Text ItemName;
    public string QuestId;

    public void Init(Quest item)
    {
        ItemName.text = item.Name;
        QuestId = item.ID;
    }

    public void SelectItem()
    {
        UIHandler.Instance.QuestLogUI.SelectQuestEntry(QuestId); 
        Debug.Log("Selected quest item!");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            SelectItem();
    }
}

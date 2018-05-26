using System;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcQuestStatusModel : MonoBehaviour
{
    public Text StatusText;
    private CanvasGroup Canvas;

    void Start()
    {
        Canvas = GetComponent<CanvasGroup>();
    }

    public void SetStatus (NpcStatus npcStatus)
    {
        switch (npcStatus)
        {
            case NpcStatus.AvailableQuest:
                StatusText.text = "<color=yellow>!</color>";
                break;
            case NpcStatus.InProgressQuest:
                StatusText.text = "<color=silver>?</color>";
                break;
            case NpcStatus.CompletableQuest:
                StatusText.text = "<color=yellow>?</color>";
                break;
            case NpcStatus.None:
                StatusText.text = " ";
                break;
            default:
                throw new ArgumentOutOfRangeException("npcStatus");
        }
    }

    void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            Canvas.alpha = GameMaster.ShowUI && Rm_RPGHandler.Instance.Questing.ShowQuestMarkers ? 1 : 0;
            Canvas.transform.rotation = GetObject.RPGCamera.transform.rotation;
        }
    }
}
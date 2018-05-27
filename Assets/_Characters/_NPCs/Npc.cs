using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonRPG.CameraUI;
using DungeonRPG;
//using Yarn.Unity.Example;

public class Npc: Interactable
{

    [SerializeField]
    private string dialogueTextIdentifier;
    
    [SerializeField]
    private string dialogueAssetFileName;

    [HideInInspector]
    private TextAsset jsonRes;
    
    // Use this for initialization
    void Start()
    {

        if (dialogueAssetFileName != "")
        {
            jsonRes = Resources.Load<TextAsset>("JSON/Dialogue/" + dialogueAssetFileName);
            if (jsonRes != null && dialogueTextIdentifier != "")
            {
                //GameManager.instance.dr.startNode = dialogueTextIdentifier;
                //GameManager.instance.dr.AddScript(jsonRes);
            }
        }
    }

    public override void Interact()
    {
        Debug.Log("Interacting with NPC.");
        //GameManager.instance.dr.startNode = dialogueTextIdentifier;
        //GameManager.instance.dr.AddScript(jsonRes);
        //GameManager.instance.dr.StartDialogue();
    }

}

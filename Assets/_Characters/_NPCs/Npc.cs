using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.CameraUI;
using Yarn.Unity.Example;

public class Npc: MonoBehaviour
{

  
    CameraRaycaster cameraRaycaster;
    [SerializeField]
    private string dialogueAsset;
    public ExampleDialogueUI dialogue;


    // Use this for initialization
    void Start()
    {
        if (dialogueAsset != "")
        {
            TextAsset jsonRes = Resources.Load<TextAsset>("JSON/Dialogue/" + dialogueAsset);
            if (jsonRes)
            {
                GameManager.instance.dr.AddScript(jsonRes);
            }
        }
        RegisterForMouseClick();
    }

    void OnMouseClick(RaycastHit raycastHit, int layerHit)
    {
        if (layerHit == CursorAffordance.nNPCLayer)
        {
            var NPC = raycastHit.collider.gameObject;

            if (IsTargetInRange(GameObject.FindGameObjectWithTag("Player")))
            {
                GameManager.instance.dr.StartDialogue();
                
            }
        }
    }

    private bool IsTargetInRange(GameObject target)
    {
        float distanceToTarget = (target.transform.position - transform.position).magnitude;

        return distanceToTarget <= 2.0f;
    }

    private void RegisterForMouseClick()
    {
        cameraRaycaster = FindObjectOfType<CameraRaycaster>();

        cameraRaycaster.notifyMouseClickObservers += OnMouseClick;
    }

 

    // Update is called once per frame
    void Update()
        {

        }
    }

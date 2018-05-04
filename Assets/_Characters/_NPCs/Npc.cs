using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.CameraUI;

public class Npc: MonoBehaviour
{

    //[SerializeField]
    //int NPCLayer = 10;

    CameraRaycaster cameraRaycaster;
    [SerializeField]
    private string dialogueAsset;
    public Dialogue dialogue;


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

            if (IsTargetInRange(NPC))
            {
                GameManager.instance.dr.StartDialogue();
                //TriggerDialogue();
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

    public void TriggerDialogue()
    {
        Debug.Log("Attempting to perform trigger Dialogue.");
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }

    // Update is called once per frame
    void Update()
        {

        }
    }

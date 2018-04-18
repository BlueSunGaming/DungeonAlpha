using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.CameraUI;

public class Npc: MonoBehaviour
{

    [SerializeField]
    int NPCLayer = 10;

    CameraRaycaster cameraRaycaster;

    public Dialogue dialogue;


    // Use this for initialization
    void Start()
    {
        RegisterForMouseClick();
    }

    void OnMouseClick(RaycastHit raycastHit, int layerHit)
    {
        if (layerHit == NPCLayer)
        {
            var NPC = raycastHit.collider.gameObject;

            if (IsTargetInRange(NPC))
            {
                TriggerDialogue();
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
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }

    // Update is called once per frame
    void Update()
        {

        }
    }

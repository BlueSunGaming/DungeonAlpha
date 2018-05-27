using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonRPG.CameraUI;
using DungeonRPG;

public class WorldInteractions : MonoBehaviour
{
    UnityEngine.AI.NavMeshAgent playerAgent;
    CameraRaycaster cameraRaycaster;
    [SerializeField]
    private LayerMask interactLayer;

    void Start()
    {
        playerAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        cameraRaycaster = FindObjectOfType<CameraRaycaster>();

        cameraRaycaster.notifyMouseClickObservers += OnMouseClick;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            GetInteraction();

        Debug.DrawRay(transform.position, transform.forward * 5f, Color.red);
    }

    void OnMouseClick(RaycastHit raycastHit, int layerHit)
    {
        //playerAgent.updateRotation = true;
        GameObject interactedObject = raycastHit.collider.gameObject;
        Debug.Log("value of interacted object is " + interactedObject);
        if (interactedObject.tag == "Enemy")
        {
            Debug.Log("move to enemy");
            interactedObject.GetComponent<Interactable>().MoveToInteraction(playerAgent);
        }
        else if (interactedObject.tag == "Interactable Object")
        {
            interactedObject.GetComponent<Interactable>().MoveToInteraction(playerAgent);
        }
        else
        {
            playerAgent.stoppingDistance = 0.5f;
            playerAgent.destination = raycastHit.point;
        }
        //playerAgent.updateRotation = false;
        //if (layerHit == CursorAffordance.nNPCLayer)
        //{
        //    var NPC = raycastHit.collider.gameObject;

        //    if (IsTargetInRange(GameObject.FindGameObjectWithTag("Player")))
        //    {
        //        //GameManager.instance.dr.Clear();
        //        GameManager.instance.dr.startNode = dialogueTextIdentifier;
        //        GameManager.instance.dr.AddScript(jsonRes);
        //        GameManager.instance.dr.StartDialogue();
        //    }
        //}
    }

    //private bool IsTargetInRange(GameObject target)
    //{
    //    float distanceToTarget = (target.transform.position - transform.position).magnitude;

    //    return distanceToTarget <= 2.0f;
    //}

    void GetInteraction()
    {
        Ray interactionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit interactionInfo;
        if (Physics.SphereCast(interactionRay, 1f, out interactionInfo, Mathf.Infinity, interactLayer))
        {
            //playerAgent.updateRotation = true;
            GameObject interactedObject = interactionInfo.collider.gameObject;
            if (interactedObject.tag == "Enemy")
            {
                Debug.Log("move to enemy");
                interactedObject.GetComponent<Interactable>().MoveToInteraction(playerAgent);
            }
            else if (interactedObject.tag == "Interactable Object")
                interactedObject.GetComponent<Interactable>().MoveToInteraction(playerAgent);
            else
            {
                playerAgent.stoppingDistance = 0.5f;
                playerAgent.destination = interactionInfo.point;
            }
            //playerAgent.updateRotation = false;
        }
    }
}

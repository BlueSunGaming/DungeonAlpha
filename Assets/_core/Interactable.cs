using UnityEngine;
using System.Collections;

public class Interactable : MonoBehaviour {

    [HideInInspector]
    public UnityEngine.AI.NavMeshAgent m_nmPlayerAgent;
    private bool m_bHasInteracted;
    bool m_bIsEnemy;

    public virtual void MoveToInteraction(UnityEngine.AI.NavMeshAgent playerAgent)
    {
        m_bIsEnemy = gameObject.tag == "Enemy";
        m_bHasInteracted = false;
        this.m_nmPlayerAgent = playerAgent;
        m_nmPlayerAgent.stoppingDistance = 2.0f;
        m_nmPlayerAgent.destination = GetTargetPosition();
        EnsureLookDirection();
    }

    void Update()
    {
        if (!m_bHasInteracted && m_nmPlayerAgent != null && !m_nmPlayerAgent.pathPending)
        {
            m_nmPlayerAgent.destination = GetTargetPosition();
            EnsureLookDirection();
            if (m_nmPlayerAgent.remainingDistance <= m_nmPlayerAgent.stoppingDistance)
            {
                if (!m_bIsEnemy)
                    Interact();
                m_bHasInteracted = true;
            }
        }
    }

    void EnsureLookDirection()
    {
        //m_nmPlayerAgent.updateRotation = true;
        Vector3 lookDirection = new Vector3(transform.position.x, m_nmPlayerAgent.transform.position.y, transform.position.z);
        m_nmPlayerAgent.transform.LookAt(lookDirection);
        //m_nmPlayerAgent.updateRotation = false;
    }

    public virtual void Interact()
    {
        Debug.Log("Interacting with base class.");
    }

    private Vector3 GetTargetPosition()
    {
        return transform.position;
    }
}

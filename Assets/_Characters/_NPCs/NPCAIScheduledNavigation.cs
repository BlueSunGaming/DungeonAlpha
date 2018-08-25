using System;
using System.Collections;
using System.Collections.Generic;
using LogicSpawn.RPGMaker.Core;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class NPCAIScheduledNavigation : MonoBehaviour {

    //GameObject targetDestination = null;
    
    public List<GameObject> targetDestinations;
    private int currentIndex_Iter=0;
    public float speed = 2;
    //private bool isMoving = false;
    //private bool isWaiting = false;

    // Use this for initialization
    void Start () {
        // Move to targeted point- Initial condition
        MoveToDestination(4.0f);
    }
	
	// Update is called once per frame
	void Update ()
	{
	    float move = speed * Time.deltaTime;

        GameObject currentGameObject = GetCurrentGameObject();
	    if (currentGameObject != null)
	    {
	        if (transform.position == currentGameObject.transform.position)
	        {
	            currentIndex_Iter++;
	            StartCoroutine(WaitThenDo(MoveToDestination));
	        }
	    }
	    else
	    {
	        currentIndex_Iter = 0;
	    }

	}

    private IEnumerator WaitThenDo(object v)
    {
        throw new NotImplementedException();
    }

    IEnumerator WaitThenDo(System.Action thingToDo)
    {
        yield return new WaitForSeconds(5);
        thingToDo();
    }

    void MoveToDestination()
    {
        float move = speed * Time.deltaTime;
        GameObject currentGameObject = GetCurrentGameObject();
        if (currentGameObject != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentGameObject.transform.position, move);
        }

    }

    void MoveToDestination(float timeSinceLastFrame)
    {
        float move = speed * timeSinceLastFrame;
        GameObject currentGameObject = GetCurrentGameObject();
        if (currentGameObject != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentGameObject.transform.position, move);
        }
        
    }


    GameObject GetCurrentGameObject()
    {
        GameObject returnGameObject = null;
        if (currentIndex_Iter == targetDestinations.Count)
        {
            currentIndex_Iter = 0;
        }

        if (currentIndex_Iter < targetDestinations.Count && currentIndex_Iter > -1)
        {
            returnGameObject = targetDestinations[currentIndex_Iter];
        }

        return returnGameObject;
    }

}


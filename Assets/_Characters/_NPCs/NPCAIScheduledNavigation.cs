using System.Collections;
using System.Collections.Generic;
using LogicSpawn.RPGMaker.Core;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class NPCAIScheduledNavigation : MonoBehaviour {

    GameObject targetDestination = null;
    public float speed = 1;

    // Use this for initialization
    void Start () {
        // Move to targeted point
        targetDestination = GameObject.FindGameObjectWithTag("Destination");
    }
	
	// Update is called once per frame
	void Update ()
	{
	    float move = speed * Time.deltaTime;	   	       	        
        transform.position = Vector3.MoveTowards(transform.position, targetDestination.transform.position, move);
	}


}


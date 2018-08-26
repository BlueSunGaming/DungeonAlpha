using System.Collections;
using System.Collections.Generic;
using LogicSpawn.RPGMaker.Core;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class NPCAIScheduledNavigation : MonoBehaviour {

    public GameObject targetDestination1 = null;
    public GameObject targetDestination2 = null;

    public float speed = 1;

    // Use this for initialization
    void Start()
    {
        if (GameClock.instance == null)
        {
            GameObject.Instantiate(GameClock.instance);
        }

        // Move to targeted point
        //targetDestination1 = GameObject.FindGameObjectWithTag("Destination");
        //targetDestination2 = GameObject.FindGameObjectWithTag("Destination");
    }

    // Update is called once per frame
    void Update ()
	{
	    if (GameClock.instance != null)
	    {
	        if (GameClock.instance.GetCurrentIsAM())
	        {
	            float move = speed * Time.deltaTime;
                switch (GameClock.instance.GetCurrentHour())
	            {
                    case 6:
                        // Go towards destination 1
                        transform.position = Vector3.MoveTowards(transform.position, targetDestination1.transform.position, move);
                        break;
	                case 7:
                    case 8:
                    case 9:
                        // Go towards destination 2
                        transform.position = Vector3.MoveTowards(transform.position, targetDestination2.transform.position, move);
                        break;
                    default:
                        // Something bad may be afoot.
                        break;
	            }
	        }
	    }
	    //else
	    //{
	    //    GameObject.Instantiate(GameClock.instance);
	    //}
	    
	}


}


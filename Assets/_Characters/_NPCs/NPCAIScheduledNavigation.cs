using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class NPCAIScheduledNavigation : MonoBehaviour {

    //Movement block
    public List<GameObject> targetDestinationList;
    private int currentDestinationIndex = 0;
    public float speed = 1;

    //Animation block
    public AnimationClip attackAnimation;
    private Animator animator;
    [SerializeField]
    AnimatorOverrideController animatorOverrideController;

    void Start()
    {
        if (GameClock.instance == null)
        {
            GameObject.Instantiate(GameClock.instance);
        }

        //Setup Overriding animator        
            animator = GetComponent<Animator>();

            
        

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
                        transform.position = Vector3.MoveTowards(transform.position, GetFirstGameObject().transform.position, move);
                        animator.SetTrigger("DestinationAnimation");
                        break;
	                case 7:
                    case 8:
                    case 9:
                        // Go towards destination 2
                        transform.position = Vector3.MoveTowards(transform.position, GetCurrentGameObject().transform.position, move);
                        break;
                    default:
                        // Something bad may be afoot. (Or a hand)
                        break;
	            }
	        }
	    }
        else
        {
            GameObject.Instantiate(GameClock.instance);
        }
    }

    private GameObject GetFirstGameObject()
    {
        GameObject returnGameObject = null;

        if (targetDestinationList.Count > 0)
        {
            returnGameObject = targetDestinationList[0];
        }
        return returnGameObject;
    }
    private GameObject GetCurrentGameObject()
    {
        GameObject returnGameObject = null;
        if (currentDestinationIndex == 0)
        {
            currentDestinationIndex++;
        }
        if (targetDestinationList.Count > 0 && currentDestinationIndex <= (targetDestinationList.Count -1))
        {
            returnGameObject = targetDestinationList[currentDestinationIndex];
        }
        return returnGameObject;
    }
}


using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class NPCAIScheduledNavigation : MonoBehaviour
{
    private const int HOURS_PER_DAY = 24;
    //Movement block
    public List<GameObject> targetDestinationList = new List<GameObject>();
    
    private int currentDestinationIndex = 0;
    public float speed = 1;

    //Animation block
    public AnimationClip destinationAnimation;
    public GameObject targetDestination;
    private Animator animator;
    [SerializeField]
    AnimatorOverrideController animatorOverrideController;

    //Scheduled Destinations Block
    
    void Start()
    {
        if (GameClock.instance == null)
        {
            GameObject.Instantiate(GameClock.instance);
        }
        //Setup Overriding animator        
        SetupRuntimeAnimator();
        if (targetDestinationList.Count < HOURS_PER_DAY)
        {
            for (int i = targetDestinationList.Count; i < HOURS_PER_DAY; ++i)
            {
                targetDestinationList.Insert(i, null);
            }
        }
    }

    // Update is called once per frame
    void Update ()
	{
	    if (GameClock.instance != null)
	    {
	        if (GameClock.instance.GetCurrentIsAM())
	        {
                // Retrieve current time from Unity Clock
	            float move = speed * Time.deltaTime;
	            int index = (GameClock.instance.GetCurrentHour());
                // Handle wrap-around for 24 hour clock
	            index = index > HOURS_PER_DAY ? index % 24 : index;
	            GameObject currentDestination = targetDestinationList[index];

	            if (currentDestination != null)
	            {
	                transform.position = Vector3.MoveTowards(transform.position, currentDestination.transform.position, move);
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

    private void SetupRuntimeAnimator()
    {
        animator = GetComponent<Animator>();

        animator.runtimeAnimatorController = animatorOverrideController;

       // animatorOverrideController["DEFAULT ATTACK"] = targetDestination.GetAttackAnimClip();
    }
}


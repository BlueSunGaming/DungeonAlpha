using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;
using Assets._Environment.destinations;

public class NPCAIScheduledNavigation : MonoBehaviour
{
    private const int HOURS_PER_DAY = 24;

    private AICharacterControl aiCharacterControl;

    //Movement block
    public List<GameObject> targetDestinationList = new List<GameObject>();
    private int currentDestinationIndex = 0;
    public float speed = 1;
   

    //Animation block
    public AnimationClip targetAnimation;
    public GameObject targetDestination;
    private Animator animator;
    [SerializeField]
    AnimatorOverrideController animatorOverrideController;
    [SerializeField]
    Destinations targetInUse;




    void Start()
    {

        if (GameClock.instance == null)
        {
            GameObject.Instantiate(GameClock.instance);
        }
       
        //Setup animator        
        SetupRuntimeAnimator();

        //Sync GameClock with target destination
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
        SetAnimation();
        

        //TODO Set npc to face walking direction instead of target position
        FaceTargetDestination();

        if (GameClock.instance != null)
	    {
	        if (GameClock.instance.GetCurrentIsAM())
	        {
                // Retrieve current time from Unity Clock
	            float move = speed * Time.deltaTime;
	            int index = (GameClock.instance.GetCurrentHour());

                // Handle wrap-around for 24 hour clock
	            currentDestinationIndex = index > HOURS_PER_DAY ? index % 24 : index;
                GameObject currentDestination = targetDestinationList[currentDestinationIndex];

	           // if (currentDestination != null)
	            //{
	            //    transform.position = Vector3.MoveTowards(transform.position, currentDestination.transform.position, move);
              //  }
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
       
      
    }

    private void FaceTargetDestination()

    {
        aiCharacterControl = GetComponent<AICharacterControl>();
        
        GameObject currentTargetDestination = targetDestinationList[currentDestinationIndex];

        Transform transformDestination = currentTargetDestination.transform;

        
        if (transform.position != currentTargetDestination.transform.position)
        {
            aiCharacterControl.SetTarget(transformDestination);
           
        }
        else
        {
            return;
        }
        }

    private void SetAnimation()
    {
        

        GameObject currentTargetDestination = targetDestinationList[currentDestinationIndex];
        //targetInUse = currentTargetDestination;
        if (currentTargetDestination != null)
        {
            if (transform.position != currentTargetDestination.transform.position)
            {
                animator.SetTrigger("moving");
            }
            else
            {
              
                animator.SetTrigger("combatIdle");
                animator.runtimeAnimatorController = animatorOverrideController;

                animatorOverrideController["combatIdle"] = targetInUse.GetDestinationAnimClip();
                //  Animator trigger is set to destination animation from Destinations.cs

            }
        }
        else
        {
            Debug.Log("There is no target destination set for the current Destination index " + currentDestinationIndex);
        }

    } 
}



using Assets.Scripts.Core.Interaction;
using Assets.Scripts.Testing;
using LogicSpawn.RPGMaker.Core;
using UnityEngine;
using System.Collections;

public class StashTrigger : InteractiveObjectMono
{
    void Awake()
    {
        Type = InteractableType.Interactable;
    }

    public override void Interaction()  
    {
        //MyGUI.Instance.EnableStashWindow = true;
    }

    protected override void OnEnable()
    {
        if (Initialised) return;
        Initialised = true;
    }

    public override void StopInteraction()          
    {   
        //MyGUI.Instance.EnableStashWindow = false;
    }
}

using System;
using System.Collections.Generic;
using LogicSpawn.RPGMaker.Beta;
using LogicSpawn.RPGMaker.Core;
using UnityEngine;

public class RPGAction
{
    public string ID;
    public readonly RPGActionType Type;
    public Dictionary<string, object> Params;
    public AnimationDefinition Animation;
    public AudioContainer Sound;
    public bool Cancellable;
    public bool UseDefaultAnimations;
    public bool UseQueuePosition;
    public bool FaceQueueTarget;
    public bool WhileActionIsActive;

    public RPGAction(RPGActionType actionType, Dictionary<string, object> parameters)
    {
        ID = Guid.NewGuid().ToString();
        Type = actionType;
        Params = parameters;
        Animation = null;
        Sound = null;
    }
    public RPGAction(RPGActionType actionType)
    {
        ID = Guid.NewGuid().ToString();
        Type = actionType;
        Params = new Dictionary<string, object>();
        Animation = null;
        Sound = null;
    }


    public RPGAction WithAnimation(string animationname, float speed = 1.0f, WrapMode wrapMode = WrapMode.Loop, bool backwards = false)
    {
        Animation = new AnimationDefinition { Animation = animationname, Speed = speed, WrapMode = wrapMode, Backwards = backwards };
        return this;
    }

    public RPGAction WithAnimation(AnimationDefinition animDef)
    {
        Animation = animDef;
        return this;
    }

    public RPGAction WithSound(AudioContainer audioContainer, bool whileActionIsActive = false)
    {
        Sound = audioContainer;
        if(whileActionIsActive)
        {
            WhileActionIsActive = true;
        }

        return this;
    }

    public RPGAction FacingPosition(Vector3 position)
    {
        if(!Params.ContainsKey("Position"))
        {
            Params.Add("Position", position);
        }
        else
        {
            Debug.LogWarning("Action already contains position!");
        }
        return this;
    }
    public RPGAction FacingPosition(BaseCharacterMono combatant)
    {
        if(!Params.ContainsKey("Combatant"))
        {
            Params.Add("Combatant", combatant);
        }
        else
        {
            Debug.LogWarning("Action already contains combatant!");
        }
        return this;
    }

    public RPGAction WithCancellable()
    {
        Cancellable = true;
        return this;
    }
    public RPGAction WithDefaultAnimations()
    {
        UseDefaultAnimations = true;
        return this;
    }

    public RPGAction UsingQueuePosition()
    {
        UseQueuePosition = true;
        return this;
    }

    public RPGAction FacingQueueTarget()
    {
        FaceQueueTarget = true;
        return this;
    }
}
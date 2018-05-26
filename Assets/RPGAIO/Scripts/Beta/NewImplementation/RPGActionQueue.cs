using System.Collections.Generic;
using LogicSpawn.RPGMaker.Beta;
using UnityEngine;

public class RPGActionQueue
{
    public List<RPGAction> Actions;
    public string SkillId;
    public string Identifier;
    public bool HasTarget;
    public BaseCharacterMono Target;
    public bool HasTargetPos;
    public Vector3 TargetPos;

    public Vector3 QueueTarget
    {
        get
        {
            if (HasTarget && Target != null) return Target.transform.position;
            if (HasTargetPos) return TargetPos;

            return Vector3.zero;
        }
    }


    public RPGActionQueue()
    {
        Actions = new List<RPGAction>();
        TargetPos = Vector3.zero;
        Identifier = "";
    }

    public static RPGActionQueue Create ()
    {
        return new RPGActionQueue();
    }

    public static RPGActionQueue Create(RPGActionType actionType, Dictionary<string, object> parameters )
    {
        var newQueue = new RPGActionQueue();
        newQueue.Actions.Add(new RPGAction(actionType, parameters));
        return newQueue;
    }
    public static RPGActionQueue Create(RPGAction action)
    {
        var newQueue = new RPGActionQueue();
        newQueue.Actions.Add(action);
        return newQueue;
    }

    public static RPGActionQueue Create(IEnumerable<RPGAction> newQueueActions)
    {
        var newQueue = new RPGActionQueue();
        newQueue.Actions.AddRange(newQueueActions);
        return newQueue;
    }

    public RPGActionQueue Add(RPGActionType actionType, Dictionary<string, object> moveParameters)
    {
        Actions.Add(new RPGAction(actionType, moveParameters));
        return this;
    }
    public RPGAction Add(RPGAction action)
    {
        Actions.Add(action);
        return action;
    }
}
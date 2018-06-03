using System;

namespace LogicSpawn.RPGMaker.Core
{
    public class InteractCondition : QuestCondition
    {
        public string ID;
        public bool IsNpc;
        public string InteractableID ;
        public string InteractionNodeTreeID ;

        public InteractCondition()
        {
            ID = Guid.NewGuid().ToString();
            ConditionType = ConditionType.Interact;
            IsNpc = true;
            InteractableID = "";
            InteractionNodeTreeID = "";
        }

    }
}
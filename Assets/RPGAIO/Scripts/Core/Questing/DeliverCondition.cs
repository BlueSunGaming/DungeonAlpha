using System;
using LogicSpawn.RPGMaker.Core;

namespace LogicSpawn.RPGMaker.Core
{
    public class DeliverCondition : QuestCondition
    {
        public string ID;
        public string ItemToDeliverID;
        public string InteractableToDeliverToID;
        public bool DeliverToNPC;
        public string InteractionNodeTreeID;

        public DeliverCondition()
        {
            ID = Guid.NewGuid().ToString();
            ConditionType = ConditionType.Deliver;
            DeliverToNPC = true;
            ItemToDeliverID = "";
            InteractableToDeliverToID = "";
            InteractionNodeTreeID = "";
        }
    }
}
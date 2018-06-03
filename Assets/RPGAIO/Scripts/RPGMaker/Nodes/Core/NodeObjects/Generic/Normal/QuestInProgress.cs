using System.Linq;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Quest", "")]
    public class QuestInProgress : BooleanNode
    {
        public override string Name
        {
            get { return "Quest In Progress"; }
        }

        public override string Description
        {
            get { return "Returns true if the quest is in progress."; }
        }

        public override string SubText
        {
            get { return ""; }
        }

        protected override void SetupParameters()
        {
            Add("Quest", PropertyType.Quest, null, "");
        }

        protected override bool Eval(NodeChain nodeChain)
        {
            var questID = (string)ValueOf("Quest");
            var quest = GetObject.PlayerSave.QuestLog.AllObjectives.FirstOrDefault(q => q.ID == questID);
            if (quest != null)
            {
                return quest.IsAccepted && !quest.TurnedIn;
            }

            return false;
        }
    }
}
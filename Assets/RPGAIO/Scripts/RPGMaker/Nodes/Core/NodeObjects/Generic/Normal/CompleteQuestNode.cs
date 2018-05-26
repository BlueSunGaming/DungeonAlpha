using System;
using System.Linq;
using LogicSpawn.RPGMaker.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Quest", "")]
    public class CompleteQuestNode : SimpleNode
    {
        public override string Name
        {
            get { return "Complete Quest"; }
        }

        public override string Description
        {
            get { return "Completes a quest."; }
        }

        public override string SubText
        {
            get { return "Mark a quest as done"; }
        }

        public override bool CanBeLinkedTo
        {
            get
            {
                return true;
            }
        }

        public override string NextNodeLinkLabel(int index)
        {
            return "Next";
        }

        protected override void SetupParameters()
        {
            Add("Quest", PropertyType.Quest,null, ""); //param value format: (index, index/string/value), e.g. 0,abc-123 => 0 represents attribute, abc-123 represents attribute
        }

        protected override void Eval(NodeChain nodeChain)
        {
            var questID = (string)ValueOf("Quest");
            var quest = GetObject.PlayerSave.QuestLog.AllObjectives.FirstOrDefault(q => q.ID == questID);
            QuestHandler.Instance.CompleteQuest(questID);

            if(quest != null)
            {
                var questInteraction = quest.CompletedDialogNodeTreeID;
                if (DialogHandler.Instance != null)
                {
                    DialogHandler.Instance.BeginQuestCompleteDialog(questInteraction, null, true,questID);
                }
            }
        }
    }
}
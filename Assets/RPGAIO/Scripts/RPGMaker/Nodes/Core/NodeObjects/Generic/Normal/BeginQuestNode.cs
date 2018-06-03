using System;
using System.Linq;
using LogicSpawn.RPGMaker.Generic;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Quest", "")]
    public class BeginQuestNode : SimpleNode
    {
        public override string Name
        {
            get { return "Begin Quest"; }
        }

        public override string Description
        {
            get { return "Begins a quest."; }
        }

        public override string SubText
        {
            get { return "Start a new quest"; }
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
            if(quest != null)
            {
                if(QuestHandler.Instance.BeginQuest(quest.ID))
                {
                    var questInteraction = quest.DialogNodeTreeID;
                    if (DialogHandler.Instance != null) 
                    {
                        DialogHandler.Instance.BeginQuestDialog(questInteraction, null, true, questID);
                    }
                }
            }
        }
    }
}
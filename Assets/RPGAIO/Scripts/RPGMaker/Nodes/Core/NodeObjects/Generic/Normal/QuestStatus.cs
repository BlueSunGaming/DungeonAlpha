using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    [NodeCategory("Quest", "")]
    public class QuestStatus : OptionsNode
    {
        protected override void SetupNextLinks()
        {
            NextNodeLinks = new List<StringField> { new StringField(), new StringField(), new StringField(), new StringField(), new StringField() };
        }

        public override bool CanRemoveLinks
        {
            get { return false; }
        }

        public override string Name
        {
            get { return "Quest Status"; }
        }

        public override string Description
        {
            get { return "Continues based on status of quest"; }
        }

        public override string SubText
        {
            get { return ""; }
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
            switch (index)
            {
                case 0:
                    return "Startable";

                case 1:
                    return "In Progress";

                case 2:
                    return "Completable";

                case 3:
                    return "Completed";

                case 4:
                    return "Not Found";
            }

            return "";
        }

        protected override void SetupParameters()
        {
            Add("Quest", PropertyType.Quest, null, "");
        }

        protected override int Eval(NodeChain nodeChain)
        {
            var questID = (string)ValueOf("Quest");
            var quest = GetObject.PlayerSave.QuestLog.AllObjectives.FirstOrDefault(q => q.ID == questID);
            if (quest != null)
            {
                if (!quest.IsAccepted && !quest.TurnedIn) //startable
                {
                    return 0;
                }
                else if (!quest.ConditionsMet && quest.IsAccepted && !quest.TurnedIn) //inprogress
                {
                    return 1;
                }
                else if (quest.ConditionsMet && quest.IsAccepted && !quest.TurnedIn) //completable
                {
                    return 2;
                }
                else if (quest.TurnedIn) //completed
                {
                    return 3;
                }
                else //notfound
                {
                    return 4;
                }
            }

            return 4;
        }
    }
}   
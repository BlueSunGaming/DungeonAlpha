using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.API;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class Quest
    {
        public string ID;
        public string Name;
        public string Description;
        public ImageContainer Image;
        public string DialogNodeTreeID;
        public string CompletedDialogNodeTreeID;
        public QuestConditionMode ConditionMode;

        public bool Repeatable;

        public bool HasTimeLimit;
        public float TimeLimit;
        public float CurrentTimeLimit;

        public bool RunEventOnAccept;
        public string EventOnAcceptID;

        public bool RunEventOnComplete;
        public string EventOnCompletionId;

        public bool RunEventOnCancel;
        public string EventOnCancelId;

        public bool PlayerKeepsQuestItems;

        public QuestRequirements Requirements;
        [JsonIgnore]
        //todo: implementation
        public bool RequirementsMet
        {
            get { return true; }
        }

        public List<QuestCondition> Conditions;
        public bool HasFinalCondition;
        public QuestCondition FinalCondition;
        [JsonIgnore]
        public bool ReadyForFinal
        {
            get { return Conditions.TrueForAll(o => o.IsDone); }
        }

        public QuestReward Rewards;

        //todo: Active conditions


        public List<Rm_CustomVariableGetSet> SetCustomVariablesOnCompletion;


        public bool HasBonusCondition ;
        public QuestCondition BonusCondition ;
        public QuestReward BonusRewards ;
        public bool BonusHasTimeLimit;
        public float BonusTimeLimit;
        public float BonusCurrentTimeLimit;

        public bool Failed;
        public bool FailedBonus
        {
            get
            {              
                return !BonusCondition.IsDone && BonusHasTimeLimit && BonusCurrentTimeLimit <= 0;
            }
        }

        
        public bool TrackSteps;
        public bool IsAccepted;
        public bool TurnedIn;
        public string QuestChainId;
        public bool CanAbandon;

        [JsonIgnore]
        public bool ConditionsMet
        {
            get
            {
                return HasFinalCondition ? Conditions.TrueForAll(o => o.IsDone) && FinalCondition.IsDone
                    : Conditions.TrueForAll(o => o.IsDone);
            }
        }

        [JsonIgnore]
        //todo: finish implementation
        public List<QuestCondition> ActiveConditions
        {
            get
            {
                var activeConditions = new List<QuestCondition>();

                if(ConditionMode == QuestConditionMode.AllAtOnce)
                {
                    if(HasFinalCondition)
                    {
                        if(ReadyForFinal)
                        {
                            activeConditions.Add(FinalCondition);
                        }
                        else
                        {
                            activeConditions.AddRange(Conditions);
                        }
                    }
                    else
                    {
                        activeConditions.AddRange(Conditions);
                    }


                    if (HasBonusCondition && !FailedBonus)
                        activeConditions.Add(BonusCondition);
                }
                else
                {
                    //note: here we don't return itemCondition that is done, so the next condition can work
                    var lastCondition = Conditions.FirstOrDefault(c => !c.IsDone) ?? Conditions.Last();
                    lastCondition = lastCondition.IsDone && HasFinalCondition ? FinalCondition : lastCondition;
                    activeConditions.Add(lastCondition);

                    if(HasBonusCondition)
                        activeConditions.Add(BonusCondition);
                }
                
                return activeConditions;
            }
        }

        [JsonIgnore]
        //todo: finish implementation
        public List<QuestCondition> AllConditions
        {
            get
            {
                var allConditions = new List<QuestCondition>();
                allConditions.AddRange(Conditions);
                if(HasBonusCondition)
                    allConditions.Add(BonusCondition);
                if(HasFinalCondition)
                    allConditions.Add(FinalCondition);

                return allConditions;
            }
        }

        public Quest()
        {
            ID = Guid.NewGuid().ToString();
            Name = "" +
                   "New Quest";
            QuestChainId = "";
            ConditionMode = QuestConditionMode.AllAtOnce;
            Requirements = new QuestRequirements();
            Rewards = new QuestReward();
            Conditions = new List<QuestCondition>();
            BonusCondition = new KillCondition();
            BonusRewards = new QuestReward();
            FinalCondition = new KillCondition();
            Image = new ImageContainer();
            IsAccepted = TurnedIn = false;
            TrackSteps = true;
            TimeLimit = 0;
            Repeatable = false;
            CanAbandon = true;
            SetCustomVariablesOnCompletion = new List<Rm_CustomVariableGetSet>();
        }

        public string TrackerDetails(bool forUpdateFunction, bool addDashes)
        {
            var text = "";
            if(!Failed)
            {
                foreach (var activeCondition in ActiveConditions.Where(c => c != BonusCondition))
                {
                    text += (addDashes ? " - " : "") + TrackerDetailsForCondition(activeCondition);
                }
            }
            if (HasTimeLimit)
            {
                text += forUpdateFunction ? (Failed ? "Failed" : "Time Left: " + (int)CurrentTimeLimit) : (Failed ? "Failed" : "Time Limited") + "\n";
            }

            if(HasBonusCondition)
            {
                //text += string.IsNullOrEmpty(text) ? "" : "\n\n";
                if(!FailedBonus)
                {
                    text += "Bonus:" + "\n";
                    text += (addDashes ? " - " : "") + TrackerDetailsForCondition(BonusCondition);
                    if (BonusHasTimeLimit)
                    {
                        text += forUpdateFunction ? (addDashes ? " - " : "") + "Time Left: " + (int)BonusCurrentTimeLimit : (addDashes ? " - " : "") + "Time Limited" + "\n";
                    }
                }
                else
                {
                    text += "Bonus: Failed" + "\n";
                }
                
            }
            
            return "\n" + text;
        }

        public string TrackerDetailsForCondition(QuestCondition activeCondition)
        {
            var text = "";
            switch (activeCondition.ConditionType)
            {
                case ConditionType.Kill:
                    var killCondition = (KillCondition)activeCondition;
                    var mobName = killCondition.IsNPC ? RPG.Npc.GetNpcName(killCondition.CombatantID) : RPG.Npc.GetEnemyName(killCondition.CombatantID);
                    if (killCondition.UseCustomText)
                        text += string.Format("{0}", (killCondition.IsDone ? killCondition.CustomCompletedText : killCondition.CustomText)) + "\n";
                    else
                        text += string.Format("Kill {0}: {1}/{2}", mobName, killCondition.NumberKilled, killCondition.NumberToKill) + "\n";
                    break;
                case ConditionType.Item:
                    var itemCondition = (ItemCondition)activeCondition;
                    var itemName = RPG.Items.GetItemName(itemCondition.ItemToCollectID);
                    if (itemCondition.UseCustomText)
                        text += string.Format("{0}", (itemCondition.IsDone ? itemCondition.CustomCompletedText : itemCondition.CustomText)) + "\n";
                    else
                        text += string.Format("{0}: {1}/{2}", itemName, itemCondition.NumberObtained, itemCondition.NumberToObtain) + "\n";
                    break;
                case ConditionType.Interact:
                    var interactCondition = (InteractCondition)activeCondition;
                    var interactNpcName = interactCondition.IsNpc ? RPG.Npc.GetNpcName(interactCondition.InteractableID) : RPG.Npc.GetInteractableName(interactCondition.InteractableID);
                    if (interactCondition.UseCustomText)
                        text += string.Format("{0}", (interactCondition.IsDone ? interactCondition.CustomCompletedText : interactCondition.CustomText)) + "\n";
                    else
                        text += string.Format("Interact with {0}: {1}", interactNpcName, (interactCondition.IsDone ? "Done" : "Not Done")) + "\n";
                    break;
                case ConditionType.Deliver:
                    var deliverCondition = (DeliverCondition)activeCondition;
                    var deliverItemName = RPG.Items.GetItemName(deliverCondition.ItemToDeliverID);
                    var deliverNpcName = deliverCondition.DeliverToNPC ? RPG.Npc.GetNpcName(deliverCondition.InteractableToDeliverToID) : RPG.Npc.GetInteractableName(deliverCondition.InteractableToDeliverToID);
                    if (deliverCondition.UseCustomText)
                        text += string.Format("{0}", (deliverCondition.IsDone ? deliverCondition.CustomCompletedText : deliverCondition.CustomText)) + "\n";
                    else
                        text += string.Format("Deliver {0} to {1}: {2}", deliverItemName, deliverNpcName, (deliverCondition.IsDone ? "Done" : "Not Done")) + "\n";
                    break;
                case ConditionType.Custom:
                    var condition = (CustomCondition)activeCondition;
                    text += string.Format("{0}", (condition.IsDone ? condition.CustomCompletedText : condition.CustomText)) + "\n";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return text;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public enum QuestConditionMode
    {
        AllAtOnce,
        Consecutive
    }
}
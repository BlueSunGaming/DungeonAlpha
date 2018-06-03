using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using System.Collections;

public class QuestHandler : MonoBehaviour
{

    public static QuestHandler Instance;

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
	public bool BeginQuest (Quest quest)
	{
	    var itemsToAddToInv = new List<Item>();
        foreach (var q in quest.AllConditions)
        {
            if (q == null) continue;

            var deliverCondition = q as DeliverCondition;
            if (deliverCondition != null)
            {
                var item = Rm_RPGHandler.Instance.Repositories.QuestItems.Get(deliverCondition.ItemToDeliverID);
                if (item != null)
                {
                    itemsToAddToInv.Add(item);
                }
            }
        }

	    if(!GetObject.PlayerCharacter.Inventory.CanAddItems(itemsToAddToInv))
	    {
	        return false;
	    }

        quest.IsAccepted = true;
        foreach(var item in itemsToAddToInv)
        {
            GetObject.PlayerCharacter.Inventory.AddItem(item);
        }

        if(quest.HasTimeLimit)
        {
            quest.CurrentTimeLimit = quest.TimeLimit;
        }

        if(quest.HasBonusCondition && quest.BonusHasTimeLimit)
        {
            quest.BonusCurrentTimeLimit = quest.BonusTimeLimit;
        }
        AudioPlayer.Instance.Play(Rm_RPGHandler.Instance.Questing.QuestStarted.Audio, AudioType.SoundFX,Vector3.zero);
	    RPG.Events.OnQuestStatusUpdate();
	    return true;
	}

    public bool BeginQuest(string QuestID)
    {
        var quest = GetObject.PlayerSave.QuestLog.GetObjective(QuestID);
        return BeginQuest(quest);
    }
	
	// Update is called once per frame
	void Update ()
	{
	    CheckItemConditionState();
	    CheckCustomConditionState();
	    HandleTimeLimits();
	}

    private void HandleTimeLimits()
    {
        var activeQuests = GetObject.PlayerSave.QuestLog.ActiveObjectives;
        foreach(var quest in activeQuests)
        {
            if (quest.HasTimeLimit)
            {
                quest.CurrentTimeLimit -= Time.deltaTime;

                if(quest.CurrentTimeLimit <= 0)
                {
                    quest.Failed = true;
                }
            }

            if (quest.HasBonusCondition && quest.BonusHasTimeLimit)
            {
                quest.BonusCurrentTimeLimit -= Time.deltaTime;

                if (quest.BonusCurrentTimeLimit <= 0)
                {
//                    quest.FailedBonus = true;
                }
            }
        }
        
    }

    public void AbandonQuest(Quest quest)
    {
        var allConditions = quest.AllConditions;
        foreach (var condition in allConditions.Where(c => c.ConditionType == ConditionType.Kill))
        {
            var killCondition = condition as KillCondition;
            killCondition.NumberKilled = 0;
            killCondition.IsDone = false;
        }
        foreach (var condition in allConditions.Where(c => c.ConditionType == ConditionType.Interact))
        {
            var interactCondition = condition as InteractCondition;
            interactCondition.IsDone = false;

        }
        foreach (var condition in allConditions.Where(c => c.ConditionType == ConditionType.Item))
        {
            var itemCondition = condition as ItemCondition;
            itemCondition.IsDone = false;
            itemCondition.NumberObtained = 0;

        }
        foreach (var condition in allConditions.Where(c => c.ConditionType == ConditionType.Deliver))
        {
            var delivCondition = condition as DeliverCondition;
            delivCondition.IsDone = false;
        }

        quest.IsAccepted = quest.TurnedIn = false;
        quest.Failed = false;

        var itemConditions = quest.AllConditions.Where(s => (s as ItemCondition) != null).Select(s => s as ItemCondition).ToList();
        foreach (var condition in itemConditions)
        {
            var existingItem =
                GetObject.PlayerCharacter.Inventory.AllItems.FirstOrDefault(i => i.ID == condition.ItemToCollectID);

            if (existingItem != null)
            {
                GetObject.PlayerCharacter.Inventory.RemoveItem(existingItem);
            }
        }

        var deliverConditions = quest.AllConditions.Where(s => (s as DeliverCondition) != null).Select(s => s as DeliverCondition).ToList();
        foreach (var condition in deliverConditions)
        {
            var existingItem =
                GetObject.PlayerCharacter.Inventory.AllItems.FirstOrDefault(i => i.ID == condition.ItemToDeliverID);

            if (existingItem != null)
            {
                GetObject.PlayerCharacter.Inventory.RemoveItem(existingItem);
            }
        }

        RPG.Events.OnQuestStatusUpdate();
    }

    private void CheckCustomConditionState()
    {
        var activeConditions = GetObject.PlayerSave.QuestLog.ActiveObjectives.SelectMany(c => c.ActiveConditions).Where(c => !c.IsDone);
        var customConditions = activeConditions.Where(s => (s as CustomCondition) != null).Select(s => s as CustomCondition).ToList();
        foreach (var condition in customConditions)
        {
            var customVarID = condition.CustomVariableRequirement.VariableID;
            var customVar = GetObject.PlayerSave.GenericStats.CustomVariables.FirstOrDefault(c => c.ID == customVarID);
            var conditionReq = condition.CustomVariableRequirement;
            if(customVar == null)
            {
                Debug.LogError("Quest condition found which is dependent on custom var which does not exist. Condition Text:" + condition.CustomText);
                continue;
            }

            var oldCondition = condition.IsDone;
            switch (customVar.VariableType)
            {
                case Rmh_CustomVariableType.Float:
                    condition.IsDone = customVar.FloatValue == conditionReq.FloatValue;
                    break;
                case Rmh_CustomVariableType.Int:
                    condition.IsDone = customVar.IntValue == conditionReq.IntValue;
                    break;
                case Rmh_CustomVariableType.Bool:
                    condition.IsDone = customVar.BoolValue == conditionReq.BoolValue;
                    break;
                case Rmh_CustomVariableType.String:
                    condition.IsDone = customVar.StringValue == conditionReq.StringValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (oldCondition != condition.IsDone)
            {
                RPG.Events.OnQuestStatusUpdate();
            }
        }

    }

    private void CheckItemConditionState()
    {
        var activeConditions = GetObject.PlayerSave.QuestLog.ActiveObjectives.SelectMany(c => c.ActiveConditions);
        var itemConditions = activeConditions.Where(s => (s as ItemCondition) != null).Select(s => s as ItemCondition).ToList();
        foreach(var condition in itemConditions)
        {
            var existingItems = GetObject.PlayerCharacter.Inventory.AllItems.Where(i => i.ID == condition.ItemToCollectID).ToList();
            var existingItem = existingItems.FirstOrDefault();
            
            if(existingItem == null)
            {
                condition.NumberObtained = 0;
            }
            else
            {
                var stack = existingItem as IStackable;
                if(stack != null)
                {
                    condition.NumberObtained = stack.CurrentStacks;
                }
                else
                {
                    condition.NumberObtained = existingItems.Count();
                }
            }

            var oldCondition = condition.IsDone;
            condition.IsDone = condition.NumberObtained >= condition.NumberToObtain;
            if(oldCondition != condition.IsDone)
            {
                RPG.Events.OnQuestStatusUpdate();
            }
        }

    }

    public void CompleteQuest(string questID)
    {
        var quest = GetObject.PlayerSave.QuestLog.GetObjective(questID);
        quest.IsAccepted = quest.TurnedIn = true;
        quest.Failed = false;

        var itemConditions = quest.AllConditions.Where(s => (s as ItemCondition) != null).Select(s => s as ItemCondition).ToList();
        if(!quest.PlayerKeepsQuestItems)
        {
            foreach (var condition in itemConditions)
            {
                var existingItem =
                    GetObject.PlayerCharacter.Inventory.AllItems.FirstOrDefault(i => i.ID == condition.ItemToCollectID);

                if (existingItem != null)
                {
                    GetObject.PlayerCharacter.Inventory.RemoveItem(existingItem);
                }
            }
        }

        var deliverConditions = quest.AllConditions.Where(s => (s as DeliverCondition) != null).Select(s => s as DeliverCondition).ToList();
        if (!quest.PlayerKeepsQuestItems)
        {
            foreach (var condition in deliverConditions)
            {
                var existingItem =
                    GetObject.PlayerCharacter.Inventory.AllItems.FirstOrDefault(i => i.ID == condition.ItemToDeliverID);

                if (existingItem != null)
                {
                    GetObject.PlayerCharacter.Inventory.RemoveItem(existingItem);
                }
            }
        }

        if(quest.HasBonusCondition && quest.BonusCondition.IsDone)
        {
            GetObject.PlayerCharacter.AddExp(quest.BonusRewards.Exp);
            GetObject.PlayerCharacter.Inventory.AddGold(quest.BonusRewards.Gold);
        }

        //todo: hand out rewards
        if(quest.Rewards.GivesReputation)
        {
            GetObject.PlayerSave.QuestLog.AddReputation(quest.Rewards.Reputation);
        }
        if(quest.Rewards.ApplysStatusEffect)
        {
            GetObject.PlayerCharacter.AddStatusEffect(quest.Rewards.StatusEffectID);
        }
        if(quest.Rewards.GivesTraitExp)
        {
            GetObject.PlayerCharacter.AddTraitExp(quest.Rewards.TraitID, quest.Rewards.TraitExp);    
        }
        if(quest.Rewards.UnlocksSkill)
        {
            var skillToUnlock = GetObject.PlayerCharacter.SkillHandler.AvailableSkills.First(s => s.ID == quest.Rewards.SkillID);
            skillToUnlock.Unlocked = true;
        }


        GetObject.PlayerCharacter.AddExp(quest.Rewards.Exp);
        GetObject.PlayerCharacter.Inventory.AddGold(quest.Rewards.Gold);

        //todo: check if we have space
        foreach(var itemReward in quest.Rewards.Items)
        {
            var item = Rm_RPGHandler.Instance.Repositories.Items.Get(itemReward.ItemID);
            var stackable = item as IStackable;
            if(stackable != null)
            {
                stackable.CurrentStacks = itemReward.Amount;
            }
            GetObject.PlayerCharacter.Inventory.AddItem(GeneralMethods.CopyObject(item));
        }

        foreach(var itemReward in quest.Rewards.CraftableItems)
        {
            var item = Rm_RPGHandler.Instance.Repositories.CraftableItems.Get(itemReward.ItemID);
            var stackable = item as IStackable;
            if(stackable != null)
            {
                stackable.CurrentStacks = itemReward.Amount;
            }
            GetObject.PlayerCharacter.Inventory.AddItem(GeneralMethods.CopyObject(item));
        }

        foreach(var itemReward in quest.Rewards.QuestItems)
        {
            var item = Rm_RPGHandler.Instance.Repositories.QuestItems.Get(itemReward.ItemID);
            var stackable = item as IStackable;
            if(stackable != null)
            {
                stackable.CurrentStacks = itemReward.Amount;
            }
            GetObject.PlayerCharacter.Inventory.AddItem(GeneralMethods.CopyObject(item));
        }

        AudioPlayer.Instance.Play(Rm_RPGHandler.Instance.Questing.QuestComplete.Audio, AudioType.SoundFX, Vector3.zero);

        //repeatable
        if(quest.Repeatable)
        {
            AbandonQuest(quest);
        }
        else
        {
            RPG.Events.OnQuestStatusUpdate();
        }
    }
}

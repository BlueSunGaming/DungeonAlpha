using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestLogUI : MonoBehaviour
{
    public static QuestLogUI Instance;
    public GameObject QuestListContainer;
    public GameObject QuestRewardsContainer;
    public Text AdditionalQuestRewardsText;
    public GameObject QuestRewardPrefab;
    public GameObject QuestEntryPrefab;
    public GameObject QuestCategoryPrefab;
    public Button AbandonQuestButton;
    public Text TrackQuestText;
    public Text QuestName;
    public Text QuestRequirement;
    public Text QuestDescription;

    public Quest SelectedQuestEntry;
    public bool Show;
    public bool LoadedQuests;
    public bool ShowActive;
    public bool ShowComplete;

    private EventSystem EventSystem
    {
        get { return UIHandler.Instance.EventSystem; }
    }

	// Use this for initialization
	void Awake () {
	    Instance = this;
        QuestRewardsContainer.transform.DestroyChildren();
	    QuestDescription.text = "";
	    QuestRequirement.text = "";
	    QuestName.text = "";
	    AdditionalQuestRewardsText.text = "";
	}


    void Update()
    {
    }

    public void ToggleTrackQuest()
    {
        if(SelectedQuestEntry != null)
        {
            SelectedQuestEntry.TrackSteps = !SelectedQuestEntry.TrackSteps;
            TrackQuestText.text = SelectedQuestEntry.TrackSteps ? "Tracking: On" : "Tracking: Off";
        }
    }
    public void AbandonQuest()
    {
        if(SelectedQuestEntry != null)
        {
            if(SelectedQuestEntry.CanAbandon)
            {
                QuestHandler.Instance.AbandonQuest(SelectedQuestEntry);
                ClearSelected();
                LoadQuests();
            }
        }
    }

    public void ToggleShowActive(bool show)
    {
        ShowActive = !ShowActive;

        //Clear selected
        ClearSelected();

        LoadQuests();
    }

    private void ClearSelected()
    {
        SelectedQuestEntry = null;
        QuestName.text = "";
        QuestRequirement.text = "";
        QuestRewardsContainer.transform.DestroyChildren();
        QuestDescription.text = "";
        AdditionalQuestRewardsText.text = "";
    }

    public void ToggleShowCompleted(bool show)
    {
        ShowComplete = !ShowComplete;

        //Clear selected
        ClearSelected();

        LoadQuests();
    }



    public void SelectQuestEntry(string questId)
    {
        UpdateSelectedQuest(questId);
    }

    private void UpdateSelectedQuest(string questId)
    {
        var quest = RPG.GetPlayerSave.QuestLog.GetObjective(questId);
        QuestName.text = quest.Name;
        QuestRequirement.text = quest.Requirements.RequireLevel ? "Level " + quest.Requirements.LevelRequired : "";
        QuestRewardsContainer.transform.DestroyChildren();

        var itemRewards = quest.Rewards.Items.Concat(quest.Rewards.CraftableItems).Concat(quest.Rewards.QuestItems);
        foreach (var reward in itemRewards)
        {
            var item = RPG.Items.GetItem(reward.ItemID) ?? RPG.Items.GetCraftableItem(reward.ItemID) ?? RPG.Items.GetQuestItem(reward.ItemID);
            var amount = reward.Amount;

            var go = Instantiate(QuestRewardPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            go.transform.SetParent(QuestRewardsContainer.transform, false);
            var itemModel = go.GetComponent<QuestRewardModel>();
            itemModel.Init(item, amount);
        }

        QuestDescription.text = quest.Description;

        QuestDescription.text += "\n";

        QuestDescription.text += quest.TrackerDetails(false, false);

        var r = quest.Rewards;
        AdditionalQuestRewardsText.text = "";

        if (quest.Rewards.Exp > 0)
            AdditionalQuestRewardsText.text += "- " + r.Exp + " Exp" + "\n";

        if (quest.Rewards.Gold > 0)
            AdditionalQuestRewardsText.text += "- " + r.Gold + " Gold" + "\n";

        if (quest.Rewards.GivesTraitExp)
            AdditionalQuestRewardsText.text += "- " + r.TraitExp + "  " + RPG.Stats.GetTraitName(r.TraitID) + " Exp" + "\n";

        if (quest.Rewards.ApplysStatusEffect)
            AdditionalQuestRewardsText.text += "- Applies " + RPG.Stats.GetStatusEffectName(r.StatusEffectID) + "\n";

        if (quest.Rewards.GivesReputation)
            AdditionalQuestRewardsText.text += "- " + r.Reputation.Value + "  " + RPG.Stats.GetReputationName(r.Reputation.ReputationID) + " Rep" + "\n";

        if (quest.Rewards.UnlocksSkill)
            AdditionalQuestRewardsText.text += "- Unlocks " + RPG.Combat.GetSkillName(r.SkillID) + "\n";

        if(quest.CanAbandon)
        {
            AbandonQuestButton.interactable = true;
        }

        if(quest.TrackSteps)
        {
            TrackQuestText.text = quest.TrackSteps ? "Tracking: On" : "Tracking: Off";
        }

        SelectedQuestEntry = quest;
    }

    public void ToggleQuestLogUI()
    {
        Show = !Show;
        if(Show)
        {
            if (!LoadedQuests)
            {
                LoadQuests();
            }

            ClearSelected();
        }
        else
        {
            LoadedQuests = false;
        }
    }

    private void LoadQuests()
    {
        var allQuests = RPG.GetPlayerSave.QuestLog.AllObjectives;
        QuestListContainer.transform.DestroyChildren();

        //Active
        var activeQuests = allQuests.Where(c => c.IsAccepted && !c.TurnedIn).ToList();
        if(ShowActive && activeQuests.Any())
        {
            var go = Instantiate(QuestCategoryPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            go.transform.SetParent(QuestListContainer.transform, false);
            var categoryName = go.GetComponent<Text>();
            categoryName.text = "- Active Quests -";

            foreach (var item in activeQuests)
            {
                var go1 = Instantiate(QuestEntryPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                go1.transform.SetParent(QuestListContainer.transform, false);
                var itemModel = go1.GetComponent<QuestEntryModel>();
                itemModel.Init(item);
            }
        }

        //Complete
        var completeQuests = allQuests.Where(c => c.TurnedIn).ToList();
        if (ShowComplete && completeQuests.Any())
        {
            var go = Instantiate(QuestCategoryPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            go.transform.SetParent(QuestListContainer.transform, false);
            var categoryName = go.GetComponent<Text>();
            categoryName.text = "- Completed Quests -";

            foreach (var item in completeQuests)
            {
                var go1 = Instantiate(QuestEntryPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                go1.transform.SetParent(QuestListContainer.transform, false);
                var itemModel = go1.GetComponent<QuestEntryModel>();
                itemModel.Init(item);
            }
        }
    }

    public void CloseQuestLog()
    {
        Show = false;
        LoadedQuests = false;
    }
}

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

public class AbilityLogUI : MonoBehaviour
{
    public enum AbilityType
    {
        Skill, Talent, Trait
    }

    public static AbilityLogUI Instance;
    public AbilityType CurrentType;
    public GameObject AbiliyListContainer;
    public GameObject AbilityEntryPrefab;
    public GameObject ShowHideUnlockedPanel;
    public Text SectionTitle;
    public Text RankUpRequirement;
    public Button RankUpButton;
    public Text RankUpButtonText;
    public Button ToggleTalentButton;
    public Text ToggleTalentButtonText;
    public Text AbilityTitle;
    public Text AbilitySubTitle;
    public Text AbilityDescription;

    public object SelectedAbility;
    public bool Show;
    public bool LoadedAbilities;
    public Toggle ShowNotYetUnlockedToggle;
    public bool ShowNotYetUnlocked = true;

    private EventSystem EventSystem
    {
        get { return UIHandler.Instance.EventSystem; }
    }

	// Use this for initialization
	void Awake () {
	    Instance = this;
	    AbilityDescription.text = "";   
	    AbilitySubTitle.text = "";
	    AbilityTitle.text = "";
	    RankUpRequirement.text = "";
	    ShowNotYetUnlockedToggle.isOn = ShowNotYetUnlocked;
	}

    void OnEnable()
    {
        RPG.Events.GainedSkillExp += UpdateTitle;
    }

    private void UpdateTitle(object sender, RPGEvents.GainedSkillExpEventArgs e)
    {
        UpdateTitle();
    }

    void OnDisable()
    {
        RPG.Events.GainedSkillExp -= UpdateTitle;
    }

    void Update()
    {
        if (SelectedAbility == null)
        {
            RankUpButton.gameObject.SetActive(false);
            ToggleTalentButton.gameObject.SetActive(false);
            return;
        }
        
        if(CurrentType == AbilityType.Skill)
        {
            var skill = SelectedAbility as Skill;
            var canRankUp = skill != null && (skill.CanUpgrade(GetObject.PlayerCharacter) || skill.CanUnlock(GetObject.PlayerCharacter));
            RankUpButtonText.text = skill != null && skill.Unlocked ? "Rank Up" : "Unlock";
            RankUpButton.gameObject.SetActive(true);
            RankUpButton.interactable = canRankUp;
            if(skill.IsMaxxedOut)
            {
                RankUpButton.gameObject.SetActive(false);
            }
            ToggleTalentButton.gameObject.SetActive(false);
            ShowHideUnlockedPanel.gameObject.SetActive(true);
        }
        else if(CurrentType == AbilityType.Talent)
        {
            var talent = SelectedAbility as Talent;
            var canRankUp = talent != null && talent.CanUpgrade(GetObject.PlayerCharacter);
            var canToggle = talent != null && talent.CanToggle;
            RankUpButtonText.text = talent != null && talent.Learnt ? "Rank Up" : "Learn";
            RankUpButton.gameObject.SetActive(true);
            RankUpButton.interactable = canRankUp;
            if (talent.IsMaxxedOut)
            {
                RankUpButton.gameObject.SetActive(false);
            }
            ToggleTalentButton.gameObject.SetActive(talent != null && talent.Learnt && canToggle);
            ShowHideUnlockedPanel.gameObject.SetActive(true);
        }
        else
        {
            RankUpButton.gameObject.SetActive(false);
            ToggleTalentButton.gameObject.SetActive(false);
            ShowHideUnlockedPanel.gameObject.SetActive(false);
        }
    }

    public void SwitchToSkills()
    {
        CurrentType = AbilityType.Skill;
        UpdateTitle();
        ClearSelected();
        LoadAbilities();
    }
    public void SwitchToTalents()
    {
        CurrentType = AbilityType.Talent;
        UpdateTitle();
        ClearSelected();
        LoadAbilities();
    }
    public void SwitchToTraits()
    {
        CurrentType = AbilityType.Trait;
        UpdateTitle();
        ClearSelected();
        LoadAbilities();
    }

    public void UpdateTitle()
    {
        switch(CurrentType)
        {
            case AbilityType.Skill:
                if (GetObject.PlayerCharacter.SkillHandler.AvailableSkills.Any(s => s.UpgradeType == SkillUpgradeType.SkillPoints))
                    SectionTitle.text = "Skills - " + GetObject.PlayerCharacter.CurrentSkillPoints;
                else
                    SectionTitle.text = "Skills";
                break;
            case AbilityType.Talent:
                if (GetObject.PlayerCharacter.TalentHandler.Talents.Any(s => s.UpgradeType == SkillUpgradeType.SkillPoints))
                    SectionTitle.text = "Talents - " + GetObject.PlayerCharacter.CurrentSkillPoints;
                else
                    SectionTitle.text = "Talents";
                break;
            case AbilityType.Trait:
                SectionTitle.text = "Traits";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void RankUp()
    {
        if(SelectedAbility != null)
        {
            var skill = SelectedAbility as Skill;
            var talent = SelectedAbility as Talent;
            var player = GetObject.PlayerCharacter;

            if(skill != null)
            {
                if (!skill.Unlocked)
                {
                    if (skill.CanUnlock(player))
                    {
                        skill.Unlock(player);
                    }
                }
                else
                {
                    if (skill.CanUpgrade(player))
                    {
                        skill.Upgrade(player);
                    }
                }
                SelectAbility(skill.ID);

            }
            else
            {
                if (!talent.Learnt)
                {
                    if (talent.CanUnlock(player))
                    {
                        talent.Unlock(player);
                    }
                }
                else
                {
                    if (talent.CanUpgrade(player))
                    {
                        talent.Upgrade(player);
                    }
                }
                SelectAbility(talent.ID);

            }
        }
    }
    public void ToggleTalent()
    {
        if (SelectedAbility != null)
        {
            var talent = SelectedAbility as Talent;
            talent.IsActive = !talent.IsActive;
            ToggleTalentButtonText.text = talent.IsActive ? "Turn Off" : "Turn On";
        }
    }

    public void ToggleShowNotUnlocked(bool show)
    {
        ShowNotYetUnlocked = !ShowNotYetUnlocked;

        //Clear selected
        ClearSelected();

        LoadAbilities();
    }

    private void ClearSelected()
    {
        SelectedAbility = null;
        AbilityTitle.text = "";
        AbilitySubTitle.text = "";
        AbilityDescription.text = "";
        RankUpRequirement.text = "";
    }
    
    public void SelectAbility(string abilityId)
    {
        switch(CurrentType)
        {
            case AbilityType.Skill:
                var skill = GetObject.PlayerCharacter.SkillHandler.AvailableSkills.First(s => s.ID == abilityId);
                var rankUpText = skill.CanUpgrade(GetObject.PlayerCharacter) || skill.CanUnlock(GetObject.PlayerCharacter) ? GetRankUpRequirement(skill) : "";
                rankUpText = skill.Unlocked ? rankUpText : GetUnlockRequirement(skill);
                UpdateSelectedQuest(skill.Name + string.Format(" [Rank {0}]", skill.CurrentRank + 1), skill.SkillType.ToString().Replace('_', ' '), skill.DescriptionFormatted, rankUpText);
                SelectedAbility = skill;
                break;
            case AbilityType.Talent:
                var talent = GetObject.PlayerCharacter.TalentHandler.Talents.First(s => s.ID == abilityId);
                var rankUpTalentText = talent.CanUpgrade(GetObject.PlayerCharacter) ? GetRankUpRequirement(talent) : "";
                rankUpTalentText = talent.Learnt ? rankUpTalentText : "Not Learnt Yet.";
                UpdateSelectedQuest(talent.Name + string.Format(" [Rank {0}]", talent.CurrentRank + 1), "", talent.DescriptionFormatted, rankUpTalentText);
                SelectedAbility = talent;
                break;
            case AbilityType.Trait:
                var trait = GetObject.PlayerCharacter.GetTraitByID(abilityId);
                var traitDesc = RPG.Stats.GetTraitDescription(trait.ID);
                UpdateSelectedQuest(RPG.Stats.GetTraitName(trait.ID) + ", Level " + trait.Level, "Exp: " + trait.Exp + "/" + trait.ExpToLevel, traitDesc, "");
                SelectedAbility = trait;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private string GetRankUpRequirement(Skill skill)
    {
        if (skill.UpgradeType == SkillUpgradeType.SkillPoints)
        {
            return "Requires " + skill.SkillPointsToLevel + " SP to rank up.";
        }
        else if (skill.UpgradeType == SkillUpgradeType.PlayerLevel)
        {
            return "Requires Level " + skill.LevelRequiredToLevel + " to rank up.";
        }

        else if (skill.UpgradeType == SkillUpgradeType.TraitLevel)
        {
            return "Requires Level " + skill.ReqTraitLevelToLevel + " " + RPG.Stats.GetTraitName(skill.TraitIDToLevel) + " to rank up.";
        }

        return "";
    }
    private string GetUnlockRequirement(Skill skill)
    {
        if (skill.UpgradeType == SkillUpgradeType.SkillPoints)
        {
            return "Requires " + skill.SkillPointsToLevel + " SP to unlock.";
        }
        else if (skill.UpgradeType == SkillUpgradeType.PlayerLevel)
        {
            return "Requires Level " + skill.LevelRequiredToLevel + " to unlock.";
        }

        else if (skill.UpgradeType == SkillUpgradeType.TraitLevel)
        {
            return "Requires Level " + skill.ReqTraitLevelToLevel + " " + RPG.Stats.GetTraitName(skill.TraitIDToLevel) + " to unlock.";
        }

        return "";
    }

    private string GetRankUpRequirement(Talent talent)
    {
        if (talent.UpgradeType == SkillUpgradeType.SkillPoints)
        {
            return "Requires " + talent.SkillPointsToLevel + " SP to rank up.";
        }
        else if (talent.UpgradeType == SkillUpgradeType.PlayerLevel)
        {
            return "Requires Level " + talent.LevelRequiredToLevel + " to rank up.";
        }

        else if (talent.UpgradeType == SkillUpgradeType.TraitLevel)
        {
            return "Requires Level " + talent.ReqTraitLevelToLevel + " " + RPG.Stats.GetTraitName(talent.TraitIDToLevel) + " to rank up.";
        }

        return "";
    }

    private void UpdateSelectedQuest(string abilityName, string abilitySubTitle, string abilityDescription, string abilityReq)
    {
        AbilityTitle.text = abilityName;
        AbilitySubTitle.text = abilitySubTitle;
        AbilityDescription.text = abilityDescription;
        RankUpRequirement.text = abilityReq;
    }

    public void ToggleAbilityLogUI()
    {
        Show = !Show;
        if(Show)
        {
            SwitchToSkills();
            if (!LoadedAbilities)
            {
                LoadAbilities();
            }

            ClearSelected();
        }
        else
        {
            LoadedAbilities = false;
        }
    }

    private void LoadAbilities()
    {
        switch (CurrentType)
        {
            case AbilityType.Skill:
                LoadSkills();
                break;
            case AbilityType.Talent:
                LoadTalents();
                break;
            case AbilityType.Trait:
                LoadTraits();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void LoadSkills()
    {
        var allAbilities = RPG.GetPlayerCharacter.SkillHandler.AvailableSkills;
        AbiliyListContainer.transform.DestroyChildren();

        //Active
        var unlockedAbilities = allAbilities.Where(a => a.Unlocked).ToList();
        if(unlockedAbilities.Any())
        {
            foreach (var item in unlockedAbilities)
            {
                var go1 = Instantiate(AbilityEntryPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                go1.transform.SetParent(AbiliyListContainer.transform, false);
                var itemModel = go1.GetComponent<AbilityEntryModel>();
                itemModel.Init(item.Image.Image, item.Name, item.ID, true);
            }
        }

        //Complete
        var notUnlockedSkills = allAbilities.Where(a => !a.Unlocked).ToList();
        if (ShowNotYetUnlocked && notUnlockedSkills.Any())
        {
            foreach (var item in notUnlockedSkills)
            {
                var go1 = Instantiate(AbilityEntryPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                go1.transform.SetParent(AbiliyListContainer.transform, false);
                var itemModel = go1.GetComponent<AbilityEntryModel>();
                itemModel.Init(item.Image.Image, item.Name, item.ID, false);
            }
        }
    }

    private void LoadTalents()
    {
        var allTalents = RPG.GetPlayerCharacter.TalentHandler.Talents;
        AbiliyListContainer.transform.DestroyChildren();

        //Active
        var unlockedAbilities = allTalents.Where(a => a.Learnt).ToList();
        if(unlockedAbilities.Any())
        {
            foreach (var item in unlockedAbilities)
            {
                var go1 = Instantiate(AbilityEntryPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                go1.transform.SetParent(AbiliyListContainer.transform, false);
                var itemModel = go1.GetComponent<AbilityEntryModel>();
                itemModel.Init(item.Image.Image, item.Name, item.ID, false);
            }
        }

        //Complete
        var notUnlockedTalents = allTalents.Where(a => !a.Learnt).ToList();
        if (ShowNotYetUnlocked && notUnlockedTalents.Any())
        {
            foreach (var item in notUnlockedTalents)
            {
                var go1 = Instantiate(AbilityEntryPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                go1.transform.SetParent(AbiliyListContainer.transform, false);
                var itemModel = go1.GetComponent<AbilityEntryModel>();
                itemModel.Init(item.Image.Image, item.Name, item.ID, false);
            }
        }
    }

    private void LoadTraits()
    {
        var allTraits = RPG.GetPlayerCharacter.Traits;
        AbiliyListContainer.transform.DestroyChildren();

        //Active
        if (allTraits.Any())
        {
            foreach (var item in allTraits)
            {
                var go1 = Instantiate(AbilityEntryPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                go1.transform.SetParent(AbiliyListContainer.transform, false);
                var itemModel = go1.GetComponent<AbilityEntryModel>();
                itemModel.Init(RPG.Stats.GetTraitImage(item.ID), RPG.Stats.GetTraitName(item.ID), item.ID, false);
            }
        }
    }

    public void CloseAbilityLog()
    {
        Show = false;
        LoadedAbilities = false;
    }
}

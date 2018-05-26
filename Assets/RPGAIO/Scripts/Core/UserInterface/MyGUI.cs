//using System;
//using System.Linq;
//using Assets.Scripts.Beta;
//using LogicSpawn.RPGMaker.API;
//using LogicSpawn.RPGMaker.Generic;
//using UnityEngine;
//using Random = UnityEngine.Random;
//
//namespace LogicSpawn.RPGMaker.Core
//{
//    public class MyGUI : MonoBehaviour
//    {
//        public GUISkin mySkin;
//        private PlayerMono _playerMono;
//        private PlayerCharacter Player
//        {
//            get { return _playerMono.Player; }
//        }
//        private Rm_ClassDefinition ClassDef
//        {
//            get { return Rm_RPGHandler.Instance.Player.CharacterDefinitions.First(c => c.ID == Player.PlayerCharacterID); }
//        }
//
//        public static bool Enable = true;
//        public bool EnableOld = false;
//        public bool EnableCharWindow = false;
//        private Vector2 inventoryScrollPos = new Vector2(0, 0);
//
//
//        private Vector2 scrollPosition = new Vector2(0, 0);
//        bool showEscMenu;
//
//        public bool EnableCraftWindow;
//        private Vector2 craftingScrollPos = new Vector2(0, 0);
//
//        public bool EnableVendorWindow;
//        private Vector2 vendorScrollPos = new Vector2(0, 0);
//
//        public bool EnableQuestWindow;
//        public bool EnableSkillWindow;
//        public bool EnableStashWindow;
//        public bool EnableSkillBar = true;
//        private Vector2 cheatScrollPos = new Vector2(0, 0);
//        private Vector2 statScrollPos = new Vector2(0, 0);
//
//        public CharWindowShow charWindowState;
//        public int stashState = 0; //global is 0, character is 1
//        void Start()
//        {
//            _playerMono = GetObject.PlayerMono;
//            charWindowState = CharWindowShow.Equipment;
//            Instance = this;
//        }
//
//        public static MyGUI Instance;
//
//        void Update()
//        {
//            if (Input.GetKeyDown(KeyCode.V))
//            {
//                EnableOld = !EnableOld;
//            }
//            if (RPG.Input.GetKeyDown(RPG.Input.CharacterSheet))
//            {
//                EnableCharWindow = !EnableCharWindow;
//            }
//            //if (Input.GetKeyDown(KeyCode.L))
//            //{
//            //    EnableCraftWindow = !EnableCraftWindow;
//            //    if(EnableCraftWindow) EnableVendorWindow = false;
//            //}
//            if (Input.GetKeyDown(KeyCode.O))
//            {
//                EnableVendorWindow = !EnableVendorWindow;
//                if(EnableVendorWindow) EnableCraftWindow = false;
//            }
//            if (RPG.Input.GetKeyDown(RPG.Input.QuestBook))
//            {
//                EnableQuestWindow = !EnableQuestWindow;
//            }
//            if (RPG.Input.GetKeyDown(RPG.Input.SkillBook))
//            {
//                EnableSkillWindow = !EnableSkillWindow;
//            }
//
//
//        }
//
//        void OnGUI()
//        {
//            GUI.skin = mySkin;
//
//            if (EnableOld)
//                OldGUI();
//
//            return;
//
//            if (showEscMenu)
//            {
//                if (GUI.Button(new Rect(Screen.width / 2, Screen.height / 2, 100, 40), "Save"))
//                {
//                    PlayerSaveLoadManager.Instance.SaveGame();
//                    ToggleEscapeMenu();
//                }
//                if (GUI.Button(new Rect(Screen.width / 2, Screen.height / 2 + 50, 100, 40), "-> MainMenu"))
//                {
//                    RPG.LoadLevel("MainMenu",false);
//                    ToggleEscapeMenu();
//                }
//                if (GUI.Button(new Rect(Screen.width / 2, Screen.height / 2 + 100, 100, 40), "Resize"))
//                {
//                    Screen.SetResolution(1280, 720, false);
//                    ToggleEscapeMenu();
//                }
//                if (GUI.Button(new Rect(Screen.width / 2, Screen.height / 2 + 150, 100, 40), "Fullscreen"))
//                {
//                    Screen.SetResolution(1920, 1080, true);
//                    ToggleEscapeMenu();
//                }
//            }
//
//
//            if (!GameMaster.GameLoaded || !GameMaster.ShowUI) return;
//            //NewGUI();
//
//            if (EnableCharWindow)
//                CharWindow();
//
//
//
//            if (EnableCraftWindow)
//                CraftWindow();
//
//            if (EnableVendorWindow)
//                VendorWindow();
//
//            if (EnableQuestWindow)
//                QuestWindow();
//
//            if (EnableSkillWindow)
//                SkillWindow();
//
//            if (EnableStashWindow)
//            {
//                StashWindow();
//                EnableVendorWindow = false;
//                EnableCraftWindow = false;
//            }
//            
//            if (EnableSkillBar)
//                Skillbar();
//        }
//
//        private void Skillbar()
//        {
//            var area = new Rect(Screen.width/2 - 250, Screen.height - 230, 560, 80);
//            var skillBarArea = new Rect(Screen.width/2 - 240, Screen.height - 225, 550, 70);
//            GUI.Box(area, "");
//            GUILayout.BeginArea(skillBarArea);
//
//            
//                GUILayout.BeginVertical();
//                GUILayout.BeginHorizontal(GUILayout.Height(40));
//                for (int i = 0; i < Player.SkillHandler.Slots.Length; i++)
//                {
//                    var slot = Player.SkillHandler.Slots[i];
//                    var content = slot.Image != null ? new GUIContent(slot.Image) : new GUIContent("");
//
//                    if(slot.Usable && (slot.IsItem || !Player.Silenced))
//                    {
//                        if (GUILayout.Button(content, GUILayout.Width(40), GUILayout.Height(40)))
//                        {
//                            if (Input.GetKey(KeyCode.LeftControl))
//                            {
//                                slot.EmptySlot();
//                                return;
//                            }
//                            slot.Use();
//                            return;
//                        }
//                    }
//                    else
//                    {
//                        GUI.enabled = false;
//                        GUILayout.Box(content, GUILayout.Width(40), GUILayout.Height(40));
//                        GUI.enabled = true;
//                    }
//                   
//                }
//                GUILayout.EndHorizontal();
//
//                GUILayout.BeginHorizontal();
//                var skillLabels = new[] {"1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "-", "="};
//                if(!ControllerChecker.UsingKeyboardMouse)
//                {
//                    skillLabels = new[] { "RB", "Y", "B", "", "", "", "", "", "", "", "", "" };
//                }
//
//                for (int i = 0; i < Player.SkillHandler.Slots.Length; i++)
//                {
//                    GUILayout.Space(10);
//                    GUILayout.Box(" " + skillLabels[i] + " ", GUILayout.Width(20)); 
//                    GUILayout.Space(10);
//                }
//                GUILayout.EndHorizontal();
//                GUILayout.EndVertical();
//            
//
//            GUILayout.EndArea();
//        }
//
//        private void QuestWindow()
//        {
//            var playerSave = GetObject.PlayerSave;
//            GUI.Box(new Rect(Screen.width- 425 , 5, 320, 420), "");
//            GUILayout.BeginArea(new Rect(Screen.width - 405, 15, 300, 400));
//            var playerDifficulty = GetObject.PlayerSave.Difficulty;
//            var difficultyDef = Rm_RPGHandler.Instance.Player.Difficulties.FirstOrDefault(d => d.ID == playerDifficulty);
//            GUILayout.Box("Difficulty:" + difficultyDef.Name);
//
//
//            GUILayout.Box("Achievements");
//
//            foreach(var achievement in playerSave.AchivementsLog.Achievements)
//            {
//                GUILayout.BeginHorizontal();
//                GUI.enabled = achievement.IsAchieved;
//                GUILayout.Box(achievement.ImageContainer.Image, GUILayout.Width(25), GUILayout.Height(25));
//                var progress = achievement.HasProgress
//                                   ? "\n" + achievement.Progress.CurrentValue + "/" + achievement.Progress.TargetValue
//                                   : "";
//                GUILayout.TextField(achievement.Name + ":" + achievement.Description + progress);
//                GUI.enabled = true;
//                GUILayout.EndHorizontal();
//            }
//
//            GUILayout.Box("Quest Log");
//
//            craftingScrollPos = GUILayout.BeginScrollView(craftingScrollPos);
//            GUILayout.Box("In progress");
//            foreach (var quest in playerSave.QuestLog.ActiveObjectives)
//            {
//                GUILayout.Label(quest.Name + (quest.Failed ? " : FAILED" : ""));
//                if(quest.Failed)
//                {
//                    if (GUILayout.Button("Abandon Quest"))
//                    {
//                        QuestHandler.Instance.AbandonQuest(quest);
//                    }
//                    continue;
//                }
//                else
//                {
//                    foreach (var condition in quest.ActiveConditions)
//                    {
//                        if (condition.ConditionType == ConditionType.Kill)
//                        {
//                            var kCond = condition as KillCondition;
//                            GUILayout.Label("- Kill: " + kCond.NumberKilled + "/" + kCond.NumberToKill + " Done:" + condition.IsDone);
//                        }
//                        else if (condition.ConditionType == ConditionType.Item)
//                        {
//                            var kCond = condition as ItemCondition;
//                            GUILayout.Label("- Item: " + kCond.NumberObtained + "/" + kCond.NumberToObtain + " Done:" + condition.IsDone);
//                        }
//                        else
//                        {
//                            GUILayout.Label(" - " + condition.ConditionType + ": " + condition.IsDone);
//                        }
//                    }
//
//                    if (quest.HasTimeLimit)
//                    {
//                        GUILayout.Label("Remaining: " + quest.CurrentTimeLimit.ToString("N0"));
//                    }
//
//                    if (quest.HasBonusCondition && quest.BonusHasTimeLimit && !quest.BonusCondition.IsDone)
//                    {
//                        if (quest.BonusCurrentTimeLimit > 0)
//                        {
//                            GUILayout.Label("Remaining: " + quest.BonusCurrentTimeLimit.ToString("N0"));
//
//                        }
//                        else
//                        {
//                            GUILayout.Label("Bonus Failed!");
//                        }
//                    }
//
//                    if (quest.CanAbandon && GUILayout.Button("Abandon Quest"))
//                    {
//                        QuestHandler.Instance.AbandonQuest(quest);
//                    }
//                }
//                
//
//                GUILayout.Space(5);
//            }
//
//            GUILayout.Box("Completed");
//            foreach (var quest in playerSave.QuestLog.CompletedObjectives)
//            {
//                GUILayout.Label(quest.Name);
//            }
//
//            GUILayout.Box("Not Started");
//
//            foreach (var quest in playerSave.QuestLog.NotStartedObjectives)
//            {
//                GUILayout.Label(quest.Name);
//            }
//            GUILayout.EndScrollView();
//            GUILayout.EndArea();
//        }
//
//        private void SkillWindow()
//        {
//            var skills = GetObject.PlayerCharacter.SkillHandler.AvailableSkills;
//            GUI.Box(new Rect(Screen.width- 425 , 5, 320, 420), "");
//            GUILayout.BeginArea(new Rect(Screen.width - 405, 15, 300, 400));
//            GUILayout.Box("Skills");
//
//            craftingScrollPos = GUILayout.BeginScrollView(craftingScrollPos);
//            GUILayout.Box("Unlocked and Learnt");
//            foreach (var skill in skills.Where(s => s.Unlocked && s.Learnt))
//            {
//                GUILayout.Box(skill.Name);
//            }
//            GUILayout.Space(10);
//            GUILayout.Box("Unlocked");
//            foreach (var skill in skills.Where(s => s.Unlocked && !s.Learnt))
//            {
//                GUILayout.Box(skill.Name);
//            }
//            GUILayout.Space(10);
//            GUILayout.Box("Locked");
//            foreach (var skill in skills.Where(s => !s.Unlocked))
//            {
//                GUILayout.Box(skill.Name);
//            }
//            GUILayout.EndScrollView();
//            GUILayout.EndArea();
//        }
//
//        private void VendorWindow()
//        {
//            var vendorShopIDToUse = Rm_RPGHandler.Instance.Repositories.Vendor.AllVendors[0].ID;
//            GUI.Box(new Rect(405, 5, 320, 420), "");
//            GUILayout.BeginArea(new Rect(415, 15, 300, 400));
//
//            var vendorShopToUse = NPCVendor.Instance.VendorShopitems(vendorShopIDToUse);
//
//            GUILayout.Box("Vendor Shop : " + Rm_RPGHandler.Instance.Repositories.Vendor.AllVendors[0].Name);
//
//            vendorScrollPos = GUILayout.BeginScrollView(vendorScrollPos);
//
//            foreach (var vendorItem in vendorShopToUse)
//            {
//                var item = Rm_RPGHandler.Instance.Repositories.Items.AllItems.First(i => i.ID == vendorItem.ItemID);
//                if (GUILayout.Button(item.Image, GUILayout.Width(40), GUILayout.Height(40)))
//                {
//                    NPCVendor.Instance.SellItemToPlayer(vendorShopIDToUse, GetObject.PlayerCharacter, vendorItem.ItemID);
//                    return;
//                }
//
//                var stockRemaining = vendorItem.InfiniteStock ? "[Inf] " : vendorItem.QuantityRemaining + "x ";
//                var buffItemStr = "";
//                if (item as BuffItem != null) buffItemStr += " (Lv." + (item as BuffItem).RequiredLevel + ") ";
//                var itemInfo = buffItemStr + item.Name;
//                GUILayout.Label(stockRemaining + itemInfo);
//            }
//            GUILayout.Box("Buyback:");
//
//            foreach(var buyBack in NPCVendor.Instance.BuyBackItems)
//            {
//                var item = buyBack;
//                if (GUILayout.Button(item.Image, GUILayout.Width(40), GUILayout.Height(40)))
//                {
//                    var success = Player.Inventory.AddItem(item);
//                    if(success)
//                    {
//                        Player.Inventory.RemoveGold(item.SellValue);
//                        NPCVendor.Instance.BuyBackItems.Remove(item);
//                    }
//                    return;
//                }
//
//                var stackString = "";
//                var stackable = item as IStackable;
//                if (stackable != null) stackString += " ( x " + stackable.CurrentStacks + ")";
//
//                var buffItemStr = "";
//                if (item as BuffItem != null) buffItemStr += " (Lv." + (item as BuffItem).RequiredLevel + ") ";
//                var itemInfo = buffItemStr + item.Name + stackString + "\n";
//                GUILayout.Label(itemInfo);
//            }
//
//            GUILayout.EndScrollView();
//            GUILayout.EndArea();
//        }
//   
//        private void CraftWindow()
//        {
//            GUI.Box(new Rect(405, 5, 320, 420), "");
//            GUILayout.BeginArea(new Rect(415, 15, 300, 400));
//            GUILayout.Box("Craftable Items");
//
//            craftingScrollPos = GUILayout.BeginScrollView(craftingScrollPos);
//
//            foreach(var craftItem in Rm_RPGHandler.Instance.Repositories.CraftableItems.AllItems)
//            {
//                GUI.enabled = CraftHandler.Instance.CanCraft(craftItem);
//                if (GUILayout.Button(craftItem.Image, GUILayout.Width(40), GUILayout.Height(40)))
//                {
//                    CraftHandler.Instance.CraftItem(craftItem);
//                }
//                GUILayout.Label(craftItem.Name);
//                GUI.enabled = true;
//            }
//            GUILayout.EndScrollView();
//            GUILayout.EndArea();
//        }
//
//
//        private void StashWindow()
//        {
//            GUI.Box(new Rect(405, 405, 620, 420), "");
//            GUILayout.BeginArea(new Rect(415, 415, 600, 400));
//
//            GUILayout.BeginHorizontal();
//            if (GUILayout.Button("GlobalStash"))
//            {
//                stashState = 0;
//            }
//            if (GUILayout.Button("CharacterStash"))
//            {
//                stashState = 1;
//            }
//            GUILayout.EndHorizontal();
//
//            GUILayout.BeginVertical();
//            var StashInventory = stashState == 0 ? GameMaster.Instance.UserSave.Stash : GetObject.PlayerSave.Stash;
//            var inventoryString = string.Format("Stash [{0}] - Weight [{1}]", StashInventory.AllItems.Count, StashInventory.CurrentWeight + " / " + StashInventory.MaxWeight);
//
//            GUILayout.Box(inventoryString);
//            GUILayout.Box("Gold [" + StashInventory.Gold + "]");
//            inventoryScrollPos = GUILayout.BeginScrollView(inventoryScrollPos);
//            foreach (var item in StashInventory.GetAllItems())
//            {
//                GUILayout.BeginHorizontal();
//                if (GUILayout.Button(item.Image, GUILayout.Width(40), GUILayout.Height(40)))
//                {
//                    Debug.Log("Clicked stash item");
//
//                    if(Player.Inventory.AddItem(item))
//                    {
//                        StashInventory.RemoveItem(item);
//                    }
//
//                    return;
//
//                }
//
//                var stackString = "";
//                var stackable = item as IStackable;
//                if (stackable != null) stackString += " ( x " + stackable.CurrentStacks + ")";
//
//                var buffItemStr = "";
//                if (item as BuffItem != null) buffItemStr += " (Lv." + (item as BuffItem).RequiredLevel + ") ";
//                var itemInfo = (EnableVendorWindow ? "[SELL] " : "") + (EnableCraftWindow ? "[DISMANTLE] " : "") + buffItemStr + item.Name + stackString + "\n";
//                GUILayout.Label(itemInfo);
//
//                GUILayout.EndHorizontal();
//            }
//
//            GUILayout.EndVertical();
//            GUILayout.EndScrollView();
//
//            GUILayout.EndArea();
//        }
//
//        private void CharWindow()
//        {
//            GUI.Box(new Rect(5, 5, 420, 420), "");
//            GUILayout.BeginArea(new Rect(15,15,400,400));
//
//
//            GUILayout.BeginHorizontal();
//            if(GUILayout.Button("Equip"))
//            {
//                charWindowState = CharWindowShow.Equipment;
//            }
//            if (GUILayout.Button("Inventory"))
//            {
//                charWindowState = CharWindowShow.Inventory;
//            }
//            if (GUILayout.Button("Stats"))
//            {
//                charWindowState = CharWindowShow.Stats;
//            }
//            if (GUILayout.Button("Talents"))
//            {
//                charWindowState = CharWindowShow.Talents;
//            }
//            GUILayout.EndHorizontal();
//
//            if(charWindowState == CharWindowShow.Equipment)
//            {
//                ShowEquipment();
//            }
//            else if(charWindowState == CharWindowShow.Inventory)
//            {
//                ShowInventory();
//            }
//            else if(charWindowState == CharWindowShow.Talents)
//            {
//                ShowTalents();
//            }
//            else
//            {
//                ShowStats();
//            }
//
//            GUILayout.FlexibleSpace();
//            GUILayout.EndArea();
//        }
//
//        private void ShowTalents()
//        {
//            GUILayout.BeginVertical();
//
//            inventoryScrollPos = GUILayout.BeginScrollView(inventoryScrollPos);
//            foreach (var talent in Player.TalentHandler.Talents)
//            {
//                if(GUILayout.Button(talent.Name + ":" + (talent.IsActive ? "On" : "Off")))
//                {
//                    Player.ToggleTalent(talent);
//                }
//            }
//
//            GUILayout.EndScrollView();
//
//            GUILayout.EndVertical();
//        }
//
//        private void ShowStats()
//        {
//            GUILayout.BeginVertical();
//
//            statScrollPos = GUILayout.BeginScrollView(statScrollPos);
//
//            GUILayout.Label("Level:" + Player.Level);
//            GUILayout.Label("Exp:" + Player.Exp);
//
//            #region A
//
//            GUILayout.Label("-- Player Damage --");
//            GUILayout.Label(Player.DamageDealable.ToString());
//
//            GUILayout.Label("-- Reputations --");
//            foreach (var rep in GetObject.PlayerSave.QuestLog.AllReputations)
//            {
//                GUILayout.Label(RPG.Stats.GetReputationName(rep.ReputationID) + rep.Value);
//            }
//
//            GUILayout.Label("-- Player Attributes --");
//            foreach (var attr in Player.Attributes)
//            {
//                var attrName = RPG.Stats.GetAttributeName(attr.ID);
//                GUILayout.Label(string.Format("{0} : Base: {1} | Skill: {2} | Equip: {3} [V: {4}]",
//                                                attrName, attr.BaseValue, attr.SkillValue, attr.EquipValue,
//                                                attr.TotalValue));
//            }
//
//            GUILayout.Label("-- Player Vitals --");
//            foreach (var stat in Player.Vitals)
//            {
//                var vitalName = RPG.Stats.GetVitalName(stat.ID);
//                GUILayout.Label(string.Format("{0} - {1} / {2} [Base: {4} | Attr: {5} | Skill: {6} | Equip: {7}] ",
//                                            vitalName, stat.CurrentValue, stat.MaxValue, stat.IsHealth, stat.ScaledBaseValue, stat.AttributeValue, stat.SkillValue, stat.EquipValue));
//            }
//
//            GUILayout.Label("-- Player Stats --");
//            foreach (var stat in Player.Stats)
//            {
//                var statName = RPG.Stats.GetStatisticName(stat.ID);
//                GUILayout.Label(string.Format("{0}: Base: {2} | Attr: {3} | Skill: {4} | Equip: {5} [Total {6}] ",
//                                            statName, stat.StatisticType, stat.ScaledBaseValue, stat.AttributeValue, stat.SkillValue, stat.EquipValue, stat.TotalValue));
//            }
//
//            GUILayout.Label("-- Player Traits --");
//            foreach (var trait in Player.Traits)
//            {
//                var traitName = RPG.Stats.GetTraitName(trait.ID);
//                GUILayout.Label(string.Format("{0} : Level: {1}/{4}  | Exp: {2} / {3}",
//                                                traitName, trait.Level, trait.Exp, trait.ExpToLevel, trait.MaxLevel));
//            }
//            GUILayout.Label("-- Custom Variables --");
//            foreach (var log in _playerMono.PlayerSave.GenericStats.CustomVariables)
//            {
//                var strin = log.Name + "> ";
//                switch(log.VariableType)
//                {
//                    case Rmh_CustomVariableType.Float:
//                        strin += log.FloatValue;
//                        break;
//                    case Rmh_CustomVariableType.Int:
//                        strin += log.IntValue;
//                        break;
//                    case Rmh_CustomVariableType.Bool:
//                        strin += log.BoolValue;
//                        break;
//                    case Rmh_CustomVariableType.String:
//                        strin += "\"" + log.StringValue + "\"";
//                        break;
//                    default:
//                        throw new ArgumentOutOfRangeException();
//                }
//                GUILayout.Label(strin);
//            }
//            
//            GUILayout.Label("-- Meta Info --");
//            foreach (var metaImmunity in _playerMono.Player.AllImmunities)
//            {
//                GUILayout.Label("Immune to " + RPG.Stats.GetMetaName(metaImmunity.ID) +
//                    (metaImmunity.HasDuration ? " - Time left" + metaImmunity.Duration : "") );
//            }
//            foreach (var metaSus in _playerMono.Player.AllSusceptibilites)
//            {
//                GUILayout.Label(RPG.Stats.GetMetaName(metaSus.ID) + ": " + (metaSus.AdditionalDamage >= 0 ? "+" : "") + metaSus.AdditionalDamage +
//                    (metaSus.HasDuration ? " - Time left" + metaSus.Duration : ""));
//            }
//
//            GUILayout.Label("-- Aura  Effects --");
//            foreach (var aura in _playerMono.Player.AuraEffects)
//            {
//                GUILayout.Label(RPG.Stats.GetSkillName(aura.SkillId) +
//                (aura.TakeResourceAmountPerSec ? " -" + aura.AuraEffectStats.ResourceRequirement + " " + RPG.Stats.GetVitalName(aura.AuraEffectStats.ResourceRequiredId) + " per sec" : "") +
//                (aura.HasDuration ? " - Time left" + aura.Duration : ""));
//            }
//
//            GUILayout.Label("-- Timed Passive  Effects --");
//            foreach (var timedPassiveEffect in _playerMono.Player.TimedPassiveEffects)
//            {
//                GUILayout.Label(">" +
//                (timedPassiveEffect.HasDuration ? " - Time left" + timedPassiveEffect.Duration : ""));
//            }
//
//            GUILayout.Label("-- Status  Effects --");
//            foreach (var statusEffect in _playerMono.Player.StatusEffects)
//            {
//                var dot = statusEffect.DamageOverTime;
//                GUILayout.Label(RPG.Stats.GetStatusEffectName(statusEffect.ID) + " " +
//                                (statusEffect.CauseStun ? "STUNS " : "") +
//                                (statusEffect.CauseRetreat ? "FEARS " : "") +
//                                (statusEffect.CauseSilence ? "SILENCES " : "") +
//                                (statusEffect.Effect.HasDuration ? " - Time left" + statusEffect.Effect.Duration : "") +
//                                (statusEffect.CausesDOT ? " +dot:" + dot.DoTName + ": " + (dot.DamagePerTick) + " per " + dot.TimeBetweenTick + "s" + (dot.HasDuration ? " - Time left" + dot.Duration : "") : ""));
//            }
//
//            GUILayout.Label("-- Vital Regen Info --");
//            foreach (var vitRegen in _playerMono.Player.VitalRegenBonuses)
//            {
//                GUILayout.Label(RPG.Stats.GetVitalName(vitRegen.VitalID) + ": " + vitRegen.RegenBonus + "per " + Rm_RPGHandler.Instance.ASVT.RegenInterval + "s" +
//                    (vitRegen.HasDuration ? " - Time left" + vitRegen.Duration : "")); 
//            }
//
//            GUILayout.Label("-- Status Reduction Info --");
//            foreach (var statReduc in _playerMono.Player.StatusReductions)
//            {
//                GUILayout.Label(RPG.Stats.GetStatusEffectName(statReduc.StatusEffectID) + ": -" + statReduc.DecreaseAmount + (statReduc.IsPercentageDecrease ? "%" : "sec") + 
//                    (statReduc.HasDuration ? " - Time left" + statReduc.Duration : "")); 
//            }
//
//            GUILayout.Label("-- Restoration Info --");
//            foreach (var vitRegen in _playerMono.Player.Restorations)
//            {
//                GUILayout.Label(RPG.Stats.GetVitalName(vitRegen.VitalToRestoreID) + ": " + (vitRegen.FixedRestore ? vitRegen.AmountToRestore.ToString() : vitRegen.PercentToRestore + "%" )+ " per " + vitRegen.SecBetweenRestore + "s" +
//                    " - Time left" + vitRegen.Duration);
//            }
//
//            GUILayout.Label("-- Proc Effects --");
//            foreach (var procEffect in _playerMono.Player.ProcEffects)
//            {
//                GUILayout.Label(procEffect.ProcEffectType + "(" + procEffect.EffectParameter + " " + procEffect.EffectParameterString + ") Condition:" + procEffect.ProcCondition + " N:" + procEffect.Parameter + "[" + +procEffect.ParameterCounter + "]" +
//                (procEffect.HasDuration ? " - Time left" + procEffect.Duration : ""));
//            }
//            GUILayout.Label("-- DoT Effects --");
//            foreach (var dot in _playerMono.Player.CurrentDoTs)
//            {
//                var newDamage = new Damage(dot.DamagePerTick){SkillMetaID = dot.SkillMetaID};
//
//                if (!string.IsNullOrEmpty(newDamage.SkillMetaID))
//                {
//                    var susceptibility = Player.AllSusceptibilites.Where(s => s.ID == newDamage.SkillMetaID).Sum(s => s.AdditionalDamage);
//                    var multiplier = Mathf.Max(0, 1 + susceptibility);
//                    newDamage.ApplyMultiplier(multiplier);
//                }
//
//                GUILayout.Label(dot.DoTName + ": " + (newDamage) + " per " + dot.TimeBetweenTick + "s" +
//                (dot.HasDuration ? " - Time left" + dot.Duration : ""));
//            }
//            #endregion
//            GUILayout.EndScrollView();
//            GUILayout.EndVertical();
//        }
//
//        private void ShowInventory()
//        {
//            GUILayout.BeginVertical();
//            var inventoryString = string.Format("Inventory [{0}] - Weight [{1}]", Player.Inventory.AllItems.Count, Player.Inventory.CurrentWeight + " / " + Player.Inventory.MaxWeight);
//
//            GUILayout.Box(inventoryString);
//            GUILayout.Box("Gold [" + Player.Inventory.Gold + "]");
//            inventoryScrollPos = GUILayout.BeginScrollView(inventoryScrollPos);
//            foreach (var item in Player.Inventory.GetAllItems())
//            {
//                GUILayout.BeginHorizontal();
//                if (GUILayout.Button(item.Image, GUILayout.Width(40), GUILayout.Height(40)))
//                {
//                    Debug.Log("Clicked item");
//
//                    if (Event.current.button == 0)
//                    {
//                        Player.Inventory.UseItemByRef(item.InventoryRefID);
//                    }
//                    
//                    if (Event.current.button == 1)
//                    {
//                        Debug.Log("Right Clicked item");
//
//                        //note: Important code: Drop item or sell if in vendor
//                        if(EnableStashWindow)
//                        {
//                            if(stashState == 0) //global
//                            {
//                                if(GameMaster.Instance.UserSave.Stash.AddItem(item))
//                                {
//                                    Player.Inventory.RemoveItem(item);
//                                }
//                            }
//                            else //character
//                            {
//                                if(GetObject.PlayerSave.Stash.AddItem(item))
//                                {
//                                    Player.Inventory.RemoveItem(item);
//                                }
//                            }
//                        }
//                        else if (EnableVendorWindow)
//                        {
//                            NPCVendor.Instance.BuyItemFromPlayer(GetObject.PlayerCharacter, item);
//                        }
//                        else if (EnableCraftWindow)
//                        {
//                            CraftHandler.Instance.DismantleItem(item);
//                        }
//                        else
//                        {
//                            LootSpawner.Instance.SpawnItem(GetObject.PlayerMonoGameObject.transform.position, item);
//                            Player.Inventory.RemoveItem(item);
//                        }
//                    }
//
//                    return;
//
//                }
//
//                var stackString = "";
//                var stackable = item as IStackable;
//                if (stackable != null) stackString += " ( x " + stackable.CurrentStacks + ")";
//
//                var buffItemStr = "";
//                if (item as BuffItem != null) buffItemStr += " (Lv." + (item as BuffItem).RequiredLevel + ") ";
//                var itemInfo = (EnableStashWindow ? "[STORE] " : "") + (EnableVendorWindow ? "[SELL] " : "") + (EnableCraftWindow ? "[DISMANTLE] " : "") + buffItemStr + item.Name + stackString + "\n";
//                GUILayout.Label(itemInfo);
//
//                GUILayout.EndHorizontal();
//            }
//
//            GUILayout.EndVertical();
//            GUILayout.EndScrollView();
//        }
//
//
//        private void ShowEquipment()
//        {
//            GUILayout.BeginVertical();
//            GUILayout.Box("Equipped Items");
//            foreach (var slot in Player.Equipment.EquippedItems)
//            {
//                GUILayout.BeginHorizontal();
//
//                if (slot.Item != null)
//                {
//                    if (GUILayout.Button(slot.Item.Image, GUILayout.Width(40), GUILayout.Height(40)))
//                    {
//                        if (Player.Equipment.UnEquipItem(slot, slot.Item))
//                        {
//                            return;
//                        }
//                        else
//                        {
//                            Debug.Log("No space in inventory!");
//                        }
//                    }
//                    GUILayout.Label(slot.Item.Name);
//                }
//                else
//                {
//                    GUILayout.Box("", GUILayout.Width(40), GUILayout.Height(40));
//                    GUILayout.Label("<" + slot.SlotName + ">");
//                }
//                GUILayout.EndHorizontal();
//            }
//
//            GUILayout.EndVertical();
//        }
//
//        void OldGUI()
//        { 
//            var playerInfo = "";
//            var attributesString = "";
//            var vitalsString = "";
//            var StatisticsString = "";
//            var TraitsString = "";
//            
//            var InventoryString = "";
//            var SkillsString = "";
//            var TalentsString = "";
//            var GenericStatsString = "";
//            var GeneralLogString = "";
//            var QuestLogString = "";
//            var AchievementsString = "";
//            
//            playerInfo += "SaveName: " + _playerMono.PlayerSave.SaveName + " | " + _playerMono.PlayerSave.CreationDate +
//                        "\n";
//            playerInfo += string.Format("Player Name: {0} | Level : {1} | Exp: {2} | Class: {3}",
//                                    Player.Name, Player.Level, Player.Exp, RPG.Player.GetClassName(Player.PlayerCharacterID)) + "\n";
//            
//            foreach(var attr in Player.Attributes)
//            {
//                var attrName = RPG.Stats.GetAttributeName(attr.ID);
//                attributesString += string.Format("{0} : Base: {1} | Skill: {2} | Equip: {3} | [Total: {4}]",
//                                                attrName, attr.BaseValue, attr.SkillValue, attr.EquipValue,
//                                                attr.TotalValue) + "\n";
//            }
//            
//            foreach (var stat in Player.Vitals)
//            {
//                var vitalName = RPG.Stats.GetVitalName(stat.ID);
//                vitalsString += string.Format("{0} - {1} / {2}  [IsHealth: {3}] Max =  Base: {4}  + Attribute: {5} + Skill: {6} + Equip: {7}] ",
//                                            vitalName,stat.CurrentValue,stat.MaxValue, stat.IsHealth, stat.BaseValue, stat.AttributeValue, stat.SkillValue, stat.EquipValue) + "\n";
//            }
//            
//            foreach(var stat in Player.Stats)
//            {
//                var statName = RPG.Stats.GetStatisticName(stat.ID);
//                StatisticsString += string.Format("{0} [{1}] : Base: {2}  | Attribute: {3} | Skill: {4} | Equip: {5} | [Total {6}] ",
//                                            statName, stat.StatisticType, stat.ScaledBaseValue, stat.AttributeValue, stat.SkillValue, stat.EquipValue, stat.TotalValue) + "\n";
//            }
//            
//            foreach (var trait in Player.Traits)
//            {
//                var traitName = RPG.Stats.GetTraitName(trait.ID);
//                TraitsString += string.Format("{0} : Level: {1}  | Exp: {2}",
//                                                traitName, trait.Level, trait.Exp) + "\n";
//            }
//            
//            InventoryString += "Items in Inventory : " + Player.Inventory.AllItems.Count + "  Weight: " +
//                            Player.Inventory.CurrentWeight + " / " + Player.Inventory.MaxWeight +
//                            " " + " Gold: " + Player.Inventory.Gold + "\n";
//            
//            InventoryString += ">>Weapons: \n";
//            foreach (var item in Player.Inventory.GetItemsByType(ItemType.Weapon))
//            {
//                var stackString = "";
//                var stackable = item as IStackable;
//                if (stackable != null) stackString += " ( x " + stackable.CurrentStacks + ")";
//            
//                var buffItemStr = "";
//                if (item as BuffItem != null) buffItemStr += " (Lv." + (item as BuffItem).RequiredLevel + ") ";
//                InventoryString += buffItemStr + item.Name + stackString + "\n";
//            
//            }
//            
//            InventoryString += ">>All Items: \n";
//            foreach(var item in Player.Inventory.GetAllItems())
//            {
//                var stackString = "";
//                var stackable = item as IStackable;
//                if (stackable != null) stackString += " ( x " + stackable.CurrentStacks + ")";
//            
//                var buffItemStr = "";
//                if (item as BuffItem != null) buffItemStr += " (Lv." + (item as BuffItem).RequiredLevel + ") ";
//                InventoryString += buffItemStr + item.Name + stackString + "\n";
//                             
//            }
//            
////            foreach(var skill in Player.SkillHandler.AvailableSkills)
////            {
////                SkillsString += skill.Name + " > Learnt: " + skill.Learnt + "\n";
////            }
//            
//            foreach (var talent in Player.TalentHandler.Talents)
//            {
//                TalentsString += talent.Name + " > Learnt: " + talent.Learnt + " Active: " + talent.IsActive + "\n";
//            }
//            
//            GenericStatsString += "Monsters Killed: " + _playerMono.PlayerSave.GenericStats.MonstersKilled + "\n";
//            GenericStatsString += "Items Looted: " + _playerMono.PlayerSave.GenericStats.ItemsLooted + "\n";
//            GenericStatsString += "Gold earned: " + _playerMono.PlayerSave.GenericStats.GoldEarned;
//            
//            GeneralLogString += "General Log: \n";
//            foreach(var log in _playerMono.PlayerSave.GeneralLog.AllEntries)
//            {
//                GeneralLogString += log.EntryType + " :" + log.LogContent + " - " + log.DateOfEntry.ToShortDateString() +
//                                    "\n";
//            }
//            GeneralLogString += "Custom Vars: \n";
//            foreach(var log in _playerMono.PlayerSave.GenericStats.CustomVariables)
//            {
//                GeneralLogString += log.Name + "> " + log.BoolValue + " |" + log.FloatValue + "| " + log.StringValue + " |" +
//                                    log.IntValue +"\n";
//            }
//            
//            foreach(var obj in _playerMono.PlayerSave.QuestLog.AllObjectives)
//            {
//                QuestLogString += obj.Name + " Active: " + obj.IsAccepted + "\n";
//                obj.Conditions.Select(c => c.ConditionType).ToList().ForEach(s => QuestLogString += s + "\n");
//            }
//            
//            foreach(var achievement in _playerMono.PlayerSave.AchivementsLog.Achievements)
//            {
//                AchievementsString += achievement.Name + " : Achieved:" + achievement.IsAchieved + "\n";
//            }
//            
//            var ul = "\n-----------------------------------------\n";
//            var combinedString = playerInfo + ul + GenericStatsString + ul + GeneralLogString + ul + QuestLogString + ul + AchievementsString + ul +
//                                ul + InventoryString + ul + SkillsString + ul + TalentsString + ul +  vitalsString + ul + attributesString + ul + 
//                                StatisticsString + ul +
//                                TraitsString;
//            
////            GUILayout.BeginArea(new Rect(120, 5, 800, 500));
////            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
////                GUILayout.TextArea(combinedString);
////                GUILayout.BeginHorizontal();
////                foreach (var item in Player.Inventory.GetAllItems())
////                {
////                    GUILayout.Box(item.Image, GUILayout.Width(40), GUILayout.Height(40));
////                }
////                GUILayout.EndHorizontal();
////            GUILayout.EndScrollView();
////            GUILayout.EndArea();
//            
//                         
//            
//            GUILayout.BeginArea(new Rect(820, 5, 200, 500));
//            cheatScrollPos = GUILayout.BeginScrollView(cheatScrollPos);
//            GUILayout.BeginVertical();
//            if (GUILayout.Button("Load Skillbar with Skills"))
//            {
//                if (Rm_RPGHandler.Instance.Repositories.Skills.AllSkills.Count <= 0) return;
//
//                for (int i = 0; i < Player.SkillHandler.Slots.Length; i++)
//                {
//                    if (Rm_RPGHandler.Instance.Repositories.Skills.AllSkills.Count -1 >= i)
//                    {
//                        Player.SkillHandler.Slots[i].ChangeSlotTo(Rm_RPGHandler.Instance.Repositories.Skills.AllSkills[i]);    
//                    }
//                }
//            }
//            if (GUILayout.Button("Load Skillbar with Item"))
//            {
//                if (Rm_RPGHandler.Instance.Repositories.Items.AllItems.Count < 0) return;
//                if (Rm_RPGHandler.Instance.Repositories.Items.AllItems.FirstOrDefault(i => (i as IStackable) == null) == null) return;
//
//                var count = Rm_RPGHandler.Instance.Repositories.Items.AllItems.Count;
//                while(Player.Inventory.AllItems.Count < 10)
//                {
//                    var item = Rm_RPGHandler.Instance.Repositories.Items.AllItems[Random.Range(0, count)];
//                    var copyItem = Rm_RPGHandler.Instance.Repositories.Items.Get(item.ID);
//                    Player.Inventory.AddItem(copyItem);
//                }
//
//                for (int i = 0; i < Player.SkillHandler.Slots.Length; i++)
//                {
//                    Player.SkillHandler.Slots[i].ChangeSlotTo(Player.Inventory.AllItems[i]);
//                }
//            }
//            if (GUILayout.Button("Add Timed Passive"))
//            {
//                Player.AddTimedPassiveEffect(new TimedPassiveEffect() {Duration = 20f, HasDuration = true});
//            }
//            if (GUILayout.Button("Add Vital Regen Bonus"))
//            {
//                Player.AddVitalRegenBonus(new VitalRegenBonus() {Duration = 5f, HasDuration = true,RegenBonus = 0.2F, VitalID = RPG.Stats.GetVitalId("Health")});
//            }
//            if (GUILayout.Button("Add Proc Effect"))
//            {
//                Player.AddProcEffect(new Rm_ProcEffect() {Duration = 60f, HasDuration = true, ProcCondition = Rm_ProcCondition.Every_N_Hits, Parameter = 3, ProcEffectType = Rm_ProcEffectType.KnockBack, EffectParameter = 2.0F});
//            }
//            if (GUILayout.Button("Add DoT Effect (Arcane)"))
//            {
//                var dmg = new Damage {MinDamage = 50, MaxDamage = 100, CriticalChance = 0.1f};
//                Player.AddDoT(new DamageOverTime() { Duration = 10f, HasDuration = true, Attacker = null, DamagePerTick = dmg, DoTName = "BURNING", TimeBetweenTick = 0.5f, SkillMetaID = RPG.Stats.GetMetaId("Arcane") });
//            }
//            if (GUILayout.Button("Add Status Effect 0"))
//            {
//
//                var statusEffect = Rm_RPGHandler.Instance.Repositories.StatusEffects.AllStatusEffects[0];
//                if (statusEffect != null)
//                {
//                    Player.AddStatusEffect(GeneralMethods.CopyObject((statusEffect)));
//                }
//                else
//                {
//                    Debug.LogError("Create a status effect skill to use this.");
//                }
//            }
//            if (GUILayout.Button("Add Status Effect 1 "))
//            {
//
//                var restoSkill = Rm_RPGHandler.Instance.Repositories.StatusEffects.AllStatusEffects[1];
//                if (restoSkill != null)
//                {
//                    Player.AddStatusEffect(GeneralMethods.CopyObject(((StatusEffect)restoSkill)));
//                }
//                else
//                {
//                    Debug.LogError("Create a status effect skill to use this.");
//                }
//            }
//            if (GUILayout.Button("Add Status Effect 2 "))
//            {
//
//                var restoSkill = Rm_RPGHandler.Instance.Repositories.StatusEffects.AllStatusEffects[2];
//                if (restoSkill != null)
//                {
//                    Player.AddStatusEffect(GeneralMethods.CopyObject(((StatusEffect)restoSkill)));
//                }
//                else
//                {
//                    Debug.LogError("Create a status effect skill to use this.");
//                }
//            }
//            if (GUILayout.Button("Add Status Effect 3 "))
//            {
//
//                var restoSkill = Rm_RPGHandler.Instance.Repositories.StatusEffects.AllStatusEffects[3];
//                if (restoSkill != null)
//                {
//                    Player.AddStatusEffect(GeneralMethods.CopyObject(((StatusEffect)restoSkill)));
//                }
//                else
//                {
//                    Debug.LogError("Create a status effect skill to use this.");
//                }
//            }
//            if (GUILayout.Button("Heal For 1000 HP over time"))
//            {
//                var restoSkill = Rm_RPGHandler.Instance.Repositories.Skills.AllSkills.FirstOrDefault(s => s.SkillType == SkillType.Restoration);
//                if(restoSkill != null)
//                {
//                    Player.AddRestoration(GeneralMethods.CopySkill(((RestorationSkill)restoSkill).Restoration));
//                }
//                else
//                {
//                    Debug.LogError("Create a restoration skill to use this.");
//                }
//            }
//            if (GUILayout.Button("Apply Aura"))
//            {
//                var restoSkill = Rm_RPGHandler.Instance.Repositories.Skills.AllSkills.FirstOrDefault(s => s.SkillType == SkillType.Aura);
//                if(restoSkill != null)
//                {
//                    Player.ToggleAura(restoSkill as AuraSkill);
//                }
//                else
//                {
//                    Debug.LogError("Create a Aura skill to use this.");
//                }
//            }
//            if (GUILayout.Button("+100 Exp"))
//            {
//                var leveled = Player.AddExp(100);
//                Player.FullUpdateStats();
//            }
//            if (GUILayout.Button("+10000 SP"))
//            {
//                Player.AddSkillPoints(10000);
//                Player.FullUpdateStats();
//            }
//            if (GUILayout.Button("+1000 Gold"))
//            {
//                Player.Inventory.AddGold(1000);
//            }
//            if (GUILayout.Button("-500 Gold"))
//            {
//                Player.Inventory.RemoveGold(500);
//            }
//            if (GUILayout.Button("set difficulty"))
//            {
//                var difficultyDef = Rm_RPGHandler.Instance.Player.Difficulties[2];
//                GetObject.PlayerSave.Difficulty = difficultyDef.ID;
//            }
//            if (GUILayout.Button("Give Item"))
//            {
//                var count = Rm_RPGHandler.Instance.Repositories.Items.AllItems.Count;
//                if(count > 0)
//                {
//                    var item = Rm_RPGHandler.Instance.Repositories.Items.AllItems[Random.Range(0,count)];
//                    var copyItem = Rm_RPGHandler.Instance.Repositories.Items.Get(item.ID);
//                    Player.Inventory.AddItem(copyItem);
//                }
//            }
//            if (GUILayout.Button("MonsterKilled++"))
//            {
//                _playerMono.PlayerSave.GenericStats.MonstersKilled++;
//            }
//            if (GUILayout.Button("ItemsLooted++"))
//            {
//                _playerMono.PlayerSave.GenericStats.ItemsLooted++;
//            }
//            if (GUILayout.Button("Gold += 1000"))
//            {
//                Player.Inventory.AddGold(1000);
//                _playerMono.PlayerSave.GenericStats.GoldEarned += 1000;
//            }
//            if (GUILayout.Button("Move +=1"))
//            {
//                Player.GetStat(Rm_RPGHandler.Instance.ASVT.StatForMovementID).BaseValue += 0.1f;
//            }
//            if (GUILayout.Button("CastSpeed +=0.1"))
//            {
//                Player.GetStatByID("Cast Speed").BaseValue += 0.1f;
//            }
//            if (GUILayout.Button("CastSpeed +=1"))
//            {
//                Player.GetStatByID("Cast Speed").BaseValue += 1f;
//            }
//            if (GUILayout.Button("STR +=50"))
//            {
//                Player.GetAttribute("Strength").BaseValue += 50;
//            }
//            if (GUILayout.Button("Health +=100"))
//            {
//                Player.GetVital("Health").BaseValue += 100;
//            }
//            if (GUILayout.Button("Refill Health"))
//            {
//                Player.GetVital("Health").CurrentValue = Player.GetVital("Health").MaxValue;
//            }
//            if (GUILayout.Button("Mana +=100"))
//            {
//                Player.GetVital("Mana").BaseValue += 100;
//            }
//            if (GUILayout.Button("Mana +=1000"))
//            {
//                Player.GetVital("Mana").BaseValue += 1000;
//            }
//            if (GUILayout.Button("Refill Mana"))
//            {
//                Player.GetVital("Mana").CurrentValue = Player.GetVital("Mana").MaxValue;
//            }
//            if (GUILayout.Button("Spawn Loot"))
//            {
//                var allItems = Rm_RPGHandler.Instance.Repositories.Items.AllItems;
//                LootSpawner.Instance.SpawnItem(GetObject.PlayerMono.transform.position, allItems[Random.Range(0, allItems.Count)]);
//            }
//            GUILayout.EndVertical();
//            GUILayout.EndScrollView();
//            GUILayout.EndArea();
//        }
//
//        void NewGUI()
//        {
//            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
//            GUILayout.BeginVertical();
//
//            GUILayout.BeginVertical();
//            GUILayout.BeginHorizontal();
//
//            GUILayout.FlexibleSpace();
//
//            GUILayout.BeginVertical();
//            var worldArea = Rm_RPGHandler.Instance.Customise.WorldMapLocations.FirstOrDefault(w => w.ID == GetObject.PlayerSave.WorldMap.CurrentWorldAreaID);
//            string locationName = null;
//            if(worldArea != null)
//            {
//                var location = worldArea.Locations.FirstOrDefault(l => l.ID == GetObject.PlayerSave.WorldMap.CurrentLocationID);    
//                if(location != null)
//                {
//                    locationName = location.Name;
//                }
//            }
//            
//            GUILayout.Box(worldArea != null ? worldArea.Name + " - " + locationName : "");
//            GUILayout.FlexibleSpace();
//            GUILayout.EndVertical();
//            GUILayout.EndHorizontal();
//            GUILayout.EndVertical();
//
//            GUILayout.BeginVertical();
//
//            GUILayout.BeginHorizontal();
//            GUILayout.EndHorizontal();
//
//            GUILayout.BeginHorizontal();
//            GUILayout.EndHorizontal();
//
//            GUILayout.EndVertical();
//
//            GUILayout.FlexibleSpace();
//            GUILayout.BeginVertical();
//
//            GUILayout.BeginHorizontal();
//            GUILayout.BeginVertical();
//            GUILayout.FlexibleSpace();
//            GUILayout.Box(ClassDef.Image, GUILayout.MaxHeight(80), GUILayout.MaxWidth(80));
//            GUILayout.Label(ClassDef.Name);
//            GUILayout.EndVertical();
//
//            GUILayout.FlexibleSpace();
//
//            GUILayout.BeginVertical();
//            GUILayout.FlexibleSpace();
//            GUILayout.Label("HP: " + Player.VitalHandler.Health.CurrentValue + " / " + Player.VitalHandler.Health.MaxValue, "Vitals");
//            GUILayout.Label("MP: " + Player.GetVital("Mana").CurrentValue + " / " + Player.GetVital("Mana").MaxValue,"Vitals");
//            GUILayout.EndVertical();
//
//            GUILayout.FlexibleSpace();
//
//            GUILayout.BeginVertical();
//            GUILayout.FlexibleSpace();
//            var seperator = "\n_____________\n";
//
//            var attributesString = "Attributes" + seperator;
//            foreach (var attr in Player.Attributes)
//            {
//                var attrName = RPG.Stats.GetAttributeName(attr.ID);
//                attributesString += string.Format("{0} : {1} ", attrName, attr.TotalValue) + "\n";
//            }
//
//            var statsString = "Statistics" + seperator;
//            foreach (var stat in Player.Stats)
//            {
//                var statName = RPG.Stats.GetStatisticName(stat.ID);
//                attributesString += string.Format("{0} : {1} ", statName, stat.TotalValue) + "\n";
//            }
//
//            attributesString += "Alive: " + Player.Alive;
//
//            GUILayout.Label(attributesString + "\n"  + statsString);
//            GUILayout.Label(Player.Name + " Level " + Player.Level);    
//            GUILayout.EndVertical();
//            GUILayout.EndHorizontal();
//
//            GUILayout.EndVertical();
//
//
//            GUILayout.EndVertical();
//            GUILayout.EndArea();
//        }
//
//        public void ToggleEscapeMenu()
//        {
//            showEscMenu = !showEscMenu;
//            GameMaster.GamePaused = showEscMenu;
//        }
//    }
//
//    public enum CharWindowShow
//    {
//        Equipment,
//        Inventory,
//        Stats,
//        Talents
//    }
//}
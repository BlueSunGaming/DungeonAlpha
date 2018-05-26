using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Editor.New;
using LogicSpawn.RPGMaker.Objectives;
using UnityEditor;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Editor
{
    public static class Rme_Main_Questing
    {
        public static Rmh_Questing Questing
        {
            get { return Rm_RPGHandler.Instance.Questing; }
        }

         public static void Options(Rect fullArea, Rect leftArea, Rect mainArea)
         {
             GUI.Box(fullArea, "", "backgroundBox");

             GUILayout.BeginArea(fullArea);
             RPGMakerGUI.Title("Objectives - Options");

             RPGMakerGUI.Toggle("Show Quest Markers?", ref Questing.ShowQuestMarkers);
             Questing.QuestStarted.Audio = RPGMakerGUI.AudioClipSelector("Quest Started Sound:", Questing.QuestStarted.Audio, ref Questing.QuestStarted.AudioPath);
             Questing.QuestComplete.Audio = RPGMakerGUI.AudioClipSelector("Quest Completed Sound:", Questing.QuestComplete.Audio, ref Questing.QuestComplete.AudioPath);

             GUILayout.EndArea();
         }

        private static QuestChain selectedQuestChain = null;
        private static Quest selectedQuestChainQuest = null;
        private static List<QuestChain> questChainList
        {
            get { return Rm_RPGHandler.Instance.Repositories.Quests.AllQuestChains; }
        }

        private static Rect leftAreaB;
        private static Rect mainAreaAlt;
        private static bool rectsSet;
        private static bool showCustomVarSetters = true;
        private static bool showReqCompletedQuests = true;
        private static bool showReqAcceptedQuests = true;
        private static bool showSelectedQuestReq = true;
        private static bool showQuestRewards = true;
        private static bool showBonusRewards = true;
        private static bool showQuestMainRewards = true;
        private static bool showQuestBonusRewards = true;
        private static bool showBonusCondition = true;
        private static bool showFinalCondition = true;
        private static bool showCustomVarReqSetters = true;
        private static bool showSelectedQuestMainConditions = true;
        private static bool showSelectedQuestDetails = true;
        private static bool[] booleanArrayForToggles = new bool[999];
        private static int selectedVarSetterBoolResult;
        private static Vector2 questChainScrollPos = Vector2.zero;
         public static void QuestChains(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            
            leftAreaB = new Rect(leftArea.xMax + 5, leftArea.y, leftArea.width, leftArea.height);
            mainAreaAlt = new Rect(leftAreaB.xMax + 5, leftArea.y, mainArea.width - (leftAreaB.width + 5),
                                    leftArea.height);

             GUI.Box(leftArea, "","backgroundBox");
             GUI.Box(leftAreaB, "", "backgroundBox");
             GUI.Box(mainAreaAlt, "", "backgroundBox");

             

             GUILayout.BeginArea(PadRect(leftArea, 0, 0));
             var selected = selectedQuestChain;
             RPGMakerGUI.ListArea(questChainList, ref selectedQuestChain, Rm_ListAreaType.QuestChains, false, false);
             if(selected != selectedQuestChain)
             {
                 booleanArrayForToggles = new bool[999];
             }
             GUILayout.EndArea();

             GUILayout.BeginArea(leftAreaB);
             if(selectedQuestChain != null)
             {
                 var rect = RPGMakerGUI.ListArea(selectedQuestChain.QuestsInChain, ref selectedQuestChainQuest, Rm_ListAreaType.Quests, false, false, Rme_ListButtonsToShow.AllExceptHelp, true, selectedQuestChain.ID);

             }
             GUILayout.EndArea();

             GUILayout.BeginArea(mainAreaAlt);
             questChainScrollPos = GUILayout.BeginScrollView(questChainScrollPos);
             RPGMakerGUI.Title("Quest Chains");
             if (selectedQuestChain != null)
             {
                 selectedQuestChain.Name = RPGMakerGUI.TextField("Name: ", selectedQuestChain.Name);
                 if (selectedQuestChainQuest != null)
                 {
                     var quest = selectedQuestChainQuest;
                     QuestDetails(quest,true);
                 }
                 else
                 {
                    EditorGUILayout.HelpBox("Select a quest in the quest chain to begin editing it.", MessageType.Info);
                 }
             }
             else
             {
                 EditorGUILayout.HelpBox("Add or select a new field to customise quest chains.", MessageType.Info);
             }
             GUILayout.EndScrollView();
             GUILayout.EndArea();
         }

        private static void QuestDetails(Quest quest, bool inQuestChain)
        {
            if (RPGMakerGUI.Foldout(ref showSelectedQuestDetails, "Selected Quest - Main Details"))
            {
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical(GUILayout.MaxWidth(85));
                quest.Image.Image = RPGMakerGUI.ImageSelector("", quest.Image.Image, ref quest.Image.ImagePath);

                GUILayout.EndVertical();
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                quest.Name = RPGMakerGUI.TextField("Name: ", quest.Name);
                quest.Description = RPGMakerGUI.TextArea("Description:",quest.Description);
                quest.ConditionMode = (QuestConditionMode)RPGMakerGUI.EnumPopup("Condition Mode:", quest.ConditionMode);
                if (GUILayout.Button("Open Dialog/Event On Accept", "genericButton", GUILayout.MaxHeight(30)))
                {
                    var trees = Rm_RPGHandler.Instance.Nodes.DialogNodeBank.NodeTrees;
                    var existingTree = trees.FirstOrDefault(t => t.ID == quest.DialogNodeTreeID);
                    if (existingTree == null)
                    {
                        existingTree = NodeWindow.GetNewTree(NodeTreeType.Dialog);
                        existingTree.ID = quest.ID;
                        existingTree.Name = quest.Name;
                        trees.Add(existingTree);
                    }

                    DialogNodeWindow.ShowWindow(quest.ID);
                    quest.DialogNodeTreeID = existingTree.ID;
                }

                if (GUILayout.Button("Open Dialog/Event On Complete", "genericButton", GUILayout.MaxHeight(30)))
                {
                    var trees = Rm_RPGHandler.Instance.Nodes.DialogNodeBank.NodeTrees;
                    var existingTree = trees.FirstOrDefault(t => t.ID == quest.CompletedDialogNodeTreeID);
                    if (existingTree == null)
                    {
                        existingTree = NodeWindow.GetNewTree(NodeTreeType.Dialog);
                        existingTree.ID = "complete_" + quest.ID;
                        existingTree.Name = "Completed " + quest.Name;
                        trees.Add(existingTree);
                    }

                    DialogNodeWindow.ShowWindow(existingTree.ID);
                    quest.CompletedDialogNodeTreeID = existingTree.ID;
                }

                GUILayout.Space(5);
                RPGMakerGUI.Toggle("Player Keeps Quest Items?", ref quest.PlayerKeepsQuestItems);
                RPGMakerGUI.Toggle("Is Repeatable?", ref quest.Repeatable);
                RPGMakerGUI.Toggle("Can Abandon?", ref quest.CanAbandon);
                if (RPGMakerGUI.Toggle("Has Time Limit?", ref quest.HasTimeLimit))
                {
                    quest.TimeLimit = RPGMakerGUI.FloatField("- Time Limit:", quest.TimeLimit);
                }
                if (RPGMakerGUI.Toggle("Run Event On Accept?", ref quest.RunEventOnAccept))
                {
                    RPGMakerGUI.PopupID<NodeChain>("- Event:", ref quest.EventOnAcceptID);
                }
                if (RPGMakerGUI.Toggle("Run Event On Completion?", ref quest.RunEventOnComplete))
                {
                    RPGMakerGUI.PopupID<NodeChain>("- Event:", ref quest.EventOnCompletionId);
                }
                if (RPGMakerGUI.Toggle("Run Event On Cancel?", ref quest.RunEventOnCancel))
                {
                    RPGMakerGUI.PopupID<NodeChain>("- Event:", ref quest.EventOnCancelId);
                }


                GUILayout.Space(5);
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();

                RPGMakerGUI.EndFoldout();

            }

            if (RPGMakerGUI.Foldout(ref showSelectedQuestReq, "Requirements"))
            {
                List<Quest> availableReqQuests = new List<Quest>();
                if (inQuestChain)
                {
                    availableReqQuests = Rm_RPGHandler.Instance.Repositories.Quests.AllQuests.Where(q => selectedQuestChain.QuestsInChain.FirstOrDefault(qu => qu.ID == q.ID) == null).ToList();
                }
                else
                {
                    availableReqQuests = Rm_RPGHandler.Instance.Repositories.Quests.AllQuests;
                }


                RPGMakerGUI.FoldoutList(ref showReqAcceptedQuests, "Required Completed Quests", quest.Requirements.QuestCompletedIDs, availableReqQuests, "+Quest",
                                           "", "Click +Quest to add a requirement for a completed quest.");

                RPGMakerGUI.FoldoutList(ref showCustomVarReqSetters, "Custom Var Requirements", quest.Requirements.CustomRequirements, Rm_RPGHandler.Instance.DefinedVariables.Vars, "+VariableReq",
                       "", "Click +VariableReq to add a varaible requirement", "VariableID", "Name", "ID", "Name", false, "Value");

                RPGMakerGUI.SubTitle("More Requirements");

                if (RPGMakerGUI.Toggle("Require Player Level:", ref quest.Requirements.RequireLevel))
                {
                    quest.Requirements.LevelRequired = RPGMakerGUI.IntField("- Required Level:", quest.Requirements.LevelRequired);
                }

                if (RPGMakerGUI.Toggle("Require Player Class:", ref quest.Requirements.RequireClass))
                {
                    RPGMakerGUI.PopupID<Rm_ClassNameDefinition>("- Class ID:", ref quest.Requirements.RequiredClassID);
                }

                RPGMakerGUI.Toggle("Require Reputation Above Amount :", ref quest.Requirements.ReqRepAboveValue);
                if (quest.Requirements.ReqRepAboveValue) quest.Requirements.ReqRepBelowValue = false;
                RPGMakerGUI.Toggle("Require Reputation Below Amount :", ref quest.Requirements.ReqRepBelowValue);
                if (quest.Requirements.ReqRepBelowValue) quest.Requirements.ReqRepAboveValue = false;

                if (quest.Requirements.ReqRepAboveValue || quest.Requirements.ReqRepBelowValue)
                {
                    RPGMakerGUI.PopupID<ReputationDefinition>("- Reputation Faction:", ref quest.Requirements.ReputationFactionID);
                    var prefix = quest.Requirements.ReqRepAboveValue ? "Above " : "Below ";
                    quest.Requirements.ReputationValue = RPGMakerGUI.IntField("- " + prefix + "Amount:", quest.Requirements.ReputationValue);
                }

                if (RPGMakerGUI.Toggle("Require Trait Level?", ref quest.Requirements.RequireTraitLevel))
                {
                    RPGMakerGUI.PopupID<Rm_TraitDefintion>("- Trait:", ref quest.Requirements.RequiredTraitID);
                    quest.Requirements.TraitLevel = RPGMakerGUI.IntField("- Level:", quest.Requirements.TraitLevel);
                }
                if (RPGMakerGUI.Toggle("Require Learnt Skill?", ref quest.Requirements.RequireLearntSkill))
                {
                    RPGMakerGUI.PopupID<Skill>("- Skill:", ref quest.Requirements.LearntSkillID);
                }

                RPGMakerGUI.EndFoldout();
            }


            var result = RPGMakerGUI.FoldoutToolBar(ref showSelectedQuestMainConditions, "Quest Conditions", "+Condition",false,false);
            if (showSelectedQuestMainConditions)
            {
                if (quest.Conditions.Count == 0)
                {
                    EditorGUILayout.HelpBox("Click +Condition to add a new quest condition.", MessageType.Info);
                }

                for (int index = 0; index < quest.Conditions.Count; index++)
                {
                    GUILayout.BeginVertical("foldoutBox");

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();

                    if (index > 0 && GUILayout.Button("Move Up", "genericButton"))
                    {
                        GUI.FocusControl("");
                        var curCondition = quest.Conditions[index];
                        var prevCondition = quest.Conditions[index - 1];

                        quest.Conditions[index - 1] = curCondition;
                        quest.Conditions[index] = prevCondition;

                        return;
                    }

                    if (index < quest.Conditions.Count - 1 && GUILayout.Button("Move Down", "genericButton"))
                    {
                        GUI.FocusControl("");
                        var curCondition = quest.Conditions[index];
                        var nextCondition = quest.Conditions[index + 1];

                        quest.Conditions[index + 1] = curCondition;
                        quest.Conditions[index] = nextCondition;

                        return;
                    }

                    GUILayout.EndHorizontal();

                    quest.Conditions[index] = ShowConditionInfo(quest.Conditions[index]);

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Delete","genericButton",GUILayout.Height(25), GUILayout.Width(100)))
                    {
                        GUI.FocusControl("");
                        quest.Conditions.RemoveAt(index);
                        index--;
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();
                }


                if (result == 0)
                {
                    quest.Conditions.Add(new KillCondition());
                }

                RPGMakerGUI.EndFoldout();
            }

            if(RPGMakerGUI.Foldout(ref showFinalCondition, "Final Condition"))
            {
                if (RPGMakerGUI.Toggle("Enable Final Condition?", ref quest.HasFinalCondition))
                {
                    quest.FinalCondition = ShowConditionInfo(quest.FinalCondition);
                }
                RPGMakerGUI.EndFoldout();
            }

            if (RPGMakerGUI.Foldout(ref showBonusCondition, "Bonus Condition"))
            {
                if (RPGMakerGUI.Toggle("Enable Bonus Condition?", ref quest.HasBonusCondition))
                {
                    quest.BonusCondition = ShowConditionInfo(quest.BonusCondition);
                    if (RPGMakerGUI.Toggle("- Has Time Limit", ref quest.BonusHasTimeLimit))
                    {
                        quest.BonusTimeLimit = RPGMakerGUI.FloatField("  - Time Limit:", quest.BonusTimeLimit);
                    }
                }

                RPGMakerGUI.EndFoldout();
            }

            

            if (RPGMakerGUI.Foldout(ref showQuestRewards, "Rewards"))
            {
                ShowQuestRewardInfo(ref showQuestMainRewards, "Item Rewards", quest.Rewards);
                RPGMakerGUI.EndFoldout();
            }

            if (quest.HasBonusCondition)
            {
                if (RPGMakerGUI.Foldout(ref showBonusRewards, "Bonus Rewards"))
                {
                    ShowQuestRewardInfo(ref showQuestBonusRewards, "Bonus Condition Reward", quest.BonusRewards);
                    RPGMakerGUI.EndFoldout();
                }
            }
            
            RPGMakerGUI.FoldoutList(ref showCustomVarSetters, "Set Custom Vars on Completion", quest.SetCustomVariablesOnCompletion, Rm_RPGHandler.Instance.DefinedVariables.Vars, "+VariableSetter",
                   "", "Click +VariableSetter to add a varaible setter", "VariableID", "Name", "ID", "Name");
        }

        private static void ShowQuestRewardInfo(ref bool show, string prefix, QuestReward reward)
        {
            RPGMakerGUI.FoldoutList(ref show, prefix, reward.Items, Rm_RPGHandler.Instance.Repositories.Items.AllItems, "+ItemReward",
                                                    "", "Click +ItemReward to add an item as a reward.", "ItemID");
            RPGMakerGUI.FoldoutList(ref show, prefix + " (Craftable Items)", reward.CraftableItems, Rm_RPGHandler.Instance.Repositories.CraftableItems.AllItems, "+CraftItemReward",
                                                    "", "Click +CraftItemReward to add a craftable item as a reward.", "ItemID");
            RPGMakerGUI.FoldoutList(ref show, prefix + " (Quest Items)", reward.QuestItems, Rm_RPGHandler.Instance.Repositories.QuestItems.AllItems, "+QuestItemReward",
                                                    "", "Click +QuestItemReward to add a quest item as a reward.", "ItemID");

            reward.Exp = RPGMakerGUI.IntField("Exp Reward:", reward.Exp);
            reward.Gold = RPGMakerGUI.IntField("Gold Reward:", reward.Gold);

            if(RPGMakerGUI.Toggle("Gives Reputation?",ref reward.GivesReputation))
            {
                RPGMakerGUI.PopupID<ReputationDefinition>("- Reputation",ref reward.Reputation.ReputationID);
                reward.Reputation.Value = RPGMakerGUI.IntField("- Amount", reward.Reputation.Value);
            }
            if(RPGMakerGUI.Toggle("Gives Trait Exp?",ref reward.GivesTraitExp))
            {
                RPGMakerGUI.PopupID<Rm_TraitDefintion>("- Trait", ref reward.TraitID);
                reward.TraitExp = RPGMakerGUI.IntField("- Amount", reward.TraitExp);
            }
            if(RPGMakerGUI.Toggle("Unlocks Skill?",ref reward.UnlocksSkill))
            {
                RPGMakerGUI.PopupID<Skill>("- Skill", ref reward.SkillID);
            }
            if(RPGMakerGUI.Toggle("Applys Status Effect?",ref reward.ApplysStatusEffect))
            {
                RPGMakerGUI.PopupID<StatusEffect>("- Status Effect", ref reward.StatusEffectID);
            }
        }

         private static QuestCondition ShowConditionInfo(QuestCondition condition)
         {

             

             var oldtype = condition.ConditionType;
             condition.ConditionType = (ConditionType)RPGMakerGUI.EnumPopup("Condition Type:", condition.ConditionType);
             if (condition.ConditionType != oldtype)
             {

                 //TODO: if no longer interact node tree than delete that node tree

                 switch (condition.ConditionType)
                 {
                     case ConditionType.Kill:
                         condition = new KillCondition();
                         break;
                     case ConditionType.Item:
                         condition = new ItemCondition();
                         break;
                     case ConditionType.Interact:
                         condition = new InteractCondition();
                         break;
                     case ConditionType.Deliver:
                         condition = new DeliverCondition();
                         break;
                     case ConditionType.Custom:
                         condition = new CustomCondition();
                         break;
                     default:
                         throw new ArgumentOutOfRangeException();
                 }
             }

             var killCondition = condition as KillCondition;
             var itemCondition = condition as ItemCondition;
             var interactCondition = condition as InteractCondition;
             var deliverCondition = condition as DeliverCondition;
             var customCondition = condition as CustomCondition;

             if (killCondition != null)
             {
                 if (Rm_RPGHandler.Instance.Combat.NPCsCanFight && Rm_RPGHandler.Instance.Combat.CanAttackNPcs)
                 {
                     RPGMakerGUI.Toggle("Is NPC?", ref killCondition.IsNPC);
                 }
                 else
                 {
                     killCondition.IsNPC = false;
                 }

                 if (killCondition.IsNPC)
                 {
                     RPGMakerGUI.PopupID<NonPlayerCharacter>("NPC to Kill:", ref killCondition.CombatantID);
                 }
                 else
                 {
                     RPGMakerGUI.PopupID<CombatCharacter>("Enemy to Kill:", ref killCondition.CombatantID);
                 }

                 killCondition.NumberToKill = RPGMakerGUI.IntField("Number To Kill:", killCondition.NumberToKill);
             }

             if (itemCondition != null)
             {
                 itemCondition.ItemType = (ItemConditionType)RPGMakerGUI.EnumPopup("Required Item Type:", itemCondition.ItemType);

                 if (itemCondition.ItemType == ItemConditionType.CraftItem)
                 {
                     RPGMakerGUI.PopupID<Item>("CraftItem To Collect:", ref itemCondition.ItemToCollectID, "ID", "Name", "Craft");
                 }
                 else if (itemCondition.ItemType == ItemConditionType.QuestItem)
                 {
                     RPGMakerGUI.PopupID<Item>("Quest Item To Collect:", ref itemCondition.ItemToCollectID, "ID", "Name", "Quest");

                     if (Rm_RPGHandler.Instance.Combat.NPCsCanFight && Rm_RPGHandler.Instance.Combat.CanAttackNPcs)
                     {
                         RPGMakerGUI.Toggle("NPC Drops Items?", ref itemCondition.NPCDropsItem);
                     }
                     else
                     {
                         itemCondition.NPCDropsItem = false;
                     }

                     if (itemCondition.NPCDropsItem)
                     {
                         RPGMakerGUI.PopupID<NonPlayerCharacter>("NPC that Drops Item:", ref itemCondition.CombatantIDThatDropsItem);
                     }
                     else
                     {
                         RPGMakerGUI.PopupID<CombatCharacter>("Enemy that Drops Item:", ref itemCondition.CombatantIDThatDropsItem);
                     }
                 }
                 else if (itemCondition.ItemType == ItemConditionType.Item)
                 {
                     RPGMakerGUI.PopupID<Item>("Item To Collect:", ref itemCondition.ItemToCollectID);
                 }

                 itemCondition.NumberToObtain = RPGMakerGUI.IntField("Number To Obtain:", itemCondition.NumberToObtain);
             }

             if (interactCondition != null)
             {
                 if (RPGMakerGUI.Toggle("Talk to NPC?", ref interactCondition.IsNpc))
                 {
                     RPGMakerGUI.PopupID<NonPlayerCharacter>("NPC to talk to:", ref interactCondition.InteractableID);
                 }
                 else
                 {
                     RPGMakerGUI.PopupID<Interactable>("Object to interact with:", ref interactCondition.InteractableID);
                 }

                 if (GUILayout.Button("Open Interaction Node Tree", "genericButton", GUILayout.MaxHeight(30)))
                 {
                     var trees = Rm_RPGHandler.Instance.Nodes.DialogNodeBank.NodeTrees;
                     var existingTree = trees.FirstOrDefault(t => t.ID == interactCondition.InteractionNodeTreeID);
                     if (existingTree == null)
                     {
                         existingTree = NodeWindow.GetNewTree(NodeTreeType.Dialog);
                         Debug.Log("ExistingTree null? " + existingTree == null);
                         existingTree.ID = interactCondition.ID;

                         Debug.Log(existingTree.ID +":::"+ existingTree.Name);

                         var curSelectedQuest = Rme_Main.Window.CurrentPageIndex == 1 ? selectedQuestChainQuest : selectedQuest;

                         //todo: need unique name
                         existingTree.Name = curSelectedQuest.Name + "Interact";
                         trees.Add(existingTree);
                     }

                     DialogNodeWindow.ShowWindow(interactCondition.ID);
                     interactCondition.InteractionNodeTreeID = existingTree.ID;
                 }

             }

             if (deliverCondition != null)
             {
                 RPGMakerGUI.PopupID<Item>("Quest Item To Deliver:", ref deliverCondition.ItemToDeliverID, "ID", "Name", "Quest");
                 if (RPGMakerGUI.Toggle("Deliver to NPC?", ref deliverCondition.DeliverToNPC))
                 {
                     RPGMakerGUI.PopupID<NonPlayerCharacter>("NPC to deliver to:", ref deliverCondition.InteractableToDeliverToID);
                 }
                 else
                 {
                     RPGMakerGUI.PopupID<Interactable>("Object to deliver with:", ref deliverCondition.InteractableToDeliverToID);
                 }

                 if (GUILayout.Button("Open Interaction On Deliver", "genericButton", GUILayout.MaxHeight(30)))
                 {
                     var trees = Rm_RPGHandler.Instance.Nodes.DialogNodeBank.NodeTrees;
                     var existingTree = trees.FirstOrDefault(t => t.ID == deliverCondition.InteractionNodeTreeID);
                     if (existingTree == null)
                     {
                         existingTree = NodeWindow.GetNewTree(NodeTreeType.Dialog);
                         existingTree.ID = deliverCondition.ID;
                         //todo: need unique name
                         var curSelectedQuest = Rme_Main.Window.CurrentPageIndex == 1 ? selectedQuestChainQuest : selectedQuest;

                         existingTree.Name = curSelectedQuest.Name + "Interact";
                         trees.Add(existingTree);
                     }

                     DialogNodeWindow.ShowWindow(deliverCondition.ID);
                     deliverCondition.InteractionNodeTreeID = existingTree.ID;
                 }
             }

             if (customCondition != null)
             {
                 var customVar = customCondition.CustomVariableRequirement;
                 RPGMakerGUI.PopupID<Rmh_CustomVariable>("Custom Variable:", ref customVar.VariableID);
                 var foundCvar = Rm_RPGHandler.Instance.DefinedVariables.Vars.FirstOrDefault(v => v.ID == customCondition.CustomVariableRequirement.VariableID);
                 if (foundCvar != null)
                 {
                     switch (foundCvar.VariableType)
                     {
                         case Rmh_CustomVariableType.Float:
                             customVar.FloatValue = RPGMakerGUI.FloatField("Required Value:", customVar.FloatValue);
                             break;
                         case Rmh_CustomVariableType.Int:
                             customVar.IntValue = RPGMakerGUI.IntField("Required Value:", customVar.IntValue);
                             break;
                         case Rmh_CustomVariableType.String:
                             customVar.StringValue = RPGMakerGUI.TextField("Required Value:", customVar.StringValue);
                             break;
                         case Rmh_CustomVariableType.Bool:
                             selectedVarSetterBoolResult = customVar.BoolValue ? 0 : 1;
                             selectedVarSetterBoolResult = EditorGUILayout.Popup("Required Value:",
                                                                                 selectedVarSetterBoolResult,
                                                                                 new[] { "True", "False" });
                             customVar.BoolValue = selectedVarSetterBoolResult == 0;
                             break;
                         default:
                             throw new ArgumentOutOfRangeException();
                     }
                 }
             }

             if (condition.ConditionType != ConditionType.Custom)
             {
                 RPGMakerGUI.Toggle("Use Custom Tracking Text:", ref condition.UseCustomText);
             }
             else
             {
                 condition.UseCustomText = true;
             }

             if (condition.UseCustomText)
             {
                 condition.CustomText = RPGMakerGUI.TextField("Custom Incomplete Text:", condition.CustomText);
                 condition.CustomCompletedText = RPGMakerGUI.TextField("Custom Completed Text:", condition.CustomCompletedText);
             }
             GUILayout.Space(5);

             return condition;
         }

        private static Quest selectedQuest = null;
            private static List<Quest> AllSingleQuestsList
            {
                get { return Rm_RPGHandler.Instance.Repositories.Quests.AllSingleQuests; }
            }

        private static Vector2 questScrollPos = Vector2.zero;
         public static void Quests(Rect fullArea, Rect leftArea, Rect mainArea)
         {
             GUI.Box(leftArea, "","backgroundBox");
             GUI.Box(mainArea, "","backgroundBoxMain");

             GUILayout.BeginArea(PadRect(leftArea, 0, 0));
             RPGMakerGUI.ListArea(AllSingleQuestsList, ref selectedQuest, Rm_ListAreaType.Quests, false, false);
             GUILayout.EndArea();


             GUILayout.BeginArea(mainArea);
             questScrollPos = GUILayout.BeginScrollView(questScrollPos);

             RPGMakerGUI.Title("Quests");
             if (selectedQuest != null)
             {
                 QuestDetails(selectedQuest,false);
             }
             else
             {
                 EditorGUILayout.HelpBox("Add or select a new field to customise quests.", MessageType.Info);
             }
             GUILayout.EndScrollView();
             GUILayout.EndArea();
         }

        public static Rect PadRect(Rect rect, int left, int top)
         {
             return new Rect(rect.x + left, rect.y + top, rect.width - (left * 2), rect.height - (top * 2));
         }
    }
}
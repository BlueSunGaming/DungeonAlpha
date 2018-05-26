
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Editor.New;
using UnityEditor;
using UnityEngine;
using System;

namespace LogicSpawn.RPGMaker.Editor
{
    public static class Rme_Main_NPC
    {

        #region NPCs
        private static bool mainInfoFoldout = true;
        private static CombatCharacter selectedCharInfo
        {
            get { return selectedNPCInfo ?? null; }
        }
        private static NonPlayerCharacter selectedNPCInfo; 
        private static GameObject gameObject = null;
        public static void NPCs(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            var list = Rm_RPGHandler.Instance.Repositories.Interactable.AllNpcs;
            GUI.Box(leftArea, "", "backgroundBox");
            GUI.Box(mainArea, "", "backgroundBoxMain");

            GUILayout.BeginArea(PadRect(leftArea, 0, 0));
            RPGMakerGUI.ListArea(list, ref selectedNPCInfo, Rm_ListAreaType.NPCs, false, true);
            GUILayout.EndArea();

            GUILayout.BeginArea(mainArea);
            RPGMakerGUI.Title("NPCs");
            if (selectedCharInfo != null)
            {
                RPGMakerGUI.BeginScrollView();
                if (RPGMakerGUI.Foldout(ref mainInfoFoldout, "Main Info"))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical(GUILayout.MaxWidth(85));
                    selectedCharInfo.Image = RPGMakerGUI.ImageSelector("", selectedCharInfo.Image,
                                                                        ref selectedCharInfo.ImagePath);

                    GUILayout.EndVertical();
                    GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                    RPGMakerGUI.TextField("ID: ", selectedCharInfo.ID);
                    selectedCharInfo.Name = RPGMakerGUI.TextField("Name: ", selectedCharInfo.Name);
                    GUILayout.BeginHorizontal();
                    gameObject = RPGMakerGUI.PrefabSelector("NPC Prefab:", gameObject, ref selectedCharInfo.CharPrefabPath);
                    gameObject = RPGMakerGUI.PrefabGeneratorButton(Rmh_PrefabType.NPC, gameObject, ref selectedCharInfo.CharPrefabPath, null, selectedCharInfo.ID);
                    GUILayout.EndHorizontal(); 


                    if (GUILayout.Button("Open Interaction Window", "genericButton", GUILayout.MaxHeight(30)))
                    {
                        var trees = Rm_RPGHandler.Instance.Nodes.DialogNodeBank.NodeTrees;
                        var existingTree = trees.FirstOrDefault(t => t.ID == selectedCharInfo.ID);
                        if (existingTree == null)
                        {
                            existingTree = NodeWindow.GetNewTree(NodeTreeType.Dialog);
                            existingTree.ID = selectedCharInfo.ID;
                            existingTree.Name = selectedCharInfo.Name;
                            trees.Add(existingTree);
                        }

                        DialogNodeWindow.ShowWindow(selectedCharInfo.ID);
                        selectedNPCInfo.Interaction.ConversationNodeId = existingTree.ID;
                    }
                    if (Rm_RPGHandler.Instance.Combat.NPCsCanFight)
                    {
                        if(RPGMakerGUI.Toggle("This NPC Can Fight?", ref selectedNPCInfo.CanFight))
                        {
                            selectedNPCInfo.CanBeKilled = RPGMakerGUI.Toggle("Can be Perma-killed? ", selectedNPCInfo.CanBeKilled);
                            Rme_Combatants.CombatantDetails(selectedNPCInfo);
                        }
                    }
                    GUILayout.Space(5);
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();

                }
                if (mainInfoFoldout) RPGMakerGUI.EndFoldout();


                Rme_Combatants.Animations(selectedNPCInfo);




                if (Rm_RPGHandler.Instance.Combat.NPCsCanFight && selectedNPCInfo.CanFight)
                {
                    Rme_Combatants.CombatStats(selectedNPCInfo);
                    Rme_Combatants.Loot(selectedNPCInfo);
                }

                RPGMakerGUI.EndScrollView();
            }
            else
            {
                EditorGUILayout.HelpBox("Add or select a new field to customise Enemy Characters.", MessageType.Info);
            }
            GUILayout.EndArea();

        } 
        #endregion

        #region VendorShops
        private static Rect leftAreaB;
        private static Rect mainAreaAlt;
        private static VendorShop selectedVendorShop = null;
        private static VendorShopItem selectedVendorShopItem = null;
        private static int selecteVendorItem = 0;
        private static bool rectsSet;
        public static void VendorShops(Rect fullArea, Rect leftArea, Rect mainArea)
        {

            
            leftAreaB = new Rect(leftArea.xMax + 5, leftArea.y, leftArea.width, leftArea.height);
            mainAreaAlt = new Rect(leftAreaB.xMax + 5, leftArea.y, mainArea.width - (leftAreaB.width + 5),
                                    leftArea.height);

            GUI.Box(leftArea, "", "backgroundBox");
            GUI.Box(leftAreaB, "", "backgroundBox");
            GUI.Box(mainAreaAlt, "", "backgroundBox");


            var list = Rm_RPGHandler.Instance.Repositories.Vendor.AllVendors;

            GUILayout.BeginArea(PadRect(leftArea, 0, 0));
            RPGMakerGUI.ListArea(list, ref selectedVendorShop, Rm_ListAreaType.VendorShops, false, true);
            GUILayout.EndArea();

            GUILayout.BeginArea(leftAreaB);
            if (selectedVendorShop != null)
            {
                RPGMakerGUI.ListArea(selectedVendorShop.VendorShopItems, ref selectedVendorShopItem, Rm_ListAreaType.VendorShopItem, false, false, Rme_ListButtonsToShow.AllExceptHelp, true);
            }
            GUILayout.EndArea();

            GUILayout.BeginArea(mainAreaAlt);
            RPGMakerGUI.Title("Vendor Shops");
            if (selectedVendorShop != null)
            {
                selectedVendorShop.Name = RPGMakerGUI.TextField("Name: ", selectedVendorShop.Name);
                if (selectedVendorShopItem != null)
                {
                    //todo: replace with PopupID
                    RPGMakerGUI.SubTitle("Selected Vendor Shop Item");
                    var allItems = Rm_RPGHandler.Instance.Repositories.Items.AllItems;
                    if (string.IsNullOrEmpty(selectedVendorShopItem.ItemID))
                    {
                        selecteVendorItem = 0;
                    }
                    else
                    {
                        var stillExists =
                            allItems.FirstOrDefault(a => a.ID == selectedVendorShopItem.ItemID);
                        selecteVendorItem = stillExists != null ? allItems.IndexOf(stillExists) : 0;
                    }
                    selecteVendorItem = EditorGUILayout.Popup("Item:", selecteVendorItem,
                                                                    allItems.Select((q,indexOf) => indexOf + ". " +  q.Name).
                                                                        ToArray());

                    if (allItems.Count > 0)
                    {
                        selectedVendorShopItem.ItemID = allItems[selecteVendorItem].ID;

                        var stackable = allItems[selecteVendorItem] as IStackable;
                        if (stackable != null)
                        {
                            selectedVendorShopItem.InfiniteStock = RPGMakerGUI.Toggle("Infinite Stock?",
                                                                                          selectedVendorShopItem.
                                                                                              InfiniteStock);


                            if (!selectedVendorShopItem.InfiniteStock)
                                selectedVendorShopItem.QuantityRemaining = RPGMakerGUI.IntField("Quantity:",
                                                                                                selectedVendorShopItem.
                                                                                                    QuantityRemaining);
                        }
                    }
                    else
                    {
                        RPGMakerGUI.Label("No Items Found.");
                    }
                        
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Add or select a new field to customise Vendor Shops.", MessageType.Info);
            }
            GUILayout.EndArea();
        } 
        #endregion

        private static ReputationDefinition selectedReputation = null;
        private static bool showRanks = true;
        public static void Reputations(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            var list = Rm_RPGHandler.Instance.Repositories.Quests.AllReputations;
            GUI.Box(leftArea, "","backgroundBox");
            GUI.Box(mainArea, "","backgroundBoxMain");

            GUILayout.BeginArea(PadRect(leftArea, 0, 0));
            RPGMakerGUI.ListArea(list, ref selectedReputation, Rm_ListAreaType.Reputations, false, false);
            GUILayout.EndArea();


            GUILayout.BeginArea(mainArea);
            RPGMakerGUI.Title("Reputations");
            if (selectedReputation != null)
            {

                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical(GUILayout.MaxWidth(85));
                selectedReputation.Image = RPGMakerGUI.ImageSelector("", selectedReputation.Image,
                                                                    ref selectedReputation.ImagePath);

                GUILayout.EndVertical();
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                selectedReputation.Name = RPGMakerGUI.TextField("Name: ", selectedReputation.Name);
                selectedReputation.StartingValue = RPGMakerGUI.IntField("Starting Value: ", selectedReputation.StartingValue);
                selectedReputation.ValueLossForNPCAttack = RPGMakerGUI.IntField("Loss on NPC Attack: ", selectedReputation.ValueLossForNPCAttack);
                selectedReputation.IsTrackable = RPGMakerGUI.Toggle("Trackable?", selectedReputation.IsTrackable);
                selectedReputation.AttackIfBelowReputation = RPGMakerGUI.Toggle("Attack if below Rep?", selectedReputation.AttackIfBelowReputation);
                if(selectedReputation.AttackIfBelowReputation)
                {
                    selectedReputation.BelowReputationValue = RPGMakerGUI.IntField("Attack if Below: ", selectedReputation.BelowReputationValue);    
                }

                GUILayout.Space(40);
                GUILayout.Label("Enemy Factions:");
                if (Rm_RPGHandler.Instance.Repositories.Quests.AllReputations.Count > 1) //As we include the current reputation
                {
                    foreach (var d in Rm_RPGHandler.Instance.Repositories.Quests.AllReputations.Where(r => r.ID != selectedReputation.ID))
                    {
                        var faction = selectedReputation.EnemyFactions.FirstOrDefault(a => a.ID == d.ID);
                        if (faction == null)
                        {
                            var factionToAdd = new FactionStatus()
                            {
                                ID = d.ID,
                                IsTrue = false
                            };

                            selectedReputation.EnemyFactions.Add(factionToAdd);
                        }
                    }

                    for (int index = 0; index < selectedReputation.EnemyFactions.Count; index++)
                    {
                        var v = selectedReputation.EnemyFactions[index];
                        var stillExists =
                            Rm_RPGHandler.Instance.Repositories.Quests.AllReputations.FirstOrDefault(
                                a => a.ID == v.ID);

                        if (stillExists == null)
                        {
                            selectedReputation.EnemyFactions.Remove(v);
                            index--;
                        }
                    }

                    foreach (var v in selectedReputation.EnemyFactions)
                    {
                        var prefix =
                            Rm_RPGHandler.Instance.Repositories.Quests.AllReputations.First(a => a.ID == v.ID).
                                Name;
                        RPGMakerGUI.Toggle(prefix, ref v.IsTrue);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("No other factions.", MessageType.Info);
                }

                #region "AlliedFactions"
                //                GUILayout.Label("Allied Factions:");
//                if (Rm_RPGHandler.Instance.Repositories.Quests.AllReputations.Count > 1) //As we include the current reputation
//                {
//                    foreach (var d in Rm_RPGHandler.Instance.Repositories.Quests.AllReputations.Where(r => r.ID != selectedReputation.ID))
//                    {
//                        var faction = selectedReputation.AlliedFactions.FirstOrDefault(a => a.ID == d.ID);
//                        if (faction == null)
//                        {
//                            var factionToAdd = new FactionStatus()
//                            {
//                                ID = d.ID,
//                                IsTrue = false
//                            };
//
//                            selectedReputation.AlliedFactions.Add(factionToAdd);
//                        }
//                    }
//
//                    for (int index = 0; index < selectedReputation.AlliedFactions.Count; index++)
//                    {
//                        var v = selectedReputation.AlliedFactions[index];
//                        var stillExists =
//                            Rm_RPGHandler.Instance.Repositories.Quests.AllReputations.FirstOrDefault(
//                                a => a.ID == v.ID);
//
//                        if (stillExists == null)
//                        {
//                            selectedReputation.AlliedFactions.Remove(v);
//                            index--;
//                        }
//                    }
//
//                    foreach (var v in selectedReputation.AlliedFactions)
//                    {
//                        var prefix =
//                            Rm_RPGHandler.Instance.Repositories.Quests.AllReputations.First(a => a.ID == v.ID).
//                                Name;
//                        RPGMakerGUI.Toggle(prefix, ref v.IsTrue);
//                    }
//                }
//                else
//                {
//                    EditorGUILayout.HelpBox("No other factions.", MessageType.Info);
                //                }
                #endregion

                var result = RPGMakerGUI.FoldoutToolBar(ref showRanks, "Faction Ranks", "+Rank");
                if(showRanks)
                {
                    for (int index = 0; index < selectedReputation.Ranks.Count; index++)
                    {
                        var rank = selectedReputation.Ranks[index];
                        GUILayout.BeginHorizontal();
                        rank.Name = EditorGUILayout.TextField("", rank.Name);
                        rank.Requirement = EditorGUILayout.IntField("Requirement:", rank.Requirement);
                        GUI.enabled = selectedReputation.Ranks.Count > 1;
                        if (RPGMakerGUI.DeleteButton(25))
                        {
                            selectedReputation.Ranks.RemoveAt(index);
                            index--;
                        }
                        GUI.enabled = true;
                        GUILayout.EndHorizontal();
                    }

                    RPGMakerGUI.EndFoldout();
                }

                if (result == 0)
                {
                    selectedReputation.Ranks.Add(new FactionRank());
                }

                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox("Add or select a new field to customise reputations.", MessageType.Info);
            }
            GUILayout.EndArea();
        }

        public static Rect PadRect(Rect rect, int left, int top)
        {
            return new Rect(rect.x + left, rect.y + top, rect.width - (left * 2), rect.height - (top * 2));
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Editor;
using LogicSpawn.RPGMaker.Editor.New;
using UnityEditor;
using UnityEngine;


namespace LogicSpawn.RPGMaker
{
    public static class Rm_DataUpdater
    {
        private static float AutoSaveTime = 0;

        public static void ScanAndUpdateData()
        {
            if (Rm_RPGHandler.Instance == null) return;
            CheckVersion();
            UpdateCharacterCreation();
            UpdateAnimations();
            UpdatePlayer();
            UpdateInteractables();
            UpdateCombatants();
            UpdateNodes();
            UpdateItemInfo();

            #if (UNITY_IOS || UNITY_ANDROID)
            //Only allow target lock for mobile
            if(Rm_RPGHandler.Instance.Combat.TargetStyle == TargetStyle.ManualTarget)
            {
                Debug.LogWarning("[RPGAIO] Mobile only supports target lock mode");
                Rm_RPGHandler.Instance.Combat.TargetStyle = TargetStyle.TargetLock;
            }
            #endif
        }

        private static void NotificationsAndWarnings()
        {

            var links = new Dictionary<string, string>();
            var info = new List<string>();
            var warnings = new List<string>();

            //Set notifications to 0 here if we want it to always show

            //Links
            var n0 = "It is highly recommended to switch to .NET 2.0 Api Compatibility level to prevent issues with RPGAIO ";
            var nlink0 = @"http://rpgaio.logicspawn.co.uk/forums/showthread.php?tid=494";
            if (PlayerPrefs.GetInt("RPGAIO_Internal_n0", 0) == 0) links.Add(n0, nlink0);

            //Info
            var n1 = "For support, visit the forums by going to Tools > LogicSpawn RPGAIO > Help and Support";
            if (PlayerPrefs.GetInt("RPGAIO_Internal_n1", 0) == 0) info.Add(n1);


            //Warning
            var n2 = "If you add new skills, talents, items, quests etc, don't forget to remake your character through character creation to prevent issues";
            if (PlayerPrefs.GetInt("RPGAIO_Internal_n2", 0) == 0) warnings.Add(n2);



            //Show window if theres notifications
            if(links.Count + info.Count + warnings.Count > 0)
            {
                Rme_NewUpdateWindow.AddLinks(links);
                Rme_NewUpdateWindow.AddInfo(info.ToArray());
                Rme_NewUpdateWindow.AddWarnings(warnings.ToArray());
                Rme_NewUpdateWindow.Init();
            }
            
            //Set as read
            PlayerPrefs.SetInt("RPGAIO_Internal_n0", 1);
            PlayerPrefs.SetInt("RPGAIO_Internal_n1", 1);
            PlayerPrefs.SetInt("RPGAIO_Internal_n2", 1);
        }

        private static void UpdateCharacterCreation()
        {
            var rpgPlayerData = Rm_RPGHandler.Instance.Player;

            if(rpgPlayerData.RaceDefinitions.Count == 0)
            {
                var r = new Rm_RaceDefinition {Name = "Default Race", Description = ""};
                rpgPlayerData.RaceDefinitions.Add(r);

                var characters = rpgPlayerData.CharacterDefinitions.Where(c => string.IsNullOrEmpty(c.ApplicableRaceID)).ToList();
                foreach(var c in characters)
                {
                    c.ApplicableRaceID = r.ID;
                }
            }

            if(rpgPlayerData.SubRaceDefinitions.Count == 0)
            {
                var r = new Rm_SubRaceDefinition {Name = "Default Sub-Race", Description = ""};
                rpgPlayerData.SubRaceDefinitions.Add(r);

                var characters = rpgPlayerData.CharacterDefinitions.Where(c => string.IsNullOrEmpty(c.ApplicableSubRaceID)).ToList();
                foreach(var c in characters)
                {
                    c.ApplicableSubRaceID = r.ID;
                }
            }

            if(rpgPlayerData.GenderDefinitions.Count == 0)
            {
                var r = new StringDefinition() {Name = "Male"};
                rpgPlayerData.GenderDefinitions.Add(r);

                var characters = rpgPlayerData.CharacterDefinitions.Where(c => string.IsNullOrEmpty(c.ApplicableGenderID)).ToList();
                foreach(var c in characters)
                {
                    c.ApplicableGenderID = r.ID;
                }
            }

            if(rpgPlayerData.ClassNameDefinitions.Count == 0)
            {
                var r = new Rm_ClassNameDefinition() {Name = "Default Class", Description = ""};
                rpgPlayerData.ClassNameDefinitions.Add(r);

                var characters = rpgPlayerData.CharacterDefinitions.Where(c => c.ApplicableClassIDs.Count == 0).ToList();
                foreach(var c in characters)
                {
                    c.ApplicableClassIDs.Add(new StringField()
                                                 {
                                                     ID = r.ID
                                                 });
                }
            }

            //Finally, check if any sub-race has no assigned race, and assign the first one
            foreach(var subRace in rpgPlayerData.SubRaceDefinitions)
            {
                if(string.IsNullOrEmpty(subRace.ApplicableRaceID))
                {
                    subRace.ApplicableRaceID = rpgPlayerData.RaceDefinitions.First().ID;
                }
            }
        }

        private static void CheckVersion()
        {
            var currentVersion = Rme_Main.RpgVersion;
            var dataVersion = Rm_RPGHandler.Instance.Version;

            if (GameDataSaveLoadManager.Instance.LoadedOnce && !EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
            {
                if (currentVersion != dataVersion)  
                {
                    //Show Notifications and Warnings
                    NotificationsAndWarnings();

                    EditorUtility.DisplayDialog("Updated RPGAIO", "An updated version of RPGAIO is detected, click continue to update your game data and prefabs.", "Continue");
                    UpdateData();

                    Debug.Log("[RPGAIO] Welcome to RPGAIO version: " + Rme_Main.RpgVersion);
                    Debug.Log("[RPGAIO] Patch Notes: " + Rme_Main.PatchInfo + "\n" + Rme_Main.PatchNotes);
                }
            }
        }

        
        private static void UpdateData()
        {
            //Perform any updates here
            Rme_Tools_Toolbar.UpdatePrefabs();

            //Update Specific: If we're upgrading to version 1.3.0 add the minimap icon
            if (Rme_Main.RpgVersion == "RPG All-In-One v1.3.0")
            {
                Rm_RPGHandler.Instance.GameInfo.MinimapOptions.PlayerIconPath = "RPGMakerAssets/minimapIcon";
            }

            //Update the version number and save
            Rm_RPGHandler.Instance.Version = Rme_Main.RpgVersion;   
            EditorGameDataSaveLoad.SaveGameData();
        }

        public static void UpdatePlayerSaveOnLoad(PlayerSave save)
        {
            UpdateLegacyAnimation(save.Character.LegacyAnimations);
        }

        private static void UpdateAnimations()
        {
            foreach (var classDef in Rm_RPGHandler.Instance.Player.CharacterDefinitions)
            {
                UpdateLegacyAnimation(classDef.LegacyAnimations);
            }
        }

        private static void UpdateLegacyAnimation(LegacyAnimation lAnim)
        {
            lAnim.UnarmedAnim.RPGAnimationSet = RPGAnimationSet.Core;
            lAnim.WalkAnim.RPGAnimationSet = RPGAnimationSet.Core;
            lAnim.WalkBackAnim.RPGAnimationSet = RPGAnimationSet.Core;
            lAnim.RunAnim.RPGAnimationSet = RPGAnimationSet.Core;
            lAnim.JumpAnim.RPGAnimationSet = RPGAnimationSet.Core;
            lAnim.StrafeRightAnim.RPGAnimationSet = RPGAnimationSet.Core;
            lAnim.StrafeLeftAnim.RPGAnimationSet = RPGAnimationSet.Core;
            lAnim.TurnRightAnim.RPGAnimationSet = RPGAnimationSet.Core;
            lAnim.TurnLeftAnim.RPGAnimationSet = RPGAnimationSet.Core;
            lAnim.IdleAnim.RPGAnimationSet = RPGAnimationSet.Core;
            lAnim.CombatIdleAnim.RPGAnimationSet = RPGAnimationSet.Core;
            lAnim.TakeHitAnim.RPGAnimationSet = RPGAnimationSet.Core;
            lAnim.FallAnim.RPGAnimationSet = RPGAnimationSet.Core;
            lAnim.DeathAnim.RPGAnimationSet = RPGAnimationSet.Core;
            lAnim.KnockBackAnim.RPGAnimationSet = RPGAnimationSet.Core;
            lAnim.KnockUpAnim.RPGAnimationSet = RPGAnimationSet.Core;

            foreach (var anim in lAnim.DefaultAttackAnimations)
            {
                anim.RPGAnimationSet = RPGAnimationSet.DefaultAttack;
            }
            foreach (var anim in lAnim.Default2HAttackAnimations)
            {
                anim.RPGAnimationSet = RPGAnimationSet.DefaultAttack;
            }
            foreach (var anim in lAnim.DefaultDWAttackAnimations)
            {
                anim.RPGAnimationSet = RPGAnimationSet.DefaultAttack;
            }

            foreach (var anim in lAnim.WeaponAnimations)
            {
                foreach (var normAnim in anim.Animations)
                {
                    normAnim.RPGAnimationSet = RPGAnimationSet.WeaponTypeAttack;
                }
                foreach (var dwAnim in anim.DualWieldAnimations)
                {
                    dwAnim.RPGAnimationSet = RPGAnimationSet.WeaponTypeAttack;
                }
            }
        }

        private static void UpdatePlayer()
        {
            //remove no longer existing attributes
            foreach(var classDef in Rm_RPGHandler.Instance.Player.CharacterDefinitions)
            {
                //Attributes
                var attrs = classDef.StartingAttributes;

                for (int i = 0; i < attrs.Count; i++)
                {
                    var v = attrs[i];
                    var id = v.AsvtID;
                    var exists = Rm_RPGHandler.Instance.ASVT.AttributesDefinitions.FirstOrDefault(e => e.ID == id);
                    if (exists == null)
                    {
                        attrs.RemoveAt(i);
                        i--;
                    }
                }

                //add non-existing elementals to damage tbl
                foreach (var def in Rm_RPGHandler.Instance.ASVT.AttributesDefinitions)
                {
                    var exists = attrs.FirstOrDefault(a => a.AsvtID == def.ID);
                    if (exists == null)
                    {
                        classDef.StartingAttributes.Add(new Rm_AsvtAmount(){AsvtID = def.ID, Amount = def.DefaultValue});                    
                    }
                }
                
                //Statistics
                var stats = classDef.StartingStats;

                for (int i = 0; i < stats.Count; i++)
                {
                    var v = stats[i];
                    var id = v.AsvtID;
                    var exists = Rm_RPGHandler.Instance.ASVT.StatisticDefinitions.FirstOrDefault(e => e.ID == id);
                    if (exists == null)
                    {
                        stats.RemoveAt(i);
                        i--;
                    }
                }

                //add non-existing stats
                foreach (var def in Rm_RPGHandler.Instance.ASVT.StatisticDefinitions)
                {
                    var exists = stats.FirstOrDefault(a => a.AsvtID == def.ID);
                    if (exists == null)
                    {
                        classDef.StartingStats.Add(new Rm_AsvtAmountFloat(){AsvtID = def.ID, Amount = def.DefaultValue});                    
                    }
                }

                //Traits
                var traits = classDef.StartingTraitLevels;

                for (int i = 0; i < traits.Count; i++)
                {
                    var v = traits[i];
                    var id = v.AsvtID;
                    var exists = Rm_RPGHandler.Instance.ASVT.TraitDefinitions.FirstOrDefault(e => e.ID == id);
                    if (exists == null)
                    {
                        traits.RemoveAt(i);
                        i--;
                    }
                }

                //add non-existing traits
                foreach (var def in Rm_RPGHandler.Instance.ASVT.TraitDefinitions)
                {
                    var exists = traits.FirstOrDefault(a => a.AsvtID == def.ID);
                    if (exists == null)
                    {
                        classDef.StartingTraitLevels.Add(new Rm_AsvtAmount(){AsvtID = def.ID, Amount = def.StartingLevel});                    
                    }
                }


                //Vitals
                var vitals = classDef.StartingVitals;

                for (int i = 0; i < vitals.Count; i++)
                {
                    var v = vitals[i];
                    var id = v.AsvtID;
                    var exists = Rm_RPGHandler.Instance.ASVT.VitalDefinitions.FirstOrDefault(e => e.ID == id);
                    if (exists == null)
                    {
                        vitals.RemoveAt(i);
                        i--;
                    }
                }

                //add non-existing traits
                foreach (var def in Rm_RPGHandler.Instance.ASVT.VitalDefinitions)
                {
                    var exists = vitals.FirstOrDefault(a => a.AsvtID == def.ID);
                    if (exists == null)
                    {
                        classDef.StartingVitals.Add(new Rm_AsvtAmount(){AsvtID = def.ID, Amount = def.DefaultValue});                    
                    }
                }
            }
            
        }

        private static void UpdateItemInfo()
        {

            //LootTables
            foreach(var lootTable in Rm_RPGHandler.Instance.Repositories.LootTables.AllTables)
            {
                foreach(var lootTableItem in lootTable.LootTableItems)
                {
                    if (!lootTableItem.IsEmpty && !lootTableItem.IsNormalItem && !lootTableItem.IsGold && !lootTableItem.IsCraftableItem && !lootTableItem.IsQuestItem)
                    {
                        lootTableItem.IsNormalItem = true;
                    }
                }
            }

            foreach(var i in Rm_RPGHandler.Instance.Repositories.Items.AllItems.Where(i => i is Weapon))
            {
                var weapon = (Weapon) i;
                var damageTypes = Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions;
                for (int j = 0; j < weapon.Damage.ElementalDamages.Count; j++)
                {
                    var eleDmg = weapon.Damage.ElementalDamages[j];
                    var stillExists = damageTypes.FirstOrDefault(a => a.ID == eleDmg.ElementID);
                    if (stillExists == null)
                    {
                        weapon.Damage.ElementalDamages.Remove(eleDmg);
                        j--;
                    }
                } 
                
                for (int index = 0; index < damageTypes.Count; index++)
                {
                    var v = damageTypes[index];
                    var exists = weapon.Damage.ElementalDamages.FirstOrDefault(a => a.ElementID == v.ID);
                    if (exists == null)
                    {
                        weapon.Damage.ElementalDamages.Add(new ElementalDamage(){ElementID = v.ID});
                    }
                }
            }

            foreach(var i in Rm_RPGHandler.Instance.Items.CraftSlotScalings)
            {
                var slot = Rm_RPGHandler.Instance.Items.ApparelSlots.FirstOrDefault(a => a.ID == i.SlotIdentifier);
                if(slot != null)
                {
                    i.SlotName = slot.Name;
                }
            }
        }

        private static void UpdateInteractables()
        {

            //Update harvestable animations
            foreach (var def in Rm_RPGHandler.Instance.Harvesting.HarvestableDefinitions)
            {
                var anims = def.ClassHarvestingAnims;
                foreach (var d in Rm_RPGHandler.Instance.Player.CharacterDefinitions)
                {
                    var classDef = anims.FirstOrDefault(a => a.ClassID == d.ID );
                    if (classDef == null)
                    {
                        var classAnimToAdd = new ClassAnimationDefinition()
                        {
                            ClassID = d.ID,
                            LegacyAnim = ""
                        };

                        anims.Add(classAnimToAdd);
                    }
                }

                for (int index = 0; index < anims.Count; index++)
                {
                    var v = anims[index];
                    var stillExists =
                        Rm_RPGHandler.Instance.Player.CharacterDefinitions.FirstOrDefault(
                            a => a.ID == v.ClassID);

                    if (stillExists == null)
                    {
                        anims.Remove(v);
                        index--;
                    }
                }
            }
            
                
        }

        private static void UpdateNodes()
        {
            var damageTakenTree = Rm_RPGHandler.Instance.Nodes.DamageTakenTree;
            var damageDealtTree = Rm_RPGHandler.Instance.Nodes.DamageDealtTree;
            var statScalingTree = Rm_RPGHandler.Instance.Nodes.StatScalingTree;
            var vitalScalingTree = Rm_RPGHandler.Instance.Nodes.VitalScalingTree;
            var worldNodeBank = Rm_RPGHandler.Instance.Nodes.WorldMapNodeBank;

            #region "Damage taken + dealt"
            if (damageTakenTree.Variables.FirstOrDefault(n => n.Name == "Attacker" && n.PropType == PropertyType.CombatCharacter) == null)
            {
                damageTakenTree.Variables.Add(new NodeTreeVar("Attacker", PropertyType.CombatCharacter));
            }
            if (statScalingTree.Variables.FirstOrDefault(n => n.Name == "Attacker" && n.PropType == PropertyType.CombatCharacter) == null)
            {
                statScalingTree.Variables.Add(new NodeTreeVar("Attacker", PropertyType.CombatCharacter));
            }
            if (vitalScalingTree.Variables.FirstOrDefault(n => n.Name == "Attacker" && n.PropType == PropertyType.CombatCharacter) == null)
            {
                vitalScalingTree.Variables.Add(new NodeTreeVar("Attacker", PropertyType.CombatCharacter));
            }
            if (damageTakenTree.Variables.FirstOrDefault(n => n.Name == "Defender" && n.PropType == PropertyType.CombatCharacter) == null)
            {
                damageTakenTree.Variables.Add(new NodeTreeVar("Defender", PropertyType.CombatCharacter));
            }

            if (damageDealtTree.Variables.FirstOrDefault(n => n.Name == "Attacker" && n.PropType == PropertyType.CombatCharacter) == null)
            {
                damageDealtTree.Variables.Add(new NodeTreeVar("Attacker", PropertyType.CombatCharacter));
            }

            //remove no longer existing elementals
            var damageTakenTreeVars = damageTakenTree.Variables.Where(n => n.ID.StartsWith("DamageDealtVar_") && n.ID != "DamageDealtVar_Physical").ToList();

            for (int i = 0; i < damageTakenTreeVars.Count; i++)
            {
                var v = damageTakenTreeVars[i];
                var elementalId = v.ID.Replace("DamageDealtVar_", "");
                var exists = Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions.FirstOrDefault(e => e.ID == elementalId);
                if(exists == null)
                {
                    damageTakenTree.Variables.RemoveAt(i);
                    i--;
                }

            }

            //add non-existing elementals to damage tbl
            foreach (var wa in Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions)
            {
                var exists = damageTakenTree.Variables.FirstOrDefault(a => a.ID.Replace("DamageDealtVar_", "") == wa.ID);
                if (exists == null)
                {
                    damageTakenTree.Variables.Add(new NodeTreeVar(wa.Name + " Damage", PropertyType.Int) { ID = "DamageDealtVar_" + wa.ID });
                }
                else
                {
                    exists.Name = wa.Name;
                }
            }

            foreach(var n in damageTakenTree.Nodes.Where(n => n is CustomDamageCombatNode))
            {
                var customDamageNode = n as CustomDamageCombatNode;
                customDamageNode.NewName = RPG.Combat.GetElementalNameById(customDamageNode.DamageId) + " Damage";
            }

            var d = damageTakenTree.Nodes.Where(n => n is CustomDamageCombatNode).ToList();
            for (int index = 0; index < d.Count; index++)
            {
                var n = d[index] as CustomDamageCombatNode;
                var exists = Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions.FirstOrDefault(e => e.ID == n.DamageId);
                if(exists == null)
                {
                    damageTakenTree.Nodes.Remove(n);
                }
            }

            #endregion

            //delete non existing world areas
            for (int index = 0; index < worldNodeBank.NodeTrees.Count; index++)
            {
                var w = worldNodeBank.NodeTrees[index];
                var existingWorldArea = Rm_RPGHandler.Instance.Customise.WorldMapLocations.FirstOrDefault(a => a.ID == w.ID);
                if (existingWorldArea == null)
                {
                    worldNodeBank.NodeTrees.RemoveAt(index);
                    index--;
                }
            }

            //locations
            foreach(var w in worldNodeBank.NodeTrees)
            {
                var existingWorldArea = Rm_RPGHandler.Instance.Customise.WorldMapLocations.First(a => a.ID == w.ID);
                //delete non-existing locations
                for (int index = 0; index < w.Nodes.Count; index++)
                {
                    var n = w.Nodes[index];
                    var existingLocation = existingWorldArea.Locations.FirstOrDefault(a => a.ID == n.ID);
                    if (existingLocation == null)
                    {
                        w.Nodes.RemoveAt(index);
                        index--;
                    }
                }
            }

            //add non-existing world locations
            foreach(var wa in Rm_RPGHandler.Instance.Customise.WorldMapLocations)
            {
                var waTree = worldNodeBank.NodeTrees.FirstOrDefault(a => a.ID == wa.ID);
                if(waTree != null)
                {
                    foreach(var loc in wa.Locations)
                    {
                        var locNode = waTree.Nodes.FirstOrDefault(a => a.ID == loc.ID);
                        if(locNode == null)
                        {
                            var nodePos = Vector2.zero;
                            var nodeToAdd = new LocationNode();
                            var nextId = updateNode_GetNextID(worldNodeBank);
                            waTree.AddNode(nodeToAdd, nodePos, nextId, loc.ID);
                        }
                        else
                        {
                            var locationNode = locNode as LocationNode;
                            locationNode.LocName = loc.Name;
                        }
                    }
                }
            }

            //update node tree var variable names
            foreach(var wa in Rm_RPGHandler.Instance.Nodes.AllTrees)
            {
                var s = wa.Nodes.OfType<NodeTreeVarNode>().ToList();
                for (int i = 0; i < s.Count(); i++)
                {
                    var n = s[i];
                    var treeVar = wa.Variables.FirstOrDefault(v => v.ID == n.VariableId);
                    if (treeVar == null)
                    {
                        wa.Nodes.Remove(n);
                        continue;
                    }
                    n.NewName = treeVar.Name;
                }
            }

            //todo: update min max damage dealt node names

            //Delete damage dealt nodes
            var nodesToDelete = damageDealtTree.Nodes.Where(
                n => n.GetType() == typeof(CombatStartNode) && n.Identifier != "MIN_Physical" && n.Identifier != "MAX_Physical" 
                    && Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions.All(e => ("MIN_" + e.ID) != n.Identifier)
                    && Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions.All(e => ("MAX_" + e.ID) != n.Identifier)
                ).ToList();

            for (int index = 0; index < nodesToDelete.Count; index++)
            {
                var delete = nodesToDelete.ToList()[index];
                updateNode_DeleteNode(delete, damageDealtTree);
            }

            //Delete skill damage dealt nodes
            var skillsWithScaling = Rm_RPGHandler.Instance.Repositories.Skills.AllSkills.Where(s => !string.IsNullOrEmpty(s.DamageScalingTreeID)).ToList();

            foreach(var customSkillTree in skillsWithScaling)
            {
                var tree = Rm_RPGHandler.Instance.Nodes.CombatNodeBank.NodeTrees.FirstOrDefault(n => n.ID == customSkillTree.DamageScalingTreeID);
                if (tree == null) continue;

                var nodeListToDelete = tree.Nodes.Where(
                n => n.GetType() == typeof(CombatStartNode) && n.Identifier != "MIN_Physical" && n.Identifier != "MAX_Physical"
                    && Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions.All(e => ("MIN_" + e.ID) != n.Identifier)
                    && Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions.All(e => ("MAX_" + e.ID) != n.Identifier)
                ).ToList();

                for (int index = 0; index < nodeListToDelete.Count; index++)
                {
                    var delete = nodeListToDelete.ToList()[index];
                    updateNode_DeleteNode(delete, damageDealtTree);
                }
            }
            
            // Delete scaling nodes
            nodesToDelete = statScalingTree.Nodes.Where(n => n.GetType() == typeof (CombatStartNode) && Rm_RPGHandler.Instance.ASVT.StatisticDefinitions.All(e => e.ID != n.Identifier)).ToList();
            for (int index = 0; index < nodesToDelete.Count; index++)
            {
                var delete = nodesToDelete.ToList()[index];
                updateNode_DeleteNode(delete, statScalingTree);
            }

            nodesToDelete = vitalScalingTree.Nodes.Where(n => n.GetType() == typeof(CombatStartNode) && Rm_RPGHandler.Instance.ASVT.VitalDefinitions.All(e => e.ID != n.Identifier)).ToList();
            for (int index = 0; index < nodesToDelete.Count; index++)
            {
                var delete = nodesToDelete.ToList()[index];
                updateNode_DeleteNode(delete, vitalScalingTree);
            }
            
            //damage dealt: add non-existing element damages
            var notAdded = Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions.Where(c => damageDealtTree.Nodes.All(n => n.Identifier != "MAX_" + c.ID)).ToList();
            var existingAmt = Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions.Count - notAdded.Count;
            for (int index = 0; index < notAdded.Count; index++)
            {
                var element = notAdded[index];

                if(Rm_RPGHandler.Instance.Items.DamageHasVariance)
                {
                    var newNode = new CombatStartNode("Min \"" + element.Name + "\" Damage", "Min Damage for " + element.Name, "Damage dealt by an element starts at 0. You can add additional calculations to increase this amount. Example: Create a statistic called fire damage and add this amount.") { Identifier = "MIN_" + element.ID };
                    newNode.WindowID = updateNode_GetNextID(Rm_RPGHandler.Instance.Nodes.CombatNodeBank);
                    newNode.Rect = new Rect(25, 250 + ((existingAmt + index) * 200), 50, 50);
                    newNode.OnCreate();
                    damageDealtTree.Nodes.Add(newNode);
                }

                var newNodeMax = new CombatStartNode("Max \"" + element.Name + "\" Damage", "Max Damage for " + element.Name, "Damage dealt by an element starts at 0. You can add additional calculations to increase this amount. Example: Create a statistic called fire damage and add this amount.") { Identifier = "MAX_" + element.ID };
                newNodeMax.WindowID = updateNode_GetNextID(Rm_RPGHandler.Instance.Nodes.CombatNodeBank);
                newNodeMax.Rect = new Rect(25, 350 + ((existingAmt + index) * 200), 50, 50);
                newNodeMax.OnCreate();
                damageDealtTree.Nodes.Add(newNodeMax);
            }
                
            //skill damage dealt: add non-existing element damages
            foreach (var customSkillTree in skillsWithScaling)
            {
                var tree = Rm_RPGHandler.Instance.Nodes.CombatNodeBank.NodeTrees.FirstOrDefault(n => n.ID == customSkillTree.DamageScalingTreeID);
                var notAddedNodes = Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions.Where(c => tree.Nodes.All(n => n.Identifier != "MAX_" + c.ID)).ToList();
                var existingAmtOfNodes = Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions.Count - notAddedNodes.Count;        
                for (int index = 0; index < notAddedNodes.Count; index++)
                {
                    var element = notAddedNodes[index];

                    if (Rm_RPGHandler.Instance.Items.DamageHasVariance)
                    {
                        var newNode = new CombatStartNode("Min \"" + element.Name + "\" Damage", "Min Damage for " + element.Name, "Damage dealt by an element starts at 0. You can add additional calculations to increase this amount. Example: Create a statistic called fire damage and add this amount.") { Identifier = "MIN_" + element.ID };
                        newNode.WindowID = updateNode_GetNextID(Rm_RPGHandler.Instance.Nodes.CombatNodeBank);
                        newNode.Rect = new Rect(25, 250 + ((existingAmtOfNodes + index) * 200), 50, 50);
                        newNode.OnCreate();
                        tree.Nodes.Add(newNode);
                    }

                    var newNodeMax = new CombatStartNode("Max \"" + element.Name + "\" Damage", "Max Damage for " + element.Name, "Damage dealt by an element starts at 0. You can add additional calculations to increase this amount. Example: Create a statistic called fire damage and add this amount.") { Identifier = "MAX_" + element.ID };
                    newNodeMax.WindowID = updateNode_GetNextID(Rm_RPGHandler.Instance.Nodes.CombatNodeBank);
                    newNodeMax.Rect = new Rect(25, 350 + ((existingAmtOfNodes + index) * 200), 50, 50);
                    newNodeMax.OnCreate();
                    tree.Nodes.Add(newNodeMax);
                }
            }

         
            foreach (var x in damageDealtTree.Nodes.Where(n => n.GetType() == typeof(CombatStartNode) && !n.Identifier.Contains("Physical")))
            {
                //Substring 3 to ignore (MAX_) or (MIN_)
                var element = Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions.FirstOrDefault(e => e.ID == x.Identifier.Substring(4));
                if (element != null)
                {
                    var n = (CombatStartNode)x;
                    n.NodeChainName = n.Identifier.Substring(0,3) + " [" + element.Name + "] Damage";
                    n.NewName = element.Name;
                    n.NewSubText = "Damage for " + element.Name;    
                }
            }

            foreach (var customSkillTree in skillsWithScaling)
            {
                var tree = Rm_RPGHandler.Instance.Nodes.CombatNodeBank.NodeTrees.FirstOrDefault(n => n.ID == customSkillTree.DamageScalingTreeID);
                foreach (var x in tree.Nodes.Where(n => n.GetType() == typeof(CombatStartNode) && !n.Identifier.Contains("Physical")))
                {
                    //Substring 3 to ignore (MAX_) or (MIN_)
                    var element = Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions.FirstOrDefault(e => e.ID == x.Identifier.Substring(4));
                    if (element != null)
                    {
                        var n = (CombatStartNode)x;
                        n.NodeChainName = n.Identifier.Substring(0, 3) + " [" + element.Name + "] Damage";
                        n.NewName = element.Name;
                        n.NewSubText = "Damage for " + element.Name;
                    }
                } 
            }
                
            
            //stat scaling: add non-existing stats
            var statsNotAdded = Rm_RPGHandler.Instance.ASVT.StatisticDefinitions.Where(c => statScalingTree.Nodes.All(n => n.Identifier != c.ID)).ToList();
            var existingStatAmt = Rm_RPGHandler.Instance.ASVT.StatisticDefinitions.Count - statsNotAdded.Count;
            for (int index = 0; index < statsNotAdded.Count; index++)
            {
                var element = statsNotAdded[index];
                var newNode = new CombatStartNode(element.Name, "Scaling for " + element.Name, "Add additional nodes to modify base value of a statistic based on other values, e.g. an attribute. Remember: don't cause circular scaling as this can result in an infinite loop.") { Identifier = element.ID };
                newNode.WindowID = updateNode_GetNextID(Rm_RPGHandler.Instance.Nodes.CombatNodeBank);
                newNode.Rect = new Rect(25, 87.5f + ((existingStatAmt + index) * 200), 50, 50);
                newNode.OnCreate();
                statScalingTree.Nodes.Add(newNode);
            }

            foreach (var x in statScalingTree.Nodes.Where(n => n.GetType() == typeof(CombatStartNode)))
            {
                var element = Rm_RPGHandler.Instance.ASVT.StatisticDefinitions.FirstOrDefault(e => e.ID == x.Identifier);
                if (element != null)
                {
                    var n = (CombatStartNode)x;
                    n.NewName = element.Name;
                    n.NewSubText = "Scaling for " + element.Name;
                }
            }


            //stat scaling: add non-existing stats
            var vitNotAdded = Rm_RPGHandler.Instance.ASVT.VitalDefinitions.Where(c => vitalScalingTree.Nodes.All(n => n.Identifier != c.ID)).ToList();
            var existingVitAmt = Rm_RPGHandler.Instance.ASVT.VitalDefinitions.Count - vitNotAdded.Count;
            for (int index = 0; index < vitNotAdded.Count; index++)
            {
                var element = vitNotAdded[index];
                var newNode = new CombatStartNode(element.Name, "Scaling for " + element.Name, "Add additional nodes to modify base value of a vital based on other values, e.g. an attribute. Remember: don't cause circular scaling as this can result in an infinite loop.") { Identifier = element.ID };
                newNode.WindowID = updateNode_GetNextID(Rm_RPGHandler.Instance.Nodes.CombatNodeBank);
                newNode.Rect = new Rect(25, 87.5f + ((existingVitAmt + index) * 200), 50, 50);
                newNode.OnCreate();
                vitalScalingTree.Nodes.Add(newNode);
            }

            foreach (var x in vitalScalingTree.Nodes.Where(n => n.GetType() == typeof(CombatStartNode)))
            {
                var element = Rm_RPGHandler.Instance.ASVT.VitalDefinitions.FirstOrDefault(e => e.ID == x.Identifier);
                if (element != null)
                {
                    var n = (CombatStartNode)x;
                    n.NewName = element.Name;
                    n.NewSubText = "Scaling for " + element.Name;
                }
            }
        }

        private static int updateNode_GetNextID(NodeBank nodeBank)
        {
            return nodeBank.NodeTrees.SelectMany(n => n.Nodes).Any()
                       ? (nodeBank.NodeTrees.SelectMany(n => n.Nodes).Max(n => n.WindowID) + 1)
                       : 0;
        }
        private static void updateNode_ClearPrevLinks(Node node, NodeTree nodeTree)
        {
            foreach (var link in node.PrevNodeLinks)
            {
                var foundNode = nodeTree.Nodes.FirstOrDefault(n => n.ID == link.ID);
                if (foundNode != null)
                {
                    foundNode.NextNodeLinks.Where(n => n.ID == node.ID).ToList().ForEach(s => s.ID = "");
                }
            }

            node.PrevNodeLinks = new List<StringField>();
        }
        private static void updateNode_DeleteNode(Node nodeToDelete, NodeTree nodeTree)
        {
            updateNode_ClearPrevLinks(nodeToDelete, nodeTree);
            for (int i = 0; i < nodeToDelete.NextNodeLinks.Count; i++)
            {
                updateNode_RemoveLink(nodeToDelete, i, nodeTree);
            }

            nodeTree.Nodes.Remove(nodeToDelete);
        }
        private static void updateNode_RemoveLink(Node node, int linkIndex, NodeTree nodeTree)
        {
            var linkedNodeID = node.NextNodeLinks[linkIndex];
            var foundNode = nodeTree.Nodes.FirstOrDefault(n => n.ID == linkedNodeID.ID);
            if (foundNode != null)
            {
                var link = foundNode.PrevNodeLinks.FirstOrDefault(s => s.ID == node.ID);
                if (link != null)
                {
                    foundNode.PrevNodeLinks.Remove(link);
                }
            }
            linkedNodeID.ID = "";
        }

        private static void UpdateCombatants()
        {
            //Update enemies and npcs
            var enemiesAndnpcs = Rm_RPGHandler.Instance.Repositories.Enemies.AllEnemies.Concat(
                Rm_RPGHandler.Instance.Repositories.Interactable.AllNpcs.Select(n => n as CombatCharacter)
                ).ToList();

            for (int charIndex = 0; charIndex < enemiesAndnpcs.Count; charIndex++)
            {
                var selectedCharInfo = enemiesAndnpcs[charIndex];

                //Update Attributes
                foreach (var d in Rm_RPGHandler.Instance.ASVT.AttributesDefinitions)
                {
                    var attr = selectedCharInfo.Attributes.FirstOrDefault(a => a.ID == d.ID);
                    if (attr == null)
                    {
                        var attributeToAdd = new Attribute
                        {
                            ID = d.ID,
                            BaseValue = d.DefaultValue
                        };

                        selectedCharInfo.Attributes.Add(attributeToAdd);
                    }
                }

                for (int index = 0; index < selectedCharInfo.Attributes.Count; index++)
                {
                    var v = selectedCharInfo.Attributes[index];
                    var stillExists =
                        Rm_RPGHandler.Instance.ASVT.AttributesDefinitions.FirstOrDefault(
                            a => a.ID == v.ID);

                    if (stillExists == null)
                    {
                        selectedCharInfo.Attributes.Remove(v);
                        index--;
                    }
                }

                //Update Vitals
                foreach (var d in Rm_RPGHandler.Instance.ASVT.VitalDefinitions)
                {
                    var vital = selectedCharInfo.Vitals.FirstOrDefault(a => a.ID == d.ID);
                    if (vital == null)
                    {
                        var vitToAdd = new Vital
                        {
                            ID = d.ID,
                            BaseValue = d.DefaultValue
                        };

                        selectedCharInfo.Vitals.Add(vitToAdd);
                    }
                }

                for (int index = 0; index < selectedCharInfo.Vitals.Count; index++)
                {
                    var v = selectedCharInfo.Vitals[index];
                    var stillExists =
                        Rm_RPGHandler.Instance.ASVT.VitalDefinitions.FirstOrDefault(
                            a => a.ID == v.ID);

                    if (stillExists == null)
                    {
                        selectedCharInfo.Vitals.Remove(v);
                        index--;
                    }
                }

                //Update statistics
                foreach (var d in Rm_RPGHandler.Instance.ASVT.StatisticDefinitions)
                {
                    var stat = selectedCharInfo.Stats.FirstOrDefault(a => a.ID == d.ID);
                    if (stat == null)
                    {
                        var statToAdd = new Statistic()
                        {
                            ID = d.ID,
                            BaseValue = d.DefaultValue
                        };

                        selectedCharInfo.Stats.Add(statToAdd);
                    }
                }

                for (int index = 0; index < selectedCharInfo.Stats.Count; index++)
                {
                    var v = selectedCharInfo.Stats[index];
                    var stillExists =
                        Rm_RPGHandler.Instance.ASVT.StatisticDefinitions.FirstOrDefault(
                            a => a.ID == v.ID);

                    if (stillExists == null)
                    {
                        selectedCharInfo.Stats.Remove(v);
                        index--;
                    }
                }

                //Update Immunities
                for (int index = 0; index < selectedCharInfo.SkillMetaImmunitiesID.Count; index++)
                {
                    var v = selectedCharInfo.SkillMetaImmunitiesID[index];
                    if (string.IsNullOrEmpty(v.ID)) continue;

                    var stillExists =
                        Rm_RPGHandler.Instance.Combat.SkillMeta.FirstOrDefault(
                            a => a.ID == v.ID);

                    if (stillExists == null)
                    {
                        selectedCharInfo.SkillMetaImmunitiesID.Remove(v);
                        index--;
                    }
                }

                //Update Susceptibilities
                for (int index = 0; index < selectedCharInfo.SkillMetaSusceptibilities.Count; index++)
                {
                    var v = selectedCharInfo.SkillMetaSusceptibilities[index];
                    if (string.IsNullOrEmpty(v.ID)) continue;

                    var stillExists =
                        Rm_RPGHandler.Instance.Combat.SkillMeta.FirstOrDefault(
                            a => a.ID == v.ID);

                    if (stillExists == null)
                    {
                        selectedCharInfo.SkillMetaSusceptibilities.Remove(v);
                        index--;
                    }
                }

                //Update skills
                for (int index = 0; index < selectedCharInfo.EnemySkills.Count; index++)
                {
                    var v = selectedCharInfo.EnemySkills[index];
                    if (string.IsNullOrEmpty(v.SkillID)) continue;

                    var stillExists =
                        Rm_RPGHandler.Instance.Repositories.Skills.AllSkills.FirstOrDefault(
                            a => a.ID == v.SkillID);

                    if (stillExists == null)
                    {
                        selectedCharInfo.EnemySkills.Remove(v);
                        index--;
                    }
                }

                //Update damages
                var dmgList = selectedCharInfo.NpcDamage.ElementalDamages;
                foreach (var d in Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions)
                {
                    var tier = dmgList.FirstOrDefault(t => t.ElementID == d.ID);
                    if (tier == null)
                    {
                        var tierToAdd = new ElementalDamage() { ElementID = d.ID };
                        dmgList.Add(tierToAdd);
                    }
                }

                for (int index = 0; index < dmgList.Count; index++)
                {
                    var v = dmgList[index];
                    var stillExists =
                        Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions.FirstOrDefault(
                            t => t.ID == v.ElementID);

                    if (stillExists == null)
                    {
                        dmgList.Remove(v);
                        index--;
                    }
                }

                //Update LootTables
                for (int index = 0; index < selectedCharInfo.LootTables.Count; index++)
                {
                    var v = selectedCharInfo.LootTables[index];
                    if (string.IsNullOrEmpty(v.LootTableID)) continue;

                    var stillExists =
                        Rm_RPGHandler.Instance.Repositories.LootTables.AllTables.FirstOrDefault(
                            a => a.ID == v.LootTableID);

                    if (stillExists == null)
                    {
                        selectedCharInfo.LootTables.Remove(v);
                        index--;
                    }
                }

                //Update Loot
                for (int index = 0; index < selectedCharInfo.GuaranteedLoot.Count; index++)
                {
                    var v = selectedCharInfo.GuaranteedLoot[index];
                    if (string.IsNullOrEmpty(v.ItemID)) continue;

                    var stillExists =
                        Rm_RPGHandler.Instance.Repositories.Items.AllItems.FirstOrDefault(
                            a => a.ID == v.ItemID);

                    if (stillExists == null)
                    {
                        selectedCharInfo.GuaranteedLoot.Remove(v);
                        index--;
                    }
                }
            }
        }

        public static void AutoSave()
        {
            if (Rm_RPGHandler.Instance == null) return;

            if (!Rm_RPGHandler.Instance.Preferences.EnableAutoSave) return;
            if (Application.isPlaying) return;

            if(AutoSaveTime == 0)
            {
                if(Rm_RPGHandler.Instance != null)  
                {
                    InitAutoSave();
                }
                else
                {
                    return;
                }
            }

            if (Time.realtimeSinceStartup >= AutoSaveTime)
            {
                EditorGameDataSaveLoad.SaveGameData(true);
                Notify.Save("Autosaved Game Data");
                InitAutoSave();
            }
        }

        public static void InitAutoSave()
        {
            Rm_RPGHandler.Instance.Preferences.AutoSaveFrequency = Mathf.Max(Rm_RPGHandler.Instance.Preferences.AutoSaveFrequency, 30);
            AutoSaveTime = Time.realtimeSinceStartup + Rm_RPGHandler.Instance.Preferences.AutoSaveFrequency;
        }
    }
}
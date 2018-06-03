using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;
using UnityEditor;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Editor
{
    public static class Rme_Main_Player
    {
        public static Rmh_Player Player
        {
            get { return Rm_RPGHandler.Instance.Player; }
        }
        #region Options

        private static int selectedStartDiffIndex;
        private static bool showDifficulties = true;
        private static Vector2 optionsScrollPos = Vector2.zero;
        public static void Options(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            GUI.Box(fullArea, "", "backgroundBox");
            GUILayout.BeginArea(fullArea);
            optionsScrollPos = GUILayout.BeginScrollView(optionsScrollPos);
            RPGMakerGUI.Title("Player - Options");

            RPGMakerGUI.SubTitle("Character Creation Options");
            if(!RPGMakerGUI.Toggle("Skip Character Creation", ref Player.SkipCharacterCreation))
            {
                RPGMakerGUI.Toggle("Skip Race Selection", ref Player.SkipRaceSelection);
                if(RPGMakerGUI.Toggle("Skip Sub-Race Selection", ref Player.SkipSubRaceSelection))
                {
                    RPGMakerGUI.Toggle("Remove Description Panel", 1, ref Player.RemoveSubRaceDescription);
                }
                RPGMakerGUI.Toggle("Enable Customisations?", ref Player.CustomisationsEnabled);
            }
            Player.ShownDescription = (DescriptionShown) RPGMakerGUI.EnumPopup("Show which description?", Player.ShownDescription);

            if (Player.SkipCharacterCreation || Player.SkipRaceSelection || Player.SkipSubRaceSelection)
            {
                EditorGUILayout.HelpBox("Skipping race or sub-race selection will use the first race / sub-race. They will not show up in character info however if skipped.", MessageType.Info);
            }
            if (Player.SkipCharacterCreation)
            {
                EditorGUILayout.HelpBox("Skipping character creation will use the first available character defined with no character name.", MessageType.Info);
            }

            RPGMakerGUI.SubTitle("More Options");
            if(RPGMakerGUI.Toggle("Use Custom Exp Formula", ref Rm_RPGHandler.Instance.Player.UseCustomExperienceFormula))
            {
                EditorGUILayout.HelpBox("Edit Script in: Assets/RPGAIO/CustomScripts/CustomExpFormula.cs", MessageType.Info);
            }

            
            if(RPGMakerGUI.Toggle("Assign Attribute Points Per Level?", ref Player.ManualAssignAttrPerLevel))
            {
                Player.AttributePointsEarnedPerLevel = RPGMakerGUI.IntField("Points Per Level", Player.AttributePointsEarnedPerLevel, 1);
            }
            if (RPGMakerGUI.Toggle("Give Skill Points Per Level?", ref Player.GiveSkillPointsPerLevel))
            {
                Player.SkillPointsEarnedPerLevel = RPGMakerGUI.IntField("Points Per Level", Player.SkillPointsEarnedPerLevel, 1);
            }

            var result = RPGMakerGUI.FoldoutToolBar(ref showDifficulties, "Difficulties", new string[] {"Add Difficulty"});
            if(showDifficulties)
            {
                for (int index = 0; index < Player.Difficulties.Count; index++)
                {
                    var difficulty = Player.Difficulties[index];
                    GUILayout.BeginHorizontal();
                    difficulty.Name = RPGMakerGUI.TextField("",difficulty.Name);
                    difficulty.DamageMultiplier = EditorGUILayout.Slider("Damage Multiplier:", difficulty.DamageMultiplier, 0.01f, 25.0f);
                    GUILayout.Label((difficulty.DamageMultiplier * 100) + "%",GUILayout.Width(80));
                    GUILayout.Space(5);
                    if (Player.Difficulties.Count > 1)
                    {
                        if (RPGMakerGUI.DeleteButton(25))
                        {
                            Player.Difficulties.Remove(difficulty);
                            index--;
                        }
                    }
                    GUILayout.EndHorizontal();
                }

                if(result == 0)
                {
                    Player.Difficulties.Add(new DifficultyDefinition("New Difficulty",100));
                }

                RPGMakerGUI.EndFoldout();
            }

            #region StartingDifficulty

            RPGMakerGUI.PopupID<DifficultyDefinition>("Starting Difficulty:",ref Player.DefaultDifficultyID);

            #endregion


            Player.LevelUpSound.Audio = RPGMakerGUI.AudioClipSelector("Level Up Sound:", Player.LevelUpSound.Audio,
                                                                ref Player.LevelUpSound.AudioPath);

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        #endregion


        #region Classes
        private static bool mainInfoFoldout = true;
        private static bool animFoldout = true;
        private static bool animTypeFoldout = true;
        private static bool skillSusFoldout = true;
        private static bool skillImmunFoldout = true;
        private static bool showMecanimAnim = true;
        private static bool wepFoldout = true;
        private static bool[] advWepFoldout = new bool[99];

        private static bool startVisualFoldout = true;
        private static bool startColorFoldout = true;
        private static bool startAttrFoldout = true;
        private static bool startStatFoldout = true;
        private static bool startVitalFoldout = true;
        private static bool startTraitFoldout = true;
        private static bool projectileInfoFoldout = true;

        private static bool startItemsFoldout = true;
        private static bool startEquipFoldout = true;

        private static bool attrPerLevelFoldout = true;
        private static bool startingSkillsFoldout = true;
        private static bool startingTalentsFoldout = true;
        private static bool startingPetFoldout = true;
        private static int[] selectedStartSkill = new int[999];
        private static int[] selectedSkillSus = new int[999];
        private static int[] selectedSkillImmunity = new int[999];
        private static int[] selectedAnim = new int[999];
        private static int selectedStartScene = 0;
        private static Rm_ClassDefinition _selectedClassInfo = null;
        private static GameObject selectedPrefab = null;
        private static List<AnimationDefinition> KeyAnimations
        {
            get{return new List<AnimationDefinition>()
                           {
                               _selectedClassInfo.LegacyAnimations.UnarmedAnim,
                               _selectedClassInfo.LegacyAnimations.WalkAnim,
                               _selectedClassInfo.LegacyAnimations.WalkBackAnim,
                               _selectedClassInfo.LegacyAnimations.RunAnim,
                               _selectedClassInfo.LegacyAnimations.JumpAnim,
                               _selectedClassInfo.LegacyAnimations.TurnRightAnim,
                               _selectedClassInfo.LegacyAnimations.TurnLeftAnim,
                               _selectedClassInfo.LegacyAnimations.StrafeRightAnim,
                               _selectedClassInfo.LegacyAnimations.StrafeLeftAnim,
                               _selectedClassInfo.LegacyAnimations.IdleAnim,
                               _selectedClassInfo.LegacyAnimations.CombatIdleAnim,
                               _selectedClassInfo.LegacyAnimations.TakeHitAnim,
                               _selectedClassInfo.LegacyAnimations.FallAnim,
                               _selectedClassInfo.LegacyAnimations.DeathAnim,
                               _selectedClassInfo.LegacyAnimations.KnockBackAnim,
                               _selectedClassInfo.LegacyAnimations.KnockUpAnim
                           };}
        }

        private static GameObject gameObject = null;
        private static bool UseSelectedForAnims;
        private static bool ShowClassNameFoldout = true;
        private static bool showTargetedGameObjects;
        private static bool showMaterialOptions;
        private static bool showScaleChildOptions;
        private static GameObject selected = null;
        private static UnityEngine.Material mat;
        private static int selectedVisualCustDisplay = 0;
        public static void Classes(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            var list = Rm_RPGHandler.Instance.Player.CharacterDefinitions;
            GUI.Box(leftArea, "", "backgroundBox");
            GUI.Box(mainArea, "", "backgroundBoxMain");

            GUILayout.BeginArea(PadRect(leftArea, 0, 0));
            RPGMakerGUI.ListArea(list, ref _selectedClassInfo, Rm_ListAreaType.Classes, false, true);
            GUILayout.EndArea();


            GUILayout.BeginArea(mainArea);
            RPGMakerGUI.Title("Classes");
            if (_selectedClassInfo != null)
            {
                RPGMakerGUI.BeginScrollView();
                if (RPGMakerGUI.Foldout(ref mainInfoFoldout, "Main Info"))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical(GUILayout.MaxWidth(85));
                    _selectedClassInfo.Image = RPGMakerGUI.ImageSelector("", _selectedClassInfo.Image,
                                                                        ref _selectedClassInfo.ImagePath);

                    GUILayout.EndVertical();
                    GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                    _selectedClassInfo.Name = RPGMakerGUI.TextField("Identifier: ", _selectedClassInfo.Name);
                    _selectedClassInfo.Description = RPGMakerGUI.TextArea("Description: ", _selectedClassInfo.Description);
                    RPGMakerGUI.Help("[RPGAIO] Ensure the sub-race matches the race.", 0);
                    RPGMakerGUI.PopupID<Rm_RaceDefinition>("Applicable Race:", ref _selectedClassInfo.ApplicableRaceID);
                    RPGMakerGUI.PopupID<Rm_SubRaceDefinition>("Applicable Sub-Race:", ref _selectedClassInfo.ApplicableSubRaceID);
                    RPGMakerGUI.PopupID("Applicable Gender:", ref _selectedClassInfo.ApplicableGenderID,"ID","Name","", Rm_RPGHandler.Instance.Player.GenderDefinitions);

                    RPGMakerGUI.FoldoutList(ref ShowClassNameFoldout, "Applicable Class Names",
                                            _selectedClassInfo.ApplicableClassIDs,
                                            Rm_RPGHandler.Instance.Player.ClassNameDefinitions, "+Class");

                    if(_selectedClassInfo.ApplicableClassIDs.Count == 0)    
                    {
                        RPGMakerGUI.Help("<color=red>Error</color> Character Definitions require at least 1 applicable class name (e.g. Warrior, Archer, Mage, etc.", 0);
                    }



                    GUILayout.BeginHorizontal();
                    gameObject = RPGMakerGUI.PrefabSelector("Character Prefab:", gameObject, ref _selectedClassInfo.ClassPrefabPath);
                    gameObject = RPGMakerGUI.PrefabGeneratorButton(Rmh_PrefabType.Player_Class, gameObject, ref _selectedClassInfo.ClassPrefabPath, null, _selectedClassInfo.ID);
                    GUILayout.EndHorizontal(); 

                    if(gameObject != null && gameObject.name != _selectedClassInfo.Name)
                    {
                        RPGMakerGUI.Help("<color=red>Error</color> Character Prefab name should match identifier. Rename the prefab in 'Resources/Prefabs/Classes'", 0);
                        RPGMakerGUI.Help("to be the same as your character's identifier in RPGAIO > Player > Characters", 0);
                    }


                    if(!Rm_RPGHandler.Instance.Player.UseCustomExperienceFormula)
                    {
                        RPGMakerGUI.PopupID<ExpDefinition>("Exp Definition:", ref _selectedClassInfo.ExpDefinitionID);    
                    }
                    
                    _selectedClassInfo.AttackStyle = (AttackStyle) RPGMakerGUI.EnumPopup("Attack Style:", _selectedClassInfo.AttackStyle);
                    

                    _selectedClassInfo.StartingGold = RPGMakerGUI.IntField("Starting Gold:", _selectedClassInfo.StartingGold);
                    
                    //todo: make this better (look nicer)
                    if(RPGMakerGUI.Toggle("Starting at World Location?", ref _selectedClassInfo.StartAtWorldLocation))
                    {

                        RPGMakerGUI.PopupID<WorldArea>("Initial World Area:", ref _selectedClassInfo.StartingWorldArea);
                        RPGMakerGUI.PopupID<Location>("Initial Locaiton:", ref _selectedClassInfo.StartingLocation);
                    }
                    else
                    {
                        RPGMakerGUI.SceneSelector("Starting Scene:", ref _selectedClassInfo.StartingScene);
                    }

                    if(RPGMakerGUI.Toggle("Has Starting Quest?",ref _selectedClassInfo.HasStartingQuest))
                    {
                        RPGMakerGUI.PopupID<Quest>("Starting Quest:", ref _selectedClassInfo.StartingQuestID);
                    }

                    RPGMakerGUI.Label("Unarmed:");
                    _selectedClassInfo.UnarmedAttackDamage = RPGMakerGUI.IntField("- Attack Damage:", _selectedClassInfo.UnarmedAttackDamage);
                    _selectedClassInfo.UnarmedAttackRange = RPGMakerGUI.FloatField("- Attack Range:", _selectedClassInfo.UnarmedAttackRange);
                    _selectedClassInfo.UnarmedAttackSpeed = RPGMakerGUI.FloatField("- Attack Speed:", _selectedClassInfo.UnarmedAttackSpeed);

                    GUILayout.Space(5);
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    RPGMakerGUI.EndFoldout();
                }

                #region AutoAttack Details

                if (RPGMakerGUI.Foldout(ref projectileInfoFoldout, "Auto Attack Details"))
                {

                    GUILayout.BeginHorizontal();
                    var isMelee = _selectedClassInfo.AttackStyle == AttackStyle.Melee;
                    var autoAttackType = isMelee ? "Melee Effect Prefab:" : "Projectile Prefab:";
                    selectedPrefab = RPGMakerGUI.PrefabSelector(autoAttackType, selectedPrefab,
                                                                        ref _selectedClassInfo.AutoAttackPrefabPath);
                    selectedPrefab = RPGMakerGUI.PrefabGeneratorButton(isMelee ? Rmh_PrefabType.Melee_Effect : Rmh_PrefabType.Auto_Attack_Projectile, selectedPrefab, ref _selectedClassInfo.AutoAttackPrefabPath);
                    GUILayout.EndHorizontal();


                    if (_selectedClassInfo.AttackStyle == AttackStyle.Ranged)
                    {
                        _selectedClassInfo.ProjectileSpeed = RPGMakerGUI.FloatField("Projectile Speed:", _selectedClassInfo.ProjectileSpeed);

                        _selectedClassInfo.ProjectileTravelSound.Audio = RPGMakerGUI.AudioClipSelector("Projectile Travel Sound:", _selectedClassInfo.ProjectileTravelSound.Audio, ref _selectedClassInfo.ProjectileTravelSound.AudioPath);
                    }


                    GUILayout.BeginHorizontal();
                    selectedPrefab = RPGMakerGUI.PrefabSelector("Impact Prefab:", selectedPrefab,
                                                                        ref _selectedClassInfo.AutoAttackImpactPrefabPath);
                    selectedPrefab = RPGMakerGUI.PrefabGeneratorButton(Rmh_PrefabType.Impact, selectedPrefab, ref _selectedClassInfo.AutoAttackImpactPrefabPath);
                    GUILayout.EndHorizontal();


                    _selectedClassInfo.AutoAttackImpactSound.Audio = RPGMakerGUI.AudioClipSelector("Impact Sound:", _selectedClassInfo.AutoAttackImpactSound.Audio, ref _selectedClassInfo.AutoAttackImpactSound.AudioPath);


                    RPGMakerGUI.EndFoldout();
                }

                #endregion

                #region Animation

                var foundAnim = false;
                String[] anims = new string[0];

                if (UseSelectedForAnims)
                {
                    if (selected != null)
                    {
                        var activeObj = Selection.activeObject;
                        var curSelected = (activeObj as GameObject);
                        if (curSelected != null && curSelected != selected)
                        {
                            UseSelectedForAnims = false;
                            selected = null;
                        }
                    }
                }
                if(UseSelectedForAnims)
                {
                    if (animFoldout || wepFoldout || advWepFoldout.Any(a => a))
                    {
                        var activeObj = Selection.activeObject;
                        selected = activeObj as GameObject;
                        Animation animation = null;
                        foundAnim = false;
                        if (selected != null)
                        {
                            animation = selected.GetComponent<Animation>();
                            if (animation != null)
                            {
                                if (animation.GetClipCount() > 0)
                                    foundAnim = true;
                            }
                        }

                        if (foundAnim)
                        {
                            anims = new string[animation.GetClipCount() + 1];
                            anims[0] = "None";
                            int counter = 1;
                            foreach (AnimationState state in animation)
                            {
                                if (counter > animation.GetClipCount()) break;
                                anims[counter] = state.name;
                                counter++;
                            }
                        }
                    }
                }

                #region Anim Types

                if (RPGMakerGUI.Foldout(ref animTypeFoldout, "Animation Mode"))
                {
                    _selectedClassInfo.AnimationType = (RPGAnimationType)RPGMakerGUI.EnumPopup("Animation Type", _selectedClassInfo.AnimationType);

                    if (_selectedClassInfo.AnimationType == RPGAnimationType.Mecanim)
                    {
                        GUILayout.Label("For more information mecanim please visit, https://www.youtube.com/watch?v=0cvwtk8Y_Uk ");
                    }

                    RPGMakerGUI.EndFoldout();
                }

                #endregion

                //if (_selectedClassInfo.AnimationType == RPGAnimationType.Legacy)
                //{

                    #region All Animation Info

                    #region Core Anims

                    if (RPGMakerGUI.Foldout(ref animFoldout, "Core Animations"))
                    {
                        if(_selectedClassInfo.AnimationType == RPGAnimationType.Legacy)
                        {
                            RPGMakerGUI.Toggle("Get Animations From Selected Object?", ref UseSelectedForAnims);
                        }

                        GUILayout.BeginHorizontal("backgroundBox");
                        GUILayout.Label("ID", GUILayout.Width(100));
                        if (_selectedClassInfo.AnimationType == RPGAnimationType.Legacy)
                        {
                            GUILayout.Label("Animation", GUILayout.Width(150));
                            GUILayout.Label("Speed", GUILayout.Width(70));
                            GUILayout.Label("Sound", GUILayout.Width(150));
                            GUILayout.Label("Reverse?", GUILayout.Width(100));
                        }
                        else
                        {
                            GUILayout.Label(" ", GUILayout.Width(150));
                            GUILayout.Label("Sound", GUILayout.Width(150));
                        }
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        if (_selectedClassInfo.AnimationType == RPGAnimationType.Legacy && foundAnim)
                        {
                            for (int index = 0; index < KeyAnimations.Count; index++)
                            {
                                var anim = KeyAnimations[index];
                                GUILayout.BeginHorizontal();
                                GUILayout.Label(anim.Name, GUILayout.Width(100));

                                selectedAnim[index] = anims.FirstOrDefault(s => s == anim.Animation) != null
                                                          ? Array.IndexOf(anims, anim.Animation)
                                                          : 0;

                                selectedAnim[index] = EditorGUILayout.Popup(selectedAnim[index], anims,
                                                                            GUILayout.Width(150));
                                anim.Animation = anims[selectedAnim[index]];
                                if (anim.Animation == "None") anim.Animation = "";
                                anim.Speed = RPGMakerGUI.FloatField("", anim.Speed, GUILayout.Width(70));
                                anim.Sound = RPGMakerGUI.AudioClipSelector("", anim.Sound, ref anim.SoundPath,
                                                                           GUILayout.Width(150));
                                anim.Backwards = RPGMakerGUI.Toggle(anim.Backwards);
                                GUILayout.FlexibleSpace();
                                GUILayout.EndHorizontal();
                            }
                        }
                        else
                        {
                            if (_selectedClassInfo.AnimationType == RPGAnimationType.Legacy)
                            {
                                EditorGUILayout.HelpBox(
                                    "Select a gameObject with the animation component and animations to get drop-downs for animation name instead of typing it in.",
                                    MessageType.Info);
                            }
                            else
                            {
                                EditorGUILayout.HelpBox("For more information on mecanim visit https://www.youtube.com/watch?v=0cvwtk8Y_Uk", MessageType.Info);
                            }
                            foreach (var anim in KeyAnimations)
                            {
                                GUILayout.BeginHorizontal();
                                GUILayout.Label(anim.Name, GUILayout.Width(100));
                                if (_selectedClassInfo.AnimationType == RPGAnimationType.Legacy)
                                {
                                    anim.Animation = RPGMakerGUI.TextField("", anim.Animation, GUILayout.Width(150));
                                    anim.Speed = RPGMakerGUI.FloatField("", anim.Speed, GUILayout.Width(70));
                                    anim.Sound = RPGMakerGUI.AudioClipSelector("", anim.Sound, ref anim.SoundPath,
                                                                               GUILayout.Width(150));
                                    anim.Backwards = RPGMakerGUI.Toggle(anim.Backwards);
                                }
                                else
                                {
                                    GUILayout.Space(150);
                                    anim.Sound = RPGMakerGUI.AudioClipSelector("", anim.Sound, ref anim.SoundPath,
                                                                               GUILayout.Width(150));
                                }
                                GUILayout.FlexibleSpace();
                                GUILayout.EndHorizontal();
                            }
                        }

                        RPGMakerGUI.EndFoldout();
                    }


                    var defaultButtons = new List<string>() {"+Animation"};
                    if (Rm_RPGHandler.Instance.Items.AllowTwoHanded) defaultButtons.Add("+2H Animation");
                    if (Rm_RPGHandler.Instance.Items.EnableOffHandSlot && Rm_RPGHandler.Instance.Items.AllowDualWield) defaultButtons.Add("+DW Animation");
                    var wepAnimResult = RPGMakerGUI.FoldoutToolBar(ref wepFoldout, "Default Attack Animations",
                                                                   defaultButtons.ToArray());
                    if (wepFoldout)
                    {
                        var listOfAnim = _selectedClassInfo.LegacyAnimations.DefaultAttackAnimations;
                        var listOf2hAnim = _selectedClassInfo.LegacyAnimations.Default2HAttackAnimations;
                        var listOfDWAnim = _selectedClassInfo.LegacyAnimations.DefaultDWAttackAnimations;

                        GUILayout.BeginHorizontal("backgroundBox");
                        GUILayout.Label("ID", GUILayout.Width(100));
                        if (_selectedClassInfo.AnimationType == RPGAnimationType.Legacy)
                        {
                            GUILayout.Label("Animation", GUILayout.Width(150));
                            GUILayout.Label("Speed", GUILayout.Width(70));
                            GUILayout.Label("Impact Time", GUILayout.Width(70));
                            GUILayout.Label("Sound", GUILayout.Width(150));
                            GUILayout.Label("Reverse?", GUILayout.Width(100));
                        }
                        else
                        {
                            GUILayout.Label("Anim Number", GUILayout.Width(150));
                            GUILayout.Label("Sound", GUILayout.Width(150));
                        }
                        
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();

                        if (_selectedClassInfo.AnimationType == RPGAnimationType.Legacy && foundAnim)
                        {
                            for (int index = 0; index < listOfAnim.Count; index++)
                            {
                                var anim = listOfAnim[index];
                                GUILayout.BeginHorizontal();
                                GUILayout.Label(anim.Name, GUILayout.Width(100));

                                selectedAnim[index] = anims.FirstOrDefault(s => s == anim.Animation) != null
                                                          ? Array.IndexOf(anims, anim.Animation)
                                                          : 0;

                                selectedAnim[index] = EditorGUILayout.Popup(selectedAnim[index], anims,
                                                                            GUILayout.Width(120));
                                anim.Animation = anims[selectedAnim[index]];
                                if (anim.Animation == "None") anim.Animation = "";
                                anim.Speed = RPGMakerGUI.FloatField("", anim.Speed, GUILayout.Width(70));
                                anim.ImpactTime = RPGMakerGUI.FloatField("", anim.ImpactTime, GUILayout.Width(70));
                                anim.Sound = RPGMakerGUI.AudioClipSelector("", anim.Sound, ref anim.SoundPath,
                                                                           GUILayout.Width(150));
                                anim.Backwards = RPGMakerGUI.Toggle(anim.Backwards);
                                if (RPGMakerGUI.DeleteButton(15))
                                {
                                    listOfAnim.Remove(anim);
                                    index--;
                                }
                                GUILayout.FlexibleSpace();
                                GUILayout.EndHorizontal();
                            }

                            for (int index = 0; index < listOf2hAnim.Count; index++)
                            {
                                var anim = listOf2hAnim[index];
                                GUILayout.BeginHorizontal();
                                GUILayout.Label(anim.Name, GUILayout.Width(100));

                                selectedAnim[index] = anims.FirstOrDefault(s => s == anim.Animation) != null
                                                          ? Array.IndexOf(anims, anim.Animation)
                                                          : 0;

                                selectedAnim[index] = EditorGUILayout.Popup(selectedAnim[index], anims,
                                                                            GUILayout.Width(120));
                                anim.Animation = anims[selectedAnim[index]];
                                if (anim.Animation == "None") anim.Animation = "";
                                anim.Speed = RPGMakerGUI.FloatField("", anim.Speed, GUILayout.Width(70));
                                anim.ImpactTime = RPGMakerGUI.FloatField("", anim.ImpactTime, GUILayout.Width(70));
                                anim.Sound = RPGMakerGUI.AudioClipSelector("", anim.Sound, ref anim.SoundPath,
                                                                           GUILayout.Width(150));
                                anim.Backwards = RPGMakerGUI.Toggle(anim.Backwards);
                                if (RPGMakerGUI.DeleteButton(15))
                                {
                                    listOf2hAnim.Remove(anim);
                                    index--;
                                }
                                GUILayout.FlexibleSpace();
                                GUILayout.EndHorizontal();
                            }

                            for (int index = 0; index < listOfDWAnim.Count; index++)
                            {
                                var anim = listOfDWAnim[index];
                                GUILayout.BeginHorizontal();
                                GUILayout.Label(anim.Name, GUILayout.Width(100));

                                selectedAnim[index] = anims.FirstOrDefault(s => s == anim.Animation) != null
                                                          ? Array.IndexOf(anims, anim.Animation)
                                                          : 0;

                                selectedAnim[index] = EditorGUILayout.Popup(selectedAnim[index], anims,
                                                                            GUILayout.Width(120));
                                anim.Animation = anims[selectedAnim[index]];
                                if (anim.Animation == "None") anim.Animation = "";
                                anim.Speed = RPGMakerGUI.FloatField("", anim.Speed, GUILayout.Width(70));
                                anim.ImpactTime = RPGMakerGUI.FloatField("", anim.ImpactTime, GUILayout.Width(70));
                                anim.Sound = RPGMakerGUI.AudioClipSelector("", anim.Sound, ref anim.SoundPath,
                                                                           GUILayout.Width(150));
                                anim.Backwards = RPGMakerGUI.Toggle(anim.Backwards);
                                if (RPGMakerGUI.DeleteButton(15))
                                {
                                    listOfDWAnim.Remove(anim);
                                    index--;
                                }
                                GUILayout.FlexibleSpace();
                                GUILayout.EndHorizontal();
                            }
                        }
                        else
                        {
                            if (_selectedClassInfo.AnimationType == RPGAnimationType.Legacy)
                            {
                                EditorGUILayout.HelpBox(
                                    "Select a gameObject with the animation component and animations to get drop-downs for animation name instead of typing it in.",
                                    MessageType.Info);
                            }
                            else
                            {
                                EditorGUILayout.HelpBox(
                                    "For more information on mecanim visit https://www.youtube.com/watch?v=0cvwtk8Y_Uk.",
                                    MessageType.Info);
                            }

                            for (int index = 0; index < listOfAnim.Count; index++)
                            {
                                var anim = listOfAnim[index];
                                GUILayout.BeginHorizontal();
                                GUILayout.Label(anim.Name, GUILayout.Width(100));
                                if (_selectedClassInfo.AnimationType == RPGAnimationType.Legacy)
                                {
                                    anim.Animation = RPGMakerGUI.TextField("", anim.Animation, GUILayout.Width(150));
                                    anim.Speed = RPGMakerGUI.FloatField("", anim.Speed, GUILayout.Width(70));
                                    anim.ImpactTime = RPGMakerGUI.FloatField("", anim.ImpactTime, GUILayout.Width(70));
                                    anim.Sound = RPGMakerGUI.AudioClipSelector("", anim.Sound, ref anim.SoundPath,
                                                                               GUILayout.Width(150));
                                    anim.Backwards = RPGMakerGUI.Toggle(anim.Backwards);
                                }
                                else
                                {
                                    anim.MecanimAnimationNumber = RPGMakerGUI.IntField("", anim.MecanimAnimationNumber, GUILayout.Width(150));
                                    anim.Sound = RPGMakerGUI.AudioClipSelector("", anim.Sound, ref anim.SoundPath,
                                                                               GUILayout.Width(150));
                                }
                                
                                if (RPGMakerGUI.DeleteButton(15))
                                {
                                    listOfAnim.Remove(anim);
                                    index--;
                                }
                                GUILayout.FlexibleSpace();
                                GUILayout.EndHorizontal();
                            }
                            for (int index = 0; index < listOf2hAnim.Count; index++)
                            {
                                var anim = listOf2hAnim[index];
                                GUILayout.BeginHorizontal();
                                GUILayout.Label(anim.Name, GUILayout.Width(100));
                                if (_selectedClassInfo.AnimationType == RPGAnimationType.Legacy)
                                {
                                    anim.Animation = RPGMakerGUI.TextField("", anim.Animation, GUILayout.Width(150));
                                    anim.Speed = RPGMakerGUI.FloatField("", anim.Speed, GUILayout.Width(70));
                                    anim.ImpactTime = RPGMakerGUI.FloatField("", anim.ImpactTime, GUILayout.Width(70));
                                    anim.Sound = RPGMakerGUI.AudioClipSelector("", anim.Sound, ref anim.SoundPath,
                                                                               GUILayout.Width(150));
                                    anim.Backwards = RPGMakerGUI.Toggle(anim.Backwards);
                                }
                                else
                                {
                                    anim.MecanimAnimationNumber = RPGMakerGUI.IntField("", anim.MecanimAnimationNumber, GUILayout.Width(150));
                                    anim.Sound = RPGMakerGUI.AudioClipSelector("", anim.Sound, ref anim.SoundPath,
                                                                               GUILayout.Width(150));
                                }

                                if (RPGMakerGUI.DeleteButton(15))
                                {
                                    listOf2hAnim.Remove(anim);
                                    index--;
                                }
                                GUILayout.FlexibleSpace();
                                GUILayout.EndHorizontal();
                            }
                            for (int index = 0; index < listOfDWAnim.Count; index++)
                            {
                                var anim = listOfDWAnim[index];
                                GUILayout.BeginHorizontal();
                                GUILayout.Label(anim.Name, GUILayout.Width(100));
                                if (_selectedClassInfo.AnimationType == RPGAnimationType.Legacy)
                                {
                                    anim.Animation = RPGMakerGUI.TextField("", anim.Animation, GUILayout.Width(150));
                                    anim.Speed = RPGMakerGUI.FloatField("", anim.Speed, GUILayout.Width(70));
                                    anim.ImpactTime = RPGMakerGUI.FloatField("", anim.ImpactTime, GUILayout.Width(70));
                                    anim.Sound = RPGMakerGUI.AudioClipSelector("", anim.Sound, ref anim.SoundPath,
                                                                               GUILayout.Width(150));
                                    anim.Backwards = RPGMakerGUI.Toggle(anim.Backwards);
                                }
                                else
                                {
                                    anim.MecanimAnimationNumber = RPGMakerGUI.IntField("", anim.MecanimAnimationNumber, GUILayout.Width(150));
                                    anim.Sound = RPGMakerGUI.AudioClipSelector("", anim.Sound, ref anim.SoundPath,
                                                                               GUILayout.Width(150));
                                }
                                if (RPGMakerGUI.DeleteButton(15))
                                {
                                    listOfDWAnim.Remove(anim);
                                    index--;
                                }
                                GUILayout.FlexibleSpace();
                                GUILayout.EndHorizontal();
                            }
                        }

                        if (wepAnimResult == 0)
                        {
                            var count = 0;
                            while (listOfAnim.FirstOrDefault(l => l.Name == "Attack " + count) != null)
                            {
                                count++;
                            }
                            _selectedClassInfo.LegacyAnimations.DefaultAttackAnimations.Add(new AnimationDefinition()
                                                                                               {
                                                                                                   Name = "Attack " + count,
                                                                                                   RPGAnimationSet = RPGAnimationSet.DefaultAttack
                                                                                               });
                        }
                        else if (wepAnimResult == 1)
                        {
                            var count = 0;
                            while (listOf2hAnim.FirstOrDefault(l => l.Name == "2H Attack " + count) != null)
                            {
                                count++;
                            }
                            _selectedClassInfo.LegacyAnimations.Default2HAttackAnimations.Add(new AnimationDefinition()
                                                                                                 {
                                                                                                     Name = "2H Attack " + count,
                                                                                                     RPGAnimationSet = RPGAnimationSet.DefaultAttack
                                                                                                 });
                        }
                        else if (wepAnimResult == 2)
                        {
                            var count = 0;
                            while (listOfDWAnim.FirstOrDefault(l => l.Name == "DW Attack " + count) != null)
                            {
                                count++;
                            }
                            _selectedClassInfo.LegacyAnimations.DefaultDWAttackAnimations.Add(new AnimationDefinition()
                                                                                                 {
                                                                                                     Name = "DW Attack " + count,
                                                                                                     RPGAnimationSet = RPGAnimationSet.DefaultAttack
                                                                                                 });
                        }
                        RPGMakerGUI.EndFoldout();
                    }

                    #endregion

                    #region Attack Anims
                    var wepAnimList = _selectedClassInfo.LegacyAnimations.WeaponAnimations;

                    #region Check For Updates

                    foreach (var d in Rm_RPGHandler.Instance.Items.WeaponTypes)
                    {
                        var wepType = wepAnimList.FirstOrDefault(t => t.WeaponTypeID == d.ID);
                        if (wepType == null)
                        {
                            var wepTypeToAdd = new WeaponAnimationDefinition() {WeaponTypeID = d.ID};
                            wepAnimList.Add(wepTypeToAdd);
                        }
                    }

                    for (int index = 0; index < wepAnimList.Count; index++)
                    {
                        var v = wepAnimList[index];
                        var stillExists =
                            Rm_RPGHandler.Instance.Items.WeaponTypes.FirstOrDefault(t => t.ID == v.WeaponTypeID);

                        if (stillExists == null)
                        {
                            wepAnimList.Remove(v);
                            index--;
                        }
                    }

                    #endregion

                    for (int index = 0; index < wepAnimList.Count; index++)
                    {
                        wepAnimList = _selectedClassInfo.LegacyAnimations.WeaponAnimations;
                        var weaponAnimationDefinition = wepAnimList[index];
                        var wepTypeDef =
                            Rm_RPGHandler.Instance.Items.WeaponTypes.First(
                                w => w.ID == weaponAnimationDefinition.WeaponTypeID);
                        var wepType = wepTypeDef.Name;

                        var buttons = new List<string>() {"+Animation"};
                        if (wepTypeDef.AllowDualWield && Rm_RPGHandler.Instance.Items.AllowDualWield) buttons.Add("+DW Animation");

                        var wepTypeResult = RPGMakerGUI.FoldoutToolBar(ref advWepFoldout[index],
                                                                       "Custom " + wepType +
                                                                       " Animations [WepType" + index + "]" , buttons.ToArray());
                        if (advWepFoldout[index])
                        {
                            if (weaponAnimationDefinition.Animations.Count == 0 && weaponAnimationDefinition.DualWieldAnimations.Count == 0)
                            {
                                EditorGUILayout.HelpBox("Click one of the +Animation buttons to add custom animations for this weapon type.", MessageType.Info);
                            }

                            var listOfWepAnim = weaponAnimationDefinition.Animations;
                            var listOfWepAnimDW = weaponAnimationDefinition.DualWieldAnimations;

                            GUILayout.BeginHorizontal("backgroundBox");
                            if (_selectedClassInfo.AnimationType == RPGAnimationType.Legacy)
                            {
                                GUILayout.Label("ID", GUILayout.Width(100));
                                GUILayout.Label("Animation", GUILayout.Width(150));
                                GUILayout.Label("Speed", GUILayout.Width(70));
                                GUILayout.Label("Impact Time", GUILayout.Width(70));
                                GUILayout.Label("Sound", GUILayout.Width(150));
                                GUILayout.Label("Reverse?", GUILayout.Width(100));
                            }
                            else
                            {
                                GUILayout.Label("ID", GUILayout.Width(300));
                                GUILayout.Label("Anim Number", GUILayout.Width(150));
                                GUILayout.Label("Sound", GUILayout.Width(150));
                            }
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();

                            if (_selectedClassInfo.AnimationType == RPGAnimationType.Legacy && foundAnim)
                            {
                                for (int Windex = 0; Windex < listOfWepAnim.Count; Windex++)
                                {
                                    var anim = listOfWepAnim[Windex];
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label(anim.Name, GUILayout.Width(100));

                                    selectedAnim[Windex] = anims.FirstOrDefault(s => s == anim.Animation) != null
                                                               ? Array.IndexOf(anims, anim.Animation)
                                                               : 0;

                                    selectedAnim[Windex] = EditorGUILayout.Popup(selectedAnim[Windex], anims,
                                                                                 GUILayout.Width(120));
                                    anim.Animation = anims[selectedAnim[Windex]];
                                    if (anim.Animation == "None") anim.Animation = "";
                                    anim.Speed = RPGMakerGUI.FloatField("", anim.Speed, GUILayout.Width(70));
                                    anim.ImpactTime = RPGMakerGUI.FloatField("", anim.ImpactTime, GUILayout.Width(70));
                                    anim.Sound = RPGMakerGUI.AudioClipSelector("", anim.Sound, ref anim.SoundPath,
                                                                               GUILayout.Width(150));
                                    anim.Backwards = RPGMakerGUI.Toggle(anim.Backwards);
                                    if (RPGMakerGUI.DeleteButton(15))
                                    {
                                        listOfWepAnim.Remove(anim);
                                        index--;
                                    }
                                    GUILayout.FlexibleSpace();
                                    GUILayout.EndHorizontal();
                                }

                                for (int Dindex = 0; Dindex < listOfWepAnimDW.Count; Dindex++)
                                {
                                    var anim = listOfWepAnimDW[Dindex];
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label(anim.Name, GUILayout.Width(100));

                                    selectedAnim[Dindex] = anims.FirstOrDefault(s => s == anim.Animation) != null
                                                               ? Array.IndexOf(anims, anim.Animation)
                                                               : 0;

                                    selectedAnim[Dindex] = EditorGUILayout.Popup(selectedAnim[Dindex], anims,
                                                                                 GUILayout.Width(120));
                                    anim.Animation = anims[selectedAnim[Dindex]];
                                    if (anim.Animation == "None") anim.Animation = "";
                                    anim.Speed = RPGMakerGUI.FloatField("", anim.Speed, GUILayout.Width(70));
                                    anim.ImpactTime = RPGMakerGUI.FloatField("", anim.ImpactTime, GUILayout.Width(70));
                                    anim.Sound = RPGMakerGUI.AudioClipSelector("", anim.Sound, ref anim.SoundPath,
                                                                               GUILayout.Width(150));
                                    anim.Backwards = RPGMakerGUI.Toggle(anim.Backwards);
                                    if (RPGMakerGUI.DeleteButton(15))
                                    {
                                        listOfWepAnimDW.Remove(anim);
                                        Dindex--;
                                    }
                                    GUILayout.FlexibleSpace();
                                    GUILayout.EndHorizontal();
                                }
                            }
                            else
                            {
                                for (int i = 0; i < listOfWepAnim.Count; i++)
                                {
                                    var anim = listOfWepAnim[i];
                                    GUILayout.BeginHorizontal();
                                    if (_selectedClassInfo.AnimationType == RPGAnimationType.Legacy)
                                    {
                                        GUILayout.Label(anim.Name, GUILayout.Width(100));
                                        anim.Animation = RPGMakerGUI.TextField("", anim.Animation, GUILayout.Width(150));
                                        anim.Speed = RPGMakerGUI.FloatField("", anim.Speed, GUILayout.Width(70));
                                        anim.ImpactTime = RPGMakerGUI.FloatField("", anim.ImpactTime, GUILayout.Width(70));
                                        anim.Sound = RPGMakerGUI.AudioClipSelector("", anim.Sound, ref anim.SoundPath,
                                                                                   GUILayout.Width(150));
                                        anim.Backwards = RPGMakerGUI.Toggle(anim.Backwards);
                                    }
                                    else
                                    {
                                        GUILayout.Label(anim.Name, GUILayout.Width(300));
                                        anim.MecanimAnimationNumber = RPGMakerGUI.IntField("", anim.MecanimAnimationNumber, GUILayout.Width(150));
                                        anim.Sound = RPGMakerGUI.AudioClipSelector("", anim.Sound, ref anim.SoundPath, GUILayout.Width(150));
                                    }

                                    if (RPGMakerGUI.DeleteButton(15))
                                    {
                                        listOfWepAnim.Remove(anim);
                                        i--;
                                    }
                                    GUILayout.FlexibleSpace();
                                    GUILayout.EndHorizontal();
                                }
                                for (int i = 0; i < listOfWepAnimDW.Count; i++)
                                {
                                    var anim = listOfWepAnimDW[i];
                                    GUILayout.BeginHorizontal();
                                    if (_selectedClassInfo.AnimationType == RPGAnimationType.Legacy)
                                    {
                                        GUILayout.Label(anim.Name, GUILayout.Width(100));
                                        anim.Animation = RPGMakerGUI.TextField("", anim.Animation, GUILayout.Width(150));
                                        anim.Speed = RPGMakerGUI.FloatField("", anim.Speed, GUILayout.Width(70));
                                        anim.ImpactTime = RPGMakerGUI.FloatField("", anim.ImpactTime, GUILayout.Width(70));
                                        anim.Sound = RPGMakerGUI.AudioClipSelector("", anim.Sound, ref anim.SoundPath,
                                                                                   GUILayout.Width(150));
                                        anim.Backwards = RPGMakerGUI.Toggle(anim.Backwards);
                                    }
                                    else
                                    {
                                        GUILayout.Label(anim.Name, GUILayout.Width(300));
                                        anim.MecanimAnimationNumber = RPGMakerGUI.IntField("", anim.MecanimAnimationNumber, GUILayout.Width(150));
                                        anim.Sound = RPGMakerGUI.AudioClipSelector("", anim.Sound, ref anim.SoundPath, GUILayout.Width(150));
                                    }
                                    if (RPGMakerGUI.DeleteButton(15))
                                    {
                                        listOfWepAnimDW.Remove(anim);
                                        i--;
                                    }
                                    GUILayout.FlexibleSpace();
                                    GUILayout.EndHorizontal();
                                }
                            }


                            if (wepTypeResult == 0)
                            {
                                var count = 0;
                                while (listOfWepAnim.FirstOrDefault(l => l.Name == "Attack " + count) != null)
                                {
                                    count++;
                                }
                                listOfWepAnim.Add(new AnimationDefinition()
                                                      {
                                                          Name = "Attack " + count + " for [WepType" + index + "]",
                                                          RPGAnimationSet = RPGAnimationSet.WeaponTypeAttack,
                                                          WeaponTypeIndex = index 
                                                      });
                            }
                            else if (wepTypeResult == 1)
                            {
                                var count = 0;
                                while (listOfWepAnimDW.FirstOrDefault(l => l.Name == "DW Attack " + count) != null)
                                {
                                    count++;
                                }
                                listOfWepAnimDW.Add(new AnimationDefinition()
                                                        {
                                                            Name = "DW Attack " + count + " for [WepType" + index + "]",
                                                            RPGAnimationSet = RPGAnimationSet.WeaponTypeAttack,
                                                            WeaponTypeIndex = index 
                                                        });
                            }

                            RPGMakerGUI.EndFoldout();
                        }
                    }
                    #endregion

                    #endregion

                //}
                #endregion

                #region Character Customisation

                var addVisual = RPGMakerGUI.FoldoutToolBar(ref startVisualFoldout, "Visual Character Customisation",
                                                         "+VisualCustomisation");
                if (startVisualFoldout)
                {
                    if (_selectedClassInfo.VisualCustomisations.Count > 0)
                    {
                        for (int index = 0; index < _selectedClassInfo.VisualCustomisations.Count; index++)
                        {
                            var visual = _selectedClassInfo.VisualCustomisations[index];
                            GUILayout.BeginVertical("backgroundBox");

                            /*
                            public string Id;
                            public string Identifier;
                            public VisualCustomisationType CustomisationType;
                            public string StringRef;*/

                            var oldCust = visual.CustomisationType;
                            visual.CustomisationType =
                                (VisualCustomisationType)
                                RPGMakerGUI.EnumPopup("Customisation Type:", visual.CustomisationType);
                            var identifierText = visual.CustomisationType == VisualCustomisationType.Category
                                                     ? "Category Name:"
                                                     : "Identifier:";
                            visual.Identifier = RPGMakerGUI.TextField(identifierText, visual.Identifier, 0);

                            if(oldCust != visual.CustomisationType)
                            {
                                //Reset some values
                                visual.StringRef = "";
                                visual.StringRefTwo = "";
                            }

                            //Show different display options based on customisation type
                            if (visual.CustomisationType != VisualCustomisationType.Category)
                            {
                                var enumOptions = ((VisualDisplayType[])Enum.GetValues(typeof (VisualDisplayType))).ToList();

                                switch(visual.CustomisationType)
                                {
                                    case VisualCustomisationType.BlendShape:
                                        enumOptions.Remove(VisualDisplayType.Color);
                                        enumOptions.Remove(VisualDisplayType.ImageOptions);
                                        enumOptions.Remove(VisualDisplayType.TextList);
                                        enumOptions.Remove(VisualDisplayType.TextOptions);
                                        break;
                                    case VisualCustomisationType.MaterialColor:
                                        enumOptions.Remove(VisualDisplayType.Slider);
                                        enumOptions.Remove(VisualDisplayType.ImageOptions);
                                        enumOptions.Remove(VisualDisplayType.TextList);
                                        enumOptions.Remove(VisualDisplayType.TextOptions);
                                        break;
                                    case VisualCustomisationType.Scale:
                                        enumOptions.Remove(VisualDisplayType.Color);
                                        enumOptions.Remove(VisualDisplayType.ImageOptions);
                                        enumOptions.Remove(VisualDisplayType.TextList);
                                        enumOptions.Remove(VisualDisplayType.TextOptions);
                                        break;
                                    case VisualCustomisationType.GameObject:
                                        enumOptions.Remove(VisualDisplayType.Color);
                                        enumOptions.Remove(VisualDisplayType.Slider);
                                        break;
                                    case VisualCustomisationType.MaterialChange:
                                        enumOptions.Remove(VisualDisplayType.Color);
                                        enumOptions.Remove(VisualDisplayType.Slider);
                                        break;
                                    case VisualCustomisationType.Category:
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                                selectedVisualCustDisplay = Array.IndexOf(enumOptions.ToArray(), visual.DisplayType);
                                if(selectedVisualCustDisplay == -1)
                                {
                                    selectedVisualCustDisplay = 0;
                                }
                                selectedVisualCustDisplay = RPGMakerGUI.Popup("Display Type:", selectedVisualCustDisplay, enumOptions.Select(e => e.ToString()).ToArray(), 0);
                                visual.DisplayType = enumOptions[selectedVisualCustDisplay];
                            }

                            
                            if (visual.CustomisationType == VisualCustomisationType.BlendShape)
                            {
                                #region "BlendShape"
                                visual.TargetedGameObjectName = RPGMakerGUI.TextField("Target GameObject:",
                                                                                      visual.TargetedGameObjectName, 0);
                                visual.StringRef = RPGMakerGUI.TextField("Blend Shape Name:", visual.StringRef, 0);
                                visual.MinFloatValue = RPGMakerGUI.FloatField("Min Value:", visual.MinFloatValue, 0);
                                visual.MaxFloatValue = RPGMakerGUI.FloatField("Max Value:", visual.MaxFloatValue, 0);
                                visual.StartingValueFloat = RPGMakerGUI.FloatField("Starting Value:", visual.StartingValueFloat, 0);


                                #endregion
                            }
                            else if (visual.CustomisationType == VisualCustomisationType.MaterialColor)
                            {
                                #region "MatColor"
                                visual.TargetedGameObjectName = RPGMakerGUI.TextField("Target GameObject:",
                                                                                      visual.TargetedGameObjectName, 0);
                                visual.StringRef = RPGMakerGUI.TextField("Material Name:", visual.StringRef, 0);
                                if (visual.StringRefTwo == "")
                                {
                                    visual.StringRefTwo = "_Color"; //Set Default Material Color Name
                                }
                                visual.StringRefTwo = RPGMakerGUI.TextField("Color Name:", visual.StringRefTwo, 0);
                                EditorGUILayout.HelpBox("The color name for the Unity Standard Material Shader is \"_Color\" .", MessageType.Info);
                                visual.StartingValueInt = RPGMakerGUI.IntField("Starting Color:", visual.StartingValueInt, 0);

                                var addColor = RPGMakerGUI.FoldoutToolBar(ref startColorFoldout, "Colors",
                                                                          "+Color");

                                if (startColorFoldout)
                                {
                                    for (int i = 0; i < visual.ColorOptions.Count; i++)
                                    {
                                        var c = visual.ColorOptions[i];
                                        GUILayout.BeginHorizontal();
                                        var color = new Color(c.r, c.g, c.b, c.a);
                                        color = EditorGUILayout.ColorField(color);
                                        c.SetFromColor(color);

                                        if (GUILayout.Button("Delete", "genericButton"))
                                        {
                                            visual.ColorOptions.RemoveAt(i);
                                            return;
                                        }

                                        GUILayout.EndHorizontal();
                                    }
                                    RPGMakerGUI.EndFoldout();
                                }

                                if (addColor == 0)
                                {
                                    visual.ColorOptions.Add(new RPG_Color());
                                }

                                if (visual.ColorOptions.Count == 0)
                                {
                                    EditorGUILayout.HelpBox(
                                        "Click +Color to add a new color option for this customisation.",
                                        MessageType.Info);
                                }

                                #endregion
                            }
                            else if (visual.CustomisationType == VisualCustomisationType.Scale)
                            {
                                #region "Scale"
                                visual.TargetedGameObjectName = RPGMakerGUI.TextField("Target GameObject:",
                                                                                      visual.TargetedGameObjectName, 0);
                                RPGMakerGUI.Toggle("Scale X:", ref visual.ScaleX);
                                RPGMakerGUI.Toggle("Scale Y:", ref visual.ScaleY);
                                RPGMakerGUI.Toggle("Scale Z:", ref visual.ScaleZ);
                                visual.MinFloatValue = RPGMakerGUI.FloatField("Min Scale:", visual.MinFloatValue, 0);
                                visual.MaxFloatValue = RPGMakerGUI.FloatField("Max Scale:", visual.MaxFloatValue, 0);
                                visual.StartingValueFloat = RPGMakerGUI.FloatField("Starting Scale:", visual.StartingValueFloat, 0);

                                var childResult = RPGMakerGUI.FoldoutToolBar(ref showScaleChildOptions, "Other Objects To Scale", "+GameObject");
                                if (showScaleChildOptions)
                                {
                                    for (int i = 0; i < visual.ChildCustomisations.Count; i++)
                                    {
                                        var childVisual = visual.ChildCustomisations[i];
                                        GUILayout.BeginVertical();
                                        childVisual.TargetedGameObjectName = RPGMakerGUI.TextField("Target GameObject:", childVisual.TargetedGameObjectName, 0);
                                        RPGMakerGUI.Toggle("Scale X:", ref childVisual.ScaleX);
                                        RPGMakerGUI.Toggle("Scale Y:", ref childVisual.ScaleY);
                                        RPGMakerGUI.Toggle("Scale Z:", ref childVisual.ScaleZ);
                                        childVisual.MinFloatValue = RPGMakerGUI.FloatField("Min Scale:", childVisual.MinFloatValue, 0);
                                        childVisual.MaxFloatValue = RPGMakerGUI.FloatField("Max Scale:", childVisual.MaxFloatValue, 0);

                                        if (GUILayout.Button("Delete", "genericButton"))
                                        {
                                            visual.ChildCustomisations.RemoveAt(i);
                                            return;
                                        }
                                        GUILayout.EndVertical();
                                    }

                                    if (childResult == 0)
                                    {
                                        visual.ChildCustomisations.Add(new VisualCustomisation());
                                    }

                                    if (visual.ChildCustomisations.Count == 0)
                                    {
                                        EditorGUILayout.HelpBox(
                                            "Click +GameObject to add a new gameobject to scale with this.",
                                            MessageType.Info);
                                    }

                                    RPGMakerGUI.EndFoldout();
                                }
                                #endregion
                            }
                            else if (visual.CustomisationType == VisualCustomisationType.GameObject)
                            {
                                #region "GameObject"
                                visual.StartingValueInt = RPGMakerGUI.IntField("Starting GameObject:", visual.StartingValueInt, 0);

                                var gameObjectResult = RPGMakerGUI.FoldoutToolBar(ref showTargetedGameObjects, "Target GameObject Names", "+GameObject");
                                if (showTargetedGameObjects)
                                {
                                    while (visual.LabelOptions.Count < visual.TargetedGameObjectNames.Count)
                                    {
                                        visual.LabelOptions.Add("");
                                    }

                                    while (visual.ImageOptions.Count < visual.TargetedGameObjectNames.Count)
                                    {
                                        visual.ImageOptions.Add(new ImageContainer());
                                    }

                                    for (int i = 0; i < visual.TargetedGameObjectNames.Count; i++)
                                    {
                                        GUILayout.BeginHorizontal();

                                        if(visual.DisplayType == VisualDisplayType.ImageOptions)
                                        {
                                            visual.ImageOptions[i].Image = RPGMakerGUI.ImageSelector("Image:", visual.ImageOptions[i].Image, ref visual.ImageOptions[i].ImagePath, true);
                                        }

                                        visual.TargetedGameObjectNames[i] = RPGMakerGUI.TextField("GameObject Name:", visual.TargetedGameObjectNames[i]);

                                        if (visual.DisplayType == VisualDisplayType.TextOptions || visual.DisplayType == VisualDisplayType.TextList)
                                        {
                                            visual.LabelOptions[i] = RPGMakerGUI.TextField("Label Name", visual.LabelOptions[i]);
                                        } 

                                        if(GUILayout.Button("Delete", "genericButton"))
                                        {
                                            visual.TargetedGameObjectNames.RemoveAt(i);
                                            return;
                                        }
                                        GUILayout.EndHorizontal();
                                    }
                                    if (gameObjectResult == 0)
                                    {
                                        visual.TargetedGameObjectNames.Add("");
                                    }

                                    if (visual.TargetedGameObjectNames.Count == 0)
                                    {
                                        EditorGUILayout.HelpBox(
                                            "Click +GameObject to add a new gameobject option for this customisation.",
                                            MessageType.Info);
                                    }

                                    RPGMakerGUI.EndFoldout();
                                }

                                #endregion
                            }
                            else if (visual.CustomisationType == VisualCustomisationType.MaterialChange)
                            {
                                #region "Material Change"

                                visual.TargetedGameObjectName = RPGMakerGUI.TextField("Target GameObject:",
                                                      visual.TargetedGameObjectName, 0);
                                visual.StartingValueInt = RPGMakerGUI.IntField("Starting Option:", visual.StartingValueInt, 0);

                                var materialResult = RPGMakerGUI.FoldoutToolBar(ref showMaterialOptions, "Material Options", "+Material Option");
                                if (showMaterialOptions)
                                {
                                    while (visual.LabelOptions.Count < visual.MaterialPaths.Count)
                                    {
                                        visual.LabelOptions.Add("");
                                    }

                                    while (visual.ImageOptions.Count < visual.MaterialPaths.Count)
                                    {
                                        visual.ImageOptions.Add(new ImageContainer());
                                    }

                                    for (int i = 0; i < visual.MaterialPaths.Count; i++)
                                    {
                                        GUILayout.BeginHorizontal();

                                        var matName = visual.MaterialPaths[i];
                                        mat = RPGMakerGUI.MaterialSelector("Material:", mat, ref matName);
                                        visual.MaterialPaths[i] = matName;


                                        if (visual.DisplayType == VisualDisplayType.ImageOptions)
                                        {
                                            visual.ImageOptions[i].Image = RPGMakerGUI.ImageSelector("Image:", visual.ImageOptions[i].Image, ref visual.ImageOptions[i].ImagePath, true);
                                        }

                                        if (visual.DisplayType == VisualDisplayType.TextList || visual.DisplayType == VisualDisplayType.TextOptions)
                                        {
                                            visual.LabelOptions[i] = RPGMakerGUI.TextField("Label Name", visual.LabelOptions[i]);
                                        }

                                        if(GUILayout.Button("Delete", "genericButton"))
                                        {
                                            visual.MaterialPaths.RemoveAt(i);
                                            return;
                                        }
                                        GUILayout.EndHorizontal();
                                    }
                                    if (materialResult == 0)
                                    {
                                        visual.MaterialPaths.Add("");
                                    }

                                    if (visual.MaterialPaths.Count == 0)
                                    {
                                        EditorGUILayout.HelpBox(
                                            "Click +Material Option to add a new material option for this customisation.",
                                            MessageType.Info);
                                    }

                                    RPGMakerGUI.EndFoldout();
                                }

                                #endregion
                            }

                            GUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();

                            if (index > 0 && GUILayout.Button("Move Up", "genericButton"))
                            {
                                GUI.FocusControl("");
                                var curCondition = _selectedClassInfo.VisualCustomisations[index];
                                var prevCondition = _selectedClassInfo.VisualCustomisations[index - 1];

                                _selectedClassInfo.VisualCustomisations[index - 1] = curCondition;
                                _selectedClassInfo.VisualCustomisations[index] = prevCondition;

                                return;
                            }

                            if (index < _selectedClassInfo.VisualCustomisations.Count - 1 && GUILayout.Button("Move Down", "genericButton"))
                            {
                                GUI.FocusControl("");
                                var curCondition = _selectedClassInfo.VisualCustomisations[index];
                                var nextCondition = _selectedClassInfo.VisualCustomisations[index + 1];

                                _selectedClassInfo.VisualCustomisations[index + 1] = curCondition;
                                _selectedClassInfo.VisualCustomisations[index] = nextCondition; 

                                return;
                            }

                            if (GUILayout.Button("Delete", "genericButton"))
                            {
                                GUI.FocusControl("");
                                _selectedClassInfo.VisualCustomisations.Remove(visual);
                                return;
                            }
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();

                            GUILayout.EndVertical();
                            GUILayout.Space(5);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Click +VisualCustomisation to add a new customisation for character creation.", MessageType.Info);
                    }

                    if (addVisual == 0)
                    {
                        _selectedClassInfo.VisualCustomisations.Add(new VisualCustomisation());
                    }

                    RPGMakerGUI.EndFoldout();
                }

                #endregion

                #region Starting Attributes

                
                if (RPGMakerGUI.Foldout(ref startAttrFoldout, "Starting Attributes"))
                {
                    if (Rm_RPGHandler.Instance.ASVT.AttributesDefinitions.Count > 0)
                    {
                        foreach (var d in Rm_RPGHandler.Instance.ASVT.AttributesDefinitions)
                        {
                            var attr = _selectedClassInfo.StartingAttributes.FirstOrDefault(a => a.AsvtID == d.ID);
                            if (attr == null)
                            {
                                _selectedClassInfo.StartingAttributes.Add(new Rm_AsvtAmount() {AsvtID = d.ID,Amount = d.DefaultValue});
                            }
                        }

                        for (int index = 0; index < _selectedClassInfo.StartingAttributes.Count; index++)
                        {
                            var v = _selectedClassInfo.StartingAttributes[index];
                            var stillExists =
                                Rm_RPGHandler.Instance.ASVT.AttributesDefinitions.FirstOrDefault(
                                    a => a.ID == v.AsvtID);

                            if (stillExists == null)
                            {
                                _selectedClassInfo.StartingAttributes.Remove(v);
                                index--;
                            }
                        }

                        foreach (var v in _selectedClassInfo.StartingAttributes)
                        {
                            var prefix =
                                Rm_RPGHandler.Instance.ASVT.AttributesDefinitions.First(a => a.ID == v.AsvtID).Name;
                            v.Amount = RPGMakerGUI.IntField(prefix, v.Amount);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No defined attributes.", MessageType.Info);
                    }
                    RPGMakerGUI.EndFoldout();
                }

                #endregion

                #region Starting Statistics

                if (RPGMakerGUI.Foldout(ref startStatFoldout, "Starting Statistics"))
                {
                    if (Rm_RPGHandler.Instance.ASVT.StatisticDefinitions.Count > 0)
                    {
                        foreach (var d in Rm_RPGHandler.Instance.ASVT.StatisticDefinitions)
                        {
                            var attr = _selectedClassInfo.StartingStats.FirstOrDefault(a => a.AsvtID == d.ID);
                            if (attr == null)
                            {
                                _selectedClassInfo.StartingStats.Add(new Rm_AsvtAmountFloat() { AsvtID = d.ID, Amount = d.DefaultValue });
                            }
                        }

                        for (int index = 0; index < _selectedClassInfo.StartingStats.Count; index++)
                        {
                            var v = _selectedClassInfo.StartingStats[index];
                            var stillExists =
                                Rm_RPGHandler.Instance.ASVT.StatisticDefinitions.FirstOrDefault(
                                    a => a.ID == v.AsvtID);

                            if (stillExists == null)
                            {
                                _selectedClassInfo.StartingStats.Remove(v);
                                index--;
                            }
                        }

                        foreach (var v in _selectedClassInfo.StartingStats)
                        {
                            var prefix =
                                Rm_RPGHandler.Instance.ASVT.StatisticDefinitions.First(a => a.ID == v.AsvtID).Name;
                            v.Amount = RPGMakerGUI.FloatField(prefix, v.Amount);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No defined statistics.", MessageType.Info);
                    }
                    RPGMakerGUI.EndFoldout();
                }

                #endregion

                #region Starting Vitals

                if (RPGMakerGUI.Foldout(ref startVitalFoldout, "Starting Vitals"))
                {
                    if (Rm_RPGHandler.Instance.ASVT.VitalDefinitions.Count > 0)
                    {
                        foreach (var d in Rm_RPGHandler.Instance.ASVT.VitalDefinitions)
                        {
                            var attr = _selectedClassInfo.StartingVitals.FirstOrDefault(a => a.AsvtID == d.ID);
                            if (attr == null)
                            {
                                _selectedClassInfo.StartingVitals.Add(new Rm_AsvtAmount() { AsvtID = d.ID, Amount = d.DefaultValue });
                            }
                        }

                        for (int index = 0; index < _selectedClassInfo.StartingVitals.Count; index++)
                        {
                            var v = _selectedClassInfo.StartingVitals[index];
                            var stillExists =
                                Rm_RPGHandler.Instance.ASVT.VitalDefinitions.FirstOrDefault(
                                    a => a.ID == v.AsvtID);

                            if (stillExists == null)
                            {
                                _selectedClassInfo.StartingVitals.Remove(v);
                                index--;
                            }
                        }

                        foreach (var v in _selectedClassInfo.StartingVitals)
                        {
                            var prefix =
                                Rm_RPGHandler.Instance.ASVT.VitalDefinitions.First(a => a.ID == v.AsvtID).Name;
                            v.Amount = RPGMakerGUI.IntField(prefix, v.Amount);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No defined vitals.", MessageType.Info);
                    }
                    RPGMakerGUI.EndFoldout();
                }

                #endregion

                #region Starting Traits

                if (RPGMakerGUI.Foldout(ref startTraitFoldout, "Starting Traits"))
                {
                    if (Rm_RPGHandler.Instance.ASVT.TraitDefinitions.Count > 0)
                    {
                        foreach (var d in Rm_RPGHandler.Instance.ASVT.TraitDefinitions)
                        {
                            var attr = _selectedClassInfo.StartingTraitLevels.FirstOrDefault(a => a.AsvtID == d.ID);
                            if (attr == null)
                            {
                                _selectedClassInfo.StartingTraitLevels.Add(new Rm_AsvtAmount() { AsvtID = d.ID, Amount = d.StartingLevel });
                            }
                        }

                        for (int index = 0; index < _selectedClassInfo.StartingTraitLevels.Count; index++)
                        {
                            var v = _selectedClassInfo.StartingTraitLevels[index];
                            var stillExists =
                                Rm_RPGHandler.Instance.ASVT.TraitDefinitions.FirstOrDefault(
                                    a => a.ID == v.AsvtID);

                            if (stillExists == null)
                            {
                                _selectedClassInfo.StartingTraitLevels.Remove(v);
                                index--;
                            }
                        }

                        foreach (var v in _selectedClassInfo.StartingTraitLevels)
                        {
                            var prefix =
                                Rm_RPGHandler.Instance.ASVT.TraitDefinitions.First(a => a.ID == v.AsvtID).Name;
                            v.Amount = RPGMakerGUI.IntField(prefix, v.Amount);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No defined traits.", MessageType.Info);
                    }
                    RPGMakerGUI.EndFoldout();
                }

                #endregion

                if (!Rm_RPGHandler.Instance.Player.ManualAssignAttrPerLevel)
                {
                    #region Attributes Per Level
                    if (RPGMakerGUI.Foldout(ref attrPerLevelFoldout, "Attributes Per Level"))
                    {
                        if (Rm_RPGHandler.Instance.ASVT.AttributesDefinitions.Count > 0)
                        {
                            foreach (var d in Rm_RPGHandler.Instance.ASVT.AttributesDefinitions)
                            {
                                var attr =
                                    _selectedClassInfo.AttributePerLevel.FirstOrDefault(a => a.AsvtID == d.ID);
                                if (attr == null)
                                {
                                    _selectedClassInfo.AttributePerLevel.Add(new Rm_AsvtAmount() {AsvtID = d.ID});
                                }
                            }

                            for (int index = 0; index < _selectedClassInfo.AttributePerLevel.Count; index++)
                            {
                                var v = _selectedClassInfo.AttributePerLevel[index];
                                var stillExists =
                                    Rm_RPGHandler.Instance.ASVT.AttributesDefinitions.FirstOrDefault(
                                        a => a.ID == v.AsvtID);

                                if (stillExists == null)
                                {
                                    _selectedClassInfo.AttributePerLevel.Remove(v);
                                    index--;
                                }
                            }

                            foreach (var v in _selectedClassInfo.AttributePerLevel)
                            {
                                var prefix =
                                    Rm_RPGHandler.Instance.ASVT.AttributesDefinitions.First(a => a.ID == v.AsvtID).
                                        Name;
                                v.Amount = RPGMakerGUI.IntField(prefix, v.Amount);
                            }
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("No defined attributes.", MessageType.Info);
                        }

                    }
                    if (attrPerLevelFoldout) RPGMakerGUI.EndFoldout();

                    #endregion
                }


                #region SkillImmunities
                var immunityResult = RPGMakerGUI.FoldoutToolBar(ref skillImmunFoldout, "Skill Immunities", new string[] { "+Immunity" });
                if (skillImmunFoldout)
                {
                    for (int index = 0; index < _selectedClassInfo.SkillMetaImmunitiesID.Count; index++)
                    {
                        var v = _selectedClassInfo.SkillMetaImmunitiesID[index];
                        if (string.IsNullOrEmpty(v)) continue;

                        var stillExists =
                            Rm_RPGHandler.Instance.Combat.SkillMeta.FirstOrDefault(
                                a => a.ID == v);

                        if (stillExists == null)
                        {
                            _selectedClassInfo.SkillMetaImmunitiesID.Remove(v);
                            index--;
                        }
                    }
                    var allSkillMetas = Rm_RPGHandler.Instance.Combat.SkillMeta;
                    if (allSkillMetas.Count > 0)
                    {
                        if (_selectedClassInfo.SkillMetaImmunitiesID.Count == 0)
                        {
                            EditorGUILayout.HelpBox("Click +Immunity to add a skill meta this class is immune to.", MessageType.Info);
                        }

                        GUILayout.Space(5);
                        for (int index = 0; index < _selectedClassInfo.SkillMetaImmunitiesID.Count; index++)
                        {
                            GUILayout.BeginHorizontal();
                            if (string.IsNullOrEmpty(_selectedClassInfo.SkillMetaImmunitiesID[index]))
                            {
                                selectedSkillImmunity[index] = 0;
                            }
                            else
                            {
                                var stillExists =
                                    allSkillMetas.FirstOrDefault(a => a.ID == _selectedClassInfo.SkillMetaImmunitiesID[index]);
                                selectedSkillImmunity[index] = stillExists != null ? allSkillMetas.IndexOf(stillExists) : 0;
                            }

                            selectedSkillImmunity[index] = EditorGUILayout.Popup("Immunity:", selectedSkillImmunity[index],
                                                                            allSkillMetas.Select((q, indexOf) => indexOf + ". " + q.Name).
                                                                                ToArray());
                            _selectedClassInfo.SkillMetaImmunitiesID[index] = allSkillMetas[selectedSkillImmunity[index]].ID;

                            if (GUILayout.Button(RPGMakerGUI.DelIcon, "genericButton", GUILayout.Width(30), GUILayout.Height(30)))
                            {
                                _selectedClassInfo.SkillMetaImmunitiesID.Remove(_selectedClassInfo.SkillMetaImmunitiesID[index]);
                                index--;
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.Space(5);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No Skill Metas Found.", MessageType.Info);
                        _selectedClassInfo.SkillMetaImmunitiesID = new List<string>();
                    }

                    if (immunityResult == 0)
                    {
                        _selectedClassInfo.SkillMetaImmunitiesID.Add("");
                    }
                }
                if (skillImmunFoldout) RPGMakerGUI.EndFoldout();
                #endregion

                #region SkillSusceptibility
                var susceptResult = RPGMakerGUI.FoldoutToolBar(ref skillSusFoldout, "Skill Susceptibilities", new string[] { "+Susceptibility" });
                if (skillSusFoldout)
                {
                    for (int index = 0; index < _selectedClassInfo.SkillMetaSusceptibilities.Count; index++)
                    {
                        var v = _selectedClassInfo.SkillMetaSusceptibilities[index];
                        if (string.IsNullOrEmpty(v.ID)) continue;

                        var stillExists =
                            Rm_RPGHandler.Instance.Combat.SkillMeta.FirstOrDefault(
                                a => a.ID == v.ID);

                        if (stillExists == null)
                        {
                            _selectedClassInfo.SkillMetaSusceptibilities.Remove(v);
                            index--;
                        }
                    }
                    var allSkillMetas = Rm_RPGHandler.Instance.Combat.SkillMeta;
                    if (allSkillMetas.Count > 0)
                    {
                        if (_selectedClassInfo.SkillMetaSusceptibilities.Count == 0)
                        {
                            EditorGUILayout.HelpBox("Click +Susceptibility to add a skill meta this class takes more damage from.", MessageType.Info);
                        }

                        GUILayout.Space(5);
                        for (int index = 0; index < _selectedClassInfo.SkillMetaSusceptibilities.Count; index++)
                        {
                            GUILayout.BeginHorizontal();
                            if (string.IsNullOrEmpty(_selectedClassInfo.SkillMetaSusceptibilities[index].ID))
                            {
                                selectedSkillSus[index] = 0;
                            }
                            else
                            {
                                var stillExists =
                                    allSkillMetas.FirstOrDefault(a => a.ID == _selectedClassInfo.SkillMetaSusceptibilities[index].ID);
                                selectedSkillSus[index] = stillExists != null ? allSkillMetas.IndexOf(stillExists) : 0;
                            }
                            GUILayout.BeginHorizontal();
                            selectedSkillSus[index] = EditorGUILayout.Popup("Susceptibility:", selectedSkillSus[index],
                                                                            allSkillMetas.Select((q, indexOf) => indexOf + ". " + q.Name).
                                                                                ToArray());
                            var selectedSus = _selectedClassInfo.SkillMetaSusceptibilities[index];
                            selectedSus.AdditionalDamage = RPGMakerGUI.FloatField("Additional Damage:", selectedSus.AdditionalDamage);
                            GUILayout.EndHorizontal();
                            _selectedClassInfo.SkillMetaSusceptibilities[index].ID = allSkillMetas[selectedSkillSus[index]].ID;

                            if (GUILayout.Button(RPGMakerGUI.DelIcon, "genericButton", GUILayout.Width(30), GUILayout.Height(30)))
                            {
                                _selectedClassInfo.SkillMetaSusceptibilities.Remove(_selectedClassInfo.SkillMetaSusceptibilities[index]);
                                index--;
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.Space(5);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No Skill Metas Found.", MessageType.Info);
                        _selectedClassInfo.SkillMetaSusceptibilities = new List<SkillMetaSusceptibility>();
                    }

                    if (susceptResult == 0)
                    {
                        _selectedClassInfo.SkillMetaSusceptibilities.Add(new SkillMetaSusceptibility());
                    }
                }
                if (skillSusFoldout) RPGMakerGUI.EndFoldout();
                #endregion

                #region "StartingPet"
                if (RPGMakerGUI.Foldout(ref startingPetFoldout, "Starting Pet"))
                {
                    GUILayout.BeginHorizontal();
                    if (RPGMakerGUI.Toggle("Enable?", ref _selectedClassInfo.HasStartingPet))
                    {
                        RPGMakerGUI.PopupID<Rm_PetDefinition>("Pet:", ref _selectedClassInfo.StartingPet);
                    }
                    GUILayout.EndHorizontal();
                    RPGMakerGUI.EndFoldout();
                }
                #endregion

                #region StartingSkills

                var result = RPGMakerGUI.FoldoutToolBar(ref startingSkillsFoldout, "Starting Skills", new string[] {"+Skill"});
                if (startingSkillsFoldout)
                {
                    for (int index = 0; index < _selectedClassInfo.StartingSkillIds.Count; index++)
                    {
                        var v = _selectedClassInfo.StartingSkillIds[index];
                        if (String.IsNullOrEmpty(v)) continue;

                        var stillExists =
                            Rm_RPGHandler.Instance.Repositories.Skills.AllSkills.FirstOrDefault(
                                a => a.ID == v);

                        if (stillExists == null)
                        {
                            _selectedClassInfo.StartingSkillIds.Remove(v);
                            index--;
                        }
                    }
                    var allSkills = Rm_RPGHandler.Instance.Repositories.Skills.AllSkills;
                    if (allSkills.Count > 0)
                    {
                        if (_selectedClassInfo.StartingSkillIds.Count == 0)
                        {
                            EditorGUILayout.HelpBox("Click +Skill to add a skill this class starts with.", MessageType.Info);
                        }

                        GUILayout.Space(5);
                        for (int index = 0; index < _selectedClassInfo.StartingSkillIds.Count; index++)
                        {

                            GUILayout.BeginHorizontal();
                            var skillId = _selectedClassInfo.StartingSkillIds[index];
                            RPGMakerGUI.PopupID<Skill>("Skill Name:", ref skillId);
                            _selectedClassInfo.StartingSkillIds[index] = skillId;

                            if (GUILayout.Button(RPGMakerGUI.DelIcon, "genericButton", GUILayout.Width(30), GUILayout.Height(30)))
                            {
                                _selectedClassInfo.StartingSkillIds.Remove(_selectedClassInfo.StartingSkillIds[index]);
                                index--;
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.Space(5);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No Skills Found.", MessageType.Warning);
                        _selectedClassInfo.StartingSkillIds = new List<string>();
                    }

                    if (result == 0)
                    {
                        _selectedClassInfo.StartingSkillIds.Add("");
                    }
                    RPGMakerGUI.EndFoldout();
                }

                #endregion

                #region StartingTalents

                var talentResult = RPGMakerGUI.FoldoutToolBar(ref startingTalentsFoldout, "Starting Talents", new string[] {"+Talent"});
                if (startingTalentsFoldout)
                {
                    for (int index = 0; index < _selectedClassInfo.StartingTalentIds.Count; index++)
                    {
                        var v = _selectedClassInfo.StartingTalentIds[index];
                        if (String.IsNullOrEmpty(v)) continue;

                        var stillExists =
                            Rm_RPGHandler.Instance.Repositories.Talents.AllTalents.FirstOrDefault(
                                a => a.ID == v);

                        if (stillExists == null)
                        {
                            _selectedClassInfo.StartingTalentIds.Remove(v);
                            index--;
                        }
                    }
                    var allTalents = Rm_RPGHandler.Instance.Repositories.Talents.AllTalents;
                    if (allTalents.Count > 0)
                    {
                        if (_selectedClassInfo.StartingTalentIds.Count == 0)
                        {
                            EditorGUILayout.HelpBox("Click +Talent to add a talent this class starts with.", MessageType.Info);
                        }

                        GUILayout.Space(5);
                        for (int index = 0; index < _selectedClassInfo.StartingTalentIds.Count; index++)
                        {

                            GUILayout.BeginHorizontal();
                            var talentId = _selectedClassInfo.StartingTalentIds[index];
                            RPGMakerGUI.PopupID<Talent>("Talent Name:", ref talentId);
                            _selectedClassInfo.StartingTalentIds[index] = talentId;

                            if (GUILayout.Button(RPGMakerGUI.DelIcon, "genericButton", GUILayout.Width(30), GUILayout.Height(30)))
                            {
                                _selectedClassInfo.StartingTalentIds.Remove(_selectedClassInfo.StartingTalentIds[index]);
                                index--;
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.Space(5);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No Talents Found.", MessageType.Warning);
                        _selectedClassInfo.StartingTalentIds = new List<string>();
                    }

                    if (talentResult == 0)
                    {
                        _selectedClassInfo.StartingTalentIds.Add("");
                    }
                    RPGMakerGUI.EndFoldout();
                }

                #endregion

                #region "Starting Items"
                var gurResult = RPGMakerGUI.FoldoutToolBar(ref startItemsFoldout, "Starting Items","+Item");
                if (startItemsFoldout)
                {
                    for (int index = 0; index < _selectedClassInfo.StartingItems.Count; index++)
                    {
                        var v = _selectedClassInfo.StartingItems[index];
                        if (string.IsNullOrEmpty(v.ItemID)) continue;

                        var stillExists =
                            Rm_RPGHandler.Instance.Repositories.Items.AllItems.FirstOrDefault(
                                a => a.ID == v.ItemID);

                        if (stillExists == null)
                        {
                            _selectedClassInfo.StartingItems.Remove(v);
                            index--;
                        }
                    }

                    var allItems = Rm_RPGHandler.Instance.Repositories.Items.AllItems;
                    if (allItems.Count > 0)
                    {
                        if (_selectedClassInfo.StartingItems.Count == 0)
                        {
                            EditorGUILayout.HelpBox("Click +Item to add an item the player will start with in their inventory.", MessageType.Info);
                        }

                        GUILayout.Space(5);



                        for (int index = 0; index < _selectedClassInfo.StartingItems.Count; index++)
                        {
                            GUILayout.BeginHorizontal();
                            var selectedSus = _selectedClassInfo.StartingItems[index];
                            RPGMakerGUI.PopupID<Item>("Item:", ref selectedSus.ItemID);

                            var stackable = allItems.FirstOrDefault(i => i.ID == selectedSus.ItemID) as IStackable;
                            if (stackable != null)
                            {
                                selectedSus.Amount = RPGMakerGUI.IntField("Amount:", selectedSus.Amount);
                            }
                            if (GUILayout.Button(RPGMakerGUI.DelIcon, "genericButton", GUILayout.Width(30), GUILayout.Height(30)))
                            {
                                _selectedClassInfo.StartingItems.Remove(_selectedClassInfo.StartingItems[index]);
                                index--;
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.Space(5);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No Items Found.", MessageType.Info);
                        _selectedClassInfo.StartingItems = new List<LootDefinition>();
                    }

                    if (gurResult == 0)
                    {
                        _selectedClassInfo.StartingItems.Add(new LootDefinition());
                    }

                    RPGMakerGUI.EndFoldout();
                }
                #endregion

                #region "Starting Equipped"

                if (RPGMakerGUI.Foldout(ref startEquipFoldout, "Starting Equipped Items"))
                {
                    for (int index = 0; index < _selectedClassInfo.StartingEquipped.Count; index++)
                    {
                        var v = _selectedClassInfo.StartingEquipped[index];
                        if (string.IsNullOrEmpty(v.SlotID)) continue;

                        var stillExists =
                            Rm_RPGHandler.Instance.Items.ApparelSlots.FirstOrDefault(
                                a => a.ID == v.SlotID);

                        if (stillExists == null)
                        {
                            _selectedClassInfo.StartingEquipped.Remove(v);
                            index--;
                        }
                    }

                    for (int index = 0; index < _selectedClassInfo.StartingEquipped.Count; index++)
                    {
                        var v = _selectedClassInfo.StartingEquipped[index];
                        if (string.IsNullOrEmpty(v.ItemID)) continue;

                        var stillExists =
                            Rm_RPGHandler.Instance.Repositories.Items.AllItems.FirstOrDefault(
                                a => a.ID == v.ItemID);

                        if (stillExists == null)
                        {
                            v.Enabled = false;
                            v.ItemID = "";
                            index--;
                        }
                    }

                    for (var i = 0; i < Rm_RPGHandler.Instance.Items.ApparelSlots.Count; i++)
                    {
                        var slotName = Rm_RPGHandler.Instance.Items.ApparelSlots[i];

                        var exists = _selectedClassInfo.StartingEquipped.FirstOrDefault(v => v.SlotID == slotName.ID);
                        if(exists == null)
                        {
                            var newStartEquip = new StartEquipDefinition() {SlotID = slotName.ID, SlotName = slotName.Name, ItemID = ""};
                            _selectedClassInfo.StartingEquipped.Add(newStartEquip);    
                        }
                    }


                    //Weapon
                    GUILayout.BeginHorizontal();
                    if (RPGMakerGUI.Toggle("Weapon?", ref _selectedClassInfo.StartingEquippedWeapon.Enabled))
                    {
                        RPGMakerGUI.PopupID<Item>("Item:", ref _selectedClassInfo.StartingEquippedWeapon.ItemID);
                    }
                    GUILayout.EndHorizontal();

                    //Rest
                    for (var i = 0; i < _selectedClassInfo.StartingEquipped.Count; i++)
                    {
                        var slotName = _selectedClassInfo.StartingEquipped[i];
                        GUILayout.BeginHorizontal();
                        if (RPGMakerGUI.Toggle(slotName.SlotName + "?", ref slotName.Enabled))
                        {
                            RPGMakerGUI.PopupID<Item>("Item:", ref slotName.ItemID);
                        }
                        GUILayout.EndHorizontal();
                    }

                    RPGMakerGUI.EndFoldout();
                }
                #endregion

                RPGMakerGUI.EndScrollView();
            }
            else
            {
                EditorGUILayout.HelpBox("Add or select a new field to customise player classes.", MessageType.Info);
            }
            GUILayout.EndArea();
        }

        private static Rm_RaceDefinition selectedRaceInfo = null;
        private static bool raceMainInfoFoldout = true;
        public static void Races(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            var list = Rm_RPGHandler.Instance.Player.RaceDefinitions;
            GUI.Box(leftArea, "", "backgroundBox");
            GUI.Box(mainArea, "", "backgroundBoxMain");

            GUILayout.BeginArea(PadRect(leftArea, 0, 0));
            RPGMakerGUI.ListArea(list, ref selectedRaceInfo, Rm_ListAreaType.Races, false, true);
            GUILayout.EndArea();


            GUILayout.BeginArea(mainArea);
            RPGMakerGUI.Title("Races");
            if (selectedRaceInfo != null)
            {
                RPGMakerGUI.BeginScrollView();
                if (RPGMakerGUI.Foldout(ref raceMainInfoFoldout, "Main Info"))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical(GUILayout.MaxWidth(85));
                    selectedRaceInfo.Image.Image = RPGMakerGUI.ImageSelector("", selectedRaceInfo.Image.Image,
                                                                        ref selectedRaceInfo.Image.ImagePath);

                    GUILayout.EndVertical();
                    GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                    selectedRaceInfo.Name = RPGMakerGUI.TextField("Name: ", selectedRaceInfo.Name);
                    selectedRaceInfo.Description = RPGMakerGUI.TextArea("Description: ", selectedRaceInfo.Description);
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    RPGMakerGUI.EndFoldout();
                }

                RPGMakerGUI.EndScrollView();
            }
            else
            {
                EditorGUILayout.HelpBox("Add or select a new field to customise races. At least 1 is always required.", MessageType.Info);
            }
            GUILayout.EndArea();
        }

        private static Rm_SubRaceDefinition selectedSubRaceInfo = null;
        private static bool subRaceMainInfoFoldout = true;
        public static void SubRaces(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            var list = Rm_RPGHandler.Instance.Player.SubRaceDefinitions;
            GUI.Box(leftArea, "", "backgroundBox");
            GUI.Box(mainArea, "", "backgroundBoxMain");

            GUILayout.BeginArea(PadRect(leftArea, 0, 0));
            RPGMakerGUI.ListArea(list, ref selectedSubRaceInfo, Rm_ListAreaType.SubRaces, false, true);
            GUILayout.EndArea();


            GUILayout.BeginArea(mainArea);
            RPGMakerGUI.Title("Sub-Races");
            if (selectedSubRaceInfo != null)
            {
                RPGMakerGUI.BeginScrollView();
                if (RPGMakerGUI.Foldout(ref subRaceMainInfoFoldout, "Main Info"))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical(GUILayout.MaxWidth(85));
                    selectedSubRaceInfo.Image.Image = RPGMakerGUI.ImageSelector("", selectedSubRaceInfo.Image.Image,
                                                                        ref selectedSubRaceInfo.Image.ImagePath);

                    GUILayout.EndVertical();
                    GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                    selectedSubRaceInfo.Name = RPGMakerGUI.TextField("Name: ", selectedSubRaceInfo.Name);
                    RPGMakerGUI.PopupID<Rm_RaceDefinition>("Applicable Race:", ref selectedSubRaceInfo.ApplicableRaceID);
                    selectedSubRaceInfo.Description = RPGMakerGUI.TextArea("Description: ", selectedSubRaceInfo.Description);
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    RPGMakerGUI.EndFoldout();
                }

                RPGMakerGUI.EndScrollView();
            }
            else
            {
                EditorGUILayout.HelpBox("Add or select a new field to customise sub-races. At least 1 is always required.", MessageType.Info);
            }
            GUILayout.EndArea();
        }

        private static Rm_MetaDataDefinition selectedMetaDataInfo = null;
        private static bool metaDataMainInfoFoldout = true;
        private static bool metaDataValuesFoldout = true;
        public static void MetaDatas(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            var list = Rm_RPGHandler.Instance.Player.MetaDataDefinitions;
            GUI.Box(leftArea, "", "backgroundBox");
            GUI.Box(mainArea, "", "backgroundBoxMain");

            GUILayout.BeginArea(PadRect(leftArea, 0, 0));
            RPGMakerGUI.ListArea(list, ref selectedMetaDataInfo, Rm_ListAreaType.MetaDatas, false, true);
            GUILayout.EndArea();


            GUILayout.BeginArea(mainArea);
            RPGMakerGUI.Title("Meta-Datas");
            if (selectedMetaDataInfo != null)
            {
                RPGMakerGUI.BeginScrollView();
                if (RPGMakerGUI.Foldout(ref metaDataMainInfoFoldout, "Main Info"))
                {
                    
                    GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                    selectedMetaDataInfo.Name = RPGMakerGUI.TextField("Name: ", selectedMetaDataInfo.Name);
                    selectedMetaDataInfo.TooltipDescription = RPGMakerGUI.TextField("Tooltip Description: ", selectedMetaDataInfo.TooltipDescription);

                    var metaDataResult = RPGMakerGUI.FoldoutToolBar(ref metaDataValuesFoldout, "MetaData Values", "+Value");
                    if (metaDataValuesFoldout)
                    {
                        for (int i = 0; i < selectedMetaDataInfo.Values.Count; i++)
                        {
                            GUILayout.BeginVertical("backgroundBox");
                                GUILayout.BeginHorizontal();
                                    GUILayout.BeginVertical(GUILayout.MaxWidth(85));
                                    selectedMetaDataInfo.Values[i].Image.Image = RPGMakerGUI.ImageSelector("", selectedMetaDataInfo.Values[i].Image.Image,
                                                                                        ref selectedMetaDataInfo.Values[i].Image.ImagePath);

                                    GUILayout.EndVertical();

                                    GUILayout.BeginVertical();
                                    selectedMetaDataInfo.Values[i].Name = RPGMakerGUI.TextField("Value Name:", selectedMetaDataInfo.Values[i].Name);
                                    selectedMetaDataInfo.Values[i].Description = RPGMakerGUI.TextArea("Description:", selectedMetaDataInfo.Values[i].Description);
                                    GUILayout.EndVertical();
                                GUILayout.EndHorizontal();

                                GUILayout.Space(10);

                                GUILayout.BeginHorizontal();
                                    GUILayout.FlexibleSpace();
                                    if (GUILayout.Button("Delete", "genericButton",GUILayout.Height(30)))   
                                    {
                                        selectedMetaDataInfo.Values.RemoveAt(i);
                                        return;
                                    }
                                    GUILayout.FlexibleSpace();
                                GUILayout.EndHorizontal();
                            GUILayout.EndVertical();

                            GUILayout.Space(20);

                        }

                        if (metaDataResult == 0)
                        {
                            selectedMetaDataInfo.Values.Add(new MetaDataValue());
                        }

                        if (selectedMetaDataInfo.Values.Count == 0)
                        {
                            EditorGUILayout.HelpBox(
                                "Click +Value to add a new value option for this metadata.",
                                MessageType.Info);
                        }

                        RPGMakerGUI.EndFoldout();
                    }

                    GUILayout.EndVertical();
                    RPGMakerGUI.EndFoldout();
                }

                RPGMakerGUI.EndScrollView();
            }
            else
            {
                EditorGUILayout.HelpBox("Add or select a new field to customise meta-datas.", MessageType.Info);
            }
            GUILayout.EndArea();
        }

        private static StringDefinition selectedGenderInfo = null;
        private static bool genderMainInfoFoldout = true;
        public static void Genders(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            var list = Rm_RPGHandler.Instance.Player.GenderDefinitions;
            GUI.Box(leftArea, "", "backgroundBox");
            GUI.Box(mainArea, "", "backgroundBoxMain");

            GUILayout.BeginArea(PadRect(leftArea, 0, 0));
            RPGMakerGUI.ListArea(list, ref selectedGenderInfo, Rm_ListAreaType.Genders, false, true);
            GUILayout.EndArea();


            GUILayout.BeginArea(mainArea);
            RPGMakerGUI.Title("Genders");
            if (selectedGenderInfo != null)
            {
                RPGMakerGUI.BeginScrollView();
                if (RPGMakerGUI.Foldout(ref genderMainInfoFoldout, "Info"))
                {
                    
                    GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                    selectedGenderInfo.Name = RPGMakerGUI.TextField("Name: ", selectedGenderInfo.Name);
                    GUILayout.EndVertical();
                    RPGMakerGUI.EndFoldout();
                }

                RPGMakerGUI.EndScrollView();
            }
            else
            {
                EditorGUILayout.HelpBox("Add or select a new field to customise genders. At least 1 is always required.", MessageType.Info);
            }
            GUILayout.EndArea();
        }

        private static Rm_ClassNameDefinition selectedClassNameInfo = null;
        private static bool classNameMainInfoFoldout = true;
        public static void ClassNames(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            var list = Rm_RPGHandler.Instance.Player.ClassNameDefinitions;
            GUI.Box(leftArea, "", "backgroundBox");
            GUI.Box(mainArea, "", "backgroundBoxMain");

            GUILayout.BeginArea(PadRect(leftArea, 0, 0));
            RPGMakerGUI.ListArea(list, ref selectedClassNameInfo, Rm_ListAreaType.ClassName, false, true);
            GUILayout.EndArea();


            GUILayout.BeginArea(mainArea);
            RPGMakerGUI.Title("Class Names");
            if (selectedClassNameInfo != null)
            {
                RPGMakerGUI.BeginScrollView();
                if (RPGMakerGUI.Foldout(ref classNameMainInfoFoldout, "Info"))
                {
                    
                    GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                    selectedClassNameInfo.Name = RPGMakerGUI.TextField("Name: ", selectedClassNameInfo.Name);
                    selectedClassNameInfo.Description = RPGMakerGUI.TextArea("Description: ", selectedClassNameInfo.Description);
                    selectedClassNameInfo.Image.Image = RPGMakerGUI.ImageSelector("", selectedClassNameInfo.Image.Image,
                                                    ref selectedClassNameInfo.Image.ImagePath);
                    GUILayout.EndVertical();
                    RPGMakerGUI.EndFoldout();
                }

                RPGMakerGUI.EndScrollView();
            }
            else
            {
                EditorGUILayout.HelpBox("Add or select a new field to customise class names.", MessageType.Info);
            }
            GUILayout.EndArea();
        }

        #endregion

        public static Rmh_Experience Exp
        {
            get { return Rm_RPGHandler.Instance.Experience; }
        }

        static Rme_Main_Player()
        {
            customExpDefFoldoutBools = new bool[999][];
            
            for (int i = 0; i < Exp.CustomExpDefinitions.Count; i++)
            {
                customExpDefFoldoutBools[i] = new []{false, false, false};
            }
        }

        public static Vector2 petsScrollPos = Vector2.zero;
        private static Rm_PetDefinition selectedPetInfo = null;
        private static int selectedPetType = 0;
        public static void Pets(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            var list = Rm_RPGHandler.Instance.Player.PetDefinitions;
            GUI.Box(leftArea, "", "backgroundBox");
            GUI.Box(mainArea, "", "backgroundBoxMain");

            GUILayout.BeginArea(PadRect(leftArea, 0, 0));
            RPGMakerGUI.ListArea(list, ref selectedPetInfo, Rm_ListAreaType.Pets, false, true);
            GUILayout.EndArea();

            GUILayout.BeginArea(mainArea);
            RPGMakerGUI.Title("Classes");
            if (selectedPetInfo != null)
            {
                RPGMakerGUI.SubTitle("Pet Definition: " + selectedPetInfo.Name);
                selectedPetInfo.Name = RPGMakerGUI.TextField("Name: ", selectedPetInfo.Name);
                selectedPetType = selectedPetInfo.IsNpc ? 0 : 1;
                var oldType = selectedPetInfo.IsNpc;
                selectedPetType = RPGMakerGUI.Popup("Pet Source:", selectedPetType, new string[] {"NPC Db", "Enemy Db"}, 0);
                selectedPetInfo.IsNpc = selectedPetType == 0;
                if (selectedPetInfo.IsNpc != oldType)
                {
                    selectedPetInfo.CharacterID = "";
                }

                if(selectedPetInfo.IsNpc)
                {
                    RPGMakerGUI.PopupID<NonPlayerCharacter>("NPC Character:", ref selectedPetInfo.CharacterID, 1);
                }
                else
                {
                    RPGMakerGUI.PopupID<CombatCharacter>("Enemy Character:", ref selectedPetInfo.CharacterID, 1);
                }

                selectedPetInfo.DefaultBehaviour = (PetBehaviour) RPGMakerGUI.EnumPopup("Default Behaviour:", selectedPetInfo.DefaultBehaviour);
            }
            GUILayout.EndArea();
        }


        public static bool showCustomExpDefinitions = true;
        public static Vector2 expScrollPos = Vector2.zero;
        public static bool[] playerExpDefFoldoutBools = new bool[] { true, false, false };
        public static bool[] traitExpDefFoldoutBools = new bool[] { true, false, false };
        public static bool[] monsterExpDefFoldoutBools = new bool[] { true, false, false };
        public static bool[][] customExpDefFoldoutBools;
        public static bool[] titleFoldoutBools = new bool[250];
        public static void Experience(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            GUI.Box(fullArea, "","backgroundBox");

            GUILayout.BeginArea(fullArea);
            expScrollPos = GUILayout.BeginScrollView(expScrollPos);

            ExpFoldout(Exp.PlayerExpDefinition, ref playerExpDefFoldoutBools,0);
            ExpFoldout(Exp.TraitExpDefinition, ref traitExpDefFoldoutBools,1);
            ExpFoldout(Exp.MonsterExpDefinition, ref monsterExpDefFoldoutBools,2);

            var result = RPGMakerGUI.FoldoutToolBar(ref showCustomExpDefinitions, "Custom Exp Definitions", "+ExpDefinition");
            if(showCustomExpDefinitions)
            {
                if (result == 0)
                {
                    Exp.CustomExpDefinitions.Add(new ExpDefinition());
                    customExpDefFoldoutBools[Exp.CustomExpDefinitions.Count - 1] = new []{false, false, false};
                }

                for (int i = 0; i < Exp.CustomExpDefinitions.Count; i++)
                {
                    var expDef = Exp.CustomExpDefinitions[i];
                    ExpFoldout(expDef, ref customExpDefFoldoutBools[i],3 + i, true);
                }

                if (Exp.CustomExpDefinitions.Count == 0)
                {
                    EditorGUILayout.HelpBox(
                        "Click +ExpDefinition to add a new exp definition which classes, traits and monster types can use.",
                        MessageType.Info);
                }

                RPGMakerGUI.EndFoldout();
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        public static void ExpFoldout(ExpDefinition expDefinition, ref bool[] foldoutBooleans, int titleFoldOutIndex, bool allowDelete = false)
        {
            if(RPGMakerGUI.Foldout(ref titleFoldoutBools[titleFoldOutIndex], "Exp Definition: " + expDefinition.Name))
            {
                if (RPGMakerGUI.Foldout(ref foldoutBooleans[0], "Experience Details"))
                {
                    if (!expDefinition.IsDefault)
                    {
                        expDefinition.Name = RPGMakerGUI.TextField("Name:", expDefinition.Name);
                        expDefinition.ExpUse = (ExpUse)RPGMakerGUI.EnumPopup("Exp Use:", expDefinition.ExpUse);

                    }

                    expDefinition.ExpType = (ExpType)RPGMakerGUI.EnumPopup("Exp Type:", expDefinition.ExpType);
                    if (expDefinition.IsDefault || expDefinition.ExpUse == ExpUse.TraitLevelling)
                    {
                        expDefinition.MaxLevel = EditorGUILayout.IntField("Max Level", expDefinition.MaxLevel);
                    }
                    else
                    {
                        if (expDefinition.ExpUse == ExpUse.PlayerLevelling)
                        {
                            expDefinition.MaxLevel = Exp.AllExpDefinitions.First(e => e.ID == Rmh_Experience.PlayerExpDefinitionID).MaxLevel;
                        }
                        else
                        {
                            expDefinition.MaxLevel = Exp.AllExpDefinitions.First(e => e.ID == Rmh_Experience.MonsterExpDefinitionID).MaxLevel;
                        }
                    }

                    var expInfoText = expDefinition.ExpUse != ExpUse.ExpGained ? "Exp To Level for " : "Exp Gained at ";

                    if (expDefinition.ExpType != ExpType.Linear)
                    {
                        expDefinition.XpForFirstLevel = (long)EditorGUILayout.FloatField(expInfoText + "First Level:", expDefinition.XpForFirstLevel);
                    }

                    if (expDefinition.ExpType == ExpType.Logarithmic)
                    {
                        expDefinition.XpForLastLevel = (long)EditorGUILayout.FloatField(expInfoText + "Max Level", expDefinition.XpForLastLevel);
                    }

                    if (expDefinition.ExpType != ExpType.Logarithmic)
                    {
                        expDefinition.Constant = (long)EditorGUILayout.FloatField("Constant Value:", expDefinition.Constant);
                    }

                    RPGMakerGUI.EndFoldout();
                }

                var expForLevelText = expDefinition.ExpUse != ExpUse.ExpGained ? "Exp To Level " : "Exp Gained At Level ";
                if (RPGMakerGUI.Foldout(ref foldoutBooleans[1], expForLevelText))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();
                    for (int i = 1; i <= expDefinition.MaxLevel; i++)
                    {
                        if (i == (expDefinition.MaxLevel / 3) + 2)
                        {
                            GUILayout.EndVertical();
                            GUILayout.BeginVertical();
                        }
                        if (i == ((expDefinition.MaxLevel / 3) * 2) + 3)
                        {
                            GUILayout.EndVertical();
                            GUILayout.BeginVertical();
                        }

                        GUILayout.Box(i + ":" + expDefinition.ExpForLevel(i), "subTitle", GUILayout.Width(271));
                    }
                    GUILayout.EndVertical();
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    RPGMakerGUI.EndFoldout();
                }

                if (expDefinition.ExpUse != ExpUse.ExpGained)
                {
                    if (RPGMakerGUI.Foldout(ref foldoutBooleans[2], "Max Exp at Level"))
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.BeginVertical();
                        long total = 0;
                        for (int i = 1; i <= expDefinition.MaxLevel; i++)
                        {
                            if (i == (expDefinition.MaxLevel / 3) + 2)
                            {
                                GUILayout.EndVertical();
                                GUILayout.BeginVertical();
                            }
                            if (i == ((expDefinition.MaxLevel / 3) * 2) + 3)
                            {
                                GUILayout.EndVertical();
                                GUILayout.BeginVertical();
                            }


                            total += expDefinition.ExpForLevel(i);
                            GUILayout.Box(i + ":" + total, "subTitle", GUILayout.Width(271));
                        }
                        GUILayout.EndVertical();
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        RPGMakerGUI.EndFoldout();
                    }
                }

                if (allowDelete)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Delete", "genericButton", GUILayout.Height(25)))
                    {
                        titleFoldoutBools[titleFoldOutIndex] = false;
                        foldoutBooleans[0] = false;
                        foldoutBooleans[1] = false;
                        foldoutBooleans[2] = false;
                        Exp.CustomExpDefinitions.Remove(expDefinition);
                        return;
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }

                RPGMakerGUI.EndFoldout();
            }
            
        }

        public static Rect PadRect(Rect rect, int left, int top)
        {
            return new Rect(rect.x + left, rect.y + top, rect.width - (left * 2), rect.height - (top * 2));
        }
    }
}
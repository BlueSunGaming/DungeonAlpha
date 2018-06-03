using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;
using UnityEditor;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Editor
{
    public static class Rme_Combatants
    {
        public static Rmh_Enemy Enemy
        {
            get { return Rm_RPGHandler.Instance.Enemy; }
        }

        private static GameObject selectedPrefab;
        private static bool animFoldout = true;
        private static bool animTypeFoldout = true;
        private static bool wepFoldout = true;
        private static int[] selectedAnim = new int[999];
        private static int selectedIdleAnim = 0;
        private static int selectedWalkAnim = 0;
        private static int selectedRunAnim = 0;

        private static bool baseAttrFoldout = true;
        private static bool baseVitFoldout = true;
        private static bool baseStatFoldout = true;
        private static bool startingSkillsFoldout = true;
        private static bool skillImmunFoldout = true;
        private static bool skillSusFoldout = true;
        private static bool showBonusDamage = true;
        private static bool projectileInfoFoldout = true;

        private static bool gurLootFoldout = true;
        private static bool lootTableFoldout = true;

        private static bool UseSelectedForAnims;
        private static GameObject selected = null;

        public static void CombatantDetails(CombatCharacter selectedCharInfo)
        {
            selectedCharInfo.Level = RPGMakerGUI.IntField("Level: ", selectedCharInfo.Level);
            RPGMakerGUI.PopupID<MonsterTypeDefinition>("Enemy Type:", ref selectedCharInfo.MonsterTypeID);

            selectedCharInfo.ProgressionGain.CombatantLevel = selectedCharInfo.Level;
            Rme_Main_General.ProgressionGain(false, selectedCharInfo.ProgressionGain, true);
            
            selectedCharInfo.AttackStyle =
                (AttackStyle)RPGMakerGUI.EnumPopup("Attack Style", selectedCharInfo.AttackStyle);
            if(RPGMakerGUI.Toggle("Is Aggresive", ref selectedCharInfo.IsAggressive))
            {
                 if(RPGMakerGUI.Toggle("Override Aggro Radius?", ref selectedCharInfo.OverrideAggroRadius))
                 {
                     selectedCharInfo.OverrideAggroRadiusValue = RPGMakerGUI.FloatField("Aggro Radius:", selectedCharInfo.OverrideAggroRadiusValue);
                 }
            }
            selectedCharInfo.RetreatsWhenLow = RPGMakerGUI.Toggle("Retreats when low health?", selectedCharInfo.RetreatsWhenLow);
            RPGMakerGUI.PopupID<ReputationDefinition>("Reputation:", ref selectedCharInfo.ReputationId);

            selectedPrefab = RPGMakerGUI.PrefabSelector("Replacement Prefab?", selectedPrefab,
                                                                ref selectedCharInfo.PrefabReplacementOnDeath);
        }
        public static void Animations(CombatCharacter selectedCharInfo)
        {
            var selectedNpc = selectedCharInfo as NonPlayerCharacter;
            var foundAnim = false;
            string[] anims = new string[0];

            if(UseSelectedForAnims)
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
                if (animFoldout || wepFoldout)
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
                selectedCharInfo.AnimationType = (RPGAnimationType)RPGMakerGUI.EnumPopup("Animation Type", selectedCharInfo.AnimationType);
                if(selectedCharInfo.AnimationType == RPGAnimationType.Mecanim)
                {
                    GUILayout.Label("For more information on mecanim please visit, https://www.youtube.com/watch?v=0cvwtk8Y_Uk");
                }
                RPGMakerGUI.EndFoldout();
            }

            #endregion

            #region All Animations

            
            #region Core Anims

            if (RPGMakerGUI.Foldout(ref animFoldout, "Core Animations"))
            {
                if (selectedCharInfo.AnimationType == RPGAnimationType.Legacy)
                {
                    RPGMakerGUI.Toggle("Get Animations From Selected Object?", ref UseSelectedForAnims);
                }

                GUILayout.BeginHorizontal("backgroundBox");
                GUILayout.Label("ID", GUILayout.Width(100));
                if (selectedCharInfo.AnimationType == RPGAnimationType.Legacy)
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

                if (selectedCharInfo.AnimationType == RPGAnimationType.Legacy && foundAnim)
                {
                    if (selectedCharInfo.CharacterType != CharacterType.NPC || (Rm_RPGHandler.Instance.Combat.NPCsCanFight && selectedNpc.CanFight))
                    {
                        #region KeyAnim

                        for (int index = 0; index < KeyAnimations(selectedCharInfo).Count; index++)
                        {
                            var anim = KeyAnimations(selectedCharInfo)[index];
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

                        #endregion
                    }
                    else
                    {
                        var anim = selectedCharInfo.LegacyAnimations.IdleAnim;
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(anim.Name, GUILayout.Width(100));

                        selectedIdleAnim = anims.FirstOrDefault(s => s == anim.Animation) != null
                                                ? Array.IndexOf(anims, anim.Animation)
                                                : 0;

                        selectedIdleAnim = EditorGUILayout.Popup(selectedIdleAnim, anims,
                                                                    GUILayout.Width(150));
                        anim.Animation = anims[selectedIdleAnim];
                        if (anim.Animation == "None") anim.Animation = "";
                        anim.Speed = RPGMakerGUI.FloatField("", anim.Speed, GUILayout.Width(70));
                        anim.Sound = RPGMakerGUI.AudioClipSelector("", anim.Sound, ref anim.SoundPath,
                                                                    GUILayout.Width(150));
                        anim.Backwards = RPGMakerGUI.Toggle(anim.Backwards);
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        GUILayout.Space(5);

                        var walkAnim = selectedCharInfo.LegacyAnimations.WalkAnim;
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(walkAnim.Name, GUILayout.Width(100));

                        selectedWalkAnim = anims.FirstOrDefault(s => s == walkAnim.Animation) != null
                                                ? Array.IndexOf(anims, walkAnim.Animation)
                                                : 0;

                        selectedWalkAnim = EditorGUILayout.Popup(selectedWalkAnim, anims,
                                                                    GUILayout.Width(150));
                        walkAnim.Animation = anims[selectedWalkAnim];
                        if (walkAnim.Animation == "None") walkAnim.Animation = "";
                        walkAnim.Speed = RPGMakerGUI.FloatField("", walkAnim.Speed, GUILayout.Width(70));
                        walkAnim.Sound = RPGMakerGUI.AudioClipSelector("", walkAnim.Sound, ref walkAnim.SoundPath,
                                                                    GUILayout.Width(150));
                        walkAnim.Backwards = RPGMakerGUI.Toggle(walkAnim.Backwards);
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        GUILayout.Space(5);


                        var runAnim = selectedCharInfo.LegacyAnimations.RunAnim;
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(runAnim.Name, GUILayout.Width(100));

                        selectedRunAnim = anims.FirstOrDefault(s => s == runAnim.Animation) != null
                                                ? Array.IndexOf(anims, runAnim.Animation)
                                                : 0;

                        selectedRunAnim = EditorGUILayout.Popup(selectedRunAnim, anims,
                                                                    GUILayout.Width(150));
                        runAnim.Animation = anims[selectedRunAnim];
                        if (runAnim.Animation == "None") runAnim.Animation = "";
                        runAnim.Speed = RPGMakerGUI.FloatField("", runAnim.Speed, GUILayout.Width(70));
                        runAnim.Sound = RPGMakerGUI.AudioClipSelector("", runAnim.Sound, ref runAnim.SoundPath,
                                                                    GUILayout.Width(150));
                        runAnim.Backwards = RPGMakerGUI.Toggle(runAnim.Backwards);
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        GUILayout.Space(5);
                    }

                }
                else
                {
                    if (selectedCharInfo.AnimationType == RPGAnimationType.Legacy)
                    {
                        EditorGUILayout.HelpBox(
                            "Select a gameObject with the animation component and animations to get drop-downs for animation name instead of typing it in.",
                            MessageType.Info);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("For more information on mecanim visit https://www.youtube.com/watch?v=0cvwtk8Y_Uk", MessageType.Info);
                    }

                    if (selectedCharInfo.CharacterType != CharacterType.NPC || (Rm_RPGHandler.Instance.Combat.NPCsCanFight && selectedNpc.CanFight))
                    {
                        #region KeyAnim

                        foreach (var anim in KeyAnimations(selectedCharInfo))
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label(anim.Name, GUILayout.Width(100));
                            if (selectedCharInfo.AnimationType == RPGAnimationType.Legacy)
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

                        #endregion
                    }
                    else
                    {
                        var anim = selectedCharInfo.LegacyAnimations.IdleAnim;
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(anim.Name, GUILayout.Width(100));
                        if (selectedCharInfo.AnimationType == RPGAnimationType.Legacy)
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
                        GUILayout.Space(5);



                        var walkAnim = selectedCharInfo.LegacyAnimations.WalkAnim;
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(walkAnim.Name, GUILayout.Width(100));
                        if (selectedCharInfo.AnimationType == RPGAnimationType.Legacy)
                        {
                            walkAnim.Animation = RPGMakerGUI.TextField("", walkAnim.Animation, GUILayout.Width(150));
                            walkAnim.Speed = RPGMakerGUI.FloatField("", walkAnim.Speed, GUILayout.Width(70));
                            walkAnim.Sound = RPGMakerGUI.AudioClipSelector("", walkAnim.Sound, ref walkAnim.SoundPath,
                                                                       GUILayout.Width(150));
                            walkAnim.Backwards = RPGMakerGUI.Toggle(walkAnim.Backwards);
                        }
                        else
                        {
                            GUILayout.Space(150);
                            walkAnim.Sound = RPGMakerGUI.AudioClipSelector("", walkAnim.Sound, ref walkAnim.SoundPath,
                                                                       GUILayout.Width(150));
                        }
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        GUILayout.Space(5);

                        var runAnim = selectedCharInfo.LegacyAnimations.RunAnim;
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(runAnim.Name, GUILayout.Width(100));
                        if (selectedCharInfo.AnimationType == RPGAnimationType.Legacy)
                        {
                            runAnim.Animation = RPGMakerGUI.TextField("", runAnim.Animation, GUILayout.Width(150));
                            runAnim.Speed = RPGMakerGUI.FloatField("", runAnim.Speed, GUILayout.Width(70));
                            runAnim.Sound = RPGMakerGUI.AudioClipSelector("", runAnim.Sound, ref runAnim.SoundPath,
                                                                       GUILayout.Width(150));
                            runAnim.Backwards = RPGMakerGUI.Toggle(runAnim.Backwards);
                        }
                        else
                        {
                            GUILayout.Space(150);
                            runAnim.Sound = RPGMakerGUI.AudioClipSelector("", runAnim.Sound, ref runAnim.SoundPath,
                                                                       GUILayout.Width(150));
                        }
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        GUILayout.Space(5);
                    }


                }

                RPGMakerGUI.EndFoldout();
            }

            #endregion

            if (selectedCharInfo.CharacterType != CharacterType.NPC || (Rm_RPGHandler.Instance.Combat.NPCsCanFight && selectedNpc.CanFight))
            {
                #region AttackAnimations

                var defaultButtons = new List<string>() {"+Animation"};
                var wepAnimResult = RPGMakerGUI.FoldoutToolBar(ref wepFoldout, "Basic-Attack Animations",
                                                                defaultButtons.ToArray());
                if (wepFoldout)
                {
                    if (selectedCharInfo.AnimationType == RPGAnimationType.Legacy)
                    {
                        RPGMakerGUI.Toggle("Get Animations From Selected Object?", ref UseSelectedForAnims);
                    }

                    var listOfAnim = selectedCharInfo.LegacyAnimations.DefaultAttackAnimations;

                    GUILayout.BeginHorizontal("backgroundBox");
                    GUILayout.Label("ID", GUILayout.Width(100));

                    if (selectedCharInfo.AnimationType == RPGAnimationType.Legacy)
                    {
                        GUILayout.Label("Animation", GUILayout.Width(120));
                        GUILayout.Label("Speed", GUILayout.Width(70));
                        GUILayout.Label("Impact Time", GUILayout.Width(70));
                        GUILayout.Label("Sound", GUILayout.Width(150));
                        GUILayout.Label("Reverse?", GUILayout.Width(100));
                    }
                    else
                    {
                        GUILayout.Label("Anim Number", GUILayout.Width(120));
                        GUILayout.Label("Sound", GUILayout.Width(150));
                    }

                    
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    if (selectedCharInfo.AnimationType == RPGAnimationType.Legacy && foundAnim)
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
                    }
                    else
                    {
                        if (selectedCharInfo.AnimationType == RPGAnimationType.Legacy)
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
                            if(selectedCharInfo.AnimationType == RPGAnimationType.Legacy)
                            {
                                anim.Animation = RPGMakerGUI.TextField("", anim.Animation, GUILayout.Width(120));
                                anim.Speed = RPGMakerGUI.FloatField("", anim.Speed, GUILayout.Width(70));
                                anim.ImpactTime = RPGMakerGUI.FloatField("", anim.ImpactTime, GUILayout.Width(70));
                                anim.Sound = RPGMakerGUI.AudioClipSelector("", anim.Sound, ref anim.SoundPath,
                                                                            GUILayout.Width(150));
                                anim.Backwards = RPGMakerGUI.Toggle(anim.Backwards);
                            }
                            else
                            {
                                anim.MecanimAnimationNumber = RPGMakerGUI.IntField("", anim.MecanimAnimationNumber, GUILayout.Width(120));
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
                    }

                    if (wepAnimResult == 0)
                    {
                        var count = 0;
                        while (listOfAnim.FirstOrDefault(l => l.Name == "Attack " + count) != null)
                        {
                            count++;
                        }
                        selectedCharInfo.LegacyAnimations.DefaultAttackAnimations.Add(new AnimationDefinition()
                                                                                            {
                                                                                                Name = "Attack " + count,
                                                                                                RPGAnimationSet = RPGAnimationSet.DefaultAttack
                                                                                            });
                    }
                    RPGMakerGUI.EndFoldout();

                }
                #endregion

            }
           
            #endregion

        }

        private static List<AnimationDefinition> KeyAnimations(CombatCharacter selectedCharInfo)
        {
            //todo: remove or implement knockbacks and turn left/rights
            return new List<AnimationDefinition>()
                        {
                            selectedCharInfo.LegacyAnimations.WalkAnim,
                            selectedCharInfo.LegacyAnimations.RunAnim,
                            selectedCharInfo.LegacyAnimations.TurnRightAnim,
                            selectedCharInfo.LegacyAnimations.TurnLeftAnim,
                            selectedCharInfo.LegacyAnimations.IdleAnim,
                            selectedCharInfo.LegacyAnimations.CombatIdleAnim,
                            selectedCharInfo.LegacyAnimations.TakeHitAnim,
                            selectedCharInfo.LegacyAnimations.DeathAnim,
                            selectedCharInfo.LegacyAnimations.KnockBackAnim,
                            selectedCharInfo.LegacyAnimations.KnockUpAnim
                        };
        }

        public static void CombatStats(CombatCharacter selectedCharInfo)
        {
            var foundAnim = false;
            string[] anims = new string[0];
            if (animFoldout || wepFoldout)
            {
                var activeObj = Selection.activeObject;
                var selected = activeObj as GameObject;
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

            #region Combat Stats
            if (RPGMakerGUI.Foldout(ref showBonusDamage, "Damage"))
            {

                #region Damage

                var dmgList = selectedCharInfo.NpcDamage.ElementalDamages;

                GUILayout.BeginHorizontal();
                if (Rm_RPGHandler.Instance.Items.DamageHasVariance)
                {
                    RPGMakerGUI.Label("Physical Damage:");
                    selectedCharInfo.NpcDamage.MinDamage = RPGMakerGUI.IntField("", selectedCharInfo.NpcDamage.MinDamage);
                    GUILayout.Label(" - ");
                    selectedCharInfo.NpcDamage.MaxDamage = RPGMakerGUI.IntField("", selectedCharInfo.NpcDamage.MaxDamage);
                }
                else
                {
                    selectedCharInfo.NpcDamage.MaxDamage = RPGMakerGUI.IntField("Physical Damage:", selectedCharInfo.NpcDamage.MaxDamage);
                }
                GUILayout.EndHorizontal();

                foreach (var eleDmg in dmgList)
                {
                    var nameOfEleDmg =
                        Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions.First(
                            e => e.ID == eleDmg.ElementID).Name;

                    GUILayout.BeginHorizontal();
                    if (Rm_RPGHandler.Instance.Items.DamageHasVariance)
                    {
                        RPGMakerGUI.Label(nameOfEleDmg + " Damage:");

                        eleDmg.MinDamage = RPGMakerGUI.IntField("", eleDmg.MinDamage);
                        GUILayout.Label(" - ");
                        eleDmg.MaxDamage = RPGMakerGUI.IntField("", eleDmg.MaxDamage);

                    }
                    else
                    {
                        eleDmg.MaxDamage = RPGMakerGUI.IntField(nameOfEleDmg + " Damage:", eleDmg.MaxDamage);
                    }
                    GUILayout.EndHorizontal();
                }

                #endregion


                RPGMakerGUI.EndFoldout();
            }
            #endregion

            #region Attributes

            RPGMakerGUI.Foldout(ref baseAttrFoldout, "Base Attributes");
            if (baseAttrFoldout)
            {
                if (Rm_RPGHandler.Instance.ASVT.AttributesDefinitions.Count > 0)
                {
                    foreach (var v in selectedCharInfo.Attributes)
                    {
                        var prefix =
                            Rm_RPGHandler.Instance.ASVT.AttributesDefinitions.First(a => a.ID == v.ID).
                                Name;
                        v.BaseValue = RPGMakerGUI.IntField(prefix, v.BaseValue);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("No defined attributes.", MessageType.Info);
                }

            }
            if (baseAttrFoldout) RPGMakerGUI.EndFoldout();

            #endregion

            #region Vitals

            RPGMakerGUI.Foldout(ref baseVitFoldout, "Base Vitals");
            if (baseVitFoldout)
            {
                if (Rm_RPGHandler.Instance.ASVT.VitalDefinitions.Count > 0)
                {
                    foreach (var v in selectedCharInfo.Vitals)
                    {
                        var prefix =
                            Rm_RPGHandler.Instance.ASVT.VitalDefinitions.First(a => a.ID == v.ID).Name;
                        v.BaseValue = RPGMakerGUI.IntField(prefix, v.BaseValue);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("No defined vitals.", MessageType.Info);
                }

            }
            if (baseVitFoldout) RPGMakerGUI.EndFoldout();

            #endregion

            #region Statistics

            RPGMakerGUI.Foldout(ref baseStatFoldout, "Base Statistics");
            if (baseStatFoldout)
            {
                if (Rm_RPGHandler.Instance.ASVT.StatisticDefinitions.Count > 0)
                {
                    foreach (var v in selectedCharInfo.Stats)
                    {
                        var prefix =
                            Rm_RPGHandler.Instance.ASVT.StatisticDefinitions.First(a => a.ID == v.ID).Name;
                        v.BaseValue = RPGMakerGUI.FloatField(prefix, v.BaseValue);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("No defined statistics.", MessageType.Info);
                }

            }
            if (baseStatFoldout) RPGMakerGUI.EndFoldout();

            #endregion

            #region Skills

            var result = RPGMakerGUI.FoldoutToolBar(ref startingSkillsFoldout, "Skills",
                                                    new string[] { "+Skill" });
            if (startingSkillsFoldout)
            {
                
                var allSkills = Rm_RPGHandler.Instance.Repositories.Skills.AllSkills;
                if (allSkills.Count > 0)
                {
                    if (selectedCharInfo.EnemySkills.Count == 0)
                    {
                        EditorGUILayout.HelpBox("Click +Skill to add a skill this class starts with.",
                                                MessageType.Info);
                    }

                    GUILayout.Space(5);
                    for (int index = 0; index < selectedCharInfo.EnemySkills.Count; index++)
                    {
                        GUILayout.BeginVertical("backgroundBox");
                        GUILayout.BeginHorizontal();



                        var enemySkill = selectedCharInfo.EnemySkills[index];

                        RPGMakerGUI.PopupID<Skill>("Skill Name:", ref enemySkill.SkillID);
                        var skill = Rm_RPGHandler.Instance.Repositories.Skills.Get(enemySkill.SkillID);
                        enemySkill.Rank = EditorGUILayout.IntSlider("Rank", enemySkill.Rank, 0, skill.MaxRank - 1);
                        enemySkill.RequireResource = RPGMakerGUI.Toggle("Require Resource?",
                                                                            enemySkill.RequireResource);
                        enemySkill.Priority = RPGMakerGUI.IntField("Cast Priority?",
                                                                            enemySkill.Priority);

                        if (GUILayout.Button(RPGMakerGUI.DelIcon, "genericButton", GUILayout.Width(30),
                                             GUILayout.Height(30)))
                        {
                            selectedCharInfo.EnemySkills.Remove(selectedCharInfo.EnemySkills[index]);
                            index--;
                            continue;
                        }

                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();

                        enemySkill.CastType =
                            (Rm_EnemySkillCastType)RPGMakerGUI.EnumPopup(enemySkill.CastType);

                        if (enemySkill.CastType != Rm_EnemySkillCastType.WhenOffCooldown)
                        {
                            if (enemySkill.CastType == Rm_EnemySkillCastType.EveryNthSeconds)
                            {
                                enemySkill.Paramater = EditorGUILayout.FloatField("N:", enemySkill.Paramater);
                            }
                            else
                            {
                                enemySkill.Paramater = EditorGUILayout.IntField("N:", (int)enemySkill.Paramater);
                            }
                        }

                        GUILayout.EndHorizontal();


                        #region NPC Skill Anims

                        var actualSkill = Rm_RPGHandler.Instance.Repositories.Skills.AllSkills.First(s => s.ID == enemySkill.SkillID);
                        if (foundAnim)
                        {
                            selectedAnim[index] = anims.FirstOrDefault(s => s == enemySkill.AnimationToUse.LegacyAnim) != null
                                                      ? Array.IndexOf(anims, enemySkill.AnimationToUse.LegacyAnim)
                                                      : 0;

                            selectedAnim[index] = EditorGUILayout.Popup("Legacy Cast Animation:", selectedAnim[index], anims);
                            enemySkill.AnimationToUse.LegacyAnim = anims[selectedAnim[index]];
                            if (enemySkill.AnimationToUse.LegacyAnim == "None") enemySkill.AnimationToUse.LegacyAnim = "";

                            selectedAnim[index] = anims.FirstOrDefault(s => s == enemySkill.AnimationToUse.CastingLegacyAnim) != null
                                                      ? Array.IndexOf(anims, enemySkill.AnimationToUse.CastingLegacyAnim)
                                                      : 0;

                            selectedAnim[index] = EditorGUILayout.Popup("Legacy Casting Animation:", selectedAnim[index], anims);
                            enemySkill.AnimationToUse.CastingLegacyAnim = anims[selectedAnim[index]];
                            if (enemySkill.AnimationToUse.CastingLegacyAnim == "None") enemySkill.AnimationToUse.CastingLegacyAnim = "";

                            if (actualSkill.MovementType != SkillMovementType.StayInPlace)
                            {
                                selectedAnim[index] = anims.FirstOrDefault(s => s == enemySkill.AnimationToUse.ApproachLegacyAnim) != null
                                                          ? Array.IndexOf(anims, enemySkill.AnimationToUse.ApproachLegacyAnim)
                                                          : 0;

                                selectedAnim[index] = EditorGUILayout.Popup("Legacy Move Animation:", selectedAnim[index], anims);
                                enemySkill.AnimationToUse.ApproachLegacyAnim = anims[selectedAnim[index]];
                                if (enemySkill.AnimationToUse.ApproachLegacyAnim == "None") enemySkill.AnimationToUse.ApproachLegacyAnim = "";

                                selectedAnim[index] = anims.FirstOrDefault(s => s == enemySkill.AnimationToUse.LandLegacyAnim) != null
                                                          ? Array.IndexOf(anims, enemySkill.AnimationToUse.LandLegacyAnim)
                                                          : 0;

                                selectedAnim[index] = EditorGUILayout.Popup("Legacy Land Animation:", selectedAnim[index], anims);
                                enemySkill.AnimationToUse.LandLegacyAnim = anims[selectedAnim[index]];
                                if (enemySkill.AnimationToUse.LandLegacyAnim == "None") enemySkill.AnimationToUse.LandLegacyAnim = "";
                            }
                        }
                        else
                        {
                            enemySkill.AnimationToUse.LegacyAnim = RPGMakerGUI.TextField("Legacy Cast Animation Int:", enemySkill.AnimationToUse.LegacyAnim);
                            enemySkill.AnimationToUse.CastingLegacyAnim = RPGMakerGUI.TextField("Legacy Casting Animation Int:", enemySkill.AnimationToUse.CastingLegacyAnim);

                            if (actualSkill.MovementType != SkillMovementType.StayInPlace)
                            {
                                enemySkill.AnimationToUse.ApproachLegacyAnim = RPGMakerGUI.TextField("Legacy Move Animation Int:", enemySkill.AnimationToUse.ApproachLegacyAnim);
                                enemySkill.AnimationToUse.LandLegacyAnim = RPGMakerGUI.TextField("Legacy Land Animation Int:", enemySkill.AnimationToUse.LandLegacyAnim);
                            }
                        }

                        #endregion


                        GUILayout.EndVertical();

                        GUILayout.Space(5);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("No Skills Found.", MessageType.Info);
                    selectedCharInfo.EnemySkills = new List<Rm_NPCSkill>();
                }

                if (result == 0)
                {
                    selectedCharInfo.EnemySkills.Add(new Rm_NPCSkill());
                }
            }
            if (startingSkillsFoldout) RPGMakerGUI.EndFoldout();

            #endregion

            #region SkillImmunities

            var immunityResult = RPGMakerGUI.FoldoutToolBar(ref skillImmunFoldout, "Skill Immunities",
                                                            new string[] { "+Immunity" });
            if (skillImmunFoldout)
            {
                var allSkillMetas = Rm_RPGHandler.Instance.Combat.SkillMeta;
                if (allSkillMetas.Count > 0)
                {
                    if (selectedCharInfo.SkillMetaImmunitiesID.Count == 0)
                    {
                        EditorGUILayout.HelpBox("Click +Immunity to add a skill meta this class is immune to.",
                                                MessageType.Info);
                    }

                    GUILayout.Space(5);
                    for (int index = 0; index < selectedCharInfo.SkillMetaImmunitiesID.Count; index++)
                    {
                        GUILayout.BeginHorizontal();
                        var skillImmunity = selectedCharInfo.SkillMetaImmunitiesID[index];
                        RPGMakerGUI.PopupID<SkillMetaDefinition>("Immunity:", ref skillImmunity.ID);

                        if (GUILayout.Button(RPGMakerGUI.DelIcon, "genericButton", GUILayout.Width(30),
                                             GUILayout.Height(30)))
                        {
                            selectedCharInfo.SkillMetaImmunitiesID.Remove(
                                selectedCharInfo.SkillMetaImmunitiesID[index]);
                            index--;
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.Space(5);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("No Skill Metas Found.", MessageType.Info);
                    selectedCharInfo.SkillMetaImmunitiesID = new List<SkillImmunity>();
                }

                if (immunityResult == 0)
                {
                    selectedCharInfo.SkillMetaImmunitiesID.Add(new SkillImmunity() { ID = "" });
                }
                RPGMakerGUI.EndFoldout();
            }

            #endregion

            #region SkillSusceptibility
            var susceptResult = RPGMakerGUI.FoldoutToolBar(ref skillSusFoldout, "Skill Susceptibilities", new string[] { "+Susceptibility" });
            if (skillSusFoldout)
            {
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
                var allSkillMetas = Rm_RPGHandler.Instance.Combat.SkillMeta;
                if (allSkillMetas.Count > 0)
                {
                    if (selectedCharInfo.SkillMetaSusceptibilities.Count == 0)
                    {
                        EditorGUILayout.HelpBox("Click +Susceptibility to add a skill meta this class takes more damage from.", MessageType.Info);
                    }

                    GUILayout.Space(5);
                    for (int index = 0; index < selectedCharInfo.SkillMetaSusceptibilities.Count; index++)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.BeginHorizontal();
                        RPGMakerGUI.PopupID<SkillMetaDefinition>("Susceptibility", ref selectedCharInfo.SkillMetaSusceptibilities[index].ID);

                        var selectedSus = selectedCharInfo.SkillMetaSusceptibilities[index];
                        selectedSus.AdditionalDamage = RPGMakerGUI.FloatField("Additional Damage:", selectedSus.AdditionalDamage);
                        GUILayout.EndHorizontal();

                        if (GUILayout.Button(RPGMakerGUI.DelIcon, "genericButton", GUILayout.Width(30), GUILayout.Height(30)))
                        {
                            selectedCharInfo.SkillMetaSusceptibilities.Remove(selectedCharInfo.SkillMetaSusceptibilities[index]);
                            index--;
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.Space(5);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("No Skill Metas Found.", MessageType.Info);
                    selectedCharInfo.SkillMetaSusceptibilities = new List<SkillMetaSusceptibility>();
                }

                if (susceptResult == 0)
                {
                    selectedCharInfo.SkillMetaSusceptibilities.Add(new SkillMetaSusceptibility());
                }
            }
            if (skillSusFoldout) RPGMakerGUI.EndFoldout();
            #endregion

        }
        public static void Loot(CombatCharacter selectedCharInfo)
        {
            #region LootTables

            var lootTableResult = RPGMakerGUI.FoldoutToolBar(ref lootTableFoldout, "Loot Tables",
                                                             new string[] { "+LootTable" });


            if (lootTableFoldout)
            {
                selectedCharInfo.MaxItemsFromLootTable = EditorGUILayout.IntField("Max Item Drop:",
                                                                                  selectedCharInfo.
                                                                                      MaxItemsFromLootTable);

                
                var allItems = Rm_RPGHandler.Instance.Repositories.LootTables.AllTables;
                if (allItems.Count > 0)
                {
                    if (selectedCharInfo.LootTables.Count == 0)
                    {
                        EditorGUILayout.HelpBox(
                            "Click +LootTable to add a loot table that the enemy can drop items from.",
                            MessageType.Info);
                    }

                    GUILayout.Space(5);
                    for (int index = 0; index < selectedCharInfo.LootTables.Count; index++)
                    {
                        var lootOption = selectedCharInfo.LootTables[index];

                        GUILayout.BeginVertical("backgroundBox");
                        GUILayout.BeginHorizontal();
                        RPGMakerGUI.PopupID<Rm_LootTable>("Loot Table:", ref lootOption.LootTableID);
                        lootOption.AlwaysGetItem = RPGMakerGUI.Toggle("Always Get Item?",
                                                                          lootOption.AlwaysGetItem);
                        if (GUILayout.Button(RPGMakerGUI.DelIcon, "genericButton", GUILayout.Width(30),
                                             GUILayout.Height(30)))
                        {
                            selectedCharInfo.LootTables.Remove(selectedCharInfo.LootTables[index]);
                            index--;
                            continue;
                        }
                        GUILayout.EndHorizontal();

                        #region AddToHundred

                        if (index == 0)
                        {
                            lootOption.sliderMin = 0;

                            if (selectedCharInfo.LootTables.Count == 1) lootOption.sliderMax = 100;

                            GUILayout.BeginHorizontal();
                            RPGMakerGUI.MinMaxSlider(new GUIContent("Chance To Use This:"), ref lootOption.sliderMin, ref lootOption.sliderMax, 0, 100);

                            lootOption.sliderMin = (int)lootOption.sliderMin;
                            lootOption.sliderMax = (int)lootOption.sliderMax;

                            lootOption.Chance = lootOption.sliderMax - lootOption.sliderMin;
                            GUILayout.Label(lootOption.Chance + "%");
                            GUILayout.EndHorizontal();
                        }
                        else if (index - 1 >= 0)
                        {
                            var prevTable = selectedCharInfo.LootTables[index - 1];
                            lootOption.sliderMin = prevTable.sliderMax;
                            if (lootOption.sliderMax < lootOption.sliderMin)
                            {
                                lootOption.sliderMax = lootOption.sliderMin;
                            }
                            if (index == selectedCharInfo.LootTables.Count - 1)
                            {
                                lootOption.sliderMax = 100;
                            }
                            GUILayout.BeginHorizontal();
                            RPGMakerGUI.MinMaxSlider(new GUIContent("Chance To Use This:"), ref lootOption.sliderMin, ref lootOption.sliderMax, 0, 100);

                            lootOption.sliderMin = (int)lootOption.sliderMin;
                            lootOption.sliderMax = (int)lootOption.sliderMax;
                            lootOption.Chance = lootOption.sliderMax - lootOption.sliderMin;
                            GUILayout.Label(lootOption.Chance + "%");
                            GUILayout.EndHorizontal();
                        }

                        #endregion

                        GUILayout.EndVertical();
                        GUILayout.Space(5);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("No loot tables found.", MessageType.Info);
                    selectedCharInfo.LootTables = new List<LootOptions>();
                }

                if (lootTableResult == 0)
                {
                    selectedCharInfo.LootTables.Add(new LootOptions());
                }
            }
            if (lootTableFoldout) RPGMakerGUI.EndFoldout();

            #endregion

            #region GuaranteedLoot
            var gurResult = RPGMakerGUI.FoldoutToolBar(ref gurLootFoldout, "Guaranteed Loot and Gold", new string[] { "+Loot", "+Gold" });
            if (gurLootFoldout)
            {
                if (selectedCharInfo.DropsGold)
                {
                    GUILayout.BeginHorizontal();
                    RPGMakerGUI.Label("Gold Drop:");
                    selectedCharInfo.MinGoldDrop = RPGMakerGUI.IntField("", selectedCharInfo.MinGoldDrop, GUILayout.Width(80));
                    GUILayout.Label("-");
                    selectedCharInfo.MaxGoldDrop = RPGMakerGUI.IntField("", selectedCharInfo.MaxGoldDrop, GUILayout.Width(80));
                    selectedCharInfo.GoldDropChance = EditorGUILayout.Slider("Chance:", selectedCharInfo.GoldDropChance, 0.0f, 1.0f);
                    GUILayout.Label((selectedCharInfo.GoldDropChance * 100) + "%");

                    if (RPGMakerGUI.DeleteButton(15))
                    {
                        selectedCharInfo.DropsGold = false;
                    }
                    GUILayout.EndHorizontal();
                }

                var allItems = Rm_RPGHandler.Instance.Repositories.Items.AllItems;
                if (allItems.Count > 0)
                {
                    if (selectedCharInfo.GuaranteedLoot.Count == 0)
                    {
                        EditorGUILayout.HelpBox("Click +Loot to add loot that will always drop from this enemy. Click +Gold to add gold drops.", MessageType.Info);
                    }

                    GUILayout.Space(5);



                    for (int index = 0; index < selectedCharInfo.GuaranteedLoot.Count; index++)
                    {
                        GUILayout.BeginHorizontal();
                        var selectedSus = selectedCharInfo.GuaranteedLoot[index];
                        RPGMakerGUI.PopupID<Item>("Item:", ref selectedSus.ItemID);

                        var stackable = allItems.FirstOrDefault(i => i.ID == selectedSus.ItemID) as IStackable;
                        if (stackable != null)
                        {
                            selectedSus.Amount = RPGMakerGUI.IntField("Amount:", selectedSus.Amount);
                        }
                        if (GUILayout.Button(RPGMakerGUI.DelIcon, "genericButton", GUILayout.Width(30), GUILayout.Height(30)))
                        {
                            selectedCharInfo.GuaranteedLoot.Remove(selectedCharInfo.GuaranteedLoot[index]);
                            index--;
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.Space(5);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("No Items Found.", MessageType.Info);
                    selectedCharInfo.GuaranteedLoot = new List<LootDefinition>();
                }

                if (gurResult == 0)
                {
                    selectedCharInfo.GuaranteedLoot.Add(new LootDefinition());
                }
                else if (gurResult == 1)
                {
                    selectedCharInfo.DropsGold = true;
                }

                RPGMakerGUI.EndFoldout();
            }
            #endregion
        }

        public static void Projectile(CombatCharacter selectedCharInfo)
        {

            #region ProjectileDetails

            
                if (RPGMakerGUI.Foldout(ref projectileInfoFoldout, "Auto Attack Details"))
                {

                    GUILayout.BeginHorizontal();
                    var isMelee = selectedCharInfo.AttackStyle == AttackStyle.Melee;
                    var autoAttackType = isMelee ? "Melee Effect Prefab:" : "Projectile Prefab:";
                    selectedPrefab = RPGMakerGUI.PrefabSelector(autoAttackType, selectedPrefab,
                                                                        ref selectedCharInfo.AutoAttackPrefabPath);
                    selectedPrefab = RPGMakerGUI.PrefabGeneratorButton(isMelee ? Rmh_PrefabType.Melee_Effect : Rmh_PrefabType.Auto_Attack_Projectile, selectedPrefab, ref selectedCharInfo.AutoAttackPrefabPath);
                    GUILayout.EndHorizontal();


                    if (selectedCharInfo.AttackStyle == AttackStyle.Ranged)
                    {
                        selectedCharInfo.ProjectileSpeed = RPGMakerGUI.FloatField("Projectile Speed:", selectedCharInfo.ProjectileSpeed);

                        selectedCharInfo.ProjectileTravelSound.Audio = RPGMakerGUI.AudioClipSelector("Projectile Travel Sound:", selectedCharInfo.ProjectileTravelSound.Audio, ref selectedCharInfo.ProjectileTravelSound.AudioPath);
                    }


                    GUILayout.BeginHorizontal();
                    selectedPrefab = RPGMakerGUI.PrefabSelector("Impact Prefab:", selectedPrefab,
                                                                        ref selectedCharInfo.AutoAttackImpactPrefabPath);
                    selectedPrefab = RPGMakerGUI.PrefabGeneratorButton(Rmh_PrefabType.Impact, selectedPrefab, ref selectedCharInfo.AutoAttackImpactPrefabPath);
                    GUILayout.EndHorizontal();


                    selectedCharInfo.AutoAttackImpactSound.Audio = RPGMakerGUI.AudioClipSelector("Impact Sound:", selectedCharInfo.AutoAttackImpactSound.Audio, ref selectedCharInfo.AutoAttackImpactSound.AudioPath);


                    RPGMakerGUI.EndFoldout();
                }
            

            #endregion

        }
    }
}

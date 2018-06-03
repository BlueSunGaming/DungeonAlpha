using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using LogicSpawn.RPGMaker.Editor.New;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LogicSpawn.RPGMaker.Editor
{
    public static class Rme_Main_Combat
    {

        public static Rmh_Combat Combat
        {
            get { return Rm_RPGHandler.Instance.Combat; }
        }
        public static Rmh_ASVT ASVT
        {
            get { return Rm_RPGHandler.Instance.ASVT; }
        }

        public static EditorWindow Window;

        public static bool showSkillTypeNames = true;
        public static bool showSkillMetaNames = true;
        public static bool showDamageDefinitions = true;
        public static bool showOptions = true;
        public static bool UseSelectedForAnims = false;
        public static bool showMaterialChangeFoldout = true;
        public static Vector2 optionsScrollPos = Vector2.zero;
        public static GameObject selectedPrefab;
        public static void Options(Rect fullArea, Rect leftArea, Rect mainArea, EditorWindow main)
        {
            Window = main;
            GUI.Box(fullArea, "", "backgroundBox");

            GUILayout.BeginArea(fullArea);
            optionsScrollPos = GUILayout.BeginScrollView(optionsScrollPos);
            RPGMakerGUI.Title("Combat - Options");

            if (RPGMakerGUI.Foldout(ref showOptions, "Options"))
            {
                Rm_RPGHandler.Instance.Items.DamageHasVariance = RPGMakerGUI.Toggle("Damage has variance?", Rm_RPGHandler.Instance.Items.DamageHasVariance);
                Combat.TargetStyle = (TargetStyle)RPGMakerGUI.EnumPopup("Target Style:", Combat.TargetStyle);
                Combat.SmartCastSkills = RPGMakerGUI.Toggle("Smart-cast skills?", Combat.SmartCastSkills);
                if (Combat.TargetStyle == TargetStyle.TargetLock)
                {
                    Combat.ShowSelected = RPGMakerGUI.Toggle("Show Selected with Texture?", Combat.ShowSelected);
                    if (Combat.ShowSelected)
                    {
                        Combat.ShowSelectedWithTexture = RPGMakerGUI.Toggle("Texture / Prefab?",
                                                                                Combat.ShowSelectedWithTexture);

                        if (Combat.ShowSelectedWithTexture)
                        {
                            Combat.SelectedTexture.Image = RPGMakerGUI.ImageSelector("Selected Texture:", Combat.SelectedTexture.Image,
                                                                                     ref
                                                                                         Combat.SelectedTexture.
                                                                                         ImagePath);
                            Combat.SelectedCombatTexture.Image = RPGMakerGUI.ImageSelector("Selected (Combat) Texture:", Combat.SelectedCombatTexture.Image,
                                                                                     ref
                                                                                         Combat.SelectedCombatTexture.
                                                                                         ImagePath);
                        }
                        else
                        {
                            selectedPrefab = RPGMakerGUI.PrefabSelector("Selected Prefab:", selectedPrefab,
                                                                        ref Combat.SelectedPrefabPath);
                            gameObject = RPGMakerGUI.PrefabGeneratorButton(Rmh_PrefabType.Target_Selected_Prefab, gameObject, ref Combat.SelectedPrefabPath);

                        }

                        Combat.SelectedYOffSet = RPGMakerGUI.FloatField("Selected Marker Y Offset:",
                                                                              Combat.SelectedYOffSet);
                    }
                }

                Combat.ShowCastArea = RPGMakerGUI.Toggle("Show Cast-Area with Texture?", Combat.ShowCastArea);
                if (Combat.ShowCastArea)
                {
                    
                        Combat.CastAreaTexture.Image = RPGMakerGUI.ImageSelector("Cast-Area Texture:", Combat.CastAreaTexture.Image,
                                                                                 ref
                                                                                         Combat.CastAreaTexture.
                                                                                     ImagePath);
                    try
                    {
                        Combat.CastAreaColor = EditorGUILayout.ColorField("Cast-Area Color:", Combat.CastAreaColor);    
                    }
                    catch {}
                    
                }
                Combat.ScaleSkillMoveByCast = RPGMakerGUI.Toggle("Scale Skill Movement by Cast Spd.?", Combat.ScaleSkillMoveByCast);

                Combat.EnableFallDamage = RPGMakerGUI.Toggle("Enable Fall Damage:", Combat.EnableFallDamage);
                Combat.EnableTauntSystem = RPGMakerGUI.Toggle("Enable Taunt System:", Combat.EnableTauntSystem);
                if(RPGMakerGUI.Toggle("Enable Floating Combat Text:", ref Combat.EnableFloatingCombatText))
                {
                    Combat.FloatDuration = RPGMakerGUI.FloatField("- Float Duration:", Combat.FloatDuration);
                    Combat.FloatSpeed = RPGMakerGUI.FloatField("- Float Speed:", Combat.FloatSpeed);
                    Combat.FloatDistBetweenVals = RPGMakerGUI.FloatField("- Distance between values (Y):", Combat.FloatDistBetweenVals);
                }

                Combat.AggroRadius = RPGMakerGUI.FloatField("Enemy Aggro Radius:", Combat.AggroRadius);

                Combat.NPCsCanFight = RPGMakerGUI.Toggle("NPCs Fight:", Combat.NPCsCanFight);
                if (Combat.NPCsCanFight)
                {
                    Combat.CanAttackNPcs = RPGMakerGUI.Toggle("Can Attack NPCs:", Combat.CanAttackNPcs);
                    if (Combat.CanAttackNPcs)
                    {
                        Combat.CanAttackUnkillableNPCs = RPGMakerGUI.Toggle("- Can Attack Un-Killable NPCs:", Combat.CanAttackUnkillableNPCs);
                    }
                }
                Combat.AllowItemsOnBar = RPGMakerGUI.Toggle("Allow Items on Skill Bar?", Combat.AllowItemsOnBar);
                Combat.SkillBarSlots = RPGMakerGUI.IntField("Slots of SkillBar:", Combat.SkillBarSlots);
                if (Combat.SkillBarSlots > 12) Combat.SkillBarSlots = 12;

                RPGMakerGUI.Toggle("Skip evaluating damage with null attacker?", ref Combat.SkipEvaluatingDmgWithNullAttack);

                RPGMakerGUI.Help(Combat.SkipEvaluatingDmgWithNullAttack 
                    ? "When there is no attacker, e.g. if dealing damage from the deal damage nodes, damage will not be recalculated through "
                    : "When there is no attacker, e.g. if dealing damage from the deal damage nodes, damage will still be calculated through" , 1);

                RPGMakerGUI.Help(Combat.SkipEvaluatingDmgWithNullAttack 
                    ? "the damage taken tree and will be dealt as is."
                    : "the damage taken tree but will use a dummy combatant with all default stats (attributes, vitals and statistics).", 1);
                
                RPGMakerGUI.EndFoldout();
            }

            if (RPGMakerGUI.Foldout(ref showSkillTypeNames, "Skill Type Names"))
            {
                for (int index = 0; index < Combat.SkillTypeNames.Count; index++)
                {
                    var itemTypeName = Combat.SkillTypeNames[index];
                    GUILayout.BeginHorizontal();
                    var itemType = itemTypeName.SkillType.ToString().Replace("_", " ");
                    itemTypeName.Name = RPGMakerGUI.TextField(itemType, itemTypeName.Name);
                    GUILayout.EndHorizontal();
                }

                RPGMakerGUI.EndFoldout();
            }

            var metaResult = RPGMakerGUI.FoldoutToolBar(ref showSkillMetaNames, "Skill Meta Definitions",
                                                        "+Meta");
            if (showSkillMetaNames)
            {
                Combat.MetaAppliesToHealing = RPGMakerGUI.Toggle("Skill Metas Apply To Healing?", Combat.MetaAppliesToHealing);

                for (int index = 0; index < Combat.SkillMeta.Count; index++)
                {
                    var def = Combat.SkillMeta[index];
                    GUILayout.BeginHorizontal();
                    def.Name = RPGMakerGUI.TextField("Meta Name:", def.Name);
                    if (RPGMakerGUI.DeleteButton())
                    {
                        Combat.SkillMeta.Remove(def);
                        index--;
                    }
                    GUILayout.EndHorizontal();
                }

                if (metaResult == 0)
                {
                    Combat.SkillMeta.Add(new SkillMetaDefinition());
                }

                if (Combat.SkillMeta.Count == 0)
                {
                    EditorGUILayout.HelpBox(
                        "Click +Meta to add a Meta - a catergorie of skill which combatants can be immune/susceptible to.",
                        MessageType.Info);
                }
                RPGMakerGUI.EndFoldout();
            }

                var result = RPGMakerGUI.FoldoutToolBar(ref showDamageDefinitions, "Damage Definitions",
                                                        "+DamageType");
                if (showDamageDefinitions)
                {
                    for (int index = 0; index < ASVT.ElementalDamageDefinitions.Count; index++)
                    {
                        var def = ASVT.ElementalDamageDefinitions[index];
                        GUILayout.BeginHorizontal();
                        def.Name = RPGMakerGUI.TextField("Damage Name:", def.Name);
                        def.Color = (Rm_UnityColors)RPGMakerGUI.EnumPopup("Color:", def.Color);
                        if (RPGMakerGUI.DeleteButton())
                        {
                            ASVT.ElementalDamageDefinitions.Remove(def);
                            index--;
                        }
                        GUILayout.EndHorizontal();
                    }

                    if (result == 0)
                    {
                        ASVT.ElementalDamageDefinitions.Add(new ElementalDamageDefinition());
                    }

                    if (ASVT.ElementalDamageDefinitions.Count == 0)
                    {
                        EditorGUILayout.HelpBox(
                            "You can define additional damage types to use in damage calculations and defend against here.",
                            MessageType.Info);
                    }
                    RPGMakerGUI.EndFoldout();
                }

            var foldOutResult = RPGMakerGUI.FoldoutToolBar(ref showMaterialChangeFoldout, "Shader Lerp On Combatant Death", "+MaterialToLerp");
            if (showMaterialChangeFoldout)
            {

                for (var index = 0; index < Combat.ShadersToLerp.Count; index++)
                {
                    var shaderToLerp = Combat.ShadersToLerp[index];
                    
                    GUILayout.BeginHorizontal();
                    RPGMakerGUI.ShaderSelector("Shader:", shaderToLerp.ShaderName, main);
                    if(RPGMakerGUI.DeleteButton(20))
                    {
                        Combat.ShadersToLerp.RemoveAt(index);
                        index--;
                        continue;
                    }

                    GUILayout.EndHorizontal();
                    for (int i = 0; i < shaderToLerp.PropsToLerp.Count; i++)
                    {
                        var prop = shaderToLerp.PropsToLerp[i];

                        if (RPGMakerGUI.Toggle("Lerp \"" + prop.PropDesc + "\" + [" + prop.PropName + "] ? ", ref prop.LerpThisProperty))
                        {
                            if (prop.PropType == ShaderType.Color)
                            {
                                if (RPGMakerGUI.Toggle("Only Lerp Alpha?", 1, ref prop.OnlyLerpAlpha))
                                {
                                    prop.LerpTo = RPGMakerGUI.FloatField("Lerp Alpha To [0 - 1]", prop.LerpTo, 1);
                                }
                                else
                                {
                                    try {
                                        prop.LerpToColor = EditorGUILayout.ColorField("-- To", prop.LerpToColor);
                                    }
                                    catch {}
                                }
                            }
                            else
                            {
                                prop.LerpTo = RPGMakerGUI.FloatField("To", prop.LerpTo, 1);
                            }
                        }
                    }

                    GUILayout.Space(15);
                }

                if(foldOutResult == 0)
                {
                    Combat.ShadersToLerp.Add(new ShaderLerpInfo());
                }

                RPGMakerGUI.EndFoldout();
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        

        private static string Shadername;
        #region Skills

        private static Skill selectedSkill;

        private static List<Skill> listOfSkills
        {
            get { return Rm_RPGHandler.Instance.Repositories.Skills.AllSkills; }
        }

        public static int selectedSkillMeta;
        public static int selectedItemFilter;
        public static int selectedResource;
        public static int[] selectedClassIndex = new int[999];
        public static int[] selectedAnim = new int[999];
        public static int[] selectedAttributeBuff = new int[999];
        public static int[] selectedStatisticBuff = new int[999];
        public static int[] selectedVitalBuff = new int[999];
        public static bool showSkillDetails = true;
        public static bool showSkillClasses = true;
        public static bool showCustomVarSetters = true;
        public static bool showCustomVarSettersOnDisable = true;
        public static bool showStatusReduction = true;
        public static bool showRankDetails = true;
        public static bool showAttrBuffs = true;
        public static bool showVitBuffs = true;
        public static bool showStatBuffs = true;

        public static bool showVitalRegens = true;
        public static bool showAnimationInfo = true;
        public static bool showTooltip = true;

        public static bool showAuraAttrBuffs = true;

        public static GameObject gameObject;
        public static GameObject castGameObject;
        public static Vector2 skillScrollPos = Vector2.zero;
        private static string[] skillTypesFilter = new[]
                                                      {
                                                          "All","Area_Of_Effect", "Projectile", "Aura", "Spawn","Restoration","Ability", "Melee"
                                                      };

        public static void Skills(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            GUI.Box(leftArea, "", "backgroundBox");
            GUI.Box(mainArea, "", "backgroundBoxMain");

            GUILayout.BeginArea(PadRect(leftArea, 0, 0));

            var filteredSkillsList = listOfSkills;

            selectedItemFilter = EditorGUILayout.Popup(selectedItemFilter, skillTypesFilter.Select(i => "Filter: " + i).ToArray(), "filterButton",
                                                           GUILayout.Height(25));

            if (selectedItemFilter != 0)
            {
                var skilltype = skillTypesFilter[selectedItemFilter];
                filteredSkillsList = filteredSkillsList.Where(i => i.SkillType.ToString() == skilltype).ToList();
            }

            var rect = RPGMakerGUI.ListArea(filteredSkillsList, ref selectedSkill, Rm_ListAreaType.Skills, true, true);
            var evt = Event.current;
            if (evt.type == EventType.MouseDown && !Rme_Main.ShowSubMenu)
            {
                var mousePos = evt.mousePosition;

                if (rect.Contains(mousePos))
                {
                    // Now create the menu, add items and show it
                    var menu = new GenericMenu();

                    foreach (var itemType in Enum.GetValues(typeof(SkillType)))
                    {
                        menu.AddItem(new GUIContent(itemType.ToString()), false, AddSkill(), itemType);
                    }

                    menu.ShowAsContext();
                    evt.Use();
                }
            }
            GUILayout.EndArea();


            GUILayout.BeginArea(mainArea);
            skillScrollPos = GUILayout.BeginScrollView(skillScrollPos);
            RPGMakerGUI.Title("Skills");
            if (selectedSkill != null)
            {


                if (selectedSkill.CurrentRank >= selectedSkill.MaxRank)
                    selectedSkill.CurrentRank = selectedSkill.MaxRank - 1;


                #region Update skill rank details

                while (selectedSkill.SkillStatistics.Count > selectedSkill.MaxRank)
                {
                    selectedSkill.SkillStatistics.RemoveAt(selectedSkill.SkillStatistics.Count - 1);
                }

                while (selectedSkill.SkillStatistics.Count < selectedSkill.MaxRank)
                {
                    if (selectedSkill.SkillStatistics.Count > 0)
                    {
                        var copyOfLast =
                            GeneralMethods.CopyObject(
                                selectedSkill.SkillStatistics[selectedSkill.SkillStatistics.Count - 1]);
                        selectedSkill.SkillStatistics.Add(copyOfLast);
                    }
                    else
                    {
                        selectedSkill.SkillStatistics.Add(new SkillStatistics());
                    }
                }

                if (selectedSkill.SkillType == SkillType.Area_Of_Effect)
                {
                    var aoeSkill = (selectedSkill as AreaOfEffectSkill);
                    while (aoeSkill.WidthStatistics.Count > selectedSkill.MaxRank)
                    {
                        aoeSkill.WidthStatistics.RemoveAt(aoeSkill.WidthStatistics.Count - 1);
                    }

                    while (aoeSkill.WidthStatistics.Count < selectedSkill.MaxRank)
                    {
                        if (aoeSkill.WidthStatistics.Count > 0)
                        {
                            var copyOfLast =
                                GeneralMethods.CopyObject(
                                    aoeSkill.WidthStatistics[aoeSkill.WidthStatistics.Count - 1]);
                            aoeSkill.WidthStatistics.Add(copyOfLast);
                        }
                        else
                        {
                            aoeSkill.WidthStatistics.Add(0.0f);
                        }
                    }

                    while (aoeSkill.LengthStatistics.Count > selectedSkill.MaxRank)
                    {
                        aoeSkill.LengthStatistics.RemoveAt(aoeSkill.LengthStatistics.Count - 1);
                    }

                    while (aoeSkill.LengthStatistics.Count < selectedSkill.MaxRank)
                    {
                        if (aoeSkill.LengthStatistics.Count > 0)
                        {
                            var copyOfLast =
                                GeneralMethods.CopyObject(
                                    aoeSkill.LengthStatistics[aoeSkill.LengthStatistics.Count - 1]);
                            aoeSkill.LengthStatistics.Add(copyOfLast);
                        }
                        else
                        {
                            aoeSkill.LengthStatistics.Add(0.0f);
                        }
                    }
                    while (aoeSkill.LengthStatistics.Count > selectedSkill.MaxRank)
                    {
                        aoeSkill.LengthStatistics.RemoveAt(aoeSkill.LengthStatistics.Count - 1);
                    }

                    while (aoeSkill.HeightStatistics.Count < selectedSkill.MaxRank)
                    {
                        if (aoeSkill.HeightStatistics.Count > 0)
                        {
                            var copyOfLast =
                                GeneralMethods.CopyObject(
                                    aoeSkill.HeightStatistics[aoeSkill.HeightStatistics.Count - 1]);
                            aoeSkill.HeightStatistics.Add(copyOfLast);
                        }
                        else
                        {
                            aoeSkill.HeightStatistics.Add(0.0f);
                        }
                    }


                    while (aoeSkill.TimeTillDestroy.Count > selectedSkill.MaxRank)
                    {
                        aoeSkill.TimeTillDestroy.RemoveAt(aoeSkill.TimeTillDestroy.Count - 1);
                    }

                    while (aoeSkill.TimeTillDestroy.Count < selectedSkill.MaxRank)
                    {
                        if (aoeSkill.TimeTillDestroy.Count > 0)
                        {
                            var copyOfLast =
                                GeneralMethods.CopyObject(
                                    aoeSkill.TimeTillDestroy[aoeSkill.TimeTillDestroy.Count - 1]);
                            aoeSkill.TimeTillDestroy.Add(copyOfLast);
                        }
                        else
                        {
                            aoeSkill.TimeTillDestroy.Add(0.0f);
                        }
                    }

                }

                if (selectedSkill.SkillType == SkillType.Projectile)
                {
                    var prjSkill = (selectedSkill as ProjectileSkill);
                    while (prjSkill.ProjectileSkillStatistics.Count > selectedSkill.MaxRank)
                    {
                        prjSkill.ProjectileSkillStatistics.RemoveAt(prjSkill.ProjectileSkillStatistics.Count - 1);
                    }

                    while (prjSkill.ProjectileSkillStatistics.Count < selectedSkill.MaxRank)
                    {
                        if (prjSkill.ProjectileSkillStatistics.Count > 0)
                        {
                            var copyOfLast =
                                GeneralMethods.CopyObject(
                                    prjSkill.ProjectileSkillStatistics[prjSkill.ProjectileSkillStatistics.Count - 1]);
                            prjSkill.ProjectileSkillStatistics.Add(copyOfLast);
                        }
                        else
                        {
                            prjSkill.ProjectileSkillStatistics.Add(new ProjectileSkillStatistics());
                        }
                    }

                }
                if (selectedSkill.SkillType == SkillType.Aura)
                {
                    var auraSkill = (selectedSkill as AuraSkill);
                    while (auraSkill.AuraEffectStatistics.Count > selectedSkill.MaxRank)
                    {
                        auraSkill.AuraEffectStatistics.RemoveAt(auraSkill.AuraEffectStatistics.Count - 1);
                    }

                    while (auraSkill.AuraEffectStatistics.Count < selectedSkill.MaxRank)
                    {
                        if (auraSkill.AuraEffectStatistics.Count > 0)
                        {
                            var copyOfLast =
                                GeneralMethods.CopyObject(
                                    auraSkill.AuraEffectStatistics[auraSkill.AuraEffectStatistics.Count - 1]);
                            auraSkill.AuraEffectStatistics.Add(copyOfLast);
                        }
                        else
                        {
                            auraSkill.AuraEffectStatistics.Add(new AuraEffect());
                        }
                    }

                }
                if (selectedSkill.SkillType == SkillType.Melee)
                {
                    var meleeSkill = (selectedSkill as MeleeSkill);
                    while (meleeSkill.MeleeSkillDetails.Count > selectedSkill.MaxRank)
                    {
                        meleeSkill.MeleeSkillDetails.RemoveAt(meleeSkill.MeleeSkillDetails.Count - 1);
                    }

                    while (meleeSkill.MeleeSkillDetails.Count < selectedSkill.MaxRank)
                    {
                        if (meleeSkill.MeleeSkillDetails.Count > 0)
                        {
                            var copyOfLast =
                                GeneralMethods.CopyObject(
                                    meleeSkill.MeleeSkillDetails[meleeSkill.MeleeSkillDetails.Count - 1]);
                            meleeSkill.MeleeSkillDetails.Add(copyOfLast);
                        }
                        else
                        {
                            meleeSkill.MeleeSkillDetails.Add(new MeleeSkillDetail());
                        }
                    }

                    //Sounds
                    var maxSounds = meleeSkill.MeleeSkillDetails.Max(m => m.Attacks);
                    while (meleeSkill.MeleeSkillSounds.Count > maxSounds)
                    {
                        meleeSkill.MeleeSkillSounds.RemoveAt(meleeSkill.MeleeSkillSounds.Count - 1);
                    }

                    while (meleeSkill.MeleeSkillSounds.Count < maxSounds)
                    {
                        meleeSkill.MeleeSkillSounds.Add(new AudioContainer());
                    }

                    while (meleeSkill.ImpactPrefabPaths.Count > maxSounds)
                    {
                        meleeSkill.ImpactPrefabPaths.RemoveAt(meleeSkill.ImpactPrefabPaths.Count - 1);
                    }

                    while (meleeSkill.ImpactPrefabPaths.Count < maxSounds)
                    {
                        meleeSkill.ImpactPrefabPaths.Add(new StringField());
                    }

                    var maxCastingSounds = meleeSkill.SeperateCastPerAttack ? maxSounds : 1;
                    while (meleeSkill.MeleeCastingSounds.Count > maxCastingSounds)
                    {
                        meleeSkill.MeleeCastingSounds.RemoveAt(meleeSkill.MeleeCastingSounds.Count - 1);
                    }

                    while (meleeSkill.MeleeCastingSounds.Count < maxCastingSounds)
                    {
                        meleeSkill.MeleeCastingSounds.Add(new AudioContainer());
                    }

                    while (meleeSkill.MeleeImpactSounds.Count > maxSounds)
                    {
                        meleeSkill.MeleeImpactSounds.RemoveAt(meleeSkill.MeleeImpactSounds.Count - 1);
                    }

                    while (meleeSkill.MeleeImpactSounds.Count < maxSounds)
                    {
                        meleeSkill.MeleeImpactSounds.Add(new AudioContainer());
                    }

                    //Animations

                    foreach (var d in Rm_RPGHandler.Instance.Player.CharacterDefinitions)
                    {
                        var tier = meleeSkill.MeleeAnimations.FirstOrDefault(t => t.ClassID == d.ID);
                        if (tier == null)
                        {

                            foreach(var className in d.ApplicableClassIDs)
                            {
                                if (selectedSkill.ClassIDs.Contains(className.ID) || selectedSkill.AllClasses)
                                {
                                    meleeSkill.MeleeAnimations.Add(new MeleeSkillAnimation() { ClassID = d.ID });
                                    break;
                                }
                            }
                        }
                    }

                    for (int index = 0; index < meleeSkill.MeleeAnimations.Count; index++)
                    {
                        var v = meleeSkill.MeleeAnimations[index];
                        var stillExists =
                            Rm_RPGHandler.Instance.Player.CharacterDefinitions.FirstOrDefault(
                                t => t.ID == v.ClassID);

                        if (stillExists == null || (!selectedSkill.ClassIDs.ContainsAnyOf(stillExists.ApplicableClassIDs) && !selectedSkill.AllClasses))
                        {
                            meleeSkill.MeleeAnimations.Remove(v);
                            index--;
                        }
                    }

                    foreach (var d in meleeSkill.MeleeAnimations)
                    {
                        while (d.Definitions.Count > maxSounds)
                        {
                            d.Definitions.RemoveAt(d.Definitions.Count - 1);
                        }


                        while (d.Definitions.Count < maxSounds)
                        {
                            d.Definitions.Add(new SkillAnimationDefinition());
                        }
                    }






                    //Impact prefabs
                    var currentDetails = meleeSkill.MeleeSkillDetails[selectedSkill.CurrentRank];
                    while (currentDetails.ImpactPrefabPaths.Count > currentDetails.Attacks)
                    {
                        currentDetails.ImpactPrefabPaths.RemoveAt(currentDetails.ImpactPrefabPaths.Count - 1);
                    }

                    while (currentDetails.ImpactPrefabPaths.Count < currentDetails.Attacks)
                    {
                        currentDetails.ImpactPrefabPaths.Add("");
                    }
                    //Scalings
                    while (currentDetails.MeleeSkillScalings.Count > currentDetails.Attacks)
                    {
                        currentDetails.MeleeSkillScalings.RemoveAt(currentDetails.MeleeSkillScalings.Count - 1);
                    }

                    while (currentDetails.MeleeSkillScalings.Count < currentDetails.Attacks)
                    {
                        currentDetails.MeleeSkillScalings.Add(1.0f);
                    }
                    //MovementTypes
                    while (currentDetails.MeleeMoveDefinitions.Count > currentDetails.Attacks)
                    {
                        currentDetails.MeleeMoveDefinitions.RemoveAt(currentDetails.MeleeMoveDefinitions.Count - 1);
                    }

                    while (currentDetails.MeleeMoveDefinitions.Count < currentDetails.Attacks)
                    {
                        currentDetails.MeleeMoveDefinitions.Add(new MeleeMoveDefinition());
                    }
                }
                if (selectedSkill.SkillType == SkillType.Restoration)
                {
                    var restoSkill = (selectedSkill as RestorationSkill);
                    while (restoSkill.RestorationEffects.Count > selectedSkill.MaxRank)
                    {
                        restoSkill.RestorationEffects.RemoveAt(restoSkill.RestorationEffects.Count - 1);
                    }

                    while (restoSkill.RestorationEffects.Count < selectedSkill.MaxRank)
                    {
                        if (restoSkill.RestorationEffects.Count > 0)
                        {
                            var copyOfLast =
                                GeneralMethods.CopyObject(
                                    restoSkill.RestorationEffects[restoSkill.RestorationEffects.Count - 1]);
                            restoSkill.RestorationEffects.Add(copyOfLast);
                        }
                        else
                        {
                            restoSkill.RestorationEffects.Add(new Restoration());
                        }
                    }
                }

                #endregion

                #region "Update skill resource requirements"
                foreach(var skill in Rm_RPGHandler.Instance.Repositories.Skills.AllSkills)
                {
                    foreach(var skillStat in skill.SkillStatistics)
                    {
                        skillStat.ResourceRequiredId = skill.ResourceIDUsed;
                    }
                }
                #endregion

                #region "Update aura skill parent ID"
                foreach(var skill in Rm_RPGHandler.Instance.Repositories.Skills.AllSkills.Where(s => s.SkillType == SkillType.Aura))
                {
                    var auraSkill = skill as AuraSkill;
                    foreach(var skillStat in auraSkill.AuraEffectStatistics)
                    {
                        skillStat.SkillId = skill.ID;
                    }
                }
                #endregion

                #region "Update restoration skill meta ID"
                foreach(var skill in Rm_RPGHandler.Instance.Repositories.Skills.AllSkills.Where(s => s.SkillType == SkillType.Restoration))
                {
                    var restorationSkill = skill as RestorationSkill;
                    foreach(var skillStat in restorationSkill.RestorationEffects)
                    {
                        skillStat.SkillMetaId = skill.HasSkillMeta ? skill.SkillMetaID : null;
                    }
                }
                #endregion

                #region "Update skill-Dots skill meta ID"
                foreach(var skill in Rm_RPGHandler.Instance.Repositories.Skills.AllSkills.Where(s => s.HasSkillMeta))
                {
                    foreach(var skillStat in skill.SkillStatistics)
                    {
                        if(skillStat.ApplyDOTOnHit)
                        {
                            skillStat.DamageOverTime.SkillMetaID = skill.SkillMetaID;
                            skillStat.DamageOverTime.DamagePerTick.SkillMetaID = skill.SkillMetaID;
                        }

                        skillStat.Damage.SkillMetaID = skill.SkillMetaID;
                    }
                }
                #endregion

                if (RPGMakerGUI.Foldout(ref showSkillDetails, "Skill Details"))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical(GUILayout.MaxWidth(85));
                    selectedSkill.Image.Image = RPGMakerGUI.ImageSelector("", selectedSkill.Image.Image, ref selectedSkill.Image.ImagePath);

                    GUILayout.EndVertical();
                    GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

                    selectedSkill.Name = RPGMakerGUI.TextField("Name: ", selectedSkill.Name);
                    selectedSkill.MinCastRange = RPGMakerGUI.FloatField("Min Cast Range: ", selectedSkill.MinCastRange);
                    selectedSkill.CastRange = RPGMakerGUI.FloatField("Max Cast Range: ", selectedSkill.CastRange);
                    if (selectedSkill.MaxRank < 1) selectedSkill.MaxRank = 1;

                    selectedSkill.UseResourceOnCast = RPGMakerGUI.Toggle("Use Resource On Cast?", selectedSkill.UseResourceOnCast);
                    if (selectedSkill.UseResourceOnCast)
                    {

                        RPGMakerGUI.PopupID<Rm_VitalDefinition>("Resource:", ref selectedSkill.ResourceIDUsed);
                    }

                    if(RPGMakerGUI.Toggle("Has Skill Meta?", ref selectedSkill.HasSkillMeta))
                    {
                        RPGMakerGUI.PopupID<SkillMetaDefinition>("Skill Meta:", ref selectedSkill.SkillMetaID);    
                    }

                    if (RPGMakerGUI.Toggle("Automatically Unlock At Level?", ref selectedSkill.AutomaticallyUnlockAtLevel))
                    {
                        selectedSkill.LevelToAutomaticallyUnlock = RPGMakerGUI.IntField("- Level:", selectedSkill.LevelToAutomaticallyUnlock);
                    }

                    if (!new[] { SkillType.Spawn }.Any(s => s == selectedSkill.SkillType))
                    {
                        selectedSkill.TargetType =
                            (TargetType)RPGMakerGUI.EnumPopup("Target Type:", selectedSkill.TargetType);
                    }

                    selectedSkill.UpgradeType = (SkillUpgradeType)RPGMakerGUI.EnumPopup("Upgrade Type:", selectedSkill.UpgradeType);
                    if (selectedSkill.UpgradeType == SkillUpgradeType.TraitLevel)
                    {
                        RPGMakerGUI.PopupID<Rm_TraitDefintion>("Trait:", ref selectedSkill.TraitIDToLevel);
                    }

                    if (!new[] { SkillType.Spawn, SkillType.Aura, SkillType.Melee}.Any(s => s == selectedSkill.SkillType))
                    {
                        selectedSkill.AppliesBuffDebuff = RPGMakerGUI.Toggle("Applies Buff/Debuff?", selectedSkill.AppliesBuffDebuff);
                    }

                    if (selectedSkill.SkillType == SkillType.Area_Of_Effect)
                    {
                        var aoeSkill = selectedSkill as AreaOfEffectSkill;
                        aoeSkill.Shape = (AOEShape)RPGMakerGUI.EnumPopup("AOE Shape:", aoeSkill.Shape);
                    }

                    if (selectedSkill.SkillType == SkillType.Ability || selectedSkill.SkillType == SkillType.Restoration || selectedSkill.SkillType == SkillType.Melee || selectedSkill.SkillType == SkillType.Projectile)
                    {
                        if (Combat.TargetStyle == TargetStyle.ManualTarget)
                        {
                            RPGMakerGUI.Toggle("Always Require Target?", ref selectedSkill.AlwaysRequireTarget);
                        }
                    }

                    if (selectedSkill.SkillType == SkillType.Ability || selectedSkill.SkillType == SkillType.Area_Of_Effect || selectedSkill.SkillType == SkillType.Restoration)
                    {
                        
                        
                        selectedSkill.MovementType = (SkillMovementType)RPGMakerGUI.EnumPopup("Movement Type:", selectedSkill.MovementType);

                        if (selectedSkill.MovementType == SkillMovementType.MoveTo ||
                            selectedSkill.MovementType == SkillMovementType.JumpTo)
                        {
                            selectedSkill.MoveToSpeed = RPGMakerGUI.FloatField("- Move To Speed:", selectedSkill.MoveToSpeed);
                        }

                        if (selectedSkill.MovementType == SkillMovementType.JumpTo)
                        {
                            selectedSkill.JumpToHeight = RPGMakerGUI.FloatField("- Jump To Speed:", selectedSkill.JumpToHeight);
                        }

                        if(selectedSkill.MovementType != SkillMovementType.StayInPlace)
                        {
                            selectedSkill.LandTime = RPGMakerGUI.FloatField("- Land Time:", selectedSkill.LandTime);
                        }

                        if (selectedSkill.MovementType == SkillMovementType.MoveTo || selectedSkill.MovementType == SkillMovementType.JumpTo)
                        {
                            GUILayout.BeginHorizontal();
                            gameObject = RPGMakerGUI.PrefabSelector("Moving Prefab", gameObject, ref selectedSkill.MovingToPrefab);
                            gameObject = RPGMakerGUI.PrefabGeneratorButton(Rmh_PrefabType.Moving_To_Effect, gameObject, ref selectedSkill.MovingToPrefab);

                            GUILayout.EndHorizontal(); 
                        }

                        if(selectedSkill.MovementType != SkillMovementType.StayInPlace)
                        {
                            GUILayout.BeginHorizontal();
                            gameObject = RPGMakerGUI.PrefabSelector("Target Reached Prefab", gameObject, ref selectedSkill.LandPrefab);
                            gameObject = RPGMakerGUI.PrefabGeneratorButton(Rmh_PrefabType.Target_Reached_Effect, gameObject, ref selectedSkill.LandPrefab);
                            GUILayout.EndHorizontal(); 
                        }
                    }

                    if(!RPGMakerGUI.Toggle("Enemy Only Skill?", ref selectedSkill.EnemyOnlySkill))
                    {
                        selectedSkill.AllClasses = RPGMakerGUI.Toggle("All Classes?", selectedSkill.AllClasses);
                    }

                    if (!selectedSkill.AllClasses)
                    {
                        GUILayout.BeginVertical("foldoutBox");
                        var result = RPGMakerGUI.ToolBar("Classes:", new string[] {"+Class"});
                        if (showSkillClasses)
                        {
                            if (selectedSkill.ClassIDs.Count == 0)
                            {
                                EditorGUILayout.HelpBox("Click +Class to add a class that can use this skill", MessageType.Info);
                            }

                            for (int i = 0; i < selectedSkill.ClassIDs.Count; i++)
                            {
                                
                                GUILayout.BeginHorizontal();
                                var refString = selectedSkill.ClassIDs[i];
                                RPGMakerGUI.PopupID<Rm_ClassNameDefinition>("", ref refString);
                                selectedSkill.ClassIDs[i] = refString;

                                if (RPGMakerGUI.DeleteButton(15))
                                {
                                    selectedSkill.ClassIDs.Remove(selectedSkill.ClassIDs[i]);
                                    i--;
                                }
                                GUILayout.EndHorizontal();
                            }

                            if (result == 0)
                            {
                                selectedSkill.ClassIDs.Add("");
                            }
                        }
                        GUILayout.EndVertical();
                    }

                    if (selectedSkill.SkillType == SkillType.Area_Of_Effect)
                    {
                        var aoeSkill = (selectedSkill as AreaOfEffectSkill);
                        aoeSkill.HitMultipleTimes = RPGMakerGUI.Toggle("Hit Multiple Times?", aoeSkill.HitMultipleTimes);
                        if (aoeSkill.HitMultipleTimes)
                        {
                            aoeSkill.DelayBetweenHits = RPGMakerGUI.FloatField("- Delay Between Hits?", aoeSkill.DelayBetweenHits);
                        }


                    }

                    if (selectedSkill.SkillType == SkillType.Melee)
                    {
                        var meleeSkill = (MeleeSkill)selectedSkill;
                        var meleeDetails = meleeSkill.MeleeSkillDetails[selectedSkill.CurrentRank];
                        if (RPGMakerGUI.Toggle("Seperate Cast Per Attack?",ref meleeSkill.SeperateCastPerAttack))
                        {
                            meleeDetails.MaxTimeBetweenCasts = RPGMakerGUI.FloatField("Max Time Between Casts:", meleeDetails.MaxTimeBetweenCasts);
                        }
                    }


                    GUILayout.BeginHorizontal();
                    gameObject = RPGMakerGUI.PrefabSelector("Casting Prefab", gameObject, ref selectedSkill.CastingPrefabPath);
                    gameObject = RPGMakerGUI.PrefabGeneratorButton(Rmh_PrefabType.Casting, gameObject, ref selectedSkill.CastingPrefabPath);
                    GUILayout.EndHorizontal();    
                
                    
                    GUILayout.BeginHorizontal();
                    gameObject = RPGMakerGUI.PrefabSelector("Cast Prefab", gameObject,
                                        ref selectedSkill.CastPrefabPath);
                    gameObject = RPGMakerGUI.PrefabGeneratorButton(Rmh_PrefabType.Cast, gameObject, ref selectedSkill.CastPrefabPath);
                    GUILayout.EndHorizontal();



                    if (!new[] { SkillType.Restoration, SkillType.Melee, SkillType.Ability, SkillType.Aura }.Any(s => s == selectedSkill.SkillType))
                    {
                        GUILayout.BeginHorizontal();
                        gameObject = RPGMakerGUI.PrefabSelector("Skill Prefab", gameObject,
                                                                ref selectedSkill.PrefabPath);
                        gameObject = RPGMakerGUI.PrefabGeneratorButton(Rmh_PrefabType.Skill, gameObject, ref selectedSkill.PrefabPath, selectedSkill.SkillType);

                        GUILayout.EndHorizontal();
                    }

                    GUILayout.Space(5);

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Open Custom Damage Scaling Window", "genericButton", GUILayout.MaxHeight(30)))
                    {
                        var trees = Rm_RPGHandler.Instance.Nodes.CombatNodeBank.NodeTrees;
                        var existingTree = trees.FirstOrDefault(t => t.ID == selectedSkill.ID);
                        if (existingTree == null)
                        {
                            existingTree = NodeWindow.GetNewTree(NodeTreeType.Combat);
                            existingTree.ID = selectedSkill.ID;
                            existingTree.Name = selectedSkill.Name;
                            trees.Add(existingTree);
                        }

                        CombatNodeWindow.ShowWindow(selectedSkill.ID);
                        selectedSkill.DamageScalingTreeID = existingTree.ID;
                    }

                    if(!string.IsNullOrEmpty(selectedSkill.DamageScalingTreeID))
                    {
                        if(GUILayout.Button("Remove","genericButton", GUILayout.MaxHeight(30)))
                        {
                            var trees = Rm_RPGHandler.Instance.Nodes.CombatNodeBank.NodeTrees;
                            var existingTree = trees.FirstOrDefault(t => t.ID == selectedSkill.ID);
                            if (existingTree != null)
                            {
                                trees.Remove(existingTree);
                            }
                            selectedSkill.DamageScalingTreeID = null;
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Space(5);
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();

                    RPGMakerGUI.SubTitle("Additional Details");
                    selectedSkill.Unlocked = RPGMakerGUI.Toggle("Starts Unlocked: ", selectedSkill.Unlocked);

                    if (!new[] { SkillType.Melee }.Any(s => s == selectedSkill.SkillType))
                    {

                        selectedSkill.Sound.Audio = RPGMakerGUI.AudioClipSelector("Cast Sound:",
                                                                                  selectedSkill.Sound.Audio,
                                                                                  ref selectedSkill.Sound.AudioPath);

                        if (selectedSkill.SkillType == SkillType.Projectile)
                        {
                            var prjSkill = (selectedSkill as ProjectileSkill);
                            prjSkill.TravelSound.Audio = RPGMakerGUI.AudioClipSelector("Travel Sound:", prjSkill.TravelSound.Audio, ref prjSkill.TravelSound.AudioPath);
                        }

                        selectedSkill.CastingSound.Audio = RPGMakerGUI.AudioClipSelector("Casting Sound:", selectedSkill.CastingSound.Audio, ref selectedSkill.CastingSound.AudioPath);
                        selectedSkill.ImpactSound.Audio = RPGMakerGUI.AudioClipSelector("Impact Sound:", selectedSkill.ImpactSound.Audio, ref selectedSkill.ImpactSound.AudioPath);


                        GUILayout.BeginHorizontal();
                        gameObject = RPGMakerGUI.PrefabSelector("Impact Prefab", gameObject, ref selectedSkill.ImpactPrefabPath);
                        gameObject = RPGMakerGUI.PrefabGeneratorButton(Rmh_PrefabType.Impact, gameObject, ref selectedSkill.ImpactPrefabPath);

                        GUILayout.EndHorizontal(); 
                    }

                    if(selectedSkill.SkillType == SkillType.Melee)
                    {
                        var meleeSkill = selectedSkill as MeleeSkill;

                        if(meleeSkill.SeperateCastPerAttack)
                        {
                            for (int index = 0; index < meleeSkill.MeleeCastingSounds.Count; index++)
                            {
                                var audioContainer = meleeSkill.MeleeCastingSounds[index];
                                audioContainer.Audio = RPGMakerGUI.AudioClipSelector("Casting Sound [" + index + "]:", audioContainer.Audio, ref audioContainer.AudioPath);
                            }
                        }
                        else
                        {
                            var audioContainer = meleeSkill.MeleeCastingSounds[0];
                            audioContainer.Audio = RPGMakerGUI.AudioClipSelector("Casting Sound :", audioContainer.Audio, ref audioContainer.AudioPath);
                        }

                        for (int index = 0; index < meleeSkill.MeleeSkillSounds.Count; index++)
                        {
                            var audioContainer = meleeSkill.MeleeSkillSounds[index];
                            audioContainer.Audio = RPGMakerGUI.AudioClipSelector("Cast Sound [" + index + "]:", audioContainer.Audio, ref audioContainer.AudioPath);
                        }

                        for (int index = 0; index < meleeSkill.ImpactPrefabPaths.Count; index++)
                        {
                            var impactPrefab = meleeSkill.ImpactPrefabPaths[index];
                            gameObject = RPGMakerGUI.PrefabSelector("Impact Prefab [" + index + "]:", gameObject, ref impactPrefab.ID);
                        }

                        for (int index = 0; index < meleeSkill.MeleeImpactSounds.Count; index++)
                        {
                            var audioContainer = meleeSkill.MeleeImpactSounds[index];
                            audioContainer.Audio = RPGMakerGUI.AudioClipSelector("Impact Sound [" + index + "]:", audioContainer.Audio, ref audioContainer.AudioPath);
                        }
                    }

                    if(RPGMakerGUI.Toggle("Add Resource On Cast?", ref selectedSkill.AddResourceOnCast))
                    {
                        RPGMakerGUI.PopupID<Rm_VitalDefinition>("Resource Added:", ref selectedSkill.ResourceAddedID);
                    }

                    if(RPGMakerGUI.Toggle("Use Event On Cast?", ref selectedSkill.UseEventOnCast))
                    {
                        RPGMakerGUI.PopupID<NodeChain>("Event:",ref selectedSkill.EventOnCastID,1);
                    }




                    selectedSkill.RequireVitalAboveX = RPGMakerGUI.Toggle("Require Vital Above X% ?", selectedSkill.RequireVitalAboveX);
                    if (selectedSkill.RequireVitalAboveX) selectedSkill.RequireVitalBelowX = false;
                    selectedSkill.RequireVitalBelowX = RPGMakerGUI.Toggle("Require Vital Below X% ?", selectedSkill.RequireVitalBelowX);
                    if (selectedSkill.RequireVitalBelowX) selectedSkill.RequireVitalAboveX = false;

                    if (selectedSkill.RequireVitalAboveX || selectedSkill.RequireVitalBelowX)
                    {
                        RPGMakerGUI.PopupID<Rm_VitalDefinition>("Vital Required Above/Below X%:", ref selectedSkill.RequiredVitalId);
                    }

                    selectedSkill.RequireEquippedWep = RPGMakerGUI.Toggle("Require Equipped Weapon Type?", selectedSkill.RequireEquippedWep);
                    if (selectedSkill.RequireEquippedWep)
                    {
                        RPGMakerGUI.PopupID<WeaponTypeDefinition>("Required Weapon Type:", ref selectedSkill.RequiredEquippedWepTypeID);
                    }

                    selectedSkill.ReqPrevSkillInCombo = RPGMakerGUI.Toggle("Require Previous Skill In Combo", selectedSkill.ReqPrevSkillInCombo);

                    if (selectedSkill.ReqPrevSkillInCombo)
                    {
                        selectedSkill.MaxComboTime = RPGMakerGUI.FloatField("- Max Time To Combo to Next Skill:", selectedSkill.MaxComboTime);
                        RPGMakerGUI.PopupID<Skill>("- Previous Skill In Combo:", ref selectedSkill.PrevSkillForComboID);
                    }

                    RPGMakerGUI.EndFoldout();
                }


                #region RequiredSkills
                var reqSkillResult = RPGMakerGUI.FoldoutToolBar(ref reqSkillFoldout, "Required Skills To Unlock", new string[] { "+RequiredSkill" });
                if (reqSkillFoldout)
                {
                    for (int index = 0; index < selectedSkill.RequiredSkills.Count; index++)
                    {
                        var v = selectedSkill.RequiredSkills[index].ID;
                        if (string.IsNullOrEmpty(v)) continue;

                        var stillExists =
                            Rm_RPGHandler.Instance.Repositories.Skills.AllSkills.FirstOrDefault(
                                a => a.ID == v);

                        if (stillExists == null)
                        {
                            selectedSkill.RequiredSkills.Remove(selectedSkill.RequiredSkills[index]);
                            index--;
                        }
                    }
                    var allSkills = Rm_RPGHandler.Instance.Repositories.Skills.AllSkills.Where(t => t != selectedSkill).ToList();
                    if (allSkills.Count > 0)
                    {
                        if (selectedSkill.RequiredSkills.Count == 0)
                        {
                            EditorGUILayout.HelpBox("Click +RequiredSkill to add a skill that needs to be unlocked to unlock this.", MessageType.Info);
                        }
                        else
                        {
                            RPGMakerGUI.Toggle("Only Require At Least One?", ref selectedSkill.OnlyRequireOneSkill);
                        }

                        GUILayout.Space(5);
                        for (int index = 0; index < selectedSkill.RequiredSkills.Count; index++)
                        {
                            GUILayout.BeginHorizontal();
                            RPGMakerGUI.PopupID<Skill>("Skill:", ref selectedSkill.RequiredSkills[index].ID);

                            if (GUILayout.Button(RPGMakerGUI.DelIcon, "genericButton", GUILayout.Width(30), GUILayout.Height(30)))
                            {
                                selectedSkill.RequiredSkills.Remove(selectedSkill.RequiredSkills[index]);
                                index--;
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.Space(5);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No Other Skills Found.", MessageType.Info);
                        selectedSkill.RequiredSkills = new List<StringField>();
                    }

                    if (reqSkillResult == 0)
                    {
                        selectedSkill.RequiredSkills.Add(new StringField());
                    }
                    RPGMakerGUI.EndFoldout();
                }
                #endregion

                if (RPGMakerGUI.Foldout(ref showRankDetails, "Rank Details"))
                {

                    GUILayout.BeginHorizontal();
                    RPGMakerGUI.Label("Skill Rank " + (selectedSkill.CurrentRank + 1) + " / " + selectedSkill.MaxRank);
                    GUI.enabled = selectedSkill.CurrentRank > 0;
                    if (GUILayout.Button("Prev Rank", "genericButton", GUILayout.Width(100),
                                         GUILayout.Height(25)))
                    {
                        selectedSkill.CurrentRank--;
                        GUI.FocusControl("");
                    }
                    GUI.enabled = selectedSkill.CurrentRank + 1 < selectedSkill.MaxRank;
                    if (GUILayout.Button("Next Rank", "genericButton", GUILayout.Width(100),
                                         GUILayout.Height(25)))
                    {
                        selectedSkill.CurrentRank++;
                        GUI.FocusControl("");
                    }
                    GUI.enabled = true;
                    if (GUILayout.Button("+Rank", "genericButton", GUILayout.Width(50),
                                         GUILayout.Height(25)))
                    {
                        selectedSkill.MaxRank++;
                        selectedSkill.CurrentRank++;
                        GUI.FocusControl("");
                        return;
                    }
                    GUI.enabled = selectedSkill.MaxRank > 1;
                    if (GUILayout.Button("-Rank", "genericButton", GUILayout.Width(50),
                                         GUILayout.Height(25)))
                    {
                        selectedSkill.MaxRank--;
                        GUI.FocusControl("");
                    }
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();

                    #region RankDetails
                   

                    //note: all skill applydotonhit should have duration
                    foreach(var rank in selectedSkill.SkillStatistics)
                    {
                        rank.DamageOverTime.HasDuration = true;
                    }

                    var currentRank = selectedSkill.SkillStatistics[selectedSkill.CurrentRank];

                    switch (selectedSkill.UpgradeType)
                    {
                        case SkillUpgradeType.PlayerLevel:
                            currentRank.LevelReqToLevel = RPGMakerGUI.IntField("Player Level Required:",
                                                                               currentRank.LevelReqToLevel);
                            break;
                        case SkillUpgradeType.SkillPoints:
                            currentRank.SkillPointsToLevel = RPGMakerGUI.IntField("Skill Points to level:",
                                                                                  currentRank.SkillPointsToLevel);
                            break;
                        case SkillUpgradeType.TraitLevel:
                            currentRank.ReqTraitLevelToLevel = RPGMakerGUI.IntField("Trait Level Required:",
                                                                                    currentRank.ReqTraitLevelToLevel);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (selectedSkill.UseResourceOnCast)
                        currentRank.ResourceRequirement = RPGMakerGUI.IntField("Resource Req To Cast:",
                                                                               currentRank.ResourceRequirement);

                    currentRank.CoolDownTime = RPGMakerGUI.FloatField("Cooldown Time:", currentRank.CoolDownTime);
                    currentRank.TotalCastTime = RPGMakerGUI.FloatField("Total Cast Time:", currentRank.TotalCastTime);
                    currentRank.CastingTime = Math.Min(RPGMakerGUI.FloatField("Of Which is Casting:", currentRank.CastingTime, 1), currentRank.TotalCastTime);

                    if (selectedSkill.SkillType == SkillType.Projectile)
                    {
                        var projSkill = (ProjectileSkill)selectedSkill;
                        if (Combat.TargetStyle == TargetStyle.ManualTarget)
                        {
                            RPGMakerGUI.Toggle("Always Lock On?", ref projSkill.ProjectileSkillStatistics[selectedSkill.CurrentRank].AlwaysLockOn);
                        }
                    }
                    if (selectedSkill.SkillType == SkillType.Aura)
                    {
                        var auraSkill = selectedSkill as AuraSkill;
                        RPGMakerGUI.Toggle("Take Resource Amount Per Sec?", ref auraSkill.AuraEffectStatistics[selectedSkill.CurrentRank].TakeResourceAmountPerSec);
                        if (RPGMakerGUI.Toggle("Apply To Allies?", ref auraSkill.AuraEffectStatistics[selectedSkill.CurrentRank].ApplyToAllies))
                        {
                            auraSkill.AuraEffectStatistics[selectedSkill.CurrentRank].Radius = RPGMakerGUI.FloatField("Aura Radius:", auraSkill.AuraEffectStatistics[selectedSkill.CurrentRank].Radius);
                        }
                    }

                    if (selectedSkill.SkillType == SkillType.Restoration)
                    {
                        var restoSkillBase = selectedSkill as RestorationSkill;
                        var restoSkill = restoSkillBase.Restoration;
                        restoSkill.RestorationType = (RestorationType)RPGMakerGUI.EnumPopup("Restoration Type:", restoSkill.RestorationType);

                        if (restoSkill.RestorationType == RestorationType.Time_Based)
                        {
                            restoSkill.Duration = RPGMakerGUI.FloatField("Duration:", restoSkill.Duration);
                            restoSkill.SecBetweenRestore = RPGMakerGUI.FloatField("Seconds Between Restore:", restoSkill.SecBetweenRestore);

                        }

                        RPGMakerGUI.PopupID<Rm_VitalDefinition>("Vital To Restore:", ref restoSkill.VitalToRestoreID);
                        if (RPGMakerGUI.Toggle("Restores Fixed Amount?", ref restoSkill.FixedRestore))
                        {
                            restoSkill.AmountToRestore = RPGMakerGUI.IntField("Amount Restored:", restoSkill.AmountToRestore);
                        }
                        else
                        {
                            GUILayout.BeginHorizontal();
                            restoSkill.PercentToRestore = RPGMakerGUI.FloatField("Amount Restored:", restoSkill.PercentToRestore);
                            RPGMakerGUI.Label((restoSkill.PercentToRestore * 100).ToString("N2") + "%");
                            GUILayout.EndHorizontal();
                        }
                    }

                    //Custom skilltype properties
                    if (!new[] { SkillType.Aura, SkillType.Restoration, SkillType.Spawn }.Any(s => s == selectedSkill.SkillType))
                    {

                        #region Damage

                        #region CheckForUpdates

                        var dmgList = currentRank.Damage.ElementalDamages;
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

                        #endregion

                        GUILayout.BeginHorizontal();
                        RPGMakerGUI.Label("Physical Damage:");
                        if (Rm_RPGHandler.Instance.Items.DamageHasVariance)
                        {
                            currentRank.Damage.MinDamage = RPGMakerGUI.IntField("", currentRank.Damage.MinDamage);
                            GUILayout.Label(" - ");
                        }
                        currentRank.Damage.MaxDamage = RPGMakerGUI.IntField("", currentRank.Damage.MaxDamage);
                        GUILayout.EndHorizontal();

                        foreach (var eleDmg in dmgList)
                        {
                            var nameOfEleDmg =
                                Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions.First(
                                    e => e.ID == eleDmg.ElementID).Name;

                            GUILayout.BeginHorizontal();
                            RPGMakerGUI.Label(nameOfEleDmg + " Damage:");
                            if (Rm_RPGHandler.Instance.Items.DamageHasVariance)
                            {
                                eleDmg.MinDamage = RPGMakerGUI.IntField("", eleDmg.MinDamage);
                                GUILayout.Label(" - ");
                            }
                            eleDmg.MaxDamage = RPGMakerGUI.IntField("", eleDmg.MaxDamage);
                            GUILayout.EndHorizontal();
                        }

                        #endregion

                        if(Rm_RPGHandler.Instance.Combat.EnableTauntSystem)
                        {
                            currentRank.BonusTaunt = RPGMakerGUI.IntField("Bonus Taunt:", currentRank.BonusTaunt);    
                        }
                    }

                    if (selectedSkill.SkillType == SkillType.Area_Of_Effect)
                    {
                        var aoeSkill = (selectedSkill as AreaOfEffectSkill);
                        if (aoeSkill.Shape == AOEShape.Sphere)
                        {
                            aoeSkill.WidthStatistics[selectedSkill.CurrentRank] = RPGMakerGUI.FloatField("AOE Diameter:", aoeSkill.WidthStatistics[selectedSkill.CurrentRank]);
                        }
                        else if (aoeSkill.Shape == AOEShape.Cuboid)
                        {
                            aoeSkill.WidthStatistics[selectedSkill.CurrentRank] = RPGMakerGUI.FloatField("AOE Width:", aoeSkill.WidthStatistics[selectedSkill.CurrentRank]);
                            aoeSkill.LengthStatistics[selectedSkill.CurrentRank] = RPGMakerGUI.FloatField("AOE Length:", aoeSkill.LengthStatistics[selectedSkill.CurrentRank]);
                            aoeSkill.HeightStatistics[selectedSkill.CurrentRank] = RPGMakerGUI.FloatField("AOE Height:", aoeSkill.HeightStatistics[selectedSkill.CurrentRank]);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                        aoeSkill.TimeTillDestroy[selectedSkill.CurrentRank] = RPGMakerGUI.FloatField("AOE Duration:", aoeSkill.TimeTillDestroy[selectedSkill.CurrentRank]);

                    }
                    if (selectedSkill.SkillType == SkillType.Projectile)
                    {
                        var prjSkill = (selectedSkill as ProjectileSkill).ProjectileSkillStatistics[selectedSkill.CurrentRank];
                        prjSkill.Speed = RPGMakerGUI.FloatField("Projectile Speed:", prjSkill.Speed);
                        prjSkill.TimeTillDestroy = RPGMakerGUI.FloatField("Max Flight Time:", prjSkill.TimeTillDestroy);

                        if (prjSkill.IsPiercing = RPGMakerGUI.Toggle("Is Piercing?", prjSkill.IsPiercing))
                        {
                            prjSkill.NumberOfPierces = EditorGUILayout.IntSlider("- Number of Pierces:", prjSkill.NumberOfPierces, 1, 8);
                            if (prjSkill.PiercingScaling.Length != prjSkill.NumberOfPierces)
                            {
                                prjSkill.PiercingScaling = new float[prjSkill.NumberOfPierces];
                                for (int i = 0; i < prjSkill.PiercingScaling.Length; i++)
                                {
                                    prjSkill.PiercingScaling[i] = 1.0f;
                                }
                            }
                            GUILayout.BeginHorizontal();
                            EditorGUILayout.PrefixLabel("- Damage Scaling:");
                            for (int index = 0; index < prjSkill.PiercingScaling.Length; index++)
                            {
                                prjSkill.PiercingScaling[index] = RPGMakerGUI.FloatField("", prjSkill.PiercingScaling[index], GUILayout.Width(50));
                                if (prjSkill.PiercingScaling[index] < 0) prjSkill.PiercingScaling[index] = 0;
                                if (prjSkill.PiercingScaling[index] > 1) prjSkill.PiercingScaling[index] = 1;
                                GUILayout.Space(5);
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.Space(5);
                        }
                    }
                    if (selectedSkill.SkillType == SkillType.Aura || selectedSkill.AppliesBuffDebuff)
                    {
                        var subtitletext = selectedSkill.SkillType == SkillType.Aura ? "Passive Effect Details" : "Buff/Debuff Effect Details";
                        RPGMakerGUI.SubTitle(subtitletext);

                        if (RPGMakerGUI.Toggle("Has Duration?", ref selectedSkill.Effect.HasDuration))
                        {
                            selectedSkill.Effect.Duration = RPGMakerGUI.FloatField("- Duration:", selectedSkill.Effect.Duration);
                        }

                        GUILayout.BeginHorizontal();
                        gameObject = RPGMakerGUI.PrefabSelector("Active Prefab:", gameObject, ref selectedSkill.Effect.ActivePrefab);
                        gameObject = RPGMakerGUI.PrefabGeneratorButton(Rmh_PrefabType.Effect_Active, gameObject, ref selectedSkill.Effect.ActivePrefab);
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        gameObject = RPGMakerGUI.PrefabSelector("On Activate Prefab:", gameObject, ref selectedSkill.Effect.OnActivatePrefab);
                        gameObject = RPGMakerGUI.PrefabGeneratorButton(Rmh_PrefabType.Effect_Activated, gameObject, ref selectedSkill.Effect.OnActivatePrefab);
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        gameObject = RPGMakerGUI.PrefabSelector("On Expired Prefab:", gameObject, ref selectedSkill.Effect.OnExpiredPrefab);
                        gameObject = RPGMakerGUI.PrefabGeneratorButton(Rmh_PrefabType.Effect_Expired, gameObject, ref selectedSkill.Effect.OnExpiredPrefab);
                        GUILayout.EndHorizontal();



                        if (selectedSkill.Effect.CanBeCancelled = RPGMakerGUI.Toggle("Cancellable by Status Effect?", selectedSkill.Effect.CanBeCancelled))
                        {
                            RPGMakerGUI.PopupID<StatusEffect>("- Status Effect", ref selectedSkill.Effect.CancellingStatusEffectID);
                        }

                        PassiveEffectDetails(selectedSkill.Effect);

                    }
                    if (selectedSkill.SkillType == SkillType.Melee)
                    {
                        var meleeSkill = (selectedSkill as MeleeSkill);
                        var meleeDetails = meleeSkill.MeleeSkillDetails[meleeSkill.CurrentRank];
                        meleeDetails.Attacks = RPGMakerGUI.IntField("Number of Attacks:", meleeDetails.Attacks);

                        for (int index = 0; index < meleeDetails.MeleeMoveDefinitions.Count; index++)
                        {
                            var moveDef = meleeDetails.MeleeMoveDefinitions[index];
                            moveDef.MovementType = (SkillMovementType)RPGMakerGUI.EnumPopup("Attack [" + index + "] Movement Type:", moveDef.MovementType);

                            if (moveDef.MovementType == SkillMovementType.MoveTo ||
                                moveDef.MovementType == SkillMovementType.JumpTo)
                            {
                                moveDef.MoveToSpeed = RPGMakerGUI.FloatField("- Move To Speed:", moveDef.MoveToSpeed);
                            }

                            if (moveDef.MovementType == SkillMovementType.JumpTo)
                            {
                                moveDef.JumpToHeight = RPGMakerGUI.FloatField("- Jump To Speed:", moveDef.JumpToHeight);
                            }

                            if(moveDef.MovementType != SkillMovementType.StayInPlace)
                            {
                                moveDef.LandTime = RPGMakerGUI.FloatField("- Land Time:", moveDef.LandTime);
                            }

                            if (moveDef.MovementType == SkillMovementType.MoveTo || moveDef.MovementType == SkillMovementType.JumpTo)
                            {
                                GUILayout.BeginHorizontal();
                                gameObject = RPGMakerGUI.PrefabSelector("- Moving Prefab", gameObject, ref moveDef.MovingToPrefab);
                                gameObject = RPGMakerGUI.PrefabGeneratorButton(Rmh_PrefabType.Moving_To_Effect, gameObject, ref moveDef.MovingToPrefab);

                                GUILayout.EndHorizontal();
                            }

                            if (moveDef.MovementType != SkillMovementType.StayInPlace)
                            {
                                GUILayout.BeginHorizontal();
                                gameObject = RPGMakerGUI.PrefabSelector("- Target Reached Prefab", gameObject, ref moveDef.LandPrefab);
                                gameObject = RPGMakerGUI.PrefabGeneratorButton(Rmh_PrefabType.Target_Reached_Effect, gameObject, ref moveDef.LandPrefab);

                                GUILayout.EndHorizontal();
                            }
                        }


                        for (int index = 0; index < meleeDetails.MeleeSkillScalings.Count; index++)
                        {
                            GUILayout.BeginHorizontal();
                            meleeDetails.MeleeSkillScalings[index] = RPGMakerGUI.FloatField("Scaling on Attack [" + index + "]:", meleeDetails.MeleeSkillScalings[index]);
                            RPGMakerGUI.Label((meleeDetails.MeleeSkillScalings[index] * 100).ToString("N2") + "%");
                            GUILayout.EndHorizontal();

                        }

                        if (meleeDetails.Attacks < 1) meleeDetails.Attacks = 1;
                    }

                    if (selectedSkill.SkillType == SkillType.Spawn)
                    {
                        if(RPGMakerGUI.Toggle("Limited Spawn Duration?", ref currentRank.HasDuration))
                        {
                            currentRank.SpawnForTime = RPGMakerGUI.FloatField("Spawn For Time:", currentRank.SpawnForTime);
                        }

                        if(RPGMakerGUI.Toggle("Limit Spawn Instances?", ref currentRank.LimitSpawnInstances))
                        {
                            currentRank.MaxInstances = RPGMakerGUI.IntField("Max Instances:", currentRank.MaxInstances);
                        }
                    }

                    //Additional Behaviours
                    if (!new[] { SkillType.Aura, SkillType.Spawn }.Any(s => s == selectedSkill.SkillType) ||
                        selectedSkill.AddResourceOnCast || (selectedSkill.RequireVitalAboveX || selectedSkill.RequireVitalBelowX))
                    {
                        RPGMakerGUI.SubTitle("Additional Behaviours");
                    }
                    if (selectedSkill.AddResourceOnCast)
                        currentRank.ResourceAddedAmount = RPGMakerGUI.IntField("Resource Added Amount:",
                                                                               currentRank.ResourceAddedAmount);

                    if (selectedSkill.RequireVitalAboveX || selectedSkill.RequireVitalBelowX)
                    {

                        GUILayout.BeginHorizontal();
                        var aboveBelow = selectedSkill.RequireVitalAboveX ? " Above " : " Below ";
                        currentRank.VitalConditionParamater =
                            EditorGUILayout.Slider("- Vital" + aboveBelow + "% To Cast:",
                                                   currentRank.VitalConditionParamater, 0.0f, 1.0f);
                        GUILayout.Label((currentRank.VitalConditionParamater * 100) + "%");
                        GUILayout.EndHorizontal();
                    }

                    if (!new[] { SkillType.Aura,SkillType.Spawn }.Any(s => s == selectedSkill.SkillType))
                    {
                        currentRank.ApplyDOTOnHit = RPGMakerGUI.Toggle("Apply DoT on Hit?", currentRank.ApplyDOTOnHit);
                        if (currentRank.ApplyDOTOnHit)
                        {
                            currentRank.DamageOverTime.DoTName = RPGMakerGUI.TextField("- DoT Name:",
                                                                                       currentRank.DamageOverTime.DoTName);
                            currentRank.DamageOverTime.TimeBetweenTick = RPGMakerGUI.FloatField("- Time Between Tick:",
                                                                                                currentRank.DamageOverTime.
                                                                                                    TimeBetweenTick);
                            currentRank.DamageOverTime.Duration = RPGMakerGUI.FloatField("  - Duration:",
                                                                                            currentRank.DamageOverTime.
                                                                                                Duration);
                            currentRank.ChanceToApplyDOT = RPGMakerGUI.FloatField("  - Chance to apply:",
                                                                                            currentRank.ChanceToApplyDOT);

                            GUILayout.BeginHorizontal();
                            gameObject = RPGMakerGUI.PrefabSelector("DoT Active Prefab", gameObject, ref currentRank.DamageOverTime.ActivePrefab);
                            gameObject = RPGMakerGUI.PrefabGeneratorButton(Rmh_PrefabType.Effect_Active, gameObject, ref currentRank.DamageOverTime.ActivePrefab);

                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            gameObject = RPGMakerGUI.PrefabSelector("DoT OnActivate Prefab", gameObject, ref currentRank.DamageOverTime.OnActivatePrefab);
                            gameObject = RPGMakerGUI.PrefabGeneratorButton(Rmh_PrefabType.Effect_Activated, gameObject, ref currentRank.DamageOverTime.OnActivatePrefab);
                            GUILayout.EndHorizontal(); 

                            GUILayout.BeginHorizontal();
                            gameObject = RPGMakerGUI.PrefabSelector("DoT Damage Tick Prefab", gameObject, ref currentRank.DamageOverTime.DamageTickPrefab);
                            gameObject = RPGMakerGUI.PrefabGeneratorButton(Rmh_PrefabType.DoT_Damage_Tick, gameObject, ref currentRank.DamageOverTime.DamageTickPrefab);

                            GUILayout.EndHorizontal(); 
                            GUILayout.BeginHorizontal();
                            gameObject = RPGMakerGUI.PrefabSelector("DoT OnExpired Prefab", gameObject, ref currentRank.DamageOverTime.OnExpiredPrefab);
                            gameObject = RPGMakerGUI.PrefabGeneratorButton(Rmh_PrefabType.Effect_Expired, gameObject, ref currentRank.DamageOverTime.OnExpiredPrefab);
                            GUILayout.EndHorizontal(); 

                            #region DotDamage

                            #region CheckForUpdates

                            var dotDmgList = currentRank.DamageOverTime.DamagePerTick.ElementalDamages;
                            foreach (var d in Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions)
                            {
                                var tier = dotDmgList.FirstOrDefault(t => t.ElementID == d.ID);
                                if (tier == null)
                                {
                                    var tierToAdd = new ElementalDamage() { ElementID = d.ID };
                                    dotDmgList.Add(tierToAdd);
                                }
                            }

                            for (int index = 0; index < dotDmgList.Count; index++)
                            {
                                var v = dotDmgList[index];
                                var stillExists =
                                    Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions.FirstOrDefault(
                                        t => t.ID == v.ElementID);

                                if (stillExists == null)
                                {
                                    dotDmgList.Remove(v);
                                    index--;
                                }
                            }

                            #endregion

                            GUILayout.BeginHorizontal();
                            RPGMakerGUI.Label("- Physical Damage Per Tick:");
                            if (Rm_RPGHandler.Instance.Items.DamageHasVariance)
                            {
                                currentRank.DamageOverTime.DamagePerTick.MinDamage = RPGMakerGUI.IntField("",
                                                                                                          currentRank.
                                                                                                              DamageOverTime
                                                                                                              .DamagePerTick
                                                                                                              .MinDamage);
                                GUILayout.Label(" - ");
                            }
                            currentRank.DamageOverTime.DamagePerTick.MaxDamage = RPGMakerGUI.IntField("",
                                                                                                      currentRank.
                                                                                                          DamageOverTime.
                                                                                                          DamagePerTick.
                                                                                                          MaxDamage);
                            GUILayout.EndHorizontal();

                            foreach (var eleDmg in dotDmgList)
                            {
                                var nameOfEleDmg =
                                    Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions.First(
                                        e => e.ID == eleDmg.ElementID).Name;

                                GUILayout.BeginHorizontal();
                                RPGMakerGUI.Label("- " + nameOfEleDmg + " Damage Per Tick:");
                                if (Rm_RPGHandler.Instance.Items.DamageHasVariance)
                                {
                                    eleDmg.MinDamage = RPGMakerGUI.IntField("", eleDmg.MinDamage);
                                    GUILayout.Label(" - ");
                                }
                                eleDmg.MaxDamage = RPGMakerGUI.IntField("", eleDmg.MaxDamage);
                                GUILayout.EndHorizontal();
                            }

                            #endregion

                        }

                        currentRank.HasProcEffect = RPGMakerGUI.Toggle("Has Proc?", currentRank.HasProcEffect);
                        if (currentRank.HasProcEffect)
                        {
                            var procEffect = currentRank.ProcEffect;

                            procEffect.ProcCondition =
                                (Rm_ProcCondition)RPGMakerGUI.EnumPopup("Proc Condition", procEffect.ProcCondition);
                            if (procEffect.ProcCondition == Rm_ProcCondition.Every_N_Hits)
                            {
                                procEffect.Parameter = RPGMakerGUI.FloatField("N:", procEffect.Parameter);
                                procEffect.Parameter = (int)procEffect.Parameter;
                            }
                            if (procEffect.ProcCondition == Rm_ProcCondition.Chance_On_Hit ||
                                procEffect.ProcCondition == Rm_ProcCondition.Chance_On_Critical_Hit)
                            {
                                procEffect.Parameter = RPGMakerGUI.FloatField("% Chance:", procEffect.Parameter);

                            }
                            procEffect.ProcEffectType =
                                (Rm_ProcEffectType)RPGMakerGUI.EnumPopup("Effect:", procEffect.ProcEffectType);

                            if (procEffect.ProcEffectType == Rm_ProcEffectType.StatusEffect ||
                                procEffect.ProcEffectType == Rm_ProcEffectType.StatusEffectOnSelf)
                            {
                                RPGMakerGUI.PopupID<StatusEffect>("Status Effect:", ref procEffect.EffectParameterString);
                            }

                            if (procEffect.ProcEffectType == Rm_ProcEffectType.CastSkill ||
                                procEffect.ProcEffectType == Rm_ProcEffectType.CastSkillOnSelf)
                            {
                                RPGMakerGUI.PopupID<Skill>("Skill Name:", ref procEffect.EffectParameterString);
                            }

                            if (procEffect.ProcEffectType == Rm_ProcEffectType.KnockBack ||
                                procEffect.ProcEffectType == Rm_ProcEffectType.KnockUp)
                            {
                                procEffect.EffectParameter = RPGMakerGUI.FloatField("Distance:",
                                                                                    procEffect.EffectParameter);
                            }

                            if (procEffect.ProcEffectType == Rm_ProcEffectType.PullTowards)
                            {
                                //if : targetlock enabled || is ability and stayinplace || restoration and stayinplace?
                                if (selectedSkill.MovementType != SkillMovementType.StayInPlace || Rm_RPGHandler.Instance.Combat.TargetStyle == TargetStyle.TargetLock)
                                {
                                    if (selectedSkill.SkillType == SkillType.Projectile)
                                    {
                                        procEffect.PullType = Rm_PullTowardsType.CasterOrSkill;
                                    }
                                    else
                                    {
                                        procEffect.PullType = (Rm_PullTowardsType)RPGMakerGUI.EnumPopup("Pull Type:", procEffect.PullType);
                                    }
                                }
                                else
                                {
                                    procEffect.PullType = Rm_PullTowardsType.CasterOrSkill;
                                }

                                if (!RPGMakerGUI.Toggle("- Pull All The Way?", ref procEffect.PullAllTheWay))
                                {
                                    procEffect.EffectParameter = RPGMakerGUI.FloatField("- Pull Distance:",
                                                                                        procEffect.EffectParameter);
                                }
                            }
                        }
                        if (RPGMakerGUI.Toggle("Run Event On Hit?",ref currentRank.RunEventOnHit))
                        {
                            RPGMakerGUI.PopupID<NodeChain>("Event:", ref currentRank.EventOnHitID,1);
                        }

                        if (RPGMakerGUI.Toggle("Apply Status Effect",ref currentRank.ApplyStatusEffect))
                        {
                            RPGMakerGUI.PopupID<StatusEffect>("Status Effect:", ref currentRank.StatusEffectID, 1);
                            if(RPGMakerGUI.Toggle("With Duration?", 1, ref currentRank.ApplyStatusEffectWithDuration))
                            {
                                currentRank.ApplyStatusEffectDuration = RPGMakerGUI.FloatField(" - Duration:", currentRank.ApplyStatusEffectDuration);
                            }

                            GUILayout.BeginHorizontal();
                            currentRank.ChanceToApplyStatusEffect = EditorGUILayout.Slider("- Chance To Apply:",
                                                                                           currentRank.
                                                                                               ChanceToApplyStatusEffect,
                                                                                           0.0f, 1.0f);
                            GUILayout.Label((currentRank.ChanceToApplyStatusEffect * 100) + "%");
                            GUILayout.EndHorizontal();
                        }

                        if (RPGMakerGUI.Toggle("Remove Status Effect",ref currentRank.RemoveStatusEffect))
                        {
                            RPGMakerGUI.PopupID<StatusEffect>("Status Effect:", ref currentRank.RemoveStatusEffectID, 1);
                            GUILayout.BeginHorizontal();
                            currentRank.ChanceToRemoveStatusEffect = EditorGUILayout.Slider("- Chance To Remove:",
                                                                                            currentRank.
                                                                                                ChanceToRemoveStatusEffect,
                                                                                            0.0f, 1.0f);
                            GUILayout.Label((currentRank.ChanceToRemoveStatusEffect * 100) + "%");
                            GUILayout.EndHorizontal();
                        }


                    }

                    if (selectedSkill.SkillType == SkillType.Ability)
                    {
                        currentRank.GivePlayerItem = RPGMakerGUI.Toggle("Give Player Item?", currentRank.GivePlayerItem);
                        if (currentRank.GivePlayerItem)
                        {
                            RPGMakerGUI.PopupID<Item>("- Item To Give:", ref currentRank.ItemToGiveID);
                            if (!string.IsNullOrEmpty(currentRank.ItemToGiveID))
                            {
                                var item = Rm_RPGHandler.Instance.Repositories.Items.Get(currentRank.ItemToGiveID);
                                var stackable = item as IStackable;
                                if (stackable != null)
                                {
                                    currentRank.ItemToGiveAmount = RPGMakerGUI.IntField("- Amount To Give", currentRank.ItemToGiveAmount);
                                }
                            }
                        }
                    }

                    if (RPGMakerGUI.Foldout(ref showTooltip, "Description/Tooltip"))
                    {
                        if (RPGMakerGUI.Toggle("Show Help?", ref showTooltipHelp))
                        {
                            RPGMakerGUI.HelpNonRichText("[Color] E.g. <color=red> [TEXT_HERE] </color> OR <color=#00ffffff> [TEXT_HERE] </color>", 0);
                            RPGMakerGUI.HelpNonRichText("[Size] E.g. <size=25> [TEXT_HERE] </size> OR <size=10> [TEXT_HERE] </size>", 0);
                            RPGMakerGUI.HelpNonRichText("[Bold] E.g. <b> [TEXT_HERE] </b>", 0);
                            RPGMakerGUI.HelpNonRichText("[Italic] E.g. <i> [TEXT_HERE] </i>", 0);

                            RPGMakerGUI.Help("[Text replacement] You can replace text from the source text to the actual text show in-game.", 0);
                            RPGMakerGUI.Help("[Text replacement] The preview below will show this as VAL, ingame it will show the actual values.", 0);
                            RPGMakerGUI.Help("", 0);

                            RPGMakerGUI.Help("[Vital] \"{Vital_[VITAL_NAME]_Current}\"", 0);
                            RPGMakerGUI.Help("[Vital] \"{Vital_[VITAL_NAME]_Max}\"", 0);
                            RPGMakerGUI.Help("[Vital] \"{Vital_[VITAL_NAME]_Base}\"", 0);
                            RPGMakerGUI.Help("[Vital] \"{Vital_[VITAL_NAME]_Skill}\"", 0);
                            RPGMakerGUI.Help("[Vital] \"{Vital_[VITAL_NAME]_Equip}\"", 0);
                            RPGMakerGUI.Help("[Vital] \"{Vital_[VITAL_NAME]_Attr}\"", 0);
                            RPGMakerGUI.Help("[Vital] E.g. \"{Vital_Health_Max}\" will be replaced by the total [Health] from all sources", 0);
                            RPGMakerGUI.Help("[Vital] E.g. \"{Vital_Mana_Attr}\" will show any vital buffs for [Mana] gained from attributes", 0);
                            RPGMakerGUI.Help("[Vital] E.g. \"{Vital_Rage_Equip}\" will show any vital buffs for [Rage] gained from attributes", 0);
                            RPGMakerGUI.Help("", 0);

                            RPGMakerGUI.Help("[Attribute] \"{Attr_[ATTRIBUTE_NAME]}\"", 0);
                            RPGMakerGUI.Help("[Attribute] \"{Attr_[ATTRIBUTE_NAME]_Base}\"", 0);
                            RPGMakerGUI.Help("[Attribute] \"{Attr_[ATTRIBUTE_NAME]_Skill}\"", 0);
                            RPGMakerGUI.Help("[Attribute] \"{Attr_[ATTRIBUTE_NAME]_Equip}\"", 0);
                            RPGMakerGUI.Help("[Attribute] E.g. \"{Attr_Strength}\" will be replaced by the player's total [Strength] from all sources", 0);
                            RPGMakerGUI.Help("[Attribute] E.g. \"{Attr_Dexterity_Base}\" will show base [Dexterity] before skill/equipment buffs are added", 0);
                            RPGMakerGUI.Help("[Attribute] E.g. \"{Attr_Intelligence_Equip}\" will show any buffs to [Intelligence] gained from equipment", 0);
                            RPGMakerGUI.Help("", 0);

                            RPGMakerGUI.Help("[Statistics] \"{Stat_[STATISTIC_NAME]}\"", 0);
                            RPGMakerGUI.Help("[Statistics] \"{Stat_[STATISTIC_NAME]_Base}\"", 0);
                            RPGMakerGUI.Help("[Statistics] \"{Stat_[STATISTIC_NAME]_Skill}\"", 0);
                            RPGMakerGUI.Help("[Statistics] \"{Stat_[STATISTIC_NAME]_Equip}\"", 0);
                            RPGMakerGUI.Help("[Statistics] \"{Stat_[STATISTIC_NAME]_Attr}\"", 0);
                            RPGMakerGUI.Help("[Statistics] Do not forget the spaces in [STATISTC_NAME]", 0);
                            RPGMakerGUI.Help("[Statistics] E.g. \"{Stat_Attack Speed}\" will be replaced by the player's total [Attack Speed] from all sources", 0);
                            RPGMakerGUI.Help("[Statistics] E.g. \"{Stat_Movement Speed_Base}\" will show base [Movement Speed] before skill/equipment/attribute buffs are added", 0);
                            RPGMakerGUI.Help("[Statistics] E.g. \"{Stat_Critical Chance_Equip}\" will show any buffs to [Critical Chance] gained from equipment", 0);


                            RPGMakerGUI.Help("", 0);
                        }

                        //EditorGUILayout.HelpBox("Help text here etc \n oo \noooo \nooosadosa", MessageType.Info, true);
                        //Two side by side textareas
                        GUILayout.BeginHorizontal();

                        GUILayout.BeginVertical();
                        RPGMakerGUI.Label("Source:");
                        selectedSkill.Description = RPGMakerGUI.TextArea("", selectedSkill.Description);
                        GUILayout.EndVertical();

                        GUILayout.BeginVertical();
                        RPGMakerGUI.Label("Preview:");
                        RPGMakerGUI.RichTextArea("", selectedSkill.DescriptionFormatted);
                        GUILayout.EndVertical();

                        GUILayout.EndHorizontal();

                        RPGMakerGUI.EndFoldout();
                    }

                    #endregion
                    RPGMakerGUI.EndFoldout();
                }



                if (RPGMakerGUI.Foldout(ref showAnimationInfo, "Player Animations"))
                {

                    RPGMakerGUI.Toggle("Get Animations From Selected Object?", ref UseSelectedForAnims);

                    #region CheckForUpdates
                    var animList = selectedSkill.AnimationsToUse;
                    foreach (var d in Rm_RPGHandler.Instance.Player.CharacterDefinitions)
                    {
                        var tier = animList.FirstOrDefault(t => t.ClassID == d.ID);
                        if (tier == null)
                        {

                            foreach (var className in d.ApplicableClassIDs)
                            {
                                if (selectedSkill.ClassIDs.Contains(className.ID) || selectedSkill.AllClasses)
                                {
                                    var tierToAdd = new SkillAnimationDefinition() { ClassID = d.ID };
                                    animList.Add(tierToAdd); 
                                    break;
                                }
                            }
                        }
                    }

                    for (int index = 0; index < animList.Count; index++)
                    {
                        var v = animList[index];
                        var stillExists =
                            Rm_RPGHandler.Instance.Player.CharacterDefinitions.FirstOrDefault(
                                t => t.ID == v.ClassID);

                        if (stillExists == null || (!selectedSkill.ClassIDs.ContainsAnyOf(stillExists.ApplicableClassIDs) && !selectedSkill.AllClasses))
                        {
                            animList.Remove(v);
                            index--;
                        }
                    }
                    #endregion

                    #region Animation
                    var foundAnim = false;
                    String[] anims = new string[0];
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
                    #endregion

                    if(!UseSelectedForAnims)
                    {
                        foundAnim = false;
                    }

                    if(selectedSkill.SkillType != SkillType.Melee)
                    {
                        for (int index = 0; index < animList.Count; index++)
                        {
                            var anim = animList[index];
                            var classInfo = Rm_RPGHandler.Instance.Player.CharacterDefinitions.First(c => c.ID == anim.ClassID);
                            var className = classInfo.Name;
                            RPGMakerGUI.SubTitle(className + " Animations:");

                            #region "Non-melee animations"
                            if (foundAnim)
                            {
                                #region foundAnim

                                selectedAnim[index] = anims.FirstOrDefault(s => s == anim.LegacyAnim) != null
                                                            ? Array.IndexOf(anims, anim.LegacyAnim)
                                                            : 0;

                                selectedAnim[index] = EditorGUILayout.Popup("Cast Animation:", selectedAnim[index], anims);
                                anim.LegacyAnim = anims[selectedAnim[index]];
                                if (anim.LegacyAnim == "None") anim.LegacyAnim = "";

                                selectedClassIndex[index] = anims.FirstOrDefault(s => s == anim.CastingLegacyAnim) != null
                                                            ? Array.IndexOf(anims, anim.CastingLegacyAnim)
                                                            : 0;

                                selectedClassIndex[index] = EditorGUILayout.Popup("Casting Animation:", selectedClassIndex[index], anims);
                                anim.CastingLegacyAnim = anims[selectedClassIndex[index]];
                                if (anim.CastingLegacyAnim == "None") anim.CastingLegacyAnim = "";

                                if (selectedSkill.MovementType != SkillMovementType.StayInPlace)
                                {
                                    if (selectedSkill.MovementType != SkillMovementType.TeleportTo)
                                    {
                                        selectedAnim[index] = anims.FirstOrDefault(s => s == anim.ApproachLegacyAnim) != null
                                                                  ? Array.IndexOf(anims, anim.ApproachLegacyAnim)
                                                                  : 0;

                                        selectedAnim[index] = EditorGUILayout.Popup("Move Animation:", selectedAnim[index], anims);
                                        anim.ApproachLegacyAnim = anims[selectedAnim[index]];
                                        if (anim.ApproachLegacyAnim == "None") anim.ApproachLegacyAnim = "";
                                    }


                                    selectedAnim[index] = anims.FirstOrDefault(s => s == anim.LandLegacyAnim) != null
                                                                ? Array.IndexOf(anims, anim.LandLegacyAnim)
                                                                : 0;

                                    selectedAnim[index] = EditorGUILayout.Popup("Land Animation:", selectedAnim[index], anims);
                                    anim.LandLegacyAnim = anims[selectedAnim[index]];
                                    if (anim.LandLegacyAnim == "None") anim.LandLegacyAnim = "";
                                }

                                #endregion
                            }
                            else
                            {
                                if (classInfo.AnimationType == RPGAnimationType.Legacy)
                                {
                                    anim.LegacyAnim = RPGMakerGUI.TextField("Cast Animation:", anim.LegacyAnim);
                                    anim.CastingLegacyAnim = RPGMakerGUI.TextField("Casting Animation:", anim.CastingLegacyAnim);

                                    if (selectedSkill.MovementType != SkillMovementType.StayInPlace)
                                    {
                                        if (selectedSkill.MovementType != SkillMovementType.TeleportTo)
                                        {
                                            anim.ApproachLegacyAnim = RPGMakerGUI.TextField("Move Animation:", anim.ApproachLegacyAnim);
                                        }
                                        anim.LandLegacyAnim = RPGMakerGUI.TextField("Land Animation:", anim.LandLegacyAnim);
                                    }
                                }
                                else
                                {
                                   
                                    GUILayout.BeginHorizontal();
                                    RPGMakerGUI.Label("Cast Animation");
                                    anim.CastSkillAnimSet = RPGMakerGUI.IntField("SkillAnimSet:", anim.CastSkillAnimSet);
                                    anim.CastAnimNumber = RPGMakerGUI.IntField("AnimNumber:", anim.CastAnimNumber);
                                    GUILayout.EndHorizontal();

                                    
                                    GUILayout.BeginHorizontal();
                                    RPGMakerGUI.Label("Casting Animation");
                                    anim.CastingSkillAnimSet = RPGMakerGUI.IntField("SkillAnimSet:", anim.CastingSkillAnimSet);
                                    anim.CastingAnimNumber = RPGMakerGUI.IntField("AnimNumber:", anim.CastingAnimNumber);
                                    GUILayout.EndHorizontal();

                                    if (selectedSkill.MovementType != SkillMovementType.StayInPlace)
                                    {
                                        if (selectedSkill.MovementType != SkillMovementType.TeleportTo)
                                        {
                                            GUILayout.BeginHorizontal();    
                                            RPGMakerGUI.Label("Approach Animation");
                                            anim.ApproachSkillAnimSet = RPGMakerGUI.IntField("SkillAnimSet:", anim.ApproachSkillAnimSet);
                                            anim.ApproachAnimNumber = RPGMakerGUI.IntField("AnimNumber:", anim.ApproachAnimNumber);
                                            GUILayout.EndHorizontal();

                                        }
                                        GUILayout.BeginHorizontal();
                                        RPGMakerGUI.Label("Land Animation");
                                        anim.LandSkillAnimSet = RPGMakerGUI.IntField("SkillAnimSet:", anim.LandSkillAnimSet);
                                        anim.LandAnimNumber = RPGMakerGUI.IntField("AnimNumber:", anim.LandAnimNumber);
                                        GUILayout.EndHorizontal();

                                    }
                                }
                                
                            }
                            #endregion

                        }
                    }
                    else
                    {
                        var meleeSkill = (MeleeSkill) selectedSkill;
                        var meleeAnims = meleeSkill.MeleeAnimations;

                        for (int i = 0; i < meleeAnims.Count; i++)
                        {
                            var definitions = meleeAnims[i].Definitions;

                            var classInfo = Rm_RPGHandler.Instance.Player.CharacterDefinitions.First(c => c.ID == meleeAnims[i].ClassID);
                            var className = classInfo.Name;

                            RPGMakerGUI.SubTitle(className + " Animations:");


                            #region "Melee animations"
                            if (classInfo.AnimationType == RPGAnimationType.Legacy && foundAnim)
                            {
                                #region t

                                for (int x = 0; x < definitions.Count; x++)
                                {
                                    var definition = definitions[x];
                                    var atkPrefix = "Attack [" + x + "] - ";
                                    var selectedMeleeDef = selectedSkill as MeleeSkill;
                                    var moveDef = selectedMeleeDef.Details.MeleeMoveDefinitions[x];
                                   

                                    selectedAnim[x] = anims.FirstOrDefault(s => s == definition.LegacyAnim) != null
                                ? Array.IndexOf(anims, definition.LegacyAnim)
                                : 0;

                                    if (!meleeSkill.SeperateCastPerAttack && x == 0 )
                                    {
                                        selectedClassIndex[x] = anims.FirstOrDefault(s => s == definition.CastingLegacyAnim) != null
                                                                    ? Array.IndexOf(anims, definition.CastingLegacyAnim)
                                                                    : 0;

                                        selectedClassIndex[x] = EditorGUILayout.Popup(atkPrefix + "Casting Animation:", selectedClassIndex[x], anims);
                                        definition.CastingLegacyAnim = anims[selectedClassIndex[x]];
                                        if (definition.CastingLegacyAnim == "None") definition.CastingLegacyAnim = "";
                                    }

                                    selectedAnim[x] = EditorGUILayout.Popup(atkPrefix + "Cast Animation:", selectedAnim[x], anims);
                                    definition.LegacyAnim = anims[selectedAnim[x]];
                                    if (definition.LegacyAnim == "None") definition.LegacyAnim = "";

                                    if(meleeSkill.SeperateCastPerAttack)
                                    {
                                        selectedClassIndex[x] = anims.FirstOrDefault(s => s == definition.CastingLegacyAnim) != null
                                                                    ? Array.IndexOf(anims, definition.CastingLegacyAnim)
                                                                    : 0;

                                        selectedClassIndex[x] = EditorGUILayout.Popup(atkPrefix + "Casting Animation:", selectedClassIndex[x], anims);
                                        definition.CastingLegacyAnim = anims[selectedClassIndex[x]];
                                        if (definition.CastingLegacyAnim == "None") definition.CastingLegacyAnim = "";
                                    }

                                    if (moveDef.MovementType != SkillMovementType.StayInPlace)
                                    {
                                        if (selectedSkill.MovementType != SkillMovementType.TeleportTo)
                                        {
                                            selectedAnim[x] = anims.FirstOrDefault(s => s == definition.ApproachLegacyAnim) != null
                                                                  ? Array.IndexOf(anims, definition.ApproachLegacyAnim)
                                                                  : 0;

                                            selectedAnim[x] = EditorGUILayout.Popup(atkPrefix + "Move Animation:", selectedAnim[x], anims);
                                            definition.ApproachLegacyAnim = anims[selectedAnim[x]];
                                            if (definition.ApproachLegacyAnim == "None") definition.ApproachLegacyAnim = "";
                                        }

                                        selectedAnim[x] = anims.FirstOrDefault(s => s == definition.LandLegacyAnim) != null
                                                                    ? Array.IndexOf(anims, definition.LandLegacyAnim)
                                                                    : 0;

                                        selectedAnim[x] = EditorGUILayout.Popup(atkPrefix + "Land Animation:", selectedAnim[x], anims);
                                        definition.LandLegacyAnim = anims[selectedAnim[x]];
                                        if (definition.LandLegacyAnim == "None") definition.LandLegacyAnim = "";
                                    }
                                }

                                #endregion

                            }
                            else
                            {
                                for (int x = 0; x < definitions.Count; x++)
                                {
                                    var definition = definitions[x];
                                    var atkPrefix = "Attack [" + x + "] - ";
                                    var selectedMeleeDef = selectedSkill as MeleeSkill;
                                    var moveDef = selectedMeleeDef.Details.MeleeMoveDefinitions[x];

                                    if(classInfo.AnimationType == RPGAnimationType.Legacy)
                                    {
                                        if (!meleeSkill.SeperateCastPerAttack && x == 0)
                                        {
                                            definition.CastingLegacyAnim = RPGMakerGUI.TextField(atkPrefix + "Casting Animation:", definition.CastingLegacyAnim);
                                        }

                                        definition.LegacyAnim = RPGMakerGUI.TextField(atkPrefix + "Cast Animation:", definition.LegacyAnim);

                                        if (meleeSkill.SeperateCastPerAttack)
                                        {
                                            definition.CastingLegacyAnim = RPGMakerGUI.TextField(atkPrefix + "Casting Animation:", definition.CastingLegacyAnim);
                                        }

                                        if (moveDef.MovementType != SkillMovementType.StayInPlace)
                                        {
                                            if (moveDef.MovementType != SkillMovementType.TeleportTo)
                                            {
                                                definition.ApproachLegacyAnim = RPGMakerGUI.TextField(atkPrefix + "Move Animation:", definition.ApproachLegacyAnim);
                                            }
                                            definition.LandLegacyAnim = RPGMakerGUI.TextField(atkPrefix + "Land Animation:", definition.LandLegacyAnim);
                                        }
                                    }
                                    else
                                    {
                                        if (!meleeSkill.SeperateCastPerAttack && x == 0)
                                        {
                                            GUILayout.BeginHorizontal();
                                            RPGMakerGUI.Label(atkPrefix + " CastingAnim");
                                            definition.CastingSkillAnimSet = RPGMakerGUI.IntField("SkillAnimSet:", definition.CastingSkillAnimSet);
                                            definition.CastingAnimNumber = RPGMakerGUI.IntField("AnimNumber:", definition.CastingAnimNumber);
                                            GUILayout.EndHorizontal();
                                        }


                                        GUILayout.BeginHorizontal();
                                        RPGMakerGUI.Label(atkPrefix + " CastAnim");
                                        definition.CastSkillAnimSet = RPGMakerGUI.IntField("SkillAnimSet:", definition.CastSkillAnimSet);
                                        definition.CastAnimNumber = RPGMakerGUI.IntField("AnimNumber:", definition.CastAnimNumber);
                                        GUILayout.EndHorizontal();

                                        if (meleeSkill.SeperateCastPerAttack)
                                        {
                                            GUILayout.BeginHorizontal();
                                            RPGMakerGUI.Label(atkPrefix + " CastingAnim");
                                            definition.CastingSkillAnimSet = RPGMakerGUI.IntField("SkillAnimSet:", definition.CastingSkillAnimSet);
                                            definition.CastingAnimNumber = RPGMakerGUI.IntField("AnimNumber:", definition.CastingAnimNumber);
                                            GUILayout.EndHorizontal();

                                        }

                                        if (moveDef.MovementType != SkillMovementType.StayInPlace)
                                        {
                                            if (moveDef.MovementType != SkillMovementType.TeleportTo)
                                            {
                                                GUILayout.BeginHorizontal();
                                                RPGMakerGUI.Label(atkPrefix + " ApproachAnim");
                                                definition.ApproachSkillAnimSet = RPGMakerGUI.IntField("SkillAnimSet:", definition.ApproachSkillAnimSet);
                                                definition.ApproachAnimNumber = RPGMakerGUI.IntField("AnimNumber:", definition.ApproachAnimNumber);
                                                GUILayout.EndHorizontal();

                                            }
                                            GUILayout.BeginHorizontal();
                                            RPGMakerGUI.Label(atkPrefix + " LandAnim");
                                            definition.LandSkillAnimSet = RPGMakerGUI.IntField("SkillAnimSet:", definition.LandSkillAnimSet);
                                            definition.LandAnimNumber = RPGMakerGUI.IntField("AnimNumber:", definition.LandAnimNumber);
                                            GUILayout.EndHorizontal();

                                        }
                                    }
                                    
                                }
                            }

                            #endregion
                        }
                    }


                    RPGMakerGUI.EndFoldout();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Add or select a new field to customise skills.", MessageType.Info);
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private static GenericMenu.MenuFunction2 AddSkill()
        {
            return AddSkill;
        }

        private static void AddSkill(object userData)
        {
            var skillType = (SkillType)userData;
            Skill skill;
            switch (skillType)
            {
                case SkillType.Area_Of_Effect:
                    skill = new AreaOfEffectSkill();
                    break;
                case SkillType.Projectile:
                    skill = new ProjectileSkill();
                    break;
                case SkillType.Aura:
                    skill = new AuraSkill();
                    break;
                case SkillType.Spawn:
                    skill = new Skill() { Name = "New Spawn Skill", SkillType = SkillType.Spawn };
                    break;
                case SkillType.Melee:
                    skill = new MeleeSkill();
                    break;
                case SkillType.Restoration:
                    skill = new RestorationSkill();
                    break;
                case SkillType.Ability:
                    skill = new Skill() { Name = "New Ability Skill", SkillType = SkillType.Ability};
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            listOfSkills.Add(skill);
            selectedSkill = skill;
            GUI.FocusControl("");
        }

        #endregion

        #region Talents

        private static Talent selectedTalent = null;
        private static Vector2 talentScrollPos = Vector2.zero;
        private static bool showTalentDetails = true;
        private static bool showTalentRankDetails = true;
        private static bool reqSkillFoldout = true;
        private static bool showTooltipHelp = true;
        public static void Talents(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            var listk = Rm_RPGHandler.Instance.Repositories.Talents.AllTalents;
            GUI.Box(leftArea, "", "backgroundBox");
            GUI.Box(mainArea, "", "backgroundBoxMain");

            GUILayout.BeginArea(PadRect(leftArea, 0, 0));
            RPGMakerGUI.ListArea(listk, ref selectedTalent, Rm_ListAreaType.Talents, false, true);
            GUILayout.EndArea();


            GUILayout.BeginArea(mainArea);
            talentScrollPos = GUILayout.BeginScrollView(talentScrollPos);
            RPGMakerGUI.Title("Talents");
            if (selectedTalent != null)
            {
                if (RPGMakerGUI.Foldout(ref showTalentDetails, "Talent Details"))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical(GUILayout.MaxWidth(85));
                    selectedTalent.Image.Image = RPGMakerGUI.ImageSelector("", selectedTalent.Image.Image,
                                                                           ref selectedTalent.Image.ImagePath);

                    GUILayout.EndVertical();
                    GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

                    selectedTalent.Name = RPGMakerGUI.TextField("Name: ", selectedTalent.Name);
                    selectedTalent.UpgradeType =
                        (SkillUpgradeType)RPGMakerGUI.EnumPopup("Upgrade Type:", selectedTalent.UpgradeType);
                    //if (selectedTalent.InTalentGroup = RPGMakerGUI.Toggle("In Group:", selectedTalent.InTalentGroup))
                    //{
                    //    RPGMakerGUI.PopupID<TalentGroup>("- Talent Group", ref selectedTalent.TalentGroupID);
                    //}

                    selectedTalent.CanToggle = RPGMakerGUI.Toggle("Can Toggle?", selectedTalent.CanToggle);

                    if (selectedTalent.UpgradeType == SkillUpgradeType.TraitLevel)
                    {
                        RPGMakerGUI.PopupID<Rm_TraitDefintion>("Trait:", ref selectedTalent.TraitIDToLevel);
                    }

                    if (RPGMakerGUI.Toggle("Automatically Unlock At Level?", ref selectedTalent.AutomaticallyUnlockAtLevel))
                    {
                        selectedTalent.LevelToAutomaticallyUnlock = RPGMakerGUI.IntField("- Level:", selectedTalent.LevelToAutomaticallyUnlock);
                    }

                    selectedTalent.AllClasses = RPGMakerGUI.Toggle("All Classes?", selectedTalent.AllClasses);
                    if (!selectedTalent.AllClasses)
                    {
                        var result = RPGMakerGUI.ToolBar("Classes:", new[] { "+Class" });
                        if (showSkillClasses)
                        {
                            GUILayout.BeginVertical("foldoutBox");
                            if (selectedTalent.ClassIDs.Count == 0)
                            {
                                EditorGUILayout.HelpBox("Click +Class to add a class that can use this talent",
                                                        MessageType.Info);
                            }

                            for (int i = 0; i < selectedTalent.ClassIDs.Count; i++)
                            {
                                GUILayout.BeginHorizontal();
                                var refString = selectedTalent.ClassIDs[i];
                                RPGMakerGUI.PopupID<Rm_ClassNameDefinition>("", ref refString);
                                selectedTalent.ClassIDs[i] = refString;

                                if (RPGMakerGUI.DeleteButton(15))
                                {
                                    selectedTalent.ClassIDs.Remove(selectedTalent.ClassIDs[i]);
                                    i--;
                                }
                                GUILayout.EndHorizontal();
                            }

                            if (result == 0)
                            {
                                selectedTalent.ClassIDs.Add("");
                            }
                            GUILayout.EndVertical();
                        }

                    }
                    GUILayout.Space(5);
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    RPGMakerGUI.EndFoldout();

                }

                #region RequiredTalents
                var reqSkillResult = RPGMakerGUI.FoldoutToolBar(ref reqSkillFoldout, "Required Talents To Unlock", new string[] { "+RequiredTalent" });
                if (reqSkillFoldout)
                {
                    for (int index = 0; index < selectedTalent.RequiredTalents.Count; index++)
                    {
                        var v = selectedTalent.RequiredTalents[index].ID;
                        if (string.IsNullOrEmpty(v)) continue;

                        var stillExists =
                            Rm_RPGHandler.Instance.Repositories.Talents.AllTalents.FirstOrDefault(
                                a => a.ID == v);

                        if (stillExists == null)
                        {
                            selectedTalent.RequiredTalents.Remove(selectedTalent.RequiredTalents[index]);
                            index--;
                        }
                    }
                    var allTalents = Rm_RPGHandler.Instance.Repositories.Talents.AllTalents.Where(t => t != selectedTalent).ToList();
                    if (allTalents.Count > 0)
                    {
                        if (selectedTalent.RequiredTalents.Count == 0)
                        {
                            EditorGUILayout.HelpBox("Click +RequiredTalent to add a talent that needs to be unlocked to unlock this.", MessageType.Info);
                        }
                        else
                        {
                            RPGMakerGUI.Toggle("Only Require One Talent?", ref selectedTalent.OnlyRequireOneTalent);
                        }

                        GUILayout.Space(5);
                        for (int index = 0; index < selectedTalent.RequiredTalents.Count; index++)
                        {
                            GUILayout.BeginHorizontal();
                            RPGMakerGUI.PopupID<Talent>("Talent:", ref selectedTalent.RequiredTalents[index].ID);

                            if (GUILayout.Button(RPGMakerGUI.DelIcon, "genericButton", GUILayout.Width(30), GUILayout.Height(30)))
                            {
                                selectedTalent.RequiredTalents.Remove(selectedTalent.RequiredTalents[index]);
                                index--;
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.Space(5);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No Other Talents Found.", MessageType.Info);
                        selectedTalent.RequiredTalents = new List<StringField>();
                    }

                    if (reqSkillResult == 0)
                    {
                        selectedTalent.RequiredTalents.Add(new StringField());
                    }
                    RPGMakerGUI.EndFoldout();
                }
                #endregion

                if (RPGMakerGUI.Foldout(ref showTalentRankDetails, "Rank Details"))
                {
                    while (selectedTalent.talentEffects.Count > selectedTalent.MaxRank)
                    {
                        selectedTalent.talentEffects.RemoveAt(selectedTalent.talentEffects.Count - 1);
                    }

                    while (selectedTalent.talentEffects.Count < selectedTalent.MaxRank)
                    {
                        if (selectedTalent.talentEffects.Count > 0)
                        {
                            var copyOfLast =
                                GeneralMethods.CopyObject(
                                    selectedTalent.talentEffects[selectedTalent.talentEffects.Count - 1]);
                            selectedTalent.talentEffects.Add(copyOfLast);
                        }
                        else
                        {
                            selectedTalent.talentEffects.Add(new TalentEffect());
                        }
                    }

                    if (selectedTalent.CurrentRank > selectedTalent.MaxRank)
                        selectedTalent.CurrentRank = selectedTalent.MaxRank - 1;

                    GUILayout.BeginHorizontal();
                    RPGMakerGUI.Label("Skill Rank " + (selectedTalent.CurrentRank + 1) + " / " + selectedTalent.MaxRank);
                    GUI.enabled = selectedTalent.CurrentRank > 0;
                    if (GUILayout.Button("Prev Rank", "genericButton", GUILayout.Width(100),
                                         GUILayout.Height(25)))
                    {
                        selectedTalent.CurrentRank--;
                        GUI.FocusControl("");
                    }
                    GUI.enabled = selectedTalent.CurrentRank + 1 < selectedTalent.MaxRank;
                    if (GUILayout.Button("Next Rank", "genericButton", GUILayout.Width(100),
                                         GUILayout.Height(25)))
                    {
                        selectedTalent.CurrentRank++;
                        GUI.FocusControl("");
                    }
                    GUI.enabled = true;
                    if (GUILayout.Button("+Rank", "genericButton", GUILayout.Width(50),
                                         GUILayout.Height(25)))
                    {
                        selectedTalent.MaxRank++;
                        selectedTalent.CurrentRank++;
                        GUI.FocusControl("");
                        return;
                    }
                    GUI.enabled = selectedTalent.MaxRank > 1;
                    if (GUILayout.Button("-Rank", "genericButton", GUILayout.Width(50),
                                         GUILayout.Height(25)))
                    {
                        selectedTalent.MaxRank--;
                        GUI.FocusControl("");
                    }
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();

                    var currentRank = selectedTalent.talentEffects[selectedTalent.CurrentRank];

                    switch (selectedTalent.UpgradeType)
                    {
                        case SkillUpgradeType.PlayerLevel:
                            currentRank.LevelReqToLevel = RPGMakerGUI.IntField("Player Level Required:",
                                                                               currentRank.LevelReqToLevel);
                            break;
                        case SkillUpgradeType.SkillPoints:
                            currentRank.SkillPointsToLevel = RPGMakerGUI.IntField("Skill Points to level:",
                                                                                  currentRank.SkillPointsToLevel);
                            break;
                        case SkillUpgradeType.TraitLevel:
                            currentRank.ReqTraitLevelToLevel = RPGMakerGUI.IntField("Trait Level Required:",
                                                                                    currentRank.ReqTraitLevelToLevel);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    //currentRank.IsAlsoSkill = RPGMakerGUI.Toggle("Is Also A Skill?", currentRank.IsAlsoSkill);
                    //if (currentRank.IsAlsoSkill)
                    //{
                    //    RPGMakerGUI.PopupID<Skill>("- Skill Name:", ref currentRank.SKillID);
                    //}

                    if (RPGMakerGUI.Foldout(ref showTooltip, "Description/Tooltip"))
                    {
                        if(RPGMakerGUI.Toggle("Show Help?", ref showTooltipHelp))
                        {
                            RPGMakerGUI.HelpNonRichText("[Color] E.g. <color=red> [TEXT_HERE] </color> OR <color=#00ffffff> [TEXT_HERE] </color>", 0);
                            RPGMakerGUI.HelpNonRichText("[Size] E.g. <size=25> [TEXT_HERE] </size> OR <size=10> [TEXT_HERE] </size>", 0);
                            RPGMakerGUI.HelpNonRichText("[Bold] E.g. <b> [TEXT_HERE] </b>", 0);
                            RPGMakerGUI.HelpNonRichText("[Italic] E.g. <i> [TEXT_HERE] </i>", 0);

                            RPGMakerGUI.Help("[Text replacement] You can replace text from the source text to the actual text show in-game.",0);
                            RPGMakerGUI.Help("[Text replacement] The preview below will show this as VAL, ingame it will show the actual values.",0);
                            RPGMakerGUI.Help("", 0);

                            RPGMakerGUI.Help("[Vital] \"{Vital_[VITAL_NAME]_Current}\"",0);
                            RPGMakerGUI.Help("[Vital] \"{Vital_[VITAL_NAME]_Max}\"", 0);
                            RPGMakerGUI.Help("[Vital] \"{Vital_[VITAL_NAME]_Base}\"", 0);
                            RPGMakerGUI.Help("[Vital] \"{Vital_[VITAL_NAME]_Skill}\"", 0);
                            RPGMakerGUI.Help("[Vital] \"{Vital_[VITAL_NAME]_Equip}\"", 0);
                            RPGMakerGUI.Help("[Vital] \"{Vital_[VITAL_NAME]_Attr}\"", 0);
                            RPGMakerGUI.Help("[Vital] E.g. \"{Vital_Health_Max}\" will be replaced by the total [Health] from all sources", 0);
                            RPGMakerGUI.Help("[Vital] E.g. \"{Vital_Mana_Attr}\" will show any vital buffs for [Mana] gained from attributes", 0);
                            RPGMakerGUI.Help("[Vital] E.g. \"{Vital_Rage_Equip}\" will show any vital buffs for [Rage] gained from attributes", 0);
                            RPGMakerGUI.Help("", 0);

                            RPGMakerGUI.Help("[Attribute] \"{Attr_[ATTRIBUTE_NAME]}\"", 0);
                            RPGMakerGUI.Help("[Attribute] \"{Attr_[ATTRIBUTE_NAME]_Base}\"", 0);
                            RPGMakerGUI.Help("[Attribute] \"{Attr_[ATTRIBUTE_NAME]_Skill}\"", 0);
                            RPGMakerGUI.Help("[Attribute] \"{Attr_[ATTRIBUTE_NAME]_Equip}\"", 0);
                            RPGMakerGUI.Help("[Attribute] E.g. \"{Attr_Strength}\" will be replaced by the player's total [Strength] from all sources", 0);
                            RPGMakerGUI.Help("[Attribute] E.g. \"{Attr_Dexterity_Base}\" will show base [Dexterity] before skill/equipment buffs are added", 0);
                            RPGMakerGUI.Help("[Attribute] E.g. \"{Attr_Intelligence_Equip}\" will show any buffs to [Intelligence] gained from equipment", 0);
                            RPGMakerGUI.Help("", 0);

                            RPGMakerGUI.Help("[Statistics] \"{Stat_[STATISTIC_NAME]}\"", 0);
                            RPGMakerGUI.Help("[Statistics] \"{Stat_[STATISTIC_NAME]_Base}\"", 0);
                            RPGMakerGUI.Help("[Statistics] \"{Stat_[STATISTIC_NAME]_Skill}\"", 0);
                            RPGMakerGUI.Help("[Statistics] \"{Stat_[STATISTIC_NAME]_Equip}\"", 0);
                            RPGMakerGUI.Help("[Statistics] \"{Stat_[STATISTIC_NAME]_Attr}\"", 0);
                            RPGMakerGUI.Help("[Statistics] Do not forget the spaces in [STATISTC_NAME]", 0);
                            RPGMakerGUI.Help("[Statistics] E.g. \"{Stat_Attack Speed}\" will be replaced by the player's total [Attack Speed] from all sources", 0);
                            RPGMakerGUI.Help("[Statistics] E.g. \"{Stat_Movement Speed_Base}\" will show base [Movement Speed] before skill/equipment/attribute buffs are added", 0);
                            RPGMakerGUI.Help("[Statistics] E.g. \"{Stat_Critical Chance_Equip}\" will show any buffs to [Critical Chance] gained from equipment", 0);


                            RPGMakerGUI.Help("",0);
                        }

                        //EditorGUILayout.HelpBox("Help text here etc \n oo \noooo \nooosadosa", MessageType.Info, true);
                        //Two side by side textareas
                        GUILayout.BeginHorizontal();

                        GUILayout.BeginVertical();
                        RPGMakerGUI.Label("Source:");
                        selectedTalent.Description = RPGMakerGUI.TextArea("", selectedTalent.Description);
                        GUILayout.EndVertical();

                        GUILayout.BeginVertical();
                        RPGMakerGUI.Label("Ingame:");
                        RPGMakerGUI.RichTextArea("", selectedTalent.DescriptionFormatted);
                        GUILayout.EndVertical();

                        GUILayout.EndHorizontal();

                        RPGMakerGUI.EndFoldout();
                    }

                    var currentEffect = currentRank.Effect;
                    PassiveEffectDetails(currentEffect);


                    RPGMakerGUI.EndFoldout();
                }

            }
            else
            {
                EditorGUILayout.HelpBox("Add or select a new field to customise talents.", MessageType.Info);
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        #endregion

        #region Talent Groups

        private static TalentGroup selectedTalentGroup = null;
        public static void TalentGroups(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            var list = Rm_RPGHandler.Instance.Repositories.Talents.AllTalentGroups;
            GUI.Box(leftArea, "", "backgroundBox");
            GUI.Box(mainArea, "", "backgroundBoxMain");

            GUILayout.BeginArea(PadRect(leftArea, 0, 0));
            RPGMakerGUI.ListArea(list, ref selectedTalentGroup, Rm_ListAreaType.TalentGroup, false, true);
            GUILayout.EndArea();


            GUILayout.BeginArea(mainArea);
            RPGMakerGUI.Title("Talent Groups");
            if (selectedTalentGroup != null)
            {
                selectedTalentGroup.Name = RPGMakerGUI.TextField("Group Name:", selectedTalentGroup.Name);
                selectedTalentGroup.NumberAllowedActive = RPGMakerGUI.IntField("Number Allowed Active:", selectedTalentGroup.NumberAllowedActive);
            }
            GUILayout.EndArea();
        }

        #endregion

        #region Status Effects

        private static StatusEffect selectedStatusEffect = null;
        private static Vector2 statusEffectScrollPos = Vector2.zero;
        private static bool showStatusDetails = true;
        private static bool showStatusEffectDetails = true;

        public static void StatusEffects(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            var list = Rm_RPGHandler.Instance.Repositories.StatusEffects.AllStatusEffects;
            GUI.Box(leftArea, "", "backgroundBox");
            GUI.Box(mainArea, "", "backgroundBoxMain");

            GUILayout.BeginArea(PadRect(leftArea, 0, 0));
            RPGMakerGUI.ListArea(list, ref selectedStatusEffect, Rm_ListAreaType.StatusEffect, false, true);
            GUILayout.EndArea();


            #region "Update skill-Dots skill meta ID"
            foreach (var skill in Rm_RPGHandler.Instance.Repositories.StatusEffects.AllStatusEffects.Where(s => s.HasSkillMeta))
            {
                if (skill.CausesDOT)
                {
                    skill.DamageOverTime.SkillMetaID = skill.SkillMetaID;
                    skill.DamageOverTime.DamagePerTick.SkillMetaID = skill.SkillMetaID;
                }
            }
            #endregion

            GUILayout.BeginArea(mainArea);
            statusEffectScrollPos = GUILayout.BeginScrollView(statusEffectScrollPos);
            RPGMakerGUI.Title("Status Effects");
            if (selectedStatusEffect != null)
            {
                if (RPGMakerGUI.Foldout(ref showTalentDetails, "Main Details"))
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical(GUILayout.MaxWidth(85));
                    selectedStatusEffect.Image.Image = RPGMakerGUI.ImageSelector("", selectedStatusEffect.Image.Image,
                                                                           ref selectedStatusEffect.Image.ImagePath);

                    GUILayout.EndVertical();
                    GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

                    selectedStatusEffect.Name = RPGMakerGUI.TextField("Name: ", selectedStatusEffect.Name);
                    if (selectedStatusEffect.ApplyToNearbyAllies = RPGMakerGUI.Toggle("Apply To Nearby Allies?", selectedStatusEffect.ApplyToNearbyAllies))
                    {
                        selectedStatusEffect.WithinRadius = RPGMakerGUI.FloatField("- Within Radius", selectedStatusEffect.WithinRadius);
                    }

                    if (RPGMakerGUI.Toggle("Has Skill Meta?", ref selectedStatusEffect.HasSkillMeta))
                    {
                        RPGMakerGUI.PopupID<SkillMetaDefinition>("Skill Meta:", ref selectedStatusEffect.SkillMetaID);
                    }

                    if(RPGMakerGUI.Toggle("Causes Stun?", ref selectedStatusEffect.CauseStun))
                    {
                        RPGMakerGUI.Toggle("Freezes Animation?",1, ref selectedStatusEffect.CauseAnimationFreeze);
                    }
                    selectedStatusEffect.CauseRetreat = RPGMakerGUI.Toggle("Causes Retreat/Fear?", selectedStatusEffect.CauseRetreat);
                    selectedStatusEffect.CauseSilence = RPGMakerGUI.Toggle("Causes Silence?", selectedStatusEffect.CauseSilence);
                    if (RPGMakerGUI.Toggle("Causes DoT?", ref selectedStatusEffect.CausesDOT))
                    {
                        var effectDot = selectedStatusEffect.DamageOverTime;

                        effectDot.DoTName = RPGMakerGUI.TextField("- DoT Name:",
                                                           effectDot.DoTName);
                        effectDot.TimeBetweenTick = RPGMakerGUI.FloatField("- Time Between Tick:",
                                                                                            effectDot.
                                                                                                TimeBetweenTick);
                        DotDamage(effectDot);
                    }

                    GUILayout.Space(5);
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                    RPGMakerGUI.EndFoldout();

                }

                if (RPGMakerGUI.Foldout(ref showStatusEffectDetails, "Effect Details"))
                {

                    var currentEffect = selectedStatusEffect.Effect; ;

                    if (currentEffect.HasDuration = RPGMakerGUI.Toggle("Has Duration?", currentEffect.HasDuration))
                    {
                        currentEffect.Duration = RPGMakerGUI.FloatField("- Duration:", currentEffect.Duration);
                    }
                    GUILayout.BeginHorizontal();
                    gameObject = RPGMakerGUI.PrefabSelector("Active Prefab:", gameObject, ref currentEffect.ActivePrefab);
                    gameObject = RPGMakerGUI.PrefabGeneratorButton(Rmh_PrefabType.Effect_Active, gameObject, ref currentEffect.ActivePrefab);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    gameObject = RPGMakerGUI.PrefabSelector("On Activate Prefab:", gameObject, ref currentEffect.OnActivatePrefab);
                    gameObject = RPGMakerGUI.PrefabGeneratorButton(Rmh_PrefabType.Effect_Activated, gameObject, ref currentEffect.OnActivatePrefab);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    gameObject = RPGMakerGUI.PrefabSelector("On Expired Prefab:", gameObject, ref currentEffect.OnExpiredPrefab);
                    gameObject = RPGMakerGUI.PrefabGeneratorButton(Rmh_PrefabType.Effect_Expired, gameObject, ref currentEffect.OnExpiredPrefab);
                    GUILayout.EndHorizontal();


                    if (currentEffect.CanBeCancelled = RPGMakerGUI.Toggle("Cancellable by Status Effect?", currentEffect.CanBeCancelled))
                    {
                        RPGMakerGUI.PopupID<StatusEffect>("- Status Effect", ref currentEffect.CancellingStatusEffectID);
                    }

                    PassiveEffectDetails(currentEffect);

                    RPGMakerGUI.EndFoldout();
                }

            }
            else
            {
                EditorGUILayout.HelpBox("Add or select a new field to customise status effects.", MessageType.Info);
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        #endregion

        public static void PassiveEffectDetails(PassiveEffect currentEffect)
        {

            #region Passive Effect Editor

            RPGMakerGUI.FoldoutList(ref showAttrBuffs, "Attribute Buffs", currentEffect.AttributeBuffs, Rm_RPGHandler.Instance.ASVT.AttributesDefinitions, "+AttributeBuff",
                    "Attribute Buff", "Click +AttributeBuff to add an attriute buff bonus", "AttributeID", "Name", "ID", "Name");
            RPGMakerGUI.FoldoutList(ref showVitBuffs, "Vital Buffs", currentEffect.VitalBuffs, Rm_RPGHandler.Instance.ASVT.VitalDefinitions, "+VitalBuff",
                    "Vital Buff", "Click +VitalBuff to add a vital buff bonus", "VitalID", "Name", "ID", "Name");
            RPGMakerGUI.FoldoutList(ref showStatBuffs, "Statistic Buffs", currentEffect.StatisticBuffs, Rm_RPGHandler.Instance.ASVT.StatisticDefinitions, "+StatisticBuff",
                    "Statistic Buff", "Click +Statistic to add a statistic buff bonus", "StatisticID", "Name", "ID", "Name");

            RPGMakerGUI.FoldoutList(ref showVitalRegens, "Vital Regen Bonuses", currentEffect.VitalRegenBonuses, Rm_RPGHandler.Instance.ASVT.VitalDefinitions, "+VitalRegenBonus",
                "Vital", "Click +VitalRegenBonus to add a regen bonus", "VitalID", "Name", "ID", "Name");
            RPGMakerGUI.FoldoutList(ref showCustomVarSetters, "Custom Var Setters", currentEffect.CustomVariableSetters, Rm_RPGHandler.Instance.DefinedVariables.Vars, "+VariableSetter",
                "Custom Variable", "Click +VariableSetter to add a varaible setter", "VariableID", "Name", "ID", "Name");
            RPGMakerGUI.FoldoutList(ref showCustomVarSettersOnDisable, "Custom Var Setters On Disable", currentEffect.CustomVariableSettersOnDisable, Rm_RPGHandler.Instance.DefinedVariables.Vars, "+VariableSetter",
                "Custom Variable", "Click +VariableSetter to add a varaible setter on effect disable", "VariableID", "Name", "ID", "Name");
            RPGMakerGUI.FoldoutList(ref showStatusReduction, "Status Reductions", currentEffect.StatusDurationReduction, Rm_RPGHandler.Instance.Repositories.StatusEffects.AllStatusEffects, "+StatusReduction",
                "Status Effect", "Click +StatusReduction to add a status reduction effect", "StatusEffectID", "Name", "ID", "Name");

            if (currentEffect.RemoveStatusEffect = RPGMakerGUI.Toggle("Remove Effect?", currentEffect.RemoveStatusEffect))
            {
                RPGMakerGUI.PopupID<StatusEffect>("- Status Effect", ref currentEffect.RemoveStatusEffectID, "ID", "Name");
            }
            if (currentEffect.AddSkillImmunity = RPGMakerGUI.Toggle("Add Immunity?", currentEffect.AddSkillImmunity))
            {
                RPGMakerGUI.PopupID<SkillMetaDefinition>("- Skill Meta", ref currentEffect.SkillImmunityID, "ID", "Name");
            }
            if (currentEffect.AddSkillSusceptibility = RPGMakerGUI.Toggle("Add Skill Susceptibility?", currentEffect.AddSkillSusceptibility))
            {
                RPGMakerGUI.PopupID<SkillMetaDefinition>("- Skill Meta", ref currentEffect.SkillSusceptibilityID, "ID", "Name");
                currentEffect.SkillSusceptibilityAmount = RPGMakerGUI.FloatField("- Amount:", currentEffect.SkillSusceptibilityAmount);
            }
            if (currentEffect.RunsEvent = RPGMakerGUI.Toggle("Run Event?", currentEffect.RunsEvent))
            {
                RPGMakerGUI.PopupID<NodeChain>("- Event", ref currentEffect.RunEventID, "ID", "Name");
            }
            #region ProcEffect
            currentEffect.HasProcEffect = RPGMakerGUI.Toggle("Has Proc?", currentEffect.HasProcEffect);
            if (currentEffect.HasProcEffect)
            {
                var procEffect = currentEffect.ProcEffect;

                procEffect.ProcCondition =
                    (Rm_ProcCondition)RPGMakerGUI.EnumPopup("Proc Condition", procEffect.ProcCondition);
                if (procEffect.ProcCondition == Rm_ProcCondition.Every_N_Hits)
                {
                    procEffect.Parameter = RPGMakerGUI.FloatField("N:", procEffect.Parameter);
                    procEffect.Parameter = (int)procEffect.Parameter;
                }
                if (procEffect.ProcCondition == Rm_ProcCondition.Chance_On_Hit ||
                    procEffect.ProcCondition == Rm_ProcCondition.Chance_On_Critical_Hit)
                {
                    procEffect.Parameter = RPGMakerGUI.FloatField("% Chance:", procEffect.Parameter);

                }
                procEffect.ProcEffectType =
                    (Rm_ProcEffectType)RPGMakerGUI.EnumPopup("Effect:", procEffect.ProcEffectType);

                if (procEffect.ProcEffectType == Rm_ProcEffectType.StatusEffect ||
                    procEffect.ProcEffectType == Rm_ProcEffectType.StatusEffectOnSelf)
                {

                    RPGMakerGUI.PopupID<StatusEffect>("Status Effect:", ref procEffect.EffectParameterString);

                }

                if (procEffect.ProcEffectType == Rm_ProcEffectType.CastSkill ||
                    procEffect.ProcEffectType == Rm_ProcEffectType.CastSkillOnSelf)
                {
                    RPGMakerGUI.PopupID<Skill>("Skill name:", ref procEffect.EffectParameterString);
                }

                if (procEffect.ProcEffectType == Rm_ProcEffectType.KnockBack ||
                    procEffect.ProcEffectType == Rm_ProcEffectType.KnockUp)
                {
                    procEffect.EffectParameter = RPGMakerGUI.FloatField("Distance:",
                                                                        procEffect.EffectParameter);
                }

                if (procEffect.ProcEffectType == Rm_ProcEffectType.PullTowards)
                {
                    //if : targetlock enabled || is ability and stayinplace || restoration and stayinplace?
                    procEffect.PullType = Rm_PullTowardsType.CasterOrSkill;

                    if (!RPGMakerGUI.Toggle("- Pull All The Way?", ref procEffect.PullAllTheWay))
                    {
                        procEffect.EffectParameter = RPGMakerGUI.FloatField("- Pull Distance:",
                                                                            procEffect.EffectParameter);
                    }
                }
            }
            #endregion

            #endregion

        }
        public static void DotDamage(DamageOverTime effectDot)
        {

            #region DotDamage

            #region CheckForUpdates

            var dotDmgList = effectDot.DamagePerTick.ElementalDamages;
            foreach (var d in Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions)
            {
                var tier = dotDmgList.FirstOrDefault(t => t.ElementID == d.ID);
                if (tier == null)
                {
                    var tierToAdd = new ElementalDamage() { ElementID = d.ID };
                    dotDmgList.Add(tierToAdd);
                }
            }

            for (int index = 0; index < dotDmgList.Count; index++)
            {
                var v = dotDmgList[index];
                var stillExists =
                    Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions.FirstOrDefault(
                        t => t.ID == v.ElementID);

                if (stillExists == null)
                {
                    dotDmgList.Remove(v);
                    index--;
                }
            }

            #endregion

            GUILayout.BeginHorizontal();
            RPGMakerGUI.Label("- Physical Damage Per Tick:");
            if (Rm_RPGHandler.Instance.Items.DamageHasVariance)
            {
                effectDot.DamagePerTick.MinDamage = RPGMakerGUI.IntField("", effectDot.DamagePerTick.MinDamage);
                GUILayout.Label(" - ");
            }
            effectDot.DamagePerTick.MaxDamage = RPGMakerGUI.IntField("", effectDot.DamagePerTick.MaxDamage);
            GUILayout.EndHorizontal();

            foreach (var eleDmg in dotDmgList)
            {
                var nameOfEleDmg =
                    Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions.First(
                        e => e.ID == eleDmg.ElementID).Name;

                GUILayout.BeginHorizontal();
                RPGMakerGUI.Label("- " + nameOfEleDmg + " Damage Per Tick:");
                if (Rm_RPGHandler.Instance.Items.DamageHasVariance)
                {
                    eleDmg.MinDamage = RPGMakerGUI.IntField("", eleDmg.MinDamage);
                    GUILayout.Label(" - ");
                }
                eleDmg.MaxDamage = RPGMakerGUI.IntField("", eleDmg.MaxDamage);
                GUILayout.EndHorizontal();
            }

            #endregion

        }

        public static Rect PadRect(Rect rect, int left, int top)
        {
            return new Rect(rect.x + left, rect.y + top, rect.width - (left * 2), rect.height - (top * 2));
        }
    }
}
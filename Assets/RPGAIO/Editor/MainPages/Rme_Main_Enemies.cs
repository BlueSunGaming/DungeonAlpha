using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;
using UnityEditor;
using UnityEngine;
using Attribute = LogicSpawn.RPGMaker.Core.Attribute;

namespace LogicSpawn.RPGMaker.Editor
{
    public static class Rme_Main_Enemies
    {
        public static Rmh_Enemy Enemy
        {
            get { return Rm_RPGHandler.Instance.Enemy; }
        }

        private static bool showMonsterTypes = true;
        private static Vector2 optionsScrollPos = Vector2.zero;
        public static void Options(Rect fullArea, Rect leftArea, Rect mainArea)
         {
             GUI.Box(fullArea, "", "backgroundBox");

             GUILayout.BeginArea(fullArea);
             optionsScrollPos = GUILayout.BeginScrollView(optionsScrollPos);
             RPGMakerGUI.Title("Enemy - Options");

             var result = RPGMakerGUI.FoldoutToolBar(ref showMonsterTypes, "Monster Types",
                                                        new[] { "+Monster Type" });
             if (showMonsterTypes)
             {
                 for (int index = 0; index < Enemy.MonsterTypes.Count; index++)
                 {
                     var def = Enemy.MonsterTypes[index];
                     GUILayout.BeginHorizontal();
                     def.Name = RPGMakerGUI.TextField("", def.Name);
                     if (RPGMakerGUI.DeleteButton(25))
                     {
                         Enemy.MonsterTypes.Remove(def);
                         index--;
                     }
                     GUILayout.EndHorizontal();
                 }

                 if (result == 0)
                 {
                     Enemy.MonsterTypes.Add(new MonsterTypeDefinition());
                 }

                 if (Enemy.MonsterTypes.Count == 0)
                 {
                     EditorGUILayout.HelpBox(
                         "You can define monster types to use elsewhere such as Boss, Miniboss, etc.",
                         MessageType.Info);
                 }
             }
             if (showMonsterTypes) RPGMakerGUI.EndFoldout();
             GUILayout.EndScrollView();
             GUILayout.EndArea();
         }

        private static bool mainInfoFoldout = true;
        private static bool animFoldout = true;
        private static bool wepFoldout = true;
        private static bool showMecanimAnim = true;
        private static List<AnimationDefinition> KeyAnimations
        {
            get
            {
                return new List<AnimationDefinition>()
                           {
                               selectedCharInfo.LegacyAnimations.WalkAnim,
                               selectedCharInfo.LegacyAnimations.RunAnim,
                               selectedCharInfo.LegacyAnimations.JumpAnim,
                               selectedCharInfo.LegacyAnimations.IdleAnim,
                               selectedCharInfo.LegacyAnimations.CombatIdleAnim,
                               selectedCharInfo.LegacyAnimations.TakeHitAnim,
                               selectedCharInfo.LegacyAnimations.FallAnim,
                               selectedCharInfo.LegacyAnimations.DeathAnim,
                               selectedCharInfo.LegacyAnimations.KnockBackAnim,
                               selectedCharInfo.LegacyAnimations.KnockUpAnim
                           };
            }
        }
        private static int[] selectedAnim = new int[999];

        private static bool baseAttrFoldout = true;
        private static bool baseVitFoldout = true;
        private static bool baseStatFoldout = true;
        private static bool showBonusDamage = true;
        private static bool startingSkillsFoldout = true;
        private static bool skillImmunFoldout = true;
        private static bool skillSusFoldout = true;
        private static bool gurLootFoldout = true;
        private static bool lootTableFoldout = true;
        private static int[] selectedStartSkill = new int[999];
        private static int[] selectedSkillImmunity = new int[999];
        private static int[] selectedSkillSus = new int[999];
        private static int[] selectedGurLoot = new int[999];
        private static int[] selectedLootTable = new int[999];
        private static int selectedMonsterType;
        private static int selectedReputation;
        private static GameObject rangedProjectile;
        private static CombatCharacter selectedCharInfo = null; 
        private static GameObject gameObject = null;
        public static void Enemies(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            var list = Rm_RPGHandler.Instance.Repositories.Enemies.AllEnemies;
            GUI.Box(leftArea, "","backgroundBox");
            GUI.Box(mainArea, "","backgroundBoxMain");

            GUILayout.BeginArea(PadRect(leftArea, 0, 0));
            RPGMakerGUI.ListArea(list, ref selectedCharInfo, Rm_ListAreaType.Enemies, false, true);
            GUILayout.EndArea();


            GUILayout.BeginArea(mainArea);
            RPGMakerGUI.Title("Enemies");
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
                    selectedCharInfo.ID = RPGMakerGUI.TextField("ID: ", selectedCharInfo.ID);
                    selectedCharInfo.Name = RPGMakerGUI.TextField("Name: ", selectedCharInfo.Name);
                    GUILayout.BeginHorizontal();
                    gameObject = RPGMakerGUI.PrefabSelector("Enemy Prefab:", gameObject, ref selectedCharInfo.CharPrefabPath);
                    gameObject = RPGMakerGUI.PrefabGeneratorButton(Rmh_PrefabType.Enemy, gameObject, ref selectedCharInfo.CharPrefabPath, null, selectedCharInfo.ID);
                    GUILayout.EndHorizontal(); 

                    Rme_Combatants.CombatantDetails(selectedCharInfo);
                    GUILayout.Space(5);

                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();

                }
                if (mainInfoFoldout) RPGMakerGUI.EndFoldout();

                Rme_Combatants.Projectile(selectedCharInfo);
                Rme_Combatants.Animations(selectedCharInfo);
                Rme_Combatants.CombatStats(selectedCharInfo);
                Rme_Combatants.Loot(selectedCharInfo);

                RPGMakerGUI.EndScrollView();
            }
            else
            {
                EditorGUILayout.HelpBox("Add or select a new field to customise Enemy Characters.", MessageType.Info);
            }
            GUILayout.EndArea();
        }
        
        public static Rect PadRect(Rect rect, int left, int top)
        {
            return new Rect(rect.x + left, rect.y + top, rect.width - (left * 2), rect.height - (top * 2));
        }
    }
}
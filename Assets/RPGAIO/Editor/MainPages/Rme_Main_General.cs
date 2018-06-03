using LogicSpawn.RPGMaker.Core;
using UnityEditor;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Editor
{
    public static class Rme_Main_General
    {
        public static void ProgressionGain(bool isHarvestable, ProgressionGain progressionGain, bool isCombatant = false)
        {

            GUILayout.BeginHorizontal();
            RPGMakerGUI.Label(isHarvestable ? "Gains on Harvest:" : "Gains On Kill:");
            progressionGain.GainExp = EditorGUILayout.ToggleLeft(" Exp?", progressionGain.GainExp,GUILayout.Width(55));
            GUILayout.Space(15);
            progressionGain.GainSkillPoints = EditorGUILayout.ToggleLeft(" Skill Points?", progressionGain.GainSkillPoints, GUILayout.Width(90));
            GUILayout.Space(15);
            progressionGain.GainTraitExp = EditorGUILayout.ToggleLeft(" Trait Exp?", progressionGain.GainTraitExp, GUILayout.Width(90));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (progressionGain.GainExp)
            {
                if (isCombatant && RPGMakerGUI.Toggle("Gain Exp with Exp Definition?", ref progressionGain.GainExpWithDefinition))
                {
                    RPGMakerGUI.PopupID<ExpDefinition>("Exp Definition:", ref progressionGain.GainExpWithDefinitionID, "ID", "Name", "ExpGained");
                }
                else
                {
                    progressionGain.ExpGained = RPGMakerGUI.IntField("- Exp ", progressionGain.ExpGained);                    
                }
            }

            if (progressionGain.GainSkillPoints)
            {
                if (isCombatant && RPGMakerGUI.Toggle("Gain Skill Points with Exp Definition?", ref progressionGain.GainSkillWithDefinition))
                {
                    RPGMakerGUI.PopupID<ExpDefinition>("Exp Definition:", ref progressionGain.GainSkillWithDefinitionID, "ID", "Name", "ExpGained");
                }
                else
                {
                    progressionGain.SkillPointsGained = RPGMakerGUI.IntField("- Skill Points: ", progressionGain.SkillPointsGained);
                }
            }

            if (progressionGain.GainTraitExp)
            {
                if (isCombatant && RPGMakerGUI.Toggle("Gain Trait Exp with Exp Definition?", ref progressionGain.GainTraitWithDefinition))
                {
                    RPGMakerGUI.PopupID<Rm_TraitDefintion>("- Trait:", ref progressionGain.TraitID);
                    RPGMakerGUI.PopupID<ExpDefinition>("Exp Definition:", ref progressionGain.GainTraitWithDefinitionID, "ID", "Name", "ExpGained");
                }
                else
                {
                    RPGMakerGUI.PopupID<Rm_TraitDefintion>("- Trait:", ref progressionGain.TraitID);
                    progressionGain.TraitExpGained = RPGMakerGUI.IntField("- Trait EXP: ", progressionGain.TraitExpGained);
                }
            }
        }

    }
}
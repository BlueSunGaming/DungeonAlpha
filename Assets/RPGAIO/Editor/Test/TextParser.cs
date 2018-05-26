//using System;
//using System.Linq;
//using LogicSpawn.RPGMaker;
//using LogicSpawn.RPGMaker.Core;
//using UnityEditor;
//using UnityEngine;
//
//public class TextParser : EditorWindow
//{
//    public PlayerSave PlayerSave;
//    public GUIStyle Style;
//
//    public string RawText = "";
//    public string FormattedText;
//
//    [MenuItem("Window/TextParser")]
//    static void Init()
//    {
//        var window = EditorWindow.GetWindow(typeof(TextParser));
//        window.minSize = new Vector2(600, 600);
//    }
//
//    public TextParser()
//    {
//        PlayerSave = new PlayerSave("MySave",Rm_RPGHandler.Instance.Player.CharacterDefinitions[0].ID,"Warrior","MyWarrior");
//        PlayerSave.Initialize();
//        PlayerSave.Character.GetAttribute("Strength").SkillValue = 50;
//        PlayerSave.Character.GetAttribute("Strength").EquipValue = 25;
//    }
//
//    void OnGUI()
//    {
//        try
//        {
//            OnGUIX();
//        }
//        catch (Exception e )
//        {
//    
//        }
//    }
//
//    void OnGUIX()
//    {
//        GUI.skin = Resources.Load("RPGMakerAssets/EditorSkinRPGMaker") as GUISkin;
//
//        GUILayout.BeginVertical();
//        GUILayout.Box("Warrior Strength:");
//        GUILayout.Box("Base:" + PlayerSave.Character.GetAttribute("Strength").BaseValue.ToString());
//        GUILayout.Box("Equip:" + PlayerSave.Character.GetAttribute("Strength").EquipValue.ToString());
//        GUILayout.Box("Skill:" + PlayerSave.Character.GetAttribute("Strength").SkillValue.ToString());
//
//        GUILayout.Box("Total:" + PlayerSave.Character.GetAttribute("Strength").TotalValue.ToString());
//
//        GUILayout.BeginHorizontal(GUILayout.Height(30));
//        if (GUILayout.Button("ADD COLOR"))
//        {
//            GUI.FocusControl("");
//            RawText = RawText + "<color=red>YOUR TEXT HERE</color>";
//            return;
//        }
//        GUILayout.EndHorizontal();
//
//        RawText = EditorGUILayout.TextArea(RawText, "NoRichText", GUILayout.Height(300));
//
//        FormattedText = Format(RawText);
//        GUILayout.Box(FormattedText, "RichText", GUILayout.Height(300));
//        GUILayout.EndVertical();
//
//
//    }
//
//    private string Format(string rawText)
//    {
//        var text = rawText;
//
//        for (int i = 0; i < Rm_RPGHandler.Instance.ASVT.VitalDefinitions.Count; i++)
//        {
//            var attr = Rm_RPGHandler.Instance.ASVT.VitalDefinitions[i];
//            text = text.Replace("{Vital_" + attr.Name + "_Current" + "}", PlayerSave.Character.GetVital(attr.Name).CurrentValue.ToString());
//            text = text.Replace("{Vital_" + attr.Name + "_Max" + "}", PlayerSave.Character.GetVital(attr.Name).MaxValue.ToString());
//            text = text.Replace("{Vital_" + attr.Name + "_Base" + "}", PlayerSave.Character.GetVital(attr.Name).BaseValue.ToString());
//            text = text.Replace("{Vital_" + attr.Name + "_Skill" + "}", PlayerSave.Character.GetVital(attr.Name).SkillValue.ToString());
//            text = text.Replace("{Vital_" + attr.Name + "_Equip" + "}", PlayerSave.Character.GetVital(attr.Name).EquipValue.ToString());
//            text = text.Replace("{Vital_" + attr.Name + "_Attr" + "}", PlayerSave.Character.GetVital(attr.Name).AttributeValue.ToString());
//            text = text.Replace("{Vital_" + attr.Name + "_Regen" + "}", PlayerSave.Character.GetVital(attr.Name).RegenPercentValue.ToString());
//        }
//
//        for (int i = 0; i < Rm_RPGHandler.Instance.ASVT.AttributesDefinitions.Count; i++)
//        {
//            var attr = Rm_RPGHandler.Instance.ASVT.AttributesDefinitions[i];
//            text = text.Replace("{Attr_" + attr.Name + "}", PlayerSave.Character.GetAttribute(attr.Name).TotalValue.ToString());
//            text = text.Replace("{Attr_" + attr.Name + "_Base" + "}", PlayerSave.Character.GetAttribute(attr.Name).BaseValue.ToString());
//            text = text.Replace("{Attr_" + attr.Name + "_Skill" + "}", PlayerSave.Character.GetAttribute(attr.Name).SkillValue.ToString());
//            text = text.Replace("{Attr_" + attr.Name + "_Equip" + "}", PlayerSave.Character.GetAttribute(attr.Name).EquipValue.ToString());
//        }
//
//        for (int i = 0; i < Rm_RPGHandler.Instance.ASVT.StatisticDefinitions.Count; i++)
//        {
//            var stat = Rm_RPGHandler.Instance.ASVT.StatisticDefinitions[i];
//            text = text.Replace("{Stat_" + stat.Name + "}", PlayerSave.Character.GetStat(stat.Name).TotalValue.ToString());
//            text = text.Replace("{Stat_" + stat.Name + "_Base" + "}", PlayerSave.Character.GetStat(stat.Name).BaseValue.ToString());
//            text = text.Replace("{Stat_" + stat.Name + "_Skill" + "}", PlayerSave.Character.GetStat(stat.Name).SkillValue.ToString());
//            text = text.Replace("{Stat_" + stat.Name + "_Equip" + "}", PlayerSave.Character.GetStat(stat.Name).EquipValue.ToString());
//            text = text.Replace("{Stat_" + stat.Name + "_Attr" + "}", PlayerSave.Character.GetStat(stat.Name).AttributeValue.ToString());
//        }
//
//        for (int i = 0; i < Rm_RPGHandler.Instance.ASVT.TraitDefinitions.Count; i++)
//        {
//            var trait = Rm_RPGHandler.Instance.ASVT.TraitDefinitions[i];
//            text = text.Replace("{Trait_" + trait.Name + "}", PlayerSave.Character.GetTrait(trait.Name).Level.ToString());
//        }
//
//        var nodes = Rm_RPGHandler.Instance.Nodes.CombatNodeBank.NodeTrees.SelectMany(n => n.Nodes);
////        var resultNodes = nodes.Where(r => r.NodeType == Rm_NodeType.Result_Node).ToList();
////
////        for (int i = 0; i < resultNodes.Count; i++)
////        {
////            var node = resultNodes[i];
////            //todo: replace node value with NodeEvaluator.Evaluate(node);
////            text = text.Replace("{Node_" + node.Identifier + "}","[NODE_VALUE]");
////        }
//
//        return text;
//    }
//}
//

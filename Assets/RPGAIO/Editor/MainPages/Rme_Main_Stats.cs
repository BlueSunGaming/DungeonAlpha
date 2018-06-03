using System.Linq;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Core;
using UnityEditor;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Editor
{
    public static class Rme_Main_Stats
    {
        public static Rmh_ASVT ASVT
        {
            get { return Rm_RPGHandler.Instance.ASVT; }
        }

        #region Options

        private static bool showDamageDefinitions = true;

        private static Vector2 optionsScrollPos;
        public static void Options(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            GUI.Box(fullArea, "", "backgroundBox");

            GUILayout.BeginArea(fullArea);
            optionsScrollPos = GUILayout.BeginScrollView(optionsScrollPos);
            RPGMakerGUI.Title("Stat Options");
            RPGMakerGUI.SubTitle("Exp Options");
            RPGMakerGUI.Toggle("Allow Exp To Overflow", ref ASVT.AllowExpToOverflow);
            RPGMakerGUI.SubTitle("Stat Options");
            ASVT.RegenInterval = RPGMakerGUI.FloatField("RegenInterval:", ASVT.RegenInterval);

            RPGMakerGUI.SubTitle("Titles");
            ASVT.AttributeNameTitle = RPGMakerGUI.TextField("Attribute Title:", ASVT.AttributeNameTitle);
            ASVT.StatisticNameTitle = RPGMakerGUI.TextField("Statistic Title:", ASVT.StatisticNameTitle);
            ASVT.VitalNameTitle = RPGMakerGUI.TextField("Vital Title:", ASVT.VitalNameTitle);
            ASVT.TraitNameTitle = RPGMakerGUI.TextField("Trait Title:", ASVT.TraitNameTitle);
            
            RPGMakerGUI.SubTitle("Movement");
            ASVT.JumpHeight = RPGMakerGUI.FloatField("Player Jump Height:", ASVT.JumpHeight);
            ASVT.BaseMovementSpeed = RPGMakerGUI.FloatField("Base Player Movement Speed:", ASVT.BaseMovementSpeed);
            ASVT.BaseNpcMovementSpeed = RPGMakerGUI.FloatField("Base NPC Movement Speed:", ASVT.BaseNpcMovementSpeed);
            if(RPGMakerGUI.Toggle("Use Stat as Movement Multiplier?", ref ASVT.UseStatForMovementSpeed))
            {
                RPGMakerGUI.PopupID<Rm_StatisticDefintion>("Statistic:", ref ASVT.StatForMovementID,1);
                RPGMakerGUI.Help("Move Speed = BaseMoveSpeed * Stat:", 1);
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        #endregion

        #region Attributes

        private static Rm_AttributeDefintion selectedAttrInfo = null;

        private static string n = "";
        public static void Attributes(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            var list = Rm_RPGHandler.Instance.ASVT.AttributesDefinitions;
            GUI.Box(leftArea, "", "backgroundBox");
            GUI.Box(mainArea, "", "backgroundBoxMain");

            GUILayout.BeginArea(PadRect(leftArea, 2, 2));
            RPGMakerGUI.ListArea(list, ref selectedAttrInfo, Rm_ListAreaType.Attributes, false, true);
            GUILayout.EndArea();


            GUILayout.BeginArea(mainArea);
            RPGMakerGUI.Title("Attributes");
            if (selectedAttrInfo != null)
            {
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                selectedAttrInfo.Name = RPGMakerGUI.TextField("Name: ", selectedAttrInfo.Name);
                selectedAttrInfo.Description = RPGMakerGUI.TextField("Description: ", selectedAttrInfo.Description);
                selectedAttrInfo.DefaultValue = RPGMakerGUI.IntField("Default Value: ", selectedAttrInfo.DefaultValue);
                selectedAttrInfo.Color = (Rm_UnityColors)RPGMakerGUI.EnumPopup("Color: ", selectedAttrInfo.Color);
                if(RPGMakerGUI.Toggle("Has Max Value?", ref selectedAttrInfo.HasMaxValue))
                {
                    selectedAttrInfo.MaxValue = RPGMakerGUI.IntField("Max Value: ", selectedAttrInfo.MaxValue,1);
                }
                selectedAttrInfo.Image = RPGMakerGUI.ImageSelector("Image:", selectedAttrInfo.Image, ref selectedAttrInfo.ImagePath, true);
                GUILayout.EndVertical();


            }
            else
            {
                EditorGUILayout.HelpBox("Add or select a new field to customise attributes.", MessageType.Info);
            }
            GUILayout.EndArea();
        }

        #endregion

        #region Vitals

        private static Rm_VitalDefinition selectedVitalInfo = null;

        public static void Vitals(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            var list = Rm_RPGHandler.Instance.ASVT.VitalDefinitions;
            GUI.Box(leftArea, "", "backgroundBox");
            GUI.Box(mainArea, "", "backgroundBoxMain");

            GUILayout.BeginArea(PadRect(leftArea, 0, 0));
            RPGMakerGUI.ListArea(list, ref selectedVitalInfo, Rm_ListAreaType.Vitals, false, true);
            GUILayout.EndArea();


            GUILayout.BeginArea(mainArea);
            RPGMakerGUI.Title("Vitals");
            if (selectedVitalInfo != null)
            {
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

                selectedVitalInfo.Name = RPGMakerGUI.TextField("Name: ", selectedVitalInfo.Name);
                selectedVitalInfo.Description = RPGMakerGUI.TextField("Description: ", selectedVitalInfo.Description);
                selectedVitalInfo.DefaultValue = RPGMakerGUI.IntField("Default Value: ", selectedVitalInfo.DefaultValue);
                selectedVitalInfo.Color = (Rm_UnityColors)RPGMakerGUI.EnumPopup("Color: ", selectedVitalInfo.Color);

                GUI.enabled = !selectedVitalInfo.IsHealth;
                var oldHealth = selectedVitalInfo.IsHealth;
                selectedVitalInfo.IsHealth = RPGMakerGUI.Toggle("Is Health Vital? ", selectedVitalInfo.IsHealth);
                GUI.enabled = true;

                if (oldHealth != selectedVitalInfo.IsHealth)
                {
                    ASVT.VitalDefinitions.Where(v => v.IsHealth && v.ID != selectedVitalInfo.ID).ToList()
                        .ForEach(vit => vit.IsHealth = false);
                }

                selectedVitalInfo.HasUpperLimit = RPGMakerGUI.Toggle("Has Max Value?",
                                                                         selectedVitalInfo.HasUpperLimit);
                if (selectedVitalInfo.HasUpperLimit)
                {
                    selectedVitalInfo.UpperLimit = RPGMakerGUI.IntField("Max Value?", selectedVitalInfo.UpperLimit);
                }

                if(!selectedVitalInfo.IsHealth)
                {
                    RPGMakerGUI.Toggle("Always Starts At Zero?", ref selectedVitalInfo.AlwaysStartsAtZero);    
                }
                else
                {
                    selectedVitalInfo.AlwaysStartsAtZero = false;
                }

                selectedVitalInfo.BaseRegenPercentValue = RPGMakerGUI.FloatField("Base Regen Percent:", selectedVitalInfo.BaseRegenPercentValue);
                RPGMakerGUI.Toggle("Also Regen While In Combat?", ref selectedVitalInfo.RegenWhileInCombat);

                /*
                         public bool ReduceHealthIfZero;
        public float ReductionIntervalSeconds;
        public bool ReduceByFixedAmount;
        public int ReductionFixedAmount;
        public float ReductionPercentageAmount;*/

                if (!selectedVitalInfo.IsHealth && RPGMakerGUI.Toggle("Reduce Health Vital At Zero?", ref selectedVitalInfo.ReduceHealthIfZero))
                {
                    selectedVitalInfo.ReductionIntervalSeconds = RPGMakerGUI.FloatField("Reduce every X seconds:", selectedVitalInfo.ReductionIntervalSeconds,1);

                    if (RPGMakerGUI.Toggle("Reduce By Fixed Amount?",1, ref selectedVitalInfo.ReduceByFixedAmount))
                    {
                        selectedVitalInfo.ReductionFixedAmount = RPGMakerGUI.IntField("Reduce by X:", selectedVitalInfo.ReductionFixedAmount, 1);
                    }
                    else
                    {
                        GUILayout.BeginHorizontal();
                        selectedVitalInfo.ReductionPercentageAmount = RPGMakerGUI.FloatField("Reduce by X percent of Max HP:", selectedVitalInfo.ReductionPercentageAmount, 1);
                        RPGMakerGUI.Label(selectedVitalInfo.ReductionPercentageAmount.ToString("#0.##%"));
                        GUILayout.EndHorizontal();
                    }
                }

                selectedVitalInfo.Image = RPGMakerGUI.ImageSelector("Image:", selectedVitalInfo.Image, ref selectedVitalInfo.ImagePath, true);

                GUILayout.EndVertical();

            }
            else
            {
                EditorGUILayout.HelpBox("Add or select a new field to customise vitals.", MessageType.Info);
            }
            GUILayout.EndArea();
        }

        #endregion

        #region Statistics

        private static Rm_StatisticDefintion selectedStatInfo = null;

        public static void Statistics(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            var list = Rm_RPGHandler.Instance.ASVT.StatisticDefinitions;
            GUI.Box(leftArea, "", "backgroundBox");
            GUI.Box(mainArea, "", "backgroundBoxMain");

            GUILayout.BeginArea(PadRect(leftArea, 0, 0));
            RPGMakerGUI.ListArea(list, ref selectedStatInfo, Rm_ListAreaType.Statistics, false, true);
            GUILayout.EndArea();


            GUILayout.BeginArea(mainArea);
            RPGMakerGUI.Title("Statistics");
            if (selectedStatInfo != null)
            {
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                selectedStatInfo.Name = RPGMakerGUI.TextField("Name:", selectedStatInfo.Name);
                selectedStatInfo.Description = RPGMakerGUI.TextField("Description: ", selectedStatInfo.Description);
                selectedStatInfo.DefaultValue = RPGMakerGUI.FloatField("Default Value:", selectedStatInfo.DefaultValue);
                selectedStatInfo.Color = (Rm_UnityColors)RPGMakerGUI.EnumPopup("Color: ", selectedStatInfo.Color);
                selectedStatInfo.IsPercentageInUI = RPGMakerGUI.Toggle("Is Percentage in UI?", selectedStatInfo.IsPercentageInUI);
                selectedStatInfo.HasMaxValue = RPGMakerGUI.Toggle("Has Max Value?", selectedStatInfo.HasMaxValue);
                if (selectedStatInfo.HasMaxValue)
                {
                    selectedStatInfo.MaxValue = RPGMakerGUI.FloatField("Max Value: ", selectedStatInfo.MaxValue);
                }
                selectedStatInfo.StatisticType =
                    (StatisticType)RPGMakerGUI.EnumPopup("Statistic Type:", selectedStatInfo.StatisticType);
                selectedStatInfo.Image = RPGMakerGUI.ImageSelector("Image:", selectedStatInfo.Image, ref selectedStatInfo.ImagePath, true);

                GUILayout.EndVertical();

                
            }
            else
            {
                EditorGUILayout.HelpBox("Add or select a new field to customise statistics.", MessageType.Info);
            }
            GUILayout.EndArea();
        }

        #endregion

        #region Traits

        private static Rm_TraitDefintion selectedTraitInfo = null;

        public static void Traits(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            var list = Rm_RPGHandler.Instance.ASVT.TraitDefinitions;
            GUI.Box(leftArea, "", "backgroundBox");
            GUI.Box(mainArea, "", "backgroundBoxMain");

            GUILayout.BeginArea(PadRect(leftArea, 0, 0));
            RPGMakerGUI.ListArea(list, ref selectedTraitInfo, Rm_ListAreaType.Traits, false, false);
            GUILayout.EndArea();


            GUILayout.BeginArea(mainArea);
            RPGMakerGUI.Title("Traits");
            if (selectedTraitInfo != null)
            {
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                selectedTraitInfo.Name = RPGMakerGUI.TextField("Name: ", selectedTraitInfo.Name);
                selectedTraitInfo.Description = RPGMakerGUI.TextArea("Description: ", selectedTraitInfo.Description);
                RPGMakerGUI.PopupID<ExpDefinition>("Exp Definition:", ref selectedTraitInfo.ExpDefinitionID,"ID","Name","Trait");

                selectedTraitInfo.StartingLevel = RPGMakerGUI.IntField("Starting Level: ", selectedTraitInfo.StartingLevel);

                selectedTraitInfo.Color = (Rm_UnityColors)RPGMakerGUI.EnumPopup("Color: ", selectedTraitInfo.Color);
                selectedTraitInfo.Image = RPGMakerGUI.ImageSelector("Image:", selectedTraitInfo.Image, ref selectedTraitInfo.ImagePath,true);

                GUILayout.EndVertical();

                
            }
            else
            {
                EditorGUILayout.HelpBox("Add or select a new field to customise traits.", MessageType.Info);
                EditorGUILayout.HelpBox("What is a trait? A trait is a stat that has a Level with exp to level." +
                                        "\nExample use: Sword Mastery (Exp gained with Combat Node check that current" +
                                        " weapon is of type Sword)", MessageType.Info);
            }
            GUILayout.EndArea();
        }

        #endregion

        public static Rect PadRect(Rect rect, int left, int top)
        {
            return new Rect(rect.x + left, rect.y + top, rect.width - (left * 2), rect.height - (top * 2));
        }

    }
}
using System;
using LogicSpawn.RPGMaker;
using UnityEditor;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Editor
{
    public static class Rme_Main_Custom
    {
        private static Rmh_CustomVariable selectedInfo = null;
        private static int selectedBooleanResult;
         public static void CustomVariables(Rect fullArea, Rect leftArea, Rect mainArea)
         {
             var list = Rm_RPGHandler.Instance.DefinedVariables.Vars;
             GUI.Box(leftArea, "","backgroundBox");
             GUI.Box(mainArea, "","backgroundBoxMain");

             GUILayout.BeginArea(PadRect(leftArea, 0, 0));
             RPGMakerGUI.ListArea(list, ref selectedInfo, Rm_ListAreaType.CustomVaraibles, false, false);
             GUILayout.EndArea();

            GUILayout.BeginArea(mainArea);
            RPGMakerGUI.Title("Defined Variables");
            if (selectedInfo != null)
            {
                selectedInfo.Name = RPGMakerGUI.TextField("Name: ", selectedInfo.Name);
                var prevSelIndex = selectedInfo.VariableType;

                selectedInfo.VariableType = (Rmh_CustomVariableType)RPGMakerGUI.EnumPopup("Type:", selectedInfo.VariableType);

                if (prevSelIndex != selectedInfo.VariableType)
                {
                    GUI.FocusControl("");
                }

                switch (selectedInfo.VariableType)
                {
                    case Rmh_CustomVariableType.Float:
                        selectedInfo.FloatValue = RPGMakerGUI.FloatField("Default Value:", selectedInfo.FloatValue);
                        break;
                    case Rmh_CustomVariableType.Int:
                        selectedInfo.IntValue = RPGMakerGUI.IntField("Default Value:", selectedInfo.IntValue);
                        break;
                    case Rmh_CustomVariableType.String:
                        selectedInfo.StringValue = RPGMakerGUI.TextField("Default Value:", selectedInfo.StringValue);
                        break;
                    case Rmh_CustomVariableType.Bool:
                        selectedBooleanResult = selectedInfo.BoolValue ? 0 : 1;
                        selectedBooleanResult = RPGMakerGUI.Popup("Default Value:", selectedBooleanResult, new[] { "True", "False" });
                        selectedInfo.BoolValue = selectedBooleanResult == 0;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                RPGMakerGUI.Title("Add or select a new field to customise credits.");
            }
            GUILayout.EndArea();
        }

         public static Rect PadRect(Rect rect, int left, int top)
         {
             return new Rect(rect.x + left, rect.y + top, rect.width - (left * 2), rect.height - (top * 2));
         }
    }
}
using UnityEngine;

namespace LogicSpawn.RPGMaker.Editor
{
    public static class Rme_Main_UI
    {
         public static void Options(Rect fullArea, Rect leftArea, Rect mainArea)
         {
             GUI.Box(leftArea,"","backgroundBox");
             GUI.Box(mainArea, "","backgroundBoxMain");

             GUILayout.BeginArea(PadRect(leftArea,0,0));
             RPGMakerGUI.Title("List 1");

             RPGMakerGUI.Title("List 2");
             RPGMakerGUI.Title("List 3");

             GUILayout.EndArea();

             
             GUILayout.BeginArea(mainArea);
                RPGMakerGUI.Title("Game page.");
             RPGMakerGUI.Title("UI - OPTIONS");
             GUILayout.EndArea();
         }

         public static Rect PadRect(Rect rect, int left, int top)
         {
             return new Rect(rect.x + left, rect.y + top, rect.width - (left * 2), rect.height - (top * 2));
         }
    }
}
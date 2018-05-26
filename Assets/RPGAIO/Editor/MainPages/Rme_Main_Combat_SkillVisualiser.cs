using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEditor;

namespace LogicSpawn.RPGMaker.Editor
{
    public static class Rme_Main_Combat_SkillVisualiser
    {
        private static Rect windowRect = new Rect(5,5,50,50);
        private static List<VisualiserWindow> Windows = new List<VisualiserWindow>();
        public static void Main(Rect fullArea, Rect leftArea, Rect mainArea, Rme_Main window)
        {
            GUI.Box(fullArea, "", "backgroundBox");

            GUILayout.BeginArea(fullArea);
            GUILayout.BeginVertical();
            if(GUILayout.Button("Add Skill"))
            {
             Windows.Add(new VisualiserWindow()
                             {
                                 ID = Windows.Count > 0 ? (Windows.Max(i => i.ID) + 1) : 0,
                                 SkillID = "",
                                 Type = VisualiserType.Skill,
                                 rect = new Rect(Random.Range(50,100+1),Random.Range(50,100+1),50,50),
                                 Area = 1
                             });   
            }
            if(GUILayout.Button("Add Horizontal"))
            {
             Windows.Add(new VisualiserWindow()
                             {
                                 ID = Windows.Count > 0 ? (Windows.Max(i => i.ID) + 1) : 0,
                                 SkillID = "",
                                 Type = VisualiserType.Hori,
                                 rect = new Rect(Random.Range(50,100+1),Random.Range(50,100+1),25,10),
                                 Area = 1
                             });  
            }
            if(GUILayout.Button("Add Vertical"))
            {
             Windows.Add(new VisualiserWindow()
                             {
                                 ID = Windows.Count > 0 ? (Windows.Max(i => i.ID) + 1) : 0,
                                 SkillID = "",
                                 Type = VisualiserType.Vert,
                                 rect = new Rect(Random.Range(50,100+1),Random.Range(50,100+1),10,25),
                                 Area = 1
                             });  
            }
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Box("",GUILayout.Width(300),GUILayout.Height(500));
            var toRectA = GUILayoutUtility.GetLastRect();

            GUILayout.FlexibleSpace();


            GUILayout.Box("", GUILayout.Width(300), GUILayout.Height(500));
            var toRectB = GUILayoutUtility.GetLastRect();

            GUILayout.FlexibleSpace();

            GUILayout.Box("", GUILayout.Width(300), GUILayout.Height(500));
            var toRectC = GUILayoutUtility.GetLastRect();

            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            window.BeginWindows();
            
            for (int i = 0; i < Windows.Count; i++)
            {
                var windowData = Windows[i];
                Rect rect;
                switch(windowData.Area)
                {
                    case 0:
                        rect = toRectA;
                        break;
                    case 1:
                        rect = toRectB;
                        break;
                    case 2:
                        rect = toRectC;
                        break;
                    default:
                        rect = toRectA;
                        break;
                }
                windowData.rect = RoundWindow(ClampWindow(GUILayout.Window(windowData.ID, windowData.rect, myWindow, "", "visualiserWindow_" + windowData.Type.ToString()), rect));
			}

            window.EndWindows();
            GUILayout.EndArea();
        }

        private static void myWindow(int id)
        {
            var windowData = Windows.First(w => w.ID == id);
            if(windowData.Type == VisualiserType.Skill)
            {
                GUILayout.Box("[Skill]", "visualiserText");    
            }
            else
            {
                GUILayout.Label("");
            }
            GUI.DragWindow();
        }

        private static Rect RoundWindow(Rect rect)
        {
            rect.x = rect.x.RoundToNearest(5f);
            rect.y = rect.y.RoundToNearest(5f);
            return rect;
        }

        private static Rect ClampWindow(Rect rect, Rect toRect)
        {
            if(Event.current.type == EventType.Repaint)
            {
                rect.x = Mathf.Clamp(rect.x, toRect.xMin, toRect.xMax - rect.width);
                rect.y = Mathf.Clamp(rect.y, toRect.yMin, toRect.yMax - rect.height);

                return rect;
            }
            return rect;
            
        }

        public static Rect PadRect(Rect rect, int left, int top)
        {
            return new Rect(rect.x + left, rect.y + top, rect.width - (left * 2), rect.height - (top * 2));
        }
    }

    public class VisualiserWindow
    {
        public int ID;
        public VisualiserType Type;
        public string SkillID;
        public Rect rect;
        public int Area;

        public VisualiserWindow()
        {
            ID = -1;
            Type = VisualiserType.Skill;
            SkillID = "";
            Area = 1;
        }
    }

    public enum VisualiserType
    {
        Skill,
        Vert,
        Hori
    }
}
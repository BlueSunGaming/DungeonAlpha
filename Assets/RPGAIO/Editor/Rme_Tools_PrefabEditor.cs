//using System;
//using UnityEngine;
//using UnityEditor;
//
//namespace LogicSpawn.RPGMaker.Editor
//{
//    public class Rme_Tools_PrefabEditor : EditorWindow
//    {
//        // Add menu named "My Window" to the Window menu
//        [MenuItem("LogicSpawn RPG Maker/Tools/Prefab Editor",false,4)]
//        private static void Init()
//        {
//            // Get existing open window or if none, make a new one:
//            var window = (Rme_Tools_PrefabEditor)GetWindow(typeof(Rme_Tools_PrefabEditor));
//            window.maxSize = new Vector2(1000, 700);
//            window.title = "Prefab Editor";
//            window.minSize = new Vector2(1000.1F, 700.1F);
//            window.position = new Rect(100, 100, 1000, 700);
//        }
//
//
//        void OnGUI()
//        {
//            try
//            {
//                OnGUIx();
//            }
//            catch (Exception e)
//            {
//                Debug.Log("Editor Error: " + e.Message + "@" + e.Source);
//            }
//        }
//
//        private void OnGUIx()
//        {
//            GUI.skin = null;
//            GUI.skin = Resources.Load("RPGMakerAssets/EditorSkinRPGMaker") as GUISkin;
//        
//        }
//
//        public Rect PadRect(Rect rect, int left, int top)
//        {
//            return new Rect(rect.x + left, rect.y + top, rect.width - (left*2), rect.height - (top*2));
//        }
//
//    
//    }
//}
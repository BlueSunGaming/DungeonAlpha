using System;
using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace LogicSpawn.RPGMaker.Editor
{
    public class Rme_PatchNoteWindow : EditorWindow
    {
        // Add menu named "My Window" to the Window menu
        [MenuItem("Tools/LogicSpawn RPG All In One/Patch Notes", false, 20)]
        private static void Init()
        {
            // Get existing open window or if none, make a new one:
            var window = (Rme_PatchNoteWindow)GetWindow(typeof(Rme_PatchNoteWindow));
            window.titleContent = new GUIContent("PatchNotes");
            window.maxSize = new Vector2(700.1f, 500.1f);
            window.position = new Rect(300, 300, 700, 500); 
        }

        public static void Open()
        {
            Init();
        }

        void OnGUI()
        {
            try
            {
                OnGUIx();
            }
            catch (Exception e)
            {
                Debug.Log("Editor Error: " + e.Message + "@" + e.Source);
            }
        }

        private void OnGUIx()
        {
            GUI.skin = null;
            GUI.skin = Resources.Load("RPGMakerAssets/EditorSkinRPGMaker") as GUISkin;
            GUILayout.BeginVertical();
            GUILayout.Label("Patch Notes:", "mainTitleCenter");
            GUILayout.TextArea(Rme_Main.PatchNotes,GUILayout.Height(430));
            if(GUILayout.Button("Enjoying RPGAIO? Don't forget to leave a 5 star rating by clicking here."))
            {
                Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/53542");
            }
            GUILayout.EndVertical();
        }
    }
}
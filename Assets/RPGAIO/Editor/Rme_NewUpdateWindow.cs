using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace LogicSpawn.RPGMaker.Editor
{
    public class Rme_NewUpdateWindow : EditorWindow
    {
        private static string[] Warnings = new string[0];
        private static string[] Info = new string[0];
        private static Dictionary<string, string> Links = new Dictionary<string, string>();
        private static double _timeToEnable;

        public static void Init()
        {
            // Get existing open window or if none, make a new one:
            var window = CreateInstance<Rme_NewUpdateWindow>();
            window.titleContent = new GUIContent("Important Info");
            window.maxSize = new Vector2(600.1f, 400.1f);
            window.ShowPopup();
            _timeToEnable = EditorApplication.timeSinceStartup + 10;
            window.position = new Rect(Screen.currentResolution.width / 2 - 300, Screen.currentResolution.height / 2 - 200, 600, 400);
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

        public static void AddInfo(params string[] text)
        {
            Info = text;
        }
        public static void AddWarnings(params string[] text)
        {
            Warnings = text;
        }
        public static void AddLinks(Dictionary<string,string> links)
        {
            Links = links;
        }

        void Update()
        {
            Repaint();
        }

        private void OnGUIx()
        {
            if(Links.Count + Warnings.Length + Info.Length < 1)
            {
                Close();
            }

            GUI.skin = null;
            GUI.skin = Resources.Load("RPGMakerAssets/EditorSkinRPGMaker") as GUISkin;
            GUILayout.BeginVertical("backgroundBox");

            RPGMakerGUI.Title("Important Update Info");
            foreach(var text in Links)
            {
                GUILayout.BeginVertical();
                EditorGUILayout.HelpBox(text.Key, MessageType.Error);
                if (GUILayout.Button("Click Here For More Information", "genericButton", GUILayout.Height(20)))
                {
                    Process.Start(text.Value);
                }
                GUILayout.EndVertical();
                GUILayout.Space(5);
            }
            GUILayout.Space(10);
            foreach(var text in Warnings)
            {
                EditorGUILayout.HelpBox(text, MessageType.Warning);
                GUILayout.Space(5);
            }
            foreach(var text in Info)
            {
                EditorGUILayout.HelpBox(text, MessageType.Info);
                GUILayout.Space(5);
            }


            GUILayout.FlexibleSpace();

            if (EditorApplication.timeSinceStartup >= _timeToEnable)
            {
                if (GUILayout.Button("Close", "genericButton", GUILayout.Height(20)))
                {
                    Close();
                }
            }
            else
            {
                GUI.enabled = false;
                GUILayout.Button("Can close in " + (_timeToEnable - EditorApplication.timeSinceStartup).ToString("F2") + " seconds", "genericButton", GUILayout.Height(20));
                GUI.enabled = false;
            }
            GUILayout.EndVertical();
        }
    }
}
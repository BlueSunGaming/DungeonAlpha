using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Editor.New
{
    public class Notify : EditorWindow
    {
        public float TimeToClose;
        public string Message;
        public Texture2D NotifyImage;
        public static int CurrentWindows = 0;   
        public static void ShowPopup(string info, Texture2D image = null)                                       
        {
            if (CurrentWindows < 0) CurrentWindows = 0;
            var window = CreateInstance<Notify>();
            window.minSize = window.maxSize = new Vector2(220,45);
            window.position = new Rect(Screen.currentResolution.width - 270, Screen.currentResolution.height - 105 - (CurrentWindows * 50), 220, 45);
            window.TimeToClose = Time.realtimeSinceStartup + 3;
            window.Message = info;
            window.NotifyImage = image ?? RPGMakerGUI.RPGMakerIcon;
            CurrentWindows += 1;
            window.ShowPopup();
        }

        public static void Warning(string warning)
        {
            //throw new NotImplementedException();
            //ShowPopup(warning, RPGMakerGUI.WarningIcon);
        }

        public static void Info(string info)
        {
            //throw new NotImplementedException();
            //ShowPopup(info, RPGMakerGUI.InfoIcon);
        }

        public static void Error(string error)
        {
            //throw new NotImplementedException();
            //ShowPopup(error, RPGMakerGUI.ErrorIcon);
        }

        public static void Save(string message)
        {
            ShowPopup(message, RPGMakerGUI.LoadingIcon);
        }

        void Update()
        {
            if(Time.realtimeSinceStartup >= TimeToClose )
            {
                CurrentWindows -= 1;
                Close();
            }
        }

        void OnGUI()
        {
            GUI.skin = Resources.Load("RPGMakerAssets/EditorSkinRPGMaker") as GUISkin;
            GUI.Box(new Rect(0, 0, 220, 45), "", "backgroundBox");
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            GUILayout.Box(NotifyImage, "notifyPopupImage", GUILayout.Width(35), GUILayout.Height(35));

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.Label(Message, "notifyPopupText");
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical(); 
            
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();


        }
    }
}
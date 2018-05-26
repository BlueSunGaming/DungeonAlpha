using System;
using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace LogicSpawn.RPGMaker.Editor
{
    public class Rme_ContactWindow : EditorWindow
    {
        private string invoiceCode;
        // Add menu named "My Window" to the Window menu
        private static void Init()
        {
            // Get existing open window or if none, make a new one:
            var window = (Rme_ContactWindow)GetWindow(typeof(Rme_ContactWindow));
            window.maxSize = new Vector2(200, 100);
            window.titleContent = new GUIContent("Contact");
            window.maxSize = new Vector2(200.1f, 100.1f);
            window.position = new Rect(300, 300, 201, 81);
        }

        void OnEnable()
        {
            invoiceCode = PlayerPrefs.GetString("RpgMaker_InvoiceCode", "");
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
            EditorGUILayout.HelpBox("Please enter your invoice code: ", MessageType.Info);
            invoiceCode = EditorGUILayout.TextField(invoiceCode);
            if (GUILayout.Button("Contact LogicSpawn"))
            {
                PlayerPrefs.SetString("RpgMaker_InvoiceCode", invoiceCode);
                SendEmail();
            }
            GUILayout.EndVertical();
        }

        void SendEmail()
        {
            var messageContent = "%0D%0A%0D%0A--------------- %0D%0A Invoice Code: " + invoiceCode +
                                 "%0D%0A%0D%0A LogicSpawn%20RPG%20AIO%20Enquiry";
            var url = "mailto:info@logicspawn.co.uk?subject=LogicSpawn%20RPG%20AIO%20Enquiry&body=" + messageContent.Replace(" ", "%20");
            Process.Start(url);
        }
    }
}
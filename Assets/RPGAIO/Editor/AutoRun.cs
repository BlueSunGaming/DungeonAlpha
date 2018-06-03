using UnityEditor;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Editor
{
    [InitializeOnLoad]
    public class Autorun
    {
        private static bool loadedData = false;
        private static double timeSinceStartup = 0;
        static Autorun()
        {
            EditorApplication.update += RunOnce;
            EditorApplication.update += UpdateData;
            EditorApplication.update += AutoSave;
        }

        private static void UpdateData()
        {
            if(EditorApplication.timeSinceStartup > timeSinceStartup + 1)
            {
                timeSinceStartup = EditorApplication.timeSinceStartup;
                Rm_DataUpdater.ScanAndUpdateData();
            }
        }

        private static void AutoSave()
        {
            Rm_DataUpdater.AutoSave();
        }

        private static void RunOnce()
        {
            EditorApplication.update -= RunOnce;
            if(EditorGameDataSaveLoad.LoadIfNotLoadedFromEditor())
            {
                Debug.Log("[RPGAIO] Loading Game Data");
            }
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        public static void DidReloadScripts()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
            {
                Debug.Log("[RPGAIO] Compiled scripts, reloading game data");
                EditorGameDataSaveLoad.LoadGameDataFromEditor();
                loadedData = true;
            }
        }
    }
}
//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using DungeonArchitect.Utils;

namespace DungeonArchitect.Editors
{
    class GUIState
    {
        public Color color;
        public Color backgroundColor;
    }
    /// <summary>
    /// Utility functions for drawing UI in the Inspector window
    /// </summary>
    public class GUIUtils
    {
        private static GUIState lastState;

        public static void SaveState()
        {
            lastState = new GUIState();
            lastState.color = GUI.color;
            lastState.backgroundColor = GUI.backgroundColor;
        }

        public static void RestoreState()
        {
            GUI.color = lastState.color;
            GUI.backgroundColor = lastState.backgroundColor;
        }
    }
}

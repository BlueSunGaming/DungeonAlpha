using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ReadSceneNames
{
    private static string[] _scenes;
    public static string[] scenes
    {
        get
        {
            _scenes = ReadNames();
            return _scenes;
        }
    }
    private static string[] ReadNames()
    {
        List<string> temp = new List<string>();
        foreach (UnityEditor.EditorBuildSettingsScene S in UnityEditor.EditorBuildSettings.scenes)
        {
            if (S.enabled)
            {
                string name = S.path.Substring(S.path.LastIndexOf('/') + 1);
                name = name.Substring(0, name.Length - 6);
                temp.Add(name);
            }
        }
        return temp.ToArray();
    }
}
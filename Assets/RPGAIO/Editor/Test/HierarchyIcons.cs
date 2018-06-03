using LogicSpawn.RPGMaker.Core;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

//[InitializeOnLoad]
//[ExecuteInEditMode]
class HierarchyIcons
{
    //Todo: additional icons for other important gameobjects, as well as some icons for
    //Alerts: Not a prefab, soundProducer, etc
    //Toggle the icon visibility in RPGMakerHelper Editor
    static Texture2D texture;
    static List<int> markedObjects;

    static HierarchyIcons()
    {
        // Init
        texture = Resources.Load("RPGMakerAssets/enemyIcon") as Texture2D;
        EditorApplication.update += UpdateCB;
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCB;
    }

    static void UpdateCB()
    {
        // Check here, every so often
        GameObject[] go = Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];

        markedObjects = new List<int>();
        foreach (GameObject g in go)
        {
            // Example: mark all lights
            if (g.GetComponent<EnemyCharacterMono>() != null)
                markedObjects.Add(g.GetInstanceID());
        }

    }

    static void HierarchyItemCB(int instanceID, Rect selectionRect)
    {
        if (Application.isPlaying) return;

        // place the icoon to the right of the list:
        Rect r = new Rect(selectionRect);
        r.x = r.x + 120;
        r.y = r.y + 2;
        r.width = 12;
        r.height = 12;

        if (markedObjects.Contains(instanceID))
        {
            // Draw the texture if it's a light (e.g.)
            GUI.DrawTexture(r, texture);
        }
    }

}
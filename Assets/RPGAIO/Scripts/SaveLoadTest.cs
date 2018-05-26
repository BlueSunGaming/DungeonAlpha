using System;
using System.IO;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

public class SaveLoadTest : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
	}

    private void Load()
    {

        //example
        var lights = FindObjectsOfType(typeof (Light)) as Light[];
        foreach(var light in lights)
        {
            light.shadows = Rm_GameConfig.Instance.Graphics.ShadowType;
        }
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(200,200,400,300));
        GUILayout.BeginVertical();
        if (GUILayout.Button("Save", GUILayout.Height(20)))
        {
            PlayerSaveLoadManager.Instance.SaveGame();
        }


        if (GUILayout.Button("Load", GUILayout.Height(20)))
        {
            PlayerSaveLoadManager.Instance.LoadLastSaved();

        }

        if (GUILayout.Button("+100 Strength", GUILayout.Height(20)))
        {
            GetObject.PlayerMono.Player.GetAttributeByID("Strength").BaseValue += 100;
        }

        if(GameMaster.GameLoaded)
            GUILayout.Box("Player Strength:" +
                GetObject.PlayerMono.Player.GetAttributeByID("Strength").TotalValue.ToString(), GUILayout.Height(20));

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    // Update is called once per frame
	void Update () {
	
	}
}

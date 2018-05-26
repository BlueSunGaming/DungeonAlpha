using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using System.Collections;

public class LoadGameData : MonoBehaviour
{
    public static LoadGameData Instance = null;
	// Use this for initialization
	void Awake ()
	{
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

	    Instance = this;
	    DontDestroyOnLoad(this);
        GameDataSaveLoadManager.Instance.LoadGameData();
        //GameSettingsSaveLoadManager.Instance.LoadSettings();
        Debug.Log("[RPGAIO] Loaded game data and settings"); 
	}
	
	// Update is called once per frame
	void Update () {

	}
}

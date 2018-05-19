﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Yarn.Unity;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null; //Static instance of GameManager which allows it to be accessed by any other script.

    public DialogueRunner dr = null;
    private DungeonGenerator dungeonGenteratorScript; //Store a reference to our DungeonGenerator which will set up the level.

    private GameObject mUITriggeringGO = null;

    private List<Item> playerItems { get; set; }
    private List<Item> AllItems { get; set; }
    private int level = 5;   //Current level number, expressed in game as "Day 1".

    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
        {
            //if not, set instance to this
            instance = this;
        }
        //If instance already exists and it's not this:
        else if (instance != this)
        {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
        }

        dr = GameObject.FindObjectOfType<DialogueRunner>();
        if (dr != null)
        {
            Debug.Log("Dialogue manager was found");
        }
        else
        {
            Debug.Log("Dialogue manager was not found");
        }
        
        BuildItemDatabase();

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        //Get a component reference to the attached DungeonGenerator script
        dungeonGenteratorScript = GetComponent<DungeonGenerator>();

        //Call the InitGame function to initialize the first level 
        InitGame();
    }

    //Initializes the game for each level.
    void InitGame()
    {
        //if (level != 0)
        //{
        //    //Call the SetupScene function of the DungeonGenerator script, pass it current level number.
        //    dungeonGenteratorScript.SetupScene(level);
        //}
    }

    private void BuildItemDatabase()
    {
        String jsonRes = Resources.Load<TextAsset>("JSON/Items").ToString();
        // Find if res/item.json exists
        if (jsonRes != "")
        {
            // if res/items.json exists then load it
            AllItems = JsonConvert.DeserializeObject<List<Item>>(Resources.Load<TextAsset>("JSON/Items").ToString());
            Debug.Log("Count of items is " + AllItems.Count);
            //foreach(Item i in AllItems)
            //{
            //    i.name = i.nItemID.ToString();
            //}
        }
    }

    public void SetUITriggeringGO(GameObject newUItriggerGO)
    {
        mUITriggeringGO = newUItriggerGO;
    }

    public GameObject GetUITriggeringGO()
    {
        return mUITriggeringGO;
    }

    // public method to check the validity of an item
    // return value: Item if it exists or null
    public Item IsValidItem(int _nItemId)
    {
        Item ret = null;

        bool t = AllItems.Exists(i => i.nItemID == _nItemId);
        Item item = AllItems.Find(x => x.nItemID == _nItemId);
        if (item != null)
        {
            ret = item;
        }

        return ret;
    }

    public List<Item> GetItems()
    {
        return AllItems;
    }

    public List<Item> GetItemsForPlayerInventory()
    {
        // perform filtering
        return playerItems;
    }

    //Update is called every frame.
    void Update()
    {

    }
}

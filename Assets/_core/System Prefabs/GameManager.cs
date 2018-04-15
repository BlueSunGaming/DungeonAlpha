using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null; //Static instance of GameManager which allows it to be accessed by any other script.

    private DungeonGenerator dungeonGenteratorScript; //Store a reference to our DungeonGenerator which will set up the level.

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
        if (level != 0)
        {
            //Call the SetupScene function of the DungeonGenerator script, pass it current level number.
            dungeonGenteratorScript.SetupScene(level);
        }
    }

    private void BuildItemDatabase()
    {
        String jsonRes = Resources.Load<TextAsset>("JSON/Items").ToString();
        // Find if res/item.json exists
        if (jsonRes != "")
        {
            // if res/items.json exists then load it
            AllItems = JsonConvert.DeserializeObject<List<Item>>(Resources.Load<TextAsset>("JSON/Items").ToString());
        }
    }

    public List<Item> GetItems()
    {
        return AllItems;
    }

    public List<Item> GetItemsForPlayInventory()
    {
        // perform filtering
        return AllItems;
    }

    //Update is called every frame.
    void Update()
    {

    }
}

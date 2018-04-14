using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class Item : MonoBehaviour {

    // Setup of External references
    public int nItemID { get; set; }
    public string sName { get; set; }
    public string sDescription { get; set; }

    //public static Item CreateFromJSON(string jsonString)
    //{
    //    return JsonUtility.FromJson<Item>(jsonString);
    //}

    [JsonConstructor]
    public Item(int itemID, string name, string desc)
    {
        this.nItemID = itemID;
        this.sName = name;
        this.sDescription = desc;
    }
    //public enum Type
    //{
    //}
}

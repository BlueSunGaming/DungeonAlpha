using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using RPG.CameraUI;

public class Item {
    // Setup of External references
    public int nItemID { get; set; }
    public string sItemIcon { get; set; }
    public string sName { get; set; }
    public string sDescription { get; set; }
    
    //public static Item CreateFromJSON(string jsonString)
    //{
    //    return JsonUtility.FromJson<Item>(jsonString);
    //}

    // Use this for initialization
    //void Start()
    //{
    //    RegisterForMouseClick();
    //}

    [JsonConstructor]
    public Item(int itemID, string name, string desc, string sItemIcon)
    {
        this.nItemID = itemID;
        this.sName = name;
        this.sDescription = desc;
        this.sItemIcon = sItemIcon;
    }

    //void OnMouseClick(RaycastHit raycastHit, int layerHit)
    //{
    //    if (layerHit == RPG.CameraUI.CursorAffordance.nItemLayer)
    //    {
    //        var item = raycastHit.collider.gameObject;

    //        if (IsTargetInRange(item))
    //        {
    //            // Open Inventory Addition/Subtraction UI with the unique ID of the Item
                
    //            Debug.Log("The item was hit.");
    //            // Remove the item from the scene now??
    //            GameObject.Destroy(item);
    //        }
    //    }
    //}

    //private bool IsTargetInRange(GameObject target)
    //{
    //    float distanceToTarget = (target.transform.position - transform.position).magnitude;

    //    return distanceToTarget <= 2.0f;
    //}

    //private void RegisterForMouseClick()
    //{
    //    cameraRaycaster = FindObjectOfType<CameraRaycaster>();

    //    cameraRaycaster.notifyMouseClickObservers += OnMouseClick;
    //}
}

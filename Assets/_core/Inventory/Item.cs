using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using DungeonRPG.CameraUI;

namespace DungeonRPG
{
    public class Item
    {
        public enum ItemTypes { Consumable, Weapon, Quest }

        // Setup of External references
        public int nItemID { get; set; }
        public string sItemIcon { get; set; }
        public string sName { get; set; }
        public string sDescription { get; set; }
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ItemTypes ItemType { get; set; }
        public string ActionName { get; set; }

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
        public Item(int itemID, string itemIcon, string name, string desc)
        {
            this.nItemID = itemID;
            this.sItemIcon = itemIcon;
            this.sName = name;
            this.sDescription = desc;
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

}
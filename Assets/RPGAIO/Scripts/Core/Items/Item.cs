using System;
using LogicSpawn.RPGMaker.API;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class Item
    {
        public string ID ;
        public string InventoryRefID ;
        public string Name ;
        public string Description ;
        public string RarityID ;

        public float Weight ;

        public int SellValue ;
        public int BuyValue ;


        public ItemType ItemType ;

        public string ImagePath;
        [JsonIgnore]
        public Texture2D _image ;
        [JsonIgnore]
        public Texture2D Image
        {
            get { return _image ?? (_image = Resources.Load(ImagePath) as Texture2D); }
            set { _image = value; }
        }

        public bool CanBeDropped ;


        public bool RunEventOnUse ;
        public string EventTreeIdToRunOnUse ;
        public bool EventHasRun ;

        public bool RunEventOnPickup;
        public string EventTreeIdToRunOnPickup ;
        public bool PickupEventHasRun;

        public bool RunEventOnDrop;
        public string EventTreeIdToRunOnDrop ;
        public bool DropEventHasRun;

        public string CustomGroundPrefabPath;

        public Item()
        {
            ID = Guid.NewGuid().ToString();
            InventoryRefID = null;
            Name = "New Item";
            Description = "";
            SellValue = BuyValue = 1;
            CanBeDropped = true;
            EventTreeIdToRunOnUse = "";
            EventTreeIdToRunOnPickup = "";
            EventTreeIdToRunOnDrop = "";
            EventHasRun = false;
            Weight = 1;
            ImagePath = "";
            ItemType = ItemType.Miscellaneous;
            CustomGroundPrefabPath = "";
        }

        public override string ToString()
        {
            return Name;
        }

        public virtual string GetTooltipDescription()
        {
            return "\n" + RPG.UI.FormatString(Rm_UnityColors.Silver, Description);
        }
    }
}
using System;
using System.Collections.Generic;
using LogicSpawn.RPGMaker.Core;

namespace LogicSpawn.RPGMaker
{
    public class Rmh_Item
    {
        //Item Info
        public List<SlotDefinition> ApparelSlots;
        public List<WeaponTypeDefinition> WeaponTypes;
        public List<ItemGroundPrefab> GroundPrefabs;

        public List<RarityDefinition> ItemRarities;
        public List<ItemTypeNames> ItemTypeNames;
        public List<ConsumableTypeNames> ConsumableTypeNames;

        public AudioContainer LootSound;
        public AudioContainer CraftSound;

        public float MaxLootRange;
        public float LootShowDistance;

        public bool EnableOffHandSlot;
        public bool AllowDualWield; //if wep + offhand
        public float DualWieldRatio; //if wep + offhand

        public bool CanRemoveSockets;
        public bool LimitItemsToClass;
        public bool ItemsHaveRequiredLevel;
        public bool PlayAudioBookOnLoot;
        public bool AllowTwoHanded;

        //Auto-equip
        public bool AutoEquipOnLoot;
        public bool AutoEquipAlways;
        public bool AutoEquipBasedOnLevel;
        public bool AutoEquipIfSlotEmpty = true;

        //Inventory
        public bool InventoryHasMaxItems;
        public int MaxItems;
        public bool InventoryUsesWeightSystem;
        public float MaxWeight;
        public bool AllowOverMax;
        public float MoveSpeedOverMax;

        //Weight Stat Modifier
        public bool BoostCarryWeightByAttr;
        public string AttrForCarryWeightID;
        public float CarryWeightBoostMultiplier;

        public bool DamageHasVariance;

        //Crafting
        public bool EnableCrafting; //todo: mono/gui/editor, just remove the option from NPCs /the window
        public bool CanCraftAnywhere; //todo: gui, directly open craft window
        public bool UseTraitLvlAsReqForCrafting;
            public string TraitIDForCrafting = "";
        public bool UsePlayerLvlAsReqForCrafting;
        public bool ScaleCraftList;
        public bool ScaleDismantleList;

        public float CraftTime; //todo: implement in our code, UI (e.g. progress bar) will just use this value for their bar, but internally in code it'll take this time

        public List<CraftSlotScaling> CraftSlotScalings;

        //Tiers
        public Rm_TierHandler RmTierHandler;
        
        //Stash related
        public StashMode StashMode;

        public bool CharacterStashHasMaxItems;
        public bool CharacterStashUsesWeightSystem;
        public int CharacterStashMaxItems;
        public float CharacterStashMaxWeight;

        public bool GlobalStashHasMaxItems;
        public bool GlobalStashUsesWeightSystem;
        public int GlobalStashMaxItems;
        public float GlobalStashMaxWeight;


        public Rmh_Item()
        {
            StashMode = StashMode.GlobalAndCharacter;
            MaxLootRange = 5;
            LootShowDistance = 10;
            CharacterStashMaxItems = 100;
            GlobalStashMaxItems = 100;
            CharacterStashMaxWeight = 1000;
            GlobalStashMaxWeight = 1000;
            CharacterStashHasMaxItems = true;
            GlobalStashHasMaxItems = true;
            CharacterStashUsesWeightSystem = false;
            GlobalStashUsesWeightSystem = false;

            BoostCarryWeightByAttr = false;
            AttrForCarryWeightID = null;
            CarryWeightBoostMultiplier = 0.1f;

            ItemRarities = new List<RarityDefinition>()
                               {
                                   new RarityDefinition
                                       {
                                           Name = "Common",
                                           Color = Rm_UnityColors.Grey
                                       }
                               };

            WeaponTypes = new List<WeaponTypeDefinition>()
                              {
                                  new WeaponTypeDefinition {ID = "Default_WepType", Name = "Default"}
                              };
            ConsumableTypeNames = new List<ConsumableTypeNames>();
            var consumEnumValues = Enum.GetValues(typeof(RestorationType)) as RestorationType[];
            for (var i = 0; i < consumEnumValues.Length; i++)
            {
                ConsumableTypeNames.Add(new ConsumableTypeNames()
                {
                    Type = consumEnumValues[i],
                    Name = consumEnumValues[i].ToString().Replace("_", " ")
                });
            }

            RmTierHandler = new Rm_TierHandler();
            ItemTypeNames = new List<ItemTypeNames>();
            GroundPrefabs = new List<ItemGroundPrefab>(){ new ItemGroundPrefab()
                                                              {
                                                                  PrefabType = GroundPrefabType.Default,
                                                                  PrefabPath = "Prefabs/GroundLootItems/LootItem"
                                                              }};
            var itemEnumValues = Enum.GetValues(typeof(ItemType)) as ItemType[];
            for (var i = 0; i < itemEnumValues.Length; i++)
            {
                ItemTypeNames.Add(new ItemTypeNames()
                                      {
                                          ItemType =  itemEnumValues[i],
                                          Name = itemEnumValues[i].ToString().Replace("_", " ")
                                      });

                GroundPrefabs.Add(new ItemGroundPrefab()
                {
                    PrefabType = GroundPrefabType.ItemType,
                    ItemType = itemEnumValues[i]
                });
            }
            CraftTime = 1.0f;
            CanRemoveSockets = true;
            LimitItemsToClass = true;
            ItemsHaveRequiredLevel = true;
            PlayAudioBookOnLoot = true;
            EnableOffHandSlot = true;
            DualWieldRatio = 1.0f;

            ApparelSlots = new List<SlotDefinition>(){new SlotDefinition()
                                                            {
                                                                ID="OffHand",
                                                                Name="OffHand"
                                                            }};
            MoveSpeedOverMax = 0.4f;
            EnableCrafting = true;
            UseTraitLvlAsReqForCrafting = false;
            UsePlayerLvlAsReqForCrafting = true;

            ScaleCraftList = true;
            ScaleDismantleList = true;
            CraftSlotScalings = new List<CraftSlotScaling>();

            InventoryHasMaxItems = true;
            MaxItems = 20;
            InventoryUsesWeightSystem = false;
            MaxWeight = 100;

            LootSound = new AudioContainer();
            CraftSound = new AudioContainer();
            DamageHasVariance = true;
        }
    }

    public enum StashMode
    {
        GlobalOnly,
        GlobalAndCharacter,
        CharacterOnly
    }
}
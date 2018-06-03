using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Beta;
using LogicSpawn.RPGMaker.Core;
using UnityEditor;
using UnityEngine;
using Material = LogicSpawn.RPGMaker.Core.Material;

namespace LogicSpawn.RPGMaker.Editor
{
    public static class Rme_Main_Items
    {
        public static Rmh_Item Items
        {
            get { return Rm_RPGHandler.Instance.Items; }
        }

        #region Options

        public static bool showInventoryFoldout = true;
        public static bool showStashFoldout = true;
        public static bool showApparelSlotFoldout = true;
        public static bool showWeaponTypeFoldout = true;
        public static bool showRarityFoldout = true;
        public static bool showItemNamesFoldout = true;
        public static bool showConsumActive = true;
        public static bool showItemOptions = true;
        public static bool showCrafting = true;
        public static bool showCraftScaling = true;
        public static bool showGroundPrefabs = true;
        public static bool[] showWepType = new bool[500];
        public static int selectedTraitNum;
        public static GameObject gameObject = null;
        public static GameObject[] slotPrefab = new GameObject[100];
        public static GameObject[] wepTypePrefab = new GameObject[100];
        public static GameObject[] wepProjPrefab = new GameObject[100];
        public static GameObject[] groundPrefab = new GameObject[100];
        public static GameObject selectedPrefab;
        public static Vector2 optionsScrollPos = Vector2.zero;
        public static void Options(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            GUI.Box(fullArea, "", "backgroundBox");

            GUILayout.BeginArea(fullArea);
            optionsScrollPos = GUILayout.BeginScrollView(optionsScrollPos);
            RPGMakerGUI.Title("Items - Options");

            #region Item Options
            if (RPGMakerGUI.Foldout(ref showItemOptions, "Options"))
            {
                Items.MaxLootRange = RPGMakerGUI.FloatField("Max Loot Range:", Items.MaxLootRange);
                Items.LootSound.Audio = RPGMakerGUI.AudioClipSelector("Loot Drop Sound:", Items.LootSound.Audio,
                                                                ref Items.LootSound.AudioPath);

                Items.AllowTwoHanded = RPGMakerGUI.Toggle("Allow Two-Handed Weapons?", Items.AllowTwoHanded);
                Items.EnableOffHandSlot = RPGMakerGUI.Toggle("Enable OffHand?", Items.EnableOffHandSlot);
                if (Items.EnableOffHandSlot)
                {
                    if(RPGMakerGUI.Toggle("Allow Dual-Wield?", ref Items.AllowDualWield))
                    {
                        Items.DualWieldRatio = RPGMakerGUI.FloatField("Damage Ratio of DW OffHand:", Items.DualWieldRatio);
                    }
                }

                Items.CanRemoveSockets = RPGMakerGUI.Toggle("Can Remove Sockets?", Items.CanRemoveSockets);
                Items.LimitItemsToClass = RPGMakerGUI.Toggle("Limit Items by Class?", Items.LimitItemsToClass);
                Items.ItemsHaveRequiredLevel = RPGMakerGUI.Toggle("Items have Required Level?", Items.ItemsHaveRequiredLevel);
                Items.PlayAudioBookOnLoot = RPGMakerGUI.Toggle("Play AudioBook on Loot?", Items.PlayAudioBookOnLoot);
                if(RPGMakerGUI.Toggle("Auto equip ground items on loot?", ref Items.AutoEquipOnLoot))
                {

                    if (Items.AutoEquipBasedOnLevel || Items.AutoEquipAlways) Items.AutoEquipIfSlotEmpty = false;
                    RPGMakerGUI.Toggle("- Auto-equip if slot is empty?", ref Items.AutoEquipIfSlotEmpty);

                    if (Items.AutoEquipBasedOnLevel || Items.AutoEquipIfSlotEmpty) Items.AutoEquipAlways = false;
                    RPGMakerGUI.Toggle("- Auto-equip always?", ref Items.AutoEquipAlways);

                    if (Items.AutoEquipAlways || Items.AutoEquipIfSlotEmpty) Items.AutoEquipBasedOnLevel = false;
                    RPGMakerGUI.Toggle("- Auto-equip if higher level?", ref Items.AutoEquipBasedOnLevel);


                    if (!Items.AutoEquipBasedOnLevel && !Items.AutoEquipAlways && !Items.AutoEquipIfSlotEmpty) Items.AutoEquipIfSlotEmpty = true;

                }

                RPGMakerGUI.EndFoldout();
            }
            #endregion

            #region Slots, WepTypes and Prefabs

            var slotResult = RPGMakerGUI.FoldoutToolBar(ref showApparelSlotFoldout, "Apparel Slots", "+ApparelSlot");
            if (showApparelSlotFoldout)
            {
                if(!Items.EnableOffHandSlot)
                {
                    var offHandSlot = Items.ApparelSlots.FirstOrDefault(s => s.ID == "OffHand");
                    if(offHandSlot != null) 
                        Items.ApparelSlots.Remove(offHandSlot);
                }
                else
                {
                    var offHandSlot = Items.ApparelSlots.FirstOrDefault(s => s.ID == "OffHand");
                    if (offHandSlot == null)
                        Items.ApparelSlots.Add(new SlotDefinition()
                        {
                            ID = "OffHand",
                            Name = "OffHand"
                        });
                }

                for (int index = 0; index < Items.ApparelSlots.Count; index++)
                {
                    var slot = Items.ApparelSlots[index];
                    GUILayout.BeginHorizontal();

                    slot.Name = EditorGUILayout.TextField("Slot Name:", slot.Name);
                    slotPrefab[index] = Resources.Load(slot.PrefabPath) as GameObject;
                    slotPrefab[index] = RPGMakerGUI.PrefabSelector("Ground Prefab:", slotPrefab[index], ref slot.PrefabPath);

                    if (slot.ID != "OffHand")
                    {
                        if (RPGMakerGUI.DeleteButton())
                        {
                            Items.ApparelSlots.Remove(slot);
                            index--;
                            GUI.FocusControl("");
                        }
                    }
                    else
                    {
                        GUILayout.Space(35);
                    }
                    GUILayout.EndHorizontal();
                    if (slot.ID == "OffHand")
                    {
                        GUILayout.Space(5);
                    }
                }

                if (slotResult == 0)
                {
                    Items.ApparelSlots.Add(new SlotDefinition() {Name = "New Apparel Slot"});
                }
                RPGMakerGUI.EndFoldout();
            }

            var wepResult = RPGMakerGUI.FoldoutToolBar(ref showWeaponTypeFoldout, "Weapon Types", "+WeaponType");
            if (showWeaponTypeFoldout)
            {

                if (Items.WeaponTypes.Count == 0)
                {
                    EditorGUILayout.HelpBox(
                        "Click +WeaponType to define a type of weapon, it's ground prefab and animations.",
                        MessageType.Info);
                }

                for (int index = 0; index < Items.WeaponTypes.Count; index++)
                {
                    var wepType = Items.WeaponTypes[index];
                    if (RPGMakerGUI.Foldout(ref showWepType[index], wepType.Name))
                    {
                        GUILayout.BeginHorizontal();
                        wepType.Name = RPGMakerGUI.TextField("Weapon Name:", wepType.Name);
                        if (Items.WeaponTypes.Count > 1 || index > 0)
                        {
                            if (RPGMakerGUI.DeleteButton(20))
                            {
                                Items.WeaponTypes.Remove(wepType);
                                index--;
                                GUI.FocusControl("");
                            }
                        }
                        GUILayout.EndHorizontal();
                        wepTypePrefab[index] = Resources.Load(wepType.PrefabPath) as GameObject;
                        GUILayout.BeginHorizontal();

                        wepTypePrefab[index] = RPGMakerGUI.PrefabSelector("Ground Prefab:", gameObject,
                                                                          ref wepType.PrefabPath);
                        RPGMakerGUI.PrefabGeneratorButton(Rmh_PrefabType.Loot_Item_Prefab, gameObject, ref wepType.PrefabPath);
                        GUILayout.EndHorizontal();

                        wepType.AttackStyle = (AttackStyle)RPGMakerGUI.EnumPopup("Attack Style:", wepType.AttackStyle);



                        if (Items.AllowTwoHanded)
                        {
                            wepType.IsTwoHanded = RPGMakerGUI.Toggle("Is Two Handed?", wepType.IsTwoHanded);
                        }
                        else
                        {
                            wepType.IsTwoHanded = false;
                        }

                        if (!wepType.IsTwoHanded && Items.AllowDualWield && Items.EnableOffHandSlot)
                        {
                            wepType.AllowDualWield = RPGMakerGUI.Toggle("Can Dual Wield?", wepType.AllowDualWield);
                        }
                        else
                        {
                            wepType.AllowDualWield = false;
                        }



                        wepType.AttackRange = RPGMakerGUI.FloatField("Attack Range:", wepType.AttackRange);
                        wepType.AttackSpeed = RPGMakerGUI.FloatField("Attack Speed:", wepType.AttackSpeed);

                        var isMelee = wepType.AttackStyle == AttackStyle.Melee;
                        var prefabName = isMelee ? "Melee Effect Prefab" : "Projectile Prefab:";

                        wepProjPrefab[index] = Resources.Load(wepType.AutoAttackPrefabPath) as GameObject;

                        GUILayout.BeginHorizontal();
                        wepProjPrefab[index] = RPGMakerGUI.PrefabSelector(prefabName, wepProjPrefab[index],
                                                                            ref wepType.AutoAttackPrefabPath);
                        selectedPrefab = RPGMakerGUI.PrefabGeneratorButton(isMelee ? Rmh_PrefabType.Melee_Effect : Rmh_PrefabType.Auto_Attack_Projectile, selectedPrefab, ref wepType.AutoAttackPrefabPath);
                        GUILayout.EndHorizontal();

                        if (wepType.AttackStyle == AttackStyle.Ranged)
                        {
                            wepType.ProjectileSpeed = RPGMakerGUI.FloatField("Projectile Speed:", wepType.ProjectileSpeed);
                            wepType.ProjectileTravelSound.Audio = RPGMakerGUI.AudioClipSelector("Projectile Travel Sound:", wepType.ProjectileTravelSound.Audio, ref wepType.ProjectileTravelSound.AudioPath);
                        }

                        GUILayout.BeginHorizontal();
                        selectedPrefab = RPGMakerGUI.PrefabSelector("Impact Prefab:", selectedPrefab,
                                                                            ref wepType.AutoAttackImpactPrefabPath);
                        selectedPrefab = RPGMakerGUI.PrefabGeneratorButton(Rmh_PrefabType.Impact, selectedPrefab, ref wepType.AutoAttackImpactPrefabPath);
                        GUILayout.EndHorizontal();


                        wepType.AutoAttackImpactSound.Audio = RPGMakerGUI.AudioClipSelector("Impact Sound:", wepType.AutoAttackImpactSound.Audio, ref wepType.AutoAttackImpactSound.AudioPath);

                        RPGMakerGUI.EndFoldout();
                    }
                }
                


                if (wepResult == 0)
                {
                    Items.WeaponTypes.Add(new WeaponTypeDefinition() {Name = "New Weapon Type"});
                }

                RPGMakerGUI.EndFoldout();
            }


            if (RPGMakerGUI.Foldout(ref showGroundPrefabs, "Ground Prefabs"))
            {
                EditorGUILayout.HelpBox(
                    "These prefabs will be used if you haven't defined individual slots/weapon types.", MessageType.Info);

                var defaultPrefab = Items.GroundPrefabs[0];
                GUILayout.BeginHorizontal();

                groundPrefab[0] = Resources.Load(defaultPrefab.PrefabPath) as GameObject;
                groundPrefab[0] = RPGMakerGUI.PrefabSelector("Default",gameObject, ref defaultPrefab.PrefabPath);
                selectedPrefab = RPGMakerGUI.PrefabGeneratorButton(Rmh_PrefabType.Loot_Item_Prefab, gameObject, ref defaultPrefab.PrefabPath);

                GUILayout.EndHorizontal();

                for (int index = 1; index < Items.GroundPrefabs.Count; index++)
                {
                    var prefab = Items.GroundPrefabs[index];
                    GUILayout.BeginHorizontal();

                    groundPrefab[index] = Resources.Load(prefab.PrefabPath) as GameObject;
                    var itemType = prefab.ItemType.ToString().Replace("_", " ");
                    groundPrefab[index] = RPGMakerGUI.PrefabSelector(itemType,gameObject, ref prefab.PrefabPath);
                    selectedPrefab = RPGMakerGUI.PrefabGeneratorButton(Rmh_PrefabType.Loot_Item_Prefab, gameObject, ref prefab.PrefabPath);

                    GUILayout.EndHorizontal();
                }

                RPGMakerGUI.EndFoldout();
            }

            #endregion

            #region Item Rarities
            var rarityResult = RPGMakerGUI.FoldoutToolBar(ref showRarityFoldout, "Rarities", "+Rarity");
            if (showRarityFoldout)
            {

                if (Items.ItemRarities.Count == 0)
                {
                    EditorGUILayout.HelpBox(
                        "Click +Rarity to define a rarity of item.",
                        MessageType.Info);
                }

                for (int index = 0; index < Items.ItemRarities.Count; index++)
                {
                    var rarity = Items.ItemRarities[index];
                    GUILayout.BeginHorizontal();

                    rarity.Name = RPGMakerGUI.TextField("", rarity.Name);
                    rarity.Color = (Rm_UnityColors)RPGMakerGUI.EnumPopup("Color:", rarity.Color);
                    if (RPGMakerGUI.DeleteButton())
                    {
                        Items.ItemRarities.Remove(rarity);
                        index--;
                        GUI.FocusControl("");
                    }
                    GUILayout.EndHorizontal();
                }

                if (rarityResult == 0)
                {
                    Items.ItemRarities.Add(new RarityDefinition());
                }
                RPGMakerGUI.EndFoldout();
            }
            #endregion

            #region Item Name
            if (RPGMakerGUI.Foldout(ref showItemNamesFoldout, "Item Type Names"))
            {
                for (int index = 0; index < Items.ItemTypeNames.Count; index++)
                {
                    var itemTypeName = Items.ItemTypeNames[index];
                    GUILayout.BeginHorizontal();
                    var itemType = itemTypeName.ItemType.ToString().Replace("_", " ");
                    itemTypeName.Name = RPGMakerGUI.TextField(itemType, itemTypeName.Name);
                    GUILayout.EndHorizontal();
                }

                RPGMakerGUI.EndFoldout();
            }
            #endregion

            #region Consumable Active Names
            if (RPGMakerGUI.Foldout(ref showConsumActive, "Consumable Active Names"))
            {
                for (int index = 0; index < Items.ConsumableTypeNames.Count; index++)
                {
                    var consumableActiveName = Items.ConsumableTypeNames[index];
                    GUILayout.BeginHorizontal();
                    var consumName = consumableActiveName.Type.ToString().Replace("_", " ");
                    consumableActiveName.Name = RPGMakerGUI.TextField(consumName, consumableActiveName.Name);
                    GUILayout.EndHorizontal();
                }

                RPGMakerGUI.EndFoldout();
            }
            #endregion

            #region Inventory

            if (RPGMakerGUI.Foldout(ref showInventoryFoldout, "Inventory"))
            {
                Items.InventoryHasMaxItems = RPGMakerGUI.Toggle("Inventory has Max Items?",
                                                                    Items.InventoryHasMaxItems);
                if (Items.InventoryHasMaxItems)
                {
                    Items.MaxItems = RPGMakerGUI.IntField("Max Items:", Items.MaxItems);
                }
                Items.InventoryUsesWeightSystem = RPGMakerGUI.Toggle("Use Weight System",
                                                                         Items.InventoryUsesWeightSystem);
                if (Items.InventoryUsesWeightSystem)
                {
                    Items.MaxWeight = RPGMakerGUI.FloatField("Max Weight:", Items.MaxWeight);
                    Items.AllowOverMax = RPGMakerGUI.Toggle("Allow Over Max Weight?", Items.AllowOverMax);
                    if (Items.AllowOverMax)
                    {
                        GUILayout.BeginHorizontal();
                        RPGMakerGUI.Label("MoveSpeed Over Max:");
                        Items.MoveSpeedOverMax = EditorGUILayout.Slider(Items.MoveSpeedOverMax, 0.0f, 1.0f);
                        GUILayout.Label((Items.MoveSpeedOverMax * 100) + "%");
                        GUILayout.EndHorizontal();
                    }

                    if (RPGMakerGUI.Toggle("Boost Carry Weight By Attribute?", ref Items.BoostCarryWeightByAttr))
                    {
                        RPGMakerGUI.PopupID<Rm_AttributeDefintion>("Attribute:", ref Items.AttrForCarryWeightID, 1);
                        Items.CarryWeightBoostMultiplier = RPGMakerGUI.FloatField("Multiplier:", Items.CarryWeightBoostMultiplier, 1);
                        RPGMakerGUI.Help("Carry Weight = BaseCarryWeight + (Attr * Multiplier):", 1);
                    }
                }
                RPGMakerGUI.EndFoldout();
            }

            #endregion

            #region Stash

            if (RPGMakerGUI.Foldout(ref showStashFoldout, "Stash"))
            {
                Items.StashMode = (StashMode) RPGMakerGUI.EnumPopup("Stash Mode:", Items.StashMode);

                if(Items.StashMode == StashMode.GlobalOnly || Items.StashMode == StashMode.GlobalAndCharacter)
                {
                    if(RPGMakerGUI.Toggle("Global Stash has Max Items?", ref Items.GlobalStashHasMaxItems))
                    {
                        Items.GlobalStashMaxItems = RPGMakerGUI.IntField("Max Items:", Items.GlobalStashMaxItems,1);
                    }

                    if(RPGMakerGUI.Toggle("Global Stash Uses Weight System", ref Items.GlobalStashUsesWeightSystem))
                    {
                        Items.GlobalStashMaxWeight = RPGMakerGUI.FloatField("Max Weight:", Items.GlobalStashMaxWeight,1);
                    }
                }

                if (Items.StashMode == StashMode.CharacterOnly || Items.StashMode == StashMode.GlobalAndCharacter)
                {
                    if (RPGMakerGUI.Toggle("Character Stash has Max Items?", ref Items.CharacterStashHasMaxItems))
                    {
                        Items.CharacterStashMaxItems = RPGMakerGUI.IntField("Max Items:", Items.CharacterStashMaxItems, 1);
                    }

                    if (RPGMakerGUI.Toggle("Char. Stash Uses Weight System", ref Items.CharacterStashUsesWeightSystem))
                    {
                        Items.CharacterStashMaxWeight = RPGMakerGUI.FloatField("Max Weight:", Items.CharacterStashMaxWeight, 1);
                    }
                }

                RPGMakerGUI.EndFoldout();
            }

            #endregion

            #region Crafting
            if (RPGMakerGUI.Foldout(ref showCrafting, "Crafting Options"))
            {
                Items.EnableCrafting = RPGMakerGUI.Toggle("Enable Crafting?", Items.EnableCrafting);

                if(Items.EnableCrafting)
                {
                    Items.CanCraftAnywhere = RPGMakerGUI.Toggle("Can Craft Anywhere?", Items.CanCraftAnywhere);
                    Items.UsePlayerLvlAsReqForCrafting = RPGMakerGUI.Toggle("Player Lv must match item to Craft?", Items.UsePlayerLvlAsReqForCrafting);
                    Items.UseTraitLvlAsReqForCrafting = RPGMakerGUI.Toggle("Trait Lv must match item to Craft?", Items.UseTraitLvlAsReqForCrafting);
                    if(Items.UseTraitLvlAsReqForCrafting)
                    {
                        RPGMakerGUI.PopupID<Rm_TraitDefintion>("Trait Req at CraftItem Level?", ref Items.TraitIDForCrafting);
                    }
                    Items.CraftSound.Audio = RPGMakerGUI.AudioClipSelector("Crafting Sound:", Items.CraftSound.Audio,
                                                                ref Items.CraftSound.AudioPath);

                    Items.CraftTime = RPGMakerGUI.FloatField("Item Craft Time:", Items.CraftTime);
                    Items.ScaleCraftList = RPGMakerGUI.Toggle("Scale Craft List by Slot?", Items.ScaleCraftList);
                    Items.ScaleDismantleList = RPGMakerGUI.Toggle("Scale Dismantle by Slot?", Items.ScaleDismantleList);
                }

                

                RPGMakerGUI.EndFoldout();
            }
            #endregion

            #region Craft Scaling
            if(Items.EnableCrafting && (Items.ScaleCraftList || Items.ScaleDismantleList))
            {
                if (RPGMakerGUI.Foldout(ref showCraftScaling, "Craft Scaling"))
                {
                    var weaponScaling = Items.CraftSlotScalings.FirstOrDefault(a => a.SlotIdentifier == "Weapon");
                    if (weaponScaling == null)
                    {
                        Items.CraftSlotScalings.Add(new CraftSlotScaling() { SlotIdentifier = "Weapon", SlotName = "Weapon" });
                    }

                    foreach (var d in Items.ApparelSlots)
                    {
                        var attr = Items.CraftSlotScalings.FirstOrDefault(a => a.SlotIdentifier == d.ID);
                        if (attr == null)
                        {
                            Items.CraftSlotScalings.Add(new CraftSlotScaling() { SlotIdentifier = d.ID, SlotName = d.Name });
                        }
                    }

                    for (int index = 0; index < Items.CraftSlotScalings.Count; index++)
                    {
                        var v = Items.CraftSlotScalings[index];
                        if (v.SlotIdentifier == "Weapon") continue;
                        var stillExists =
                            Items.ApparelSlots.FirstOrDefault(
                                a => a.ID == v.SlotIdentifier);

                        if (stillExists == null)
                        {
                            Items.CraftSlotScalings.Remove(v);
                            index--;
                        }
                    }
                    for (int index = 0; index < Items.CraftSlotScalings.Count; index++)
                    {
                        var slotScaling = Items.CraftSlotScalings[index];
                        GUILayout.BeginHorizontal();
                        slotScaling.Multiplier = RPGMakerGUI.IntField(slotScaling.SlotName, slotScaling.Multiplier);
                        GUILayout.EndHorizontal();
                    }

                    RPGMakerGUI.EndFoldout();
                }
            }
            #endregion

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        #endregion

        #region ItemDb
        private static Item selectedItem = null;
        private static string[] ItemTypes = new[]
                                                {
                                                    "Apparel", "Weapon", "Consumable","Material","Socket", "Book",
                                                    "Miscellaneous"
                                                };
        private static string[] itemTypesFilter = new[]
                                                      {
                                                          "All", "Apparel", "Weapon", "Consumable","Material","Socket", "Book",
                                                    "Miscellaneous"
                                                      };
        
        private static List<Item> itemList
        {
            get { return Rm_RPGHandler.Instance.Repositories.Items.AllItems; }
        }
        private static int selectedItemFilter;
        private static int numberOfSockets;
        private static int selectedVarSetterBoolResult;
        private static bool showMainInfo = true;
        private static bool showBookInfo = true;
        private static bool showAttrBuffs = true;
        private static bool showStatBuffs = true;
        private static bool showVitBuffs = true;
        private static bool showBuffItemInfo = true;
        private static bool skillSusFoldout = true;
        private static bool skillImmunFoldout = true;
        private static bool showStatusReduction = true;
        private static bool showVitalRegens = true;
        private static GameObject customItemGroundPrefab;
        private static GameObject weaponProjectile;
        private static Vector2 itemDbScrollPos = Vector2.zero;
        public static void ItemDB(Rect fullArea, Rect leftArea, Rect mainArea)
        {

            GUI.Box(leftArea, "","backgroundBox");
            GUI.Box(mainArea, "","backgroundBoxMain");

            GUILayout.BeginArea(PadRect(leftArea, 0, 0));
            var allItemsList = itemList;

            selectedItemFilter = EditorGUILayout.Popup(selectedItemFilter, itemTypesFilter.Select(i => "Filter: " + i).ToArray(), "filterButton",
                                                           GUILayout.Height(25));
            
            if (selectedItemFilter != 0)
            {
                var item = itemTypesFilter[selectedItemFilter];
                allItemsList = allItemsList.Where(i => i.ItemType.ToString() == item).ToList();
            }
                

            var rect = RPGMakerGUI.ListArea(allItemsList, ref selectedItem, Rm_ListAreaType.Items, true, true);
            var evt = Event.current;
            
            
            if (evt.type == EventType.MouseDown)
            {
                var mousePos = evt.mousePosition;

                if (rect.Contains(mousePos))
                {
                    // Now create the menu, add items and show it
                    var menu = new GenericMenu();

                    foreach (var itemType in ItemTypes)
                    {
                        menu.AddItem(new GUIContent(itemType), false, AddItem(), new [] { itemType, "ItemDB" });
                    }

                    menu.ShowAsContext();
                    evt.Use();
                }
            }
            if (GUI.Button(rect, RPGMakerGUI.AddIcon, "toolBarButtonFlex")) { }

            GUILayout.EndArea();

            GUILayout.BeginArea(mainArea);
            itemDbScrollPos = GUILayout.BeginScrollView(itemDbScrollPos);
            RPGMakerGUI.Title("Item DB");
            if (selectedItem != null)
            {
                ItemDetails(selectedItem);
                BookDetails(selectedItem);

                var buffItem = selectedItem as BuffItem;
                var socket = selectedItem as Socket;
                var consumable = selectedItem as Consumable;
                BuffItemDetails(buffItem);
                if(buffItem != null) ItemBuffs(buffItem);
                if (socket != null) ItemBuffs(socket);
                if (consumable != null) ConsumableItemDetails(consumable);
            }
            else
            {
                EditorGUILayout.HelpBox("Item DB does not include QuestItems and CraftableItems", MessageType.Info);
                RPGMakerGUI.Title("Add or select a new field to customise items.");
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private static GenericMenu.MenuFunction2 AddItem()
        {
            return AddItem;
        }

        private static void AddItem(object userData)
        {
            Item item;
            var array = (string[]) userData;


            var itemToAdd = array[0];
            var listToUse = array[1] == "ItemDB" ? itemList : CraftableItemList;
            switch (itemToAdd)
            {
                case "Miscellaneous":
                    item = new Item {Name = "Misc Item "};
                    break;
                case "Apparel":
                    item = new Apparel { Name = "Apparel Item "};
                    break;
                case "Material":
                    item = new Material() { Name = "New Material Item"};
                    break;
                case "Weapon":
                    item = new Weapon { Name = "Weapon Item "};
                    break;
                case "Consumable":
                    item = new Consumable { Name = "Consumable Item "};
                    break;
                case "Book":
                    item = new Book { Name = "Book Item "};
                    break;
//                case "Key":
//                    item = new Key { Name = "Key Item "};
//                    break;
                case "Socket":
                    item = new Socket { Name = "Socket Item "};
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            listToUse.Add(item);
            if(array[1] == "ItemDB")
            {
                selectedItem = item;
            }
            else
            {
                selectedCraftItemInfo = item;
            }
            GUI.FocusControl("");
        }

        #endregion

        #region Item Details
        private static void ItemDetails(Item curSelectedItem)
        {
            if (RPGMakerGUI.Foldout(ref showMainInfo, "Item Details"))
            {
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical(GUILayout.MaxWidth(85));
                curSelectedItem.Image = RPGMakerGUI.ImageSelector("", curSelectedItem.Image, ref curSelectedItem.ImagePath);
                if (GUILayout.Button("Spawn\nAs Loot", GUILayout.Width(85),GUILayout.Height(50)))
                {
                    var spawnPos = Vector3.zero;
                    Selection.activeObject = SceneView.currentDrawingSceneView;
                    if (Selection.activeObject != null)
                    {
                        var sceneCam = SceneView.currentDrawingSceneView.camera;
                        spawnPos = sceneCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 3f));
                    }

                    var isItem = Rm_RPGHandler.Instance.Repositories.Items.Get(curSelectedItem.ID) != null;
                    var isCraft = Rm_RPGHandler.Instance.Repositories.CraftableItems.Get(curSelectedItem.ID) != null;

                    var type = isItem ? 0 : (isCraft ? 1 : 2);

                    LootSpawner.SpawnWorldLootItem(spawnPos, type, curSelectedItem);
                }
                GUILayout.EndVertical();
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

                curSelectedItem.Name = RPGMakerGUI.TextField("Name: ", curSelectedItem.Name);
                var apparel = curSelectedItem as Apparel;
                if (apparel != null)
                {
                    RPGMakerGUI.PopupID<SlotDefinition>("Slot:", ref apparel.apparelSlotID);
                }

                var weapon = curSelectedItem as Weapon;
                if (weapon != null)
                {
                    RPGMakerGUI.PopupID<WeaponTypeDefinition>("Weapon Type:", ref weapon.WeaponTypeID);

                    #region Damage

                    #region CheckForUpdates

                    var list = weapon.Damage.ElementalDamages;
                    foreach (var d in Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions)
                    {
                        var tier = list.FirstOrDefault(t => t.ElementID == d.ID);
                        if (tier == null)
                        {
                            var tierToAdd = new ElementalDamage() { ElementID = d.ID };
                            list.Add(tierToAdd);
                        }
                    }

                    for (int index = 0; index < list.Count; index++)
                    {
                        var v = list[index];
                        var stillExists =
                            Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions.FirstOrDefault(
                                t => t.ID == v.ElementID);

                        if (stillExists == null)
                        {
                            list.Remove(v);
                            index--;
                        }
                    }

                    #endregion

                    GUILayout.BeginHorizontal();
                    RPGMakerGUI.Label("Physical Damage:");
                    if (Rm_RPGHandler.Instance.Items.DamageHasVariance)
                    {
                        weapon.Damage.MinDamage = RPGMakerGUI.IntField("", weapon.Damage.MinDamage);
                        GUILayout.Label(" - ");
                    }
                    weapon.Damage.MaxDamage = RPGMakerGUI.IntField("", weapon.Damage.MaxDamage);
                    GUILayout.EndHorizontal();

                    foreach (var eleDmg in list)
                    {
                        var nameOfEleDmg =
                            Rm_RPGHandler.Instance.ASVT.ElementalDamageDefinitions.First(
                                e => e.ID == eleDmg.ElementID).Name;

                        GUILayout.BeginHorizontal();
                        RPGMakerGUI.Label(nameOfEleDmg + " Damage:");
                        if (Rm_RPGHandler.Instance.Items.DamageHasVariance)
                        {
                            eleDmg.MinDamage = RPGMakerGUI.IntField("", eleDmg.MinDamage);
                            GUILayout.Label(" - ");
                        }
                        eleDmg.MaxDamage = RPGMakerGUI.IntField("", eleDmg.MaxDamage);
                        GUILayout.EndHorizontal();
                    }

                    #endregion

                    if(RPGMakerGUI.Toggle("Override Weapon Type Range?", ref weapon.OverrideAttackRange))
                    {
                        weapon.NewAttackRange = RPGMakerGUI.FloatField("New Attack Range:", weapon.NewAttackRange);
                    }

                    if (weapon.AttackStyle == AttackStyle.Ranged)
                    {
                        if (RPGMakerGUI.Toggle("Override Projectile Speed?", ref weapon.OverrideProjectileSpeed))
                        {
                            weapon.NewProjectileSpeed = RPGMakerGUI.FloatField("New Projectile Speed:", weapon.NewProjectileSpeed);
                        }


                        weaponProjectile = Resources.Load(weapon.OverrideProjectilePrefabPath) as GameObject;
                        weaponProjectile = RPGMakerGUI.PrefabSelector("Override Projectile Prefab:", weaponProjectile,
                                                                      ref weapon.OverrideProjectilePrefabPath);
                        if (GUILayout.Button("Generate Prefab", "genericButton", GUILayout.MaxHeight(30)))
                        {
                            //todo: spawn existing if it exists
                            //todo: implement
                            //todo: should be same code as weapontype generate prefab
                        }
                    }

                    if (RPGMakerGUI.Toggle("Override Attack Speed?", ref weapon.OverrideAttackSpeed))
                    {
                        weapon.NewAttackSpeed = RPGMakerGUI.FloatField("New Attack Speed:", weapon.NewAttackSpeed);
                    }
                }

                var requiredLv = curSelectedItem as IRequireLevel;
                if (requiredLv != null)
                {
                    if (Items.ItemsHaveRequiredLevel)
                    {
                        requiredLv.RequiredLevel = RPGMakerGUI.IntField("Required Level:", requiredLv.RequiredLevel);
                    }
                }

                if ((weapon != null || apparel != null) && Items.LimitItemsToClass)
                {
                    var isBuffItem = curSelectedItem as BuffItem;
                    if(!RPGMakerGUI.Toggle("All Classes?", ref isBuffItem.AllClasses))
                    {
                            GUILayout.BeginVertical("foldoutBox");
                            var result = RPGMakerGUI.ToolBar("Classes:", new string[] { "+Class" });
                            if (isBuffItem.ClassIDs.Count == 0)
                            {
                                EditorGUILayout.HelpBox("Click +Class to add a class that can use this skill", MessageType.Info);
                            }

                            for (int i = 0; i < isBuffItem.ClassIDs.Count; i++)
                            {

                                GUILayout.BeginHorizontal();
                                var refString = isBuffItem.ClassIDs[i];
                                RPGMakerGUI.PopupID<Rm_ClassNameDefinition>("", ref refString);
                                isBuffItem.ClassIDs[i] = refString;

                                if (RPGMakerGUI.DeleteButton(15))
                                {
                                    isBuffItem.ClassIDs.Remove(isBuffItem.ClassIDs[i]);
                                    i--;
                                }
                                GUILayout.EndHorizontal();
                            }

                            if (result == 0)
                            {
                                isBuffItem.ClassIDs.Add("");
                            }
                            GUILayout.EndVertical();
                    }
                }

                curSelectedItem.Description = RPGMakerGUI.TextArea("Description:",curSelectedItem.Description, 100);

                RPGMakerGUI.PopupID<RarityDefinition>("Rarity:", ref curSelectedItem.RarityID);

                if (Items.InventoryUsesWeightSystem)
                    curSelectedItem.Weight = RPGMakerGUI.FloatField("Weight:", curSelectedItem.Weight);

                curSelectedItem.SellValue = RPGMakerGUI.IntField("Sell Price:", curSelectedItem.SellValue);
                curSelectedItem.BuyValue = RPGMakerGUI.IntField("Buy Price:", curSelectedItem.BuyValue);

                curSelectedItem.CanBeDropped = RPGMakerGUI.Toggle("Can be Dropped?",
                                                                     curSelectedItem.CanBeDropped);

                curSelectedItem.RunEventOnUse = RPGMakerGUI.Toggle("Run Event on Use?", curSelectedItem.RunEventOnUse);
                if (curSelectedItem.RunEventOnUse)
                {
                    RPGMakerGUI.PopupID<NodeChain>("Event To Run:", ref curSelectedItem.EventTreeIdToRunOnUse);
                }

                curSelectedItem.RunEventOnPickup = RPGMakerGUI.Toggle("Run Event on Pickup?", curSelectedItem.RunEventOnPickup);
                if (curSelectedItem.RunEventOnPickup)
                {
                    RPGMakerGUI.PopupID<NodeChain>("Event To Run:", ref curSelectedItem.EventTreeIdToRunOnPickup);
                }

                curSelectedItem.RunEventOnDrop = RPGMakerGUI.Toggle("Run Event on Drop?", curSelectedItem.RunEventOnDrop);
                if (curSelectedItem.RunEventOnDrop)
                {
                    RPGMakerGUI.PopupID<NodeChain>("Event To Run:", ref curSelectedItem.EventTreeIdToRunOnDrop);
                }

                customItemGroundPrefab = Resources.Load(curSelectedItem.CustomGroundPrefabPath) as GameObject;
                customItemGroundPrefab = RPGMakerGUI.PrefabSelector("Custom Ground Prefab:",
                                                                    customItemGroundPrefab,
                                                                    ref curSelectedItem.CustomGroundPrefabPath);

                GUILayout.EndVertical();
                GUILayout.EndHorizontal();

                RPGMakerGUI.EndFoldout();
            }
        }
        private static void BookDetails(Item curSelectedItem)
        {
            if (curSelectedItem.ItemType != ItemType.Book) return;

            var book = curSelectedItem as Book;
            int bookResult;
            if (book.BookType != BookType.Audio)
            {
                bookResult = RPGMakerGUI.FoldoutToolBar(ref showBookInfo, "Book Details", "+Page");
            }
            else
            {
                bookResult = -1;
                RPGMakerGUI.Foldout(ref showBookInfo, "Book Details");
            }

            if (showBookInfo)
            {
                book.Title = RPGMakerGUI.TextField("Book Title:", book.Title);
                book.Author = RPGMakerGUI.TextField("Book Author:", book.Author);
                var oldBookType = book.BookType;
                book.BookType = (BookType)RPGMakerGUI.EnumPopup("Book Type:", book.BookType);
                if (book.BookType != oldBookType)
                {
                    book.CurrentPage = 0;
                }

                if (book.BookType == BookType.Picture)
                {
                    book.ImageSize.X = RPGMakerGUI.FloatField("Images' Width:", book.ImageSize.X);
                    book.ImageSize.Y = RPGMakerGUI.FloatField("Images' Height:", book.ImageSize.Y);
                }

                if (book.BookType != BookType.Audio)
                {
                    GUILayout.BeginHorizontal();
                    RPGMakerGUI.Label("Page " + (book.CurrentPage + 1) + " " + book.BookType);
                    GUI.enabled = book.CurrentPage > 0;
                    if (GUILayout.Button("Prev Page", "genericButton", GUILayout.Width(100),
                                         GUILayout.Height(25)))
                    {
                        book.CurrentPage--;
                        GUI.FocusControl("");
                    }
                    GUI.enabled = book.CurrentPage + 1 < book.Pages;
                    if (GUILayout.Button("Next Page", "genericButton", GUILayout.Width(100),
                                         GUILayout.Height(25)))
                    {
                        book.CurrentPage++;
                        GUI.FocusControl("");
                    }
                    GUI.enabled = book.Pages > 1;
                    if (RPGMakerGUI.DeleteButton())
                    {
                        switch (book.BookType)
                        {
                            case BookType.Text:
                                book.PageText.RemoveAt(book.CurrentPage);
                                break;
                            case BookType.Picture:
                                book.Images.RemoveAt(book.CurrentPage);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        book.CurrentPage = Math.Max(book.CurrentPage - 1, 0);
                    }
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();
                }
                GUILayout.Space(5);
                if (book.BookType == BookType.Text)
                {
                    book.PageText[book.CurrentPage] = RPGMakerGUI.TextArea("Page Text:",book.PageText[book.CurrentPage], 100);
                }
                else if (book.BookType == BookType.Audio)
                {
                    book.Audio = RPGMakerGUI.AudioClipSelector("Audio", book.Audio, ref book.AudioPath);
                }
                else
                {
                    book.Images[book.CurrentPage].Image = RPGMakerGUI.ImageSelector("Image",
                                                                                    book.Images[book.CurrentPage
                                                                                        ].Image,
                                                                                    ref
                                                                                        book.Images[
                                                                                            book.CurrentPage].
                                                                                        ImagePath, true,
                                                                                    GUILayout.Width(200),
                                                                                    GUILayout.Height(113));
                }

                if (bookResult == 0)
                {
                    switch (book.BookType)
                    {
                        case BookType.Text:
                            book.PageText.Add("New Page");
                            break;
                        case BookType.Picture:
                            book.Images.Add(new ImageContainer());
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                RPGMakerGUI.EndFoldout();
            }
        }

        private static void ConsumableItemDetails(Consumable consumable)
        {
            if (RPGMakerGUI.Foldout(ref showBuffItemInfo, "Equippable Details"))
            {
                consumable.Restoration.RestorationType = (RestorationType)RPGMakerGUI.EnumPopup("Consumable Type:", consumable.Restoration.RestorationType);

                if (consumable.Restoration.RestorationType == RestorationType.Time_Based)
                {
                    consumable.Restoration.Duration = RPGMakerGUI.FloatField("Duration:", consumable.Restoration.Duration);
                    consumable.Restoration.SecBetweenRestore = RPGMakerGUI.FloatField("Seconds Between Restore:", consumable.Restoration.SecBetweenRestore);    

                }

                if(RPGMakerGUI.Toggle("Restores Vital?", ref consumable.RestoresVital))
                {
                    RPGMakerGUI.PopupID<Rm_VitalDefinition>("Vital To Restore:", ref consumable.Restoration.VitalToRestoreID);
                    if (RPGMakerGUI.Toggle("Restores Fixed Amount?", ref consumable.Restoration.FixedRestore))
                    {
                        consumable.Restoration.AmountToRestore = RPGMakerGUI.IntField("Amount Restored:", consumable.Restoration.AmountToRestore);
                    }
                    else
                    {
                        GUILayout.BeginHorizontal();
                        consumable.Restoration.PercentToRestore = RPGMakerGUI.FloatField("Amount Restored:", consumable.Restoration.PercentToRestore);
                        RPGMakerGUI.Label((consumable.Restoration.PercentToRestore * 100).ToString("N2") + "%");
                        GUILayout.EndHorizontal();
                    }
                }

                if (RPGMakerGUI.Toggle("Adds Status Effect?", ref consumable.AddsStatusEffect))
                {
                    RPGMakerGUI.PopupID<StatusEffect>("Status Effect To Add:", ref consumable.AddStatusEffectID);
                }

                if (RPGMakerGUI.Toggle("Removes Status Effect?", ref consumable.RemovesStatusEffect))
                {
                    RPGMakerGUI.PopupID<StatusEffect>("Status Effect To Remove:", ref consumable.RemoveStatusEffectID);
                }

                consumable.Cooldown = RPGMakerGUI.FloatField("Cooldown:", consumable.Cooldown);

                RPGMakerGUI.EndFoldout();
            }
        }

        private static void BuffItemDetails(BuffItem buffItem)
        {
            if (buffItem != null)
            {

                if (RPGMakerGUI.Foldout(ref showBuffItemInfo, "Equippable Details"))
                {
                    numberOfSockets = buffItem.SocketHolder.Count;
                    numberOfSockets = RPGMakerGUI.IntField("Socket Slots:", numberOfSockets);
                    if (numberOfSockets < 0) numberOfSockets = 0;

                    buffItem.SocketHolder.Sockets = new Socket[numberOfSockets];

                    if (buffItem as Weapon != null)
                    {
                        var weapon = buffItem as Weapon;
                        weapon.HasProcEffect = RPGMakerGUI.Toggle("Has Proc?", weapon.HasProcEffect);
                        if (weapon.HasProcEffect)
                        {
                            GUILayout.BeginVertical("backgroundBox");
                            var procEffect = weapon.ProcEffect;

                            procEffect.ProcCondition =
                                (Rm_ProcCondition)RPGMakerGUI.EnumPopup("Proc Condition", procEffect.ProcCondition);
                            if (procEffect.ProcCondition == Rm_ProcCondition.Every_N_Hits)
                            {
                                procEffect.Parameter = RPGMakerGUI.FloatField("N:", procEffect.Parameter);
                                procEffect.Parameter = (int)procEffect.Parameter;
                            }
                            if (procEffect.ProcCondition == Rm_ProcCondition.Chance_On_Hit ||
                                procEffect.ProcCondition == Rm_ProcCondition.Chance_On_Critical_Hit)
                            {
                                procEffect.Parameter = RPGMakerGUI.FloatField("% Chance:", procEffect.Parameter);

                            }
                            procEffect.ProcEffectType =
                                (Rm_ProcEffectType)RPGMakerGUI.EnumPopup("Effect:", procEffect.ProcEffectType);

                            if (procEffect.ProcEffectType == Rm_ProcEffectType.StatusEffect ||
                                procEffect.ProcEffectType == Rm_ProcEffectType.StatusEffectOnSelf)
                            {
                                RPGMakerGUI.PopupID<StatusEffect>("Status Effect:", ref procEffect.EffectParameterString);
                            }

                            if (procEffect.ProcEffectType == Rm_ProcEffectType.CastSkill || procEffect.ProcEffectType == Rm_ProcEffectType.CastSkillOnSelf)
                            {
                                RPGMakerGUI.PopupID<Skill>("Skill Name:", ref procEffect.EffectParameterString);
                            }

                            if (procEffect.ProcEffectType == Rm_ProcEffectType.KnockBack || procEffect.ProcEffectType == Rm_ProcEffectType.KnockUp)
                            {
                                procEffect.EffectParameter = RPGMakerGUI.FloatField("Distance:", procEffect.EffectParameter);
                            }
                            GUILayout.EndVertical();
                        }
                    }

                    buffItem.SetCVarOnEquip = RPGMakerGUI.Toggle("Set Custom Var on Equip?", buffItem.SetCVarOnEquip);

                    if (buffItem.SetCVarOnEquip)
                    {
                        var varSetter = buffItem.CustomVariableOnEquip;
                        RPGMakerGUI.PopupID<Rmh_CustomVariable>("Custom Variable:", ref varSetter.VariableID);
                        var foundCvar = Rm_RPGHandler.Instance.DefinedVariables.Vars.FirstOrDefault(v => v.ID == varSetter.VariableID);
                        if(foundCvar != null)
                        {
                            CustomVariableOptions(varSetter,foundCvar.VariableType);
                        }
                    }

                    buffItem.SetCVarOnUnEquip = RPGMakerGUI.Toggle("Set Custom Var on Unequip?", buffItem.SetCVarOnUnEquip);
                    if (buffItem.SetCVarOnUnEquip)
                    {
                        var varSetter = buffItem.CustomVariableOnUnEquip;
                        RPGMakerGUI.PopupID<Rmh_CustomVariable>("Custom Variable:", ref varSetter.VariableID);
                        var foundCvar = Rm_RPGHandler.Instance.DefinedVariables.Vars.FirstOrDefault(v => v.ID == varSetter.VariableID);
                        if (foundCvar != null)
                        {
                            CustomVariableOptions(varSetter, foundCvar.VariableType);
                        }
                    }
                    RPGMakerGUI.EndFoldout();
                }
            }
        }
        private static void ItemBuffs(Socket socket)
        {
            ItemBuffs(socket.AttributeBuffs,socket.StatisticBuffs, socket.VitalBuffs,
                socket.StatusDurationReduction, socket.VitalRegenBonuses, ref socket.SkillMetaImmunitiesID, ref socket.SkillMetaSusceptibilities);
        }
        private static void ItemBuffs(BuffItem buffItem)
        {
            ItemBuffs(buffItem.AttributeBuffs, buffItem.StatisticBuffs, buffItem.VitalBuffs,
                buffItem.StatusDurationReduction, buffItem.VitalRegenBonuses, ref buffItem.SkillMetaImmunitiesID, ref buffItem.SkillMetaSusceptibilities);
        }
        private static void ItemBuffs(List<AttributeBuff> attrBuffs, List<StatisticBuff> statBuffs, List<VitalBuff> vitBuffs,
            List<ReduceStatusDuration> statReducs, List<VitalRegenBonus> vitRegens, ref List<SkillImmunity> skillImmuns, ref List<SkillMetaSusceptibility> skillSus)
        {
            var attrBuffResult = RPGMakerGUI.FoldoutToolBar(ref showAttrBuffs, "Attribute Buffs",
                                                                "+AttributeBuff");
                if (showAttrBuffs)
                {

                    for (int index = 0; index < attrBuffs.Count; index++)
                    {
                        var v = attrBuffs[index];
                        if (String.IsNullOrEmpty(v.AttributeID)) continue;

                        var stillExists =
                            Rm_RPGHandler.Instance.ASVT.AttributesDefinitions.FirstOrDefault(
                                a => a.ID == v.AttributeID);

                        if (stillExists == null)
                        {
                            attrBuffs.Remove(v);
                            index--;
                        }
                    }
                    var allAttributes = Rm_RPGHandler.Instance.ASVT.AttributesDefinitions;
                    if (allAttributes.Count > 0)
                    {
                        if (attrBuffs.Count == 0)
                        {
                            EditorGUILayout.HelpBox("Click +AttributeBuff to add an attribute buff.",
                                                    MessageType.Info);
                        }

                        GUILayout.Space(5);
                        for (int index = 0; index < attrBuffs.Count; index++)
                        {
                            GUILayout.BeginHorizontal();

                            GUILayout.BeginHorizontal();
                            RPGMakerGUI.PopupID<Rm_AttributeDefintion>("Attribute Name:", ref attrBuffs[index].AttributeID);
                            attrBuffs[index].Amount = RPGMakerGUI.IntField("Amount:", attrBuffs[index].Amount);
                            if (RPGMakerGUI.DeleteButton())
                            {
                                attrBuffs.Remove(attrBuffs[index]);
                                index--;
                                return;
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.EndHorizontal();
                            GUILayout.Space(5);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No Attributes Found.", MessageType.Warning);
                        attrBuffs = new List<AttributeBuff>();
                    }

                    if (attrBuffResult == 0)
                    {
                        attrBuffs.Add(new AttributeBuff());
                        GUI.FocusControl("");
                    }
                    RPGMakerGUI.EndFoldout();
                }

                var statBuffResult = RPGMakerGUI.FoldoutToolBar(ref showStatBuffs, "Statistic Buffs",
                                                                "+StatisticBuff");
                if (showStatBuffs)
                {
                    for (int index = 0; index < statBuffs.Count; index++)
                    {
                        var v = statBuffs[index];
                        if (String.IsNullOrEmpty(v.StatisticID)) continue;

                        var stillExists =
                            Rm_RPGHandler.Instance.ASVT.StatisticDefinitions.FirstOrDefault(
                                a => a.ID == v.StatisticID);

                        if (stillExists == null)
                        {
                            statBuffs.Remove(v);
                            index--;
                        }
                    }
                    var allStatistics = Rm_RPGHandler.Instance.ASVT.StatisticDefinitions;
                    if (allStatistics.Count > 0)
                    {
                        if (statBuffs.Count == 0)
                        {
                            EditorGUILayout.HelpBox("Click +StatisticBuff to add a Statistic buff.",
                                                    MessageType.Info);
                        }

                        GUILayout.Space(5);
                        for (int index = 0; index < statBuffs.Count; index++)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.BeginHorizontal();
                            RPGMakerGUI.PopupID<Rm_StatisticDefintion>("Statistic Name:", ref statBuffs[index].StatisticID);
                            statBuffs[index].Amount = RPGMakerGUI.FloatField("Amount:", statBuffs[index].Amount);
                            if (RPGMakerGUI.DeleteButton())
                            {
                                statBuffs.Remove(statBuffs[index]);
                                index--;
                                return;
                            }
                            GUILayout.EndHorizontal();


                            GUILayout.EndHorizontal();
                            GUILayout.Space(5);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No Statistics Found.", MessageType.Warning);
                        statBuffs = new List<StatisticBuff>();
                    }

                    if (statBuffResult == 0)
                    {
                        statBuffs.Add(new StatisticBuff());
                        GUI.FocusControl("");
                    }
                    RPGMakerGUI.EndFoldout();
                }

                var vitBuffResult = RPGMakerGUI.FoldoutToolBar(ref showVitBuffs, "Vital Buffs", "+VitalBuff");
                if (showVitBuffs)
                {
                    for (int index = 0; index < vitBuffs.Count; index++)
                    {
                        var v = vitBuffs[index];
                        if (String.IsNullOrEmpty(v.VitalID)) continue;

                        var stillExists =
                            Rm_RPGHandler.Instance.ASVT.VitalDefinitions.FirstOrDefault(
                                a => a.ID == v.VitalID);

                        if (stillExists == null)
                        {
                            vitBuffs.Remove(v);
                            index--;
                        }
                    }
                    var allVitals = Rm_RPGHandler.Instance.ASVT.VitalDefinitions;
                    if (allVitals.Count > 0)
                    {
                        if (vitBuffs.Count == 0)
                        {
                            EditorGUILayout.HelpBox("Click +VitalBuff to add an Vital buff.", MessageType.Info);
                        }

                        GUILayout.Space(5);
                        for (int index = 0; index < vitBuffs.Count; index++)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.BeginHorizontal();
                            RPGMakerGUI.PopupID<Rm_VitalDefinition>("Vital Name:", ref vitBuffs[index].VitalID);
                            vitBuffs[index].Amount = RPGMakerGUI.IntField("Amount:", vitBuffs[index].Amount);
                            if (RPGMakerGUI.DeleteButton())
                            {
                                vitBuffs.Remove(vitBuffs[index]);
                                index--;
                                return;
                            }
                            GUILayout.EndHorizontal();


                            GUILayout.EndHorizontal();
                            GUILayout.Space(5);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No Vitals Found.", MessageType.Warning);
                        vitBuffs = new List<VitalBuff>();
                    }

                    if (vitBuffResult == 0)
                    {
                        vitBuffs.Add(new VitalBuff());
                        GUI.FocusControl("");
                    }
                    RPGMakerGUI.EndFoldout();
                }


                RPGMakerGUI.FoldoutList(ref showStatusReduction, "Status Reductions", statReducs, Rm_RPGHandler.Instance.Repositories.StatusEffects.AllStatusEffects, "+StatusReduction",
                    "Status Effect", "Click +StatusReduction to add a status reduction effect", "StatusEffectID", "Name", "ID", "Name");
                RPGMakerGUI.FoldoutList(ref showVitalRegens, "Vital Regen Bonuses", vitRegens, Rm_RPGHandler.Instance.ASVT.VitalDefinitions, "+VitalRegenBonus",
                    "Vital", "Click +VitalRegenBonus to add a regen bonus", "VitalID", "Name", "ID", "Name");


                #region SkillImmunities

                var immunityResult = RPGMakerGUI.FoldoutToolBar(ref skillImmunFoldout, "Skill Immunities",
                                                                new string[] { "+Immunity" });
                if (skillImmunFoldout)
                {
                    var allSkillMetas = Rm_RPGHandler.Instance.Combat.SkillMeta;
                    if (allSkillMetas.Count > 0)
                    {
                        if (skillImmuns.Count == 0)
                        {
                            EditorGUILayout.HelpBox("Click +Immunity to add a skill meta this class is immune to.",
                                                    MessageType.Info);
                        }

                        GUILayout.Space(5);
                        for (int index = 0; index < skillImmuns.Count; index++)
                        {
                            GUILayout.BeginHorizontal();
                            var skillImmunity = skillImmuns[index];
                            RPGMakerGUI.PopupID<SkillMetaDefinition>("Immunity:", ref skillImmunity.ID);

                            if (GUILayout.Button(RPGMakerGUI.DelIcon, "genericButton", GUILayout.Width(30),
                                                 GUILayout.Height(30)))
                            {
                                skillImmuns.Remove(skillImmuns[index]);
                                index--;
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.Space(5);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No Skill Metas Found.", MessageType.Info);
                        skillImmuns = new List<SkillImmunity>();
                    }

                    if (immunityResult == 0)
                    {
                        skillImmuns.Add(new SkillImmunity() { ID = "" });
                    }
                    RPGMakerGUI.EndFoldout();
                }

                #endregion

                #region SkillSusceptibility
                var susceptResult = RPGMakerGUI.FoldoutToolBar(ref skillSusFoldout, "Skill Susceptibilities", new string[] { "+Susceptibility" });
                if (skillSusFoldout)
                {
                    for (int index = 0; index < skillSus.Count; index++)
                    {
                        var v = skillSus[index];
                        if (string.IsNullOrEmpty(v.ID)) continue;

                        var stillExists =
                            Rm_RPGHandler.Instance.Combat.SkillMeta.FirstOrDefault(
                                a => a.ID == v.ID);

                        if (stillExists == null)
                        {
                            skillSus.Remove(v);
                            index--;
                        }
                    }
                    var allSkillMetas = Rm_RPGHandler.Instance.Combat.SkillMeta;
                    if (allSkillMetas.Count > 0)
                    {
                        if (skillSus.Count == 0)
                        {
                            EditorGUILayout.HelpBox("Click +Susceptibility to add a skill meta this class takes more damage from.", MessageType.Info);
                        }

                        GUILayout.Space(5);
                        for (int index = 0; index < skillSus.Count; index++)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.BeginHorizontal();
                            RPGMakerGUI.PopupID<SkillMetaDefinition>("Susceptibility", ref skillSus[index].ID);

                            var selectedSus = skillSus[index];
                            selectedSus.AdditionalDamage = RPGMakerGUI.FloatField("Additional Damage:", selectedSus.AdditionalDamage);
                            GUILayout.EndHorizontal();

                            if (GUILayout.Button(RPGMakerGUI.DelIcon, "genericButton", GUILayout.Width(30), GUILayout.Height(30)))
                            {
                                skillSus.Remove(skillSus[index]);
                                index--;
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.Space(5);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No Skill Metas Found.", MessageType.Info);
                        skillSus = new List<SkillMetaSusceptibility>();
                    }

                    if (susceptResult == 0)
                    {
                        skillSus.Add(new SkillMetaSusceptibility());
                    }
                    RPGMakerGUI.EndFoldout();
                }
                #endregion
        }
        private static void CustomVariableOptions(Rm_CustomVariableGetSet varSetter, Rmh_CustomVariableType varType)
        {
            switch (varType)
            {
                case Rmh_CustomVariableType.Float:
                    varSetter.FloatValue = RPGMakerGUI.FloatField("Set To:", varSetter.FloatValue);
                    break;
                case Rmh_CustomVariableType.Int:
                    varSetter.IntValue = RPGMakerGUI.IntField("Set To:", varSetter.IntValue);
                    break;
                case Rmh_CustomVariableType.String:
                    varSetter.StringValue = RPGMakerGUI.TextField("Set To:", varSetter.StringValue);
                    break;
                case Rmh_CustomVariableType.Bool:
                    selectedVarSetterBoolResult = varSetter.BoolValue ? 0 : 1;
                    selectedVarSetterBoolResult = EditorGUILayout.Popup("Set To:",
                                                                        selectedVarSetterBoolResult,
                                                                        new[] { "True", "False" });
                    varSetter.BoolValue = selectedVarSetterBoolResult == 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        #endregion

        #region CraftableItems

        private static List<Item> CraftableItemList
        {
            get { return Rm_RPGHandler.Instance.Repositories.CraftableItems.AllItems; }
        }

        private static int selectedCraftItemFilter = 0;
        private static Item selectedCraftItemInfo = null;
        private static Vector2 craftItemScrollPos = Vector2.zero;

        public static void CraftableItems(Rect fullArea, Rect leftArea, Rect mainArea)
        {

            GUI.Box(leftArea, "","backgroundBox");
            GUI.Box(mainArea, "", "backgroundBoxMain");

            GUILayout.BeginArea(PadRect(leftArea, 0, 0));
            var allItemsList = CraftableItemList;

            selectedCraftItemFilter = EditorGUILayout.Popup(selectedCraftItemFilter, itemTypesFilter, "Box",
                                                            GUILayout.Height(25));
            if (selectedCraftItemFilter != 0)
                allItemsList =
                    allItemsList.Where(i => i.ItemType.ToString() == itemTypesFilter[selectedCraftItemFilter]).ToList();

            var rect = RPGMakerGUI.ListArea(allItemsList, ref selectedCraftItemInfo, Rm_ListAreaType.Items, true, true);
            var evt = Event.current;
            if (evt.type == EventType.MouseDown)
            {
                var mousePos = evt.mousePosition;

                if (rect.Contains(mousePos))
                {
                    // Now create the menu, add items and show it
                    var menu = new GenericMenu();

                    foreach (var itemType in ItemTypes)
                    {
                        menu.AddItem(new GUIContent(itemType), false, AddItem(), new[] {itemType, "CraftItemDB"});
                    }

                    menu.ShowAsContext();
                    evt.Use();
                }
            }
            GUILayout.EndArea();

            GUILayout.BeginArea(mainArea);
            craftItemScrollPos = GUILayout.BeginScrollView(craftItemScrollPos);
            RPGMakerGUI.Title("Craftable Item DB");
            if (selectedCraftItemInfo != null)
            {
                ItemDetails(selectedCraftItemInfo); 
                BookDetails(selectedCraftItemInfo);

                var buffItem = selectedCraftItemInfo as BuffItem;
                var socket = selectedCraftItemInfo as Socket;
                var consumable = selectedCraftItemInfo as Consumable;

                BuffItemDetails(buffItem);
                if (buffItem != null) ItemBuffs(buffItem);
                if (socket != null) ItemBuffs(socket);
                if(consumable != null) ConsumableItemDetails(consumable);

            }
            else
            {
                EditorGUILayout.HelpBox("Add or select a new field to customise craftable items.", MessageType.Info);
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        #endregion

        #region DoubleListRects

        private static Rect leftAreaB;
        private static Rect mainAreaAlt;
        private static bool rectsSet;
        #endregion

        #region Tiers
        public static Rm_TierHandler TierHandler
        {
            get { return Items.RmTierHandler; }
        }
        public static Vector2 tierScrollPos = Vector2.zero;
        public static bool showTierInfos = true;
        public static void Tiers(Rect fullArea, Rect leftBar, Rect mainArea)
        {
            GUI.Box(fullArea, "", "backgroundBox");
            GUILayout.BeginArea(fullArea);
            tierScrollPos = GUILayout.BeginScrollView(tierScrollPos);

            RPGMakerGUI.Title("Items - Tiers");
            var tierResult = RPGMakerGUI.FoldoutToolBar(ref showTierInfos, "Tiers", "+Tier");
            if (showTierInfos)
            {
                if (TierHandler.Tiers.Count == 0)
                {
                    EditorGUILayout.HelpBox("Click +Tier to add a crafting tier to the game.", MessageType.Info);
                }

                for (int index = 0; index < TierHandler.Tiers.Count; index++)
                {
                    var tier = TierHandler.Tiers[index];
                    GUILayout.BeginHorizontal();

                    tier.Name = RPGMakerGUI.TextField("Tier Name:", tier.Name);
                    if (index == 0)
                    {
                        tier.minLevel = 0;

                        if (TierHandler.Tiers.Count == 1) tier.maxLevel = Rm_RPGHandler.Instance.Experience.MaxPlayerLevel;

                        GUILayout.BeginHorizontal();
                        RPGMakerGUI.MinMaxSlider(new GUIContent("Level Range:"),
                                                        ref tier.minLevel, ref tier.maxLevel, 0,
                                                        Rm_RPGHandler.Instance.Experience.MaxPlayerLevel);
                        tier.minLevel = tier.MinLevel;
                        tier.maxLevel = tier.MaxLevel;
                        GUILayout.Label(tier.MinLevel + " - " + tier.MaxLevel, GUILayout.Width(120));
                        GUILayout.EndHorizontal();
                    }
                    else if (index - 1 >= 0)
                    {
                        var prevTable = TierHandler.Tiers[index - 1];
                        tier.minLevel = prevTable.maxLevel + 1;
                        if (tier.maxLevel < tier.minLevel)
                        {
                            tier.maxLevel = tier.minLevel;
                        }
                        if (index == TierHandler.Tiers.Count - 1)
                        {
                            tier.maxLevel = Rm_RPGHandler.Instance.Experience.MaxPlayerLevel;
                        }
                        if (tier.minLevel > Rm_RPGHandler.Instance.Experience.MaxPlayerLevel)
                        {
                            tier.minLevel = tier.maxLevel = -1;
                        }
                        GUILayout.BeginHorizontal();
                        RPGMakerGUI.MinMaxSlider(new GUIContent("Level Range:"),
                                                        ref tier.minLevel, ref tier.maxLevel, 0,
                                                        Rm_RPGHandler.Instance.Experience.MaxPlayerLevel);
                        tier.minLevel = tier.MinLevel;
                        tier.maxLevel = tier.MaxLevel;
                        if(tier.minLevel == -1 && tier.maxLevel == -1 )
                        {
                            GUILayout.Label("", GUILayout.Width(120));    
                        }
                        else
                        {
                            GUILayout.Label(tier.MinLevel + " - " + tier.MaxLevel, GUILayout.Width(120));    
                        }
                        
                        GUILayout.EndHorizontal();
                    }

                    if (RPGMakerGUI.DeleteButton())
                    {
                        TierHandler.Tiers.Remove(tier);
                        index--;
                    }
                    GUILayout.EndHorizontal();
                }

                if (tierResult == 0)
                {
                    TierHandler.Tiers.Add(new Rm_Tier());
                }

                RPGMakerGUI.EndFoldout();
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
        #endregion

        #region CraftLists

        private static Rm_CustomCraftList selectedCustomListInfo = null;
        private static Rm_TierCraftList selectedTierListInfo = null;
        private static CraftListItem selectedCraftListItem = null;
        private static int selectedCraftListList;
        private static int selectedCraftListItemIndex;
        private static int selectedCustomCraftItemIndex;

        public static void CraftLists(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            
            leftAreaB = new Rect(leftArea.xMax + 5, leftArea.y, leftArea.width, leftArea.height);
            mainAreaAlt = new Rect(leftAreaB.xMax + 5, leftArea.y, mainArea.width - (leftAreaB.width + 5),
                                    leftArea.height);

            GUI.Box(leftArea, "", "backgroundBox");
            GUI.Box(leftAreaB, "", "backgroundBox");
            GUI.Box(mainAreaAlt, "", "backgroundBox");

             

            GUILayout.BeginArea(PadRect(leftArea, 0, 0));
            selectedCraftListList = EditorGUILayout.Popup(selectedCraftListList,
                                                          new[] { "Mode: Tier Craft Lists", "Mode: Custom Craft Lists" }, "filterButton",
                                                          GUILayout.Height(25));
            if (selectedCraftListList == 0)
            {
                var list = Rm_RPGHandler.Instance.Repositories.CraftLists.TierCraftLists;


                #region CheckForUpdates
                foreach (var d in TierHandler.Tiers)
                {
                    var tier = list.FirstOrDefault(t => t.TierID == d.TierID);
                    if (tier == null)
                    {
                        var tierToAdd = new Rm_TierCraftList() {TierID = d.TierID};
                        list.Add(tierToAdd);
                    }
                }

                for (int index = 0; index < list.Count; index++)
                {
                    var v = list[index];
                    var stillExists =
                        TierHandler.Tiers.FirstOrDefault(t => t.TierID == v.TierID);

                    if (stillExists == null)
                    {
                        list.Remove(v);
                        index--;
                    }
                }

                #endregion

                RPGMakerGUI.ListArea(list, ref selectedTierListInfo, Rm_ListAreaType.TierCraftList, false, true, Rme_ListButtonsToShow.HelpOnly);
            }
            else
            {
                var list = Rm_RPGHandler.Instance.Repositories.CraftLists.CustomCraftLists;
                RPGMakerGUI.ListArea(list, ref selectedCustomListInfo, Rm_ListAreaType.CraftList, false, true);
            }
            GUILayout.EndArea();

            var selectedCraftList = selectedCraftListList == 0
                            ? (Rm_CraftList)selectedTierListInfo
                            : (Rm_CraftList)selectedCustomListInfo;

            GUILayout.BeginArea(leftAreaB);
            if (selectedCraftList != null)
            {
                RPGMakerGUI.ListArea(selectedCraftList.ItemsNeededIDs, ref selectedCraftListItem, Rm_ListAreaType.CraftListItem, false, false, Rme_ListButtonsToShow.AllExceptHelp, true);

            }
            GUILayout.EndArea();

            GUILayout.BeginArea(mainAreaAlt);
            RPGMakerGUI.Title("Craft Lists");

            if (selectedCraftList != null)
            {
                var tierList = selectedCraftList as Rm_TierCraftList;
                var customList = selectedCraftList as Rm_CustomCraftList;

                if (tierList != null)
                {
                    RPGMakerGUI.SubTitle("Tier Craft List");
                    EditorGUILayout.LabelField("Tier:",Rm_RPGHandler.Instance.Items.RmTierHandler.GetTierName(tierList.TierID));
                }

                if (customList != null)
                {
                    RPGMakerGUI.SubTitle("Custom Craft List");

                    var allCraftableItems = Rm_RPGHandler.Instance.Repositories.CraftableItems.AllItems;

                    if (allCraftableItems.Count == 0)
                    {
                        EditorGUILayout.HelpBox("No Craftable Items Found.", MessageType.Warning);
                        selectedCraftList.ItemsNeededIDs = new List<CraftListItem>();
                    }
                    else
                    {
                        RPGMakerGUI.PopupID<Item>("Item:", ref selectedCustomListInfo.ItemID,"ID","Name","Craft");
                    }
                }

                if (selectedCraftListItem != null)
                {
                    RPGMakerGUI.SubTitle("Selected Craft Material");

                    var allItems = Rm_RPGHandler.Instance.Repositories.CraftableItems.AllItems.Concat(Rm_RPGHandler.Instance.Repositories.Items.AllItems).ToList();
//                    var allItems = Rm_RPGHandler.Instance.Repositories.Items.AllItems;
                    RPGMakerGUI.PopupID<Item>("Item:", ref selectedCraftListItem.ItemID, "ID", "Name", "CraftAndItem");

                    if (allItems.Count > 0)
                    {
                        var stackable = allItems.FirstOrDefault(i => i.ID == selectedCraftListItem.ItemID) as IStackable;
                        if (stackable != null)
                        {
                            selectedCraftListItem.Quantity = RPGMakerGUI.IntField("Quantity:",
                                                                                  selectedCraftListItem.Quantity);
                        }
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Add or select a new field to customise Craft Lists.", MessageType.Info);
            }
            GUILayout.EndArea();
        }

        #endregion

        #region QuestItems

        private static Item selectedQuestItem;
        private static Vector2 questItemScrollPos = Vector2.zero;
        public static void QuestItems(Rect fullArea, Rect leftArea, Rect mainArea)
        {
            var listOfQuestItems = Rm_RPGHandler.Instance.Repositories.QuestItems.AllItems;
            GUI.Box(leftArea, "", "backgroundBox");
            GUI.Box(mainArea, "", "backgroundBoxMain");

            GUILayout.BeginArea(PadRect(leftArea, 0, 0));
            RPGMakerGUI.ListArea(listOfQuestItems, ref selectedQuestItem, Rm_ListAreaType.QuestItems, false, false);
            GUILayout.EndArea();


            GUILayout.BeginArea(mainArea);
            questItemScrollPos = GUILayout.BeginScrollView(questItemScrollPos);

            RPGMakerGUI.Title("Quest Items");
            if (selectedQuestItem != null)
            {
                ItemDetails(selectedQuestItem);
            }
            else
            {
                EditorGUILayout.HelpBox("Add or select a new field to customise quest items.", MessageType.Info);
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        #endregion

        #region LootTables

        private static Rm_LootTable selectedLootTable;
        private static int selectedLootTableItemIndex;
        private static bool showLootTableItems = true;
        private static Vector2 lootTableScrollPos = Vector2.zero;

        public static void LootTables(Rect fullArea, Rect leftArea, Rect mainArea)
        {

            GUI.Box(leftArea, "", "backgroundBox");
            GUI.Box(mainArea, "", "backgroundBox");

            var listOfQuestItems = Rm_RPGHandler.Instance.Repositories.LootTables.AllTables;

            GUILayout.BeginArea(PadRect(leftArea, 0, 0));
            RPGMakerGUI.ListArea(listOfQuestItems, ref selectedLootTable, Rm_ListAreaType.LootTables, false, false);
            GUILayout.EndArea();


            GUILayout.BeginArea(mainArea);
            lootTableScrollPos = GUILayout.BeginScrollView(lootTableScrollPos);
            RPGMakerGUI.Title("Loot Tables");
            if (selectedLootTable != null)
            {
                RPGMakerGUI.SubTitle("Main Info");
                selectedLootTable.Name = RPGMakerGUI.TextField("Name:", selectedLootTable.Name);
                selectedLootTable.ChanceForItem = EditorGUILayout.IntSlider("% Chance To Loot:",selectedLootTable.ChanceForItem,0,100);

                var lootTableResult = RPGMakerGUI.FoldoutToolBar(ref showLootTableItems, "Loot Table Items", new []{"+Item","+CraftItem","+QuestItem", "+Gold", "+Empty"});
                if(showLootTableItems)
                {
                    GUILayout.BeginHorizontal();
                    RPGMakerGUI.Label("Item", GUILayout.Width(150));
                    RPGMakerGUI.Label("Quantity (Min-Max)", GUILayout.Width(120));
                    RPGMakerGUI.Label("Chance");
                    GUILayout.EndHorizontal();
                    var allItems = Rm_RPGHandler.Instance.Repositories.Items.AllItems;
                    var allCraftItems = Rm_RPGHandler.Instance.Repositories.CraftableItems.AllItems;
                    var allQuestItems = Rm_RPGHandler.Instance.Repositories.QuestItems.AllItems;

                    if(allItems.Count == 0)
                    {
                        EditorGUILayout.HelpBox("No Items Found.",MessageType.Warning);
                        selectedLootTable.LootTableItems = new List<Rm_LootTableItem>();
                    }

                    for (int index = 0; index < selectedLootTable.LootTableItems.Count; index++)
                    {
                        var selectedLootTableItem = selectedLootTable.LootTableItems[index];

                        if (string.IsNullOrEmpty(selectedLootTableItem.ItemID))
                        {
                            selectedLootTableItemIndex = 0;
                        }
                        else
                        {
                            Item stillExists = null;
                            if (selectedLootTableItem.IsNormalItem)
                            {
                                stillExists = allItems.FirstOrDefault(a => a.ID == selectedLootTableItem.ItemID);
                                selectedLootTableItemIndex = stillExists != null ? allItems.IndexOf(stillExists) : 0;
                            }
                            else if (selectedLootTableItem.IsQuestItem)
                            {
                                stillExists = allQuestItems.FirstOrDefault(a => a.ID == selectedLootTableItem.ItemID);
                                selectedLootTableItemIndex = stillExists != null ? allQuestItems.IndexOf(stillExists) : 0;
                            }
                            else if (selectedLootTableItem.IsCraftableItem)
                            {
                                stillExists = allCraftItems.FirstOrDefault(a => a.ID == selectedLootTableItem.ItemID);
                                selectedLootTableItemIndex = stillExists != null ? allCraftItems.IndexOf(stillExists) : 0;
                            }
                        }
                        GUILayout.BeginHorizontal("backgroundBox");

                        if(selectedLootTableItem.IsGold)
                        {
                            RPGMakerGUI.Label("Gold Amount","lootTableItemText", GUILayout.Width(150));
                            selectedLootTableItem.MinQuantity = EditorGUILayout.IntField(selectedLootTableItem.
                                                                                         MinQuantity,
                                                                                     GUILayout.Width(60));
                            selectedLootTableItem.MaxQuantity = EditorGUILayout.IntField(selectedLootTableItem.
                                                                                         MaxQuantity,
                                                                                     GUILayout.Width(60));
                        }
                        else if(selectedLootTableItem.IsEmpty)
                        {
                            RPGMakerGUI.Label("Chance @ No Loot", "lootTableItemText", GUILayout.Width(150));
                            GUILayout.Space(125);
                        }
                        else
                        {
                            var itemListToUse = allItems;
                            if (selectedLootTableItem.IsNormalItem)
                            {
                                itemListToUse = allItems;
                            }
                            else if (selectedLootTableItem.IsQuestItem)
                            {
                                itemListToUse = allQuestItems;
                            }
                            else if (selectedLootTableItem.IsCraftableItem)
                            {
                                itemListToUse = allCraftItems;
                            }

                            if (itemListToUse.Count > 0)
                            {
                                selectedLootTableItemIndex = EditorGUILayout.Popup(selectedLootTableItemIndex,
                                                                            itemListToUse.Select((q, indexOf) => indexOf + ". " + q.Name).
                                                                                ToArray(), GUILayout.Width(150));
                                selectedLootTableItem.ItemID = itemListToUse[selectedLootTableItemIndex].ID;

                                //todo: add to 100 : selectedLootTableItem.ChanceForItem
                                var foundItem = itemListToUse[selectedLootTableItemIndex];

                                var stackable = foundItem as IStackable;
                                if (stackable != null)
                                {
                                    selectedLootTableItem.MinQuantity = EditorGUILayout.IntField(selectedLootTableItem.
                                                                                                 MinQuantity,
                                                                                             GUILayout.MaxWidth(60));
                                    selectedLootTableItem.MaxQuantity = EditorGUILayout.IntField(selectedLootTableItem.
                                                                                                 MaxQuantity,
                                                                                             GUILayout.MaxWidth(60));
                                }
                                else
                                {
                                    GUILayout.Space(125);
                                } 
                            }
                            else
                            {
                                GUILayout.Label("[No Items of this Type!]");
                            }
                        }


                        if (index == 0)
                        {
                            selectedLootTableItem.sliderMin = 0;

                            if (selectedLootTable.LootTableItems.Count == 1) selectedLootTableItem.sliderMax = 100;

                            RPGMakerGUI.MinMaxSlider(ref selectedLootTableItem.sliderMin,
                                                         ref selectedLootTableItem.sliderMax, 0, 100);

                            selectedLootTableItem.sliderMin = (int)selectedLootTableItem.sliderMin;
                            selectedLootTableItem.sliderMax = (int)selectedLootTableItem.sliderMax;

                            selectedLootTableItem.Chance = selectedLootTableItem.sliderMax -
                                                           selectedLootTableItem.sliderMin;
                            GUILayout.Label(selectedLootTableItem.Chance + "%", GUILayout.Width(50));
                        }
                        else if (index - 1 >= 0)
                        {
                            var prevTable = selectedLootTable.LootTableItems[index - 1];
                            selectedLootTableItem.sliderMin = prevTable.sliderMax;
                            if (selectedLootTableItem.sliderMax < selectedLootTableItem.sliderMin)
                            {
                                selectedLootTableItem.sliderMax = selectedLootTableItem.sliderMin;
                            }
                            if (index == selectedLootTable.LootTableItems.Count - 1)
                            {
                                selectedLootTableItem.sliderMax = 100;
                            }
                            RPGMakerGUI.MinMaxSlider(ref selectedLootTableItem.sliderMin,
                                                         ref selectedLootTableItem.sliderMax, 0, 100);

                            selectedLootTableItem.sliderMin = (int)selectedLootTableItem.sliderMin;
                            selectedLootTableItem.sliderMax = (int)selectedLootTableItem.sliderMax;

                            selectedLootTableItem.Chance = selectedLootTableItem.sliderMax -
                                                           selectedLootTableItem.sliderMin;
                            GUILayout.Label(selectedLootTableItem.Chance + "%", GUILayout.Width(50));
                        }

                        if(RPGMakerGUI.DeleteButton(25))
                        {
                            selectedLootTable.LootTableItems.RemoveAt(index);
                            index--;
                        }

                        GUILayout.EndHorizontal();

                        GUILayout.Space(10);
                    }

                    GUILayout.BeginHorizontal();
                    RPGMakerGUI.Label("Item", GUILayout.Width(150));
                    RPGMakerGUI.Label("Quantity (Min-Max)", GUILayout.Width(120));
                    RPGMakerGUI.Label("Chance");
                    GUILayout.EndHorizontal();

                    if (lootTableResult == 0 && allItems.Count > 0)
                    {
                        selectedLootTable.LootTableItems.Add(new Rm_LootTableItem(){IsNormalItem = true});
                        GUI.FocusControl("");
                    }
                    else if(lootTableResult == 1)
                    {
                        selectedLootTable.LootTableItems.Add(new Rm_LootTableItem() {IsCraftableItem = true});
                        GUI.FocusControl("");

                    }
                    else if(lootTableResult == 2)
                    {
                        selectedLootTable.LootTableItems.Add(new Rm_LootTableItem() {IsQuestItem = true});
                        GUI.FocusControl("");

                    }
                    else if(lootTableResult == 3)
                    {
                        selectedLootTable.LootTableItems.Add(new Rm_LootTableItem() {IsGold = true});
                        GUI.FocusControl("");

                    }
                    else if(lootTableResult == 4)
                    {
                        selectedLootTable.LootTableItems.Add(new Rm_LootTableItem() {IsEmpty = true});
                        GUI.FocusControl("");
                    }
                    RPGMakerGUI.EndFoldout();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Add or select a new field to customise loot tables.", MessageType.Info);
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        #endregion

        #region Dismantling

        private static DismantleDefintion selectedDismantleDefinition;
        private static DismantleItem selectedDismantleItem;
        private static int selectedDismantleItemIndex;

        public static void Dismantling(Rect fullArea, Rect leftArea, Rect mainArea)
        {

            
            leftAreaB = new Rect(leftArea.xMax + 5, leftArea.y, leftArea.width, leftArea.height);
            mainAreaAlt = new Rect(leftAreaB.xMax + 5, leftArea.y, mainArea.width - (leftAreaB.width + 5),
                                    leftArea.height);

            GUI.Box(leftArea, "", "backgroundBox");
            GUI.Box(leftAreaB, "", "backgroundBox");
            GUI.Box(mainAreaAlt, "", "backgroundBox");

            var list = Rm_RPGHandler.Instance.Repositories.Dismantle.TierToDismantleItems;
            #region CheckForUpdates
            foreach (var d in TierHandler.Tiers)
            {
                var tier = list.FirstOrDefault(t => t.TierID == d.TierID);
                if (tier == null)
                {
                    var tierToAdd = new DismantleDefintion(){ TierID = d.TierID };
                    list.Add(tierToAdd);
                }
            }

            for (int index = 0; index < list.Count; index++)
            {
                var v = list[index];
                var stillExists =
                    TierHandler.Tiers.FirstOrDefault(t => t.TierID == v.TierID);

                if (stillExists == null)
                {
                    list.Remove(v);
                    index--;
                }
            }

            #endregion

            GUILayout.BeginArea(PadRect(leftArea, 0, 0));
            if (selectedDismantleDefinition == null)
            {
                GUILayout.BeginHorizontal("toolBarBackground", GUILayout.MaxHeight(30));
                GUILayout.Label("Definitions", "subTitleNoBg");
                GUILayout.EndHorizontal();
            }
            RPGMakerGUI.ListArea(list, ref selectedDismantleDefinition, Rm_ListAreaType.DismantleDefinition, false,
                                 false, Rme_ListButtonsToShow.HelpOnly);
            GUILayout.EndArea();


            GUILayout.BeginArea(leftAreaB);
            if (selectedDismantleDefinition != null)
            {
                RPGMakerGUI.ListArea(selectedDismantleDefinition.DismantleItems, ref selectedDismantleItem,
                                     Rm_ListAreaType.DismantleItem, false, false, Rme_ListButtonsToShow.AllExceptHelp,
                                     true);

            }
            GUILayout.EndArea();

            GUILayout.BeginArea(mainAreaAlt);
            RPGMakerGUI.Title("Dismantle Definitions");
            if (selectedDismantleDefinition != null)
            {
                RPGMakerGUI.SubTitle("For Tier: " + selectedDismantleDefinition);
                var allItems = Rm_RPGHandler.Instance.Repositories.Items.AllItems;
                if(allItems.Count == 0)
                {
                    EditorGUILayout.HelpBox("No Items Found.", MessageType.Warning);
                    selectedDismantleDefinition.DismantleItems = new List<DismantleItem>();
                }
                if (selectedDismantleItem != null)
                {
                    RPGMakerGUI.SubTitle("Selected Dismatle Item");
                    if(allItems.Count > 0)
                    {
                        if (string.IsNullOrEmpty(selectedDismantleItem.ItemID))
                        {
                            selectedDismantleItemIndex = 0;
                        }
                        else
                        {
                            var stillExists =
                                allItems.FirstOrDefault(a => a.ID == selectedDismantleItem.ItemID);
                            selectedDismantleItemIndex = stillExists != null ? allItems.IndexOf(stillExists) : 0;
                        }
                        selectedDismantleItemIndex = EditorGUILayout.Popup("Item:", selectedDismantleItemIndex,
                                                                        allItems.Select((q,indexOf) => indexOf + ". " + q.Name).
                                                                            ToArray());

                    
                        selectedDismantleItem.ItemID = allItems[selectedDismantleItemIndex].ID;

                        var foundItem = allItems[selectedDismantleItemIndex];

                        var stackable = foundItem as IStackable;
                        if (stackable != null)
                        {
                            selectedDismantleItem.MinQuantity = RPGMakerGUI.IntField("Min Quantity:",
                                                                                     selectedDismantleItem.MinQuantity);
                            selectedDismantleItem.MaxQuantity = RPGMakerGUI.IntField("Max Quantity:",
                                                                                     selectedDismantleItem.MaxQuantity);
                        }
                        else
                        {
                            selectedDismantleItem.MinQuantity = selectedDismantleItem.MaxQuantity = 1;
                        }
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Add or select a new field to customise dismantle items.", MessageType.Info);
            }
            GUILayout.EndArea();
        }

        #endregion

        public static Rect PadRect(Rect rect, int left, int top)
        {
            return new Rect(rect.x + left, rect.y + top, rect.width - (left * 2), rect.height - (top * 2));
        }
    }
}
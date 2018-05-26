using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Testing;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Beta;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Beta
{
    public class LootSpawner : MonoBehaviour
    {
        
        public static LootSpawner Instance;

        private const string WorldLootItemPath = "RPGMakerAssets/WorldLootItem";

        void Awake()
        {
            Instance = this;
        }

        public void SpawnLoot(BaseCharacterMono character)
        {
            var positionToSpawn = character.transform.position + (character.transform.up);

            var itemsToLoot = new List<Item>();
            var goldToLoot = 0;

            //todo: nicer code this, maybe move to function
            var mm = GetObject.PlayerSave.QuestLog.ActiveObjectives;
            //note: we are double checking isDone here as we don't want to spawn loot for already completed itemConditions
            var active = mm.SelectMany(y => y.ActiveConditions).Where(i => !i.IsDone).ToList();
            var conditions = active.Select(a => a as ItemCondition);
            var interactConditions = conditions.Where(c => c != null);
            var foundCondition = interactConditions.FirstOrDefault(i => i.CombatantIDThatDropsItem == (character.Character as CombatCharacter).ID);
            if (foundCondition != null)
            {
                var qItem = Rm_RPGHandler.Instance.Repositories.QuestItems.Get(foundCondition.ItemToCollectID);
                if(qItem != null) itemsToLoot.Add(qItem);
            }

            var enemyInfo = character.Character as CombatCharacter;
            foreach (var loot in enemyInfo.GuaranteedLoot)
            {
                var item = Rm_RPGHandler.Instance.Repositories.Items.Get(loot.ItemID);
                if (item == null)
                {
                    Debug.LogError("Did not find item in guaranteed loot of : " + enemyInfo.Name);
                    continue;
                }

                if (loot.Amount != 0)
                {
                    var stackable = item as IStackable;
                    
                    if(stackable != null)
                        stackable.CurrentStacks = loot.Amount;
                }

                itemsToLoot.Add(item);
                
            }

            var randomGold = Random.Range(0, 100 + 1);
            if(enemyInfo.DropsGold)
            {
                var drops = randomGold <= (enemyInfo.GoldDropChance*100);
                if(drops)
                {
                    goldToLoot += Random.Range(enemyInfo.MinGoldDrop, enemyInfo.MaxGoldDrop + 1);
                }
            }

            if(enemyInfo.LootTables.Count > 0)
            {
                var random = Random.Range(0, 100 + 1);
                //Debug.Log("LootOption Random: " + random);
                var currentProbability = random;
                var prevChance = 0;
                var currentIndex = 0;
                LootOptions lootOptionsToUse = null;

                while (prevChance < 100)
                {
                    var currentTableChance = enemyInfo.LootTables[currentIndex].Chance + prevChance;
                    prevChance = (int)currentTableChance;


                    //Debug.Log("currentTableChance:" + currentTableChance);
                    if(currentProbability <= currentTableChance)
                    {
                        lootOptionsToUse = enemyInfo.LootTables[currentIndex];
                        break;
                    }
                    else
                    {
                        currentIndex++;
                    }
                }

                if(lootOptionsToUse != null)
                {
                    var lootTableToUse = Rm_RPGHandler.Instance.Repositories.LootTables.AllTables.FirstOrDefault(l => l.ID == enemyInfo.LootTables[currentIndex].LootTableID);
                    for (int x = 0; x < enemyInfo.MaxItemsFromLootTable; x++)
                    {
                        var chanceToGet = lootOptionsToUse.AlwaysGetItem ? 100 : lootTableToUse.ChanceForItem;
                        var randomRoll = Random.Range(0, 100 + 1);
                        //Debug.Log("LootTableItem Random: " + random);
                        if(randomRoll <= chanceToGet)
                        {
                            randomRoll = Random.Range(0, 100 + 1);
                            var currentProb = randomRoll;
                            var currentItemIndex = 0;
                            var prevLootChance = 0;
                            Rm_LootTableItem lootTableItemToLoot = null;
                            while (prevLootChance < 100)
                            {
                                var currentItemChance = lootTableToUse.LootTableItems[currentItemIndex].Chance + prevLootChance;
                                prevLootChance = (int)currentItemChance;

                                //Debug.Log("currentItemChance:" + currentItemChance);
                                if (currentProb <= currentItemChance)
                                {
                                    lootTableItemToLoot = lootTableToUse.LootTableItems[currentItemIndex];
                                    break;
                                }
                                else
                                {
                                    currentItemIndex++;
                                }
                            }
                            if(lootTableItemToLoot != null)
                            {
                                if(!lootTableItemToLoot.IsGold && !lootTableItemToLoot.IsEmpty)
                                {
                                    Item item = null;

                                    if (lootTableItemToLoot.IsNormalItem)
                                    {
                                        item = Rm_RPGHandler.Instance.Repositories.Items.Get(lootTableItemToLoot.ItemID);
                                    }
                                    else if (lootTableItemToLoot.IsQuestItem)
                                    {
                                        item = Rm_RPGHandler.Instance.Repositories.QuestItems.Get(lootTableItemToLoot.ItemID);
                                    }
                                    else if (lootTableItemToLoot.IsCraftableItem)
                                    {
                                        item = Rm_RPGHandler.Instance.Repositories.CraftableItems.Get(lootTableItemToLoot.ItemID);
                                    }


                                    //item = Rm_RPGHandler.Instance.Repositories.Items.AllItems.FirstOrDefault(i => i.ID == lootTableItemToLoot.ItemID);
                                    if (item == null)
                                    {
                                        Debug.LogError("Did not find item in guaranteed loot of : " + enemyInfo.Name);
                                        continue;
                                    }

                                    var itemCopy = GeneralMethods.CopyObject(item);

                                    if (lootTableItemToLoot.MaxQuantity != 1)
                                    {
                                        var stackable = itemCopy as IStackable;
                                        stackable.CurrentStacks = Random.Range(lootTableItemToLoot.MinQuantity, lootTableItemToLoot.MaxQuantity + 1);
                                    }

                                    itemsToLoot.Add(itemCopy);
                                }
                                else if(lootTableItemToLoot.IsGold)
                                {
                                    goldToLoot += Random.Range(lootTableItemToLoot.MinQuantity, lootTableItemToLoot.MaxQuantity + 1);
                                }
                            }
                        }
                    }
                }

            }

            StartCoroutine(SpawnListOfItems(itemsToLoot, positionToSpawn, goldToLoot));

        }

        IEnumerator SpawnListOfItems(List<Item> itemsToLoot, Vector3 positionToSpawn, int goldToLoot )
        {
            if (goldToLoot > 0)
            {
                var spawnPos = positionToSpawn + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0.2f, 0.3f), Random.Range(-0.5f, 0.5f));
                var lootItem = Instantiate(Resources.Load("Prefabs/GroundLootItems/LootItem"), spawnPos, Quaternion.identity) as GameObject;
                var lootMono = lootItem.GetComponent<LootItemMono>();
                lootMono.LootItem = new LootItem
                {
                    Gold = goldToLoot
                };
                lootMono.Init();

                AudioPlayer.Instance.Play(Rm_RPGHandler.Instance.Items.LootSound.Audio, AudioType.SoundFX, positionToSpawn);
                yield return new WaitForSeconds(0.1f);
            }

            for (int i = 0; i < itemsToLoot.Count; i++)
            {
                var spawnPos = positionToSpawn + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0.2f, 0.3f), Random.Range(-0.5f, 0.5f));
                var spawnPrefab = Resources.Load(GetSpawnPrefab(itemsToLoot[i])) as GameObject;
                var lootItem = Instantiate(spawnPrefab, spawnPos, Quaternion.identity) as GameObject;
                var lootMono = lootItem.GetComponent<LootItemMono>();
                lootMono.LootItem = new LootItem
                {
                    Item = itemsToLoot[i]
                };
                lootMono.Init();
                AudioPlayer.Instance.Play(Rm_RPGHandler.Instance.Items.LootSound.Audio, AudioType.SoundFX, positionToSpawn);
                yield return new WaitForSeconds(0.1f);
            }

            yield return null;
        }

        public void SpawnItem(Vector3 position, Item itemToDrop)
        {
            var spawnPos = position + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(0.2f, 0.3f), Random.Range(-0.5f, 0.5f));
            var spawnPrefab = Resources.Load(GetSpawnPrefab(itemToDrop)) as GameObject;
            var lootItem = Instantiate(spawnPrefab, spawnPos, Quaternion.identity) as GameObject;
            var lootMono = lootItem.GetComponent<LootItemMono>();
            lootMono.LootItem = new LootItem
            {
                Item = itemToDrop
            };
            lootMono.Init();
            AudioPlayer.Instance.Play(Rm_RPGHandler.Instance.Items.LootSound.Audio, AudioType.SoundFX,position);
        }

        /// <summary>
        /// Spawns an item at a position.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="type">0 = item, 1 = craftable, 2 = quest</param>
        /// <param name="itemToDrop"></param>
        public static void SpawnWorldLootItem(Vector3 position, int type, Item itemToDrop)
        {
            var spawnPos = position;
            var spawnPrefab = Resources.Load(WorldLootItemPath) as GameObject;
            var lootItem = Instantiate(spawnPrefab, spawnPos, Quaternion.identity) as GameObject;
            lootItem.name = itemToDrop.Name + "_WorldLootable"; 

            var lootMono = lootItem.GetComponent<WorldLootItemMono>();

            var quantity = 1;
            var stackable = itemToDrop as IStackable;
            if(stackable != null)
            {
                quantity = stackable.CurrentStacks;
            }
            lootMono.LootItem = new WorldLootItem() 
            {
                Type = type,
                ItemId = itemToDrop.ID,
                Quantity = quantity
            };
            lootMono.Init();
        }

        private static string GetSpawnPrefab(Item itemToDrop)
        {
            if (!string.IsNullOrEmpty(itemToDrop.CustomGroundPrefabPath))
            {
                return itemToDrop.CustomGroundPrefabPath;
            }

            if(itemToDrop.ItemType == ItemType.Weapon)
            {
                var wep = itemToDrop as Weapon;
                var wepType = Rm_RPGHandler.Instance.Items.WeaponTypes.FirstOrDefault(w => w.ID == wep.WeaponTypeID);
                if(wepType != null)
                {
                    if(!string.IsNullOrEmpty(wepType.PrefabPath))
                    {
                        return wepType.PrefabPath;
                    }
                }
            }

            if(itemToDrop.ItemType == ItemType.Apparel)
            {
                var app = itemToDrop as Apparel;
                var appType = Rm_RPGHandler.Instance.Items.ApparelSlots.FirstOrDefault(a => a.ID == app.apparelSlotID);
                if (appType != null)
                {
                    if (!string.IsNullOrEmpty(appType.PrefabPath))
                    {
                        return appType.PrefabPath;
                    }
                }
            }

            var itemTypePrefab = Rm_RPGHandler.Instance.Items.GroundPrefabs.FirstOrDefault(i => i.ItemType == itemToDrop.ItemType);
            if(itemTypePrefab != null && !string.IsNullOrEmpty(itemTypePrefab.PrefabPath))
            {
                return itemTypePrefab.PrefabPath;
            }

            return Rm_RPGHandler.Instance.Items.GroundPrefabs.First(g => g.PrefabType == GroundPrefabType.Default).PrefabPath;
        }
    }
}
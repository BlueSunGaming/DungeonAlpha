using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Core.Interaction;
using Assets.Scripts.Testing;
using LogicSpawn.RPGMaker.Beta;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class SpawnSkillHandler : MonoBehaviour
    {
        public static Dictionary<string, int> SpawnedObjectInstanceTracker;
        private Skill _spawnSkill;
        private BaseCharacterMono _charTarget;
        private bool _initialised = true;

        void Awake()
        {
            if(SpawnedObjectInstanceTracker == null)
            {
                SpawnedObjectInstanceTracker = new Dictionary<string, int>();
                Debug.Log("init spawn instance dictionary.");
            }
        }

        public void Init(Skill spawnSkill, BaseCharacterMono target)
        {
            if (spawnSkill.LimitSpawnInstances)
            {
                if(SpawnedObjectInstanceTracker.ContainsKey(spawnSkill.ID))
                {
                    var currentInstances = SpawnedObjectInstanceTracker[spawnSkill.ID];
                    if(currentInstances < spawnSkill.MaxInstances)
                    {
                        SpawnedObjectInstanceTracker[spawnSkill.ID] += 1;
                    }
                    else
                    {
                        Debug.Log("Max instances of this spawn reached.");
                        Destroy(gameObject);
                        return;
                    }
                }
                else
                {
                    SpawnedObjectInstanceTracker.Add(spawnSkill.ID , 1);
                }
            }

            _spawnSkill = GeneralMethods.CopySkill(spawnSkill);
            _charTarget = target;
            transform.position = transform.position + new Vector3(0, transform.localScale.y / 2, 0);

            
            if(spawnSkill.HasDuration)
            {
                Destroy(gameObject, spawnSkill.SpawnForTime);    
            }
            
            

            //LoadAllData();

            _initialised = true;
            Debug.Log("Initialised skill");
        }

        void Update()
        {
            if(transform.childCount == 0)
            {
                Destroy(gameObject);
            }
        }

        void OnDestroy()
        {
            if(_spawnSkill != null)
            {
                if (SpawnedObjectInstanceTracker.ContainsKey(_spawnSkill.ID))
                {
                    SpawnedObjectInstanceTracker[_spawnSkill.ID] -= 1;
                }
            }
        }

        private void LoadAllData()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                LoadData(child);
            }
        }

        private void LoadData(Transform child)
        {
            var enemyMono = child.GetComponent<EnemyCharacterMono>();
            var npcMono = child.GetComponent<NpcCharacterMono>();
            var harvestable = child.GetComponent<InteractableHarvestable>();
            var interactable = child.GetComponent<InteractiveObjectMono>();

            if (enemyMono != null)
            {
                if (!string.IsNullOrEmpty(enemyMono.EnemyID))
                {
                    var enemyChar = Rm_RPGHandler.Instance.Repositories.Enemies.AllEnemies.FirstOrDefault(i => i.ID == enemyMono.EnemyID);
                    if (enemyChar != null)
                    {
                        enemyMono.SetEnemy(enemyChar);
                    }
                    else
                    {
                        Debug.LogError("Could not find Enemy data for Spawned Enemy: " + enemyMono.EnemyID + ". Destroying.");
                        Destroy(gameObject);
                    }
                }
            }

            if (npcMono != null)
            {
                if (!string.IsNullOrEmpty(npcMono.NpcID))
                {
                    var getNpc = Rm_RPGHandler.Instance.Repositories.Interactable.AllNpcs.FirstOrDefault(i => i.ID == npcMono.NpcID);
                    if (getNpc != null)
                    {
                        npcMono.SetNPC(getNpc);
                    }
                    else
                    {
                        Debug.LogError("Could not find NPC data for Spawned NPC: " + npcMono.NpcID + ". Destroying.");
                        Destroy(gameObject);
                    }
                }
            }

            if (harvestable != null)
            {
                if (!string.IsNullOrEmpty(harvestable.ObjectID))
                {
                    var harvestableObj = Rm_RPGHandler.Instance.Harvesting.HarvestableDefinitions.FirstOrDefault(i => i.ID == harvestable.ObjectID);
                    if (harvestableObj != null)
                    {
                        harvestable.SetHarvestable(harvestableObj);
                    }
                    else
                    {
                        Debug.LogError("Could not find Harvestable data for Spawned Harvestable: " + harvestable.ObjectID + ". Destroying.");
                        Destroy(gameObject);
                    }
                }
            }

            if (interactable != null)
            {
                if (!string.IsNullOrEmpty(interactable.ObjectID))
                {
                    var interactableObj = Rm_RPGHandler.Instance.Repositories.Interactable.AllInteractables.FirstOrDefault(i => i.ID == interactable.ObjectID);
                    if (interactableObj != null)
                    {
                        interactable.SetInteractable(interactableObj);
                    }
                    else
                    {
                        Debug.LogError("Could not find Interactable data for Spawned Interactable: " + interactable.ObjectID + ". Destroying.");
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}
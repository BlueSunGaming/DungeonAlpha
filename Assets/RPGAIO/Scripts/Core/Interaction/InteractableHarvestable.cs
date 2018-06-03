using System.Collections;
using System.Linq;
using Assets.Scripts.Beta.NewImplementation;
using Assets.Scripts.Testing;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace Assets.Scripts.Core.Interaction
{
    public class InteractableHarvestable : InteractiveObjectMono
    {
        private bool Harvesting;
        public Harvestable Harvestable;
        public float RegenCounter;

        public int MaterialsRemaining
        {
            get { return Harvestable.MaterialsRemaining; }
            set { Harvestable.MaterialsRemaining = value; }
        }        
        
        public Item HarvestedItem
        {
            get
            {
                if(Harvestable.IsQuestItem)
                {
                    return Rm_RPGHandler.Instance.Repositories.QuestItems.Get(Harvestable.HarvestedObjectID);
                }
                
                return Rm_RPGHandler.Instance.Repositories.Items.Get(Harvestable.HarvestedObjectID);
            }
        }

        public float HarvestTime
        {
            get { return Harvestable.TimeToHarvest; }
        }

        public override void Update()
        {
            if (!Initialised) return;

            if(Harvestable == null)
            {
                Debug.LogError("Destroying Harvestable GameObject, Reason: Game data not found for : " + ObjectID);
                Destroy(gameObject);
                return;
            }

            base.Update();

            if (Harvestable.RegensResources && MaterialsRemaining < Harvestable.MaxAtOnce)
            {
                if(RegenCounter >= Harvestable.TimeInSecToRegen)
                {
                    if(MaterialsRemaining < Harvestable.MaxAtOnce)
                    {
                        Harvestable.MaterialsRemaining += Harvestable.AmountRegenerated;
                        Harvestable.MaterialsRemaining = Mathf.Min(Harvestable.MaterialsRemaining, Harvestable.MaxAtOnce);
                        RegenCounter = 0;
                    }
                }
                else
                {
                    RegenCounter += Time.deltaTime;
                }
            }
            
        }
        
        void Awake()
        {
            Type = InteractableType.Harvest;
        }

        protected override void OnEnable()
        {
            if (Initialised) return;

            if (!string.IsNullOrEmpty(ObjectID))
            {
                var harvestableObj = Rm_RPGHandler.Instance.Harvesting.HarvestableDefinitions.FirstOrDefault(i => i.ID == ObjectID);
                if (harvestableObj != null)
                {
                    SetHarvestable(harvestableObj);
                }
                else
                {
                    Debug.LogError("Could not find Harvestable data for Spawned Harvestable: " + ObjectID + ". Destroying.");
                    Destroy(gameObject);
                }
            }
            else
            {
                Debug.LogError("Could not find Harvestable data for Spawned Harvestable: " + ObjectID + ". Destroying.");
                Destroy(gameObject);
            }

            Initialised = true;
        }

        public override void Interaction()
        {
            if (!Harvesting && RequirementsMet() && Harvestable.MaterialsRemaining > 0)
            {
                RPG.Events.OnStartHarvesting(new RPGEvents.StartHarvestingEventArgs(){Harvestable = this});
                Harvesting = true;
                StartCoroutine("Harvest");
                CurrentInteraction = this;
            }
        }

        private bool RequirementsMet()
        {
            if (Harvestable.IsQuestItem)
            {
                var quest = GetObject.PlayerSave.QuestLog.ActiveObjectives.FirstOrDefault(q => q.ID == Harvestable.QuestAcceptedID);
                if (quest == null)
                {
                    Debug.Log("[RPGAIO] Required quest is not active to harvest a quest item from this.");
                    return false;
                }                
            }

            if(Harvestable.RequireLevel)
            {
                if (GetObject.PlayerCharacter.Level < Harvestable.LevelRequired)
                    return false;
            }

            if(Harvestable.RequireTraitLevel)
            {
                var trait = GetObject.PlayerCharacter.GetTraitByID(Harvestable.RequiredTraitID);
                if (trait != null)
                {
                    if(trait.Level < Harvestable.TraitLevelRequired)
                        return false;
                }
                else
                {
                    Debug.LogError("[RPGAIO] Could not find required trait for harvesting. Harvest ID: " + ObjectID + " Trait ID:" + Harvestable.RequiredTraitID);
                }
            }

            return true;
        }

        public override void OnGUI()
        {
            //if (!Initialised) return;
            //
            //base.OnGUI();
            //if (!Harvesting) return;
            //
            //if(GUI.Button(new Rect(Screen.width/2 - 125, Screen.height/2 - 25, 250, 50),"Stop Harvesting (" + MaterialsRemaining + ")" ))
            //{
            //    StopHarvesting();
            //}
        }

        private void StopHarvesting()
        {
            StopInteraction();
        }

        public override void StopInteraction()
        {
            RPG.Events.OnStopHarvesting(new RPGEvents.StopHarvestingEventArgs());
            Harvesting = false;
            GetObject.PlayerController.ForceStopHandlingActions();
            StopCoroutine("Harvest");
            base.StopInteraction();

            if (GetObject.PlayerController.Character.AnimationType == RPGAnimationType.Mecanim)
            {
                GetObject.PlayerController.Animator.SetBool("isHarvesting",false);
            }
        }

        IEnumerator Harvest()
        {
            if (GetObject.PlayerController.Character.AnimationType == RPGAnimationType.Mecanim)
            {
                GetObject.PlayerController.Animator.SetBool("isHarvesting", true);
            }

            while(Harvesting)
            {

                if (MaterialsRemaining == 0)
                {
                    Harvesting = false;

                    if (!Harvestable.RegensResources)
                        Destroy(gameObject, 3.0f);
                }

                if(Harvesting)
                {
                    var playerController = GetObject.PlayerController;
                    var animDefinition = Harvestable.ClassHarvestingAnims.First(a => a.ClassID == GetObject.PlayerCharacter.PlayerCharacterID);
                    //playerController.RPGAnimation.CrossfadeAnimation(animName);
                    var queue = RPGActionQueue.Create();
                    if(playerController.Character.AnimationType == RPGAnimationType.Legacy)
                    {
                        var animName = animDefinition.LegacyAnim;
                        queue.Add(RPGActionFactory.PlayAnimation(animName));
                    }
                    else
                    {
                        var animName = animDefinition.LegacyAnim;
                        queue.Add(RPGActionFactory.PlayAnimation(new AnimationDefinition()
                                                                 {
                                                                     Name = "HarvestAnimation", 
                                                                     MecanimAnimationNumber = animDefinition.AnimNumber, 
                                                                     RPGAnimationSet = RPGAnimationSet.Harvesting
                                                                 }));
                    }
                    
                    queue.Add(RPGActionFactory.WaitForSeconds(HarvestTime));
                    playerController.BeginActionQueue(queue);

                    AudioPlayer.Instance.Play(Harvestable.HarvestSound, AudioType.SoundFX, transform.position);

                    Harvestable.CurrentHarvestTime = 0;
                    while(Harvestable.CurrentHarvestTime < Harvestable.TimeToHarvest)
                    {
                        Harvestable.CurrentHarvestTime += Time.deltaTime;
                        yield return null;
                    }

                    var harvestedItem = GeneralMethods.CopyObject(HarvestedItem);
                    var quantityGained = Random.Range(Harvestable.MinAmountGained, Harvestable.MaxAmountGained + 1);
                    quantityGained = Mathf.Min(quantityGained, MaterialsRemaining);

                    var stack = harvestedItem as IStackable;
                    if (stack != null)
                    {
                        stack.CurrentStacks = quantityGained;
                    }

                    if (Rm_RPGHandler.Instance.Interactables.AddHarvestItemsToInventory)
                    {
                        GetObject.PlayerCharacter.Inventory.AddItem(harvestedItem);
                    }
                    else
                    {
                        var itemspawner = GetObject.LootSpawner;
                        itemspawner.SpawnItem(transform.position + transform.forward * 1.8f, harvestedItem);
                    }

                    MaterialsRemaining -= quantityGained;
                    GetObject.PlayerCharacter.AddProgression(Harvestable.ProgressionGain);

                }
                
                yield return null;

                if(MaterialsRemaining < 1)
                {
                    if (!Harvestable.RegensResources)
                        Destroy(gameObject, 3.0f);

                    break;
                }
            }

            StopHarvesting();
        }

        public override void CheckInteraction()
        {
            base.CheckInteraction();

            if (MaterialsRemaining < 1 || !RequirementsMet())
            {
                StopInteraction();
            }
        }

        public void SetHarvestable(Harvestable harvestableObj)
        {
            Harvestable = GeneralMethods.CopyObject(harvestableObj);

            Harvestable.MaterialsRemaining = Harvestable.RegensResources 
                ? Harvestable.MaxAtOnce 
                : Random.Range(Harvestable.MinObtainable, Harvestable.MaxObtainable + 1);
        }
    }
}
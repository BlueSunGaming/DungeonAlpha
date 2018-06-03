using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Beta.NewImplementation;
using Assets.Scripts.Testing;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Beta;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class NpcCharacterMono : BaseCharacterMono
    { 
        public string NpcID = "";
        public NonPlayerCharacter NPC;
        public List<Quest> myAvailableQuests;
        public List<Quest> myInProgressQuests;
        public List<Quest> myCompletableQuests;
        public NpcQuestStatusModel QuestStatusModel;

        public NpcStatus GetStatus
        {
            get
            {
                if(myCompletableQuests.Any())
                {
                    return NpcStatus.CompletableQuest;
                }

                if(myAvailableQuests.Any())
                {
                    return NpcStatus.AvailableQuest;
                }

                if(myInProgressQuests.Any())
                {
                    return NpcStatus.InProgressQuest;
                }

                return NpcStatus.None;
            }
        }

        private void EventsOnQuestStatusUpdate(object sender, EventArgs eventArgs)
        {
            UpdateQuestStatus();
        }

        protected override void DoStart()
        {
            UpdateQuestStatus();
            InvokeRepeating("UpdateQuestStatus", 0, 5);


        }

        private void UpdateQuestStatus()
        {
            if (!Initialised) return;
            if (QuestStatusModel == null) return;

            myAvailableQuests = new List<Quest>();
            myInProgressQuests = new List<Quest>();
            myCompletableQuests = new List<Quest>();

            var npcDefinition = Rm_RPGHandler.Instance.Repositories.Interactable.GetNPC(NpcID);
            if (npcDefinition == null) return;

            var dialogId = npcDefinition.Interaction.ConversationNodeId;
            if (!string.IsNullOrEmpty(dialogId))
            {
                var nodeTree = Rm_RPGHandler.Instance.Nodes.DialogNodeBank.NodeTrees.FirstOrDefault(n => n.ID == dialogId);
                if (nodeTree != null)
                {
                    var nodeChain = new NodeChain(nodeTree, typeof(DialogStartNode));
                    var myQuests = nodeChain.Nodes.Where(n => n is BeginQuestNode || n is CompleteQuestNode).ToList();
                    foreach (var q in myQuests)
                    {
                        var beginQuest = q as BeginQuestNode;
                        var endQuest = q as CompleteQuestNode;

                        if (beginQuest != null)
                        {
                            var quest = GetObject.PlayerSave.QuestLog.GetObjective((string)beginQuest.ValueOf("Quest"));
                            if (quest != null)
                            {
                                if (!quest.IsAccepted && !quest.TurnedIn)
                                {
                                    myAvailableQuests.Add(quest);
                                }
                                else if (quest.IsAccepted && !quest.ConditionsMet && !quest.TurnedIn)
                                {
                                    myInProgressQuests.Add(quest);
                                }
                            }
                        }

                        if (endQuest != null)
                        {
                            var quest = GetObject.PlayerSave.QuestLog.GetObjective((string)endQuest.ValueOf("Quest"));
                            if (quest != null)
                            {
                                if (quest.IsAccepted && quest.ConditionsMet && !quest.TurnedIn)
                                {
                                    myCompletableQuests.Add(quest);
                                }
                            }
                        }
                    }
                }
            }

            QuestStatusModel.SetStatus(GetStatus);
        }

        protected override void DoUpdate()
        {
            if (!Initialised) return;



            //Debug.Log("NPC Status:" + GetStatus);

            if (NPC == null)
            {
                Debug.LogError("Destroying NPC GameObject, Reason: Game data not found for : " + gameObject.name);
                Destroy(gameObject);
            }
        }

        protected void OnEnable()
        {
            if (Initialised) return;

            RPG.Events.QuestStatusUpdate += EventsOnQuestStatusUpdate;

            if (!string.IsNullOrEmpty(NpcID))
            {
                var getNpc = Rm_RPGHandler.Instance.Repositories.Interactable.AllNpcs.FirstOrDefault(i => i.ID == NpcID);
                if (getNpc != null)
                {
                    SetNPC(getNpc);
                }
                else
                {
                    Debug.LogError("Could not find NPC data for Spawned NPC: " + NpcID + ". Destroying.");
                    Destroy(gameObject);
                }
            }
            else
            {
                Debug.LogError("Could not find NPC data for Spawned NPC: " + NpcID + ". Destroying.");
                Destroy(gameObject);
            }
        }


        protected void OnDisable()
        {
            Initialised = false;
            RPG.Events.QuestStatusUpdate -= EventsOnQuestStatusUpdate;
        }

        public void SetNPC(NonPlayerCharacter player)
        {
            player = GeneralMethods.CopyObject(player);
            NPC = player;
            NPC.CharacterMono = this;
            Controller.SpawnPosition = transform.position;
            Controller.State = RPGControllerState.Idle;
            NPC.VitalHandler = new VitalHandler(NPC);
            NPC.VitalHandler.Health.CurrentValue = NPC.VitalHandler.Health.MaxValue;
            Character = player;
            NPC.CCInit();
            Controller.SpawnPosition = transform.position;
            GetComponent<InteractableNPC>().ObjectID = NPC.ID;
            RefreshPrefabs();
            Initialised = true;
        }
    }
}
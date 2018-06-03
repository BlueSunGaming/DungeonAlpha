using System.Linq;
using Assets.Scripts.Core.Interaction;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace Assets.Scripts.Testing
{
    public class InteractiveObjectMono : MonoBehaviour
    {
        protected bool Initialised;
        public static InteractiveObjectMono CurrentInteraction;
        public static InteractiveObjectMono CurrentTarget;
        public string ObjectID;
        public Interactable InteractiveObject;
        private bool ShowInteractHint;
        protected InteractableType Type;
        private bool Interacting
        {
            get { return CurrentInteraction == this; }
        }

        public virtual void Interaction()
        {
            DialogHandler.Instance.BeginDialog(ObjectID, this);
        }

        private float InteractDistance
        {
            get { return Rm_RPGHandler.Instance.Interactables.InteractDistance; }
        }

        private QuestCondition CheckForQuestInteraction()
        {
            if (Type == InteractableType.Harvest) return null;

            var x = GetObject.PlayerSave.QuestLog.ActiveObjectives;
            var active = x.SelectMany(y => y.ActiveConditions).Where(c => !c.IsDone).ToList();
            var iConditions = active.Select(a => a as InteractCondition).Where(c => c != null);;
            var dConditions = active.Select(a => a as DeliverCondition).Where(c => c != null);;

            QuestCondition foundCondition = iConditions.FirstOrDefault(i => i.InteractableID == ObjectID);
            if (foundCondition == null) foundCondition = dConditions.FirstOrDefault(i => i.InteractableToDeliverToID == ObjectID);

            return foundCondition;
        }

        public virtual void Update()
        {
            if (!Initialised) return;   

            if(this is InteractableNPC)
            {
                var npcInteraction = this as InteractableNPC;
                if (npcInteraction.NPC.Controller.InCombat || !npcInteraction.NPC.Character.Alive)
                {
                    if(CurrentInteraction == this)
                        StopInteraction();
                }
            }
            var tooFarAway = !(Vector3.Distance(transform.position, GetObject.PlayerMonoGameObject.transform.position) <= InteractDistance);
            if (tooFarAway && !GameMaster.CutsceneActive)
            {
                if(Interacting)
                {
                    StopInteraction();
                }

                ShowInteractHint = false;
                return;
            }

            CurrentTarget = this;
            ShowInteractHint = true;

            if (Rm_RPGHandler.Instance.DefaultSettings.EnableInteractWithKey && RPG.Input.GetKeyDown(RPG.Input.InteractKey))
            {
                if (CurrentTarget == this) CheckInteraction();
            }

        }

        protected virtual void OnEnable()
        {
            if (Initialised) return;

            if (!string.IsNullOrEmpty(ObjectID))
            {
                var interactableObj = Rm_RPGHandler.Instance.Repositories.Interactable.AllInteractables.FirstOrDefault(i => i.ID == ObjectID);
                if (interactableObj != null)
                {
                    SetInteractable(interactableObj);
                }
                else
                {
                    Debug.LogError("Could not find Interactable data for Spawned Interactable: " + ObjectID + ". Destroying.");
                    Destroy(gameObject);
                }
            }
            else
            {
                Debug.LogError("Could not find Interactable data for Spawned Interactable: " + ObjectID + ". Destroying.");
                Destroy(gameObject);
            }
            Initialised = true;
        }

        public virtual void OnGUI()
        {
            //if (!Initialised) return;
            //if (Rm_RPGHandler.Instance.DefaultSettings.EnableInteractWithKey && ShowInteractHint && DialogHandler.Instance.DialogNodeChain == null)
            //{
            //    GUI.Box(new Rect(Screen.width / 2 - 60 , Screen.height - 195, 120, 40), "Press F to interact");
            //}
            //
            //if (InteractiveObject != null)
            //{
            //    var screenPos = GetObject.RPGCamera.GetComponent<Camera>().WorldToScreenPoint(transform.position + transform.up);
            //
            //    if (transform.GetChild(0).GetComponent<Renderer>().isVisible)
            //        GUI.Box(new Rect(screenPos.x - 50, Screen.height - screenPos.y, 100, 25), InteractiveObject.Name);
            //}
        }

        public void OnMouseOver()
        {
            if (!Initialised) return;
            if(RPG.Input.GetKeyDown(RPG.Input.InteractMouse)){
                CheckInteraction();
            }
        }

        public void OnMouseDown()
        {
#if (UNITY_IOS || UNITY_ANDROID)
            CheckInteraction();
#endif
        }


        public virtual void CheckInteraction()
        {
            if(CurrentInteraction != null && CurrentInteraction != this)
            {
                CurrentInteraction.StopInteraction();
            }

            if (Vector3.Distance(transform.position, GetObject.PlayerMonoGameObject.transform.position) <= InteractDistance
                && DialogHandler.Instance.DialogNodeChain == null)
            {

                if (GetObject.PlayerController.InCombat)
                {
                    Debug.Log("Cannot interact while in combat.");
                    return;
                }

                CurrentInteraction = this;
                GetObject.PlayerController.Interacting = true;

                var questCondition = CheckForQuestInteraction();
                if (questCondition != null)
                {
                    DialogHandler.Instance.BeginDialog(questCondition);
                }
                else
                {
                    Interaction();
                }
            }
        }

        public virtual void StopInteraction()
        {
            DialogHandler.Instance.EndDialog();
            CurrentInteraction = null;
            GetObject.PlayerController.Interacting = false;
        }


        public void SetInteractable(Interactable interactableObj)
        {
            InteractiveObject = interactableObj;
        }

        public virtual string GetName()
        {
            return InteractiveObject.Name;
        }
        public virtual Texture2D GetImage()
        {
            return InteractiveObject.Image;
        }
    }
}
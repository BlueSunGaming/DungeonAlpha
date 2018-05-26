using System;
using System.Linq;
using System.Reflection;
using Assets.RPGAIO.Scripts.Core.Interaction;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class RPG_EventTrigger: MonoBehaviour
    {
        //todo: custom inspector links to EventNodeBank events
        public string EventID;
        public InteractType InteractType;
        public float Distance;
        public string GameObjectName;
        public bool AllowRetrigger = false;
        public bool RetriggerBasedOnDistance;
        public float MinDistanceBeforeRetrigger = 5;    
        public bool AllowOnlyIfNotInteracting = false;
        public bool TurnOffDraggableOnEvent = true;

        private Transform _myTransform;
        private bool triggerHandled = false;
        private GameObject _triggerObject;

        void OnEnable()
        {
            _myTransform = transform;
        }

        void OnMouseDown()
        {
            if (triggerHandled) return;

            if(InteractType == InteractType.Click && !triggerHandled)
            {
                PerformEvent();
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (triggerHandled) return;

            if (InteractType == InteractType.Collide)
            {
                if (other.CompareTag("Player"))
                {
                    PerformEvent();
                }
            }

            if(InteractType == InteractType.GameObject)
            {
                //Trim name check to avoid typos
                if(other.name.Trim() == GameObjectName.Trim())
                {
                    var success = PerformEvent();

                    if(TurnOffDraggableOnEvent)
                    {
                        _triggerObject = other.gameObject;
                        Invoke("TurnOffDrag", 1);
                    }
                }
            }
        }

        void TurnOffDrag()
        {
            var dragScript = _triggerObject.GetComponent<DraggableObject>();
            var rBody = _triggerObject.GetComponent<Rigidbody>();
            if (rBody != null)
            {
                rBody.isKinematic = true;
            }
            Destroy(dragScript);
        }

        void Update()
        {
            if(!triggerHandled && InteractType == InteractType.NearTo)
            {
                if(Vector3.Distance(_myTransform.position, GetObject.PlayerMono.transform.position) < Distance)
                {
                    PerformEvent();
                }
            }

            if(RetriggerBasedOnDistance)
            {
                if (Vector3.Distance(_myTransform.position, GetObject.PlayerMono.transform.position) > MinDistanceBeforeRetrigger)
                {
                    triggerHandled = false;
                }
            }
        }

        bool PerformEvent()
        {
            if(AllowOnlyIfNotInteracting && GetObject.PlayerController.Interacting)
            {
                return false;
            }

            if (!AllowRetrigger)
            {
                triggerHandled = true;
            }
            
            var eventRun = GetObject.EventHandler.RunEvent(EventID);

            return true;
        }
    }
}
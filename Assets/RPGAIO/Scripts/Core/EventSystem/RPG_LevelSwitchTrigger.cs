using System;
using System.Linq;
using System.Reflection;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class RPG_LevelSwitchTrigger : MonoBehaviour
    {
        //todo: custom inspector links to EventNodeBank events
        public string SceneName;
        public InteractType InteractType;
        public float Distance;
        private Transform _myTransform;
        private bool collisionHandled = false;

        void Awake()
        {
            _myTransform = transform;
        }

        void OnMouseDown()
        {
            if(InteractType == InteractType.Click)
            {
                PerformEvent();
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (collisionHandled) return;
            if(other.CompareTag("Player"))
            {
                PerformEvent();
                collisionHandled = true;
            }
        }

        void OnCollisionEnter(Collision other)
        {
            if (collisionHandled) return;

            if (other.transform.CompareTag("Player"))
            {
                PerformEvent();
                collisionHandled = true;
            }
        }

        void Update()
        {
            if(InteractType == InteractType.NearTo)
            {
                if(Vector3.Distance(_myTransform.position, GetObject.PlayerMono.transform.position) < Distance)
                {
                    PerformEvent();
                }
            }
        }

        void PerformEvent()
        {
            RPG.LoadLevel(SceneName, true, true);
        }
    }
}
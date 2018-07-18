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
        [SerializeField]
        public string SceneName;
        [SerializeField]
        private int BuildIndex;
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
            if (BuildIndex != 0)
            {
                RPG.LoadLevel(BuildIndex);
                UserStatistics.IncrementCurrentFloor();
            }
            else if (SceneName != "")
            {
                RPG.LoadLevel(SceneName, true, true);
                UserStatistics.IncrementCurrentFloor();
            }
            else
            {
                Debug.Log("Attempted to load via build index and Scene name but both were default values.");
                RPG.LoadLevel("Dungeon", true, true);
                UserStatistics.IncrementCurrentFloor();
            }
        }
    }
}
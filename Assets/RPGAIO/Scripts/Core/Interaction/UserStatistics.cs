using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Testing;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class UserStatistics : MonoBehaviour
    {
        public static UserStatistics Instance;
        public int maxFloorReached = 0;
        public int timePlayed = 0;
        public int stepsTaken = 0;
        public int numBooksRead = 0;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.Log("UserStatistics Instance was errantly created elsewhere");
            }
        }


        void Update()
        {

        }

     
    }
}
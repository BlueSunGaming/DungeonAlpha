using System;
using UnityEngine;

namespace LogicSpawn.RPGMaker
{
    public class Rm_PrefabIdentifier : MonoBehaviour
    {
        public string ID;
        public string SearchName;
        public Rmh_PrefabType PrefabType;

        public Rm_PrefabIdentifier()
        {
            ID = Guid.NewGuid().ToString();
            SearchName = "";
            PrefabType = Rmh_PrefabType.Misc;
        }
    }
}
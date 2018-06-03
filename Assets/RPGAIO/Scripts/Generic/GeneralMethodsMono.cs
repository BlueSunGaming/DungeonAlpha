using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Generic
{
    public class GeneralMethodsMono : MonoBehaviour
    {
        public static GeneralMethodsMono Instance;

        public GeneralMethodsMono()
        {
            Instance = this;
        }

        public static List<T> FindObjectsOfInterface<T>() where T : class
        {
            var monoBehaviours = FindObjectsOfType(typeof(MonoBehaviour)) as MonoBehaviour[];
            return monoBehaviours != null ? monoBehaviours.Select(behaviour => behaviour.GetComponent(typeof(T))).OfType<T>().ToList() : new List<T>();
        } 
    }
}
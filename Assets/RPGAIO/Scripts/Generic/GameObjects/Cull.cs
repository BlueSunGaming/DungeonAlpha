using UnityEngine;

namespace LogicSpawn.RPGMaker.Generic
{
    public class Cull : MonoBehaviour {
        void Start ()
        {
            var renderers = GetComponentsInChildren<Renderer>();
            foreach(var r in renderers)
            {
                r.enabled = false;    
            }
        }
    }
}

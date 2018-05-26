using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class CastAreaLayers : MonoBehaviour
    {
        public static LayerMask LayerMask;
        public LayerMask Layers;

        private void Awake()
        {
            LayerMask = Layers;
        }

        private void Update()
        {
            LayerMask = Layers;
        }
    }
}
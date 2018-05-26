using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class ClickAttackLayers: MonoBehaviour
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
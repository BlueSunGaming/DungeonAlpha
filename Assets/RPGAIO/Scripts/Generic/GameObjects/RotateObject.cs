using UnityEngine;

namespace LogicSpawn.RPGMaker.Generic
{
    public class RotateObject : MonoBehaviour {

        public Vector3 RotationPerSecond;

        // Update is called once per frame
        void Update () {
            transform.Rotate(RotationPerSecond * Time.deltaTime);
        }
    }
}

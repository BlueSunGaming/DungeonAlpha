using UnityEngine;

namespace LogicSpawn.RPGMaker
{
    public class Billboard : MonoBehaviour
    {
        void Update()
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                Camera.main.transform.rotation * Vector3.up);
        }
    }
}

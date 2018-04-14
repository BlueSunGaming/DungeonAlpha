using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.CameraUI
{
    public class CameraFollow : MonoBehaviour
    {


        GameObject player = null;

        // Use this for initialization
        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (player)
            {
                transform.position = player.transform.position;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace DungeonRPG.Character
{
    [RequireComponent(typeof(RawImage))]

    public class PlayerHealthBar : MonoBehaviour
    {
        RawImage healthBarRawImage;

        DungeonRPG.Character.Player player;

        // Use this for initialization
        void Start()
        {
            player = FindObjectOfType<DungeonRPG.Character.Player>();

            healthBarRawImage = GetComponent<RawImage>();
        }

        // Update is called once per frame
        void Update()
        {
            float xValue = -(player.healthAsPercentage / 2f) - 0.5f;

            healthBarRawImage.uvRect = new Rect(xValue, 0f, 0.5f, 1f);
        }
    }
}
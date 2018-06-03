using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LogicSpawn.RPGMaker.Core
{
    public class FloatingCombatText : MonoBehaviour
    {
        public Canvas Canvas;
        public Text Text;
        private float _duration;
        private float _floatSpeed;
        private float _fontSize;
        void Start()
        {
            _duration = Rm_RPGHandler.Instance.Combat.FloatDuration;
            _floatSpeed = Rm_RPGHandler.Instance.Combat.FloatSpeed;

            //Add random
            transform.position += new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-0.1f, 0.3f), 0);

            Destroy(this.gameObject, _duration);
            //get float and duration from rpg settings

            _fontSize = Text.fontSize;
        }
            
        void Update()
        {
            transform.rotation = GetObject.RPGCamera.transform.rotation;
            transform.position += new Vector3(0, _floatSpeed, 0) * Time.deltaTime;
            Text.color = new Color(Text.color.r, Text.color.g, Text.color.b, Text.color.a - ((1.0f / _duration) * 1.1f * Time.deltaTime));
            _fontSize -= 10 * Time.deltaTime;
            Text.fontSize = (int) _fontSize;
        }

        public void SetUp(string text)
        {
            Text.text = text;
        }
    }
}
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LogicSpawn.RPGMaker
{
    public class RPGShowText : MonoBehaviour
    {
        public ShowTextType ShowType;
        public float ShowDistance = 2;
        public float MinMouseOverShowDistance = 5;
        public Text TextRef;
        public bool BillboardText = true;
        private bool _show;
        private Transform _playerRef;

        void OnEnable()
        {
            _playerRef = GetObject.PlayerMonoGameObject.transform;  
        }

        void Update()
        {
            
            if (ShowType == ShowTextType.WhenNear)
            {
                _show = Vector3.Distance(_playerRef.position, transform.position) <= ShowDistance;
            }

            if (!GameMaster.ShowUI || GameMaster.CutsceneActive || GameMaster.GamePaused)
            {
                _show = false;
            }

            if(BillboardText)
            {
                TextRef.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                    Camera.main.transform.rotation * Vector3.up);
            }

            TextRef.gameObject.SetActive(_show);
        }

        void OnMouseOver()
        {
            if (ShowType == ShowTextType.OnMouseOver && Vector3.Distance(_playerRef.position, transform.position) >= MinMouseOverShowDistance)
            {
                _show = true;
            }
        }

        void OnMouseExit()
        {
            if (ShowType == ShowTextType.OnMouseOver)
            {
                _show = false;
            }
        } 
    }

    public enum ShowTextType
    {
        WhenNear,
        OnMouseOver
    }
}
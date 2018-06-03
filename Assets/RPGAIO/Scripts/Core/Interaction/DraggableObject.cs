using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace Assets.RPGAIO.Scripts.Core.Interaction
{
    public class DraggableObject : MonoBehaviour
    {
        public float ObjectDistInFrontOfPlayer = 3;
        public float MaxDragDistance = 25;
        public float MaxHeightAbovePlayer = 4;
        public bool ResetPosAfterIdle = true;
        public float ResetPosTime = 30;

        private float _resetPosTimer = 0;
        private Transform _playerTransform;
        private Camera _camera;
        private Vector3 _startPos;
        private Rigidbody _rigidbody;
        private bool _reset;

        void Awake()
        {
            _camera = Camera.main;
            _playerTransform = GetObject.PlayerMonoGameObject.transform;
            _startPos = transform.position;
            _rigidbody = GetComponent<Rigidbody>();
        }

        void Update()
        {
            if(ResetPosAfterIdle)
            {
                _resetPosTimer += Time.deltaTime;
                if(_resetPosTimer >= ResetPosTime)
                {
                    transform.position = _startPos;
                    _rigidbody.velocity = Vector3.zero;
                }
            }
        }

        void LateUpdate()
        {
            if(_resetPosTimer > 0 && !_reset)
            {
                _rigidbody.velocity = Vector3.zero;
                _reset = true;
            }
        }

        public void OnMouseDrag()
        {
            if (Vector3.Distance(_playerTransform.transform.position, transform.position) > MaxDragDistance)
            {
                _rigidbody.velocity = Vector3.zero;
                return;
            }
            _reset = false;
            _resetPosTimer = 0f;

            var distance = Vector3.Distance(_camera.transform.position, _playerTransform.position) + ObjectDistInFrontOfPlayer;

            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
            Vector3 objPosition = _camera.ScreenToWorldPoint(mousePosition);

            transform.position = new Vector3(objPosition.x, Mathf.Clamp(objPosition.y, _playerTransform.position.y + 2, _playerTransform.position.y + 2 + MaxHeightAbovePlayer), objPosition.z);
        }

        void OnMouseEnter()
        {
            
        }
    }
}
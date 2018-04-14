using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine.AI;
using RPG.CameraUI;


namespace RPG.Character
{
    [RequireComponent(typeof(NavMeshAgent))]

    [RequireComponent(typeof(AICharacterControl))]

    [RequireComponent(typeof(ThirdPersonCharacter))]


    public class PlayerMovement : MonoBehaviour

    {
        ThirdPersonCharacter ThirdPersonCharacter = null;   // A reference to the ThirdPersonCharacter on the object

        CameraRaycaster cameraRaycaster = null;

        Vector3 clickPoint;

        AICharacterControl aICharacterControl = null;

        GameObject walkTarget = null;

        [SerializeField] const int walkableLayerNumber = 8;

        [SerializeField] const int enemyLayerNumber = 9;


        private void Start()
        {
            cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();

            ThirdPersonCharacter = GetComponent<ThirdPersonCharacter>();

            aICharacterControl = GetComponent<AICharacterControl>();

            walkTarget = new GameObject("walkTarget");

            cameraRaycaster.notifyMouseClickObservers += ProcessMouseClick;
        }

        void Update()
        {
            var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;

            var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;
            
            transform.Rotate(0, x, 0);

            transform.Translate(0, 0, z);
        }

        void ProcessMouseClick(RaycastHit raycastHit, int layerHit)
        {
            switch (layerHit)
            {
                case enemyLayerNumber:

                    GameObject enemy = raycastHit.collider.gameObject;

                    transform.Rotate(0, enemy.transform.position.x, 0);

                    aICharacterControl.SetTarget(enemy.transform);

                    break;

                case walkableLayerNumber:

                    walkTarget.transform.position = raycastHit.point;

                    aICharacterControl.SetTarget(walkTarget.transform);

                    break;

                default:                    

                    return;

            }
        }

    }

}
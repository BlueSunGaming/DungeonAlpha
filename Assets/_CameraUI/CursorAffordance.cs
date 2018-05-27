using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DungeonRPG.CameraUI
{
    [RequireComponent(typeof(CameraRaycaster))]

    public class CursorAffordance : MonoBehaviour
    {

        [SerializeField] Texture2D itemCursor = null;
        [SerializeField] Texture2D walkCursor = null;
        [SerializeField] Texture2D unknownCursor = null;
        [SerializeField] Texture2D targetCursor = null;
        [SerializeField] Texture2D buttonCursor = null;
     
        [SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);
        public const int nWalkableLayerNumber = 8;
        public const int nEnemyLayerNumber = 11;
        public const int nNPCLayer = 10;
        public const int nItemLayer = 9;

        CameraRaycaster cameraRaycaster;


        // Use this for initialization
        void Start() {
            cameraRaycaster = GetComponent<CameraRaycaster>();

            cameraRaycaster.notifyLayerChangeObservers += OnLayerChanged; // registering
        }

        void OnLayerChanged(int newLayer)
        {
            switch (newLayer)
            {
                case 5: // TODO make cameraRaycaster member variables
                    Cursor.SetCursor(buttonCursor, cursorHotspot, CursorMode.Auto);

                    break;

                case nWalkableLayerNumber:

                    Cursor.SetCursor(walkCursor, cursorHotspot, CursorMode.Auto);

                    break;

                case nEnemyLayerNumber:

                    Cursor.SetCursor(targetCursor, cursorHotspot, CursorMode.Auto);

                    break;

                case nNPCLayer:

                    Cursor.SetCursor(targetCursor, cursorHotspot, CursorMode.Auto);

                    break;

                case nItemLayer:

                    Cursor.SetCursor(targetCursor, cursorHotspot, CursorMode.Auto);

                    break;

                default:

                    Cursor.SetCursor(unknownCursor, cursorHotspot, CursorMode.Auto);

                    return;
            }
        }

        // TODO consider de-registering OnLayerChanged on leaving all game scenes
    }
}

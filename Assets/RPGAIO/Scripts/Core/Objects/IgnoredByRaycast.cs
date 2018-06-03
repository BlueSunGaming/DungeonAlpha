using UnityEngine;

public class IgnoredByRaycast : MonoBehaviour
{
        void Start()
        {
            SetLayerRecursively(gameObject, LayerMask.NameToLayer("Ignore Raycast"));
        }

        void SetLayerRecursively(GameObject obj, int newLayer)
        {
            if (null == obj)
            {
                return;
            }

            obj.layer = newLayer;

            foreach (Transform child in obj.transform)
            {
                if (null == child)
                {
                    continue;
                }
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }
}
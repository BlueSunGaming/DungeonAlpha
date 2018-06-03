using System.Collections.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Generic
{
    public static class UnityExtensions 
    {
        public static string Colorfy(this string t, Rm_UnityColors color)
        {
            return "<color=" + color.ToString() + ">" + t + "</color>";
        }
        public static string Colorfy(this string t, string color)
        {
            return "<color=" + color + ">" + t + "</color>";

        }

        public static void SetLayerRecursively(this GameObject obj, int layer)
        {
            obj.layer = layer;

            foreach (Transform child in obj.transform)
            {
                child.gameObject.SetLayerRecursively(layer);
            }
        }

        public static Transform FindInChildren(this Transform self, string name, bool onlyIfActive = false)
        {
            int count = self.childCount;
            for (int i = 0; i < count; i++)
            {
                Transform child = self.GetChild(i);
                if (child.name == name && (!onlyIfActive || child.gameObject.activeInHierarchy)) return child;
                Transform subChild = child.FindInChildren(name);
                if (subChild != null) return subChild;
            }
            return null;
        }

        public static GameObject FindInChildren(this GameObject self, string name, bool onlyIfActive = false)
        {
            Transform transform = self.transform;
            Transform child = transform.FindInChildren(name,onlyIfActive);
            return child != null ? child.gameObject : null;
        }

        public static void DestroyChildren(this Transform t)
        {
            foreach (Transform child in t)
            {
                Object.Destroy(child.gameObject);
            }
        }

        public static GameObject GetChild(this Transform t, string childName)
        {
            foreach (Transform child in t)
            {
                if(child.name == childName)
                {
                    return child.gameObject;
                }
            }

            return null;
        }

        public static T[] GetAllChildren<T>(this Transform aObj) where T : Component
        {
            List<T> result = new List<T>();
            ProcessChild<T>(aObj, ref result);
            return result.ToArray();
        }

        public static T[] GetAllChildren<T>(this GameObject aObj) where T : Component
        {
            List<T> result = new List<T>();
            ProcessChild<T>(aObj.transform, ref result);
            return result.ToArray();
        }

        private static void ProcessChild<T>(Transform aObj, ref List<T> aList) where T : Component
        {
            T c = aObj.GetComponent<T>();
            if (c != null)
                aList.Add(c);
            foreach (Transform child in aObj)
                ProcessChild<T>(child, ref aList);
        }

        /// <summary>
        /// Get's the center of a transform's capsule collider if it has one
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static Vector3 Center(this Transform transform)
        {
            var capCol = transform.GetComponent<CapsuleCollider>();
            var cc = transform.GetComponent<CharacterController>();

            if(capCol != null)
            {
                return capCol.bounds.center;
            }
            
            return cc.bounds.center;
        }

        public static bool IsCloseTo(this Color color, Color otherColor)
        {
            Vector3 test1 = new Vector4(color.r, color.g, color.b, color.a);
            Vector3 test2 = new Vector4(otherColor.r, otherColor.b, otherColor.g, otherColor.a);
            var magnitude = ((test2 - test1).magnitude);
            //Debug.Log(magnitude);
            return magnitude < 5.0f;
        }

        public static void SetX(this Transform transform, float localx)
        {
            var newPosition = new Vector3(localx, transform.localPosition.y, transform.localPosition.z);
            transform.localPosition = newPosition;
        }
        
        public static void SetY(this Transform transform, float localy)
        {
            var newPosition = new Vector3(transform.localPosition.x, localy, transform.localPosition.z);
            transform.localPosition = newPosition;
        }

        public static void SetZ(this Transform transform, float localz)
        {
            var newPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, localz);
            transform.localPosition = newPosition;
        }


        public static void SetX(this RectTransform transform, float localx)
        {
            var newPosition = new Vector3(localx, transform.localPosition.y, transform.localPosition.z);
            transform.localPosition = newPosition;
        }

        public static void SetY(this RectTransform transform, float localy)
        {
            var newPosition = new Vector3(transform.localPosition.x, localy, transform.localPosition.z);
            transform.localPosition = newPosition;
        }

        public static void SetZ(this RectTransform transform, float localz)
        {
            var newPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, localz);
            transform.localPosition = newPosition;
        }

        public static void SetGlobalX(this Transform transform, float x)
        {
            var newPosition = new Vector3(x, transform.position.y, transform.position.z);
            transform.position = newPosition;
        }

        public static void SetGlobalY(this Transform transform, float y)
        {
            var newPosition = new Vector3(transform.position.x, y, transform.position.z);
            transform.position = newPosition;
        }

        public static void SetGlobalZ(this Transform transform, float z)
        {
            var newPosition = new Vector3(transform.position.x, transform.position.y, z);
            transform.position = newPosition;
        }
    }
}

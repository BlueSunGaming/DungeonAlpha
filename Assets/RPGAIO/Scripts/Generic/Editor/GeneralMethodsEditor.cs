using LogicSpawn.RPGMaker.Editor;
using UnityEditor;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Generic
{
    public static class GeneralMethodsEditor
    {
        public static void InstantiateInView(GameObject gameObjectPrefab)
        {
            Selection.activeObject = SceneView.currentDrawingSceneView;
            var sceneCam = SceneView.currentDrawingSceneView.camera;
            var spawnPos = sceneCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 3f));
            var obj = PrefabUtility.InstantiatePrefab(gameObjectPrefab);
            ((GameObject)obj).transform.position = spawnPos;
        }


        public static GameObject SpawnLinkedPrefab(string prefabPath, Vector3 position, Quaternion rotation, Transform parent)
        {


            GameObject go = null;
            var prefab = Resources.Load(prefabPath) as GameObject;
            if (prefab != null)
            {
                go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                go.transform.position = position;
                go.transform.rotation = rotation;
                if (parent != null)
                {
                    go.transform.parent = parent;
                }
            }
            return go;
        }

        public static string CreatePrefab(GameObject gameObject)
        {
            var resourcepath = Rme_Tools_PrefabRepository.BasePrefabPath + "/" + gameObject.name;
            var path = "Assets/Resources/" + resourcepath;
            var emptyPrefab = PrefabUtility.CreateEmptyPrefab(path + ".prefab");
            PrefabUtility.ReplacePrefab(gameObject, emptyPrefab, ReplacePrefabOptions.ConnectToPrefab);
            return resourcepath;
        }
        public static string CreatePrefab(GameObject gameObject, string customPath)
        {
            var resourcepath = customPath + "/" + gameObject.name;
            var path = "Assets/Resources/" + resourcepath;
            var emptyPrefab = PrefabUtility.CreateEmptyPrefab(path + ".prefab");
            PrefabUtility.ReplacePrefab(gameObject, emptyPrefab, ReplacePrefabOptions.ConnectToPrefab);
            return resourcepath;
        }

        public static string CreatePrefab(GameObject gameObject, out GameObject prefabRef)
        {
            var resourcepath = Rme_Tools_PrefabRepository.BasePrefabPath + "/" + gameObject.name;
            var path = "Assets/Resources/" + resourcepath;
            var emptyPrefab = PrefabUtility.CreateEmptyPrefab(path + ".prefab");
            var prefab = PrefabUtility.ReplacePrefab(gameObject, emptyPrefab, ReplacePrefabOptions.ConnectToPrefab);
            prefabRef = prefab;
            return resourcepath;
        }
    }
}
using System;
using LogicSpawn.RPGMaker.Core;
using Newtonsoft.Json;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace LogicSpawn.RPGMaker.Generic
{
    public static class GeneralMethods
    {
        public delegate void ActionByRef(ref object value);

        private const string PlayerPrefsPrefabIdString = "RPG_Prefab_Id";

        public static T GetRandomEnum<T>()
        {
            var a = System.Enum.GetValues(typeof(T));
            var v = (T)a.GetValue(Random.Range(0, a.Length));
            return v;
        }

        public static T CopyObject<T>(T other)
        {
            var objectStr = JsonConvert.SerializeObject(other,serializerSettings);
            var item = JsonConvert.DeserializeObject<T>(objectStr,serializerSettings);
            return item;
        }

        private static JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects,
            TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
            ObjectCreationHandling = ObjectCreationHandling.Replace
        };

        public static string NextID
        {
            get
            {
                var saveId = PlayerPrefs.GetInt(PlayerPrefsPrefabIdString, -1);
                if (saveId != -1)
                {
                    var nextId = saveId + 1;
                    PlayerPrefs.SetInt(PlayerPrefsPrefabIdString, nextId);
                    return nextId.ToString();
                }
                else
                {
                    PlayerPrefs.SetInt(PlayerPrefsPrefabIdString, 0);
                    return 0.ToString();
                }
            }
        }

        public static GameObject SpawnPrefab(string prefabPath, Vector3 position, Quaternion rotation, Transform parent)
        {
            GameObject go = null;
            var prefab = Resources.Load(prefabPath) as GameObject;
            if (prefab != null)
            {
                go = (GameObject)Object.Instantiate(prefab, position, rotation);
                if (parent != null)
                {
                    go.transform.parent = parent;
                }
            }
            return go;
        }

        public static T CopySkill<T>(T other)
        {
            var skill = other as Skill;
            var casterMono = skill.CasterMono;
            var caster = skill.Caster;

            var copy = CopyObject(other);

            var skillCopy = copy as Skill;
            skillCopy.CasterMono = casterMono;
            skillCopy.Caster = caster;

            return copy;
        }

        public static Sprite CreateSprite(Texture2D image)
        {
            if (image == null) return null;

            return Sprite.Create(image, new Rect(0, 0, image.width, image.height), Vector2.zero);
        }
        public static Sprite CreateSprite(Texture2D image, Rect rect, Vector2 pivot)
        {
            if (image == null) return null;

            return Sprite.Create(image, rect, pivot);
        }
    }
}

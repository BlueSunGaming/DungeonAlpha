using System;
using System.Collections.Generic;
using LogicSpawn.RPGMaker.Beta;
using LogicSpawn.RPGMaker.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LogicSpawn.RPGMaker.Core
{
    public class SavableGameObject : MonoBehaviour
    {
        public static List<GameObject> AllSavableObjects = new List<GameObject>();
        public static List<SavableGameObjectData> DestroyedObjects = new List<SavableGameObjectData>();
        public string UniqueID;
        public SavableGameObjectData _saveSavableGameObjectData = new SavableGameObjectData();

        [ExecuteInEditMode]
        public SavableGameObject()
        {
            UniqueID = Guid.NewGuid().ToString();
            _saveSavableGameObjectData.UniqueID = UniqueID;
        }

        void Awake()
        {
            if(UniqueID == null)
            {
                UniqueID = Guid.NewGuid().ToString();
                _saveSavableGameObjectData.UniqueID = UniqueID;
            }
            AllSavableObjects.Add(gameObject);
        }

        public void UpdateSaveData()
        {
            _saveSavableGameObjectData.Active = gameObject.activeSelf;
            _saveSavableGameObjectData.Position = new SerializableVector3(gameObject.transform.position);
            _saveSavableGameObjectData.Rotation = new SerializableQuaternion(gameObject.transform.rotation);
            _saveSavableGameObjectData.Scale = new SerializableVector3(gameObject.transform.localScale);

            var loadedScene = SceneManager.GetActiveScene().name;
            _saveSavableGameObjectData.SceneName = loadedScene;

            var enemyData = gameObject.GetComponent<BaseCharacterMono>();
            if(enemyData)
            {
                _saveSavableGameObjectData.AdditionalData = enemyData.Character as CombatCharacter;
            }
        }

        void OnDisable()
        {
            _saveSavableGameObjectData.Active = false;
            //Debug.Log("Ondisable results in: " + gameObject.activeSelf);
        }

        void OnEnable()
        {
            _saveSavableGameObjectData.Active = true;
            //Debug.Log("Oneanble results in: " + gameObject.activeSelf);
        }

        void OnDestroy()
        {
            _saveSavableGameObjectData.Destroyed = true;
            UpdateSaveData();
            AllSavableObjects.Remove(gameObject);

            _saveSavableGameObjectData.UniqueID = UniqueID;
            DestroyedObjects.Add(_saveSavableGameObjectData);
        }

        public void LoadSavable(SavableGameObjectData firstOrDefault)
        {
            if(firstOrDefault != null)
            {
                //Debug.Log("Aply settings");
                _saveSavableGameObjectData = firstOrDefault;
                //Debug.Log("My Pos: " + gameObject.transform.position + "  Target Pos:" + _saveSavableGameObjectData.Position);
                gameObject.transform.position = (Vector3)_saveSavableGameObjectData.Position;
                gameObject.transform.rotation = (Quaternion)_saveSavableGameObjectData.Rotation;
                gameObject.transform.localScale = (Vector3) _saveSavableGameObjectData.Scale;
                gameObject.SetActive(_saveSavableGameObjectData.Active);
                if(_saveSavableGameObjectData.AdditionalData != null)
                {
                    var data = _saveSavableGameObjectData.AdditionalData;
                    if(data.CharacterType == CharacterType.Enemy)
                    {
                        GetComponent<EnemyCharacterMono>().SetEnemy(data);
                    }
                    else
                    {
                        GetComponent<NpcCharacterMono>().SetNPC(data as NonPlayerCharacter);

                    }
                }
                if(_saveSavableGameObjectData.Destroyed)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
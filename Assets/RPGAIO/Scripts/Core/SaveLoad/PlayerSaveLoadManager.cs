using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using nCrypto.Crypters;
using Object = UnityEngine.Object;

namespace LogicSpawn.RPGMaker.Core
{
    public class PlayerSaveLoadManager
    {
        private static readonly PlayerSaveLoadManager MyInstance = new PlayerSaveLoadManager();
        public static PlayerSaveLoadManager Instance
        {
            get { return MyInstance; }
        }

        public float TimeLastSaved;

        public string NextID
        {
            get
            {
                var saveId = PlayerPrefs.GetInt(PlayerPrefsIDString, -1);
                if(saveId != -1)
                {
                    var nextId = saveId + 1;
                    PlayerPrefs.SetInt(PlayerPrefsIDString, nextId);
                    return nextId.ToString();
                }
                else
                {
                    PlayerPrefs.SetInt(PlayerPrefsIDString, 0);
                    return 0.ToString();
                }
            }
        }

        private static string _savePath;
        public static string LastSavedGameKey = "LastSavePath";
        public static string LastSavePPrefIDKey = "LastSavePPrefID";
        private string PlayerPrefsIDString = "PlayerSaveID";
        private string PlayerPrefsSavePrefix = "Player_Save_";

        private bool encryptFiles = true;

        public PlayerSaveLoadManager()
        {
            if (Rm_RPGHandler.Instance == null) return;

#if (!UNITY_IOS && !UNITY_ANDROID)

#if(UNITY_STANDALONE_OSX)
                _savePath = Path.Combine(Application.persistentDataPath,
                                            @"Saves\");
#else
            _savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                        @"My Games\" + Rm_RPGHandler.Instance.GameInfo.GameCompany +
                                        @"\" + Rm_RPGHandler.Instance.GameInfo.GameTitle + @"\Saves\");
#endif


#else
            _savePath = "";
#endif
        }

        public void SetSavePath()
        {
#if(UNITY_STANDALONE_OSX)
            _savePath = Path.Combine(Application.persistentDataPath,
                                        @"Saves\");
#else
            _savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                        @"My Games\" + Rm_RPGHandler.Instance.GameInfo.GameCompany +
                                        @"\" + Rm_RPGHandler.Instance.GameInfo.GameTitle + @"\Saves\");
#endif
        }

        public List<string> GetSaveFileIds()
        {
            return Directory.GetFiles(_savePath).ToList();
        }

        public void Save(PlayerSave playerToCreate, bool firstSave)
        {
            //playerToCreate.Initialize();
            if(!firstSave)
                playerToCreate.Character.FullUpdateStats();
            playerToCreate.LastSaved = DateTime.Now;
            var timePlayed = (Time.time - TimeLastSaved);
            playerToCreate.TimePlayed = playerToCreate.TimePlayed.Add(new TimeSpan(0, 0, (int)timePlayed));
            //Debug.Log("Total Time played:" + playerToCreate.TimePlayed.ToString());
            TimeLastSaved = Time.time;

            //todo: playerToCreate.currentScene = SceneHandler.getSceneName();
            //Debug.Log("Saving ...");
            var fileName = playerToCreate.SaveName.Replace(" ", "_") + "_" +
                            playerToCreate.SaveID.Substring(playerToCreate.SaveID.Length - 16, 12) + ".saveFile";

            playerToCreate.SavePath = Path.Combine(_savePath, fileName);
            var saveFile = JsonConvert.SerializeObject(playerToCreate, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
                ObjectCreationHandling = ObjectCreationHandling.Replace
            });

#if (!UNITY_IOS && !UNITY_ANDROID)

            var saveFileEncoded = saveFile;

            if (encryptFiles)
            {
                saveFileEncoded = new Xor().Encrypt(saveFileEncoded, GameDataSaveLoadManager.Instance.EncryptKey);
            }



            Directory.CreateDirectory(_savePath);

            File.WriteAllText(playerToCreate.SavePath, saveFileEncoded);

            SaveSavableGameObjects(playerToCreate, fileName);
            
            //Set last saved
            PlayerPrefs.SetString(LastSavedGameKey, playerToCreate.SavePath);
#else
            PlayerPrefs.SetString(PlayerPrefsSavePrefix + playerToCreate.SaveID, saveFile);
            PlayerPrefs.SetString(LastSavePPrefIDKey, PlayerPrefsSavePrefix + playerToCreate.SaveID);
#endif

            //Debug.Log("Saved file with ID " + playerToCreate.SaveID);
            UserSaveLoadManager.Instance.SaveUserData();

        }

        public bool Load(string savePathOrPPrefKey)
        {
            GetObject.ClearReferences();
#if (!UNITY_IOS && !UNITY_ANDROID)
            if (!File.Exists(savePathOrPPrefKey)) return false;
            Debug.Log("SavePath: " + savePathOrPPrefKey);
            var fileLoad = File.ReadAllText(savePathOrPPrefKey);
            var jsonFromEncode = fileLoad;
            if (encryptFiles)
            {
                jsonFromEncode = new Xor().Decrypt(fileLoad, GameDataSaveLoadManager.Instance.EncryptKey);
            }
#else
            var jsonFromEncode = PlayerPrefs.GetString(savePathOrPPrefKey, "");
#endif

            if(string.IsNullOrEmpty(jsonFromEncode))
            {
                return false;
            }

            var loadedPlayerSave = JsonConvert.DeserializeObject<PlayerSave>(jsonFromEncode, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
                ObjectCreationHandling = ObjectCreationHandling.Replace
            });

            var loadedScene = SceneManager.GetActiveScene().name;
            if (loadedScene != loadedPlayerSave.CurrentScene)
            {
                RPG.LoadLevel(loadedPlayerSave.CurrentScene,false);

            }

            //todo: check this
            var classDef = Rm_RPGHandler.Instance.Player.CharacterDefinitions.FirstOrDefault(c => c.ID == loadedPlayerSave.Character.PlayerCharacterID);
            if(classDef == null)
            {
                Debug.Log("Class not found for character you're trying to load. Try creating a new character.");
                return false;
            }

            var className = classDef.Name;

            var gameObject = Resources.Load(classDef.ClassPrefabPath) as GameObject;
            var spawnTransform = GetObject.SpawnPositionTransform;

            var spawnPosition = spawnTransform.transform.position;
            if(!string.IsNullOrEmpty(loadedPlayerSave.WorldMap.CurrentLocationID))
            {
                var location = Rm_RPGHandler.Instance.Customise.WorldMapLocations.First(w => w.ID == loadedPlayerSave.WorldMap.CurrentWorldAreaID).
                    Locations.First(l => l.ID == loadedPlayerSave.WorldMap.CurrentLocationID);

                if(location.UseCustomLocation)
                {
                    spawnPosition = location.CustomSpawnLocation;
                }
            }

            if(spawnTransform != null)
            {
                //Spawn Player
                Object.Instantiate(gameObject, spawnPosition, GetObject.SpawnPositionTransform.rotation);

                GetObject.RPGCamera.transform.position = spawnPosition - (GetObject.SpawnPositionTransform.forward*2);
            }
            else
            {
                Debug.LogError("[RPGAIO] Did not find spawn position. This is required to spawn the player.");
            }
                

            GetObject.ClearReferences();
            var playerMono = GetObject.PlayerMono;
            playerMono.SetPlayerSave(loadedPlayerSave);


#if (!UNITY_IOS && !UNITY_ANDROID)
            //Todo: allow savables for mobile
            //Debug.Log("Loading savables");
            LoadSavableGameObjects(savePathOrPPrefKey);
#endif


            //Debug.Log("Loaded file with ID " + loadedPlayerSave.SaveID);
            UserSaveLoadManager.Instance.LoadUserData();
            TimeLastSaved = Time.time;
            return true;
        }

#if (!UNITY_IOS && !UNITY_ANDROID)

        public PlayerSave GetPlayerSave(string saveFile)
        {
            var fileLoad = File.ReadAllText(saveFile);
            var jsonFromEncode = fileLoad;
            if (encryptFiles)
            {
                jsonFromEncode = new Xor().Decrypt(fileLoad, GameDataSaveLoadManager.Instance.EncryptKey);
            }
            if (string.IsNullOrEmpty(jsonFromEncode))
            {
                return null;
            }

            var loadedPlayerSave = JsonConvert.DeserializeObject<PlayerSave>(jsonFromEncode, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
                ObjectCreationHandling = ObjectCreationHandling.Replace
            });

            return loadedPlayerSave;
        }
#endif

        public bool LoadLastSaved()
        {
#if (!UNITY_IOS && !UNITY_ANDROID)

            var lastSavePath = PlayerPrefs.GetString(LastSavedGameKey,"");
#else
            var lastSavePath = PlayerPrefs.GetString(LastSavePPrefIDKey, "");
#endif

            if (string.IsNullOrEmpty(lastSavePath))
            {
                return false;
            }

            return Load(lastSavePath);
        }

        public void SaveGame()
        {
            var player = GetObject.PlayerMonoGameObject;
            if(player != null)
            {
                var playerToSave = player.GetComponent<PlayerMono>();
                Instance.Save(playerToSave.PlayerSave,false);
            }
        }        
        
        public void SavePlayerSave(PlayerSave save)
        {
            Instance.Save(save, false);
        }

#if (!UNITY_IOS && !UNITY_ANDROID)
        public void SaveSavableGameObjects(PlayerSave playerSave, string fileName)
        {

            //Debug.Log("Saving savable gameObjects...");

            var gameObjectsWithSavable = SavableGameObject.AllSavableObjects;
            var objectsWithSavable = gameObjectsWithSavable.Select(g => g.GetComponent<SavableGameObject>()).ToArray();
            var listOfSavabableData = new List<SavableGameObjectData>();
            foreach(var obj in objectsWithSavable)
            {
                obj.UpdateSaveData();
                obj._saveSavableGameObjectData.UniqueID = obj.UniqueID;
                listOfSavabableData.Add(obj._saveSavableGameObjectData);
            }

            foreach (var obj in SavableGameObject.DestroyedObjects)
            {
                listOfSavabableData.Add(obj);
            }

            var saveFile = JsonConvert.SerializeObject(listOfSavabableData, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
                ObjectCreationHandling = ObjectCreationHandling.Reuse
            });
            var saveFileEncoded = saveFile;

            fileName = fileName + ".savables";

            Directory.CreateDirectory(_savePath);

            File.WriteAllText(Path.Combine(_savePath,fileName), saveFileEncoded);
            //Debug.Log("Saved savables " + playerSave.SaveID);
        }

        public void LoadSavableGameObjects(string savePath)
        {
            if (!File.Exists(savePath + ".savables")) return;

            var fileLoad = File.ReadAllText(savePath + ".savables");
            
            var jsonFromEncode = fileLoad;

            var loadedSavables = JsonConvert.DeserializeObject<List<SavableGameObjectData>>(jsonFromEncode, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
                ObjectCreationHandling = ObjectCreationHandling.Reuse
            });

            foreach(var i in loadedSavables)
            {
                Debug.Log("Unique ID from loaded:"  + i.UniqueID);
            }

            var objectsWithSavable = Object.FindObjectsOfType(typeof (SavableGameObject)) as SavableGameObject[];

            foreach(var obj in objectsWithSavable)
            {
                var foundData = loadedSavables.FirstOrDefault(l => l.UniqueID == obj.UniqueID);

                if(foundData != null)
                    obj.LoadSavable(foundData);
            }

            //Debug.Log("Done Loaded Savables");
        }
#endif

        public List<PlayerSave> GetSaves()
        {
#if (!UNITY_IOS && !UNITY_ANDROID)

            var saves = new List<PlayerSave>();
            Directory.CreateDirectory(_savePath);
            var saveFiles = Directory.GetFiles(_savePath, "*.saveFile");
            foreach(var savePath in saveFiles)
            {
                var loadedSave = GetPlayerSave(savePath);
                if(loadedSave != null) saves.Add(loadedSave);
            }

            return saves;
#else
            var saves = new List<PlayerSave>();
            var recent = RecentSave();
            if(recent != null) saves.Add(recent);
            return saves;
#endif
        }

        public PlayerSave RecentSave()
        {
#if (!UNITY_IOS && !UNITY_ANDROID)

            var lastSavePath = PlayerPrefs.GetString(LastSavedGameKey, "");
#else
            var lastSavePath = PlayerPrefs.GetString(LastSavePPrefIDKey, "");
#endif

#if (!UNITY_IOS && !UNITY_ANDROID)
            if (!File.Exists(lastSavePath)) return null;
            //Debug.Log("SavePath: " + lastSavePath);
            var fileLoad = File.ReadAllText(lastSavePath);
            var jsonFromEncode = fileLoad;
            if (encryptFiles)
            {
                jsonFromEncode = new Xor().Decrypt(fileLoad, GameDataSaveLoadManager.Instance.EncryptKey);
            }
#else
            var jsonFromEncode = "";
            if(!string.IsNullOrEmpty(lastSavePath))
            {
                jsonFromEncode = PlayerPrefs.GetString(lastSavePath, "");
            }
#endif

            if (string.IsNullOrEmpty(jsonFromEncode))
            {
                return null;
            }

            PlayerSave loadedPlayerSave = null;
            try
            {
                loadedPlayerSave = JsonConvert.DeserializeObject<PlayerSave>(jsonFromEncode, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                });
            }
            catch(Exception e)
            {
                Debug.LogError(e);
            }

            return loadedPlayerSave;
        }

        public void LoadPlayer(PlayerSave player, string savePath, bool saveCurrent)
        {
            PlayerPrefs.SetString(LastSavedGameKey, savePath);
            RPG.LoadLevel(player.CurrentScene, false, saveCurrent);
        }

        public void LoadPlayerWithForcedReload(PlayerSave player, string savePath, bool saveCurrent)
        {
            PlayerPrefs.SetString(LastSavedGameKey, savePath);
            RPG.LoadLevel(player.CurrentScene, false, saveCurrent, null, null, true);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LogicSpawn.RPGMaker.API
{
    public  static partial class RPG
    {
        

        //todo: check scene exists
        public static bool LoadLevel(string name, bool updatePlayerLevel, bool saveGame = true, WorldArea worldArea = null, Location location = null, bool forceReload = false)
        {
            //check we can load
            //if we can load then:

            if (updatePlayerLevel && GetObject.PlayerSave != null)
            {
                GetObject.PlayerSave.CurrentScene = name;
                if(worldArea != null)
                {
                    GetObject.PlayerSave.WorldMap.CurrentWorldAreaID = worldArea.ID;    
                }
                
                if(location != null)
                {
                    GetObject.PlayerSave.WorldMap.CurrentLocationID = location.ID;    
                }
            
            }

            if(location != null)
            {
                GetObject.PlayerSave.WorldMap.AddVisitedLocation(location.ID);
            }

            if(saveGame)
                PlayerSaveLoadManager.Instance.SaveGame();

            GetObject.ClearReferences();
            Time.timeScale = 1;

            var loadedPlayerSave = GetObject.PlayerSave;
            var saveToLoad = PlayerSaveLoadManager.Instance.RecentSave();

            var loadedScene = SceneManager.GetActiveScene().name;

            if (forceReload || loadedScene != name || (loadedPlayerSave == null || loadedPlayerSave.SaveID != saveToLoad.SaveID)) //check it's the same save
            {
                SceneManager.LoadScene(name);    
                return true;
            }
            else
            {
                var spawnPosition = GetObject.SpawnPositionTransform.transform.position;
                var playerSave = GetObject.PlayerSave;
                if (!string.IsNullOrEmpty(playerSave.WorldMap.CurrentLocationID))
                {
                    var newlocation = Rm_RPGHandler.Instance.Customise.WorldMapLocations.First(w => w.ID == playerSave.WorldMap.CurrentWorldAreaID).
                        Locations.First(l => l.ID == playerSave.WorldMap.CurrentLocationID);

                    if (newlocation.UseCustomLocation)
                    {
                        spawnPosition = location.CustomSpawnLocation;
                    }
                }
                GetObject.PlayerController.Resume();
                GetObject.PlayerMonoGameObject.transform.position = spawnPosition;
            }

            return false;
        }

        public static void LoadLevel(int index)
        {
            GetObject.ClearReferences();
            Time.timeScale = 1;
            SceneManager.LoadScene(index);
            GameMaster.GamePaused = false;
        }

        public static void LoadLevel(string name)
        {
            GetObject.ClearReferences();
            Time.timeScale = 1;
            RPG.LoadLevel(name, false);
            GameMaster.GamePaused = false;
        }

        //New way:
        public static RPGEvents Events = new RPGEvents();

        public static List<PlayerSave> PlayerSaves
        {
            get { return PlayerSaveLoadManager.Instance.GetSaves(); }
        }

        public static PlayerSave GetPlayerSave
        {
            get { return GetObject.PlayerSave; }
        }

        public static PlayerCharacter GetPlayerCharacter
        {
            get { return GetObject.PlayerCharacter; }
        }

        public static GameObject GetPlayerGameObject
        {
            get { return GetObject.PlayerMonoGameObject; }
        }

        public static Rm_GameConfig GameSettings
        {
            get { return Rm_GameConfig.Instance; }
        }

        public static void Save()
        {
            PlayerSaveLoadManager.Instance.SaveGame();
        }
    }
}
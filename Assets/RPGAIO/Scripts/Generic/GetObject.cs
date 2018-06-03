using Assets.Scripts.Beta;
using Assets.Scripts.Beta.NewImplementation;
using LogicSpawn.RPGMaker.Core;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Generic
{
    public static class GetObject
    {
        private static GameObject _playerMonoGo;
        public static GameObject PlayerMonoGameObject
        {
            get { return _playerMonoGo ?? (_playerMonoGo = GameObject.FindGameObjectWithTag("Player")); }
        }

        private static PlayerMono _playerMono;
        public static PlayerMono PlayerMono
        {
            get { return PlayerMonoGameObject != null 
                ? _playerMono ?? (_playerMono = PlayerMonoGameObject.GetComponent<PlayerMono>())
                : null; }
        }

        private static PlayerCharacter _playerCharacter;
        public static PlayerCharacter PlayerCharacter
        {
            get { return PlayerMono != null
                ? _playerCharacter ?? (_playerCharacter = PlayerMono.Player)
                : null; }
        }

        private static RPG_Camera _rpgCamera;
        public static RPG_Camera RPGCamera
        {
            get { return _rpgCamera ?? (_rpgCamera = Camera.main.GetComponent<RPG_Camera>()); }
        }

        private static RPG_MinimapCamera _rpgMinimapCamera;
        public static RPG_MinimapCamera RPGMinimapCamera
        {
            get { return _rpgMinimapCamera ?? 
                (_rpgMinimapCamera = GameObject.FindGameObjectWithTag("MinimapCamera") != null ? GameObject.FindGameObjectWithTag("MinimapCamera").GetComponent<RPG_MinimapCamera>() : null); }
        }

        private static UIHandler _uiHandler;
        public static UIHandler UIHandler
        {
            get { return _uiHandler ?? (_uiHandler = GameObject.FindGameObjectWithTag("UIHandler").GetComponent<UIHandler>()); }
        }

        public static bool InGame
        {
            get { return GameObject.FindGameObjectWithTag("UIHandler") != null; }
        }

        private static PlayerSave _playerSave;
        public static PlayerSave PlayerSave
        {
            get { return PlayerMono != null 
                ? _playerSave ?? (_playerSave = PlayerMono.PlayerSave) 
                : null; }
        }

        private static GameObject _sceneMasterGameObject;
        public static GameObject SceneMasterGameObject
        {
            get { return _sceneMasterGameObject ?? (_sceneMasterGameObject = GameObject.FindGameObjectWithTag("SceneMaster")); }
        }

        public static Transform SpawnPositionTransform
        {
            get { return GameObject.FindGameObjectWithTag("SpawnPosition") != null ? GameObject.FindGameObjectWithTag("SpawnPosition").transform : null; }
        }

        private static LootSpawner _lootSpawner;
        public static LootSpawner LootSpawner
        {
            get
            {
                var lootSpawner = GameObject.FindGameObjectWithTag("LootSpawner");
                return _lootSpawner ?? (_lootSpawner = lootSpawner != null ? lootSpawner.GetComponent<LootSpawner>() : null);
            }

        }
        private static AudioPlayer _audioPlayer;
        public static AudioPlayer AudioPlayer
        {
            get
            {
                var audioPlayer = GameObject.FindGameObjectWithTag("AudioPlayer");
                return _audioPlayer ?? (_audioPlayer = audioPlayer != null ? audioPlayer.GetComponent<AudioPlayer>() : null);
            }
        }

        private static IRPGController _playerController;
        public static IRPGController PlayerController
        {
            get
            {
                var playerMono = PlayerMonoGameObject;
                if(playerMono == null) return null;
                var playerController = playerMono.GetComponent<RPGController>();
                return _playerController ?? (_playerController = playerController != null ? playerController.GetComponent<RPGController>() : null);
            }
        }
        private static RPGEventHandler _eventHandler;
        public static RPGEventHandler EventHandler
        {
            get
            {
                return _eventHandler ?? (_eventHandler = GameObject.FindGameObjectWithTag("EventHandler").GetComponent<RPGEventHandler>());
            }
        }

        public static void ClearReferences()
        {
            _playerMonoGo = null;
            _playerMono = null;
            _playerCharacter = null;
            _rpgCamera = null;
            _playerSave = null;
            _sceneMasterGameObject = null;
            _lootSpawner = null;
            _audioPlayer = null;
            _playerController = null;
            _eventHandler = null;
            _uiHandler = null;
            _rpgMinimapCamera = null;
        }
    }
}
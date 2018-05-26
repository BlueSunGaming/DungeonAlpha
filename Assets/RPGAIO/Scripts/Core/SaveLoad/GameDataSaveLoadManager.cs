using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using nCrypto.Crypters;

namespace LogicSpawn.RPGMaker.Core
{
    public class GameDataSaveLoadManager
    {
        private static readonly GameDataSaveLoadManager MyInstance = new GameDataSaveLoadManager();
        public static GameDataSaveLoadManager Instance
        {
            get { return MyInstance; }
        }

        public string GameDataFileName = "main.GAMEDATA.txt";
        public bool EncryptFiles = false;
        public string EncryptKey = "msoa2l0szmm9129ls";
        public bool LoadedOnce;

        public GameDataSaveLoadManager()
        {
        }

        public void LoadNodes()
        {

            var n1 = Rm_RPGHandler.Instance.Nodes.AchievementsNodeBank.NodeTrees;
            var n2 = Rm_RPGHandler.Instance.Nodes.CombatNodeBank.NodeTrees;
            var n3 = Rm_RPGHandler.Instance.Nodes.DialogNodeBank.NodeTrees;
            var n4 = Rm_RPGHandler.Instance.Nodes.EventNodeBank.NodeTrees;
            var n5 = Rm_RPGHandler.Instance.Nodes.WorldMapNodeBank.NodeTrees;

            var all = n1.Concat(n2).Concat(n3).Concat(n4).Concat(n5);
            foreach (var x in all)
            {
                x.Variables.ForEach(v => v.ResetValue());
            }
        }

        //todo: this doesn't load right for some reason...
        public void LoadGameData()
        {
            //Debug.Log("Loading game data from ingame");
            var foundData = false;
            var textAsset = (Resources.Load("GameData/main.GAMEDATA") as TextAsset);
            if(textAsset != null)
            {
                var jsonFromEncode = textAsset.text;
                //Debug.Log(jsonFromEncode);
                if (!string.IsNullOrEmpty(jsonFromEncode))
                {
                    foundData = true;
                    if (EncryptFiles)
                    {
                        jsonFromEncode = new Xor().Decrypt(jsonFromEncode, EncryptKey);
                    }

                    Rm_RPGHandler loadedGameData = null;
                    try
                    {

                        loadedGameData = JsonConvert.DeserializeObject<Rm_RPGHandler>(jsonFromEncode,
                                                                                            new JsonSerializerSettings
                                                                                            {
                                                                                                TypeNameHandling =
                                                                                                    TypeNameHandling.Objects,
                                                                                                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
                                                                                                ObjectCreationHandling = ObjectCreationHandling.Replace
                                                                                            });

                        Rm_RPGHandler.Instance = null;
                        Rm_RPGHandler.Instance = loadedGameData;
                        LoadNodes();
                    }
                    catch(Exception e)
                    {
                        throw e;
                        foundData = false;
                    }
                }
            }

            
            if(!foundData)
            {
                throw new Exception("Fatal Exception. Game data not found or is corrupt.");
            }

            LoadedOnce = true;
        }

        public void LoadIfNotLoaded()
        {
            //Load
            if(!LoadedOnce)
            {
                Debug.Log("Loading game data from ingame first time");
                LoadGameData();
            }
        }
    }
}
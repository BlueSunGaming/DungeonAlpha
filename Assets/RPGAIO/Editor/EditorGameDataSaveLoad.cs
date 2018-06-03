using System;
using System.Collections.Generic;
using System.IO;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Editor.New;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using nCrypto.Crypters;

#if UNITY_EDITOR
public static class EditorGameDataSaveLoad
{
    static EditorGameDataSaveLoad()
    {
        SavePath = Path.Combine(Application.dataPath, "Resources/GameData/");
    }

    private static readonly string SavePath;

    public static void LoadGameDataFromEditor()
    {
        var foundData = false;

        string jsonFromEncode = null;

        //Create prefab repository if it does not exist
        Directory.CreateDirectory(Application.dataPath + "/Resources/RPGAIOPrefabRepository");

        try
        {
            if (File.Exists(SavePath + GameDataSaveLoadManager.Instance.GameDataFileName))
            {

                jsonFromEncode = File.ReadAllText(SavePath + GameDataSaveLoadManager.Instance.GameDataFileName);
                if (!string.IsNullOrEmpty(jsonFromEncode))
                {
                    foundData = true;
                    if (GameDataSaveLoadManager.Instance.EncryptFiles)
                    {
                        jsonFromEncode = new Xor().Decrypt(jsonFromEncode, GameDataSaveLoadManager.Instance.EncryptKey);
                    }

                    Rm_RPGHandler loadedGameData = null;
                    

                        loadedGameData = JsonConvert.DeserializeObject<Rm_RPGHandler>(jsonFromEncode,
                                                                                            new JsonSerializerSettings
                                                                                            {
                                                                                                TypeNameHandling = TypeNameHandling.Objects,
                                                                                                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
                                                                                                ObjectCreationHandling = ObjectCreationHandling.Replace
                                                                                            });
                    
                    

                    if (foundData)
                    {

                        Rm_RPGHandler.Instance = null;
                        Rm_RPGHandler.Instance = loadedGameData;
                        GameDataSaveLoadManager.Instance.LoadNodes();
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            Debug.LogError("JSON Deserialization error. Creating new data. Check .error.bak to restore.");
            using (var stringwriter = new StreamWriter(SavePath + GameDataSaveLoadManager.Instance.GameDataFileName + "-" + DateTime.Now.ToString("ddMMMyyyy") + ".error.bak", false))
            {
                stringwriter.Write(jsonFromEncode);
            }
            foundData = false;

        }

        if (!foundData)
        {
            Debug.Log("[RPGAIO] No game data found. Creating new game data. ");
            NewData();
        }

        //Debug.Log("Loaded game data");

        GameDataSaveLoadManager.Instance.LoadedOnce = true;
    }
            
    public static void SaveGameData(bool isAutoSave = false)
    {

        var gameData = Rm_RPGHandler.Instance;

        if(!isAutoSave)
        {
            Debug.Log("[RPGAIO] Saving Game Data ...");
        }

        var saveFile = JsonConvert.SerializeObject(gameData, Formatting.Indented, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects,
            TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple, 
            ObjectCreationHandling = ObjectCreationHandling.Replace
        });

        string saveFileEncoded;
        if (GameDataSaveLoadManager.Instance.EncryptFiles)
        {
            var crypto = new Xor();

            saveFileEncoded = crypto.Encrypt(saveFile, GameDataSaveLoadManager.Instance.EncryptKey);
        }
        else
        {
            saveFileEncoded = saveFile;
        }

        Directory.CreateDirectory(SavePath);

        var oldFileName = SavePath + GameDataSaveLoadManager.Instance.GameDataFileName;
        if (Rm_RPGHandler.Instance.Preferences.EnableBackupOnSave && !isAutoSave && File.Exists(oldFileName))
        {
            if (Rm_RPGHandler.Instance.Preferences.OneBackupPerDay)
            {
                var newFileName = SavePath + GameDataSaveLoadManager.Instance.GameDataFileName + "-" + DateTime.Now.ToString("ddMMMyyyy") + ".bak";
                if (File.Exists(newFileName))
                {
                    File.Delete(newFileName);
                }
                File.Move(oldFileName, newFileName);
            }
            else
            {
                var newFileName = SavePath + GameDataSaveLoadManager.Instance.GameDataFileName + "-" + DateTime.Now.ToString("ddMMMyyyy") + " at " + DateTime.Now.ToString("HH-mm-ss") + ".bak";

                if (File.Exists(newFileName))
                {
                    File.Delete(newFileName);
                }
                File.Move(oldFileName, newFileName);
            }
        }
        else if (isAutoSave)
        {
            var newFileName = SavePath + GameDataSaveLoadManager.Instance.GameDataFileName + ".autosave.bak";

            if (File.Exists(newFileName))
            {
                File.Delete(newFileName);
            }

            File.Move(oldFileName, newFileName);
        }

        using(var stringwriter = new StreamWriter(SavePath + GameDataSaveLoadManager.Instance.GameDataFileName,false))
        {
            stringwriter.Write(saveFileEncoded);
        }

        if(!isAutoSave)
        {   
            Notify.Save("Saved Game Data");
        }

        //Update game config defaults, in case we added button etc
        GameSettingsSaveLoadManager.Instance.NewData();
        AssetDatabase.Refresh();

    }
            
    public static void NewData()
    {
        var newData = new Rm_RPGHandler();
        PlayerPrefs.SetString(PlayerSaveLoadManager.LastSavedGameKey, "");
        PlayerPrefs.SetString(PlayerSaveLoadManager.LastSavePPrefIDKey, "");
        AddDummyData(newData);
        Rm_RPGHandler.Instance = newData;
        SaveGameData();
    }

    //todo: this is where we will have templates
    private static void AddDummyData(Rm_RPGHandler newData)
    {
        //Dummy Data
        var attStr = new Rm_AttributeDefintion() { Name = "Strength", DefaultValue = 10 };
        var attStr3 = new Rm_AttributeDefintion() { Name = "Dexterity", DefaultValue = 10 };
        var attStr4 = new Rm_AttributeDefintion() { Name = "Intelligence", DefaultValue = 10 };
        var attStr5 = new Rm_AttributeDefintion() { Name = "Vitality", DefaultValue = 10 };
        newData.ASVT.AttributesDefinitions.Add(attStr);
        newData.ASVT.AttributesDefinitions.Add(attStr3);
        newData.ASVT.AttributesDefinitions.Add(attStr4);
        newData.ASVT.AttributesDefinitions.Add(attStr5);

        var attStr2 = new Rm_VitalDefinition() { Name = "Mana", DefaultValue = 100 };
        newData.ASVT.VitalDefinitions.Add(attStr2);

        var statDef1 = new Rm_StatisticDefintion() { ID = "Movement", Name = "Movement", DefaultValue = 1, IsDefault = true };
        var statDef2 = new Rm_StatisticDefintion() { ID = "Attack Speed", Name = "Attack Speed", DefaultValue = 1, IsDefault = true };
        var statDef3 = new Rm_StatisticDefintion() { ID = "Attack Range", Name = "Attack Range", DefaultValue = 2, IsDefault = true };
        var statDef4 = new Rm_StatisticDefintion() { ID = "Cast Speed", Name = "Cast Speed", DefaultValue = 1, IsDefault = true };
        var statDef5 = new Rm_StatisticDefintion() { ID = "Critical Chance", Name = "Critical Chance", DefaultValue = 0, IsDefault = true };
        newData.ASVT.StatisticDefinitions.AddRange(new[] { statDef1, statDef2, statDef3 , statDef4, statDef5});
        var classDef = new Rm_ClassDefinition()
        {
            Name = "Default",
            ClassPrefabPath = "Prefabs/Classes/Default",
            AttributePerLevel = new List<Rm_AsvtAmount>()
                                                        {
                                                            new Rm_AsvtAmount() { Amount = 5,
                                                                AsvtID = attStr.ID}
                                                        },
            StartingScene = "DemoScene",
            ApplicableRaceID = "Default_Builtin_Race_10101010",
            ApplicableSubRaceID = "Default_Builtin_SubRace_10101010",
            ApplicableGenderID = "Default_Builtin_Gender_10101010",
            ApplicableClassIDs = new List<StringField>()
                                     {
                                         new StringField()
                                             {
                                                 ID = "Default_Builtin_Class_10101010"
                                             }
                                     }
            
        };
        newData.Player.CharacterDefinitions.Add(classDef);
        newData.GameInfo.MinimapOptions.PlayerIconPath = "RPGMakerAssets/minimapIcon";
    }

    public static bool LoadIfNotLoadedFromEditor()
    {
        if (!GameDataSaveLoadManager.Instance.LoadedOnce)
        {
            LoadGameDataFromEditor();
            return true;
        }

        return false;
    }
}
#endif
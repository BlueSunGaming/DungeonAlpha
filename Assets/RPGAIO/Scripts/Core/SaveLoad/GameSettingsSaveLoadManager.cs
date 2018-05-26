using System;
using System.Globalization;
using System.IO;
using IniParser;
using IniParser.Model;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class GameSettingsSaveLoadManager
    {
        private static readonly GameSettingsSaveLoadManager MyInstance = new GameSettingsSaveLoadManager();
        public static GameSettingsSaveLoadManager Instance
        {
            get { return MyInstance; }
        }

        private string _savePath;
        private string settingsFileName = "settings.cfg";
        private string iniFileName = "config.ini";
        private string PlayerPrefsKey = "Game_Config_Settings";


        public void GetSavePath()
        {

#if(UNITY_STANDALONE_OSX)
            _savePath = Path.Combine(Application.persistentDataPath,
                                        @"\");
#else
            _savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                        @"My Games\" + Rm_RPGHandler.Instance.GameInfo.GameCompany +
                                        @"\" + Rm_RPGHandler.Instance.GameInfo.GameTitle + @"\");
#endif
        }

        public void SaveSettings()
        {
#if (!UNITY_IOS && !UNITY_ANDROID)
            GetSavePath();
#endif
            var gameData = Rm_GameConfig.Instance;
            if(gameData == null)
            {
                Debug.LogError("GameSettings are null.");
                return;
            }
            //Debug.Log("[RPGAIO] Saving settings ...");

            var saveFile = JsonConvert.SerializeObject(gameData.Controls, Formatting.Indented);

#if (UNITY_IOS || UNITY_ANDROID)
            PlayerPrefs.SetString(PlayerPrefsKey,saveFile);
            PlayerPrefs.SetInt(PlayerPrefsKey + "_MobileMasterVol", gameData.Audio.MasterVolume);
            PlayerPrefs.SetInt(PlayerPrefsKey + "_MobileMusicVol", gameData.Audio.MusicVolume);
            PlayerPrefs.SetInt(PlayerPrefsKey + "_MobileSFXVol", gameData.Audio.SoundEffectVolume);
            PlayerPrefs.SetInt(PlayerPrefsKey + "_MobileVoiceVol", gameData.Audio.VoiceVolume);
            PlayerPrefs.SetInt(PlayerPrefsKey + "_MobileAmbientVol", gameData.Audio.AmbientVolume);
#else
            Directory.CreateDirectory(_savePath);
            //Save Graphics + Audio + Game Settings + UI Settings to INI
            var parser = new FileIniDataParser();
            var data = new IniData();
            data.Sections.AddSection("Graphics");
            data["Graphics"].AddKey("FullScreen", (gameData.Graphics.FullScreen ? 1 : 0).ToString(CultureInfo.InvariantCulture));
            data["Graphics"].AddKey("ResolutionWidth", gameData.Graphics.Resolution.width.ToString(CultureInfo.InvariantCulture));
            data["Graphics"].AddKey("ResolutionHeight", gameData.Graphics.Resolution.height.ToString(CultureInfo.InvariantCulture));
            data["Graphics"].AddKey("ResolutionRefreshRate", gameData.Graphics.Resolution.refreshRate.ToString(CultureInfo.InvariantCulture));
            data["Graphics"].AddKey("QualityLevel", gameData.Graphics.QualityLevel.ToString(CultureInfo.InvariantCulture));
            data["Graphics"].AddKey("AnisotropicFiltering", ((int)gameData.Graphics.AnisotropicFiltering).ToString(CultureInfo.InvariantCulture));
            data["Graphics"].AddKey("TextureQuality", gameData.Graphics.TextureQuality.ToString(CultureInfo.InvariantCulture));
            data["Graphics"].AddKey("VSyncCount", gameData.Graphics.VSyncCount.ToString(CultureInfo.InvariantCulture));
            data["Graphics"].AddKey("ShadowType", ((int)gameData.Graphics.ShadowType).ToString(CultureInfo.InvariantCulture));

            data.Sections.AddSection("Audio");
            data["Audio"].AddKey("MasterVolume", gameData.Audio.MasterVolume.ToString(CultureInfo.InvariantCulture));
            data["Audio"].AddKey("MusicVolume", gameData.Audio.MusicVolume.ToString(CultureInfo.InvariantCulture));
            data["Audio"].AddKey("SfxVolume", gameData.Audio.SoundEffectVolume.ToString(CultureInfo.InvariantCulture));
            data["Audio"].AddKey("DialogVolume", gameData.Audio.VoiceVolume.ToString(CultureInfo.InvariantCulture));
            data["Audio"].AddKey("AmbientVolume", gameData.Audio.AmbientVolume.ToString(CultureInfo.InvariantCulture));
            data["Audio"].AddKey("SpeakerMode", ((int)gameData.Audio.SpeakerMode).ToString(CultureInfo.InvariantCulture));
            parser.WriteFile(_savePath + iniFileName,data);

            //Save control settings to cfg
            File.WriteAllText(_savePath + settingsFileName, saveFile);
#endif

            //Debug.Log("[RPGAIO] Saved settings");
        }

        public void LoadSettings()
        {
#if (!UNITY_IOS && !UNITY_ANDROID)

            GetSavePath();
#endif
            var foundData = false;
#if (UNITY_IOS || UNITY_ANDROID)
            var stringFromPlayerPrefs = PlayerPrefs.GetString(PlayerPrefsKey, "");
            if(!string.IsNullOrEmpty(stringFromPlayerPrefs))
            {
                var jsonFromEncode = stringFromPlayerPrefs;

#else
            if (File.Exists(_savePath + settingsFileName))
            {
                var jsonFromEncode = File.ReadAllText(_savePath + settingsFileName);
#endif
                if (!String.IsNullOrEmpty(jsonFromEncode))
                {
                    foundData = true;
                    try
                    {

                        var loadedGameData = JsonConvert.DeserializeObject<Rm_ControlSetup>(jsonFromEncode,
                                                                                          new JsonSerializerSettings
                                                                                              {
                                                                                                  TypeNameHandling =
                                                                                                      TypeNameHandling.
                                                                                                      Objects,
                                                                                                  ObjectCreationHandling
                                                                                                      =
                                                                                                      ObjectCreationHandling
                                                                                                      .Replace
                                                                                              });

                        Rm_GameConfig.Instance = new Rm_GameConfig
                                                     {
                                                         Controls = loadedGameData
                                                     };

#if (!UNITY_IOS && !UNITY_ANDROID)

                        var parser = new FileIniDataParser();
                        IniData data = parser.ReadFile(_savePath + iniFileName);

                        Rm_GameConfig.Instance.Graphics.FullScreen = Convert.ToBoolean(data["Graphics"]["Fullscreen"]);
                        Rm_GameConfig.Instance.Graphics.Resolution = new Resolution()
                                                                         {
                                                                             height = Convert.ToInt32(data["Graphics"]["ResolutionHeight"]),
                                                                             width = Convert.ToInt32(data["Graphics"]["ResolutionWidth"]),
                                                                             refreshRate = Convert.ToInt32(data["Graphics"]["ResolutionRefreshRate"]),
                                                                         };

                        Rm_GameConfig.Instance.Graphics.QualityLevel = Convert.ToInt32(data["Graphics"]["QualityLevel"]);
                        Rm_GameConfig.Instance.Graphics.AnisotropicFiltering = (AnisotropicFiltering)Convert.ToInt32(data["Graphics"]["AnisotropicFiltering"]);
                        Rm_GameConfig.Instance.Graphics.TextureQuality = Convert.ToInt32(data["Graphics"]["TextureQuality"]);
                        Rm_GameConfig.Instance.Graphics.VSyncCount = Convert.ToInt32(data["Graphics"]["VSyncCount"]);
                        Rm_GameConfig.Instance.Graphics.ShadowType = (LightShadows)Convert.ToInt32(data["Graphics"]["ShadowType"]);

                        Rm_GameConfig.Instance.Audio.MasterVolume = Convert.ToInt32(data["Audio"]["MasterVolume"]);
                        Rm_GameConfig.Instance.Audio.MusicVolume = Convert.ToInt32(data["Audio"]["MusicVolume"]);
                        Rm_GameConfig.Instance.Audio.SoundEffectVolume = Convert.ToInt32(data["Audio"]["SfxVolume"]);
                        Rm_GameConfig.Instance.Audio.VoiceVolume = Convert.ToInt32(data["Audio"]["DialogVolume"]);
                        Rm_GameConfig.Instance.Audio.AmbientVolume = Convert.ToInt32(data["Audio"]["AmbientVolume"]);
                        Rm_GameConfig.Instance.Audio.SpeakerMode = (AudioSpeakerMode)Convert.ToInt32(data["Audio"]["SpeakerMode"]); 
#else
                        Rm_GameConfig.Instance.Audio.MasterVolume = PlayerPrefs.GetInt(PlayerPrefsKey + "_MobileMasterVol", 20);
                        Rm_GameConfig.Instance.Audio.MusicVolume = PlayerPrefs.GetInt(PlayerPrefsKey + "_MobileMusicVol", 100);
                        Rm_GameConfig.Instance.Audio.SoundEffectVolume = PlayerPrefs.GetInt(PlayerPrefsKey + "_MobileSFXVol", 100);
                        Rm_GameConfig.Instance.Audio.VoiceVolume = PlayerPrefs.GetInt(PlayerPrefsKey + "_MobileVoiceVol", 100);
                        Rm_GameConfig.Instance.Audio.AmbientVolume = PlayerPrefs.GetInt(PlayerPrefsKey + "_MobileAmbientVol", 100);

#endif
                        Rm_GameConfig.Instance.ApplySettings();

                    }
                    catch(Exception e)
                    {
                        foundData = false;
                    }
                }
            }

            if(!foundData)
            {
                Debug.Log("No settings file found... creating a new one.");
                NewData();
                LoadSettings();
            }

            //Debug.Log("[EDITOR] Loaded settings...");
        }

        public void NewData()
        {
            Rm_GameConfig.Instance = new Rm_GameConfig
                                         {
                                             Audio = GeneralMethods.CopyObject(Rm_RPGHandler.Instance.DefaultSettings.DefaultAudio),
                                             Controls = GeneralMethods.CopyObject(Rm_RPGHandler.Instance.DefaultSettings.DefaultControls),
                                             Graphics = GeneralMethods.CopyObject(Rm_RPGHandler.Instance.DefaultSettings.DefaultGraphics)
                                         };
            Rm_GameConfig.Instance.ApplySettings();     
            SaveSettings();
        }
    }
}
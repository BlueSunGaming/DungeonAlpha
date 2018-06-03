using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class Rm_GameConfig
    {
        [JsonIgnore] private static Rm_GameConfig _myInstance;
        [JsonIgnore]
        public static Rm_GameConfig Instance
        {
            get { return _myInstance ?? (_myInstance = new Rm_GameConfig()); }  
            set { _myInstance = value; }
        }

        public Rm_GraphicsSetup Graphics;
        public Rm_AudioSetup Audio;
        public Rm_ControlSetup Controls;

        public Rm_GameConfig()
        {
            Graphics = new Rm_GraphicsSetup();
            Audio = new Rm_AudioSetup();
            Controls = new Rm_ControlSetup();
        }

        public void ResetAllToDefault()
        {
            DefaultAudio();
            DefaultControls();
            DefaultGraphics();
        }

        public void DefaultGraphics()
        {
            Graphics = GeneralMethods.CopyObject(Rm_RPGHandler.Instance.DefaultSettings.DefaultGraphics);
            ApplySettings();
        }
        public void DefaultAudio()
        {
            Audio = GeneralMethods.CopyObject(Rm_RPGHandler.Instance.DefaultSettings.DefaultAudio);
            ApplySettings();
        }
        public void DefaultControls()
        {
            Controls = GeneralMethods.CopyObject(Rm_RPGHandler.Instance.DefaultSettings.DefaultControls);
            ApplySettings();
        }

        public void ApplySettings()
        {

            #if (!UNITY_IOS && !UNITY_ANDROID)

            Screen.SetResolution(Graphics.Resolution.width,Graphics.Resolution.height,
                Graphics.FullScreen,Graphics.Resolution.refreshRate);
            QualitySettings.SetQualityLevel(Graphics.QualityLevel);
            QualitySettings.anisotropicFiltering = Graphics.AnisotropicFiltering;
            QualitySettings.antiAliasing = Graphics.AntialiasingLevel;
            QualitySettings.masterTextureLimit = Graphics.TextureQuality;
            QualitySettings.vSyncCount = Graphics.VSyncCount;
            var shadowSources = GameObject.FindGameObjectsWithTag("ShadowSource");
            foreach(var shadowSource in shadowSources)
            {
                var light = shadowSource.GetComponent<Light>();
                light.shadows = Graphics.ShadowType;
            }
#else

            #endif
        }
    }
}
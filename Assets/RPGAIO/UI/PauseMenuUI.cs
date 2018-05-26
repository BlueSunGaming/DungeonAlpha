using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Beta.NewImplementation;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    public GameObject DefaultMenu;
    public GameObject LoadMenu;
    public GameObject OptionsMenu;
    public GameObject ConfirmExit;
    public bool ExitingToMenu = true;
    public bool Showing = false;

    public GameObject SaveContainer;
    public GameObject SaveSelectButton;
    public Text SaveFullInfo;

    public Text ExitToDesktopButton;
    public Text LoadGameButton;
    public List<GameObject> NonMobileOptions;

    public SliderOptionModel MasterVolume;
    public SliderOptionModel MusicVolume;
    public SliderOptionModel SfxVolume;
    public SliderOptionModel DialogueVolume;
    public SliderOptionModel AmbientVolume;

    public ListOptionModel Resolution;
    public ListOptionModel QualityLevel;
    public ListOptionModel TextureQuality;
    public ListOptionModel Shadows;

    public ToggleOptionModel FullScreen;
    public ToggleOptionModel AnisotropicFiltering;
    public ToggleOptionModel VSync;

    void Awake()
    {
#if (UNITY_IOS || UNITY_ANDROID)
        ExitToDesktopButton.text = "Exit Game";
        LoadGameButton.text = "Load Last Save";

        foreach(var g in NonMobileOptions)
        {
            g.SetActive(false);
        }
#endif
    }

    public void BackToDefaultMenu()
    {
        LoadMenu.SetActive(false);
        OptionsMenu.SetActive(false);
        ConfirmExit.SetActive(false);
        DefaultMenu.SetActive(true);
    }

    public void CloseMenu()
    {
        UIHandler.Instance.ToggleMenu(null,null);
    }

#region "Default Menu"
    public void SaveButton()
    {
        RPG.Save();
        CloseMenu();
    }

    public void LoadButton()
    {
        DefaultMenu.SetActive(false);
#if (UNITY_IOS || UNITY_ANDROID)
        var save = PlayerSaveLoadManager.Instance.RecentSave();
        PlayerSaveLoadManager.Instance.LoadPlayerWithForcedReload(save, save.SavePath, false);
#else
        SaveContainer.transform.DestroyChildren();
        var playerSaves = RPG.PlayerSaves;
        var saveNum = playerSaves.Count;
        foreach (var save in playerSaves.OrderByDescending(d => d.LastSaved))
        {
            var go = Instantiate(SaveSelectButton, Vector3.zero, Quaternion.identity) as GameObject;
            go.transform.SetParent(SaveContainer.transform, false);
            var saveSelect = go.GetComponent<SaveSelectModel>();
            saveSelect.Save = save;
            saveSelect.SavePath = save.SavePath;
            saveSelect.FullInfoRef = SaveFullInfo;
            var timePlayed = save.TimePlayed;
            saveSelect.ButtonText.text = saveNum.ToString("000") + "\t\t\t" + save.CurrentScene + " " + string.Format("[{0}h{1}m]", timePlayed.Hours, timePlayed.Minutes);
            saveNum--;
        }

        LoadMenu.SetActive(true);
#endif

    }

    public void OptionsButton()
    {
        DefaultMenu.SetActive(false);
        SetOptionMenuValues();
        OptionsMenu.SetActive(true);
    }

    public void MainMenuButton()
    {
        DefaultMenu.SetActive(false);
        ConfirmExit.SetActive(true);
        ExitingToMenu = true;
    }

    public void ExitButton()
    {
        DefaultMenu.SetActive(false);
        ConfirmExit.SetActive(true);
        ExitingToMenu = false;
    }
#endregion

#region "Load Menu"
#endregion

#region "Options Menu"

    private void SetOptionMenuValues()
    {
        var settings = RPG.GameSettings;
    
        //Audio
        MasterVolume.Slider.value = settings.Audio.MasterVolume;
        MusicVolume.Slider.value = settings.Audio.MusicVolume;
        SfxVolume.Slider.value = settings.Audio.SoundEffectVolume;
        DialogueVolume.Slider.value = settings.Audio.VoiceVolume;
        AmbientVolume.Slider.value = settings.Audio.AmbientVolume;

        //Graphics

        /*public ListOptionModel Resolution;
    public ListOptionModel QualityLevel;
    public ListOptionModel TextureQuality;
    public ListOptionModel Shadows;

    public ToggleOptionModel FullScreen;
    public ToggleOptionModel AnisotropicFiltering;
    public ToggleOptionModel VSync;*/

        var selectedQuality = settings.Graphics.QualityLevel;
        var selectableQualities = QualitySettings.names.ToList();
        QualityLevel.Set(selectedQuality, selectableQualities);

#if (!UNITY_IOS && !UNITY_ANDROID)

        var selectedRes = settings.Graphics.Resolution;
        var selectedResIndex = Array.IndexOf(Screen.resolutions, selectedRes);
        selectedResIndex = selectedResIndex == -1 ? 0 : selectedResIndex;
        var selectableResolutions = Screen.resolutions.Select(r => r.width + "x" + r.height + "@" + r.refreshRate + "hz").ToList();
        Resolution.Set(selectedResIndex, selectableResolutions);



        var selectedTexQual = settings.Graphics.TextureQuality;
        var selectableTexQualities = new string[] { "Max", "High","Mid","Low"}.ToList();
        TextureQuality.Set(selectedTexQual, selectableTexQualities);

        var selectedShadows = settings.Graphics.ShadowType;
        var selectedShadowInt = 0;
        switch (selectedShadows)
        {
            case LightShadows.None:
                selectedShadowInt = 0;
                break;
            case LightShadows.Hard:
                selectedShadowInt = 1;
                break;
            case LightShadows.Soft:
                selectedShadowInt = 2;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        var selectableShadows = new string[] { "None", "Hard","Soft"}.ToList();
        Shadows.Set(selectedShadowInt, selectableShadows);

        FullScreen.Toggle.isOn = settings.Graphics.FullScreen;
        AnisotropicFiltering.Toggle.isOn = settings.Graphics.AnisotropicFiltering != UnityEngine.AnisotropicFiltering.Disable;
        VSync.Toggle.isOn = settings.Graphics.VSyncCount > 0;

#endif
    }

    public void ApplyDefaults()
    {
        Rm_GameConfig.Instance.ResetAllToDefault();
        Rm_GameConfig.Instance.ApplySettings();
        GameSettingsSaveLoadManager.Instance.SaveSettings();
        SetOptionMenuValues();
    }

    public void ApplySettings()
    {
        ApplyValuesToSettings();
        Rm_GameConfig.Instance.ApplySettings();
        GameSettingsSaveLoadManager.Instance.SaveSettings();
    }

    private void ApplyValuesToSettings()
    {
        var settings = RPG.GameSettings;

        //Audio
        settings.Audio.MasterVolume = (int)MasterVolume.Slider.value;
        settings.Audio.MusicVolume = (int)MusicVolume.Slider.value;
        settings.Audio.SoundEffectVolume = (int)SfxVolume.Slider.value;
        settings.Audio.VoiceVolume = (int)DialogueVolume.Slider.value;
        settings.Audio.AmbientVolume = (int)AmbientVolume.Slider.value;

        //Graphics
        settings.Graphics.Resolution = Screen.resolutions[Resolution.SelectedOption];
        settings.Graphics.QualityLevel = QualityLevel.SelectedOption;
        settings.Graphics.TextureQuality = TextureQuality.SelectedOption;
        switch(Shadows.SelectedOption)
        {
            case 0:
                settings.Graphics.ShadowType = LightShadows.None;
                break;
            case 1:
                settings.Graphics.ShadowType = LightShadows.Hard;
                break;
            case 2:
                settings.Graphics.ShadowType = LightShadows.Soft;
                break;
        }

        settings.Graphics.FullScreen = FullScreen.Toggle.isOn;
        settings.Graphics.AnisotropicFiltering = AnisotropicFiltering.Toggle.isOn ? UnityEngine.AnisotropicFiltering.Enable : UnityEngine.AnisotropicFiltering.Disable;
        settings.Graphics.VSyncCount = VSync.Toggle.isOn ? 1 : 0;

        settings.ApplySettings();
        GameSettingsSaveLoadManager.Instance.SaveSettings();
    }

    #endregion

#region "Confirm Exit Menu"
    public void Confirm()
    {
        if(ExitingToMenu)
        {
            CloseMenu();
            ConfirmExitToMainMenu();
        }
        else
        {
            CloseMenu();
            ConfirmExitToDesktop();
        }
    }

    public void ConfirmExitToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
    public void ConfirmExitToDesktop()
    {
        Application.Quit();
    }
#endregion

    void OnDisable()
    {
        CloseMenu();
    }
}

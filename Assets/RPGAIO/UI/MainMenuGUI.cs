using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuGUI : MonoBehaviour
{
    public static MainMenuGUI Instance;
    public Button ContinueButton;
    public Button LoadButton;
    public MainMenuMode MainMenuMode = MainMenuMode.Home;
    private float creditsYPos;
    public PlayerSave RecentSave = null;
    public CreditsHandler CreditsHandler;
    public Text GameTitle;
    public Text GameCompany;
    public GameObject SaveSection;
    public GameObject SaveContainer;
    public List<PlayerSave> PlayerSaves;
    public GameObject SaveSelectButton;
    public Text SaveFullInfo;
    public AudioClip ClickSound;

    void Start()
    {
        Instance = this;
        CreditsHandler = GetComponent<CreditsHandler>();
        SaveSection.SetActive(false);
        creditsYPos = Screen.height/2.0f;
        GameTitle.text = Rm_RPGHandler.Instance.GameInfo.GameTitle;
        GameCompany.text = Rm_RPGHandler.Instance.GameInfo.GameCompany;
        RecentSave = PlayerSaveLoadManager.Instance.RecentSave();
        ContinueButton.gameObject.SetActive(RecentSave != null);

#if (!UNITY_STANDALONE)
        Destroy(LoadButton.gameObject);
#endif

        var themeObj = GameObject.Find("MainMenuTheme");
        if (themeObj != null)
        {
            themeObj.AddComponent<Audio_BgMusic>();
        }
    }
    
    void Update()
    {
        if(MainMenuMode == MainMenuMode.Credits)
        {
            if(Input.GetKeyDown(KeyCode.Escape) || Input.touchCount == 1)
            {
                CreditsHandler.End();
            }
        }
    }

    private void PlayClickSound()
    {
        var g = new GameObject();
        var audioS = g.AddComponent<AudioSource>();
        g.transform.position = Camera.main.transform.position;
        audioS.clip = ClickSound;
        audioS.Play();
        audioS.volume = 0.5f;
        Destroy(g, ClickSound.length + 0.1f);
    }

    public void ContinueClick()
    {
        PlayClickSound();
        RPG.LoadLevel(RecentSave.CurrentScene, false);
    }

    public void NewClick()
    {
        PlayClickSound();
        SceneManager.LoadScene("CharCreate");
    }

    public void LoadClick()
    {
        PlayClickSound();
        if(MainMenuMode != MainMenuMode.Load)
        {
            LoadSaves();
            MainMenuMode = MainMenuMode.Load;
        }
        else
        {
            SaveSection.SetActive(false);
            MainMenuMode = MainMenuMode.Home;
        }
    }

    public void LoadSaves()
    {
        SaveContainer.transform.DestroyChildren();
        PlayerSaves = RPG.PlayerSaves;
        var saveNum = PlayerSaves.Count;
        foreach (var save in PlayerSaves.OrderByDescending(d => d.LastSaved))
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

        SaveSection.SetActive(true);
    }

    public void CreditsClick()
    {
        MainMenuMode = MainMenuMode.Credits;
        CreditsHandler.Begin();
        PlayClickSound();
    }

    public void QuitClick()
    {
        PlayClickSound();
        Application.Quit();
    }
}

public enum MainMenuMode
{
    Home,
    Load,
    Credits
}

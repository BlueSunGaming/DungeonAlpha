using System.IO;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeleteCharacterButtonModel : MonoBehaviour
{
    public static DeleteCharacterButtonModel Instance;
    public PlayerSave SelectedSave;
    public Text FullInfoRef;

    void Awake()
    {
        Instance = this;
    }

    public void DeleteSelectedSave()
    {
        File.Delete(SelectedSave.SavePath);
        FullInfoRef.text = "";
        SelectedSave = null;
        MainMenuGUI.Instance.LoadSaves();
    }
}
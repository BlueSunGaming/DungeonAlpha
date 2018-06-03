using LogicSpawn.RPGMaker.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSelectModel : MonoBehaviour, IPointerClickHandler, IPointerExitHandler
{
    public Text ButtonText;
    public PlayerSave Save;
    public string SavePath;

    public Text FullInfoRef;
    private bool ClickedOnce;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!ClickedOnce)
        {
            ClickedOnce = true;
            if(DeleteCharacterButtonModel.Instance != null)
            {
                DeleteCharacterButtonModel.Instance.SelectedSave = Save;    
            }
            FullInfoRef.text = "";
            FullInfoRef.text += "Load:\n";
            FullInfoRef.text += Save.Character.Name + ", Level " + Save.Character.Level + "\n";
            FullInfoRef.text += Save.LastSaved.ToShortDateString() + ", " + Save.LastSaved.ToShortTimeString();
        }
        else
        {
            var loadedScene = SceneManager.GetActiveScene().name;
            if (loadedScene == "MainMenu")
            {
                PlayerSaveLoadManager.Instance.LoadPlayer(Save, SavePath, false);
            }
            else
            {
                PlayerSaveLoadManager.Instance.LoadPlayerWithForcedReload(Save, SavePath, false);        
            }
            
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ClickedOnce = false;
    }
}
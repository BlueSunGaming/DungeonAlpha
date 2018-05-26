using System.Linq;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LocationModel : MonoBehaviour
{
    public Button Button;
    public Text LocationName;
    public Image LocationIcon;
    public Image LocationBackgroundImage;
    public string WorldAreaID;
    public string LocationID;

    public void Init(string worldAreaID, string locationID)
    {
        WorldAreaID = worldAreaID;
        LocationID = locationID;

        Button.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        if(!Rm_RPGHandler.Instance.DefaultSettings.CanFastTravelOnMap) return;

        var worldArea = Rm_RPGHandler.Instance.Customise.WorldMapLocations.FirstOrDefault(w => w.ID == WorldAreaID);
        var location = worldArea.Locations.FirstOrDefault(w => w.ID == LocationID);
        GameMaster.Instance.LoadLevel(location.SceneName, true, true, worldArea, location);
        Debug.Log("Clicked on " + location.Name);
    }
}

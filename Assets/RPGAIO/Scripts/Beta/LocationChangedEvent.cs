using System.Linq;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

public class LocationChangedEvent : MonoBehaviour {

    private bool _played = false;
    public string LocationName;
	// Use this for initialization
    void OnTriggerEnter()
    {
        var location =
            Rm_RPGHandler.Instance.Customise.WorldMapLocations.SelectMany(n => n.Locations).FirstOrDefault(
                l => l.Name == LocationName);
            
        if(location != null)
        {
            //var worldArea = Rm_RPGHandler.Instance.Customise.WorldMapLocations.FirstOrDefault(w => w.ID == location.WorldAreaID);

            if (GetObject.PlayerSave.WorldMap.CurrentLocationID != location.ID)
            {
                WorldMapUI.Instance.ShowLocation(location.Name);    
            }
            
            GetObject.PlayerSave.WorldMap.CurrentWorldAreaID = location.WorldAreaID;
            GetObject.PlayerSave.WorldMap.CurrentLocationID = location.ID;            
        }
    }
}

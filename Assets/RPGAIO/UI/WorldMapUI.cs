using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class WorldMapUI : MonoBehaviour
{
    public static WorldMapUI Instance;
    public GameObject WorldMapPanel;
    public GameObject MapLocationHolder;
    public GameObject MapLocation;
    public GameObject EnteredAreaObject;
    public Text WorldAreaLabel;
    public WorldArea CurrentWorldArea;
    public Color LocationBackgroundColor = new Color(0.8f, 0.8f, 0.8f, 0.5f);
    public Color CurrentLocationBackgroundColor= new Color(0.3f, 0.6f, 0.9f, 0.5f);

    public Location CurrentLocation
    {
        get
        {
            var id = GetObject.PlayerSave.WorldMap.CurrentWorldAreaID;
            var worldArea = CurrentWorldArea;
            if(worldArea != null)
            {
                var location = worldArea.Locations.FirstOrDefault(l => l.ID == id);
                return location;
            }

            return null;
        }
    }

    private bool _show;



	// Use this for initialization
	void Awake () {
	    Instance = this;
	}

    void Start()
    {
        var id = GetObject.PlayerSave.WorldMap.CurrentWorldAreaID;
        var worldArea = Rm_RPGHandler.Instance.Customise.WorldMapLocations.FirstOrDefault(w => w.ID == id);
        CurrentWorldArea =  worldArea;
        EnteredAreaObject.SetActive(false);
        WorldMapPanel.SetActive(false);
        _show = false;
    }

    public void Init()
    {
        var worldArea = CurrentWorldArea;
        if (worldArea == null) return;

        WorldAreaLabel.text = worldArea.Name;

        //Clear previous UI elements
        MapLocationHolder.transform.DestroyChildren();

        var worldMapPanelImage = worldArea.ImageContainer.Image;
        WorldMapPanel.GetComponent<Image>().sprite = Sprite.Create(worldMapPanelImage, new Rect(0, 0, worldMapPanelImage.width, worldMapPanelImage.height), Vector2.zero);    

        //Add new UI elements
        foreach(var location in worldArea.Locations)
        {
            var go = Instantiate(MapLocation, Vector3.zero, Quaternion.identity) as GameObject;
            var model = go.GetComponent<LocationModel>();
            model.Init(worldArea.ID, location.ID);

            model.LocationName.text = location.Name;

            model.LocationBackgroundImage.color = GetObject.PlayerSave.WorldMap.CurrentLocationID == location.ID 
                ? CurrentLocationBackgroundColor 
                : LocationBackgroundColor;

            var locImage = location.ImageContainer.Image;
            if(locImage != null)
            {
                model.LocationIcon.sprite = Sprite.Create(locImage, new Rect(0, 0, locImage.width, locImage.height), Vector2.zero);    
            }

            var worldAreaTree = Rm_RPGHandler.Instance.Nodes.WorldMapNodeBank.NodeTrees.FirstOrDefault(n => n.ID == worldArea.ID);
            var locNode = worldAreaTree.Nodes.FirstOrDefault(n => n.ID == location.ID);
            go.transform.SetParent(MapLocationHolder.transform, false);
            go.transform.position = new Vector3(locNode.Rect.x, locNode.Rect.y * -1 + Screen.height);
        }

    }

    public void ToggleMap()
    {
        _show = !_show;
        WorldMapPanel.SetActive(_show);
        if(_show)
        {
            CurrentWorldArea = Rm_RPGHandler.Instance.Customise.WorldMapLocations.FirstOrDefault(w => w.ID == GetObject.PlayerSave.WorldMap.CurrentWorldAreaID);
            Init();
        }
    }

    public void SwitchWorldArea(bool forward)
    {
        var worldAreas = Rm_RPGHandler.Instance.Customise.WorldMapLocations;
        var curIndex = worldAreas.IndexOf(CurrentWorldArea);

        if(forward)
        {
            curIndex += 1;
            curIndex = curIndex > worldAreas.Count - 1 ? 0 : curIndex;
        }
        else
        {
            curIndex -= 1;
            curIndex = curIndex < 0 ? worldAreas.Count - 1 : curIndex;
        }

        CurrentWorldArea = worldAreas[curIndex];
        Init();
    }

    public IEnumerator ShowEnteredLocation(string location)
    {
        EnteredAreaObject.GetComponent<Text>().text = location;
        EnteredAreaObject.SetActive(true);
        
        yield return new WaitForSeconds(3.0f);
        EnteredAreaObject.SetActive(false);
    }

    public void ShowLocation(string s)
    {
        StartCoroutine(ShowEnteredLocation(s));
    }
}

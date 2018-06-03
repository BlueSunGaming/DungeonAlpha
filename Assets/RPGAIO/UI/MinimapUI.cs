using System;
using Assets.Scripts.Core.Interaction;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class MinimapUI : MonoBehaviour
{
    public static MinimapUI Instance;
    public bool Show;

    private EventSystem EventSystem
    {
        get { return UIHandler.Instance.EventSystem; }
    }

    // Use this for initialization
    void Awake()
    {
        Instance = this;
    }
    
    public void ZoomIn()
    {
        GetObject.RPGMinimapCamera.ZoomIn();
        RPG.Events.OnMinimapZoom();
    }
    public void ZoomOut()
    {
        GetObject.RPGMinimapCamera.ZoomOut();
        RPG.Events.OnMinimapZoom();
    }
}
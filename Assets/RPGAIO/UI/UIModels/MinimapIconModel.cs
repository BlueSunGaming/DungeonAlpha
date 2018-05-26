using System;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapIconModel : MonoBehaviour
{
    public RectTransform rectTransform;
    public MinimapIconType Type;
    public Image Image;
    public bool RotateWithObject;

    void OnEnable()
    {
        RPG.Events.MinimapZoom += OnZoom;
    }

    void OnDisable()
    {
        RPG.Events.MinimapZoom -= OnZoom;
    }

    void Start()
    {
        var icons = Rm_RPGHandler.Instance.GameInfo.MinimapOptions;
        switch(Type)
        {
            case MinimapIconType.Player:
                GetComponent<Canvas>().sortingOrder = 1000;
                Image.sprite = icons.PlayerSprite;
                break;
            case MinimapIconType.NPC:
                Image.sprite = icons.NpcSprite;
                break;
            case MinimapIconType.Enemy:
                Image.sprite = icons.EnemySprite;
                break;
            case MinimapIconType.Interactable:
                Image.sprite = icons.InteractSprite;
                break;
            case MinimapIconType.Harvestable:
                Image.sprite = icons.HarvestSprite;
                break;
            case MinimapIconType.Custom:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        rectTransform.position += new Vector3(0, 100, 0);
    }

    void Update()
    {
        var mcam = GetObject.RPGMinimapCamera;
        if(mcam != null && !RotateWithObject)
        {
            transform.rotation = mcam.transform.rotation;
        }
    }

    private void OnZoom(object sender, EventArgs e)
    {
        rectTransform.sizeDelta 
            = new Vector2(GetObject.RPGMinimapCamera.IconScale, GetObject.RPGMinimapCamera.IconScale);
    }
}
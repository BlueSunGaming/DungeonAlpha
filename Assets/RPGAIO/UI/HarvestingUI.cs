using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Core.Interaction;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Core;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HarvestingUI : MonoBehaviour
{
    public static HarvestingUI Instance;
    public InteractableHarvestable CurrentHarvestable;
    public GameObject HarvestingPanel;
    public Image HarvestProgressBar;
    public Text HarvestInfo;
    public bool Harvesting;

	// Use this for initialization
    void Awake()
    {
        Instance = this;

    }

    void OnEnable()
    {
        RPG.Events.StartHarvesting += StartHarvesting;
        RPG.Events.StopHarvesting += StopHarvest;
    }
    void OnDisable()
    {
        RPG.Events.StartHarvesting -= StartHarvesting;
        RPG.Events.StopHarvesting -= StopHarvest;
    }

    private void StartHarvesting(object sender, RPGEvents.StartHarvestingEventArgs e)
    {
        CurrentHarvestable = e.Harvestable;
        HarvestInfo.text = CurrentHarvestable.HarvestedItem.Name + " - " + CurrentHarvestable.MaterialsRemaining + " remaining";
        Harvesting = true;
    }

    private void StopHarvest(object sender, RPGEvents.StopHarvestingEventArgs e)
    {
        StopHarvesting();
    }

    void Update()
    {
        HarvestingPanel.SetActive(Harvesting);
        if (Harvesting && CurrentHarvestable != null && CurrentHarvestable.MaterialsRemaining > 0 &&  HarvestProgressBar.fillAmount < 0.99f)
        {
            HarvestInfo.text = CurrentHarvestable.HarvestedItem.Name + " - " + CurrentHarvestable.MaterialsRemaining + " remaining";
            var harvestTime = CurrentHarvestable.HarvestTime;
            HarvestProgressBar.fillAmount = CurrentHarvestable.Harvestable.CurrentHarvestTime / CurrentHarvestable.Harvestable.TimeToHarvest;

            if (HarvestProgressBar.fillAmount >= 0.99f)
            {
                HarvestProgressBar.fillAmount = 0;
                if(CurrentHarvestable.Harvestable.MaterialsRemaining <= 0)
                {
                    Harvesting = false;
                }
            }
        }
    }

    public void StopHarvesting()
    {
        if(Harvesting)
        {
            Harvesting = false;
            HarvestProgressBar.fillAmount = 0;
            if(CurrentHarvestable != null)
            {
                CurrentHarvestable.StopInteraction();
                CurrentHarvestable = null;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PetUI : MonoBehaviour
{
    public static PetUI Instance;
    public Text PetName;
    public Image PetHp;
    public Image PassiveSelected;
    public Image AggresiveSelected;
    public bool Show;

    private EventSystem EventSystem
    {
        get { return UIHandler.Instance.EventSystem; }
    }

	// Use this for initialization
	void Awake () {
	    Instance = this;
	}
    
    void Update()
    {
        var curPet = GetObject.PlayerCharacter.CurrentPet;

        if(Show)
        {
            if (curPet != null)
            {
                try
                {
                    PetName.text = curPet.PetData.Name;
                    var petHp = curPet.Controller.Character.VitalHandler.Health;
                    var fillAmount = (float)petHp.CurrentValue / petHp.MaxValue;
                    PetHp.fillAmount = fillAmount;
                    PassiveSelected.gameObject.SetActive(curPet.PetData.CurrentBehaviour == PetBehaviour.Assist);
                    AggresiveSelected.gameObject.SetActive(curPet.PetData.CurrentBehaviour == PetBehaviour.Aggresive);
                }
                catch(Exception e){}
            }
        }
    }

    public void SwitchToPassive()
    {
        GetObject.PlayerCharacter.CurrentPet.PetData.CurrentBehaviour = PetBehaviour.Assist;
    }

    public void SwitchToAggresive()
    {
        GetObject.PlayerCharacter.CurrentPet.PetData.CurrentBehaviour = PetBehaviour.Aggresive;
    }
}

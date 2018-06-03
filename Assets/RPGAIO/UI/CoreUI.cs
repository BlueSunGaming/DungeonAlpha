using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Beta.NewImplementation;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CoreUI : MonoBehaviour
{
    public CharacterPortraitModel CharacterPortrait;
    public List<CoreVitalModel> Vitals;
    public CorePlayerModel PlayerModel;
    private Rm_ClassDefinition _classDefinition;

    public void Init()
    {
        var playerChar = GetObject.PlayerCharacter;
        _classDefinition = Rm_RPGHandler.Instance.Player.CharacterDefinitions.First(c => c.ID == playerChar.PlayerCharacterID);
        if(_classDefinition.Image != null)
        {
            if(CharacterPortrait.Portrait != null)
                CharacterPortrait.Portrait.sprite = GeneralMethods.CreateSprite(_classDefinition.Image);    
        }
        else
        {
            if(CharacterPortrait != null)
                Destroy(CharacterPortrait.gameObject);
        }
    }

	// Update is called once per frame
	void Update ()
	{
	    var player = GetObject.PlayerMono;
	    var playerChar = GetObject.PlayerCharacter;

        if (player == null) return;

	    foreach (var vitalModel in Vitals)
	    {
            var vitalName = vitalModel.VitalName;
            var vitalBar = vitalModel.VitalBar;
            var vitalText = vitalModel.VitalText;

	        var vital = player.Character.GetVital(vitalName);
            vitalBar.fillAmount = (float)vital.CurrentValue / vital.MaxValue;
            vitalText.text = vitalName + ": " + vital.CurrentValue + "/" + vital.MaxValue;
	    }

	    var exp = playerChar.Exp;
	    var expToLevel = playerChar.ExpToLevel;
	    PlayerModel.ExpBar.fillAmount = (float) exp/expToLevel;
	    var playerName = string.IsNullOrEmpty(playerChar.Name) ? "" : playerChar.Name + " - ";
        PlayerModel.NameAndLevel.text = playerName + "Level " + playerChar.Level;
	}
}

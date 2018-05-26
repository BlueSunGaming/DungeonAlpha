using System.Linq;
using Assets.Scripts.Beta.NewImplementation;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class QuestTrackerUI : MonoBehaviour
{
    public GameObject Title;
    public GameObject TrackerSection;
    public bool ShowTrackerSection;
    public Text TrackerText;
    public int QuestTitleSize = 21;
    public string QuestTitleColor = "Orange";
    public string QuestConditionColor = "White";
    private float timePassed;

    public void Awake()
    {
        Title.SetActive(false);
        TrackerText.gameObject.SetActive(false);
    }

    public void ToggleTrackerSection()
    {
        ShowTrackerSection = !ShowTrackerSection;
        TrackerSection.SetActive(ShowTrackerSection);
    }

	// Update is called once per frame
	void Update ()
	{
	    var trackedQuests = RPG.GetPlayerSave.QuestLog.ActiveObjectives.Where(a => a.TrackSteps).ToList();
	    timePassed += Time.deltaTime;
        if(timePassed > 0.2f)
        {
            timePassed = 0;
            var text = "";

            foreach (var quest in trackedQuests)
            {
                text += "<color=" + QuestTitleColor + "><size=" + QuestTitleSize + ">" + quest.Name + "</size></color>";
                text += "<color=" + QuestConditionColor + ">" + quest.TrackerDetails(true, true) + "</color>";
                text += "\n";
                text += "\n";
            }

            TrackerText.text = text;
        }
        Title.SetActive(trackedQuests.Any());
	    TrackerText.gameObject.SetActive(trackedQuests.Any());
	}
}

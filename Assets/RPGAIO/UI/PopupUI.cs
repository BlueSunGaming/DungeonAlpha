using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Core.Interaction;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PopupUI : MonoBehaviour
{
    public static PopupUI Instance;
    public GameObject PopupPanel;
    public GameObject PopupPrefab;

    public bool ShowLevelUp = true;
    public bool ShowExpGain = true;
    public bool ShowTraitLevelUps = true;
    public bool ShowResourceAlerts = true;

    private Dictionary<GameObject,RPG.PopupInfo> _popups = new Dictionary<GameObject, RPG.PopupInfo>();
	// Use this for initialization
    void Awake()
    {
        Instance = this;
        PopupPanel.transform.DestroyChildren();
    }

    void Update()
    {
        for (int index = 0; index < RPG.Popup.PopupQueue.Count; index++)
        {
            var popup = RPG.Popup.PopupQueue[index];
            if (!popup.Shown)
            {
                popup.Shown = true;
                if (!ShowLevelUp && popup.Text.Contains("Reached")) continue;
                if (!ShowExpGain && popup.Text.Contains("Exp")) continue;
                if (!ShowTraitLevelUps && popup.Text.Contains("Achieved")) continue;
                if (!ShowResourceAlerts && popup.Text.Contains("Not enough")) continue;

                var go = Instantiate(PopupPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                go.transform.SetParent(PopupPanel.transform, false);
                go.transform.SetAsFirstSibling();
                go.GetComponent<Text>().text = popup.Text;
                _popups.Add(go, popup);
            }

            RPG.Popup.PopupQueue.RemoveAt(index);
            index--;
        }

        var keys = new List<GameObject>(_popups.Keys);
        foreach (var key in keys)
        {
            var go = key;
            var popup = _popups[go];
            popup.ShowTime -= Time.deltaTime;
            if(popup.ShowTime <= 0)
            {
                _popups.Remove(go);
                Destroy(go);
            }
        }
    }
}

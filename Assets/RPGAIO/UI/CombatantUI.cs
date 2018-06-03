using Assets.Scripts.Beta.NewImplementation;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CombatantUI : MonoBehaviour
{

    public RPGController Controller;
    public GameObject Canvas;
    public Image HealthBar;
    public Text HealthText;

    void Start()
    {
        Controller = GetComponent<RPGController>();
    }

    void OnEnable()
    {
        Controller = GetComponent<RPGController>();
    }

	// Update is called once per frame
	void Update () {
        if(Canvas == null)
        {
            return;
        }

	    Canvas.transform.rotation = GetObject.RPGCamera.transform.rotation;

	    if(Controller.Character.Alive)
	    {
            if(Controller.InCombat)
            {
                Canvas.SetActive(true);
                var health = Controller.Character.VitalHandler.Health;
                HealthBar.fillAmount = (float)health.CurrentValue / health.MaxValue;

                if(Controller.Target == GetObject.PlayerMonoGameObject.transform)
                {
                    HealthText.text = health.CurrentValue.ToString();
                }
                else
                {
                    HealthText.text = "";
                }
            }
            else
            {
                Canvas.SetActive(false);
                HealthText.text = "";
            }
        }
        else
	    {
	        Canvas.GetComponent<CanvasGroup>().alpha -= 1.0f *Time.deltaTime;
            HealthBar.fillAmount = 0;
            HealthText.text = "0";
        }

        if(Canvas.activeInHierarchy)
        {
            Canvas.SetActive(GameMaster.ShowUI);
        }
    }
}

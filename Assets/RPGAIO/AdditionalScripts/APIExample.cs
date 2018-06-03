using System.Collections;
using LogicSpawn.RPGMaker.API;
using UnityEngine;

public class APIExample : MonoBehaviour {

    private bool ShowGUI;
    public Color alphaColor;
    private int ExpGained;
    private float yValue;

    void Start()
    {

    }

    void OnEnable()
    {
        RPG.Events.GainedExp += GainedExpAlt;
    }
    void OnDisable()
    {
        RPG.Events.GainedExp -= GainedExpAlt;    
    }

    private void GainedExpAlt(object sender, RPGEvents.GainedExpEventArgs e)
    {
        StartCoroutine("ShowExpGained",e.ExpGained);
    }

    void Update()
    {       
        if(ShowGUI)
        {
            alphaColor.a -= 1*Time.deltaTime;
            yValue -= 100.00f * Time.deltaTime;
        }
    }


    IEnumerator ShowExpGained(int exp)
    {
        if(ShowGUI) StopCoroutine("ShowExpGained");

        ExpGained = exp;
        ToggleGUI();
        yield return new WaitForSeconds(1);
        ToggleGUI();
    }

    private void ToggleGUI()
    {
        ShowGUI = !ShowGUI;
        if (ShowGUI)
        {
            
            alphaColor = new Color(GUI.color.a,GUI.color.g,GUI.color.b,255);
            yValue = Screen.height - 100;
        }
    }

    void OnGUI()
    {

        if(ShowGUI)
        {
            GUI.color = alphaColor;
            GUI.Label(new Rect(Screen.width/2 - 50, yValue, 100,20 ),"+" + ExpGained + " Exp!" );
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameClock : MonoBehaviour
{
    //[SerializeField]
    private Text floatingClockLabel;
    private float lastchange = 0.0f;
    private int day = 0;
    private int hour = 6;
    private int minutes = 0;
    private string ampm = "am";

    private void Start()
    {
        floatingClockLabel = GameObject.Find("GameClock").GetComponent<Text>();
        if (floatingClockLabel == null)
        {
            //floatingClockLabel;
            Debug.Log("GameClock game Object was not found.");
        }
        else
        {
            floatingClockLabel.text = (hour.ToString() + ":" + minutes.ToString() + "0" + ampm);
        }
        Debug.Log(hour.ToString() + ":" + minutes.ToString() + "0" + ampm);
        // We want this object to persist between scene loads
        //DontDestroyOnLoad(this);
    }

    private void Update()
    {
        // Every 12 seconds increment as much as possible
        if ((Time.time - lastchange) > 12.0f)
        {
            minutes++;
            if (minutes == 6)
            {
                minutes = 0;
                hour++;
                if (hour == 12)
                {
                    if (ampm == "am")
                    {
                        ampm = "pm";
                    }
                    else
                    {
                        NewDay();
                    }
                }

                // support wrap around for AM/PM trickiness of 12:00 clocks
                if (hour == 13)
                {
                    hour = 1;
                }
            }

            lastchange = Time.time;
            floatingClockLabel.text = (hour.ToString() + ":" + minutes.ToString() + "0" + ampm);
        }
    }

    private void OnGUI()
    {
        //GUILayout.Label(hour.ToString() + ":" + minutes.ToString() + "0" + ampm);
    }

    int NewDay()
    {
        int ret = 0;
        if (day > -1)
        {
            ret = ++day;

        }
        // whatever happens on a new day
        return ret;
    }
}
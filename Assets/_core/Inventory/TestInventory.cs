using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestInventory : MonoBehaviour
{
    private Text ListOfItems;
    private GameManager gm;
    private GameObject iw;

    // Use this for initialization
    void Start ()
    {
        gm = Object.FindObjectOfType<GameManager>();
        iw = GameObject.FindGameObjectWithTag("ItemWindow");
        if (gm && iw)
        {
            foreach (Item i in gm.GetItems())
            {
                ListOfItems.text += i.sName + " ";
            }
        }
        else
        {
            Debug.Log("Could not locate Game ManagerByType or ItemWindowByTag");
        }
    }
	
	// Update is called once per frame
	void Update ()
	{
		
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateItemUI : MonoBehaviour {
    private Item item = null;

    //private InputField GenerateItemText = this;

    //// Use this for initialization
    void Start()
    {
        var input = gameObject.GetComponent<InputField>();
        var se = new InputField.SubmitEvent();
        se.AddListener(LockInput);
        input.onEndEdit = se;

        //Adds a listener that invokes the "LockInput" method when the player finishes editing the main input field.
        //Passes the main input field into the method when "LockInput" is invoked
        //input.onEndEdit.AddListener(
        //    delegate { LockInput(string arg0); }
        //);
    }

    // Checks if there is anything entered into the input field.
    void LockInput(string arg0)
    {
        int x = 0;
        Debug.Log(arg0);
        if (arg0.Length > 0)
        {
            Int32.TryParse(arg0, out x);
            Item i = GameManager.instance.IsValidItem(x);
            if (i != null)
            {
                UIEventHandler.ItemAddedToInventory(i);
            }
        }
        else
        {
            Debug.Log("Main Input Empty");
        }
    }

    public void SetItem(Item item, InputField GenerateItemText)
    {

    }

    // Update is called once per frame
    void Update () {

	}
}

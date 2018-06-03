using System;
using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CreditsHandler : MonoBehaviour
{
    public float CreditsSpeed = 15;
    public GameObject MainMenuButtons;
    public GameObject SaveFileSection;
    public GameObject CreditsSection;
    public GameObject CreditsHolder;
    public GameObject TextPrefab;
    public GameObject ImagePrefab;
    public GameObject NamePrefab;
    public GameObject RoleTitlePrefab;
    public GameObject TitlePrefab;
    public GameObject SpacePrefab;
    public GameObject LastEntry;
    public int LastEntrySeen;
    public Vector3 StartPos;
    public bool Running;

    void Start()
    {
        StartPos = CreditsHolder.GetComponent<RectTransform>().position;
        CreditsSection.SetActive(false);
    }

    void Update()
    {
        if(Running)
        {
            CreditsHolder.GetComponent<RectTransform>().position += new Vector3(0,1,0) * CreditsSpeed * Time.deltaTime;

            Vector3[] v = new Vector3[4];
            LastEntry.GetComponent<RectTransform>().GetWorldCorners(v);

            float maxY = Mathf.Max(v[0].y, v[1].y, v[2].y, v[3].y);
            float minY = Mathf.Min(v[0].y, v[1].y, v[2].y, v[3].y);
            //No need to check horizontal visibility: there is only a vertical scroll rect
            //float maxX = Mathf.Max (v [0].x, v [1].x, v [2].x, v [3].x);
            //float minX = Mathf.Min (v [0].x, v [1].x, v [2].x, v [3].x);

            if (maxY < 0 || minY > Screen.height + 50)
            {
                if(LastEntrySeen > 1) //For some reason needs to be seen twice
                {
                    //Debug.Log("Not visible, ending credits");
                    End();
                }
            }
            else
            {
                //Debug.Log("Visible");
                LastEntrySeen++;
            }

            if(Input.touchCount > 0 || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                End();
            }
        }
    }

    public void Begin()
    {
        Running = true;
        MainMenuButtons.SetActive(false);
        SaveFileSection.SetActive(false);
        CreditsHolder.GetComponent<RectTransform>().position = StartPos;
        //load credits
        CreditsHolder.transform.DestroyChildren();
        foreach (var creditEntry in RPG.Game.Credits)
        {

            switch(creditEntry.Type)
            {
                case CreditsType.Logo:
                    {
                        var go = Instantiate(ImagePrefab, Vector3.zero, Quaternion.identity) as GameObject;
                        go.transform.SetParent(CreditsHolder.transform, false);
                        var classSelect = go.GetComponent<CreditsModel>();
                        classSelect.Image.sprite = GeneralMethods.CreateSprite(creditEntry.Image);
                        LastEntry = go;
                    }
                    break;
                case CreditsType.Name:
                    {
                        var go = Instantiate(NamePrefab, Vector3.zero, Quaternion.identity) as GameObject;
                        go.transform.SetParent(CreditsHolder.transform, false);
                        var classSelect = go.GetComponent<CreditsModel>();
                        classSelect.Text.text = creditEntry.Name;
                        LastEntry = go;
                    }
                    break;
                case CreditsType.RoleTitle:
                    {
                        var go = Instantiate(RoleTitlePrefab, Vector3.zero, Quaternion.identity) as GameObject;
                        go.transform.SetParent(CreditsHolder.transform, false);
                        var classSelect = go.GetComponent<CreditsModel>();
                        classSelect.Text.text = creditEntry.Role;
                        LastEntry = go;
                    }
                    break;
                case CreditsType.Text:
                    {
                        var go = Instantiate(TextPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                        go.transform.SetParent(CreditsHolder.transform, false);
                        var classSelect = go.GetComponent<CreditsModel>();
                        classSelect.Text.text = creditEntry.Text;
                        LastEntry = go;
                    }
                    break;
                case CreditsType.Title:
                    {
                        var go = Instantiate(TitlePrefab, Vector3.zero, Quaternion.identity) as GameObject;
                        go.transform.SetParent(CreditsHolder.transform, false);
                        var classSelect = go.GetComponent<CreditsModel>();
                        classSelect.Text.text = creditEntry.Title;
                        LastEntry = go;
                    }
                    break;
                case CreditsType.Space:
                    {
                        var go = Instantiate(SpacePrefab, Vector3.zero, Quaternion.identity) as GameObject;
                        go.transform.SetParent(CreditsHolder.transform, false);
                        var classSelect = go.GetComponent<CreditsModel>();
                        classSelect.LayoutElement.minHeight = creditEntry.Space;
                        LastEntry = go;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        CreditsSection.SetActive(true);
    }

    public void End()
    {
        CreditsSection.SetActive(false);
        MainMenuButtons.SetActive(true);
        Running = false;
        LastEntrySeen = 0;
    }
}
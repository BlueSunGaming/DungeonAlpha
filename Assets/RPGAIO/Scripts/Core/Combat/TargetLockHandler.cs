using System;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class TargetLockHandler : MonoBehaviour
{
    public bool ShowWithTexture;
    public TargetLockState State;
    public GameObject TargetLockObject;
    private TargetLockPrefab TargetLockPrefab;
    private Material _material;
    private Texture2D _image;
    private const float BasePlaneScale = 1.2f;

    void Start()
    {

        if(TargetLockObject.name == "TargetLock")
        {
            TargetLockObject.SetActive(false);
        }

        if (!Rm_RPGHandler.Instance.Combat.ShowSelected) return;


        ShowWithTexture = Rm_RPGHandler.Instance.Combat.ShowSelectedWithTexture;
        if(ShowWithTexture)
        {
            _material = TargetLockObject.GetComponent<MeshRenderer>().materials[0];
            TargetLockObject.SetActive(false);
            TargetLockObject.transform.SetY(Rm_RPGHandler.Instance.Combat.SelectedYOffSet);
            var radius = GetComponent<NavMeshAgent>().radius;
            TargetLockObject.transform.localScale = new Vector3(BasePlaneScale, 0.001f, BasePlaneScale);
            TargetLockObject.transform.localScale *= (radius * 2);
        }
        else
        {
            var t = transform.Find("TargetLockPrefab");
            if (t == null)
            {
                t = GeneralMethods.SpawnPrefab(Rm_RPGHandler.Instance.Combat.SelectedPrefabPath, transform.position, transform.rotation, transform).transform;
                t.name = "TargetLockPrefab";
            }

            TargetLockObject = t.gameObject;
            TargetLockPrefab = TargetLockObject.GetComponent<TargetLockPrefab>();
            TargetLockPrefab.Set(TargetLockState.Unselected);
        }

        State = TargetLockState.Unselected;
        

    }

    public void ChangeState(TargetLockState state)
    {
        State = state;
        if (!Rm_RPGHandler.Instance.Combat.ShowSelected) return;

        _material = TargetLockObject.GetComponent<MeshRenderer>().sharedMaterial;

        if (Rm_RPGHandler.Instance.Combat.TargetStyle == TargetStyle.ManualTarget)
        {
            if (ShowWithTexture)
            {
                TargetLockObject.SetActive(false);
            }
            else
            {
                TargetLockPrefab.Set(TargetLockState.Unselected);
            }

            return;
        }

        switch (State)
        {
            case TargetLockState.Unselected:
                if(ShowWithTexture)
                {
                    TargetLockObject.SetActive(false);
                }
                else
                {
                    TargetLockPrefab.Set(TargetLockState.Unselected);
                }
                break;
            case TargetLockState.Selected:
                if (ShowWithTexture)
                {
                    TargetLockObject.SetActive(true);
                    if (Rm_RPGHandler.Instance.Combat.SelectedTexture.Image != null)
                    {
                        //_material.SetTexture("Albedo", Rm_RPGHandler.Instance.Combat.SelectedTexture.Image);
                        _material.mainTexture = Rm_RPGHandler.Instance.Combat.SelectedTexture.Image;
                    }
                    else
                    {
                        Debug.Log("null image");
                    }
                }
                else
                {
                    TargetLockPrefab.Set(TargetLockState.Selected);
                }
                break;
            case TargetLockState.InCombat:
                if (ShowWithTexture)
                {
                    TargetLockObject.SetActive(true);
                    if (Rm_RPGHandler.Instance.Combat.SelectedCombatTexture.Image != null)
                        _material.SetTexture("Albedo", Rm_RPGHandler.Instance.Combat.SelectedCombatTexture.Image);

                }
                else
                {
                    TargetLockPrefab.Set(TargetLockState.InCombat);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException("state");
        }
    }
}

public enum TargetLockState
{
    Unselected,
    Selected,
    InCombat
}

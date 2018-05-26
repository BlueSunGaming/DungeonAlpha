using System.Linq;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class SkillDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static GameObject itemBeingDragged;

    public bool IsSkill;
    public string RefId;

    Vector3 startPosition;
    Transform startParent;
    Transform canvas;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(IsSkill)
        {
            var skill = GetObject.PlayerCharacter.SkillHandler.AvailableSkills.First(s => s.ID == RefId);
            if (!skill.Unlocked) return;
        }

        itemBeingDragged = gameObject;
        startPosition = transform.position;
        startParent = transform.parent;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        canvas = GameObject.FindGameObjectWithTag("UIHandler").transform;
        transform.SetParent(canvas, false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsSkill)
        {
            var skill = GetObject.PlayerCharacter.SkillHandler.AvailableSkills.First(s => s.ID == RefId);
            if (!skill.Unlocked) return;
        }

        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)   
    {
        if (IsSkill)
        {
            var skill = GetObject.PlayerCharacter.SkillHandler.AvailableSkills.First(s => s.ID == RefId);
            if (!skill.Unlocked) return;
        }

        itemBeingDragged = null;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        if (transform.parent == canvas)
        {
            transform.SetParent(startParent,false);
            transform.position = startPosition;
        }
    }
}﻿
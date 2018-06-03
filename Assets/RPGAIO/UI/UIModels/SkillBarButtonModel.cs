using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillBarButtonModel : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public Image SkillImage;
    public Text SkillText;
    public int SkillSlot;
    public Image CooldownOverlay;
    public Color CooldownColor;
    private bool _pointerDown;

    private bool _setSlot;
    private SkillBarSlot _slot = null;
    void Update()
    {
        if(!_setSlot)
        {
            _slot = GetObject.PlayerCharacter.SkillHandler.Slots[SkillSlot];
            _setSlot = true;
        }

        bool hasCd = false;

        if(_slot.InUse)
        {
            var cooldownPercent = 0f;
            if (_slot.IsItem)
            {
                var consum = _slot.Item as Consumable;
                if(consum != null && consum.CurrentCooldown > 0)
                {
                    cooldownPercent = consum.CurrentCooldown/consum.Cooldown;
                    hasCd = true;
                }
            }
            else
            {
                var skill = _slot.Skill;
                if (skill.CurrentCoolDownTime > 0)
                {
                    cooldownPercent = skill.CurrentCoolDownTime / skill.CoolDownTime;
                    hasCd = true;
                }
            }

            if(hasCd)
            {
                CooldownOverlay.color = CooldownColor;
                CooldownOverlay.fillAmount = cooldownPercent;
            }
        }


        if(!hasCd)
        {
            if(CooldownOverlay != null)
                CooldownOverlay.color = Color.clear;
        }

        if (_pointerDown)
        {
            _slot.Use();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //if (eventData.button == PointerEventData.InputButton.Left)
        //    _slot.Use();
        //else 
        if (eventData.button == PointerEventData.InputButton.Right)
            _slot.EmptySlot();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pointerDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pointerDown = false;
    }
}
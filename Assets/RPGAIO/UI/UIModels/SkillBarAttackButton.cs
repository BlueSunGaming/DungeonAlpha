using Assets.Scripts.Beta.NewImplementation;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Beta;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillBarAttackButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private IRPGCombat _combat;
    private Transform _playerTransform;
    private bool _pointerDown;
    void Awake()
    {
        _combat = GetObject.PlayerController.RPGCombat;
        _playerTransform = GetObject.PlayerMonoGameObject.transform;
    }


    void Update()
    {
        if (_pointerDown)
        {
            if (Rm_RPGHandler.Instance.Combat.TargetStyle == TargetStyle.TargetLock)
            {
                var target = GetObject.PlayerController.Target;
                if (target != null)
                    _combat.Attack(target);
            }
            else
            {
                _combat.Attack(null, _playerTransform.position + _playerTransform.forward * 1);
            }
        }
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
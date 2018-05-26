using System;
using System.Linq;
using Assets.Scripts.Beta.NewImplementation;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.Core;
using UnityEngine;

public class RPGAnimation : MonoBehaviour, IRPGAnimation
{
    private IRPGController _controller;
    private string _animationId;

    public Animation Animation
    {
        get { return _controller.Animation; }
    }

    public Animator Animator
    {
        get { return _controller.Animator; }
    }

    public bool IsLegacy
    {
        get { return _controller.Character.AnimationType == RPGAnimationType.Legacy; }
    }

    void Awake()
    {
        _controller = GetComponent<RPGController>();
        _animationId = Guid.NewGuid().ToString();
    }

    private void MecanimAnimation(AnimationDefinition animDef)
    {
        switch(animDef.RPGAnimationSet)
        {
            case RPGAnimationSet.Core:
                CoreMecanimAnimation(animDef);
                break;
            case RPGAnimationSet.DefaultAttack:
                Animator.SetInteger("weaponType", (int)animDef.RPGAnimationSet);
                Animator.SetInteger("animNumber", animDef.MecanimAnimationNumber);
                Animator.SetBool("falling", false);
                Animator.SetBool("idle", false);
                Animator.SetBool("combatIdle", false);
                Animator.SetBool("jumping", false);
                Animator.SetTrigger("attackTrigger");
                break;
            case RPGAnimationSet.WeaponTypeAttack:
                Animator.SetInteger("weaponType", animDef.WeaponTypeIndex);
                Animator.SetInteger("animNumber", animDef.MecanimAnimationNumber);
                Animator.SetBool("falling", false);
                Animator.SetBool("idle", false);
                Animator.SetBool("combatIdle", false);
                Animator.SetBool("jumping", false);
                Animator.SetTrigger("attackTrigger");
                break;
            case RPGAnimationSet.Skill:
                Animator.SetBool("isCastingAnimation", _controller.IsCastingSkill);
                Animator.SetInteger("skillAnimSet", animDef.MecanimAnimationGroup);
                Animator.SetInteger("animNumber", animDef.MecanimAnimationNumber);
                Animator.SetBool("falling", false);
                Animator.SetBool("idle", false);
                Animator.SetBool("combatIdle", false);
                Animator.SetBool("jumping", false);
                Animator.SetTrigger("skillTrigger");
                break;
            case RPGAnimationSet.Harvesting:
                Animator.SetInteger("animNumber", animDef.MecanimAnimationNumber);
                Animator.SetBool("falling", false);
                Animator.SetBool("idle", false);
                Animator.SetBool("combatIdle", false);
                Animator.SetBool("jumping", false);
                Animator.SetTrigger("harvestTrigger");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void CoreMecanimAnimation(AnimationDefinition animDef)
    {
        float curSpeed;
        switch (animDef.Name)
        {
            case "Idle":
                curSpeed = Animator.GetFloat("speed");
                Animator.SetFloat("speed", Mathf.Lerp(curSpeed, 0f, 5 * Time.deltaTime));
                Animator.SetBool("moving", false);
                Animator.SetBool("strafing", false);
                Animator.SetBool("falling", false);
                Animator.SetBool("idle", true);
                Animator.SetBool("combatIdle", false);
                Animator.SetBool("jumping", false);
                break;
            case "Combat Idle":
                curSpeed = Animator.GetFloat("speed");
                Animator.SetFloat("speed", Mathf.Lerp(curSpeed, 0f, 5 * Time.deltaTime));
                Animator.SetBool("moving", false);
                Animator.SetBool("strafing", false);
                Animator.SetBool("falling", false);
                Animator.SetBool("idle", true);
                Animator.SetBool("combatIdle", false);
                Animator.SetBool("jumping", false);
                break;
            case "Walk":
                curSpeed = Animator.GetFloat("speed");
                Animator.SetFloat("speed", Mathf.Lerp(curSpeed, 0.7f, 5 * Time.deltaTime));
                Animator.SetBool("moving", true);
                break;
            case "Run":
                curSpeed = Animator.GetFloat("speed");
                Animator.SetFloat("speed", Mathf.Lerp(curSpeed, 2.0f, 5 * Time.deltaTime));
                Animator.SetBool("moving", true);
                break;
            case "Strafe Left":
                curSpeed = Animator.GetFloat("speed");
                Animator.SetFloat("speed", Mathf.Lerp(curSpeed, -1.5f, 10 * Time.deltaTime));
                Animator.SetBool("strafing", true);
                break;
            case "Strafe Right":
                curSpeed = Animator.GetFloat("speed");
                Animator.SetFloat("speed", Mathf.Lerp(curSpeed, 1.5f, 10 * Time.deltaTime));
                Animator.SetBool("strafing", true);
                break;
            case "Jump":
                Animator.Play("Jumping");
                break;
            case "Death":
                Animator.Play("Death");
                break;
            case "Knock Back":
                Animator.Play("Knock Back");
                break;
            case "Knock Up":
                Animator.Play("Knock Up");
                break;
            case "Take Hit":
                Animator.Play("Hit");
                break;
        }
    }

    private void LegacyAnimation(AnimationDefinition animDef)
    {
        if (!string.IsNullOrEmpty(animDef.Animation))
        {
            var thisAnimation = Animation[animDef.Animation];
            thisAnimation.wrapMode = animDef.WrapMode;
            thisAnimation.speed = animDef.Speed;

            if (animDef.Backwards)
            {
                thisAnimation.speed *= -1;
            }

            var asvt = Rm_RPGHandler.Instance.ASVT;
            if (!string.IsNullOrEmpty(animDef.Name) && asvt.UseStatForMovementSpeed && new[] { "Walk, Strafe, Run" }.Any(s => s.Contains(animDef.Name)))
            {
                var asvtStat = asvt.StatForMovementID;
                var bonusMove = _controller.Character.GetStatByID(asvtStat).TotalValue - 1;
                if (bonusMove > 0)
                {
                    var add = 1 + (bonusMove / 4);
                    thisAnimation.speed *= add;
                }
            }

            Animation.CrossFade(animDef.Animation);
        }
        else
        {
            if (!string.IsNullOrEmpty(_controller.LegacyAnimation.IdleAnim.Animation))
                Animation.CrossFade(_controller.LegacyAnimation.IdleAnim.Animation);
        }


    }

    public void CrossfadeAnimation(AnimationDefinition animDef)
    {


        if (IsLegacy && Animation == null)
        {
            return;
        }
        
        if(!IsLegacy && Animator == null)
        {
            return;
        }

        if(IsLegacy)
        {
            LegacyAnimation(animDef);
        }
        else
        {
            //Debug.Log("Mecanim: " + animDef.Name);
            MecanimAnimation(animDef);
        }

        var canLoop = new[] { "Jump", "Knock Back", "Knock Up", "Take Hit", "Death" }.All(s => s != animDef.Name);
        if (canLoop && animDef.Sound != null)
        {
            var id = animDef.Name;

            if (animDef.Name.Contains("Walk") || animDef.Name.Contains("Run"))
                id = "Walk";
            else if (animDef.Name.Contains("Strafe"))
                id = "Strafe";
            else if (animDef.Name.Contains("Turn"))
                id = "Turn";
            else if (animDef.Name.Contains("Attack"))
                id = "";

            AudioPlayer.Instance.Play(animDef.Sound, AudioType.SoundFX, transform.position, transform, id != "" ? "anim_Core_" + id + "_" + _animationId : "");
        }
     }

    public void CrossfadeAnimation(string animationname, float speed = 1.0f, WrapMode wrapMode = WrapMode.Loop, bool backwards = false)
    {
        var animDef = new AnimationDefinition {Animation = animationname, Speed = speed, WrapMode = wrapMode, Backwards = backwards};
        CrossfadeAnimation(animDef);
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Beta.NewImplementation;
using LogicSpawn.RPGMaker.Beta;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class SkillHitHandler : MonoBehaviour
    {
        protected bool Active;
        protected Skill _skill;
        protected bool _initialised;
        //note: public for debug
        public List<BaseCharacterMono> _permTargetTracker;
        public List<BaseCharacterMono> _targetTracker;

        void Awake()
        {
            Active = true;
            _targetTracker = new List<BaseCharacterMono>();
            _permTargetTracker = new List<BaseCharacterMono>();
        }

        protected bool AddTarget(Transform other, bool dontHitIfInPermTracker = true)
        {
            bool added = false;
            if (other.CompareTag("Enemy") || other.CompareTag("NPC") || other.CompareTag("Player"))
            {
                var baseMono = other.GetComponent<BaseCharacterMono>();
                var foundTarget = !((_skill.TargetType == TargetType.Self && baseMono != _skill.CasterMono) ||
                                     (_skill.TargetType == TargetType.SelfOrAlly && (_skill.CasterMono != null) && !_skill.CasterMono.Controller.IsFriendly(baseMono) && baseMono != _skill.CasterMono) ||
                                     (_skill.TargetType == TargetType.Ally && (_skill.CasterMono != null) && !_skill.CasterMono.Controller.IsFriendly(baseMono)) ||
                                     (_skill.TargetType == TargetType.Enemy && (_skill.CasterMono != null) && _skill.CasterMono.Controller.IsFriendly(baseMono)) ||
                                     (_skill.TargetType == TargetType.NotSelf && baseMono == _skill.CasterMono) ||
                                     (_skill.TargetType == TargetType.Any && baseMono == null));


                if (baseMono != null && baseMono.Character.CharacterType == CharacterType.NPC)
                {
                    if(!Rm_RPGHandler.Instance.Combat.NPCsCanFight)
                    {
                        foundTarget = false;
                    }
                    else if(_skill.Caster.CharacterType == CharacterType.Player)
                    {
                        if(!Rm_RPGHandler.Instance.Combat.CanAttackNPcs)
                        {
                            foundTarget = false;
                        }
                        else
                        {
                            var npcChar = baseMono.Character as NonPlayerCharacter;
                            if(npcChar.CanBeKilled && !Rm_RPGHandler.Instance.Combat.CanAttackUnkillableNPCs)
                            {
                                foundTarget = false;
                            }
                        }
                    }
                }


                if(foundTarget)
                {
                    var isInPerm = _permTargetTracker.FirstOrDefault(t => t.ID == baseMono.ID) != null;
                    if (_targetTracker.FirstOrDefault(t => t.ID == baseMono.ID) == null)
                    {
                        _targetTracker.Add(baseMono);
                        if (dontHitIfInPermTracker && !isInPerm)
                        {
                            HandleSkillOnTarget(_skill, baseMono);
                        }
                        added = true;
                        //Debug.Log("Added target" + baseMono.Character.Name);
                    }
                    else
                    {
                        Debug.Log("Did not re-add target" + baseMono.Character.Name);
                    }

                    if (_permTargetTracker.FirstOrDefault(t => t.ID == baseMono.ID) == null)
                    {
                        _permTargetTracker.Add(baseMono);
                    }
                }
            }

            return added;
        }

        protected void RemoveTarget(Collider other)
        {
            if (other.CompareTag("Enemy") || other.CompareTag("NPC"))
            {
                var baseMono = other.GetComponent<BaseCharacterMono>();
                _targetTracker.Remove(baseMono);
                Debug.Log("Removed target" + baseMono.Character.Name);
            }
        }

        private void HandleSkillOnTarget(Skill originalSkill, BaseCharacterMono target)
        {
            var skillToUse = GeneralMethods.CopySkill(originalSkill);

            //Debug.Log("Handled skill hit for " + target.Character.Name);
            if (skillToUse.Caster != null)
            {
                DamageOutcome outcome = new DamageOutcome(new Damage(), AttackOutcome.Success);
                var damageToDeal = skillToUse.Damage;
                if(damageToDeal.MaxTotal > 0)
                {
                    var projSkillHandler = this as ProjectileSkillHandler;
                    if (projSkillHandler != null)
                    {
                        var projSkill = (ProjectileSkill)_skill;
                        if (projSkill.IsPiercing)
                        {
                            damageToDeal.ApplyMultiplier(projSkill.PiercingScaling[projSkillHandler.PierceCounter]);
                        }
                    }

                    if (skillToUse.CasterMono != null)
                    {
                        outcome = skillToUse.CasterMono.Controller.RPGCombat.DamageTarget(target, damageToDeal);
                        var cc = target.Character as CombatCharacter;
                        if (cc != null)
                        {
                            var targetTaunt = cc.TauntHandler;
                            if (!targetTaunt.Tracker.ContainsKey(skillToUse.CasterMono))
                            {
                                targetTaunt.Tracker.Add(skillToUse.CasterMono, 1);
                            }

                            targetTaunt.Tracker[skillToUse.CasterMono] += skillToUse.BonusTaunt;
                        }
                    }
                    else
                    {
                        outcome = target.Character.VitalHandler.TakeDamage(skillToUse.Caster, damageToDeal);
                    }  
                }
                


                var onCritProcs = new List<Rm_ProcEffect>();
                var onHitProcs = new List<Rm_ProcEffect>();

                var procs = skillToUse.Caster.ProcEffects;

                if (skillToUse.HasProcEffect)
                {
                    switch (skillToUse.ProcEffect.ProcCondition)
                    {
                        case Rm_ProcCondition.Every_N_Hits:
                        case Rm_ProcCondition.Chance_On_Hit:
                        case Rm_ProcCondition.On_Hit:
                            onHitProcs.Add(skillToUse.ProcEffect);
                            break;
                        case Rm_ProcCondition.Chance_On_Critical_Hit:
                            onCritProcs.Add(skillToUse.ProcEffect);
                            break;
                    }
                }

                foreach (var procEffect in procs)
                {
                    switch (procEffect.ProcCondition)
                    {
                        case Rm_ProcCondition.Every_N_Hits:
                        case Rm_ProcCondition.Chance_On_Hit:
                        case Rm_ProcCondition.On_Hit:
                            onHitProcs.Add(procEffect);
                            break;
                        case Rm_ProcCondition.Chance_On_Critical_Hit:
                            onCritProcs.Add(procEffect);
                            break;
                    }
                }

                if (damageToDeal.MaxTotal == 0 || outcome != null && (outcome.AttackOutcome == AttackOutcome.Success || outcome.AttackOutcome == AttackOutcome.Critical))
                {
                    foreach (var proc in onHitProcs)
                    {
                        IncrementSkillCounters(skillToUse);
                        HandleProc(proc, target);
                    }

                    if (skillToUse.ApplyDOTOnHit)
                    {
                        var canApply = Random.Range(0, 100 + 1) <= (int)(skillToUse.ChanceToApplyDOT * 100);
                        if (canApply)
                        {
                            skillToUse.DamageOverTime.SkillMetaID = skillToUse.SkillMetaID;
                            skillToUse.DamageOverTime.Attacker = skillToUse.Caster;
                            target.Character.AddDoT(skillToUse.DamageOverTime);
                        }
                    }

                    if (skillToUse.RunEventOnHit)
                    {
                        if (!string.IsNullOrEmpty(skillToUse.EventOnHitID))
                        {
                            Debug.Log("Not implemented: do event:" + skillToUse.EventOnHitID);
                        }
                    }

                    if (skillToUse.SkillType == SkillType.Aura)
                    {
                        var auraSkill = (AuraSkill)skillToUse;
                        target.Character.ToggleAura(auraSkill,true);
                    }
                    else if (skillToUse.SkillType == SkillType.Restoration)
                    {
                        var restoSkill = (RestorationSkill)skillToUse;
                        target.Character.AddRestoration(restoSkill.Restoration);
                    }

                    if (skillToUse.AppliesBuffDebuff)
                    {
                        target.Character.AddTimedPassiveEffect(skillToUse.Effect);
                    }

                    if (skillToUse.ApplyStatusEffect)
                    {
                        var apply = Random.Range(0, 100 + 1) <= (int)(skillToUse.ChanceToApplyStatusEffect * 100);
                        if (apply)
                        {
                            var statusEffect = Rm_RPGHandler.Instance.Repositories.StatusEffects.Get(skillToUse.StatusEffectID);
                            if (statusEffect != null)
                            {
                                statusEffect.Effect.HasDuration = skillToUse.ApplyStatusEffectWithDuration;
                                if (statusEffect.Effect.HasDuration)
                                {
                                    statusEffect.Effect.Duration = skillToUse.ApplyStatusEffectDuration;
                                }
                                target.Character.AddStatusEffect(statusEffect);
                            }
                        }
                    }

                    if (skillToUse.RemoveStatusEffect)
                    {
                        var apply = Random.Range(0, 100 + 1) <= (int)(skillToUse.ChanceToRemoveStatusEffect * 100);
                        if (apply)
                        {
                            target.Character.RemoveStatusEffect(skillToUse.RemoveStatusEffectID);
                        }
                    }

                    var impact = GeneralMethods.SpawnPrefab(skillToUse.ImpactPrefabPath, target.transform.position, Quaternion.identity, target.transform);
                    if (impact != null)
                    {
                        impact.GetComponent<DestroyHelper>().Init(DestroyCondition.Time, 0.3f);
                    }
                    AudioPlayer.Instance.Play(skillToUse.ImpactSound.Audio, AudioType.SoundFX, target.transform.position, target.transform);
                }

                if (outcome != null && outcome.AttackOutcome == AttackOutcome.Critical)
                {
                    foreach (var proc in onCritProcs)
                    {
                        IncrementSkillCounters(skillToUse);
                        HandleProc(proc, target);
                    }
                }
            }
        }

        private void IncrementSkillCounters(Skill skillToUse)
        {
            if (skillToUse.CasterMono != null)
            {
                if(skillToUse.CasterMono.Character.CharacterType == CharacterType.Player)
                {
                    var player = (PlayerCharacter)skillToUse.CasterMono.Character;
                    var skill = player.SkillHandler.AvailableSkills.FirstOrDefault(s => s.ID == skillToUse.ID);
                    if (skill != null)
                    {
                        skill.TimesUsed += 1;
                        if (skill.ProcEffect != null)
                        {
                            skill.ProcEffect.ParameterCounter += 1;
                        }
                    }

                }
                else
                {
                    var cc = (CombatCharacter)skillToUse.CasterMono.Character;
                    var skill = cc.EnemySkills.FirstOrDefault(s => s.SkillID == skillToUse.ID);
                    if (skill != null)
                    {
                        skill.SkillRef.TimesUsed += 1;
                        if (skill.SkillRef.ProcEffect != null)
                        {
                            skill.SkillRef.ProcEffect.ParameterCounter += 1;
                        }
                    }
                }
            }
        }

        protected void HandleProc(Rm_ProcEffect procEffect, BaseCharacterMono target)
        {
            var canProc = false;
            if (procEffect.ProcCondition == Rm_ProcCondition.On_Hit)
            {
                canProc = true;
            }
            else if (procEffect.ProcCondition == Rm_ProcCondition.Chance_On_Critical_Hit ||
                     procEffect.ProcCondition == Rm_ProcCondition.Chance_On_Hit)
            {
                canProc = Random.Range(0, 100 + 1) <= (int)(procEffect.Parameter * 100);
            }
            else if (procEffect.ProcCondition == Rm_ProcCondition.Every_N_Hits)
            {
                var n = (int)procEffect.Parameter;
                canProc = procEffect.ParameterCounter % n == 0;
                Debug.Log("Proc:" + procEffect.ParameterCounter % n + " / " + n + "  " + canProc);

            }

            if (canProc)
            {
                //add actions
                if (procEffect.ProcEffectType == Rm_ProcEffectType.StatusEffect ||
                    procEffect.ProcEffectType == Rm_ProcEffectType.StatusEffectOnSelf)
                {
                    if (procEffect.ProcEffectType == Rm_ProcEffectType.StatusEffect)
                    {
                        target.Character.AddStatusEffect(procEffect.EffectParameterString);
                    }
                    else
                    {
                        if (_skill.CasterMono != null)
                        {
                            _skill.CasterMono.Character.AddStatusEffect(procEffect.EffectParameterString);
                        }
                    }
                }
                else if (procEffect.ProcEffectType == Rm_ProcEffectType.CastSkill ||
                         procEffect.ProcEffectType == Rm_ProcEffectType.CastSkillOnSelf)
                {

                    var allSkills = Rm_RPGHandler.Instance.Repositories.Skills.AllSkills;
                    var skill = Rm_RPGHandler.Instance.Repositories.Skills.Get(procEffect.EffectParameterString);
                    if (skill != null)
                    {
                        skill.CasterMono = _skill.CasterMono;
                        skill.Caster = _skill.Caster;

                        if (procEffect.ProcEffectType == Rm_ProcEffectType.CastSkill)
                        {
                            CastSkill(skill, target, null);
                        }
                        else
                        {
                            if (_skill.CasterMono != null)
                            {
                                CastSkill(skill, _skill.CasterMono, null);
                            }
                        }
                    }
                }
                else if (procEffect.ProcEffectType == Rm_ProcEffectType.KnockBack)
                {
                    target.Controller.AddImpact(Direction.Back, procEffect.EffectParameter);

                }
                else if (procEffect.ProcEffectType == Rm_ProcEffectType.KnockUp)
                {
                    target.Controller.AddImpact(Direction.Up, procEffect.EffectParameter);
                }
                else if(procEffect.ProcEffectType == Rm_ProcEffectType.PullTowards)
                {
                    var pullposition = transform.position;
                    pullposition.y = target.transform.position.y;
                    target.Controller.PullTo(pullposition, procEffect.PullAllTheWay ? -1 : procEffect.EffectParameter);
                }
            }
        }
        
        protected void CastSkill(Skill originalSkill, BaseCharacterMono target, Vector3? nullableTargetPos)
        {
            if (originalSkill.SkillType == SkillType.Area_Of_Effect || originalSkill.SkillType == SkillType.Spawn)
            {
                if (nullableTargetPos == null)
                {
                    if (target != null)
                    {
                        nullableTargetPos = target.transform.position;
                        target = null;
                    }
                }
            }

            if (target == null && !nullableTargetPos.HasValue) return;
            if (target != null && !target.Character.Alive) return;

            var hasCharacterTarget = target != null;
            var targetPos = nullableTargetPos.GetValueOrDefault();

            if (hasCharacterTarget)
            {
                HandleSkillOnTarget(originalSkill, target);
            }
            else
            {
                var skillToUse = GeneralMethods.CopySkill(originalSkill);

                var newTargetPos = targetPos;
                if (skillToUse.SkillType == SkillType.Area_Of_Effect)
                {
                    var aoeSkill = (AreaOfEffectSkill)skillToUse;
                    newTargetPos = targetPos + new Vector3(0, aoeSkill.Height / 2, 0);
                    var skillPrefab = GeneralMethods.SpawnPrefab(aoeSkill.PrefabPath, newTargetPos, Quaternion.identity, null);
                    skillPrefab.GetComponent<AreaOfEffectSkillHandler>().Init(aoeSkill);
                }
                else if (skillToUse.SkillType == SkillType.Projectile)
                {
                    var projSkill = (ProjectileSkill)skillToUse;

                    var projectilePosition = targetPos;

                    var casterTransform = transform;
                    newTargetPos = casterTransform.position + (casterTransform.up * 1.3f);

                    var skillPrefab = GeneralMethods.SpawnPrefab(skillToUse.PrefabPath, newTargetPos, Quaternion.identity, null);
                    skillPrefab.GetComponent<ProjectileSkillHandler>().Init(projSkill, null, projectilePosition);
                }
                else if (skillToUse.SkillType == SkillType.Spawn)
                {
                    var spawnSkill = skillToUse;
                    newTargetPos = targetPos + new Vector3(0, 1, 0);
                    var skillPrefab = GeneralMethods.SpawnPrefab(skillToUse.PrefabPath, newTargetPos, Quaternion.identity, null);
                    skillPrefab.GetComponent<SpawnSkillHandler>().Init(spawnSkill, target);

                }
            }
        }

        protected void HandleSkillToTargets()
        {
            //Debug.Log("Handled skill hit for all stored targets");
            foreach (var target in _targetTracker)  
            {
                HandleSkillOnTarget(_skill, target);
            }
        }
    }
}
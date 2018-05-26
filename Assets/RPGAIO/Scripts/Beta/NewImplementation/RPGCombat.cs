using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LogicSpawn.RPGMaker;
using LogicSpawn.RPGMaker.API;
using LogicSpawn.RPGMaker.Beta;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Beta.NewImplementation
{
    public class RPGCombat : MonoBehaviour, IRPGCombat
    {
        private const float ImpactPrefabTime = 0.3f;
        private const float CastPrefabTime = 0.3f;

        private IRPGController _controller;
        private BaseCharacter Character
        {
            get { return _controller.Character; }
        }

        private bool IsMecanim
        {
            get { return _controller.Character.AnimationType == RPGAnimationType.Mecanim; }
        }

        private BaseCharacterMono _characterMono;
        private BaseCharacterMono CharacterMono
        {
            get { return _characterMono ?? (_characterMono = GetComponent<BaseCharacterMono>()); }
        }

        public float LastAttackTime { get; set; }

        public Rmh_Combat Combat
        {
            get { return Rm_RPGHandler.Instance.Combat; }
        }

        void Awake()
        {
            _controller = GetComponent<RPGController>();
            LastAttackTime = 0f;
        }

        public void UseSkill(Skill skillToUse,  BaseCharacterMono target)
        {
            UseSkill(skillToUse, null, target);
        }

        public void UseSkill(Skill skillToUse, Vector3 targetPos)
        {
            UseSkill(skillToUse, targetPos, null);
        }

        public void UseSkill(Skill skillToUse, Vector3 targetPos, Quaternion rotation)
        {
            UseSkill(skillToUse,targetPos, null ,rotation);
        }

        private void UseSkill(Skill originalSkill, Vector3? nullableTargetPos, BaseCharacterMono target, Quaternion rotation = default(Quaternion))
        {
            var stop = new Stopwatch();
            stop.Start();

            if (_controller.HandlingActions) return;

            //todo: moveto without collision

            if(originalSkill.SkillType == SkillType.Area_Of_Effect || originalSkill.SkillType == SkillType.Spawn)
            {
                if(nullableTargetPos == null)
                {
                    if(target != null)
                    {
                        nullableTargetPos = target.transform.position;
                        target = null;
                    }
                }
            }


            if (target == null && !nullableTargetPos.HasValue) return;

            if (target != null && !target.Character.Alive) return;

            //todo: cooldowns, requirements, etc

            const float impactTime = 0.8f;
            const float postImpactTime = 1 - impactTime;

            //Make a copy of the skill
            var skillToUse = GeneralMethods.CopySkill(originalSkill);
            skillToUse.CasterMono = CharacterMono;
            skillToUse.Caster = GeneralMethods.CopyObject(Character);

            var player = _controller.Character as PlayerCharacter;
            var npcChar = _controller.Character as CombatCharacter;
            Rm_NPCSkill enemySkill = null;
            if(npcChar != null)
            {
                enemySkill = npcChar.EnemySkills.First(e => e.SkillID == skillToUse.ID);
            }

            var hasCharacterTarget = target != null;
            var targetPos = nullableTargetPos.GetValueOrDefault();
            
            var queue = RPGActionQueue.Create();
            queue.SkillId = skillToUse.ID;
            queue.HasTarget = hasCharacterTarget;
            queue.Target = target;
            queue.HasTargetPos = targetPos != Vector3.zero || queue.HasTarget ;
            queue.TargetPos = target != null ? target.transform.position : targetPos;

            var lookAtPos = new Vector3(queue.TargetPos.x, transform.position.y, queue.TargetPos.z);
            transform.LookAt(lookAtPos);

            var approachRange = hasCharacterTarget ? 1.5f + target.Controller.NavMeshAgent.radius : 0f;
            SkillAnimationDefinition animations;
            if(_controller.Character.CharacterType == CharacterType.Player)
            {
                animations = skillToUse.LegacyAnimationToUse(player.PlayerCharacterID);
            }
            else
            {
                animations = enemySkill.AnimationToUse;
            }

            var currentMeleeAttack = -1;
            MeleeSkill meleeSkill = null;
            MeleeSkillAnimation meleeSkillDefinition = null;
            if (skillToUse.SkillType == SkillType.Melee)
            {
                meleeSkill = (MeleeSkill) originalSkill;
                currentMeleeAttack = meleeSkill.CurrentAttack;
                meleeSkillDefinition = Character.CharacterType == CharacterType.Player
                                           ? meleeSkill.MeleeAnimations.FirstOrDefault(m => m.ClassID == player.PlayerCharacterID)
                                           : enemySkill.MeleeSkillAnimations;
            }


            var casterSpeed = Character.GetStatByID("Cast Speed").TotalValue;
            var castingTime = skillToUse.CastingTime / casterSpeed;
             //Debug.Log(skillToUse.CastingTime + " -> " + castingTime);
            var castingAnimName = animations.CastingLegacyAnim;
            var castingAnimGroup = animations.CastingSkillAnimSet;
            var castingAnimNumber = animations.CastingAnimNumber;
            if (skillToUse.SkillType == SkillType.Melee)
            {
                if (meleeSkillDefinition != null)
                {
                    castingAnimName = meleeSkillDefinition.Definitions[currentMeleeAttack].CastingLegacyAnim;
                }
            }

            var castingAnimLength = !string.IsNullOrEmpty(castingAnimName) && _controller.Animation != null ? _controller.Animation[castingAnimName].length : 0.5f;
                        
            var castingAnim = new AnimationDefinition
                                  {
                                      Animation = castingAnimName,
                                      MecanimAnimationGroup = castingAnimGroup,
                                      MecanimAnimationNumber = castingAnimNumber,
                                      RPGAnimationSet = RPGAnimationSet.Skill,
                                      WrapMode = WrapMode.Loop,
                                      Speed = 1/(castingTime/castingAnimLength),
                                      Backwards = false
                                  };


            

            //Casting:
            if (IsMecanim || !string.IsNullOrEmpty(castingAnimName) && castingTime > 0)
            {
                queue.Add(RPGActionFactory.StartMecanimCasting());
                queue.Add(RPGActionFactory.PlayAnimation(castingAnim)).FacingQueueTarget();
            }

            var castingWait = RPGActionFactory.WaitForSeconds(castingTime).FacingQueueTarget();
            if(!string.IsNullOrEmpty(skillToUse.CastingPrefabPath) && castingTime > 0)
            {

                Action<GameObject> myAction = o => o.GetComponent<DestroyHelper>().Init(DestroyCondition.ActionNotPlaying, _controller, castingWait.ID);
                queue.Add(RPGActionFactory.SpawnPrefab(skillToUse.CastingPrefabPath, transform.position, transform.rotation, myAction, transform));    
            }
            queue.Add(castingWait).WithCancellable().WithSound(skillToUse.CastingSound,true);
            queue.Add(RPGActionFactory.EndMecanimCasting());

            //Cast
            if (!string.IsNullOrEmpty(skillToUse.CastPrefabPath))
            {
                Action<GameObject> myAction = o => o.GetComponent<DestroyHelper>().Init(DestroyCondition.Time, CastPrefabTime);
                queue.Add(RPGActionFactory.SpawnPrefab(skillToUse.CastPrefabPath, transform.position, Quaternion.identity, myAction, transform));
            }

            //Cast Anim -> Movement -> Handle Skill -> Cleanup
            if (skillToUse.SkillType != SkillType.Melee || meleeSkill.SeperateCastPerAttack)
            {
                
                CastSkill(skillToUse, target, targetPos, hasCharacterTarget, animations, meleeSkillDefinition,
                    currentMeleeAttack, meleeSkill, impactTime, approachRange, ref queue, rotation, postImpactTime);

                if(meleeSkill != null && meleeSkill.CurrentAttack >= meleeSkill.Attacks)
                {
                    meleeSkill.CurrentAttack = 0;
                }
            }
            else
            {
                for (int i = 0; i < meleeSkill.Attacks; i++)
                {
                    CastSkill(skillToUse, target, targetPos, hasCharacterTarget, animations, meleeSkillDefinition,
                        currentMeleeAttack, meleeSkill, impactTime, approachRange, ref queue, rotation, postImpactTime);
                }

                meleeSkill.CurrentAttack = 0;
            }
            
            
            if (_controller.BeginActionQueue(queue))
            {
                if(originalSkill.UseResourceOnCast && _controller.Character is PlayerCharacter)
                {
                    _controller.Character.GetVitalByID(originalSkill.ResourceIDUsed).CurrentValue -= originalSkill.ResourceRequirement;
                }
                originalSkill.CurrentCoolDownTime = originalSkill.CoolDownTime;
            }

            //if(stop.ElapsedMilliseconds > 50)
            //    Debug.Log("SKILL SPIKE " + stop.ElapsedMilliseconds);
        }

        private void CastSkill(Skill skillToUse, BaseCharacterMono target, Vector3 targetPos, bool hasCharacterTarget,
            SkillAnimationDefinition animations, MeleeSkillAnimation meleeSkillDefinition, int currentMeleeAttack,
            MeleeSkill meleeSkill, float impactTime, float approachRange, ref RPGActionQueue queue, Quaternion rotation,
            float postImpactTime)
        {
            var casterSpeed = Character.GetStatByID("Cast Speed").TotalValue;
            var castTime = (skillToUse.TotalCastTime - skillToUse.CastingTime) / casterSpeed;
            var castAnimName = animations.LegacyAnim;
            var castLength = !string.IsNullOrEmpty(castAnimName) && _controller.Animation != null ? _controller.Animation[castAnimName].length : 0.5f;
            if (skillToUse.SkillType == SkillType.Melee)
            {
                if (meleeSkillDefinition != null)
                {
                    castAnimName = meleeSkillDefinition.Definitions[currentMeleeAttack].LegacyAnim;
                    castLength = !string.IsNullOrEmpty(castAnimName) && _controller.Animation != null ? _controller.Animation[castAnimName].length : 0.5f;
                }
            }

            var castAnim = new AnimationDefinition
            {
                Animation = castAnimName,
                MecanimAnimationGroup = animations.CastSkillAnimSet,
                MecanimAnimationNumber = animations.CastAnimNumber,
                RPGAnimationSet = RPGAnimationSet.Skill,
                WrapMode = WrapMode.Loop,
                Speed = 1 / (castTime / castLength),
                Backwards = false
            };

            if(skillToUse.SkillType == SkillType.Melee)
            {
                currentMeleeAttack = meleeSkill.CurrentAttack;         
            }
 
            var skillMoveType = skillToUse.SkillType != SkillType.Melee ? skillToUse.MovementType : meleeSkill.Details.MeleeMoveDefinitions[currentMeleeAttack].MovementType;

            

            if (skillMoveType == SkillMovementType.StayInPlace)
            {
                if (IsMecanim || !string.IsNullOrEmpty(castAnimName) && castTime > 0)
                {
                    queue.Add(RPGActionFactory.PlayAnimation(castAnim));
                }
                queue.Add(RPGActionFactory.WaitForSeconds(castTime * impactTime)).WithSound(skillToUse.Sound).FacingQueueTarget();
            }
            else
            {
                queue.Add(RPGActionFactory.PlaySound(skillToUse.Sound, AudioType.SoundFX));
                if (skillMoveType == SkillMovementType.TeleportTo)
                {
                    if (hasCharacterTarget)
                    {
                        if(_controller.Character.AnimationType == RPGAnimationType.Legacy)
                        {
                            queue.Add(RPGActionFactory.WarpToPosition(target)).WithAnimation(animations.ApproachLegacyAnim);
                        }
                        else
                        {
                            var animDef = new AnimationDefinition
                            {
                                Name = "Approach Skill Anim",
                                Animation = castAnimName,
                                MecanimAnimationGroup = animations.ApproachSkillAnimSet,
                                MecanimAnimationNumber = animations.ApproachAnimNumber,
                                RPGAnimationSet = RPGAnimationSet.Skill,
                                WrapMode = WrapMode.Loop,
                                Speed = 1 / (castTime / castLength),
                                Backwards = false
                            };
                            queue.Add(RPGActionFactory.WarpToPosition(target)).WithAnimation(animDef);
                        }
                    }
                    else
                    {
                        if (_controller.Character.AnimationType == RPGAnimationType.Legacy)
                        {
                            queue.Add(RPGActionFactory.WarpToPosition(targetPos)).WithAnimation(animations.ApproachLegacyAnim);
                        }
                        else
                        {
                            var animDef = new AnimationDefinition
                            {
                                Name = "Approach Skill Anim",
                                Animation = castAnimName,
                                MecanimAnimationGroup = animations.ApproachSkillAnimSet,
                                MecanimAnimationNumber = animations.ApproachAnimNumber,
                                RPGAnimationSet = RPGAnimationSet.Skill,
                                WrapMode = WrapMode.Loop,
                                Speed = 1 / (castTime / castLength),
                                Backwards = false
                            };
                            queue.Add(RPGActionFactory.WarpToPosition(targetPos)).WithAnimation(animDef);
                        }
                    }
                }
                else if (skillMoveType == SkillMovementType.MoveTo)
                {
                    RPGAction movingToAction;
                    queue.Add(RPGActionFactory.StartMecanimCasting());
                    if (hasCharacterTarget)
                    {
                        if (skillToUse.SkillType != SkillType.Melee)
                        {
                            var moveToSpeed = skillToUse.MoveToSpeed;
                            if(Combat.ScaleSkillMoveByCast)
                            {
                                moveToSpeed *= casterSpeed;
                            }
                            movingToAction = RPGActionFactory.MoveToPosition(target, approachRange, moveToSpeed);
                        }
                        else
                        {
                            var moveToSpeed = meleeSkill.Details.MeleeMoveDefinitions[currentMeleeAttack].MoveToSpeed;
                            if (Combat.ScaleSkillMoveByCast)
                            {
                                moveToSpeed *= casterSpeed;
                            }
                            movingToAction = RPGActionFactory.MoveToPosition(target, approachRange, moveToSpeed);
                        }
                    }
                    else
                    {
                        if (skillToUse.SkillType != SkillType.Melee)
                        {
                            var moveToSpeed = skillToUse.MoveToSpeed;
                            if (Combat.ScaleSkillMoveByCast)
                            {
                                moveToSpeed *= casterSpeed;
                            }
                            movingToAction = RPGActionFactory.MoveToPosition(targetPos, approachRange, moveToSpeed);
                        }
                        else
                        {
                            var moveToSpeed = meleeSkill.Details.MeleeMoveDefinitions[currentMeleeAttack].MoveToSpeed;
                            if (Combat.ScaleSkillMoveByCast)
                            {
                                moveToSpeed *= casterSpeed;
                            }
                            movingToAction = RPGActionFactory.MoveToPosition(targetPos, approachRange, moveToSpeed);

                        }
                    }

                    var movingToPrefab = skillToUse.SkillType != SkillType.Melee ? skillToUse.MovingToPrefab : meleeSkill.Details.MeleeMoveDefinitions[currentMeleeAttack].MovingToPrefab;
                    if (!string.IsNullOrEmpty(movingToPrefab))
                    {
                        Action<GameObject> myAction = o => o.GetComponent<DestroyHelper>().Init(DestroyCondition.ActionNotPlaying, _controller, movingToAction.ID);
                        queue.Add(RPGActionFactory.SpawnPrefab(movingToPrefab, transform.position, transform.rotation, myAction, transform));
                    }


                    if (skillToUse.SkillType == SkillType.Melee)
                    {

                        if (_controller.Character.AnimationType == RPGAnimationType.Legacy)
                        {
                            queue.Add(movingToAction).WithAnimation(meleeSkillDefinition.Definitions[currentMeleeAttack].ApproachLegacyAnim);
                        }
                        else
                        {
                            var animDef = new AnimationDefinition
                            {
                                Name = "Approach Skill Anim",
                                Animation = castAnimName,
                                MecanimAnimationGroup = meleeSkillDefinition.Definitions[currentMeleeAttack].ApproachSkillAnimSet,
                                MecanimAnimationNumber = meleeSkillDefinition.Definitions[currentMeleeAttack].ApproachAnimNumber,
                                RPGAnimationSet = RPGAnimationSet.Skill,
                                WrapMode = WrapMode.Loop,
                                Speed = 1 / (castTime / castLength),
                                Backwards = false
                            };
                            queue.Add(movingToAction).WithAnimation(animDef);
                        }

                    }
                    else
                    {
                        if (_controller.Character.AnimationType == RPGAnimationType.Legacy)
                        {
                            queue.Add(movingToAction).WithAnimation(animations.ApproachLegacyAnim);
                        }
                        else
                        {
                            var animDef = new AnimationDefinition
                            {
                                Name = "Approach Skill Anim",
                                Animation = castAnimName,
                                MecanimAnimationGroup = animations.ApproachSkillAnimSet,
                                MecanimAnimationNumber = animations.ApproachAnimNumber,
                                RPGAnimationSet = RPGAnimationSet.Skill,
                                WrapMode = WrapMode.Loop,
                                Speed = 1 / (castTime / castLength),
                                Backwards = false
                            };
                            queue.Add(movingToAction).WithAnimation(animDef);
                        }
                    }

                    queue.Add(RPGActionFactory.EndMecanimCasting());
                }
                else if (skillMoveType == SkillMovementType.JumpTo)
                {
                    RPGAction jumpToAction;
                    queue.Add(RPGActionFactory.StartMecanimCasting());
                    if (hasCharacterTarget)
                    {
                        if(skillToUse.SkillType != SkillType.Melee)
                        {
                            var moveToSpeed = skillToUse.MoveToSpeed;
                            var jumpToHeight = skillToUse.JumpToHeight;
                            if (Combat.ScaleSkillMoveByCast)
                            {
                                moveToSpeed *= casterSpeed;
                            }
                            jumpToAction = RPGActionFactory.JumpToPosition(target, jumpToHeight, moveToSpeed);    
                        }
                        else
                        {
                            var moveToSpeed = meleeSkill.Details.MeleeMoveDefinitions[currentMeleeAttack].MoveToSpeed;
                            var jumpToHeight = meleeSkill.Details.MeleeMoveDefinitions[currentMeleeAttack].JumpToHeight;
                            if (Combat.ScaleSkillMoveByCast)
                            {
                                moveToSpeed *= casterSpeed;
                            }
                            jumpToAction = RPGActionFactory.JumpToPosition(target, jumpToHeight, moveToSpeed);    
                        }
                    }
                    else
                    {
                        if (skillToUse.SkillType != SkillType.Melee)
                        {
                            var moveToSpeed = skillToUse.MoveToSpeed;
                            var jumpToHeight = skillToUse.JumpToHeight;
                            if (Combat.ScaleSkillMoveByCast)
                            {
                                moveToSpeed *= casterSpeed;
                            }
                            jumpToAction = RPGActionFactory.JumpToPosition(targetPos, jumpToHeight, moveToSpeed);
                        }
                        else
                        {
                            var moveToSpeed = meleeSkill.Details.MeleeMoveDefinitions[currentMeleeAttack].MoveToSpeed;
                            var jumpToHeight = meleeSkill.Details.MeleeMoveDefinitions[currentMeleeAttack].JumpToHeight;
                            if (Combat.ScaleSkillMoveByCast)
                            {
                                moveToSpeed *= casterSpeed;
                            }
                            jumpToAction = RPGActionFactory.JumpToPosition(targetPos, jumpToHeight, moveToSpeed);
                        }
                    }

                    var movingToPrefab = skillToUse.SkillType != SkillType.Melee ? skillToUse.MovingToPrefab : meleeSkill.Details.MeleeMoveDefinitions[currentMeleeAttack].MovingToPrefab;
                    if (!string.IsNullOrEmpty(movingToPrefab))
                    {
                        Action<GameObject> myAction = o => o.GetComponent<DestroyHelper>().Init(DestroyCondition.ActionNotPlaying, _controller, jumpToAction.ID);
                        queue.Add(RPGActionFactory.SpawnPrefab(movingToPrefab, transform.position, transform.rotation, myAction, transform));
                    }

                    var jumpToAnimName = animations.ApproachLegacyAnim;
                    if(skillToUse.SkillType == SkillType.Melee)
                    {
                        jumpToAnimName = meleeSkillDefinition.Definitions[currentMeleeAttack].ApproachLegacyAnim;
                    }

                    var jumpTime = Vector3.Distance(targetPos, transform.position) / (float)jumpToAction.Params["MoveSpeed"];
                    var jumpAnimLength = !string.IsNullOrEmpty(jumpToAnimName) && _controller.Animation != null ? _controller.Animation[jumpToAnimName].length : 0.5f;

                    var animSpeed = 1.0f;
                    if (jumpTime > 0.5f)
                    {
                        animSpeed = 1 / (jumpTime / jumpAnimLength);
                    }

                    var jumpAnim = new AnimationDefinition
                    {
                        Animation = jumpToAnimName,
                        MecanimAnimationGroup = animations.ApproachSkillAnimSet,
                        MecanimAnimationNumber = animations.ApproachAnimNumber,
                        RPGAnimationSet = RPGAnimationSet.Skill,
                        WrapMode = WrapMode.Loop,
                        Speed = animSpeed,
                        Backwards = false
                    };

                    queue.Add(jumpToAction).WithAnimation(jumpAnim);
                    queue.Add(RPGActionFactory.EndMecanimCasting());
                }
            }

            var skillLandTime = skillToUse.LandTime;
            var landAnimName = animations.LandLegacyAnim;
            if (skillToUse.SkillType == SkillType.Melee)
            {
                if (meleeSkillDefinition != null)
                {
                    landAnimName = meleeSkillDefinition.Definitions[currentMeleeAttack].LandLegacyAnim;
                    skillLandTime = meleeSkill.Details.MeleeMoveDefinitions[currentMeleeAttack].LandTime;
                }
            }
            var landTime = skillLandTime / casterSpeed;

            var landLength = !string.IsNullOrEmpty(landAnimName) && _controller.Animation != null ? _controller.Animation[landAnimName].length : 0.5f;
            var landAnim = new AnimationDefinition
            {
                Animation = landAnimName,
                WrapMode = WrapMode.Loop,
                MecanimAnimationGroup = animations.LandSkillAnimSet,
                MecanimAnimationNumber = animations.LandAnimNumber,
                RPGAnimationSet = RPGAnimationSet.Skill,
                Speed = 1 / (landTime / landLength),
                Backwards = false
            };

            //Handle Skill

            if ( target == null || (!target.Character.ImmuneTo(skillToUse) || new []{SkillType.Spawn, SkillType.Projectile, SkillType.Area_Of_Effect}.Any(s => s == skillToUse.SkillType)))
            {
                Damage oldDamage = null;
                if (skillToUse.SkillType == SkillType.Melee)
                {
                    oldDamage = new Damage(skillToUse.Damage);
                    skillToUse.Damage.ApplyMultiplier(meleeSkill.Details.MeleeSkillScalings[currentMeleeAttack]);
                }
                var handleActions = hasCharacterTarget ? HandleSkill(queue, skillToUse, target) : HandleSkill(queue, skillToUse, targetPos);
                if (skillToUse.SkillType == SkillType.Melee)
                {
                    skillToUse.SkillStatistics[skillToUse.CurrentRank].Damage = oldDamage;
                }

                foreach (var handleAction in handleActions)
                {
                    if (handleAction.Type == RPGActionType.SpawnPrefab)
                    {
                        handleAction.Params["Rotation"] = rotation;
                    }
                }
                queue.Actions.AddRange(handleActions);
            }
            else
            {
                var damage = new Damage {SkillMetaID = skillToUse.SkillMetaID};
                queue.Add(RPGActionFactory.DamageTarget(target, damage, null, null, skillToUse));
            }


            if (skillToUse.GivePlayerItem && CharacterMono.Character.CharacterType == CharacterType.Player)
            {
                queue.Add(RPGActionFactory.GivePlayerItem(ItemGroup.Normal, skillToUse.ItemToGiveID, skillToUse.ItemToGiveAmount));
            }

            if (skillToUse.AddResourceOnCast)
            {
                Action<IRPGController> myAction = o => o.Character.GetVitalByID(skillToUse.ResourceAddedID).CurrentValue += skillToUse.ResourceAddedAmount;
                queue.Add(RPGActionFactory.DoAction(_controller, myAction));
            }

            //Cleanup
            if (skillMoveType != SkillMovementType.StayInPlace)
            {
                if (IsMecanim || !string.IsNullOrEmpty(landAnimName))
                {
                    queue.Add(RPGActionFactory.PlayAnimation(landAnim));
                }

                queue.Add(RPGActionFactory.WaitForSeconds(landTime * impactTime));
            }

            //Done
            if (skillMoveType == SkillMovementType.StayInPlace)
            {
                queue.Add(RPGActionFactory.WaitForSeconds(castTime * postImpactTime)).WithCancellable();
            }
            else if (skillMoveType == SkillMovementType.MoveTo || skillMoveType == SkillMovementType.JumpTo)
            {
                var landPrefab = skillToUse.SkillType != SkillType.Melee ? skillToUse.LandPrefab : meleeSkill.Details.MeleeMoveDefinitions[currentMeleeAttack].LandPrefab;
                if (!string.IsNullOrEmpty(landPrefab))
                {
                    Action<GameObject> myAction = o =>
                                                      {
                                                          o.GetComponent<DestroyHelper>().Init(DestroyCondition.Time,ImpactPrefabTime);
                                                          o.transform.localPosition = Vector3.zero;
                                                          o.transform.localRotation = Quaternion.identity;
                                                      };

                    queue.Add(RPGActionFactory.SpawnPrefab(landPrefab, transform.position, transform.rotation, myAction, transform));
                }
                queue.Add(RPGActionFactory.WaitForSeconds(landTime * postImpactTime));
            }

            if (meleeSkill != null)
            {
                meleeSkill.CurrentAttack++;
            }

        }

        private List<RPGAction> HandleSkill(RPGActionQueue queue, Skill skillToUse, Vector3 targetPosition)
        {
            return HandleSkill(queue, skillToUse, null, targetPosition);
        }

        private List<RPGAction> HandleSkill(RPGActionQueue queue, Skill skillToUse, BaseCharacterMono target)
        {
            return HandleSkill(queue, skillToUse, target, null);
        }

        private List<RPGAction> HandleSkill(RPGActionQueue queue, Skill skillToUse, BaseCharacterMono target, Vector3? nullableTargetPos)
        {

            if (skillToUse.SkillType == SkillType.Area_Of_Effect || skillToUse.SkillType == SkillType.Spawn)
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

            if (target == null && !nullableTargetPos.HasValue) return new List<RPGAction>();
            if (target != null && !target.Character.Alive) return new List<RPGAction>();

            var hasCharacterTarget = target != null;
            var targetPos = nullableTargetPos.GetValueOrDefault();

            var rpgActions = new List<RPGAction>();
            var damageToDeal = skillToUse.Damage;

            if (skillToUse.SkillType == SkillType.Area_Of_Effect)
            {
                var aoeSkill = (AreaOfEffectSkill)skillToUse;
                Action<GameObject> doAction = o => o.GetComponent<AreaOfEffectSkillHandler>().Init(
                    aoeSkill);
                var newTargetPos = targetPos + new Vector3(0, aoeSkill.Height / 2, 0);
                queue.TargetPos = newTargetPos;
                var prefabAction = RPGActionFactory.SpawnPrefab(skillToUse.PrefabPath, newTargetPos, Quaternion.identity, doAction, null).UsingQueuePosition();
                rpgActions.Add(prefabAction);
            }
            else if(skillToUse.SkillType == SkillType.Projectile)
            {
                var projSkill = (ProjectileSkill)skillToUse;

                var projectilePosition = new Vector3();
                if(!hasCharacterTarget)
                {                    
                    projectilePosition = targetPos;
                    projectilePosition.y = (transform.position + (transform.up/2)).y;
                }

                Action<GameObject> doAction = o => o.GetComponent<ProjectileSkillHandler>().Init(
                    projSkill, hasCharacterTarget ? target : null, hasCharacterTarget ? Vector3.zero : projectilePosition);

                var casterTransform = CharacterMono.transform;
                var newTargetPos = casterTransform.position + (casterTransform.right / 2.6f) + (casterTransform.forward*1.2f) + (casterTransform.up * 1.2f);
 
                queue.TargetPos = newTargetPos;
                var prefabAction = RPGActionFactory.SpawnPrefab(skillToUse.PrefabPath, newTargetPos, Quaternion.identity, doAction, null);
                rpgActions.Add(prefabAction);
            }
            else if(skillToUse.SkillType == SkillType.Spawn)
            {
                var spawnSkill = skillToUse;
                //todo: spawnSkill y offset

                Action<GameObject> doAction = o => o.GetComponent<SpawnSkillHandler>().Init(spawnSkill, target);
                var newTargetPos = targetPos + new Vector3(0,1,0);
                queue.TargetPos = newTargetPos;
                var prefabAction = RPGActionFactory.SpawnPrefab(skillToUse.PrefabPath, newTargetPos, Quaternion.identity, doAction, null);
                rpgActions.Add(prefabAction);

            }
            else if (hasCharacterTarget)
            {
                if (damageToDeal.MaxTotal > 0)
                {
                    var onHitActions = new List<RPGAction>();
                    var onCritHitActions = new List<RPGAction>();

                    if (skillToUse.ApplyDOTOnHit)
                    {
                        var canApply = Random.Range(0, 100 + 1) <= (int) (skillToUse.ChanceToApplyDOT*100);
                        if (canApply)
                        {
                            skillToUse.DamageOverTime.SkillMetaID = skillToUse.SkillMetaID;
                            onHitActions.Add(RPGActionFactory.AddDoT(target, skillToUse.DamageOverTime));
                        }
                    }

                    if (skillToUse.RunEventOnHit)
                    {
                        if (!string.IsNullOrEmpty(skillToUse.EventOnHitID))
                        {
                            onHitActions.Add(RPGActionFactory.RunEvent(skillToUse.EventOnHitID));
                        }
                    }

                    if (skillToUse.HasProcEffect)
                    {
                        var procactions = GetProcActions(queue, skillToUse.ProcEffect, target, skillToUse);

                            if (skillToUse.ProcEffect.ProcCondition == Rm_ProcCondition.On_Hit ||
                                skillToUse.ProcEffect.ProcCondition == Rm_ProcCondition.Chance_On_Hit)
                            {
                                onHitActions.AddRange(procactions);
                            }
                            else if (skillToUse.ProcEffect.ProcCondition == Rm_ProcCondition.Chance_On_Critical_Hit)
                            {
                                onCritHitActions.AddRange(procactions);
                            }
                            else if (skillToUse.ProcEffect.ProcCondition == Rm_ProcCondition.Every_N_Hits)
                            {
                                onHitActions.AddRange(procactions);
                            }
                    }

                    damageToDeal.SkillMetaID = skillToUse.SkillMetaID;
                    rpgActions.Add(RPGActionFactory.DamageTarget(target, damageToDeal, onHitActions, onCritHitActions, skillToUse));
                    rpgActions.Add(RPGActionFactory.DealBonusTaunt(target,skillToUse.BonusTaunt));
                }

                //Skill specific:
                if(skillToUse.SkillType == SkillType.Aura)
                {
                    var auraSkill = (AuraSkill)skillToUse;
                    rpgActions.Add(RPGActionFactory.AddAuraEffect(target, auraSkill));
                }
                else if(skillToUse.SkillType == SkillType.Restoration)
                {
                    var restoSkill = (RestorationSkill) skillToUse;
                    rpgActions.Add(RPGActionFactory.AddRestorationEffect(target, restoSkill.Restoration));
                }


                if (skillToUse.AppliesBuffDebuff)
                {
                    rpgActions.Add(RPGActionFactory.AddTimedPassiveEffect(target, skillToUse.Effect));
                }

                if (skillToUse.ApplyStatusEffect)
                {
                    var apply = Random.Range(0, 100 + 1) <= (int) (skillToUse.ChanceToApplyStatusEffect*100);
                    if (apply)
                    {
                        rpgActions.Add(RPGActionFactory.AddStatusEffect(target, skillToUse.StatusEffectID,
                                                                        skillToUse.ApplyStatusEffectWithDuration, skillToUse.ApplyStatusEffectDuration));
                    }
                }
                if (skillToUse.RemoveStatusEffect)
                {
                    var apply = Random.Range(0, 100 + 1) <= (int) (skillToUse.ChanceToRemoveStatusEffect*100);
                    if (apply)
                    {
                        rpgActions.Add(RPGActionFactory.RemoveStatusEffect(target, skillToUse.RemoveStatusEffectID));
                    }
                }
            }
            else
            {
                //skills which can not have a target: AOE, SPAWN, PROJECTILE, ABILITY
                
                //RPGActionFactory.MoveWithoutCollision(Vector3.zero, range, speed)
            }

            if (skillToUse.UseEventOnCast)
            {
                rpgActions.Add(RPGActionFactory.RunEvent(skillToUse.EventOnCastID));
            }

            if (!string.IsNullOrEmpty(skillToUse.ImpactPrefabPath))
            {
                Action<GameObject> myAction = o => o.GetComponent<DestroyHelper>().Init(DestroyCondition.Time, ImpactPrefabTime);
                var prefabPos = hasCharacterTarget ? target.transform.Center() : targetPos;
                var prefabParent = hasCharacterTarget ? target.transform : null;
                rpgActions.Add(RPGActionFactory.SpawnPrefab(skillToUse.ImpactPrefabPath, prefabPos, Quaternion.identity, myAction, prefabParent).UsingQueuePosition());
            }
            rpgActions.Add(RPGActionFactory.PlaySound(skillToUse.ImpactSound, AudioType.SoundFX));

            return rpgActions;
        }

        public IEnumerable<RPGAction> GetProcActions(RPGActionQueue queue, Rm_ProcEffect procEffect, BaseCharacterMono target, Skill skillToUse)
        {
            var procactions = new List<RPGAction>();

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

            if(canProc)
            {
                //add actions
                if (procEffect.ProcEffectType == Rm_ProcEffectType.StatusEffect ||
                    procEffect.ProcEffectType == Rm_ProcEffectType.StatusEffectOnSelf)
                {
                    if (procEffect.ProcEffectType == Rm_ProcEffectType.StatusEffect)
                    {
                        procactions.Add(RPGActionFactory.AddStatusEffect(target, procEffect.EffectParameterString));
                    }
                    else
                    {
                        procactions.Add(RPGActionFactory.AddStatusEffect(CharacterMono, procEffect.EffectParameterString));
                    }
                }
                else if (procEffect.ProcEffectType == Rm_ProcEffectType.CastSkill ||
                         procEffect.ProcEffectType == Rm_ProcEffectType.CastSkillOnSelf)
                {

                    var allSkills = Rm_RPGHandler.Instance.Repositories.Skills.AllSkills;
                    var skill = allSkills.FirstOrDefault(s => s.ID == procEffect.EffectParameterString);
                    if (skill != null)
                    {
                        if (procEffect.ProcEffectType == Rm_ProcEffectType.CastSkill)
                        {
                            procactions.AddRange(HandleSkill(queue, skill, target));
                        }
                        else
                        {
                            procactions.AddRange(HandleSkill(queue, skill, CharacterMono));
                        }
                    }
                }
                else if (procEffect.ProcEffectType == Rm_ProcEffectType.KnockBack)
                {
                    procactions.Add(RPGActionFactory.KnockBack(target, Direction.Back, procEffect.EffectParameter));

                }
                else if (procEffect.ProcEffectType == Rm_ProcEffectType.KnockUp)
                {
                    procactions.Add(RPGActionFactory.KnockBack(target, Direction.Up, procEffect.EffectParameter));
                }
                else if (procEffect.ProcEffectType == Rm_ProcEffectType.PullTowards)
                {
                    var distance = -1f;
                    if(!procEffect.PullAllTheWay)
                    {
                        distance = procEffect.EffectParameter;
                    }

                    if(procEffect.PullType == Rm_PullTowardsType.CasterOrSkill)
                    {
                        procactions.Add(RPGActionFactory.PullTowards(target, CharacterMono,distance));    
                    }
                    else if(procEffect.PullType == Rm_PullTowardsType.TargetedPosition)
                    {
                        if(skillToUse.MovementType == SkillMovementType.StayInPlace)
                        {
                            var targetPullPos = _controller.GetPositionAtMousePosition();
                            if(Vector3.Distance(transform.position, targetPullPos) < skillToUse.CastRange)
                            {
                                procactions.Add(RPGActionFactory.PullTowards(target, targetPullPos, distance));        
                            }
                        }
                        else
                        {
                            procactions.Add(RPGActionFactory.PullTowards(target, null, distance).UsingQueuePosition());        
                        }
                    }
                }
            }
            

            return procactions;
        }

        public void Attack(BaseCharacterMono target, Vector3 position = default(Vector3))
        {
            if (target != null && !target.Character.Alive) return;
            if (_controller.HandlingActions) return;
            if (target != null && _controller.IsFriendly(target)) return;

            //Core information
            var attackSpeed = Character.AttackSpeed;
            var attackInterval = 1 / attackSpeed;
            var timeSinceLastAttack = Time.time - LastAttackTime;
            if (timeSinceLastAttack < attackInterval)
            {
                return;
            }
            else
            {
                _controller.ForceStopHandlingActions();
            }

            var attackRange = Character.AttackRange + 0.25f + (target != null ? target.Controller.NavMeshAgent.radius : 0 );
            var animationToUse = GetDefaultAttackAnimToUse();
            var animationName = animationToUse != null && _controller.Animation != null ? animationToUse.Animation : "";
            var animLength = animationToUse != null && _controller.Animation != null ? _controller.Animation[animationName].length / animationToUse.Speed : 0.5f;
            var impactTime = animationToUse != null && _controller.Animation != null ? animationToUse.ImpactTime : 0.5f;
            var postImpactTime = (1 - impactTime) * 0.6f;

            var attackDuration = attackInterval * 0.7f;
            var animSpeedScaling = animLength / attackDuration;

            if(animSpeedScaling > 1.00f)
            {
                animSpeedScaling = animLength/attackInterval;
                attackDuration = attackInterval;
            }


            //var damageToDeal = CombatCalcEvaluator.EvaluateDamageDealt(Character);
            var damageToDeal = Character.DamageDealable;

            //Animation speed scaling:
            if (animLength > attackDuration)
            {
                if (animationToUse != null)
                {
                    animationToUse.Speed *= animSpeedScaling;
                }
                animLength = attackDuration;
            }

            //Start queue
            var queue = RPGActionQueue.Create();
            queue.Identifier = "RPG_Combat";
            queue.HasTarget = position == Vector3.zero;
            queue.HasTargetPos = position != Vector3.zero;
            queue.Target = target;
            queue.TargetPos = position;

            if(queue.HasTarget && Vector3.Distance(transform.position,queue.Target.transform.position) > attackRange)
            {
                queue.Add(RPGActionFactory.MoveToPosition(target, attackRange)).WithAnimation(_controller.LegacyAnimation.RunAnim).WithCancellable();    
            }

            var impactPath = "";
            var meleeEffectPath = "";
            AudioContainer impactSound = null;
            var attackStyle = Character.AttackStyle;
            if (Character.CharacterType == CharacterType.Player)
            {
                var player = (PlayerCharacter)Character;
                var hasWeapon = !player.Equipment.Unarmed;
                var weapon = (player.Equipment.EquippedWeapon ?? player.Equipment.EquippedOffHand) as Weapon;

                if (hasWeapon)
                {
                    var wepDef = Rm_RPGHandler.Instance.Items.WeaponTypes.First(w => w.ID == weapon.WeaponTypeID);
                    attackStyle = wepDef.AttackStyle;
                }
            }

            if(attackStyle == AttackStyle.Melee)
            {

                if (Character.CharacterType == CharacterType.Player)
                {
                    var classDef = Rm_RPGHandler.Instance.Player.CharacterDefinitions.First(c => c.ID == ((PlayerCharacter)Character).PlayerCharacterID);
                    meleeEffectPath = classDef.AutoAttackPrefabPath;
                    impactPath = classDef.AutoAttackImpactPrefabPath;
                    impactSound = classDef.AutoAttackImpactSound;
                }
                else
                {
                    var cc = (CombatCharacter)Character;
                    meleeEffectPath = cc.AutoAttackPrefabPath;
                    impactPath = cc.AutoAttackImpactPrefabPath;
                    impactSound = cc.AutoAttackImpactSound;
                }

                if (Character.CharacterType == CharacterType.Player)
                {
                    var player = (PlayerCharacter)Character;
                    var weapon = player.Equipment.EquippedWeapon as Weapon;
                    weapon = weapon ?? player.Equipment.EquippedOffHand as Weapon;
                    if (weapon != null)
                    {
                        var wepDef = Rm_RPGHandler.Instance.Items.WeaponTypes.First(w => w.ID == weapon.WeaponTypeID);
                        meleeEffectPath = wepDef.AutoAttackPrefabPath;
                        impactPath = wepDef.AutoAttackImpactPrefabPath;
                        impactSound = wepDef.AutoAttackImpactSound;
                    }
                }

                if (!string.IsNullOrEmpty(meleeEffectPath))
                {
                    Action<GameObject> myAction = o => o.GetComponent<DestroyHelper>().Init(DestroyCondition.Time, 0.25f);
                    var posForMeleeEffect = transform.Center() + transform.forward;
                    queue.Add(RPGActionFactory.SpawnPrefab(meleeEffectPath, posForMeleeEffect, transform.rotation, myAction));
                }
            }

            queue.Add(RPGActionFactory.SetLastAttackTime());

            if (animationToUse != null)
            {
                queue.Add(RPGActionFactory.PlayAnimation(animationToUse));
            }

            queue.Add(RPGActionFactory.WaitForSeconds(animLength*impactTime)).FacingQueueTarget();
            if(Rm_RPGHandler.Instance.Combat.TargetStyle == TargetStyle.TargetLock)
            {
                if (attackStyle == AttackStyle.Melee)
                {
                    queue.Add(RPGActionFactory.DamageTarget(target, damageToDeal));

                    Action<GameObject> myAction = o => o.GetComponent<DestroyHelper>().Init(DestroyCondition.Time, ImpactPrefabTime);
                    queue.Add(RPGActionFactory.SpawnPrefab(impactPath, target.transform.Center(), target.transform.rotation, myAction, target.transform));
                    queue.Add(RPGActionFactory.PlaySound(impactSound, AudioType.SoundFX));
                }
                else
                {
                    queue.Add(RPGActionFactory.AutoAttack(target, damageToDeal, Vector3.zero));
                }

                
            }
            else
            {
                if(queue.HasTarget) 
                {
                    queue.Add(RPGActionFactory.AutoAttack(target, damageToDeal, Vector3.zero));    
                }
                else
                {
                    queue.Add(RPGActionFactory.AutoAttack(null, damageToDeal, position));    
                }
                
            }

            queue.Add(RPGActionFactory.WaitForSeconds(animLength * postImpactTime));

            if(_controller.BeginActionQueue(queue))
            {
                _controller.InCombat = true;
                if(Character.CharacterType != CharacterType.Player)
                {
                    var cc = (CombatCharacter) Character;
                    cc.AttackCounter++;
                }
            }

        }

        public void Attack(Transform target)
        {
            if (target == null) return;

            var targetMono = target.GetComponent<BaseCharacterMono>();
            if (targetMono != null)
            {
                Attack(targetMono);
            }
        }

        private AnimationDefinition GetDefaultAttackAnimToUse()
        {
            //todo: implement weapon specific

            var legacyAnim = _controller.LegacyAnimation;

            var player = Character as PlayerCharacter;
            if(player != null && player.Equipment.Unarmed)
            {
                var animToUse = legacyAnim.UnarmedAnim;

                if(!string.IsNullOrEmpty(animToUse.Animation))
                    return GeneralMethods.CopyObject(animToUse);
            }

            if (player != null && !player.Equipment.Unarmed)
            {
                var equippedWep = player.Equipment.EquippedWeapon as Weapon;
                equippedWep = equippedWep ?? player.Equipment.EquippedOffHand as Weapon;

                var wepDef = equippedWep != null ? legacyAnim.WeaponAnimations.FirstOrDefault(w => w.WeaponTypeID == equippedWep.WeaponTypeID) : null;
                if(wepDef != null)
                {
                    var offHand = player.Equipment.EquippedOffHand != null ?  player.Equipment.EquippedOffHand as Weapon : null;
                    List<AnimationDefinition> anims;

                    if(offHand != null)
                    {
                        anims = wepDef.DualWieldAnimations;
                    }
                    else
                    {
                        anims = wepDef.Animations;
                    }

                    if(anims.Count > 0)
                    {
                        var animToUse = anims[Random.Range(0, anims.Count)];
                        return GeneralMethods.CopyObject(animToUse);
                    }
                }
            }
            
            if (player != null)
            {
                var default2HAnimations = legacyAnim.Default2HAttackAnimations;
                var defaultDWAnimations = legacyAnim.DefaultDWAttackAnimations;

                var equippedWep = player.Equipment.EquippedWeapon as Weapon;
                var offHand = player.Equipment.EquippedOffHand != null ? player.Equipment.EquippedOffHand as Weapon : null;

                List<AnimationDefinition> anims = null;

                if (equippedWep != null && offHand != null)
                {
                    anims = defaultDWAnimations;
                }
                else if(equippedWep != null)
                {
                    if(Rm_RPGHandler.Instance.Items.WeaponTypes.First(w => w.ID == equippedWep.WeaponTypeID).IsTwoHanded)
                    {
                        anims = default2HAnimations;
                    }
                }

                if (anims != null && anims.Count > 0)
                {
                    var animToUse = anims[Random.Range(0, anims.Count)];
                    return GeneralMethods.CopyObject(animToUse);
                }
            }

            var defaultAnimations = legacyAnim.DefaultAttackAnimations;
            if (defaultAnimations.Count > 0)
            {
                var animToUse = defaultAnimations[Random.Range(0, defaultAnimations.Count)];
                return GeneralMethods.CopyObject(animToUse);
            }
            else
            {
                return null;
            }
        }

        public void EnterCombat(Transform combatantTransform)
        {
            
            if (CharacterMono == null  || CharacterMono.transform == null || combatantTransform == null) return;
            if(CharacterMono.transform == combatantTransform)
            {
                Debug.Log(Character.Name + " is trying to attack itself.");
                return;
            }

            if(!_controller.RetreatingToSpawn)
            {
                _controller.TimeOutOfCombat = 0f;
                _controller.InCombat = true;

                if (_controller.Target == null && (_controller.Character.CharacterType != CharacterType.Player || Rm_RPGHandler.Instance.Combat.TargetStyle == TargetStyle.TargetLock))
                {
                    _controller.Target = combatantTransform;    
                }
            }
            else
            {
                _controller.TimeOutOfCombat = 0f;
            }
        }

        public DamageOutcome DamageTarget(BaseCharacterMono target, Damage damageToDeal)
        {
            if (!target.Character.Alive)
            {
                _controller.Target = null;
                return null;
            }

            var outcome = DealDamageToTarget(target, damageToDeal);
            if(outcome != null)
            {
                if (_controller.IsPlayerControlled)
                {
                    HandleOutcome(target, outcome);
                }

                if (Character.CharacterType != CharacterType.Player)
                {
                    var lockHandler = GetComponent<TargetLockHandler>();
                    if (lockHandler != null && lockHandler.State == TargetLockState.Selected)
                    {
                        var isInCombat = _controller.Target == GetObject.PlayerMono.transform;
                        lockHandler.ChangeState(isInCombat ? TargetLockState.InCombat : TargetLockState.Selected);
                    }
                }

                if(target != null)
                {
                    EnterCombat(target.transform);
                }

                if (!outcome.KilledTarget)
                {
                    target.Controller.RPGCombat.EnterCombat(transform);
                    if(target.Controller.Animation != null && !target.Controller.HandlingActions)
                    {
                        var queue = new RPGActionQueue();
                        var getHitAnim = target.Controller.LegacyAnimation.TakeHitAnim;
                        if((IsMecanim || !string.IsNullOrEmpty(getHitAnim.Animation)) && target.Controller.IsGrounded)
                        {
                            queue.Add(RPGActionFactory.PlayAnimation(getHitAnim));
                            var animLength = target.Controller.Animation[getHitAnim.Animation].length;
                            queue.Add(RPGActionFactory.WaitForSeconds(animLength).WithCancellable());
                            target.Controller.BeginActionQueue(queue);  
                        }
                    }
                }
                else
                {
                    _controller.Target = null;
                    if(_controller.Character.CharacterType != CharacterType.Player)
                    {
                        _controller.InCombat = false;    
                    }
                    if(target.Controller.Target == null)
                    {
                        target.Controller.Target = transform;
                    }
                }
            }
            return outcome;
        }

        private void HandleOutcome(BaseCharacterMono target, DamageOutcome outcome)
        {
            if (!outcome.KilledTarget) return;

            var combatChar = (CombatCharacter)target.Character;

            var x = GetObject.PlayerSave.QuestLog.ActiveObjectives;
            var activeConditions = x.SelectMany(y => y.ActiveConditions).ToList();
            var conditions = activeConditions.Select(a => a as KillCondition);
            var interactConditions = conditions.Where(c => c != null);
            var foundCondition = interactConditions.FirstOrDefault(i => i.CombatantID == combatChar.ID);
            if (foundCondition != null)
            {
                foundCondition.NumberKilled += 1;
                if (foundCondition.NumberKilled >= foundCondition.NumberToKill)
                {
                    foundCondition.NumberKilled = foundCondition.NumberToKill;
                    foundCondition.IsDone = true;
                    RPG.Events.OnQuestStatusUpdate();
                }
            }

            if (target.Character.CharacterType == CharacterType.NPC)
            {
                var npcChar = target.Character as NonPlayerCharacter;
                if (!npcChar.CanBeKilled)
                {
                    return;
                }
            }

            LootSpawner.Instance.SpawnLoot(target);
            GetObject.PlayerSave.GenericStats.MonstersKilled++;
            var playerCharacter = (PlayerCharacter)Character;
            playerCharacter.AddProgression(combatChar.ProgressionGain);
        }

        private DamageOutcome DealDamageToTarget(BaseCharacterMono target, Damage damageToDeal)
        {
            var outcome = target.Character.VitalHandler.TakeDamage(Character, damageToDeal);
            
            if (Rm_RPGHandler.Instance.Combat.EnableTauntSystem)
            {
                if (target.Character.CharacterType != CharacterType.Player)
                {
                    var cc = ((CombatCharacter)target.Character);
                    var targetTaunt = cc.TauntHandler;
                    if (!targetTaunt.Tracker.ContainsKey(CharacterMono))
                    {
                        targetTaunt.Tracker.Add(CharacterMono, 1);
                    }
                    if(outcome.DamageDealt != null)
                    {
                        targetTaunt.Tracker[CharacterMono] += outcome.DamageDealt.Total;    
                    }
                    
                }
            }

            return outcome;
        }
    }
}
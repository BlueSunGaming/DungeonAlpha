//using System;
//using System.Collections;
//using LogicSpawn.RPGMaker.Core;
//using UnityEngine;
//using Random = UnityEngine.Random;
//
//namespace LogicSpawn.RPGMaker.Core
//{
//    public class AICombat : MonoBehaviour
//    {
//        public CombatState CombatState ;
//        public EnemyAIHandler EnemyAIHandler ;
//        public BaseCharacter Target ;
//        
//
//        private float _timeBetweenAttacks;
//        private float _timeSinceLastAttack;
//        public bool CanFight;
//        private bool _attacking;
//
//        public void BeginFight(Transform target)
//        {
//            Target = GetTarget(target);  
//        }
//
//        private BaseCharacter GetTarget(Transform target)
//        {
//            if (target.GetComponent<PlayerMono>() != null)
//            {
//                return target.GetComponent<PlayerMono>().Player;
//            }
//
//            if (target.GetComponent<EnemyCharacterMono>() != null)
//            {
//                return target.GetComponent<EnemyCharacterMono>().Player;
//            }
//
//            return null;
//        }
//
//        public void EndFight()
//        {
//            Target = null;
//            EnemyAIHandler.State = EnemyAIHandler.Character.Alive ? AIState.ReturnToSpawn : AIState.Dead;
//        }
//
//        void Update()
//        {
//            _timeSinceLastAttack += Time.deltaTime;
//            
//
//            if (!GameMaster.GameLoaded) return;
//
////            Array.ForEach(EnemyAIHandler.Character.AttackHandler.SkillsInUse,
////                s => { if (s != null) s.CurrentCoolDownTime += Time.deltaTime; });
//
//            if (!CanFight) return;
//
//            if (Target == null) BeginFight(EnemyAIHandler.Target);
//
//            if (_attacking) return;
//
//            //Debug.Log("Attack speed: " + AIHandler.Character.GetStatByID(StatisticName.Attack_Speed).TotalValue);
////            _timeBetweenAttacks = 1 / EnemyAIHandler.Character.GetStatByID(StatisticName.Attack_Speed).TotalValue;
//            var canAttack = _timeSinceLastAttack > _timeBetweenAttacks;
//
//            Skill skillToUse = null;
//            if (canAttack)
//            {
//                //skillToUse = AIHandler.Character.AttackHandler.SkillsInUse.FirstOrDefault(s => s.CurrentCoolDownTime > s.CoolDownTime);
////                for (int i = 0; i < EnemyAIHandler.Character.AttackHandler.SkillsInUse.Length; i++)
////                {
////                    var skillsInUse = EnemyAIHandler.Character.AttackHandler.SkillsInUse;
////                    if (skillsInUse[i] == null) continue;
////
////                    if (skillsInUse[i].CurrentCoolDownTime > skillsInUse[i].CoolDownTime)
////                    {
////                        skillToUse = skillsInUse[i];
////                        break;
////                    }
////                }
//                CombatState = skillToUse != null ? CombatState.UseASpell : CombatState.AutoAttack;
//            }
//            else
//            {
//                CombatState = CombatState.CombatIdle;
//            }
//
//            switch (CombatState)
//            {
//                case CombatState.CombatIdle:
//                    EnemyAIHandler.Animation.CrossFade(EnemyAIHandler.AnimationNames.CombatIdleAnimationName);
//                    break;
//                case CombatState.AutoAttack:
//                    _timeSinceLastAttack = 0;
//                    StartCoroutine(UseAttack());
//                    break;
//                case CombatState.UseASpell:
//                    _timeSinceLastAttack = 0;
//                    StartCoroutine(UseSkill(skillToUse));
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException();
//            }
//        }
//
//        IEnumerator UseAttack()
//        {
////            _attacking = true;
////            var animationToUse =
////                EnemyAIHandler.AnimationNames.AttackAnimations[Random.Range(0, EnemyAIHandler.AnimationNames.AttackAnimations.Count)];
////
////            EnemyAIHandler.Animation[animationToUse].speed = (EnemyAIHandler.Animation[animationToUse].length / _timeBetweenAttacks * 0.8f);
////            EnemyAIHandler.Animation.CrossFade(animationToUse);
////            yield return new WaitForSeconds(EnemyAIHandler.Animation[animationToUse].length * 0.20f);
////
////            Debug.Log("Dealing auto-attack damage:\n " + EnemyAIHandler.Character.GetDamage);
////            Target.VitalHandler.TakeDamage(EnemyAIHandler.Character, EnemyAIHandler.Character.GetDamage);
////            yield return new WaitForSeconds(EnemyAIHandler.Animation[animationToUse].length * 0.70f);
////            _attacking = false;
//            yield return new WaitForSeconds(0);
//        }
//
//        //TODO:
//        IEnumerator UseSkill(Skill skillToUse)
//        {
//            _attacking = true;
//
//            Damage damageToDeal = null;
////            var skillIndex = Array.FindIndex(EnemyAIHandler.Character.AttackHandler.SkillsInUse, s => s.Name == skillToUse.Name);
//            switch (skillToUse.SkillType)
//            {
//                case SkillType.Area_Of_Effect:
//                    Debug.Log(EnemyAIHandler.Character.Name + " used area of effect skill: " + skillToUse.Name);
//                    damageToDeal = skillToUse.Damage;
//                    break;
//                case SkillType.Projectile:
//                    Debug.Log(EnemyAIHandler.Character.Name + " used projectile skill: " + skillToUse.Name);
//                    damageToDeal = skillToUse.Damage;
//                    break;
//                case SkillType.Aura:
//                    Debug.Log(EnemyAIHandler.Character.Name + " used aura skill: " + skillToUse.Name);
//                    break;
//                case SkillType.Buff:
//                    Debug.Log(EnemyAIHandler.Character.Name + " used buff skill: " + skillToUse.Name);
//                    break;
//                case SkillType.Debuff:
//                    Debug.Log(EnemyAIHandler.Character.Name + " used debuff skill: " + skillToUse.Name);
//                    break;
//                case SkillType.Spawn:
//                    Debug.Log(EnemyAIHandler.Character.Name + " used spawn skill: " + skillToUse.Name);
//                    break;
//                case SkillType.Melee:
//                    Debug.Log(EnemyAIHandler.Character.Name + " used melee skill: " + skillToUse.Name);
//                    damageToDeal = skillToUse.Damage;
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException();
//            }
//
//            skillToUse.CurrentCoolDownTime = 0;
//            var animationToUse = skillToUse.AnimationToUse;
//            EnemyAIHandler.Animation[animationToUse].speed =
//                (EnemyAIHandler.Animation[animationToUse].length / _timeBetweenAttacks * 0.8f);
//            EnemyAIHandler.Animation.CrossFade(animationToUse);
//
//            yield return new WaitForSeconds(EnemyAIHandler.Animation[animationToUse].length * 0.20f);
//
//            if (damageToDeal != null)
//            {
//                Debug.Log("Dealing spell damage:\n " + EnemyAIHandler.Character.GetDamage);
//                Target.VitalHandler.TakeDamage(EnemyAIHandler.Character, damageToDeal);
//            }
//            
//            yield return new WaitForSeconds(EnemyAIHandler.Animation[animationToUse].length * 0.70f);
//
//            _attacking = false;
//        }
//    }
//
//    public enum CombatState
//    {
//        CombatIdle,
//        AutoAttack,
//        UseASpell
//    }
//}
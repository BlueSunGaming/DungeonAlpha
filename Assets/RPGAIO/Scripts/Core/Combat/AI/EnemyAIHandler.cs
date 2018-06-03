//using System;
//using System.Linq;
//using LogicSpawn.RPGMaker.Core;
//using LogicSpawn.RPGMaker.Generic;
//using UnityEngine;
//
//namespace LogicSpawn.RPGMaker.Core
//{
//
//    //TODO: We need options for controlling behaviour of Character combat, 
//    //e.g. "Aggresive mode: roam for kills always", "Backup: only fight monsters play fights", "Follow: just follow"
//
//    [RequireComponent(typeof(AIMovement))]
//    [RequireComponent(typeof(CharacterController))]
//    [RequireComponent(typeof(AnimationNames))]
//    public class EnemyAIHandler : MonoBehaviour
//    {
//        public Animation Animation;
//        public Transform Target;
//        public CombatCharacter Character ;
//        public AnimationNames AnimationNames ;
//        public AIMovement AIMovement ;
//        public AICombat AICombat ;
//
//        [HideInInspector]
//        public const float DefaultMoveCutOff = 0.6f;
//        public const float MaxAggroDistance = 1000.0f;
//        public AIState State;
//
//
//
//        public void Start()
//        {
//            State = AIState.Idle;
//            AnimationNames = Character.AnimationNames;
//            AIMovement = GetComponent<AIMovement>();
//            AICombat = GetComponent<AICombat>();
//            AICombat.EnemyAIHandler = AIMovement.EnemyAIHandler = this;
//            EnableAI();
//        }
//
//        void Update()
//        {
//            if (!GameMaster.GameLoaded) return;
//
//            if(!Character.Alive) State = AIState.Dead;
//
//            switch (State)
//            {
//                case AIState.UserControlled:
//                    break;
//                case AIState.Idle:
//                    Idle();
//                    break;
//                case AIState.MovingTotargetPosition:
//                    var targetPositionReached = MoveTotargetPosition(Target, DefaultMoveCutOff);
//                    State = targetPositionReached ? AIState.Idle : AIState.MovingTotargetPosition;
//                    break;
//                case AIState.Retreat:
//                    Retreat();
//                    break;
//                case AIState.ReturnToSpawn:
//                    ReturnToSpawn();
//                    break;
//                case AIState.Combat:
//                    DoCombat();
//                    break;
//                case AIState.Roaming:
//                    Roam();
//                    break;
//                case AIState.Dead:
//                    Die();
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException();
//            }
//        }
//
//        private void EnableAI()
//        {
//            AIMovement.enabled = true;
//            AICombat.enabled = true;
//        }
//
//        public void DisableAI()
//        {
//            AIMovement.enabled = false;
//            AICombat.enabled = false;
//        }
//
//        private void ReturnToSpawn()
//        {
//            var attargetPosition = MoveTotargetPosition(GameObject.Find("Home").transform, DefaultMoveCutOff);
//            if (attargetPosition)
//            {
//                State = AIState.Idle;
//            }
//        }
//
//        public bool MoveTotargetPosition(Transform target, float stopDistanceRange)
//        {
//            AIMovement.Target = target;
//            AIMovement.MovementState = MovementState.Moving;
//            var dist = Vector3.Distance(transform.position, target.position);
//            if (dist <= stopDistanceRange)
//            {
//                AIMovement.MovementState = MovementState.NotMoving;
//                return true;
//            }
//
//            return false;
//        }
//
//        private void Retreat()
//        {
//            //Spawn gameobject in emptyGameObject called RetreatPoints and assign to AIMovement
//            //Call MoveToTarget AIMovement.retreatTransform => once reached roam/idle
//            throw new NotImplementedException();
//        }
//
//        private void DoCombat()
//        {
//            Target = GetObject.PlayerMono.transform;
//            
//                
//            if(Target == null)
//            {
//                return;
//            }
//
//            Debug.Log("Range: " + Character.AttackHandler.OverrideAttackRange);
//            var attargetPosition = MoveTotargetPosition(Target, Character.AttackHandler.OverrideAttackRange);
//            if (AIMovement.DistanceFromTarget > MaxAggroDistance)
//            {
//                State = AIState.ReturnToSpawn;
//            }
//            if (!attargetPosition) return;
//            Debug.Log("Combat time!");
//            AICombat.CanFight = true;
//        }
//
//        private void Roam()
//        {
//            throw new NotImplementedException();
//        }
//
//        private void Die()
//        {
//            throw new NotImplementedException();
//        }
//
//        private void Idle()
//        {
//            Animation.Blend(AnimationNames.IdleAnimationName,1,0.05f);
//            AIMovement.MovementState = MovementState.NotMoving;
//
//            if (Character.IsAggressive) State = AIState.Combat;
//        }
//
//        public void MoveToTarget(Transform target)
//        {
//            Target = target;
//            State = AIState.MovingTotargetPosition;
//
//        }
//    }
//
//    public enum AIState
//    {
//        UserControlled,
//        Idle,
//        MovingTotargetPosition,
//        Retreat,
//        ReturnToSpawn,
//        Combat,
//        Roaming,
//        Dead
//    }
//}
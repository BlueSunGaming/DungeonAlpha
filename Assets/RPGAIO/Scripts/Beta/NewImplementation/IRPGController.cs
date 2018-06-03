using System;
using System.Collections;
using System.Collections.Generic;
using LogicSpawn.RPGMaker.Beta;
using LogicSpawn.RPGMaker.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Beta.NewImplementation
{
    public interface IRPGController
    {
        BaseCharacter Character { get; }
        GameObject CharacterModel { get; }
        RPGControllerState State { get; set; }
        Transform Target { get; set; }

        bool PlayerCanControl { get; }
        bool ControlledByAI { get; }
        bool HandlingActions { get; }
        bool IsCastingSkill { get; }
        bool Interacting { get; set; }
        bool RetreatingToSpawn { get; set; }
        bool IsPlayerControlled { get; }
        Vector3 SpawnPosition { get; set; }

        bool IsGrounded { get; }

        CharacterController CharacterController { get; }
        NavMeshAgent NavMeshAgent { get; }

        float Gravity { get; }

        float JumpHeight { get; }
        float MoveSpeed { get; }
        Vector3 Impact { get; }

        bool TargetReached { get; }
        bool AutoAttack { get; set; }
        bool InCombat { get; set; }

        //getters for our Controller, Movement, Animation and Combat modules
        IRPGAnimation RPGAnimation { get; set; }
        IRPGCombat RPGCombat { get; set; }
        LegacyAnimation LegacyAnimation { get; }
        float TimeOutOfCombat { get; set; }
        bool IsPlayerCharacter { get; set; }
        RPGAction CurrentAction { get; }
        RPGActionQueue CurrentQueue { get; }
        float MouseSensitivity { get; }
        BaseCharacterMono CharacterMono { get; }
        Animation Animation { get; }
        Animator Animator { get; }
        bool MovingForward { get; }
        bool Running { get; }


        void AddImpact(Direction direction, float wantedDistance);
        bool BeginActionQueue(RPGActionQueue queue);
        void Pause();
        void Resume();
        void ToggleAI(bool onOff);
        Vector3 GetApproachPosition(Vector3 approachPosition, float stopRange);
        void SetPlayerControl(GameObject combatant);
        void PullTo(Vector3 targetPos, float distance = -1);
        Vector3 GetPositionAtMousePosition();
        bool IsFriendly(BaseCharacterMono target);
        List<BaseCharacterMono> GetNearbyTargets(float radius);
        Transform GetNearestEnemy();
        Transform GetNearestAlly();
        List<BaseCharacterMono> GetNearbyAllies(float distance);
        List<BaseCharacterMono> GetNearbyEnemies(float distance);
        void ForceStopHandlingActions();

        void TryJump();
        void UseSkill(int i);
        void UseRefSkill(Skill skill);
    }
}
using System.Collections.Generic;
using LogicSpawn.RPGMaker.Beta;
using LogicSpawn.RPGMaker.Core;
using UnityEngine;

namespace Assets.Scripts.Beta.NewImplementation
{
    public interface IRPGCombat
    {
        void Attack(BaseCharacterMono target, Vector3 position = default(Vector3));
        void Attack(Transform target);
        void EnterCombat(Transform transform);
        DamageOutcome DamageTarget(BaseCharacterMono target, Damage damageToDeal);
        float LastAttackTime { get; set; }
        void UseSkill(Skill skillToUse, BaseCharacterMono target);
        void UseSkill(Skill skillToUse, Vector3 targetPos);
        void UseSkill(Skill skillToUse, Vector3 targetPos, Quaternion rotation);
        IEnumerable<RPGAction> GetProcActions(RPGActionQueue queue, Rm_ProcEffect procEffect, BaseCharacterMono target, Skill fromSkill);
    }
}
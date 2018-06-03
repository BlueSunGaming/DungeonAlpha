using System.Linq;
using LogicSpawn.RPGMaker.Beta;
using UnityEngine;
using UnityEngine.AI;

namespace LogicSpawn.RPGMaker.Core
{
    public class MeleeAutoAttackHandler : AutoAttackHitHandler
    {
        protected BaseCharacterMono Target;
        protected bool HaveTransform;

        public void Init(BaseCharacterMono caster, Damage damage, Vector3 targetPos, BaseCharacterMono target = null)
        {
            Caster = caster.Character;
            CasterMono = caster;
            _damage = damage;
            Target = target;
            HaveTransform = Target != null;

            transform.LookAt(new Vector3(targetPos.x, targetPos.y,targetPos.z));

            //resize for attack range and radius
            var boxCollider = GetComponent<BoxCollider>();
            var casterNavAgent = caster.GetComponent<NavMeshAgent>();
            var attackRange = caster.Character.AttackRange;
            boxCollider.size = new Vector3(casterNavAgent.radius + 0.25f, casterNavAgent.height, attackRange);
            transform.position += transform.forward * (attackRange / 2);
            transform.position += new Vector3(0, casterNavAgent.height/2, 0);

            if(caster.Character.CharacterType == CharacterType.Player )
            {
                var player = (PlayerCharacter)caster.Character;
                var classDef = Rm_RPGHandler.Instance.Player.CharacterDefinitions.First(c => c.ID == player.PlayerCharacterID);
                ImpactPrefabPath = classDef.AutoAttackImpactPrefabPath;
                ImpactSound = classDef.AutoAttackImpactSound;

                var weapon = player.Equipment.EquippedWeapon as Weapon;
                weapon = weapon ?? player.Equipment.EquippedOffHand as Weapon;
                if (weapon != null)
                {
                    var wepDef = Rm_RPGHandler.Instance.Items.WeaponTypes.First(w => w.ID == weapon.WeaponTypeID);
                    ImpactPrefabPath = wepDef.AutoAttackImpactPrefabPath;
                    ImpactSound = wepDef.AutoAttackImpactSound;
                }
            }
            else
            {
                var cc = (CombatCharacter)caster.Character;
                ImpactPrefabPath = cc.AutoAttackImpactPrefabPath;
                ImpactSound = cc.AutoAttackImpactSound;
            }
            
            Destroy(gameObject, 0.25f);
            _initialised = true;
        }

        void OnTriggerEnter(Collider other)
        {
            if (!Active) return;
            if (HaveTransform && other.transform != Target.transform) return;

            if (AddTarget(other.transform))
            {
                Active = false;
                Destroy(gameObject);
            }
        }
    }
}
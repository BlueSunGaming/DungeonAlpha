using System.Linq;
using LogicSpawn.RPGMaker.Beta;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class ProjectileAutoAttackHandler : AutoAttackHitHandler
    {
        protected BaseCharacterMono Target;
        protected Vector3 Direction;
        protected float _baseHeight;

        public float ProjectileSpeed;
        public GameObject SoundGameObject;
        protected AudioContainer TravelSound;


        public void Init(BaseCharacterMono caster, Damage damage, BaseCharacterMono target, Vector3 targetPos = default(Vector3))
        {
            Caster = caster.Character;
            CasterMono = caster;
            _damage = damage;
            Target = target;
            Direction = targetPos;
            _baseHeight = transform.localScale.y + 0.2f;
            transform.position = transform.position + new Vector3(0, _baseHeight, 0);

            var spawnPosition = caster.GetComponentInChildren<ProjectileSpawnPosition>();
            if(spawnPosition != null)
            {
                transform.position = spawnPosition.transform.position;
            }

            if (Direction != Vector3.zero)
            {
                transform.LookAt(Direction);
            }
            else if (target != null)
            {
                transform.LookAt(target.transform.position);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            //todo: hi

            if(caster.Character.CharacterType == CharacterType.Player )
            {
                var player = (PlayerCharacter)caster.Character;
                var classDef = Rm_RPGHandler.Instance.Player.CharacterDefinitions.First(c => c.ID == player.PlayerCharacterID);
                ProjectileSpeed = classDef.ProjectileSpeed;
                ImpactPrefabPath = classDef.AutoAttackImpactPrefabPath;
                ImpactSound = classDef.AutoAttackImpactSound;
                TravelSound = classDef.ProjectileTravelSound;

                var weapon = player.Equipment.EquippedWeapon as Weapon;
                weapon = weapon ?? player.Equipment.EquippedOffHand as Weapon;
                if (weapon != null)
                {
                    var wepDef = Rm_RPGHandler.Instance.Items.WeaponTypes.First(w => w.ID == weapon.WeaponTypeID);
                    ProjectileSpeed = wepDef.ProjectileSpeed;
                    ImpactPrefabPath = wepDef.AutoAttackImpactPrefabPath;
                    ImpactSound = wepDef.AutoAttackImpactSound;
                    TravelSound = wepDef.ProjectileTravelSound;
                }
            }
            else
            {
                var cc = (CombatCharacter)caster.Character;
                ProjectileSpeed = cc.ProjectileSpeed;
                ImpactPrefabPath = cc.AutoAttackImpactPrefabPath;
                ImpactSound = cc.AutoAttackImpactSound;
                TravelSound = cc.ProjectileTravelSound;
            }

            if (TravelSound.Audio != null)
            {
                SoundGameObject = AudioPlayer.Instance.Play(TravelSound.Audio, AudioType.SoundFX, transform.position, transform);
                SoundGameObject.GetComponent<AudioSource>().loop = true;
                SoundGameObject.AddComponent<DestroyHelper>().Init(DestroyCondition.GameObjectIsNull, gameObject);
            }

            Destroy(gameObject, 5.0f);
            _initialised = true;
        }

        void FixedUpdate()
        {
            if (!_initialised) return;

            if (LockOn)
            {
                if (Target == null)
                {
                    Destroy(gameObject);
                }
                else
                {
                    if (!Target.Character.Alive)
                    {
                        Destroy(gameObject);
                        return;
                    }
                    var targetPos = Target.transform.Center();
                    transform.LookAt(targetPos);
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, ProjectileSpeed * Time.deltaTime);
                }
            }
            else
            {
                transform.Translate(Vector3.forward * ProjectileSpeed * Time.deltaTime);
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (!_initialised) return;

            var other = collision.collider.transform;
            
            //need to get parent
            var parent = other;
            while (parent != null)
            {
                var characterMono = parent.GetComponent<BaseCharacterMono>();
                if (characterMono != null)
                {
                    other = characterMono.transform;
                    break;
                }
                parent = parent.parent;
            }

            if (!Active) return;
            if (LockOn && other.transform != Target.transform) return;
            if (CasterMono != null && other.transform == CasterMono.transform) return;
            if (CasterMono != null && other.transform == CasterMono.transform) return;

            var col = GetComponent<Collider>();
            Destroy(col);

            if (AddTarget(other))
            {
                Active = false;
                Destroy(gameObject);
            }
            else if (other.GetComponent<ProjectileSkillHandler>() == null &&
                other.GetComponent<MeleeAutoAttackHandler>() == null &&
                other.GetComponent<ProjectileAutoAttackHandler>() == null &&
                !other.CompareTag("IgnoreProjectiles"))
            {
                Active = false;
                Destroy(gameObject);
            }
        }
    }
}
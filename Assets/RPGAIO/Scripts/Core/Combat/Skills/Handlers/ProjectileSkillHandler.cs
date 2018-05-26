using System.Collections;
using System.Collections.Generic;
using LogicSpawn.RPGMaker.Beta;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class ProjectileSkillHandler : SkillHitHandler
    {
        private ProjectileSkill _projSkill;
        private BaseCharacterMono _charTarget;
        private Vector3 _direction;
        private float _baseHeight;

        protected internal int PierceCounter;

        private bool LockOn 
        {
            get
            {
                if (_projSkill.AlwaysLockOn) return true;
                if (Rm_RPGHandler.Instance.Combat.TargetStyle == TargetStyle.ManualTarget) return false;
                return _charTarget != null || _direction == Vector3.zero;
            }
        }
        public void Init(ProjectileSkill projSkill, BaseCharacterMono target, Vector3 targetPos = default(Vector3))
        {
            _projSkill = GeneralMethods.CopySkill(projSkill);
            _skill = _projSkill;
            _charTarget = target;
            _direction = targetPos;
            _baseHeight = transform.localScale.y/2;
            transform.position = transform.position + new Vector3(0,_baseHeight, 0);

            if(projSkill.CasterMono != null)
            {
                var spawnPosition = projSkill.CasterMono.GetComponentInChildren<ProjectileSpawnPosition>();
                if (spawnPosition != null)
                {
                    transform.position = spawnPosition.transform.position;
                } 
            }
            


            if(targetPos != Vector3.zero)
            {
                transform.LookAt(new Vector3(targetPos.x, transform.position.y, targetPos.z));
            }
            else if(target != null)
            {
                transform.LookAt(target.transform.position);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            if(_projSkill.TravelSound.Audio != null)
            {
                var sound = AudioPlayer.Instance.Play(_projSkill.TravelSound.Audio, AudioType.SoundFX, transform.position,transform);
                sound.GetComponent<AudioSource>().loop = true;
                sound.AddComponent<DestroyHelper>().Init(DestroyCondition.GameObjectIsNull, gameObject);
            }

            //todo: projectile radius: transform.localScale = new Vector3(_projSkill.Diameter, _projSkill.Diameter, _projSkill.Diameter);

            Destroy(gameObject, projSkill.TimeTillDestroy);
            _initialised = true;
            //Debug.Log("Initialised skill");
        }

        void FixedUpdate()
        {
            if (!_initialised) return;

            if(LockOn)
            {
                if(_charTarget == null)
                {
                    transform.Translate(Vector3.forward * _projSkill.Speed * Time.deltaTime);
                }
                else
                {
                    if(!_charTarget.Character.Alive)
                    {
                        Destroy(gameObject);
                        return;
                    }
                    var targetPos = _charTarget.transform.Center();
                    var projectileSpeed = 5;
                    transform.position = Vector3.Lerp(transform.position, targetPos, projectileSpeed * Time.deltaTime);
                }
                //Debug.Log("PROJECTILE AUTOLOCKING");

            }
            else
            {
                //Debug.Log("PROJECTILE MOVING IN DIRECTION");
                transform.Translate(Vector3.forward * _projSkill.Speed * Time.deltaTime);
            }
        }

        void OnCollisionEnter(Collision collision)
        {
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
            if (LockOn && other.transform != _charTarget.transform) return; 
            if (_projSkill.CasterMono != null && other.transform == _projSkill.CasterMono.transform) return;
            if (other.gameObject.layer == LayerMask.NameToLayer("Combatant") && other.GetComponent<BaseCharacterMono>() == null) return;
            
            if (AddTarget(other))
            {
                PierceCounter++;
            
                if (!_projSkill.IsPiercing || LockOn || PierceCounter >= _projSkill.NumberOfPierces)
                {
                    Active = false; 
                    var col = GetComponent<Collider>(); 
                    Destroy(col);
            
                    Destroy(gameObject);
                }
            }
            else if (!_projSkill.IsPiercing && other.GetComponent<ProjectileSkillHandler>() == null
                && other.GetComponent<MeleeAutoAttackHandler>() == null &&
                other.GetComponent<ProjectileAutoAttackHandler>() == null &&
                !other.CompareTag("IgnoreProjectiles"))
            {
                Active = false;
            
                var col = GetComponent<Collider>();
                Destroy(col);
                Destroy(gameObject);
            }
        }

        void OnCollisionExit(Collision collision)
        {
            var other = collision.collider;

            if(!_projSkill.IsPiercing) return;
            RemoveTarget(other);
        }
    }
}
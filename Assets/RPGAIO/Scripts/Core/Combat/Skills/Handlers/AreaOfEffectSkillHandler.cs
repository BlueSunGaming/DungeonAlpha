using System.Collections;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class AreaOfEffectSkillHandler : SkillHitHandler
    {
        private AreaOfEffectSkill _aoeSkill;
        private bool _multiHitStarted;

        public void Init(AreaOfEffectSkill aoeSkill)
        {
            _aoeSkill = GeneralMethods.CopySkill(aoeSkill);
            _skill = _aoeSkill;

            if(_aoeSkill.Shape == AOEShape.Sphere)
            {
                var colliderToRemove = GetComponent<BoxCollider>();
                if(colliderToRemove != null)
                {
                    Destroy(colliderToRemove);
                }
                transform.localScale = new Vector3(_aoeSkill.Diameter, _aoeSkill.Diameter, _aoeSkill.Diameter);
            }
            else
            {
                var colliderToRemove = GetComponent<SphereCollider>();
                if (colliderToRemove != null)
                {
                    Destroy(colliderToRemove);
                }

                if(Rm_RPGHandler.Instance.Combat.AutomaticallyScaleAOE)
                {
                    transform.localScale = new Vector3(_aoeSkill.Width, _aoeSkill.Height, _aoeSkill.Length);
                }
            }

            Destroy(gameObject, aoeSkill.Duration);
            _initialised = true;
            //Debug.Log("Initialised skill");
        }
        
        private IEnumerator DealAoeDamageMultipleTimes()
        {   
            HandleSkillToTargets();
            var wait = new WaitForSeconds(_aoeSkill.DelayBetweenHits); 

            while(true)
            {
                yield return wait;
                HandleSkillToTargets();
                yield return null;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if(AddTarget(other.transform) && !_multiHitStarted)
            {
                if (_aoeSkill.HitMultipleTimes)
                {
                    StartCoroutine(DealAoeDamageMultipleTimes());
                }
                _multiHitStarted = true;
            }
        }
        
        void OnTriggerExit(Collider other)
        {
            if(!_aoeSkill.HitMultipleTimes) return;
            RemoveTarget(other);
        }

        void OnDestroy()
        {
            //StopCoroutine("DealAoeDamageMultipleTimes");
        }
    }
}
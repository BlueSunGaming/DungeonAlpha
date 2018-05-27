using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace DungeonRPG.Weapons
{
    public class Weapon : MonoBehaviour //TODO: investigate difference between ScriptableObject & Mono Behaviour
    {
        //TODO: set default tag and layer (maybe a new Weapon Layer)
        public float minTimeBetweenHits = .5f;
        public float maxAttackRange = 2f;
        public GameObject weaponPrefab;
        public uint itemID;
        public AnimationClip attackAnimation;
        public Transform weaponGripTransform;

        [SerializeField] protected float m_fBaseDamage = 1.0f;
        
        public float GetMinTimeBetweenHit()
        {
            return minTimeBetweenHits;
        }

        public float GetMaxAttackRange()
        {
            return maxAttackRange;
        }

        public virtual float GetBaseDamage()
        {
            return m_fBaseDamage;
        }

        protected virtual void SetBaseDamage( float baseDamage)
        {
            m_fBaseDamage = baseDamage;
        }

        public GameObject GetWeaponPrefab()
        {
            return weaponPrefab;
        }
        

        public AnimationClip GetAttackAnimClip()
        {
            RemoveAnimationEvent();

            return attackAnimation;
        }

        //So that asset packs cannot cause crashes
        private void RemoveAnimationEvent()
        {
            attackAnimation.events = new AnimationEvent[0];
        }
    }
}

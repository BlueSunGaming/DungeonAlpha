using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace RPG.Weapons
{
    [CreateAssetMenu(menuName = ("RPG/Weapon"))]

    public class Weapon : ScriptableObject
    {
        [SerializeField]
        float minTimeBetweenHits = .5f;

        [SerializeField]
        float maxAttackRange = 2f;

        [SerializeField]
        GameObject weaponPrefab;

        [SerializeField]
        AnimationClip attackAnimation;

        public Transform weaponGripTransform;
        
        public float GetMinTimeBetweenHit()
        {
            return minTimeBetweenHits;
        }

        public float GetMaxAttackRange()
        {
            return maxAttackRange;
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

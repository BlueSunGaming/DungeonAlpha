using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace RPG.Weapons
{


    public class Weapon : ScriptableObject
    {
        //TODO: set default tag and layer (maybe a new Weapon Layer)
        public float minTimeBetweenHits = .5f;
        public float maxAttackRange = 2f;
        public GameObject weaponPrefab;
        public uint itemID;
        public AnimationClip attackAnimation;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonRPG.Armor
{
    public class Armor : MonoBehaviour //TODO: investigate difference between ScriptableObject & Mono Behaviour
    {
        //TODO: set default tag and layer (maybe a new Weapon Layer)

        public GameObject armorPrefab;
        public uint itemID;
        public Transform armorSittingTransform;

        [SerializeField] protected float m_fBaseProtection = 1.0f;

        public virtual float GetBaseProtection()
        {
            return m_fBaseProtection;
        }

        protected virtual void SetBaseProtection(float baseProtection)
        {
            m_fBaseProtection = baseProtection;
        }
    }
}


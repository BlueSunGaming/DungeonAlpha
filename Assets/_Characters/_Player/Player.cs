using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using DungeonRPG.CameraUI;
using DungeonRPG.Core;
using DungeonRPG.Weapons;

namespace DungeonRPG.Character
{
    public class Player : IDamageable
    {
        Animator animator;

        [SerializeField]
        int enemyLayer = 11;
        
        [SerializeField]
        float damagePerHit = 12f;

        [SerializeField]
        Weapon weaponInUse;
        
        [SerializeField]
        GameObject weaponSocket;

        [SerializeField]
        AnimatorOverrideController animatorOverrideController;

        [SerializeField]
        PlayerAttributes m_attributes;
        
        float lastHitTime = 0;

        CameraRaycaster cameraRaycaster;

        void Start()
        {
            RegisterForMouseClick();

            GenerateHealth();

            PutWeaponInHand();

            SetupRuntimeAnimator();
        }

        private void GenerateHealth()
        {
            currentHealthPoints = maxHealthPoints;
        }

        private void SetupRuntimeAnimator()
        {
            animator = GetComponent<Animator>();

            animator.runtimeAnimatorController = animatorOverrideController;

            animatorOverrideController["DEFAULT ATTACK"] = weaponInUse.GetAttackAnimClip();
        }

        private void PutWeaponInHand()
        {
            var weaponPrefab = weaponInUse.GetWeaponPrefab();

            var weapon = Instantiate(weaponPrefab, weaponSocket.transform);

            weapon.transform.localPosition = weaponInUse.weaponGripTransform.localPosition;

            weapon.transform.localRotation = weaponInUse.weaponGripTransform.localRotation;
        }

        private void RegisterForMouseClick()
        {
            cameraRaycaster = FindObjectOfType<CameraRaycaster>();

            cameraRaycaster.notifyMouseClickObservers += OnMouseClick;
        }
 
        void OnMouseClick(RaycastHit raycastHit, int layerHit)
        {
            if (layerHit == enemyLayer)
            {
                var enemy = raycastHit.collider.gameObject;

                if (IsTargetInRange(enemy))
                {
                    AttackTarget(enemy);
                }
            }
        }

        private void AttackTarget(GameObject target)
        {
            var enemyComponent = target.GetComponent<Enemy>();

            if (Time.time - lastHitTime > weaponInUse.GetMinTimeBetweenHit())
            {
                animator.SetTrigger("Attack"); // TODO make const
                
                enemyComponent.TakeDamage(damagePerHit);

                lastHitTime = Time.time;   
            }
            else
            {
                animator.SetTrigger("Grounded");
            }
        }

        private bool IsTargetInRange(GameObject target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;

            return distanceToTarget <= weaponInUse.GetMaxAttackRange();
        }

        public float healthAsPercentage
        {
            get
            {
                return currentHealthPoints / (float)maxHealthPoints;
            }
        }

        private List<DungeonRPG.Armor.Armor> GetAllArmorPieces()
        {
            List<DungeonRPG.Armor.Armor> retArmors = new List<DungeonRPG.Armor.Armor>();
            return retArmors;
        }

        public override void TakeDamage(float damage)
        {
            float defenseDamageReduction = naturalBaseDefense + GetDefenseRating(GetAllArmorPieces());
            // TODO: make less convoluted using an if statement or two.
            damage = (damage - defenseDamageReduction) > 0.0f ? damage - defenseDamageReduction : 0.0f;
            base.TakeDamage(damage);
        }

        public void OnCollisionEnter(Collision collision)
        {
            print("collision object value is :" + collision.ToString());
            print(collision.transform.tag);
        }
    }
}

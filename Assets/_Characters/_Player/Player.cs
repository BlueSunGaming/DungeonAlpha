using UnityEngine;
using System.Collections;
using System;
using RPG.CameraUI;
using RPG.Core;
using RPG.Weapons;

namespace RPG.Character
{
    public class Player : MonoBehaviour, IDamageable
    {
        Animator animator;

        [SerializeField]
        int enemyLayer = 11;

        [SerializeField]
        float maxHealthPoints = 100f;

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


        float currentHealthPoints;

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
                
                //enemyComponent.TakeDamage(damagePerHit);

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

        public void TakeDamage(float damage)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 1f, maxHealthPoints);
        }

        public void OnCollisionEnter(Collision collision)
        {
            print("collision object value is :" + collision.ToString());
            print(collision.transform.tag);
        }
    }
}

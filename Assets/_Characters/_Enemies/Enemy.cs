﻿using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.ThirdPerson;
using DungeonRPG.Core;
using DungeonRPG.Weapons;
using System;

namespace DungeonRPG.Character
{
    public class Enemy : IDamageable
    {
        [SerializeField] float attackRadius = 5f;
        [SerializeField] float chaseRadius = 7f;
        [SerializeField] float damagePerShot = 7f;
        [SerializeField] float secondsBetweenShots = .5f;
        
        [SerializeField] GameObject projectileToUse;
        [SerializeField] GameObject projectileSocket;
        [SerializeField] Vector3 aimOffset = new Vector3(0, 1f, 0);

        Vector3 hitChanceAffect = new Vector3(0, 0f, 0);
        GameObject player = null;

        bool isAttacking = false;

        AICharacterControl aiCharacterControl = null;
        
        public float healthAsPercentage
        {
            get
            {
                return currentHealthPoints / (float)maxHealthPoints;
            }
        }

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player");

            aiCharacterControl = GetComponent<AICharacterControl>();

            currentHealthPoints = maxHealthPoints;//TODO Change this
        }

        private void Update()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

            if (distanceToPlayer <= attackRadius && !isAttacking)
            {
                isAttacking = true;

                InvokeRepeating("SpawnProjectile", 0, secondsBetweenShots);
            }

            if (distanceToPlayer > attackRadius)
            {          
                isAttacking = false;

                CancelInvoke();
            }

            if (aiCharacterControl != null)
            {
                if (distanceToPlayer <= chaseRadius)
                {
                    aiCharacterControl.SetTarget(player.transform);
                }
                else
                {
                    aiCharacterControl.SetTarget(transform);
                }
            }
            else
            {
                Debug.Log("aiCharacterControl serializable field is not set on " + gameObject.name);
            }

        }
        void SpawnProjectile() 
        {
            GameObject newProjectile = Instantiate(projectileToUse, projectileSocket.transform.position, Quaternion.identity);

            Component weaponComponent = projectileToUse.GetComponent(typeof(Weapon));

            float dmg = 0.0f;
            if (weaponComponent != null)
            {
                 dmg = GetDamageModifier((weaponComponent as Weapon));
            }
            else
            {
                Debug.Log("Intended projectile to use does not inherit from Weapon. It's value was: " + projectileToUse);
            }

            Projectile projectileComponent = newProjectile.GetComponent<Projectile>();
            // TODO: protect with a #Debug
            Debug.Log("Damage Calculated with modifier is " + dmg);
            // Check the chance to hit affects the damage done with the projectile
            dmg = GetChanceToHit() < 50.0f ? 0 : dmg;       // TODO: potential for partial damage
            Debug.Log("Damage Calculated with chance to hit modifier is " + dmg);
            projectileComponent.setShooter(gameObject);     // TODO: is this parentage relationship necessary, validate yes or no.

            Vector3 unitVectorToPlayer = (player.transform.position - projectileSocket.transform.position + aimOffset + hitChanceAffect).normalized;

            float projectileSpeed = projectileComponent.projectileSpeed;

            newProjectile.GetComponent<Rigidbody>().velocity = unitVectorToPlayer * projectileSpeed;
        }

        public override void TakeDamage(float damage)
        {
            damage = (damage - naturalBaseDefense) > 0.0f ? damage - naturalBaseDefense : 0.0f;
            base.TakeDamage(damage);
            // Set resulting damageText location
            //Vector3 resultPosition = gameObject.transform.position;
            GameObject textPopUpSocket = gameObject.transform.Find("TextPopUpSocket1").gameObject;
            //resultPosition.x += 9.5f;
            //resultPosition.y += 9.5f;
            //Debug.Log("Enemy position original =" + gameObject.transform.position.y);
            //Debug.Log("Enemy DmgText position =" + resultPosition.y);
            GameObject damageText = GameObject.Instantiate(Resources.Load("UI/Canvas/damageTextParent")) as GameObject;
            damageText.transform.SetParent(GameObject.Find("Game Canvas").transform, false);
            damageText.transform.position = new Vector3(textPopUpSocket.transform.position.x,
                textPopUpSocket.transform.position.y + 25f, textPopUpSocket.transform.position.z);
            Debug.Log("TextPopupSocket position is " + textPopUpSocket.transform.position);
            Debug.Log("damageText position is " + damageText.transform.position);
            if (damageText != null && damageText.transform.childCount > 0)
            {
                Debug.Log("damageText was instantiated and damageText set with dmg =" + damage);
                GameObject childTransform = damageText.transform.Find("damageText").gameObject;
                if (childTransform != null)
                {
                    UnityEngine.UI.Text txt = childTransform.GetComponent<UnityEngine.UI.Text>();
                    //childTransform.transform.position = new Vector3(textPopUpSocket.transform.position.x + 25f,
                    //    textPopUpSocket.transform.position.y, textPopUpSocket.transform.position.z);
                    txt.text = damage.ToString();

                    Destroy(damageText, 1.0f); // UIConstants.TIME_TO_DISPLAY_DAMAGE_TEXT);
                }
            }

            if (currentHealthPoints <= 1.0f)
            {
                Destroy(gameObject);
            }
        }

        public void OnCollisionEnter(Collision collision)
        {
            // Prevent enemies/environment from hurting each other
            if (collision.collider.tag != "Untagged")
            {
                if (collision.collider.tag == "Player")
                {
                    print("Collider value is " + collision.collider.ToString());
                    print("Collider tag is " + collision.collider.tag.ToString());
                    float damage = 12.0f;
                    currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 1.0f, maxHealthPoints);

                    if (currentHealthPoints <= 1.0f)
                    {
                        Destroy(gameObject);
                    }
                }
                else
                {
                    print("gameObject value is " + collision.gameObject.ToString());
                    print("gameObject tag is " + collision.gameObject.tag.ToString());
                    print(collision.ToString());
                }
            }
            else
            {
                Debug.Log("We attempted to process a collision but the tag was Untagged with value " + collision.collider.tag);
            }
            //collision.collider.CompareTag()
        }

        void OnDrawGizmos()
        {
            //Attack sphere
            Gizmos.color = new Color(255f, 0f, 0, .5f);

            Gizmos.DrawWireSphere(transform.position, attackRadius);

            //Chase sphere
            Gizmos.color = new Color(0f, 0f, 255f, .5f);

            Gizmos.DrawWireSphere(transform.position, chaseRadius);
        }

        //void IDamageable.OnCollisionEnter(Collision collision)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
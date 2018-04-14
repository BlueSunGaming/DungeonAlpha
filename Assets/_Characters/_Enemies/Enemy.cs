using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.ThirdPerson;
using RPG.Core;
using RPG.Weapons;
using System;

namespace RPG.Character
{
    public class Enemy : MonoBehaviour, IDamageable
    {

        [SerializeField] float maxHealthPoints;

        [SerializeField] float attackRadius = 5f;

        [SerializeField] float chaseRadius = 7f;

        [SerializeField] float damagePerShot = 7f;

        [SerializeField] float secondsBetweenShots = .5f;


        [SerializeField] GameObject projectileToUse;

        [SerializeField] GameObject projectileSocket;
        

        [SerializeField] Vector3 aimOffset = new Vector3(0, 1f, 0);

        GameObject player = null;

        bool isAttacking = false;

        float currentHealthPoints;

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

            if (distanceToPlayer <= chaseRadius)
            {
                aiCharacterControl.SetTarget(player.transform);
            }

            else
            {
                aiCharacterControl.SetTarget(transform);
            }

        }
        void SpawnProjectile() 
        {
            GameObject newProjectile = Instantiate(projectileToUse, projectileSocket.transform.position, Quaternion.identity);

            Projectile projectileComponent = newProjectile.GetComponent<Projectile>();

            projectileComponent.SetDamage(damagePerShot);

            projectileComponent.setShooter(gameObject);

            Vector3 unitVectorToPlayer = (player.transform.position - projectileSocket.transform.position + aimOffset).normalized;

            float projectileSpeed = projectileComponent.projectileSpeed;

            newProjectile.GetComponent<Rigidbody>().velocity = unitVectorToPlayer * projectileSpeed;
        }

        public void TakeDamage(float damage)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 1f, maxHealthPoints);

            if (currentHealthPoints <= 1)
            {
                Destroy(gameObject);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.tag != "Untagged")
            {
                if (collision.collider.tag == "Player")
                {
                    print("Collider value is " + collision.collider.ToString());
                    print("Collider tag is " + collision.collider.tag.ToString());
                    float damage = 12.0f;
                    currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 1f, maxHealthPoints);

                    if (currentHealthPoints <= 1)
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
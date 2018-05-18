using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;


namespace RPG.Weapons
{
    public class Projectile : MonoBehaviour
    {

        public float projectileSpeed;

        GameObject shooter;

        float damageCaused;

        public void setShooter(GameObject shooter)
        {
            this.shooter = shooter;
        }

        public void SetDamage(float damage)
        {
            damageCaused = damage;
        }

        public float GetDefaultLaunchSpeed()
        {
            return projectileSpeed;
        }

        private void OnCollisionEnter(Collision collision)
        {
            var layerCollidedWith = collision.gameObject.layer;

            if (shooter && layerCollidedWith != shooter.layer)
            {
                DamageIfDamageable(collision);
            }
        }

        // Process Damage if damage-able using all collisions between this projectile and another game object
        private void DamageIfDamageable(Collision collision)
        {

            Component damageableComponent = collision.gameObject.GetComponent(typeof(IDamageable));

            if (damageableComponent != null)
            {

                (damageableComponent as IDamageable).TakeDamage(damageCaused);

            }
            Destroy(gameObject);


        }
        
        



    }
}
using RPG.Character;
using UnityEngine;

namespace RPG.Core
{
    struct Damage
    {
        private float baseDamage;
        private float percentPierce;
    }
    public class IDamageable : PlayerAttributes
    {
        protected float currentHealthPoints;

        [SerializeField]
        protected float maxHealthPoints = 100.0f;
        [SerializeField]
        protected float naturalBaseDefense = 3f;

        //protected virtual void OnCollisionEnter(Collision collision)
        //{

        //}

        public virtual void TakeDamage(float damage)
        {
            Debug.Log("Damage to be taken =" + damage);
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 1.0f, maxHealthPoints);
        }
    }
}
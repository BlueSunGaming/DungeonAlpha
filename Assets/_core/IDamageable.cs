using UnityEngine;

namespace RPG.Core
{
    public interface IDamageable
    {
        //void OnCollisionEnter(Collision collision);

        void TakeDamage(float damage);

        //bool StillAlive();
    }
}
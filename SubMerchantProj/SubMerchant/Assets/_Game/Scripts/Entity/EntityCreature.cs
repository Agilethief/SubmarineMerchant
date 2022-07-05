using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CargoGame
{
    public class EntityCreature : Entity, IDamagable
    {
        [SerializeField]
        private int healthCurrent, healthMax;


        public void Die()
        {
            Destroy(this.gameObject);
        }

        public void HealDamage(int healAmount)
        {
            healthCurrent += healAmount;
            if(healthCurrent > healthMax) healthCurrent = healthMax;
        }

        public void TakeDamage(int damageAmount, IDamagable.DamageType damageType)
        {
            healthCurrent -= damageAmount;
            if(healthCurrent < 0)
                Die();
        }
    }
}

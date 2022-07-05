using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    public enum DamageType { Normal, Special};

    public void TakeDamage(int damageAmount, DamageType damageType);
    public void HealDamage(int healAmount);
    
    public void Die();
}

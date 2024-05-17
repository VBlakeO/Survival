using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void ApplyDamage(float damage);
    public void ApplyDamage(float damage, DamageType damageType);

    public void ApplyHealing(float damage);

    public void Die();

    public bool IsDead();
}

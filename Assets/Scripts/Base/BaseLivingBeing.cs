using UnityEngine;

public class BaseLivingBeing : MonoBehaviour, IDamageable
{
    [Header("BaseLivingBeing")]
    [SerializeField] protected float maxLife = 100f;
    [SerializeField] protected float currentLife = 0f;
    
    public virtual void Awake() 
    {
        currentLife = maxLife;   
    }

    public virtual void ApplyDamage(float damage)
    {
        if(IsDead())
            return;

        currentLife -= damage;

        if(IsDead())
            Die();

    }

    public virtual void ApplyDamage(float damage, DamageType damageType)
    {
        ApplyDamage(damage);
    }

    public virtual void ApplyHealing(float healing)
    {
        currentLife += healing;

        if (currentLife > maxLife)
            currentLife = maxLife;
        
    }

    public virtual void Die()
    {

    }

    public bool IsDead()
    {
        return currentLife <= 0f;
    }
}

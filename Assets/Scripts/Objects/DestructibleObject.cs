using UnityEngine;

public class DestructibleObject : BaseLivingBeing
{
    [Space]
    [SerializeField] private DamageType[] vulnerability ;
    [SerializeField] private DamageType[] invulnerability;
    [Space]
    
    [SerializeField] private GameObject itemDropped = null;
    [SerializeField] private Vector2 itemDroppedAmount = new Vector2(0, 1);

    public override void ApplyDamage(float damage, DamageType damageType)
    {
        float damageMultiplier = 1f;
        
        for (int i = 0; i < vulnerability.Length; i++)
        {
            if (damageType == vulnerability[i])
            {
                damageMultiplier = 1f;
                break;
            }
            else
                damageMultiplier = 0.3f;
        }
            

        for (int i = 0; i < invulnerability.Length; i++)
        {
            if (damageType == invulnerability[i])
            {
                damageMultiplier = 0f; 
                break;
            }

            continue;
        }

        base.ApplyDamage(damage * damageMultiplier, damageType);
    }

    public override void Die()
    {
        base.Die();

        if (itemDropped == null)
            return;

        for (int i = 0; i < Random.Range(itemDroppedAmount.x, itemDroppedAmount.y); i++)
            Instantiate(itemDropped, transform.position, Quaternion.identity);

        DieEffect();
    }

    public virtual void DieEffect()
    {
        Destroy(gameObject);
    }
}

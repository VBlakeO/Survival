using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Events;

public enum DamageType{Axe, Pickaxe, Hammer, Knife, Spear, Bow}
public class EquipmentBase : MonoBehaviour
{
    public DamageType equipmentType = 0;
    public float damage = 0f; 
    [SerializeField] protected float toolDurability = 0f; 
    [SerializeField] protected float durabilityDamage = 0f; 
    public float interactionRange = 3f; 
    [Space]

    public float staminaUsed = 6f;
    [SerializeField] protected float useRate = 2f;
    [Space]

    [SerializeField] protected bool continuousUse = false;
    [Space]

    [SerializeField] protected AnimatorController animController = null;
    [SerializeField] protected Animator anim;
    [SerializeField] protected int attackAnimVariations = 0;

    protected int currentAnimIndex = 0;
    protected float currentRate = 0f;
    protected bool isUsing = false;

    public bool equipped = false;

    protected PlayerEquipmentController playerEquipmentController = null;

    void Start()
    {
        playerEquipmentController = PlayerEquipmentController.Instance;
    }

    protected virtual void OnEnable()
    {
        currentRate = useRate;
        anim.runtimeAnimatorController = animController;
    }

    protected virtual void OnDisable()
    {
        isUsing = false;
        equipped = false;
    }

    protected virtual bool CanUse()
    {
        return currentRate >= useRate && toolDurability > 0 && equipped;
    }

    public virtual bool TryAttack()
    {
        if (!CanUse())
            return false;

        if (!isUsing || continuousUse)
        {
            AttackEffect();
            return true;
        }

        return false;
    }

    public void ResetEquipped()
    {
        anim.runtimeAnimatorController = animController;
    }

    public void Equipped()
    {
        equipped = true;
        anim.runtimeAnimatorController = animController;
    }

    public virtual void Attack()
    {
        if (playerEquipmentController.Attack())
        toolDurability -= durabilityDamage;
    }

    protected virtual void AttackEffect()
    {
        isUsing = true;

        anim.SetTrigger("Attack");
        currentAnimIndex++;

        if (currentAnimIndex > attackAnimVariations)
            currentAnimIndex = 0;

        anim.SetInteger("AtkIndex", currentAnimIndex);

        currentRate = 0f;
    }

    public virtual void StopAttack()
    {
        anim.ResetTrigger("Attack");
        isUsing = false;
    }

    private void FixedUpdate()
    {
        if (currentRate < useRate)
            currentRate += Time.deltaTime;
    }
}

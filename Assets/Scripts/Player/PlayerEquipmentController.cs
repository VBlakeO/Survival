using UnityEngine;

public class PlayerEquipmentController : Singleton<PlayerEquipmentController>
{
    [SerializeField] private LayerMask interactableLayer;
    [Space]

    [SerializeField] private HotbarDisplay hotbar = null;
    [SerializeField] private InputControl inputControl = null;
    [SerializeField] private StaminaControl staminaControl = null;

    private EquipmentBase equipmentBase;
    private IDamageable damageableObj;

    protected override void Awake()
    {
        base.Awake();
    }

    private bool CanUse()
    {   
        if (hotbar.currentEquipament) equipmentBase = hotbar.currentEquipament;
        return equipmentBase != null && staminaControl.GetCurrentStamina() >= equipmentBase.staminaUsed;
    }

    private void OnEnable() 
    {
        inputControl.WhilePressMouse0 += UseItem;
        inputControl.OnReleasingMouse0 += StopUseItem;
    }

    private void OnDisable()
    {
        inputControl.WhilePressMouse0 -= UseItem;
        inputControl.OnReleasingMouse0 -= StopUseItem;
    }

    private void UseItem()
    {
        if (CanUse())
        {   
            if (equipmentBase.TryAttack())
                staminaControl.UseStamina(equipmentBase.staminaUsed);
        }
    }

    private void StopUseItem()
    {
        if (equipmentBase != null)
            equipmentBase.StopAttack();
    }

    public bool Attack()
    {
        print("Attack");

        if (GetRaycastHit(out RaycastHit _hit))
        {
            damageableObj = _hit.transform.GetComponent<IDamageable>();

            if (damageableObj == null)
                return false;

            damageableObj.ApplyDamage(equipmentBase.damage, equipmentBase.equipmentType);
        }
        
        return false;
    }

    private bool GetRaycastHit(out RaycastHit _hit) => Physics.Raycast(transform.position, transform.forward, out _hit, equipmentBase.interactionRange, interactableLayer, QueryTriggerInteraction.Ignore);
}

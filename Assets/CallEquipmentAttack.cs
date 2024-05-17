using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallEquipmentAttack : MonoBehaviour
{
    public EquipmentBase equipmentBase = null;

    public void Equipped()
    {
        equipmentBase?.Equipped();
    }

    public void Attack()
    {
        equipmentBase?.Attack();
    }

}

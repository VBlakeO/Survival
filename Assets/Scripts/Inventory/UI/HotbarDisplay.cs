using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class HotbarDisplay : StaticInventoryDisplay
{
    private int maxIndexSize = 9;
    private int currentIndex = 0;

    [SerializeField] private InputControl inputControl = null;
    [SerializeField] private List<StaticItem> items = null;
    [HideInInspector] public StaticItem currentItem = null;
    [HideInInspector] public EquipmentBase currentEquipament = null;
    [Space]
    
    [SerializeField] protected Animator anim = null;
    [SerializeField] protected AnimatorController playerAnimController = null;
    [SerializeField] private CallEquipmentAttack callEquipment = null;
    
    protected override void Start()
    {
        base.Start();
        currentIndex = 0;
        maxIndexSize = slots.Length - 1;
        slots[currentIndex].SetHighlight(true);

        EquipItem(slots[currentIndex]);
    }


    protected override void OnEnable()
    {
        base.OnEnable();

        inputControl.OnNumPressed += SetIndex;
        inputControl.OnScrollRoll += ChangeIndex;
        //inputControl.OnPressMouse0 += UseItem;

        foreach (var slot in slots)
            slot.OnUpdateSlot += EquipItem;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        inputControl.OnNumPressed -= SetIndex;
        inputControl.OnScrollRoll -= ChangeIndex;
        
        foreach (var slot in slots)
            slot.OnUpdateSlot -= EquipItem;
    }

    private void ChangeIndex(int _direction)
    {
        slots[currentIndex].SetHighlight(false);
        currentIndex += _direction * -1;

        if (currentIndex > maxIndexSize) currentIndex = 0;
        if (currentIndex < 0) currentIndex = maxIndexSize;

        slots[currentIndex].SetHighlight(true);
        EquipItem(slots[currentIndex]);
    }

    public void SetIndex(int newIndex)
    {
        slots[currentIndex].SetHighlight(false);

        if (newIndex < 0) currentIndex = 0;
        if (newIndex > maxIndexSize) newIndex = maxIndexSize;

        currentIndex = newIndex;
        slots[currentIndex].SetHighlight(true);

        EquipItem(slots[currentIndex]);
    }

    private void EquipItem(InventorySlot_UI _currentSlot)
    {
        if (slots[currentIndex] != _currentSlot)
            return;

        currentItem = null;
        currentEquipament = null;
        anim.runtimeAnimatorController = playerAnimController;

        items.ForEach(item =>
        {
            if (item.ItemData == slots[currentIndex].AssignedInventorySlot.ItemData &&
                slots[currentIndex].AssignedInventorySlot.ItemData.itemCategory != ItemCategory.Resources)
            {
                item.gameObject.SetActive(true);
                currentItem = item;

                if (item.GetComponent<EquipmentBase>())
                {
                    currentEquipament = item.GetComponent<EquipmentBase>();
                    currentEquipament.ResetEquipped();
                    callEquipment.equipmentBase = currentEquipament;
                }
            }
            else
            {
                item.gameObject.SetActive(false);
            }
        });
    }



    private void UseItem()
    {
        if (slots[currentIndex].AssignedInventorySlot.ItemData == null)
            return;

        slots[currentIndex].AssignedInventorySlot.ItemData.UseItem();
    }
}


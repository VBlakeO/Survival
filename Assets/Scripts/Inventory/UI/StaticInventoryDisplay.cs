using System.Collections.Generic;
using UnityEngine;

public class StaticInventoryDisplay : InventoryDisplay
{
    [SerializeField] private InventoryHolder inventoryHolder = null;
    [SerializeField] protected InventorySlot_UI[] slots;

    protected virtual void OnEnable()
    {
        PlayerInventoryHolder.OnPlayerInventoryChange += RefreshStaticDisplay;
    }
    
    protected virtual void OnDisable()
    {
        PlayerInventoryHolder.OnPlayerInventoryChange -= RefreshStaticDisplay;
    }

    private void RefreshStaticDisplay()
    {
        if (inventoryHolder != null)
        {
            inventorySystem = inventoryHolder.MyInventorySystem;
            inventorySystem.OnInventorySlotChanged += UpdateSlot;
        }

        AssignSlot(inventorySystem, 0);
    }

    protected virtual void Start()
    {
        RefreshStaticDisplay();
    }

    public override void AssignSlot(InventorySystem invToDisplay, int offeset)
    {
        slotDictionary = new Dictionary<InventorySlot_UI, InventorySlot>();

        for (int i = 0; i < inventoryHolder.Offset; i++)
        {
            slotDictionary.Add(slots[i], inventorySystem.InventorySlots[i]);
            slots[i].Init(inventorySystem.InventorySlots[i]);
        }
    }
}

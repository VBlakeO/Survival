using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System.Linq;

public class DynamicInventoryDisplay : InventoryDisplay
{
    [SerializeField] private InventorySlot_UI slotPrefab = null;
    [SerializeField] private  ControlSortUi sortTabControl = null;
    [Space]
    
    [SerializeField] private Transform slotsParent = null;
    [Space]

    public List<InventorySlot_UI> slotsUi = new();

    public UnityAction OnCloseInventoryDisplay;
    
    public void RefreshDynamicInventory(InventorySystem invToDisplay, int offset)
    {
        ClearSlots();
        inventorySystem = invToDisplay;
        
        if (inventorySystem != null)
            inventorySystem.OnInventorySlotChanged += UpdateSlot;

        AssignSlot(invToDisplay, offset);
    }

    public override void AssignSlot(InventorySystem invToDisplay, int offset)
    {
        slotDictionary = new Dictionary<InventorySlot_UI, InventorySlot>();
        
        if (invToDisplay == null)
            return;

        for (int i = offset; i < invToDisplay.InventorySize; i++)
        {
            var uiSlot = Instantiate(slotPrefab, slotsParent);

            slotDictionary.Add(uiSlot, invToDisplay.InventorySlots[i]);
            slotsUi.Add(uiSlot);
            uiSlot.dynamicInventoryDisplay = this;
            uiSlot.Init(invToDisplay.InventorySlots[i]);
            uiSlot.UpdateUISlot();
        }
    }

    public void SetInventoryHolderState(bool state)
    {
        gameObject.SetActive(state);
    }

    private void ClearSlots()
    {
        slotsUi = new();
        foreach (var item in slotsParent.Cast<Transform>())
        {
            //TODO: Change To Pool
            Destroy(item.gameObject);
        }

        if (slotDictionary != null)
            slotDictionary.Clear();
    }

    public void SortItems(int type)
    {
        foreach (var item in slotsUi)
            item.gameObject.SetActive(true);

        if (type == 0)
            return;

        foreach (var item in slotsUi)
        {
            if (item.AssignedInventorySlot.ItemData == null)
                item.gameObject.SetActive(false);

            if (item.AssignedInventorySlot.ItemData != null)
            {
                if (item.AssignedInventorySlot.ItemData.itemType != type)
                    item.gameObject.SetActive(false);
            }
        }
        print("type " + type);
    }

    public void DisableSlotsHighlight()
    {
        foreach (var slot in slotsUi)
            slot.SetHighlight(false);
    }

    public void TryOrganizeItems(bool isPlayerBackpack)
    {
        OrganizeItems( isPlayerBackpack);
        UpdateAllSlots();
    }

    public void TryTransferSameItems()
    {
        if (InventoryUIController.Instance.storagePanel.gameObject.activeInHierarchy)
        {
            List<int> sameIDs = new();

            if (inventorySystem == InventoryController.Instance.playerInventoryHolder.MyInventorySystem) //Backpack
            {
                foreach (var slot in InventoryController.Instance.externalInventoryHolder.MyInventorySystem.InventorySlots)
                    if (slot.ItemData != null)
                        sameIDs.Add(slot.ItemData.ID);

                foreach (var slot in inventorySystem.InventorySlots.Skip(9))
                {
                    if (slot.ItemData != null)
                    {
                        if (sameIDs.Contains(slot.ItemData.ID))
                        {
                            if (InventoryUIController.Instance.storagePanel.inventorySystem.AddToInventory(slot.ItemData, slot.StackSize, out int amountAccepted))
                                slot.RemoveToStack(amountAccepted);
                        }
                    }
                }
            }


            if (inventorySystem == InventoryController.Instance.externalInventoryHolder.MyInventorySystem) //Storage
            {
                foreach (var slot in InventoryController.Instance.playerInventoryHolder.MyInventorySystem.InventorySlots)
                    if (slot.ItemData != null)
                        sameIDs.Add(slot.ItemData.ID);

                foreach (var slot in inventorySystem.InventorySlots)
                {
                    if (slot.ItemData != null)
                    {
                        if (sameIDs.Contains(slot.ItemData.ID))
                        {
                            if (InventoryUIController.Instance.backpackPanel.inventorySystem.AddToInventory(slot.ItemData, slot.StackSize, out int amountAccepted, true))
                                slot.RemoveToStack(amountAccepted);
                        }
                    }
                }
            }
        }

        UpdateAllSlots();
    }

    public void TryTransferAllItems(bool isPlayerBackpack)
    {   
        if (InventoryUIController.Instance.storagePanel.gameObject.activeInHierarchy)
        {
            if (inventorySystem == InventoryController.Instance.playerInventoryHolder.MyInventorySystem) //Backpack
            {
                foreach (var slot in inventorySystem.InventorySlots.Skip(9))
                {
                    if (slot.ItemData != null)
                    {
                        if (InventoryUIController.Instance.storagePanel.inventorySystem.AddToInventory(slot.ItemData, slot.StackSize, out int amountAccepted))
                            slot.RemoveToStack(amountAccepted);
                    }
                }
            }

            if (inventorySystem == InventoryController.Instance.externalInventoryHolder.MyInventorySystem) //Storage
            {
                foreach (var slot in inventorySystem.InventorySlots)
                {
                    if (slot.ItemData != null)
                    {
                        if (InventoryUIController.Instance.backpackPanel.inventorySystem.AddToInventory(slot.ItemData, slot.StackSize, out int amountAccepted, true))
                            slot.RemoveToStack(amountAccepted);
                    }
                }
            }
        }

        UpdateAllSlots();
    }

    private void UpdateAllSlots()
    {
        foreach (var slot in slotsUi)
            slot.UpdateUISlot();
    }

    private void OnDisable()
    {
        if (inventorySystem != null)
        {
            inventorySystem.OnInventorySlotChanged -= UpdateSlot;
            slotsUi = new();
        }

        if (sortTabControl != null)
        {    
            sortTabControl.ActiveHighlight(0);
           
            if (inventorySystem == InventoryController.Instance.playerInventoryHolder.MyInventorySystem)
                sortTabControl.DisableOrderslot();
        }
    }
}

using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class InventorySystem
{
    [SerializeField] private List<InventorySlot> inventorySlots;
    public List<InventorySlot> InventorySlots => inventorySlots;

    public int InventorySize => InventorySlots.Count;

    public UnityAction<InventorySlot> OnInventorySlotChanged;

    public InventorySystem(int size) // Constructor that sets the amount of slots;
    {
        inventorySlots = new List<InventorySlot>(size);

        for (int i = 0; i < size; i++)
            inventorySlots.Add(new InventorySlot(i));
    }

    public bool AddToInventory(InventoryItemData itemToAdd, int _amountToAdd, out int amountAccepted, bool checkAbove8 = false)
    {
        // Create a local variable to track the amount to add, initialized with the value of the _amountToAdd parameter
        int localAmountToAdd = _amountToAdd;

        // Check if the item is already present in the inventory
        if (ContainsItem(itemToAdd, out List<InventorySlot> sameItemSlots))
        {
            // Iterate over the slots that contain the same item
            foreach (InventorySlot slot in sameItemSlots)
            {
                // If necessary, check if the slots are above 8 and, if so, continue to the next slot
                if (checkAbove8 && slot.SlotID <= 8)
                    continue;

                // Get the remaining space in the slot for the item
                if (slot.GetStackRemainingSpace(itemToAdd, out int amountRemaining))
                {
                    Debug.Log(amountRemaining);

                    // If the remaining space is sufficient for the amount to be added
                    if (amountRemaining >= localAmountToAdd)
                    {
                        // Add the total amount to the slot
                        slot.AddToStack(localAmountToAdd);
                        OnInventorySlotChanged?.Invoke(slot);
                        localAmountToAdd = 0; // Set the amount to be added to zero, as no additional items need to be added
                        amountAccepted = _amountToAdd; // Update the amount accepted to the original value (_amountToAdd)
                        return true; // Return true indicating the addition was successful
                    }
                    else
                    {
                        // Add the remaining amount to the slot
                        slot.AddToStack(amountRemaining);
                        OnInventorySlotChanged?.Invoke(slot);
                        localAmountToAdd -= amountRemaining; // Reduce the amount to be added by the value added to the slot

                        // If there is no more amount to be added
                        if (localAmountToAdd == 0)
                        {
                            amountAccepted = _amountToAdd; // Update the amount accepted to the original value (_amountToAdd)
                            return true; // Return true indicating the addition was successful
                        }
                    }
                }
            }
        }

        // If the item is not present in the inventory or there are not enough slots, look for the first empty slot
        if (HasFreeSlot(checkAbove8, out InventorySlot freeSlot))
        {
            // Get the remaining space in the free slot for the item
            if (freeSlot.GetStackRemainingSpace(itemToAdd, out int amountRemaning))
            {
                // Add the total amount to the free slot
                freeSlot.UpdateInventorySlot(itemToAdd, localAmountToAdd);
                OnInventorySlotChanged?.Invoke(freeSlot);
                localAmountToAdd = 0; // Set the amount to be added to zero, as no additional items need to be added
            }
        }

        // Calculate the amount accepted by subtracting the original amount (_amountToAdd) by the amount not added (localAmountToAdd)
        amountAccepted = _amountToAdd - localAmountToAdd;

        // Return true if at least one item was accepted, or false otherwise
        return amountAccepted != 0;
    }

    public bool ContainsItem(InventoryItemData itemToAdd, out List<InventorySlot> invSlot)
    {
        invSlot = InventorySlots.Where(i => i.ItemData == itemToAdd).ToList();
        return invSlot.Count > 0;
    }

    public bool HasFreeSlot(bool isPlayerBackpack, out InventorySlot freeSlot)
    {
        if (isPlayerBackpack)
            freeSlot = InventorySlots.Skip(9).FirstOrDefault(i => i.ItemData == null);
        else
            freeSlot = InventorySlots.FirstOrDefault(i => i.ItemData == null);

        return freeSlot != null;
    }
}
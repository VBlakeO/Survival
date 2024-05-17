using UnityEngine;
using System;

[Serializable]
public class InventorySlot : ItemSlot
{
    public InventorySlot(InventoryItemData source, int amount) // Constructor to make a Occupied inventory slot;
    {
        itemData = source;
        _itemID = itemData.ID;
        stackSize = amount;
    }

    public InventorySlot(int id)  // Constructor to make a Empty invenotory slot;
    {
        slotID = id;
        ClearSlot();
    }

    public void UpdateInventorySlot(InventoryItemData data, int amount) // Update slot directly;
    {
        itemData = data;
       
        if (data == null)
            _itemID = -1;
        else
            _itemID = itemData.ID;
            
        stackSize = amount;
    }

    public bool SplitStack(out InventorySlot splitStack) 
    {
        if (stackSize <= 1) // Is there enough to actually split? If no returnFalse;
        {
            splitStack = null;
            return false;
        }
     
        int halfStack = Mathf.RoundToInt(stackSize / 2); // Get half the stack;
        RemoveToStack(halfStack);

        splitStack = new InventorySlot(ItemData, halfStack); // Creates a copy of this slot with half the stack size;
        return true;
    }

    public bool EnoughRoomLeftInStack(int amountToAdd, out int amountRemaning)  // Would there be enough room in the stack for the amount we're trying to add;
    {
        amountRemaning = ItemData.MaxStackSize - stackSize;
        return EnoughRoomLeftInStack(amountToAdd);
    }

    public bool EnoughRoomLeftInStack(int amountToAdd)
    {
        if (ItemData == null || ItemData != null && stackSize + amountToAdd <= itemData.MaxStackSize) return true;
        else return false;
    }
    
    public bool GetStackRemainingSpace(InventoryItemData _itemData ,out int amountRemaning)
    {
        if(ItemData == null)
        { 
            amountRemaning = _itemData.MaxStackSize;
            return true;
        }
        else
        {
            amountRemaning = ItemData.MaxStackSize - stackSize;
            return amountRemaning > 0;
        }
    }
}

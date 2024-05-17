using System;
using UnityEngine;

public class ItemSlot : ISerializationCallbackReceiver
{
    //[NonSerialized] protected InventoryItemData itemData; // Itens
    [SerializeField] protected InventoryItemData itemData; // Itens
    [SerializeField] protected int _itemID = -1; // Item atual
    [SerializeField] protected int stackSize; // Quantidade de itens
    [SerializeField] protected int slotID; // Quantidade de itens

    public InventoryItemData ItemData => itemData;
    public int StackSize => stackSize;
    public int SlotID => slotID;
    public Action OnClear;

    public void ClearSlot() // Clear the slot;
    {
        itemData = null;
        _itemID = -1;
        stackSize = -1;
        OnClear?.Invoke();
    }


    public void AssignItem(InventorySlot inventorySlot) // Assings as item to the slot  // Atribuir item
    {
        if (ItemData == inventorySlot.ItemData) // Does the slot contain the same item?
        {
            AddToStack(inventorySlot.stackSize); // Add to stack if so;
        }
        else // Overwrite slot with the inventory slot that we're passing in. 
        {
            itemData = inventorySlot.ItemData;
            _itemID = itemData.ID;
            stackSize = 0;
            AddToStack(inventorySlot.stackSize);
        }
    }

    public void AssignItem(InventoryItemData data, int amount)
    {
        if (itemData == data)  
            AddToStack(amount);
        else
        {
            itemData = data;
            _itemID = data.ID;
            stackSize = 0;
            AddToStack(amount);
        }
    }

    public void AddToStack(int amount)
    {
        stackSize += amount;
    }

    public int RemoveToStack(int amount)
    {
        stackSize -= amount;
        
        if(stackSize <= 0)
            ClearSlot();
        
        return stackSize;
    }



    public void OnBeforeSerialize()
    {
        
    }

    public void OnAfterDeserialize()
    {
        if (_itemID == -1) return;

        if(ResourceManager.Instance != null)
            itemData = ResourceManager.Instance.LoadItemData(_itemID);
    }
}

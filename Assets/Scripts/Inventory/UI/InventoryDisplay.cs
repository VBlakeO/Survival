using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryDisplay : MonoBehaviour
{
    [SerializeField] MouseItemData mouseInventoryItem = null;
    protected InventorySystem inventorySystem = null;
    protected InventorySystem tempInvntorySystem = null;
    protected Dictionary<InventorySlot_UI, InventorySlot> slotDictionary; // Pair up the UI slots with the system slots;
 
    public InventorySystem InventorySystem => inventorySystem;
    public InventorySystem TempInventorySystem => tempInvntorySystem;
    public Dictionary<InventorySlot_UI, InventorySlot> SlotDictionary => slotDictionary;

    public abstract void AssignSlot(InventorySystem invToDisplay, int offset);  // Implement in child classes;

    protected virtual void UpdateSlot(InventorySlot updateSlot)
    {
        foreach (var slot in slotDictionary)  // Slot value - the "under the hood" inventory slot;
        { 
            if (slot.Value == updateSlot)   // Slot key - the UI representation of the value;
                slot.Key.UpdateUISlot(updateSlot);
        }
    }

    public void SlotCliked(InventorySlot_UI clickedUISlot)
    {
        if(clickedUISlot.AssignedInventorySlot.ItemData != null && mouseInventoryItem.AssignedInventorySlot.ItemData == null)  // Quando o slot NÃO está vazio E o mause slot está vazio 
        {
            // Pick up the item in the clicked slot;
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                mouseInventoryItem.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);
                clickedUISlot.ClearSlot();
                return;
            }
        }
        
        if (clickedUISlot.AssignedInventorySlot.ItemData == null && mouseInventoryItem.AssignedInventorySlot.ItemData != null) // Quando o slot está vazio E o mause slot NÃO está vazio 
        {
            clickedUISlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
            clickedUISlot.UpdateUISlot();

            mouseInventoryItem.ClearSlot();
            return;
        }

        if (clickedUISlot.AssignedInventorySlot.ItemData != null && mouseInventoryItem.AssignedInventorySlot.ItemData != null) // Quando o slot NÃO está vazio E o mause slot NÃO está vazio 
        {
            bool isSameItem = clickedUISlot.AssignedInventorySlot.ItemData == mouseInventoryItem.AssignedInventorySlot.ItemData;
            if (isSameItem && clickedUISlot.AssignedInventorySlot.EnoughRoomLeftInStack(mouseInventoryItem.AssignedInventorySlot.StackSize))
            {
                clickedUISlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
                clickedUISlot.UpdateUISlot();

                mouseInventoryItem.ClearSlot();
                return;
            }
            else if (isSameItem && !clickedUISlot.AssignedInventorySlot.EnoughRoomLeftInStack(mouseInventoryItem.AssignedInventorySlot.StackSize, out int leftInStack))
            {
                if (leftInStack < 1)          // Stadk is full so swap the items.
                    SwapSlots(clickedUISlot);
                else                          // Slot is not at max, so take what's need from the mouse inventory.
                {
                    int remaningOnMouse = mouseInventoryItem.AssignedInventorySlot.StackSize - leftInStack;
                    clickedUISlot.AssignedInventorySlot.AddToStack(leftInStack);
                    clickedUISlot.UpdateUISlot();

                    var newItem = new InventorySlot(mouseInventoryItem.AssignedInventorySlot.ItemData, remaningOnMouse);
                    mouseInventoryItem.ClearSlot();
                    mouseInventoryItem.UpdateMouseSlot(newItem);
                    return;
                }
            }
            else if (!isSameItem)
            {
                SwapSlots(clickedUISlot);
                return;
            }
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (InventoryUIController.Instance.storagePanel.gameObject.activeInHierarchy)
            {
                if (clickedUISlot.AssignedInventorySlot.ItemData == null)
                    return;

                if (inventorySystem == InventoryController.Instance.playerInventoryHolder.MyInventorySystem) //Backpack
                {
                    if (InventoryUIController.Instance.storagePanel.inventorySystem.AddToInventory(clickedUISlot.AssignedInventorySlot.ItemData, clickedUISlot.AssignedInventorySlot.StackSize, out int amountAccepted))
                    {
                        clickedUISlot.AssignedInventorySlot.RemoveToStack(amountAccepted);
                        clickedUISlot.UpdateUISlot();
                        return;
                    }
                }

                if (inventorySystem == InventoryController.Instance.externalInventoryHolder.MyInventorySystem) //Storage
                {
                    if (InventoryUIController.Instance.backpackPanel.inventorySystem.AddToInventory(clickedUISlot.AssignedInventorySlot.ItemData, clickedUISlot.AssignedInventorySlot.StackSize, out int amountAccepted, true))
                    {
                        clickedUISlot.AssignedInventorySlot.RemoveToStack(amountAccepted);
                        clickedUISlot.UpdateUISlot();
                        return;
                    }
                }
            }
            else
            {
                if (clickedUISlot.AssignedInventorySlot.ItemData == null)
                    return;

                if (inventorySystem == InventoryController.Instance.playerInventoryHolder.MyInventorySystem) //Backpack
                {
                    if (clickedUISlot.AssignedInventorySlot.SlotID <= 8)
                    {
                        if (inventorySystem.AddToInventory(clickedUISlot.AssignedInventorySlot.ItemData, clickedUISlot.AssignedInventorySlot.StackSize, out int amountAccepted, true))
                        {
                            clickedUISlot.AssignedInventorySlot.RemoveToStack(amountAccepted);
                            clickedUISlot.UpdateUISlot();
                            return;
                        }
                    }
                }
            }

            clickedUISlot.UpdateUISlot();
        }
    }

    public void SlotRightCliked(InventorySlot_UI clickedUISlot)
    {
        if (clickedUISlot.AssignedInventorySlot.ItemData != null && mouseInventoryItem.AssignedInventorySlot.ItemData == null)
        {
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                if (clickedUISlot.AssignedInventorySlot.SplitStack(out InventorySlot halfStackSlot)) // Split Stack
                {
                    mouseInventoryItem.UpdateMouseSlot(halfStackSlot);
                    clickedUISlot.UpdateUISlot();
                    return;
                }
                else
                {
                    // Pick up the item in the clicked slot;
                    mouseInventoryItem.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);
                    clickedUISlot.ClearSlot();
                    return;
                }
            }
        }

        if (clickedUISlot.AssignedInventorySlot.ItemData == null && mouseInventoryItem.AssignedInventorySlot.ItemData != null)
        {
            clickedUISlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
            clickedUISlot.UpdateUISlot();

            mouseInventoryItem.ClearSlot();
            return;
        }

        if (clickedUISlot.AssignedInventorySlot.ItemData != null && mouseInventoryItem.AssignedInventorySlot.ItemData != null)
        {
            bool isSameItem = clickedUISlot.AssignedInventorySlot.ItemData == mouseInventoryItem.AssignedInventorySlot.ItemData;
            if (isSameItem && clickedUISlot.AssignedInventorySlot.EnoughRoomLeftInStack(mouseInventoryItem.AssignedInventorySlot.StackSize))
            {
                clickedUISlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
                clickedUISlot.UpdateUISlot();

                mouseInventoryItem.ClearSlot();
                return;
            }
            else if (isSameItem && !clickedUISlot.AssignedInventorySlot.EnoughRoomLeftInStack(mouseInventoryItem.AssignedInventorySlot.StackSize, out int leftInStack))
            {
                if (leftInStack < 1)          // Stadk is full so swap the items.
                    SwapSlots(clickedUISlot);
                else                          // Slot is not at max, so take what's need from the mouse inventory.
                {
                    int remaningOnMouse = mouseInventoryItem.AssignedInventorySlot.StackSize - leftInStack;
                    clickedUISlot.AssignedInventorySlot.AddToStack(leftInStack);
                    clickedUISlot.UpdateUISlot();

                    var newItem = new InventorySlot(mouseInventoryItem.AssignedInventorySlot.ItemData, remaningOnMouse);
                    mouseInventoryItem.ClearSlot();
                    mouseInventoryItem.UpdateMouseSlot(newItem);
                    return;
                }
            }
            else if (!isSameItem)
            {
                SwapSlots(clickedUISlot);
                return;
            }
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (InventoryUIController.Instance.storagePanel.gameObject.activeInHierarchy)
            {
                if (clickedUISlot.AssignedInventorySlot.ItemData == null)
                    return;

                if (inventorySystem == InventoryController.Instance.playerInventoryHolder.MyInventorySystem) //Backpack
                {
                    if (InventoryUIController.Instance.storagePanel.inventorySystem.AddToInventory(clickedUISlot.AssignedInventorySlot.ItemData, SplitStack(clickedUISlot.AssignedInventorySlot.StackSize), out int amountAccepted))
                    {
                        clickedUISlot.AssignedInventorySlot.RemoveToStack(amountAccepted);
                        clickedUISlot.UpdateUISlot();
                        return;
                    }
                }

                if (inventorySystem == InventoryController.Instance.externalInventoryHolder.MyInventorySystem) //Storage
                {
                    if (InventoryUIController.Instance.backpackPanel.inventorySystem.AddToInventory(clickedUISlot.AssignedInventorySlot.ItemData, SplitStack(clickedUISlot.AssignedInventorySlot.StackSize), out int amountAccepted, true))
                    {
                        clickedUISlot.AssignedInventorySlot.RemoveToStack(amountAccepted);
                        clickedUISlot.UpdateUISlot();
                        return;
                    }
                }
            }
        }
    }

    public void OrganizeItems(bool isPlayerBackpack)
    {
        List<InventorySlot> ivSlots = new();

        int offset = isPlayerBackpack? 9 : 0;
        for (int i = offset; i < inventorySystem.InventorySlots.Count; i++)
        {
            if (inventorySystem.InventorySlots[i].ItemData)
                ivSlots.Add(inventorySystem.InventorySlots[i]);
        }

        tempInvntorySystem = new InventorySystem(ivSlots.Count);

        foreach (var slot in ivSlots)
        {
            if (tempInvntorySystem.AddToInventory(slot.ItemData, slot.StackSize, out int amountAccepted))
                slot.ClearSlot();
        }

        ivSlots = new();
        foreach (var slot in tempInvntorySystem.InventorySlots)
        {
            if (slot.ItemData != null)
                ivSlots.Add(slot);
        }

        ivSlots.Sort((a, b) => a.ItemData.ID.CompareTo(b.ItemData.ID));

        foreach (var slot in ivSlots)
        {
            if (inventorySystem.AddToInventory(slot.ItemData, slot.StackSize, out int amountAccepted, isPlayerBackpack))
                slot.ClearSlot();
        }
    }

    // public void OrganizeItems2(bool isPlayerBackpack)
    // {
    //     List<InventorySlot> ivSlots = new();

    //     int offset = isPlayerBackpack ? 9 : 0;
    //     for (int i = offset; i < inventorySystem.InventorySlots.Count; i++)
    //     {
    //         if (inventorySystem.InventorySlots[i].ItemData)
    //             ivSlots.Add(inventorySystem.InventorySlots[i]);
    //     }

    //     tempInvntorySystem = new InventorySystem(ivSlots.Count);

    //     foreach (var slot in ivSlots)
    //     {
    //         if (tempInvntorySystem.AddToInventory2(slot.ItemData, slot.StackSize))
    //         {
    //             slot.ClearSlot();
    //         }
    //     }

    //     ivSlots = new();
    //     foreach (var slot in tempInvntorySystem.InventorySlots)
    //     {
    //         if (slot.ItemData != null)
    //             ivSlots.Add(slot);
    //     }

    //     ivSlots.Sort((a, b) => a.ItemData.ID.CompareTo(b.ItemData.ID));

    //     foreach (var slot in ivSlots)
    //     {
    //         if (isPlayerBackpack)
    //         {

    //             // if (inventorySystem.AddToInventory(slot.ItemData, slot.StackSize, true))
    //             //     slot.ClearSlot();

    //             if (inventorySystem.AddToInventory2(slot.ItemData, slot.StackSize, true))
    //             {
    //                 slot.ClearSlot();
    //             }
    //         }
    //         else
    //         {
    //             // if (inventorySystem.AddToInventory(slot.ItemData, slot.StackSize, false))
    //             //     slot.ClearSlot();

    //             if (inventorySystem.AddToInventory2(slot.ItemData, slot.StackSize))//, out int amountAccepted))
    //             {
    //                 slot.ClearSlot();
    //                 // if (amountAccepted == slot.StackSize)
    //                 //     slot.ClearSlot();
    //                 // else
    //                 //     slot.RemoveToStack(amountAccepted);
    //             }
    //         }
    //     }
    // }

    private void SwapSlots(InventorySlot_UI clickedUISlot)
    {
        var clonedSlot = new InventorySlot(mouseInventoryItem.AssignedInventorySlot.ItemData, mouseInventoryItem.AssignedInventorySlot.StackSize);
        mouseInventoryItem.ClearSlot();

        mouseInventoryItem.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);

        clickedUISlot.ClearSlot();
        clickedUISlot.AssignedInventorySlot.AssignItem(clonedSlot);
        clickedUISlot.UpdateUISlot();
    }

    private int SplitStack(int stack)
    {
        int half = stack / 2;

        bool isOdd = stack % 2 != 0;

        if (isOdd)
            half += stack > 0 ? 1 : -1; // Adiciona 1 se o número original for positivo, subtrai 1 se for negativo

        return half;
    }
}

using UnityEngine;

public class AddItemDemo : MonoBehaviour
{
    [SerializeField] private InventoryItemData itemData = null;
    [SerializeField] private HotbarDisplay hotbarDisplay = null;
    [SerializeField] private DynamicInventoryDisplay backpackDisplay = null;

    public void AddItem()
    {
        if(backpackDisplay.slotsUi.Count == 0)
            hotbarDisplay.InventorySystem.AddToInventory(itemData, 1, out int AmountAcepted);
        else
            backpackDisplay.InventorySystem.AddToInventory(itemData, 1, out int AmountAcepted);
    }

    public void AddStackItem()
    {
        if(backpackDisplay.slotsUi.Count == 0)
            hotbarDisplay.InventorySystem.AddToInventory(itemData, itemData.MaxStackSize, out int AmountAcepted);
        else
            backpackDisplay.InventorySystem.AddToInventory(itemData, itemData.MaxStackSize, out int AmountAcepted);
    }
}

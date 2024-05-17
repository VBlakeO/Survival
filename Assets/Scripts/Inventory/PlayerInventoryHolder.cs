using UnityEngine.Events;
using UnityEngine;

public class PlayerInventoryHolder : InventoryHolder
{
    [Space]
    public GameObject backpackBase = null;
    public static UnityAction OnPlayerInventoryChange;
    public static UnityAction<InventorySystem, int> OnPlayerInventoryDisplayRequested;

    [SerializeField] private PlayerMovementSystem playerMovementSystem = null;
    [SerializeField] private InventoryUIController inventoryUIController = null;

    private void Start()
    {
        SaveGameManager.data.playerInventory = new InventorySaveData(m_InventorySystem);
        
        if (inventoryUIController == null)
            inventoryUIController = InventoryUIController.Instance;

        if (playerMovementSystem == null)
            playerMovementSystem = PlayerMovementSystem.Instance;
    }

    protected override void LoadInventory(SaveData data)
    {
        if (data.playerInventory.InventorySystem != null)
        {
            this.m_InventorySystem = data.playerInventory.InventorySystem;
            OnPlayerInventoryChange?.Invoke();
        }
    }

    private void HandleFloatingSlot()
    {
        MouseItemData _floatingSlot = InventoryController.Instance.mouseItemData;
        if (_floatingSlot.AssignedInventorySlot.ItemData != null)
        {
            if (InventoryUIController.Instance.backpackPanel.InventorySystem.AddToInventory(_floatingSlot.AssignedInventorySlot.ItemData, _floatingSlot.AssignedInventorySlot.StackSize, out int amountAccepted))
            {
                if (_floatingSlot.AssignedInventorySlot.RemoveToStack(amountAccepted) <= 0)
                    _floatingSlot.ClearSlot();
                else
                    InventoryController.Instance.mouseItemData.Drop();
            }
            else
            {
                InventoryController.Instance.mouseItemData.Drop();
            }
        }
    }

    #region BackpackAccess
    public bool ToggleBackpackAccess()
    {
        if (!backpackBase.activeInHierarchy)
        {
            OpenBackpack();
            return true;
        }
        else
        {
            CloseBackpack();
            return false;
        }
    }

    public void OpenBackpack()
    {
        if (!backpackBase.activeInHierarchy)
        {
            OnPlayerInventoryDisplayRequested?.Invoke(MyInventorySystem, offset);
            inventoryUIController.CursorController(true);
            playerMovementSystem.LockVision(true);
        }
    }

    public void CloseBackpack()
    {
        if (backpackBase.activeInHierarchy)
        {
            backpackBase.SetActive(false);
            inventoryUIController.CursorController(false);
            playerMovementSystem.LockVision(false);

            HandleFloatingSlot();
        }

    }
    #endregion
}
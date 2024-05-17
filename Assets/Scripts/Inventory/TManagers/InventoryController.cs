using UnityEngine;
using UnityEngine.Events;

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance;

    public PlayerInventoryHolder playerInventoryHolder = null; 
    public InventoryHolder externalInventoryHolder = null; 
    public MouseItemData mouseItemData = null;
    [Space]

    [SerializeField] private InventoryUIController inventoryUIController = null; 
    [SerializeField] private InputControl inputControl = null; 
    

    public UnityAction<bool> OnBackpackStateChange;
    public UnityAction<bool> OnExternalInventoryOpen;

    private void Awake() 
    {
        Instance = this;
    }

    private void OnEnable()
    {
        inputControl.OnPressTab += TogglePlayerBackpack;
        inputControl.OnPressEscape += CloseBackpack;
    }

    private void OnDisable()
    {
        inputControl.OnPressTab -= TogglePlayerBackpack;
    }

    public void SetExternalInventoryHolder(InventoryHolder inventory)
    {
        externalInventoryHolder = inventory;
        OnExternalInventoryOpen?.Invoke(inventory != null);
    }

    public void TogglePlayerBackpack()
    {
        if (externalInventoryHolder != null)
            return;

        bool backpackAccess = playerInventoryHolder.ToggleBackpackAccess();
        OnBackpackStateChange?.Invoke(backpackAccess);
    }

    private void CloseBackpack()
    {
        playerInventoryHolder.CloseBackpack();
        OnBackpackStateChange?.Invoke(false);
    }
}

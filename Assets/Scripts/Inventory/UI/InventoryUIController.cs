using UnityEngine;
using UnityEngine.Events;

public class InventoryUIController : MonoBehaviour
{
    public static InventoryUIController Instance;

    public DynamicInventoryDisplay storagePanel = null;
    public DynamicInventoryDisplay backpackPanel = null;

    public UnityAction OnOpenStorage;
    public UnityAction OnOpenBackpack;

    public GameObject transferBox = null;

    private void Awake()
    {
        Instance = this;

        storagePanel.SetInventoryHolderState(false);
        backpackPanel.SetInventoryHolderState(false);
    }

    private void OnEnable()
    {
        InputControl.Instance.OnPressEscape += CloseInventoryHolders;

        InventoryHolder.OnDynamicInventoryDisplayRequested += DisplayInventory;
        PlayerInventoryHolder.OnPlayerInventoryDisplayRequested += DisplayPlayerInventory;
    }

    private void OnDisable()
    {
        InventoryHolder.OnDynamicInventoryDisplayRequested -= DisplayInventory;
        PlayerInventoryHolder.OnPlayerInventoryDisplayRequested -= DisplayPlayerInventory;
    }


    public void CloseInventoryHolders()
    {
        if (backpackPanel.gameObject.activeInHierarchy)  
            backpackPanel.SetInventoryHolderState(false);

        if (storagePanel.gameObject.activeInHierarchy)
            storagePanel.SetInventoryHolderState(false);
    }

    public void CursorController(bool state)
    {
        Cursor.visible = state;

        if (state)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;
    }


    private void DisplayInventory(InventorySystem invToDisplay, int offset)
    {
        transferBox.SetActive(true);
        
        storagePanel.SetInventoryHolderState(true);
        storagePanel.RefreshDynamicInventory(invToDisplay, offset);
        OnOpenStorage?.Invoke();
    }

    private void DisplayPlayerInventory(InventorySystem invToDisplay, int offset)
    {
        backpackPanel.SetInventoryHolderState(true);
        backpackPanel.RefreshDynamicInventory(invToDisplay, offset);
        OnOpenBackpack?.Invoke();
    }
}

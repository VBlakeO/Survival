using UnityEngine.Events;
using UnityEngine;

public abstract class InventoryHolder : MonoBehaviour
{
    [SerializeField] private int inventorySize = 0;
    [SerializeField] protected int offset = 8;
    [SerializeField] protected InventorySystem m_InventorySystem;

    public int Offset => offset;

    public InventorySystem MyInventorySystem => m_InventorySystem;

    public static UnityAction<InventorySystem, int> OnDynamicInventoryDisplayRequested; // Inv System to Display, amount to offset display by

    protected virtual void Awake()
    {
        SaveLoad.OnLoadGame += LoadInventory;
        m_InventorySystem = new InventorySystem(inventorySize);
    }

    protected abstract void LoadInventory(SaveData data);

    public bool AddToInventory(InventoryItemData data, int amount)
    {
        if (m_InventorySystem.AddToInventory(data, amount, out int amountAccepted))
            return true;
        else
            return false;
    }
}

[System.Serializable]
public struct InventorySaveData
{
    public InventorySystem InventorySystem;
    public Vector3 Position;
    public Quaternion Rotation;

    public InventorySaveData(InventorySystem _inventorySystem, Vector3 _position, Quaternion _rotation)
    {
        InventorySystem = _inventorySystem;
        Position = _position;
        Rotation = _rotation;
    }

    public InventorySaveData(InventorySystem _inventorySystem)
    {
        InventorySystem = _inventorySystem;
        Position = Vector3.zero;
        Rotation = Quaternion.identity;
    }
}
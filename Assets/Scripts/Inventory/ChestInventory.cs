using UnityEngine.Events;
using UnityEngine;

[RequireComponent(typeof(UniqueID))]
public class ChestInventory : InventoryHolder, IInteractable
{
    public UnityAction<IInteractable> OnInteractionComplite { get; set; }
    private NewPlayerInteraction playerInteraction = null;

    protected override void Awake()
    {
        base.Awake();
        SaveLoad.OnLoadGame += LoadInventory;
    }

    private void Start()
    {
        SaveInventory();
    }

    private void SaveInventory()
    {
        var chestSaveData = new InventorySaveData(m_InventorySystem, transform.position, transform.rotation);

        if (!SaveGameManager.data.chestDictionary.ContainsKey(GetComponent<UniqueID>().ID))
            SaveGameManager.data.chestDictionary.Add(GetComponent<UniqueID>().ID, chestSaveData);
    }

    protected override void LoadInventory(SaveData data)
    {
        if (data.chestDictionary.TryGetValue(GetComponent<UniqueID>().ID, out InventorySaveData chestSaveData))
        {
            this.m_InventorySystem = chestSaveData.InventorySystem;
            this.transform.position = chestSaveData.Position;
            this.transform.rotation = chestSaveData.Rotation;
        }
    }

    public void Interact(NewPlayerInteraction _playerInteraction)
    {
        InventoryController.Instance.SetExternalInventoryHolder(this);
        OnDynamicInventoryDisplayRequested?.Invoke(MyInventorySystem, 0);
        InventoryController.Instance.playerInventoryHolder.OpenBackpack();
    }

    public void EndInteraction()
    {
        InventoryController.Instance.SetExternalInventoryHolder(null);
    }
}
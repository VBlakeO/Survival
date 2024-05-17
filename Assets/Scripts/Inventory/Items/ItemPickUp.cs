using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(UniqueID))]
public class ItemPickUp : MonoBehaviour
{
    public InventoryItemData ItemData = null;
    public float PickUpRadius = 1f;
    private SphereCollider myCollider = null;

    [SerializeField] private ItemPickUpSaveData itemSaveData; 
    private string id;


    private void Awake()
    {
        id = GetComponent<UniqueID>().ID;
        SaveLoad.OnLoadGame += LoadGame;
        itemSaveData = new ItemPickUpSaveData(ItemData, transform.position, transform.rotation);

        myCollider = GetComponent<SphereCollider>();
        myCollider.isTrigger = true;
        myCollider.radius = PickUpRadius;
    }

    private void Start() 
    {
        SaveGameManager.data.activeItems.Add(id, itemSaveData);
    }

    private void LoadGame(SaveData data)
    {
        if (data.collectedItems.Contains(id))
            Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        if (SaveGameManager.data.activeItems.ContainsKey(id))
            SaveGameManager.data.activeItems.Remove(id);
        SaveLoad.OnLoadGame -= LoadGame;
    }

    private void OnTriggerEnter(Collider other)
    {
        var inventory = other.transform.GetComponent<PlayerInventoryHolder>();

        if (!inventory)
            return;
            
        if (inventory.AddToInventory(ItemData, 1))
        {
            SaveGameManager.data.collectedItems.Add(id);
            Destroy(gameObject);
        }
    }

}

[System.Serializable]
public struct ItemPickUpSaveData
{
    public InventoryItemData ItemData;
    public Vector3 Position;
    public Quaternion Rotation;

    public ItemPickUpSaveData(InventoryItemData _data, Vector3 _positin, Quaternion _rotation)
    {
        ItemData = _data;
        Position = _positin;
        Rotation = _rotation;
    }
}

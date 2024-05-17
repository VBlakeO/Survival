using UnityEngine;

public class ResourceManager : MonoBehaviour
{   
    public static ResourceManager Instance = null;

    void Awake()
    {
        Instance = this;
    }

    public InventoryItemData LoadItemData(int itemId)
    {
        var db = Resources.Load<Database>("Database");
        return db.GetItem(itemId);
    }
}

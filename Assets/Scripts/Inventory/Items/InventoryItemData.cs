using UnityEngine;

public enum ItemCategory {Resources = 0, Equipment = 1, Consumables = 2};

[CreateAssetMenu(menuName = "Inventory System/ Inventory Item")]
public class InventoryItemData : ScriptableObject
{
    public int ID = -1;
    public int itemType = 0;
    public ItemCategory itemCategory = ItemCategory.Resources;
    [Space]

    public Sprite Icon;
    public string DisplayName;
    [TextArea(4,4)] public string Description;
    [Space]
    
    public int MaxStackSize;
    public GameObject itemPrefab;
    [Space]

    public float durability = 100f;

    public void UseItem()
    {
        //Debug.Log("Use Item: " + DisplayName);
    }
}

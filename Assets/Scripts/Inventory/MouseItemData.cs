using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class MouseItemData : Singleton<MouseItemData>
{
    public Image ItemSprite = null;
    public Image TextBox = null;
    public TextMeshProUGUI ItemCountText = null;
    public InventorySlot AssignedInventorySlot = null;
    
    public GameObject Player = null;
    public float dropOffset = 1f;
    [Space]

    private Transform _playerTransform = null;

    protected override void Awake()
    {
        base.Awake();
        ClearSlot();
        ItemSprite.preserveAspect = true;
        _playerTransform = Player.GetComponent<Transform>();
    }

    public void UpdateMouseSlot(InventorySlot inventorySlot)
    {
        AssignedInventorySlot.AssignItem(inventorySlot);
        UpdateMouseSlot();
    }

    public void UpdateMouseSlot()
    {
        ItemSprite.sprite = AssignedInventorySlot.ItemData.Icon;
        ItemCountText.text = AssignedInventorySlot.StackSize.ToString();
        ItemSprite.color = Color.white;

        if (AssignedInventorySlot.ItemData.MaxStackSize > 1)
            TextBox.gameObject.SetActive(true);
        else
            TextBox.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (AssignedInventorySlot.ItemData != null) // if has an item, follow the mouse position;
        {
            transform.position = Input.mousePosition;

            if ((Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1)) && !IsPointerOverUIObject())
                Drop();
        }
    }

    public void Drop()
    {
        if (AssignedInventorySlot.ItemData != null &&AssignedInventorySlot.ItemData.itemPrefab != null)
        {
            for (int i = 0; i < AssignedInventorySlot.StackSize; i++)
                Instantiate(AssignedInventorySlot.ItemData.itemPrefab, _playerTransform.position + _playerTransform.forward * dropOffset, Quaternion.identity);

            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        AssignedInventorySlot?.ClearSlot();
        ItemSprite.sprite = null;
        ItemSprite.color = Color.clear;
        ItemCountText.text = "";
        print("Clear");

        TextBox.gameObject.SetActive(false);
    }

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new(EventSystem.current)
        {
            position = Input.mousePosition
        };
        
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
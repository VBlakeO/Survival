using UnityEngine;
using TMPro;

public class InspectorControl : MonoBehaviour
{
    public static InspectorControl Instance;

    public TextMeshProUGUI itemNameText = null;
    public TextMeshProUGUI itemDescriptionText = null;

    public GameObject useButton = null;
    public GameObject repairButton = null;
    public GameObject recycleButton = null;

    private InventorySlot selectedSlot = null;

    public void Awake()
    {
        Instance = this;
    }

    private void OnEnable() {
        ClearInspector();
    }

    public void UpdateInspector(InventorySlot _slot)
    {
        if (_slot.ItemData == null)
            return;

        selectedSlot = _slot;
        itemNameText.text = _slot.ItemData.DisplayName;
        itemDescriptionText.text = _slot.ItemData.Description;

        useButton.SetActive(false);
        repairButton.SetActive(false);
        recycleButton.SetActive(false);

        switch (_slot.ItemData.itemType)
        {
            case 2:
                repairButton.SetActive(true);
                recycleButton.SetActive(true);
                break;
            case 3:
                repairButton.SetActive(true);
                recycleButton.SetActive(true);
                break;

            case 4:
                useButton.SetActive(true);
                break;

            case 5:
                useButton.SetActive(true);
                break;

            default:
                useButton.SetActive(false);
                repairButton.SetActive(false);
                recycleButton.SetActive(false);
                break;
        }
    }

    public void ClearInspector()
    {
        selectedSlot = null;
        itemNameText.text = "";
        itemDescriptionText.text = "";

        useButton.SetActive(false);
        repairButton.SetActive(false);
        recycleButton.SetActive(false);

    }
}

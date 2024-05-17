using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class InventorySlot_UI : MonoBehaviour, IPointerDownHandler, IPointerMoveHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private Image TextBox = null;
    [SerializeField] private Image itemSprite = null;
    [SerializeField] private GameObject slotHighlight = null;
    [SerializeField] private TextMeshProUGUI itemCountText = null;
    [SerializeField] private InventorySlot assignedInventorySlot = null;
    

    private bool _using = false;
    private MouseItemData mouseItemData = null;
    private InspectorControl inspectorControl = null;


    public InventorySlot AssignedInventorySlot => assignedInventorySlot;
    public InventoryDisplay ParentDisplay { get; private set; }
    public DynamicInventoryDisplay dynamicInventoryDisplay = null;

    public UnityAction<InventorySlot_UI> OnUpdateSlot = null;

    public void ToggleHighlight() => slotHighlight.SetActive(!slotHighlight.activeInHierarchy);

    public void SetHighlight(bool state) => slotHighlight.SetActive(state);

    private void Awake()
    {
        ClearSlot();

        itemSprite.preserveAspect = true;
        ParentDisplay = dynamicInventoryDisplay;

        if (ParentDisplay == null)
            ParentDisplay = transform.GetComponentInParent<InventoryDisplay>();
    }

    public void Init(InventorySlot slot)
    {
        assignedInventorySlot = slot;
        UpdateUISlot(slot);

        slot.OnClear += PartialClearSlot;
        
        mouseItemData = MouseItemData.Instance;
        inspectorControl = InspectorControl.Instance;
    }

    private void OnDisable() 
    {
        if (assignedInventorySlot != null) 
            assignedInventorySlot.OnClear -= PartialClearSlot;
    }

    public void UpdateUISlot(InventorySlot slot)
    {
        if (slot.ItemData != null)
        {
            itemSprite.sprite = slot.ItemData.Icon;
            itemSprite.color = Color.white;
            itemCountText.text = slot.StackSize.ToString();

            if (slot.ItemData.MaxStackSize > 1)
                TextBox.gameObject.SetActive(true);
            else
                TextBox.gameObject.SetActive(false);
        }
        else
        {
            ClearSlot();
        }

        OnUpdateSlot?.Invoke(this);        
    }

    public void UpdateUISlot()
    {
        if (assignedInventorySlot != null) 
            UpdateUISlot(assignedInventorySlot);
    }

    public void PartialClearSlot()
    {
        itemSprite.sprite = null;
        itemSprite.color = Color.clear;
        itemCountText.text = "";
        TextBox.gameObject.SetActive(false);
    }

    public void ClearSlot()
    {
        itemSprite.sprite = null;
        itemSprite.color = Color.clear;
        itemCountText.text = "";
        TextBox.gameObject.SetActive(false);
        assignedInventorySlot?.ClearSlot();
    }


    public void OnUISlotClick()
    {
        ParentDisplay?.SlotCliked(this);
        UpdateUISlot();
    }

    public void OnUISlotRightClick()
    {
        ParentDisplay?.SlotRightCliked(this);
        UpdateUISlot();
    }

    public void OnPointerDown(PointerEventData eventData)
    {   
        if (mouseItemData == null)
            mouseItemData = MouseItemData.Instance;

        if (eventData.button == PointerEventData.InputButton.Right)
            OnUISlotRightClick();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            OnUISlotClick();
            return;
        }
        
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (mouseItemData.AssignedInventorySlot.ItemData != null )
            {
                OnUISlotClick();
            }
            else
            {
                dynamicInventoryDisplay?.DisableSlotsHighlight();

                if (inspectorControl == null)
                    inspectorControl = InspectorControl.Instance;

                inspectorControl.UpdateInspector(assignedInventorySlot);

                SetHighlight(true);
                _using = true;
            }
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (_using)
        {
            OnUISlotClick();
            _using = false;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_using)
            _using = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (assignedInventorySlot.ItemData == null && slotHighlight.activeInHierarchy)
        {
            inspectorControl?.ClearInspector();
            SetHighlight(false);
        }
    }
}
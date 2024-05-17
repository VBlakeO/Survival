using UnityEngine.UI;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float InteractionDistance = 3;
    public LayerMask InteractionLayer;
    public bool canInteract = true;
    [Space]
    public GameObject inventoryPanel;

    public bool IsInteracting { get; private set; }

    public Image aimCirculeImage;
    public PlayerMovement playerMovement;
    [SerializeField] private PlayerInventoryHolder playerInventory;

    //[SerializeField] private InventoryController invController = null;
    [SerializeField] private InventoryUIController invUIController = null;

    

    private IInteractable interactable = null;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        //InputControl.Instance.OnPressInsteraction += TryInteract;
        //InputControl.Instance.OnPressEscape += EndInteraction;
    }

    // private void OnDisable()
    // {
    //     InputControl.Instance.OnPressInsteraction -= TryInteract;
    //     InputControl.Instance.OnPressEscape -= EndInteraction;
    // }

    bool CanInteract()
    {
        if (Time.timeScale <= 0 || !canInteract)
            return false;
        else
            return true;
    }

    void Update()
    {
        aimCirculeImage.enabled = Physics.Raycast(transform.position, transform.forward, InteractionDistance, InteractionLayer, QueryTriggerInteraction.Ignore) && CanInteract();
    }

    public void SetPlayerCameraState(bool state)
    {
        playerMovement.m_CantLook = state;
    }


    private void TryInteract()
    {
        interactable = null;

        if (!CanInteract())
            return;

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, InteractionDistance, InteractionLayer, QueryTriggerInteraction.Ignore))
        {
            interactable = hit.transform.GetComponent<IInteractable>();

            if (interactable != null)
                StartInteraction(interactable, hit.transform.gameObject);
        }
    }

    private void StartInteraction(IInteractable interactable, GameObject obj)
    {
        //interactable.Interact();
        IsInteracting = true;

        //invUIController.CursorController(true);

        if (obj.GetComponent<InventoryHolder>())
            playerInventory.OpenBackpack();
    }


    private void EndInteraction()
    {
        if (InventoryController.Instance.mouseItemData.AssignedInventorySlot.ItemData != null)
        {
            if (InventoryController.Instance.playerInventoryHolder.AddToInventory(InventoryController.Instance.mouseItemData.AssignedInventorySlot.ItemData, InventoryController.Instance.mouseItemData.AssignedInventorySlot.StackSize))
            {
                InventoryController.Instance.mouseItemData.AssignedInventorySlot.ClearSlot();
                InventoryController.Instance.mouseItemData.ClearSlot();
            }
            else
                InventoryController.Instance.mouseItemData.Drop();
        }

        invUIController.CloseInventoryHolders();
        //invUIController.CursorController(false);

        EndInteraction(interactable);
    }

    public void EndInteraction(IInteractable _interactable)
    {
        _interactable?.EndInteraction();

        interactable = null;
        IsInteracting = false;
    }
}

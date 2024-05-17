using UnityEngine;
using UnityEngine.Events;

public class NewPlayerInteraction : MonoBehaviour
{
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [Space]

    [SerializeField] private bool cantInteract = false;
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactableLayer = 1;
    [Space]

    [SerializeField] private HudManager hudManager = null;
    [SerializeField] private IInteractable interactableObj = null;

    [SerializeField] private bool isInteracting = false;
    [SerializeField] private bool isInventoryHolder = false;

    [SerializeField] private PlayerMovementSystem playerMovementSystem = null;
    public UnityAction OnInteractWithInventoryHolder = null;

    private bool CanInteract() => !cantInteract;

    private void Update() 
    {
        hudManager?.SetInteractiveObjeteDebug(GetRaycastHit());

        if (Input.GetKey(interactionKey))
            TryInteract();

        if (Input.GetKeyUp(interactionKey) && isInteracting)
        {
            if (!isInventoryHolder)
                EndInteraction();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            EndInteraction();
    }

    public void TryInteract()
    {
        if (!CanInteract())
            return;

        if (GetRaycastHit(out RaycastHit _hit))
        {
            interactableObj = _hit.transform.GetComponent<IInteractable>();

            if (interactableObj == null)
                return;

            isInteracting = true;

            if (_hit.transform.GetComponent<InventoryHolder>())
            {
                if (!isInventoryHolder)
                {
                    interactableObj.Interact(this);
                    isInventoryHolder = true;
                    playerMovementSystem.LockPlayer(true);
                }
            }
            else
            {
                interactableObj.Interact(this);
            }
        }
        else if (interactableObj != null)
        {
            interactableObj.EndInteraction();
            interactableObj = null;
            isInteracting = false;
        }   
    }

    public void EndInteraction()
    {
        if(interactableObj == null)
        return;
        
        if (isInventoryHolder)
        {
            HandleFloatingSlot();
            playerMovementSystem.LockPlayer(false);
        }

        interactableObj.EndInteraction();
        interactableObj = null;
        isInteracting = false;
    }

    private void HandleFloatingSlot()
    {
        MouseItemData _floatingSlot = InventoryController.Instance.mouseItemData;
        if (_floatingSlot.AssignedInventorySlot.ItemData != null)
        {
            if (InventoryUIController.Instance.backpackPanel.InventorySystem.AddToInventory(_floatingSlot.AssignedInventorySlot.ItemData, _floatingSlot.AssignedInventorySlot.StackSize, out int amountAccepted))
            {
                if (_floatingSlot.AssignedInventorySlot.RemoveToStack(amountAccepted) <= 0)
                    _floatingSlot.ClearSlot();
                else
                    InventoryController.Instance.mouseItemData.Drop();
            }
            else
            {
                InventoryController.Instance.mouseItemData.Drop();
            }
        }

        isInventoryHolder = false;
    }

    private bool GetRaycastHit() => Physics.Raycast(transform.position, transform.forward, interactionRange, interactableLayer, QueryTriggerInteraction.Ignore) && CanInteract();

    private bool GetRaycastHit(out RaycastHit _hit) => Physics.Raycast(transform.position, transform.forward, out _hit, interactionRange, interactableLayer, QueryTriggerInteraction.Ignore);
}
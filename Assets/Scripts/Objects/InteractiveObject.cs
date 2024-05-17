using UnityEngine;
using UnityEngine.Events;

public class InteractiveObject : MonoBehaviour, IInteractable
{
    public UnityAction<IInteractable> OnInteractionComplite { get; set; }
    public UnityEvent OnCancelInteraction = null;
    public UnityEvent OnInteractionCompliteEvent = null;

    [SerializeField] private bool cantInteract = false;
    [Space]

    [SerializeField] private float currentInteractionProgress = 0f;
    [SerializeField] private float requiredInteractionTime = 0f;
    [Space]
    
    [SerializeField] private bool interacted = false;
    [SerializeField] private bool uniqueInteraction = false;
    [SerializeField] private bool continuousInteraction = true;

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }

    private bool CanInteract()
    {
        if(cantInteract)
            return false;
        
        if(uniqueInteraction && interacted)
            return false;
        else 
            return true;
    }

    public float Progress() => currentInteractionProgress / requiredInteractionTime;

    public void Interact(NewPlayerInteraction playerInteraction)
    {
        if (!CanInteract())
            return;

        if (requiredInteractionTime == 0f)
        {
            InteractionAction();
            interacted = true;
            playerInteraction.EndInteraction();
        }
        else
        {
            currentInteractionProgress += Time.deltaTime;

            if (Progress() > 1f)
            {
                InteractionAction();
                interacted = true;
                playerInteraction.EndInteraction();
                currentInteractionProgress = 0f;
            }
        }
    }

    private void InteractionAction()
    {
        OnInteractionCompliteEvent?.Invoke();
        OnInteractionComplite?.Invoke(this);
    }   
     
    private void CancelInteraction()
    {
        if (continuousInteraction)
            currentInteractionProgress = 0f;

        OnCancelInteraction?.Invoke();
    }

    public void EndInteraction()
    {
        if (requiredInteractionTime != 0 && Progress() < 1f)
            CancelInteraction();
    }
}

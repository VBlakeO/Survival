using UnityEngine.Events;

public interface IInteractable
{
    public UnityAction<IInteractable> OnInteractionComplite {get; set;}

    public void Interact(NewPlayerInteraction playerInteraction);

    public void EndInteraction();
}

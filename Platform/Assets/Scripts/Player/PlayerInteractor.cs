using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private IInteractable currentInteractable;

    public string CurrentPrompt => currentInteractable != null ? currentInteractable.PromptText : string.Empty;

    private void Update()
    {
        if (currentInteractable != null && Input.GetKeyDown(interactKey))
        {
            currentInteractable.Interact(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            currentInteractable = interactable;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null && interactable == currentInteractable)
        {
            currentInteractable = null;
        }
    }
}

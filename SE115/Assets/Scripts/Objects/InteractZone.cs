using UnityEngine;

public class InteractZone : MonoBehaviour
{
    private IInteractable interactable;

    private void Awake()
    {
        if(interactable == null)
            interactable = GetComponentInParent<IInteractable>();
        if (interactable == null) 
            interactable = GetComponent<IInteractable>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null && interactable != null)
        {
            interactable.SetPlayerInRange(player);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null && interactable != null)
        {
            interactable.SetPlayerInRange(null);
        }
    }
}
using UnityEngine;

public class GateDoor : TriggerReceiver
{
    [SerializeField] private Collider2D blockingCollider;
    [SerializeField] private Animator animator;

    [SerializeField] private bool startOpened = false;
    private void Start()
    {
        if (startOpened)
        {
            isActivated = true;
            OnTriggerActive();
        }
        else
        {
            isActivated = false;
            OnTriggerInactive(); 
        }
    }
    protected override void OnTriggerActive()
    {
        if (animator != null) animator.SetBool("isOpen", true);
        if (blockingCollider != null) blockingCollider.enabled = false;

        Debug.Log($"[{gameObject.name}] Opened");
    }

    protected override void OnTriggerInactive()
    {
        if (animator != null) animator.SetBool("isOpen", false);
        if (blockingCollider != null) blockingCollider.enabled = true;

        Debug.Log($"[{gameObject.name}] Closed");
    }
}
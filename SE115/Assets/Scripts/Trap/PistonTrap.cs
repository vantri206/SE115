using UnityEngine;
using System.Collections;

public class PistonTrap : TriggerReceiver
{
    [Header("Settings")]
    [SerializeField] private float startDelayTime = 0.2f;
    [SerializeField] private float stayPushTime = 1.0f;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    public bool isPushing = false;
    private bool isPulling = true;

    private bool isActivting = false;

    private void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        isActivated = false;
    }
    protected override void OnTriggerActive()
    {
        if (!isActivting)
        {
            StartCoroutine(PistonnRoutine());
        }
    }

    protected override void OnTriggerInactive() { }

    IEnumerator PistonnRoutine()
    {
        isActivting = true; 

        yield return new WaitForSeconds(startDelayTime);

        animator.SetTrigger("Push");

        yield return new WaitForSeconds(0.2f);
        yield return new WaitUntil(() => isPushing == false);

        yield return new WaitForSeconds(stayPushTime);

        isPulling = false;
        animator.SetTrigger("Pull");

        yield return new WaitUntil(() => isPulling == true);

        isActivting = false;
        isActivated = false; 
    }

    public void AE_StartPush() { isPushing = true; }
    public void AE_StopPush() { isPushing = false; }
    public void AE_FinishPulling() { isPulling = true; }
}
using UnityEngine;
using System.Collections;

public class PistonTrapController : MonoBehaviour
{
    [Header("Physics Settings")]
    [SerializeField] private float pushForce = 40f;
    [SerializeField] private float liftForce = 8f;

    [Header("Timing Settings")]
    [SerializeField] private float startDelayTime = 0f;
    [SerializeField] private float stayPushTime = 1.0f;
    [SerializeField] private float idleTime = 2.0f;   

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    private bool isPushing = false;
    private bool isPulling = true;

    private void Start()
    {
        if (rb == null) 
            rb = GetComponent<Rigidbody2D>();
        if (animator == null) 
            animator = GetComponent<Animator>();
        StartCoroutine(PistonLoop());
    }

    IEnumerator PistonLoop()
    {
        yield return new WaitForSeconds(startDelayTime);

        while (true)
        {
            animator.SetTrigger("Push");

            yield return new WaitForSeconds(0.15f);
            yield return new WaitUntil(() => isPushing == false);

            yield return new WaitForSeconds(stayPushTime);

            isPulling = false; 
            animator.SetTrigger("Pull");

            yield return new WaitUntil(() => isPulling == true);

            yield return new WaitForSeconds(idleTime);
        }
    }
    public void AE_StartPush()
    {
        isPushing = true;
    }
    public void AE_StopPush()
    {
        isPushing = false;
    }
    public void AE_FinishPulling()
    {
        isPulling = true;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isPushing && collision.gameObject.CompareTag("Player"))
        {
            Vector2 pushDirection = transform.up; 
            Vector2 dirToPlayer = collision.transform.position - transform.position;

            float dot = Vector2.Dot(pushDirection, dirToPlayer.normalized);

            if (dot > 0)
            {
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.linearVelocity = Vector2.zero;

                    Vector2 finalVelocity = pushDirection * pushForce;

                    if (Mathf.Abs(pushDirection.y) < 0.5f)
                    {
                        finalVelocity.y += liftForce;
                    }

                    playerRb.linearVelocity = finalVelocity;
                }
            }
        }
    }
}
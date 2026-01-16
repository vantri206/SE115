using UnityEngine;

public class PistonPushHitbox : MonoBehaviour
{
    [Header("Push Settings")]
    [SerializeField] private float pushForce = 60f; 
    [SerializeField] private float liftForce = 8f;  

    [Header("Reference")]
    [SerializeField] private PistonTrap pistonTrap;
    private void OnTriggerStay2D(Collider2D other)
    {
        if (pistonTrap != null && pistonTrap.isPushing && other.CompareTag("Player"))
        {
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();

            if (playerRb != null)
            {
                Vector2 pushDir = transform.up;
                playerRb.linearVelocity = Vector2.zero;

                Vector2 finalVelocity = pushDir * pushForce;

                if (Mathf.Abs(pushDir.y) < 0.5f)
                {
                    finalVelocity.y += liftForce;
                }
                playerRb.linearVelocity = finalVelocity;
                Debug.Log("Piston push player");
            }
        }
    }
}
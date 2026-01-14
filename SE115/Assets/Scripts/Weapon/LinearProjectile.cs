using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class LinearProjectile : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Speed")]
    [SerializeField] private float speed = 10f;
    [Tooltip("Life Time")]
    [SerializeField] private float lifetime = 5f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime); 
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = transform.right * speed;
    }
}
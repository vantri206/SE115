using System.Collections;
using UnityEngine;

public class MagicOrb : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float detectionRange = 15f;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 200f;
    [SerializeField] private float acceleration = 2f;
    private float currentSpeed = 0f;

    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private float explosionDamage = 25f;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private float explosionDistance = 0.5f;

    [Header("Lifetime Settings")]
    [SerializeField] private float maxLifetime = 10f;
    private float lifetimeTimer = 0f;


    private Rigidbody2D rb;
    private bool hasExploded = false;
    private Vector2 moveDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    void Start()
    {
        FindTarget();
    }

    void Update()
    {
        if (hasExploded) return;

        lifetimeTimer += Time.deltaTime;

        if (lifetimeTimer >= maxLifetime)
        {
            Explode();
            return;
        }

        if (target == null)
        {
            FindTarget();
        }

        if (target != null)
        {
            float distanceToTarget = Vector2.Distance(transform.position, target.position);

            if (distanceToTarget <= explosionDistance)
            {
                Explode();
                return;
            }
        }
    }

    void FixedUpdate()
    {
        if (hasExploded || target == null) return;

        Vector2 directionToTarget = (target.position - transform.position).normalized;
        moveDirection = directionToTarget;

        currentSpeed = Mathf.MoveTowards(currentSpeed, moveSpeed, acceleration * Time.fixedDeltaTime);

        rb.linearVelocity = moveDirection * currentSpeed;

    }

    private void FindTarget()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(
            transform.position,
            detectionRange,
            playerLayer
        );

        if (playerCollider != null)
        {
            target = playerCollider.transform;
        }
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;


        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(
                explosionEffect,
                transform.position,
                Quaternion.identity
            );
            Destroy(explosion, 2f);
        }

        DealExplosionDamage();


        Destroy(gameObject);
    }

    private void DealExplosionDamage()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(
            transform.position,
            explosionRadius,
            playerLayer
        );

        foreach (Collider2D hitCollider in hitColliders)
        {
            IDamageable damageable = hitCollider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(explosionDamage, transform.position);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Weapon"))
        {
            Explode();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, explosionDistance);
    }
}
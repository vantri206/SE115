using System;
using UnityEngine;

public class BoomerangProjectiles : MonoBehaviour
{
    private Transform owner;
    [Header("References")]
    public Rigidbody2D myRigidbody;
    private SwordDamage damage;

    [Header("Settings")]
    [SerializeField] private float flySpeed = 10.0f;
    [SerializeField] private float flyReturnSpeed = 15.0f;
    [SerializeField] private float flyDuration = 2.0f;
    [SerializeField] private float catchDistance = 0.25f;

    private Vector2 flyingDirection;

    private float timer;
    private bool hasCaught = false;
    public bool isReturning { get; private set; } = false;

    private Action onCaught;
    private Action onReturn;

    private void Awake()
    {
        if (myRigidbody == null)
            myRigidbody = GetComponent<Rigidbody2D>();
        if (damage == null)
            damage = GetComponent<SwordDamage>();
    }
    public void Initialize(Transform owner, Vector2 direction, Action onReturn, Action onCaught)
    {
        this.owner = owner;
        flyingDirection = direction.normalized;
        this.onReturn = onReturn;
        this.onCaught = onCaught;
        hasCaught = false;
    }
    private void Start()
    {
        timer = flyDuration;

        if (flyingDirection == Vector2.zero)
            flyingDirection = transform.right;
    }

    private void Update()
    {
        if (!isReturning)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                OnReturn();
            }
        }
    }
    private void FixedUpdate()
    {
        if (isReturning)
        {
            MovingReturn();
        }
        else
        {
            Moving();
        }
    }
    private void Moving()
    {
        myRigidbody.linearVelocity = flyingDirection * flySpeed;
    }
    private void MovingReturn()
    {
        if (owner == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 directionToOwner = (owner.position - transform.position).normalized;

        myRigidbody.linearVelocity = directionToOwner * flyReturnSpeed;

        float distance = Vector2.Distance(transform.position, owner.position);
        if (distance < catchDistance)
        {
            OnCaught();
        }
    }
    private void OnCaught()
    {
        if (!hasCaught)
        {
            hasCaught = true;
            onCaught?.Invoke();
        }
        Destroy(gameObject);
    }
    private void OnReturn()
    {
        if (!isReturning)
        {
            onReturn?.Invoke();
            isReturning = true;
        }
    }
    private void OnDestroy()
    {
        onCaught.Invoke();
    }
}
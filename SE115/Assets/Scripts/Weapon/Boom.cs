using NUnit.Framework;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;

public class Boom : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D myRigidbody;
    public Collider2D myCollider;
    public SwordDamage damage;
    public Hitbox hitbox;

    [SerializeField] private float explosionTime = 2.0f;
    private float explosionTimer = 0.0f;
    private bool isOnGround = false;

    public void Awake()
    {
        if(animator == null)  
            animator = GetComponent<Animator>();
        if(damage == null) 
            damage = GetComponent<SwordDamage>();
        if (myRigidbody == null)
            myRigidbody = GetComponent<Rigidbody2D>();
        if (myCollider == null)
            myCollider = GetComponent<Collider2D>();
        if (hitbox == null)
            hitbox = GetComponent<Hitbox>();
    }
    private void Start()
    {
        hitbox.gameObject.SetActive(false);
    }
    private void Update()
    {
        isOnGround = CheckOnGround();

        animator.SetBool("isOnGround", isOnGround);

        if (isOnGround) explosionTimer += Time.deltaTime;
        else explosionTimer = 0;

        if (explosionTimer >= explosionTime)
            Explosion();
    }
    public void Throw(Vector2 throwForce)
    {
        myRigidbody.AddForce(throwForce, ForceMode2D.Impulse);

        animator.SetTrigger("isThrow");
    }
    public void Explosion()
    {
        animator.SetTrigger("isExplosion");
    }
    public void StartDamage()
    {
        hitbox.gameObject.SetActive(true);
    }
    public void FinishDamage()
    {
        hitbox.gameObject.SetActive(false);
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
    public bool CheckOnGround()
    {
        Bounds bounds = myCollider.bounds;

        float checkDistance = 0.1f;
        float offsetX = 0.1f;

        Vector2 left = new Vector2(bounds.min.x + offsetX, bounds.min.y);
        Vector2 center = new Vector2(bounds.center.x, bounds.min.y);
        Vector2 right = new Vector2(bounds.max.x - offsetX, bounds.min.y);

        RaycastHit2D hitLeft = Physics2D.Raycast(left, Vector2.down, checkDistance, LayerMask.GetMask("Platformer"));
        RaycastHit2D hitCenter = Physics2D.Raycast(center, Vector2.down, checkDistance, LayerMask.GetMask("Platformer"));
        RaycastHit2D hitRight = Physics2D.Raycast(right, Vector2.down, checkDistance, LayerMask.GetMask("Platformer"));

        return hitLeft || hitCenter || hitRight;
    }
}
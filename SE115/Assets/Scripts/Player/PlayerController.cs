using System.Windows.Input;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public StateManager stateManager;

    public Rigidbody2D myRigidbody;

    public Animator animator;
    public Sword sword;


    public BoxCollider2D myCollider;

    public Vector2 facingDirection;

    public PlayerInput input;

    public Movement movement;

    public bool isOnGround = false;
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        myRigidbody = gameObject.GetComponent<Rigidbody2D>();
        stateManager = gameObject.GetComponent<StateManager>();
        sword.gameObject.SetActive(false);
        myCollider = gameObject.GetComponent<BoxCollider2D>();
        movement = gameObject.GetComponent<Movement>();
    }
    private void FixedUpdate()
    {
        CheckOnGround();
    }
    private void CheckOnGround()
    {
        this.isOnGround = false;
        Bounds bounds = myCollider.bounds;

        Vector2 origin = new Vector2(bounds.min.x + bounds.extents.x, bounds.min.y);
        Vector2 size = new Vector2(bounds.size.x * 0.95f, 0.05f);
        float distance = 0.05f;
        RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0.0f, Vector2.down, distance, LayerMask.GetMask("Ground"));
        if (hit.collider != null) this.isOnGround = true;

        animator.SetBool("isOnGround", this.isOnGround);
    }
    public void SetFacingDirection(Vector2 facingDirection)
    {
        this.facingDirection = facingDirection;
        this.transform.localScale = new Vector3(facingDirection.x, 1, 1);
    }
}

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public enum MovementType
{
    Ground,
    Flying
}
public class EnemyMovement : MonoBehaviour
{
    [Header("Settings")]
    public MovementType movementType = MovementType.Ground;

    [Header("Enemy Refrences")]
    [SerializeField] private EnemyController enemy;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Vector2 moveDirection;

    [Header("Check Plaformer")]
    [SerializeField] private LayerMask platformerLayer;
    [SerializeField] private Transform groundCheckPos;

    private float groundCheckDistance = 1.0f;
    private float wallCheckDistance = 0.5f;
    private float ceilingCheckDistance = 3.0f;
    public bool canMoveContinuous
    {
        get
        {
            bool hasGroundAhead = CheckGroundAhead();
            bool hasWallAhead = CheckWallAhead();

            if (movementType == MovementType.Flying)
            {
                return !hasWallAhead;
            }
            else
            {
                return hasGroundAhead && !hasWallAhead;
            }
        }
    }
    private void Awake()
    {
        this.enemy = GetComponent<EnemyController>();
        this.rb = enemy.myRigidbody;

        if (groundCheckPos == null)
        {
            groundCheckPos = transform;
        }
    }
    private void Start()
    {
        if(movementType == MovementType.Flying)
        {
            rb.gravityScale = 0.0f;
        }
    }
    public void CheckMove(Vector2 direction, float speed)
    {
        moveDirection = direction.normalized;
        if (moveDirection != Vector2.zero)
        {
            enemy.CheckFacingDirection(moveDirection);

            if (!canMoveContinuous)
            {
                StopMove();
                return; 
            }
            Move(moveDirection, speed);
        }
        else
        {
            StopMove();
        }
    }
    public void MoveNoCheck(Vector2 direction, float speed)
    {
        moveDirection = direction.normalized;
        if (moveDirection != Vector2.zero)
        {
            enemy.CheckFacingDirection(moveDirection);
            Move(moveDirection, speed);
        }
        else
        {
            StopMove();
        }
    }
    public void RunToTarget(Vector2 target, float speed)
    {
        Vector2 direction = new Vector2();
        if (movementType == MovementType.Ground)
        {
            direction = new Vector2(target.x - enemy.transform.position.x, 0).normalized;
        }
        else if(movementType == MovementType.Flying)
        {
            direction = (target - (Vector2)enemy.transform.position).normalized;
        }
        CheckMove(direction, speed);
    }
    public void StopMove()
    {
        if(movementType == MovementType.Ground)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocityY);
        }
        else if(movementType == MovementType.Flying)
        {
            rb.linearVelocity = new Vector2(0f, 0f);
        }

        enemy.animator.SetBool("isMoving", false);
    }
    public void Move(Vector2 moveDirection, float speed)
    {
        if (movementType == MovementType.Ground)
        {
            rb.linearVelocity = new Vector2(moveDirection.x * speed, rb.linearVelocityY);
        }
        else if (movementType == MovementType.Flying)
        {
            rb.linearVelocity = moveDirection * speed;
        }

        enemy.animator.SetBool("isMoving", true);
    }
    public bool CheckGroundAhead()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheckPos.position, Vector2.down, groundCheckDistance, platformerLayer);
        return hit.collider != null;
    }
    public bool CheckWallAhead()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheckPos.position, Vector2.right * enemy.facingDirection.x, wallCheckDistance, platformerLayer);
        return hit.collider != null;
    }
    public bool CheckCeilingAhead()
    {
        return Physics2D.Raycast(groundCheckPos.position, Vector2.up, ceilingCheckDistance, platformerLayer);
    }
    public void OnDrawGizmos()
    {
        Vector3 startPos = (groundCheckPos != null) ? groundCheckPos.position : transform.position;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(startPos, startPos + Vector3.down * groundCheckDistance);

        float direction = 1.0f;
        if (enemy != null)
        {
            if (enemy.facingDirection.x != 0.0f)
            {
                direction = enemy.facingDirection.x;
            }
        }
        else if (transform.localScale.x < 0)
        {
            direction = -1.0f;
        }

        Gizmos.color = Color.purple;
        Vector3 wallDirection = Vector3.right * direction;

        Gizmos.DrawLine(startPos, startPos + wallDirection * wallCheckDistance);

        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(groundCheckPos.position, groundCheckPos.position + Vector3.up * ceilingCheckDistance);
    }
}

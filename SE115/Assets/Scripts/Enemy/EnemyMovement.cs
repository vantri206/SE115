using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public class EnemyMovement : MonoBehaviour
{
    [Header("Enemy Refrences")]
    [SerializeField] private EnemyController enemy;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Vector2 moveDirection;

    [Header("Check Ground")]
    [SerializeField] private LayerMask platformerLayer;
    [SerializeField] private Transform groundCheckPos;

    private float groundCheckDistance = 1.0f;
    private float wallCheckDistance = 0.5f;

    public bool canMoveContinous;

    public bool canAirMoving = false;

    public bool canMoveContinuous
    {
        get
        {
            bool hasGroundAhead = CheckGroundAhead();
            bool hasWallAhead = CheckWallAhead();

            if (canAirMoving) 
                return !hasWallAhead;
            else
                return hasGroundAhead && !hasWallAhead;
        }
    }

    private void Awake()
    {
        this.enemy = GetComponent<EnemyController>();
        this.rb = enemy.myRigidbody;
    }
    public void Move(Vector2 direction, float speed)
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

            rb.linearVelocity = new Vector2(moveDirection.x * speed, rb.linearVelocityY);
        }
        else
        {
            StopMove();
        }
    }
    public void RunToTarget(Vector2 target, float speed)
    {
        Vector2 direction = new Vector2(target.x - enemy.transform.position.x, 0).normalized;
        Move(direction, speed);
        enemy.animator.SetBool("isMoving", true);
    }
    public void StopMove()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocityY);
        enemy.animator.SetBool("isMoving", false);
    }
    public bool CheckGroundAhead()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheckPos.position, Vector2.down, groundCheckDistance, platformerLayer);
        return hit.collider != null;
    }
    public bool CheckWallAhead()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheckPos.position, transform.right * enemy.facingDirection, wallCheckDistance, platformerLayer);
        return hit.collider != null;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(groundCheckPos.position, groundCheckPos.position + Vector3.down * groundCheckDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(groundCheckPos.position, groundCheckPos.position + new Vector3(transform.right.x * enemy.facingDirection.x, 
                                                                                       transform.right.y * enemy.facingDirection.y, transform.right.z)
                                                                                       * wallCheckDistance);
    }
}

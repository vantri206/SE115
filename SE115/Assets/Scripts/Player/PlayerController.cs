using System;
using System.Windows.Input;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Component")]
    public Rigidbody2D myRigidbody;
    public Animator animator;
    public BoxCollider2D myCollider;
    public PlayerInput input;
    public PlayerMovement movement;
    public PlayerCombat combat;
    public PlayerHealth health;
    public PlayerEffect effect;
    public PlayerSkillManager skill;

    [Space(5)]
    [Header("Player Data")]
    public PlayerData data;
    [SerializeField] private int onAirAttackCount = 1;

    [Space(5)]

    [Header("Weapon")]
    public SwordDamage sword;

    private PlayerStateManager stateManager;
    public float lastOnGroundTime { get; private set; }
    public float lastPressedJumpTime { get; private set; }
    public float lastPressedDashTime { get; private set; }

    public int onAirAttackLeft { get; set; }

    [Header("State Parameters")]
    public bool isJumping = false;
    public bool isAttacking = false;
    public bool isDead = false;
    public bool isHurting = false;
    public bool isDashing = false;
    public bool isSliding = false;

    [SerializeField] private LayerMask platformerLayer;
    [SerializeField] private LayerMask oneWayPlatformerLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Raycast Check Settings")]
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private float wallCheckDistance = 0.2f;
    [SerializeField] private float raycastOffset = 0.05f;

    public Vector2 facingDirection;

    public Vector2 startDirection = Vector2.right;

    private int dashLeft;
    private int jumpLeft;

    private float dashTimer;

    public Action<int> onJumpLeftChanged;
    void Awake()
    {
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<BoxCollider2D>();
        movement = GetComponent<PlayerMovement>();
        input = GetComponent<PlayerInput>();
        health = GetComponent<PlayerHealth>();
        effect = GetComponent<PlayerEffect>();
        skill = GetComponent<PlayerSkillManager>();
        stateManager = new PlayerStateManager(this);


        health.onTakeDamage += OnTakeDamage;
        health.onDead += StartDead;

        sword.gameObject.SetActive(false);
    }
    void Start()
    {
        stateManager.ChangeState(stateManager.IdleState);
        this.CheckFacingDirection(startDirection);

        dashLeft = data.dashCountAmount;
    }
    void Update()
    {
        //Timer
        lastOnGroundTime -= Time.deltaTime;
        lastPressedJumpTime -= Time.deltaTime;
        lastPressedDashTime -= Time.deltaTime;

        dashTimer += Time.deltaTime;

        bool isGrounded = CheckOnGround();
        animator.SetBool("isOnGround", isGrounded);
        //Check 
        if (isGrounded)
        {
            lastOnGroundTime = data.coyoteTime;

            if (myRigidbody.linearVelocityY <= 0.2f && jumpLeft < data.jumpCountAmount)
            {
                FillJump(data.jumpCountAmount);
            }
            if (!isDashing && dashLeft < data.dashCountAmount)
            {
                dashLeft = data.dashCountAmount;
            }
            if (!isAttacking && onAirAttackLeft < onAirAttackCount)
            {
                onAirAttackLeft = onAirAttackCount;
            }
        }
        if (isSliding)
        {
            if(myRigidbody.linearVelocityY <= 0.2f && jumpLeft < data.jumpCountAmount)
            {
                FillJump(1);
            }
            if (dashLeft < data.dashCountAmount)
            {
                dashLeft = data.dashCountAmount;
            }
            if (onAirAttackLeft < onAirAttackCount)
            {
                onAirAttackLeft = onAirAttackCount;
            }
        }

        //Handle input
        if (input.isJumpPressed)
        {
            lastPressedJumpTime = data.jumpInputBufferTime;
            input.ResetJumpPressed();
        }
        if(input.isDashPressed)
        {
            lastPressedDashTime = data.dashInputBufferTime;
            input.ResetDashPressed();
        }

        stateManager.Update();
    }
    private void FixedUpdate()
    {
        stateManager.FixedUpdate();
    }

    #region Attack
    public bool CanAttack()
    {
        if (CheckOnGround()) 
            return true;
        else 
            return onAirAttackLeft > 0;
    }
    #endregion

    #region Jump 
    public void OnStartJump()
    {
        lastPressedJumpTime = 0;
        lastOnGroundTime = 0;

        isJumping = true;
        animator.SetBool("isJumping", true);

        ConsumeJump(1);
    }
    public void OnEndJump()
    {
        isJumping = false;
        animator.SetBool("isJumping", false);
    }
    public bool CanJump()
    {
        return jumpLeft > 0;
    }
    public bool IsFalling()
    {
        return (!CheckOnGround() && myRigidbody.linearVelocity.y < -0.2f);
    }
    private void FillJump(int amount)
    {
        jumpLeft = Mathf.Min(data.jumpCountAmount, jumpLeft + amount);
        onJumpLeftChanged?.Invoke(jumpLeft);
    }
    private void ConsumeJump(int amount)
    {
        jumpLeft = Mathf.Max(0, jumpLeft - amount);
        onJumpLeftChanged?.Invoke(jumpLeft);
    }
    #endregion

    #region Hurt/Dead
    public void OnTakeDamage()
    {
        isHurting = true;
    }
    public void StartDead()
    {
        DisablePhysic();
        isDead = true;
    }
    public void FinishHurt()
    {
        isHurting = false;
    }
    #endregion

    #region Dash
    public void OnStartDash()
    {
        lastPressedDashTime = 0;
        lastOnGroundTime = 0;

        dashLeft--;
        dashTimer = float.PositiveInfinity;

        isDashing = true;
        isJumping = false;

        animator.SetBool("isDashing", true);

        health.SetInvincible(true);

        effect.SpawnDashEffect();
    }
    public void FinishDash()
    {
        isDashing = false;
        dashTimer = 0;

        animator.SetBool("isDashing", false);

        health.SetInvincible(false);

        effect.FinishDashEffect();
    }
    public bool CanDash()
    {
        return (dashLeft > 0 && dashTimer >= data.dashCooldownTime);
    }
    #endregion

    #region Sliding
    public void OnStartSliding()
    {
        isSliding = true;
        animator.SetBool("isSliding", true);
    }
    public void OnEndSliding()
    {
        isSliding = false;
        animator.SetBool("isSliding", false);
    }
    public bool CanWallSliding()
    {
        if (CheckWall() && IsFalling()) 
            return true;
        return 
            false;
    }
    #endregion

    #region Helper Function

    public bool CheckOnGround()
    {
        float offset = 0.05f;

        Bounds bounds = myCollider.bounds;

        Vector2 left = new Vector2(bounds.min.x + offset, bounds.min.y);
        Vector2 mid = new Vector2(bounds.center.x, bounds.min.y);
        Vector2 right = new Vector2(bounds.max.x - offset, bounds.min.y);

        RaycastHit2D hitLeft = Physics2D.Raycast(left, Vector2.down, groundCheckDistance, platformerLayer | oneWayPlatformerLayer | wallLayer);
        RaycastHit2D hitMid = Physics2D.Raycast(mid, Vector2.down, groundCheckDistance, platformerLayer | oneWayPlatformerLayer | wallLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(right, Vector2.down, groundCheckDistance, platformerLayer | oneWayPlatformerLayer | wallLayer);

        return hitLeft.collider != null || hitMid.collider != null || hitRight.collider != null;
    }

    public bool CheckWall()
    {
        Bounds bounds = myCollider.bounds;

        Vector2 dir = facingDirection;

        float xPos = (dir.x > 0) ? bounds.max.x : bounds.min.x;

        Vector2 originTop = new Vector2(xPos, bounds.max.y - raycastOffset);
        Vector2 originBot = new Vector2(xPos, bounds.min.y + raycastOffset);

        RaycastHit2D hitTop = Physics2D.Raycast(originTop, dir, wallCheckDistance, platformerLayer | wallLayer);
        RaycastHit2D hitBot = Physics2D.Raycast(originBot, dir, wallCheckDistance, platformerLayer | wallLayer);

        return hitTop.collider != null || hitBot.collider != null;
    }
    public void CheckFacingDirection(Vector2 facingDirection)
    {
        if (this.facingDirection.x != facingDirection.x)
        {
            this.facingDirection = facingDirection;
            this.transform.localScale = new Vector3(this.facingDirection.x * Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
        }
    }
    public void SetGravityScale(float scale)
    {
        myRigidbody.gravityScale = scale;
    }
    private void DisablePhysic()
    {
        myRigidbody.bodyType = RigidbodyType2D.Kinematic;
        myRigidbody.linearVelocity = Vector2.zero;
        myCollider.enabled = false;
    }
    private void OnDrawGizmos()
    {
        if (myCollider == null) return;

        Bounds bounds = myCollider.bounds;

        Gizmos.color = Color.red;
        float y = bounds.min.y;
        Vector2 left = new Vector2(bounds.min.x + raycastOffset, y);
        Vector2 mid = new Vector2(bounds.center.x, y);
        Vector2 right = new Vector2(bounds.max.x - raycastOffset, y);

        Gizmos.DrawLine(left, left + Vector2.down * groundCheckDistance);
        Gizmos.DrawLine(mid, mid + Vector2.down * groundCheckDistance);
        Gizmos.DrawLine(right, right + Vector2.down * groundCheckDistance);

        Gizmos.color = Color.blue;

        Vector2 dir = facingDirection;
        float x = (dir.x > 0) ? bounds.max.x : bounds.min.x;

        Vector2 top = new Vector2(x, bounds.max.y - raycastOffset);
        Vector2 bot = new Vector2(x, bounds.min.y + raycastOffset);

        Gizmos.DrawLine(top, top + dir * wallCheckDistance);
        Gizmos.DrawLine(bot, bot + dir * wallCheckDistance);
    }
    #endregion
}

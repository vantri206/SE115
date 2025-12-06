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
    public float dashTimer { get; private set; }
    public int dashLeft { get; private set; }

    public int onAirAttackLeft { get; set; }

    [Header("State Parameters")]
    public bool isJumpCut = false;
    public bool isJumping = false;
    public bool isAttacking = false;
    public bool isDead = false;
    public bool isHurting = false;
    public bool isDashing = false;

    public Vector2 facingDirection;

    public Vector2 startDirection = Vector2.right;
    void Awake()
    {
        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<BoxCollider2D>();
        movement = GetComponent<PlayerMovement>();
        input = GetComponent<PlayerInput>();
        health = GetComponent<PlayerHealth>();
        effect = GetComponent<PlayerEffect>();
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

        //Check 
        if (CheckOnGround())
        {
            lastOnGroundTime = data.coyoteTime;
        }
        if (CanJump())
        {
            isJumpCut = false;
            input.ResetJumpPressUp();
        }

        if (CheckOnGround())
        {
            //Fill dash
            if (!isDashing && dashLeft < data.dashCountAmount)
            {
                dashLeft = data.dashCountAmount;
            }
            //Fill attack on air
            if (!isAttacking && onAirAttackLeft < onAirAttackCount)
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
        if (input.isJumpPressUp)
        {
            if (isJumping && !IsFalling())
                isJumpCut = true;
            input.ResetJumpPressUp();
        }
        if(input.isDashPressed)
        {
            lastPressedDashTime = data.dashInputBufferTime;
            input.ResetDashPressed();
        }

        animator.SetBool("isJumping", isJumping);

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
        isJumpCut = false;
    }
    public bool CanJump()
    {
        return lastOnGroundTime > 0 && !isJumping;
    }
    public bool IsFalling()
    {
        return (!CheckOnGround() && myRigidbody.linearVelocity.y < -0.2f);
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
        isJumpCut = false;

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

    #region Helper Function
    public bool CheckOnGround()
    {
        Bounds bounds = myCollider.bounds;

        float checkDistance = 0.1f;
        float offsetX = 0.00f;

        Vector2 left = new Vector2(bounds.min.x + offsetX, bounds.min.y);
        Vector2 center = new Vector2(bounds.center.x, bounds.min.y);
        Vector2 right = new Vector2(bounds.max.x - offsetX, bounds.min.y);

        RaycastHit2D hitLeft = Physics2D.Raycast(left, Vector2.down, checkDistance, LayerMask.GetMask("Platformer"));
        RaycastHit2D hitCenter = Physics2D.Raycast(center, Vector2.down, checkDistance, LayerMask.GetMask("Platformer"));
        RaycastHit2D hitRight = Physics2D.Raycast(right, Vector2.down, checkDistance, LayerMask.GetMask("Platformer"));

        bool isHitGround = hitLeft || hitCenter || hitRight;

        return isHitGround && myRigidbody.linearVelocity.y <= 0.01f;
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
    #endregion
}

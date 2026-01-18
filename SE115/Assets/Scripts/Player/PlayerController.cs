using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

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
    public PlayerMana mana;
    public PlayerEffect effect;
    public PlayerSkillManager skill;
    public SpriteRenderer spriteRenderer;

    [Space(5)]
    [Header("Player Data")]
    public PlayerData data;
    [SerializeField] private int onAirAttackCount = 1;

    [Space(5)]
    [Header("Invincible Settings")]
    public float invincibleTime = 2.0f;   
    public float blinkInterval = 0.1f;   

    [Space(5)]

    [Header("Weapon")]
    public SwordDamage sword;

    [Header("Slash Dash")]
    public bool unlockSlashDash = false;
    public float slashDamage = 20.0f;
    public LayerMask enemyLayer;
    public GameObject slashDashPrefabs;

    [Header("Sword Wave")]
    public bool unlockSwordWave = false;
    public GameObject swordWavePrefab;  
    public Transform swordWaveFirePoint;        

    private PlayerStateManager stateManager;
    public float lastOnGroundTime { get; private set; }
    public float lastPressedJumpTime { get; private set; }
    public float lastPressedDashTime { get; private set; }
    public float lastPressedInteractTime { get; private set; }
    public int onAirAttackLeft { get; set; }

    [Header("State Parameters")]
    public bool isJumping = false;
    public bool isAttacking = false;
    public bool isDead = false;
    public bool isHurting = false;
    public bool isDashing = false;
    public bool isSliding = false;
    public bool isShielding = false;

    [SerializeField] private LayerMask platformerLayer;
    [SerializeField] private LayerMask oneWayPlatformerLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Raycast Check Settings")]
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private float wallCheckDistance = 0.2f;
    [SerializeField] private float raycastOffset = 0.05f;
    [SerializeField] private float wallRaycastTopOffset = 0.25f;
    [SerializeField] private float wallRaycastBotOffset = 0.25f;

    [Header("Reflect Skill")]
    public GameObject reflectShieldObj;

    [Header("Control Settings")]
    public bool isInputLocked = false;

    public Vector2 facingDirection;

    public Vector2 startDirection = Vector2.right;

    public int dashLeft { get; private set; }
    public int jumpLeft { get; private set; }

    private float dashTimer;

    public Action<int> onJumpLeftChanged;

    public static PlayerController Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }

        animator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        movement = GetComponent<PlayerMovement>();
        input = GetComponent<PlayerInput>();
        health = GetComponent<PlayerHealth>();
        effect = GetComponent<PlayerEffect>();
        skill = GetComponent<PlayerSkillManager>();
        mana = GetComponent<PlayerMana>();
        stateManager = new PlayerStateManager(this);

        if (health != null)
        {
            health.onTakeDamage += OnTakeDamage;
            health.onDead += StartDead;
        }

        if (reflectShieldObj != null) 
            reflectShieldObj.SetActive(false);

        if(sword != null) 
            sword.gameObject.SetActive(false);
    }
    void OnDestroy()
    {
        if (health != null)
        {
            health.onTakeDamage -= OnTakeDamage;
            health.onDead -= StartDead;
        }
    }
    void Start()
    {
        stateManager.ChangeState(stateManager.IdleState);
        this.CheckFacingDirection(startDirection);

        dashLeft = data.dashCountAmount;

        if(GameplayHUDManager.Instance != null)
        {
            GameplayHUDManager.Instance.AssignPlayer(this);
        }
    }
    void Update()
    {
        //Lock input

        if (isInputLocked || (GameManager.Instance != null && GameManager.Instance.isTransitioning))
        {
            StopPlayer();
            return; 
        }

        //Timer
        lastOnGroundTime -= Time.deltaTime;
        lastPressedJumpTime -= Time.deltaTime;
        lastPressedDashTime -= Time.deltaTime;
        lastPressedInteractTime -= Time.deltaTime;

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
        if(input.isInteractPressed)
        {
            lastPressedInteractTime = 0.2f;
            input.ResetInteractPressed();
        }
        stateManager.Update();
    }
    private void FixedUpdate()
    {
        if (isInputLocked || (GameManager.Instance != null && GameManager.Instance.isTransitioning))
        {
            if (myRigidbody.bodyType == RigidbodyType2D.Dynamic)
            {
                myRigidbody.linearVelocity = new Vector2(0, myRigidbody.linearVelocity.y);
            }
            return; 
        }

        stateManager.FixedUpdate();
    }
    private void StopPlayer()
    {
        if (myRigidbody.bodyType == RigidbodyType2D.Dynamic)
        {
            myRigidbody.linearVelocity = new Vector2(0, myRigidbody.linearVelocity.y);
        }

        input.ResetAttackPressed();
        input.ResetJumpPressed();
        input.ResetDashPressed();
        input.ResetInteractPressed();

        if (!isDead && !isHurting)
        {
            animator.SetBool("isRunning", false); 
            animator.SetBool("isJumping", false);
            animator.SetBool("isDashing", false);
            animator.SetBool("isSliding", false);
        }
    }
    public void LockInput(bool locked)
    {
        isInputLocked = locked;
        if (locked)
        {
            StopPlayer();
        }
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
        if (isDead) return;

        isHurting = true; 
        animator.SetTrigger("Hurt");

        StartCoroutine(IInvincibleRoutine());
    }
    private IEnumerator IInvincibleRoutine()
    {
        health.SetInvincible(true);

        Color originalColor = spriteRenderer.color;
        Color blinkColor = new Color(1f, 1f, 1f, 0f);

        float invincibleTimer = 0f;

        while (invincibleTimer < invincibleTime)
        {
            spriteRenderer.color = (spriteRenderer.color.a > 0.5f) ? blinkColor : originalColor;

            yield return new WaitForSeconds(blinkInterval);

            invincibleTimer += blinkInterval;
        }

        spriteRenderer.color = originalColor; 
        health.SetInvincible(false);

        isHurting = false;
    }
    public void StartDead()
    {
        myRigidbody.bodyType = RigidbodyType2D.Kinematic;
        myRigidbody.linearVelocity = Vector2.zero;
        myCollider.enabled = false;
        spriteRenderer.enabled = true;

        isDead = true;
    }
    public void Dead()
    {
        spriteRenderer.enabled = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RespawnPlayer();
        }
        else
        {
            Debug.LogError("Not found Game Manager!");
        }
    }
    public void FinishHurt()
    {
        isHurting = false;
    }
    public void Respawn(Vector2 checkpointPosition)
    {
        transform.position = checkpointPosition;

        isDead = false;
        isHurting = false;

        StopAllCoroutines(); 
        health.SetInvincible(false); 
        spriteRenderer.color = Color.white;

        myRigidbody.bodyType = RigidbodyType2D.Dynamic;
        myRigidbody.linearVelocity = Vector2.zero;
        myCollider.enabled = true;
        spriteRenderer.enabled = true;

        RestoreStats();
        
        if (stateManager != null)
        {
            stateManager.ChangeState(stateManager.IdleState);
        }

        CinemachineCamera vCam = FindFirstObjectByType<CinemachineCamera>();
        if (vCam != null) 
        {
            vCam.OnTargetObjectWarped(transform, checkpointPosition - (Vector2)transform.position);
        }
    }
    public void RestoreStats()
    {
        health.SetCurrentHeal(health.maxHealth);
        mana.RestoreMana(mana.maxMana);
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

        //health.SetInvincible(true);

        if (!unlockSlashDash)
        {
            effect.SpawnDashEffect();
        }
    }
    public void FinishDash()
    {
        isDashing = false;
        dashTimer = 0;

        animator.SetBool("isDashing", false);

        //health.SetInvincible(false);

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
        Bounds bounds = myCollider.bounds;

        Vector2 left = new Vector2(bounds.min.x + raycastOffset, bounds.min.y);
        Vector2 mid = new Vector2(bounds.center.x, bounds.min.y);
        Vector2 right = new Vector2(bounds.max.x - raycastOffset, bounds.min.y);

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

        Vector2 originTop = new Vector2(xPos, bounds.max.y - wallRaycastTopOffset);
        Vector2 originBot = new Vector2(xPos, bounds.min.y + wallRaycastBotOffset);

        RaycastHit2D hitTop = Physics2D.Raycast(originTop, dir, wallCheckDistance, wallLayer);
        RaycastHit2D hitBot = Physics2D.Raycast(originBot, dir, wallCheckDistance, wallLayer);

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

        Vector2 top = new Vector2(x, bounds.max.y - wallRaycastTopOffset);
        Vector2 bot = new Vector2(x, bounds.min.y + wallRaycastBotOffset);

        Gizmos.DrawLine(top, top + dir * wallCheckDistance);
        Gizmos.DrawLine(bot, bot + dir * wallCheckDistance);
    }
    #endregion

    #region FOR LOAD AND RELOAD
    public PlayerSaveData GetCurrentPlayerData()
    {
        return new PlayerSaveData
        {
            sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,

            position = transform.position,
            rotation = transform.rotation,
            localScale = transform.localScale,
            gravityScale = myRigidbody.gravityScale,

            sprite = spriteRenderer,

            currentHealth = health.currentHealth,

            jumpLeft = this.jumpLeft,
            dashLeft = this.dashLeft,
            onAirAttackLeft = this.onAirAttackLeft,
            facingDirection = this.facingDirection,

            lastOnGroundTime = this.lastOnGroundTime
        };
    }
    public void RestorePlayerData(PlayerSaveData data)
    {
        if (data == null) return;

        transform.position = data.position;
        transform.rotation = data.rotation;

        health.SetCurrentHeal(data.currentHealth);

        spriteRenderer = data.sprite;

        CheckFacingDirection(data.facingDirection); 
        this.jumpLeft = data.jumpLeft;
        this.dashLeft = data.dashLeft;
        this.onAirAttackLeft = data.onAirAttackLeft;

        onJumpLeftChanged?.Invoke(this.jumpLeft);

        this.lastOnGroundTime = data.lastOnGroundTime;
        myRigidbody.gravityScale = data.gravityScale;

        movement.StopMoving();

        isJumping = false;
        isAttacking = false;
        isHurting = false;
        isDashing = false;
        isSliding = false;

        dashTimer = float.PositiveInfinity;

        animator.SetBool("isJumping", false);
        animator.SetBool("isDashing", false);
        animator.SetBool("isSliding", false);
        animator.SetBool("isAttack", false); 


        stateManager.ChangeState(stateManager.IdleState);
    }
    #endregion

    public void AE_Dead() { Dead(); }

    public void ExecuteSlash(Vector2 startPos, Vector2 endPos)
    {
        float distance = Vector2.Distance(startPos, endPos);
        if (distance < 0.5f) return;

        Vector2 direction = (endPos - startPos).normalized;
        Vector2 centerPos = (startPos + endPos) / 2f; 
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float height = 1f;
        if (myCollider != null)
        {
            height = myCollider.bounds.size.y;
        }

        Vector2 boxSize = new Vector2(distance, height);

        RaycastHit2D[] hits = Physics2D.BoxCastAll(centerPos, boxSize, angle, Vector2.zero, 0f, enemyLayer);

        foreach (var hit in hits)
        {
            Hurtbox targetHurtbox = hit.transform.GetComponentInChildren<Hurtbox>();

            if (targetHurtbox != null)
            {
                targetHurtbox.health.TakeDamage(slashDamage, (Vector2)transform.position);
            }
        }

        if (slashDashPrefabs != null)
        {
            GameObject slashObj = Instantiate(slashDashPrefabs, centerPos, Quaternion.Euler(0, 0, angle));
            slashObj.transform.localScale = new Vector3(distance, 2.0f, 1f);
        }
    }
    public void StartShield()
    {
        if (isShielding) return;

        isShielding = true;
        stateManager.ChangeState(stateManager.ShieldingState);
    }
    public void AE_SpawnSwordWave()
    {
        if (!unlockSwordWave || swordWavePrefab == null) return;

        Vector2 spawnPos = (swordWaveFirePoint != null) ? swordWaveFirePoint.position : transform.position;

        GameObject swordWave = Instantiate(swordWavePrefab, spawnPos, Quaternion.identity);

        float directionX = Mathf.Sign(transform.localScale.x);

        LinearProjectiles linearProjectiles = swordWave.GetComponent<LinearProjectiles>();
        if (linearProjectiles != null)
        {
            linearProjectiles.SetDirection(new Vector2(directionX, 0));
        }

        Vector3 scale = swordWave.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * directionX * (-1);
        swordWave.transform.localScale = scale;
    }
    public void UnlockDoubleJump()
    {
        data.jumpCountAmount = 2;
    }
}

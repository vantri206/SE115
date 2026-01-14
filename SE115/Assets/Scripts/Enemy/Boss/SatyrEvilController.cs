using UnityEngine;
using System.Collections;

public enum BossState
{
    Aggressive,
    MovingToPerch,
    SniperMode,
    Stunned,
    Dead
}

public enum CrossUpAction
{
    Melee,
    Shoot
}

public class SatyrEvilController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform[] perchPoints;
    [SerializeField] private GameObject stunVFX;
    [SerializeField] private BossEnemyHealth health;
    [SerializeField] private GameObject rewardInteract;
    [SerializeField] private SatyrReward rewardInteractable;

    [Header("VFX & Hazards")]
    [SerializeField] private GameObject bloodEffectPrefab;
    [SerializeField] private GameObject[] fireHazards;
    [SerializeField] private GameObject alertObject;
    [SerializeField] private float alertDelay = 0.5f;

    [Header("Sniper Mode Mechanics")]
    [SerializeField] private GameObject shieldItemPrefab;
    [SerializeField] private Transform[] itemSpawnPoints;
    [SerializeField] private float spawnShieldInterval = 7.5f; 
    private GameObject currentShieldItem;
    private Coroutine spawnItemCoroutine;

    [Header("Weapons")]
    [SerializeField] private EnemyMelee meleeWeapon;
    [SerializeField] private SatyrBeamShooter beamWeapon;
    [SerializeField] private SatyrProjectileShooter projectileWeapon;

    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float jumpSpeed = 20f;
    public float jumpCooldown = 1.0f;
    public float crossDashSpeed = 45f;
    public float dashStopDuration = 0.3f;
    public float dashDistanceMelee = 1.5f;
    public float dashDistanceShoot = 7.0f;

    [Header("Phase Settings")]
    public float[] phaseThresholds = { 0.8f, 0.5f, 0.2f };
    private int currentThresholdIndex = 0;

    [Header("Combat Settings")]
    public float beamDuration = 2.0f;
    public float meleeWindupTime = 0.4f;
    public float groundShootWindup = 1.0f;
    public float stunDuration = 6.0f;   
    public float reflectDamage = 20.0f;  

    private Transform player;
    private BossState currentState;
    private bool isFacingRight = true;
    private bool isAttacking = false;
    private float actionCooldown = 1.0f;
    private float jumpTimer = 0.0f;
    private float actionTimer = 0.0f;
    private float dropDirection = 0f;

    [Header("Ground Check Settings")]
    public Vector2 groundBoxSize = new Vector2(0.8f, 0.2f);

    public bool isGrounded
    {
        get
        {
            if (groundCheck == null) return false;
            return Physics2D.OverlapBox(groundCheck.position, groundBoxSize, 0f, groundLayer);
        }
    }

    private void OnEnable()
    {
        if (health != null)
        {
            health.onDead += HandleDeath;
            health.onTakeDamage += HandleTakeDamage;
        }
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.onDead -= HandleDeath;
            health.onTakeDamage -= HandleTakeDamage;
        }
    }

    private void Start()
    {
        if (health == null) health = GetComponent<BossEnemyHealth>();

        if (GameplayHUDManager.Instance != null)
            GameplayHUDManager.Instance.ShowBossHealth(health);

        GameObject target = GameObject.FindGameObjectWithTag("Player");
        if (target != null) player = target.transform;

        if (stunVFX) stunVFX.SetActive(false);
        if (alertObject) alertObject.SetActive(false);

        isFacingRight = transform.localScale.x > 0;

        ToggleFires(true);
        SwitchState(BossState.Aggressive);
    }

    private void Update()
    {
        if (player == null) return;

        jumpTimer += Time.deltaTime;
        actionTimer += Time.deltaTime;

        if (health.isDead || currentState == BossState.Dead)
        {
            if (currentState != BossState.Dead) Die();
            return;
        }

        switch (currentState)
        {
            case BossState.Aggressive:
                HandleAggressiveLogic();
                break;
        }

        HandleAnimationState();
    }

    void HandleAnimationState()
    {
        if (isGrounded)
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
        }
        else if (rb.linearVelocity.y < -0.1f)
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", true);
        }
    }

    public void SwitchState(BossState newState)
    {
        currentState = newState;
        StopAllCoroutines(); 

        if (spawnItemCoroutine != null)
        {
            StopCoroutine(spawnItemCoroutine);
            spawnItemCoroutine = null;
        }
        if (currentShieldItem != null) Destroy(currentShieldItem);

        if (meleeWeapon) meleeWeapon.FinishAttack();
        if (beamWeapon) beamWeapon.FinishAttack();

        isAttacking = false;
        rb.gravityScale = 3.0f;
        animator.SetBool("isMoving", false);
        animator.SetBool("isJumping", false);
        animator.SetBool("isFalling", false);
        animator.SetBool("isDashing", false);

        switch (currentState)
        {
            case BossState.Aggressive:
                if (health) health.SetInvincible(false);
                rb.gravityScale = 3.0f;
                rb.bodyType = RigidbodyType2D.Dynamic;
                break;
            case BossState.MovingToPerch:
                if (health) health.SetInvincible(true);
                StartCoroutine(MoveToPerchRoutine());
                break;
            case BossState.SniperMode:
                if (health) health.SetInvincible(true);
                rb.gravityScale = 0f;
                rb.linearVelocity = Vector2.zero;
                spawnItemCoroutine = StartCoroutine(SpawnShieldItemRoutine());
                StartCoroutine(SniperPattern());
                break;
            case BossState.Stunned:
                if (health) health.SetInvincible(false);
                StartCoroutine(StunRoutine());
                break;
            case BossState.Dead:
                Die();
                break;
        }
    }
    void ToggleFires(bool isActive)
    {
        if (fireHazards != null)
        {
            foreach (GameObject fire in fireHazards)
            {
                if (fire != null) fire.SetActive(isActive);
            }
        }
    }

    IEnumerator SpawnShieldItemRoutine()
    {
        while (currentState == BossState.SniperMode)
        {
            yield return new WaitForSeconds(spawnShieldInterval);
            SpawnShieldItem();
        }
    }

    void SpawnShieldItem()
    {
        if (shieldItemPrefab == null || itemSpawnPoints.Length == 0) return;

        if (currentShieldItem != null) Destroy(currentShieldItem);

        int index = Random.Range(0, itemSpawnPoints.Length);
        currentShieldItem = Instantiate(shieldItemPrefab, itemSpawnPoints[index].position, Quaternion.identity);
    }

    void HandleAggressiveLogic()
    {
        if (health != null && currentThresholdIndex < phaseThresholds.Length)
        {
            if (health.currentHealth <= health.maxHealth * phaseThresholds[currentThresholdIndex])
            {
                currentThresholdIndex++;
                SwitchState(BossState.MovingToPerch);
                return;
            }
        }

        if (isAttacking) return;

        if (actionTimer <= actionCooldown)
        {
            float xDiffWait = player.position.x - transform.position.x;
            if (Mathf.Abs(xDiffWait) > 1.0f) CheckFacingDirection(Mathf.Sign(xDiffWait));

            if (isGrounded) rb.linearVelocity = new Vector2(0, 0);
            else rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            animator.SetBool("isMoving", false);
            return;
        }

        float distanceX = Mathf.Abs(player.position.x - transform.position.x);
        float distanceY = player.position.y - transform.position.y;
        float rand = Random.value;


        bool isHighAir = !isGrounded || transform.position.y > 1.5f;

        if (distanceY > 2.0f || distanceY < -1.0f)
        {
            MoveToPlayer();
            return;
        }

        if (distanceX > 4.0f) 
        {
            if (isHighAir)
            {
                StartCoroutine(PerformGroundShoot());
            }
            else
            {
                if (rand < 0.20f) StartCoroutine(PerformCrossUp(CrossUpAction.Shoot));
                else if (rand < 0.60f) StartCoroutine(PerformGroundShoot());
                else StartCoroutine(MeleeRushRoutine());
            }
        }
        else 
        {
            if (isHighAir)
            {
                StartCoroutine(PerformMeleeAttack());
            }
            else
            {
                if (rand < 0.35f) StartCoroutine(PerformCrossUp(CrossUpAction.Melee));
                else if (rand < 0.70f) StartCoroutine(MeleeRushRoutine());
                else StartCoroutine(PerformCrossUp(CrossUpAction.Shoot));
            }
        }
    }
    IEnumerator MeleeRushRoutine()
    {
        isAttacking = true;
        float rushTimer = 0f;
        float rushDuration = 3.0f;

        animator.SetBool("isMoving", true);

        while (rushTimer < rushDuration)
        {
            float distance = Mathf.Abs(player.position.x - transform.position.x);

            if (distance <= dashDistanceMelee)
            {
                break;
            }

            float dirX = Mathf.Sign(player.position.x - transform.position.x);
            CheckFacingDirection(dirX);

            rb.linearVelocity = new Vector2(dirX * moveSpeed * 1.25f, rb.linearVelocity.y);

            rushTimer += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isMoving", false);

        yield return StartCoroutine(PerformMeleeAttack());
    }

    void MoveToPlayer()
    {
        animator.ResetTrigger("StartShoot");
        animator.ResetTrigger("EndShoot");

        float xDiff = player.position.x - transform.position.x;
        float yDiff = player.position.y - transform.position.y;
        float dirX = Mathf.Sign(xDiff);

        float targetVelX = 0;
        bool shouldMove = false;

        if (yDiff > -0.5f) dropDirection = 0f;

        if (dropDirection != 0f)
        {
            CheckFacingDirection(dropDirection);
            targetVelX = dropDirection * moveSpeed;
            shouldMove = true;

            rb.linearVelocity = new Vector2(targetVelX, rb.linearVelocity.y);
            if (isGrounded) animator.SetBool("isMoving", shouldMove);
            return;
        }

        if (yDiff < -1.0f && isGrounded)
        {
            if (dropDirection == 0f)
            {
                if (Mathf.Abs(xDiff) > 0.5f) dropDirection = dirX;
                else dropDirection = isFacingRight ? 1f : -1f;

                float wallCheckDist = 2.0f;
                RaycastHit2D wallHit = Physics2D.Raycast(transform.position, Vector2.right * dropDirection, wallCheckDist, groundLayer);
                if (wallHit.collider != null) dropDirection *= -1f; 
            }

            CheckFacingDirection(dropDirection);
            targetVelX = dropDirection * moveSpeed;
            shouldMove = true;
        }
        else if (yDiff > 2.0f && isGrounded)
        {
            if (Mathf.Abs(xDiff) > 0.5f)
            {
                CheckFacingDirection(dirX);
                targetVelX = dirX * moveSpeed;
                shouldMove = true;
            }
            else
            {
                if (jumpTimer >= jumpCooldown)
                {
                    StartCoroutine(PerformHighJump(yDiff));
                    jumpTimer = 0.0f;
                    return;
                }
            }
        }
        else if (isGrounded)
        {
            if (Mathf.Abs(xDiff) > 1.0f) 
            {
                CheckFacingDirection(dirX);
                targetVelX = dirX * moveSpeed;
                shouldMove = true;
            }
        }

        rb.linearVelocity = new Vector2(targetVelX, rb.linearVelocity.y);

        if (isGrounded)
        {
            animator.SetBool("isMoving", shouldMove);
        }
    }

    IEnumerator PerformHighJump(float heightDiff)
    {
        isAttacking = true;
        animator.SetBool("isMoving", false);
        animator.SetBool("isJumping", true);

        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;

        float targetY = transform.position.y + heightDiff + 1.5f;
        Vector2 targetPos = new Vector2(transform.position.x, targetY);

        while (transform.position.y < targetY - 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, jumpSpeed * Time.deltaTime);
            yield return null;
        }

        rb.gravityScale = 3.0f;
        rb.linearVelocity = new Vector2(0, -5f); 

        isAttacking = false;
    }

    IEnumerator     PerformCrossUp(CrossUpAction actionType)
    {
        isAttacking = true;
        animator.SetBool("isMoving", false);
        rb.linearVelocity = Vector2.zero; 

        if (health) health.SetInvincible(true);

        float dashDirection = (player.position.x > transform.position.x) ? 1f : -1f;
        CheckFacingDirection(dashDirection);

        float maxDashDist = (actionType == CrossUpAction.Melee) ? dashDistanceMelee : dashDistanceShoot;
        BoxCollider2D bodyCollider = GetComponent<BoxCollider2D>();
        Vector2 size = bodyCollider != null ? bodyCollider.size : new Vector2(1f, 1f);
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, size, 0f, Vector2.right * dashDirection, maxDashDist, groundLayer);

        float finalDistance = maxDashDist;
        if (hit.collider != null) finalDistance = hit.distance - size.x / 2 - 0.2f;
        if (finalDistance < 0) finalDistance = 0;

        Vector2 startPos = rb.position;
        Vector2 targetPos = startPos + new Vector2(dashDirection * finalDistance, 0);

        animator.SetBool("isDashing", true);

        RigidbodyType2D originalType = rb.bodyType;
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (finalDistance > 0.5f)
        {
            while (Vector2.Distance(rb.position, targetPos) > 0.5f)
            {
                Vector2 newPos = Vector2.MoveTowards(rb.position, targetPos, crossDashSpeed * Time.deltaTime);
                rb.MovePosition(newPos);
                yield return null;
            }
            rb.MovePosition(targetPos);
        }

        rb.bodyType = originalType;
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isDashing", false);

        yield return new WaitForSeconds(dashStopDuration);

        if (health) health.SetInvincible(false);

        float dirToPlayer = (player.position.x > transform.position.x) ? 1f : -1f;
        CheckFacingDirection(dirToPlayer);

        if (actionType == CrossUpAction.Melee)
        {
            animator.SetTrigger("Attack");
            yield return new WaitForSeconds(0.4f);
        }
        else 
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            yield return StartCoroutine(ShowAlert());

            animator.SetTrigger("StartShoot");
            yield return new WaitForSeconds(groundShootWindup);
            if (beamWeapon) beamWeapon.PerformAttack();

            yield return new WaitForSeconds(beamDuration);

            animator.SetTrigger("EndShoot");
            if (beamWeapon) beamWeapon.FinishAttack();


            yield return new WaitForSeconds(2.0f);
        }

        actionTimer = 0.0f;
        isAttacking = false;
    }
    IEnumerator PerformMeleeAttack()
    {
        if (!isAttacking) isAttacking = true;

        animator.SetBool("isMoving", false);
        rb.linearVelocity = Vector2.zero;

        float dirToPlayer = (player.position.x > transform.position.x) ? 1f : -1f;
        CheckFacingDirection(dirToPlayer);

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(meleeWindupTime);

        actionTimer = 0.0f;
        isAttacking = false;
    }

    IEnumerator PerformGroundShoot()
    {
        isAttacking = true;
        animator.SetBool("isMoving", false);
        rb.linearVelocity = Vector2.zero;

        float dirToPlayer = (player.position.x > transform.position.x) ? 1f : -1f;
        CheckFacingDirection(dirToPlayer);

        yield return StartCoroutine(ShowAlert());

        animator.SetTrigger("StartShoot");
        yield return new WaitForSeconds(groundShootWindup);

        if (projectileWeapon)
            projectileWeapon.ShootStraight(isFacingRight ? 1f : -1f);

        animator.SetTrigger("EndShoot");
        yield return new WaitForSeconds(2.0f);

        actionTimer = 0.0f;
        isAttacking = false;
    }

    IEnumerator MoveToPerchRoutine()
    {
        animator.SetBool("isJumping", true);
        animator.SetBool("isMoving", false);
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;

        Transform target = perchPoints[Random.Range(0, perchPoints.Length)];
        while (Vector2.Distance(transform.position, target.position) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, jumpSpeed * Time.deltaTime);
            yield return null;
        }

        animator.SetBool("isJumping", false);
        SwitchState(BossState.SniperMode);
    }

    IEnumerator SniperPattern()
    {
        float dirToPlayer = (player.position.x > transform.position.x) ? 1f : -1f;
        CheckFacingDirection(dirToPlayer);

        animator.SetTrigger("StartShoot");
        yield return new WaitForSeconds(groundShootWindup);

        while (currentState == BossState.SniperMode)
        {
            if (projectileWeapon)
                projectileWeapon.ShootAtTarget(player.position);

            yield return new WaitForSeconds(2.0f);
        }
        animator.SetTrigger("EndShoot");
        yield return new WaitForSeconds(2.0f);
        SwitchState(BossState.Aggressive);
    }

    public void GetHitByReflect()
    {
        if (currentState == BossState.Stunned || currentState == BossState.Dead) return;

        if (health)
        {
            health.SetInvincible(false);
            health.TakeDamage(reflectDamage, player.position); 
        }
        animator.SetTrigger("EndShoot");

        SwitchState(BossState.Stunned);
    }

    IEnumerator StunRoutine()
    {
  
        ToggleFires(false);

        animator.ResetTrigger("StartShoot");
        animator.SetTrigger("Hurt");

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 2.0f;

        if (stunVFX) stunVFX.SetActive(true);

        yield return new WaitUntil(() => isGrounded);

        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(stunDuration); 

        if (stunVFX) stunVFX.SetActive(false);

        animator.SetTrigger("Dash");
        if (health) health.SetInvincible(true);

        float dashDirection = isFacingRight ? 1.0f : -1.0f;
        rb.AddForce(new Vector2(dashDirection * 25f, 0), ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.3f);
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(dashStopDuration);

        ToggleFires(true);
        SwitchState(BossState.Aggressive);
    }

    IEnumerator ShowAlert()
    {
        if (alertObject != null)
        {
            alertObject.SetActive(true);
            yield return new WaitForSeconds(alertDelay);
            alertObject.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(alertDelay);
        }
    }

    void HandleTakeDamage()
    {
        if (currentState == BossState.Dead) return;

        if (bloodEffectPrefab != null)
        {
            Instantiate(bloodEffectPrefab, transform.position, Quaternion.identity);
        }

        if (currentState == BossState.Stunned) return;

        if (currentThresholdIndex < phaseThresholds.Length)
        {
            if (health.currentHealth <= health.maxHealth * phaseThresholds[currentThresholdIndex])
            {
                currentThresholdIndex++;
                SwitchState(BossState.MovingToPerch);
                return;
            }
        }
    }

    void HandleDeath()
    {
        if (currentState == BossState.Dead) return;
        SwitchState(BossState.Dead);
    }

    void Die()
    {
        currentState = BossState.Dead;

        StopAllCoroutines();
        enabled = false; 
        isAttacking = false;

        ToggleFires(false);
        if (currentShieldItem != null) Destroy(currentShieldItem);

        if (meleeWeapon) meleeWeapon.FinishAttack();
        if (beamWeapon) beamWeapon.FinishAttack();
        if (stunVFX) stunVFX.SetActive(false);

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 3.0f; 
        rb.linearVelocity = Vector2.zero; 
        rb.freezeRotation = true;
        rb.simulated = true;

        animator.SetBool("isMoving", false);
        animator.SetBool("isJumping", false);
        animator.SetBool("isFalling", false);
        animator.SetBool("isDashing", false);
        animator.SetTrigger("Dead");

        if (rewardInteract != null)
            rewardInteract.SetActive(true);

        if (rewardInteractable != null)
            rewardInteractable.enabled = true; 

        Debug.Log("Mini Boss Defeated!");
    }

    public void CheckFacingDirection(float direction)
    {
        if (Mathf.Abs(direction) > 0.0f)
        {
            float directionX = Mathf.Sign(direction);
            if ((directionX > 0 && !isFacingRight) || (directionX < 0 && isFacingRight))
            {
                isFacingRight = (directionX > 0);
                Vector3 currentScale = transform.localScale;
                transform.localScale = new Vector3(Mathf.Abs(currentScale.x) * directionX, currentScale.y, currentScale.z);
            }
        }
    }

    private void MeleeAttack() { if (meleeWeapon) meleeWeapon.PerformAttack(); }
    private void FinishMeleeAttack() { isAttacking = false; if (meleeWeapon) meleeWeapon.FinishAttack(); }
    public void AE_MeleeAttack() { MeleeAttack(); }
    public void AE_FinishMeleeAttack() { FinishMeleeAttack(); }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireCube(groundCheck.position, groundBoxSize);
        }
    }
}
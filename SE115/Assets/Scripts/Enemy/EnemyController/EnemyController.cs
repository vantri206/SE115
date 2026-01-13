using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Component")]
    public EnemyStateManager stateManager { get; private set; }
    public Rigidbody2D myRigidbody;
    public Animator animator;
    public Collider2D myCollider;
    public EnemyHealth health;
    public EnemyMovement movement;

    [Space(5)]

    [Header("Weapon")]
    public EnemyWeapon[] weapons;

    [Header("Moving parameters")]

    public Vector2 facingDirection;
    public Vector2 startDirection = Vector2.right;
    public Vector2 startPosition { get; private set; }
    public Vector2 takenDamageSourcePos { get; set; }

    [Header("Attack And Aggro Range Settings")]
    public float attackRange = 1.0f;
    public float aggroTriggerRange = 8.0f;
    public float detectionHeightRange = 5.0f;       //height range for aggro check
    public float attackHeightRange = 5.0f;

    [Header("Attack Cooldown")]
    public float attackCooldown = 2.0f;
    public float attackCooldownTimer { get; set; } = 0.0f;


    [Header("State SO")]
    [SerializeField] private EnemyIdleSOBase enemyIdleBase;
    [SerializeField] private EnemyChaseSOBase enemyChaseBase;
    [SerializeField] private EnemyAttackSOBase enemyAttackBase;
    [SerializeField] private EnemyHurtSOBase enemyHurtBase;
    [SerializeField] private EnemyDeadSOBase enemyDeadBase;

    public EnemyIdleSOBase enemyIdleBaseInstance { get; set; }
    public EnemyChaseSOBase enemyChaseBaseInstance { get; set; }
    public EnemyAttackSOBase enemyAttackBaseInstance { get; set; }
    public EnemyHurtSOBase enemyHurtBaseInstance { get; set; }
    public EnemyDeadSOBase enemyDeadBaseInstance { get; set; }


    [Header("Check parameters")]
    public bool isAttacking = false;
    public bool isHurtStun = false;
    public bool isAggroed = false;

    [Space(5)]
    [Header("Player Refrences")]
    public LayerMask playerLayer;
    public Transform playerTarget = null;
    public Transform attackTarget = null;

    public Action onFinishDead;
    protected virtual void Awake()
    {
        #region State SO Instantiate
        enemyIdleBaseInstance = Instantiate(enemyIdleBase);
        enemyChaseBaseInstance = Instantiate(enemyChaseBase);
        enemyAttackBaseInstance = Instantiate(enemyAttackBase);
        enemyHurtBaseInstance = Instantiate(enemyHurtBase);
        enemyDeadBaseInstance = Instantiate(enemyDeadBase);
        #endregion

        animator = gameObject.GetComponent<Animator>();
        myRigidbody = gameObject.GetComponent<Rigidbody2D>();
        myCollider = gameObject.GetComponent<Collider2D>();
        health = gameObject.GetComponent<EnemyHealth>();
        stateManager = new EnemyStateManager(this);

        #region State SO Initialize
        enemyIdleBaseInstance.Initalize(gameObject, this);
        enemyChaseBaseInstance.Initalize(gameObject, this);
        enemyAttackBaseInstance.Initalize(gameObject, this);
        enemyHurtBaseInstance.Initalize(gameObject, this);
        enemyDeadBaseInstance.Initalize(gameObject, this);
        #endregion

        health.onTakeDamage += OnTakeDamage;
        health.onDead += StartDead;

        startPosition = transform.position;
    }
    protected virtual void Start()
    {
        stateManager.ChangeState(stateManager.EnemyIdleState);
        CheckFacingDirection(startDirection);
    }
    protected virtual void Update()
    {
        attackCooldownTimer += Time.deltaTime;

        CheckAttackRange();
        CheckAggroRange();

        stateManager.Update();
    }
    protected virtual void FixedUpdate()
    {
        stateManager.FixedUpdate();
    }
    protected virtual void OnCollisionEnter2D(Collision2D collision) { }
    protected virtual void OnTriggerEnter2D(Collider2D collider) { }
    #region Attack method
    public virtual bool CanAttack()
    {
        return attackCooldownTimer > attackCooldown;
    }
    public virtual void Attack()
    {
        foreach (EnemyWeapon weapon in weapons)
            weapon.PerformAttack();
    }
    public virtual void FinishAttack()
    {
        isAttacking = false;
        foreach (EnemyWeapon weapon in weapons)
            weapon.FinishAttack();
    }
    #endregion
    #region Take Damage and Dead method
    protected virtual void OnTakeDamage(Vector2 sourcePos)
    {
        isHurtStun = true;
        takenDamageSourcePos = sourcePos;
    }
    public virtual void FinishHurt()
    {
        isHurtStun = false;
        takenDamageSourcePos = Vector2.positiveInfinity;
    }
    public void StartDead()
    {
        movement.StopMove();

        FinishAttack();

        DisablePhysicAndCollider();

        animator.SetTrigger("Dead");
    }
    public void Dead()
    {
        gameObject.SetActive(false);
        onFinishDead?.Invoke();
    }
    private void OnDestroy()
    {
        if (health != null)
        {
            health.onTakeDamage -= OnTakeDamage;
            health.onDead -= StartDead;
        }
    }
    #endregion
    #region Find player method
    public virtual void CheckAggroRange()
    {
        Bounds bounds = myCollider.bounds;

        float bottomY = bounds.min.y;
        float newCenterY = bottomY + (detectionHeightRange / 2);
        Vector3 raycastCenter = new Vector3(bounds.center.x, newCenterY, bounds.center.z);

        RaycastHit2D hit = Physics2D.BoxCast(raycastCenter, 
                                             new Vector2(aggroTriggerRange, detectionHeightRange), 
                                             0, facingDirection, 0, playerLayer);
        if (hit.collider != null)
        {
            playerTarget = hit.collider.transform;
            this.isAggroed = true;
        }
        else
        {
            playerTarget = null;
            this.isAggroed = false;
        }
    }
    public virtual void CheckAttackRange()
    {
        Bounds bounds = myCollider.bounds;

        RaycastHit2D hit = Physics2D.BoxCast(bounds.center, new Vector2(attackRange, attackHeightRange), 0, facingDirection, 0, playerLayer);

        if (hit.collider != null)
        {
            attackTarget = hit.collider.transform;
        }
        else
        {
            attackTarget = null;
        }
    }
    #endregion
    public void ResetEnemyState()
    {
        health.currentHealth = health.maxHealth;

        isAttacking = false;
        isHurtStun = false;
        isAggroed = false;
        playerTarget = null;
        attackTarget = null;

        stateManager.ChangeState(stateManager.EnemyIdleState);
        this.CheckFacingDirection(startDirection);

        this.enabled = false;
    }
    public void CheckFacingDirection(Vector2 facingDirection)
    {
        if (Mathf.Abs(facingDirection.x) > 0.0f)
        {
            float directionX = Mathf.Sign(facingDirection.x);

            if (directionX != this.facingDirection.x)

            {
                this.facingDirection = new Vector2(directionX, this.facingDirection.y);
                Vector3 currentScale = this.transform.localScale;
                this.transform.localScale = new Vector3
                (
                    directionX * Mathf.Abs(currentScale.x),
                    currentScale.y,
                    currentScale.z

                );
            }
        }
    }
    public void ActivateEnemyAI()
    {
        this.enabled = true;
    }
    public void DisablePhysicAndCollider()
    {
        myRigidbody.bodyType = RigidbodyType2D.Kinematic;
        myRigidbody.linearVelocity = Vector2.zero;
        myCollider.enabled = false;
    }
    public void EnablePhysicAndCollider()
    {
        myRigidbody.bodyType = RigidbodyType2D.Dynamic;
        myRigidbody.linearVelocity = Vector2.zero;
        myCollider.enabled = true;
    }
    public void AE_Attack() { Attack(); }
    public void AE_FinishAttack() { FinishAttack(); }
    public void AE_FinishHurt() { FinishHurt(); }
    public void AE_Dead() { Dead(); }
    public virtual void OnDrawGizmos()
    {
        Bounds bounds = myCollider.bounds;

        float bottomY = bounds.min.y;
        float newCenterY = bottomY + (detectionHeightRange / 2);
        Vector3 drawCenter = new Vector3(bounds.center.x, newCenterY, bounds.center.z);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(drawCenter, new Vector3(aggroTriggerRange, detectionHeightRange, 0.0f));

        Gizmos.color = Color.darkCyan;
        Vector3 centerOffset = (Vector3)facingDirection * (attackRange / 2);
        Gizmos.DrawWireCube(bounds.center, new Vector3(attackRange, attackHeightRange, 0.0f));

    }

}

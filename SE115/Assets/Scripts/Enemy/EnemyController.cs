using System;
using UnityEngine;


public class EnemyController : MonoBehaviour
{
    [Header("Enemy Component")]
    public EnemyStateManager stateManager { get; private set; }
    public Rigidbody2D myRigidbody;
    public Animator animator;
    public BoxCollider2D myCollider;
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

    /* Attack */
    public float attackRange = 1.0f;
    public float attackCooldown = 2.0f;
    public float attackCooldownTimer { get; set; } = 0.0f;

    /* Aggro */
    public float aggroTriggerRange = 8.0f;

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
    public LayerMask playerLayer;
    public Transform playerTarget = null;
    public Transform attackTarget = null;

    public Action onFinishDead;
    void Awake()
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
        myCollider = gameObject.GetComponent<BoxCollider2D>();
        health = gameObject.GetComponent<EnemyHealth>();
        stateManager = new EnemyStateManager(this);

        health.onTakeDamage += OnTakeDamage;
        health.onDead += StartDead;

        startPosition = transform.position;
    }
    void Start()
    {
        #region State SO Initialize
        enemyIdleBaseInstance.Initalize(gameObject, this);
        enemyChaseBaseInstance.Initalize(gameObject, this);
        enemyAttackBaseInstance.Initalize(gameObject, this);
        enemyHurtBaseInstance.Initalize(gameObject, this);
        enemyDeadBaseInstance.Initalize(gameObject, this);

        stateManager.ChangeState(stateManager.EnemyIdleState);
        #endregion

        this.CheckFacingDirection(startDirection);
    }
    void Update()
    {
        attackCooldownTimer += Time.deltaTime;

        CheckAttackRange();
        CheckAggroRange();

        stateManager.Update();
    }
    void FixedUpdate()
    {
        stateManager.FixedUpdate();
    }
    #region Attack method
    public bool CanAttack()
    {
        return attackCooldownTimer > attackCooldown;
    }
    public void Attack()
    {
        foreach (EnemyWeapon weapon in weapons)
            weapon.PerformAttack();
    }
    public void FinishAttack()
    {
        isAttacking = false;
        foreach (EnemyWeapon weapon in weapons)
            weapon.FinishAttack();
    }
    #endregion
    #region Take Damage and Dead method
    private void OnTakeDamage(Vector2 sourcePos)
    {
        isHurtStun = true;
        takenDamageSourcePos = sourcePos;
    }
    public void FinishHurt()
    {
        isHurtStun = false;
        takenDamageSourcePos = Vector2.positiveInfinity;
    }
    public void StartDead()
    {
        animator.SetTrigger("Dead");
        movement.StopMove();
        FinishAttack();
        DisablePhysicAndCollider();
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
    public void CheckAggroRange()
    {
        Bounds bounds = myCollider.bounds;
        RaycastHit2D hit = Physics2D.BoxCast(bounds.center, new Vector2(aggroTriggerRange, bounds.size.y), 0, facingDirection, 0, playerLayer);
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
    public void CheckAttackRange()
    {
        Bounds bounds = myCollider.bounds;
        RaycastHit2D hit = Physics2D.BoxCast(bounds.center, new Vector2(attackRange, bounds.size.y), 0, facingDirection, 0, playerLayer);
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
    public void ActivateEnemyAI()
    {
        this.enabled = true;
    }
    public void CheckFacingDirection(Vector2 facingDirection)
    {
        if (this.facingDirection.x != facingDirection.x)
        {
            this.facingDirection = facingDirection;
            this.transform.localScale = new Vector3(this.facingDirection.x * Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
        }
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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.orange;
        Bounds bounds = myCollider.bounds;
        Gizmos.DrawWireCube(bounds.center, new Vector3(attackRange, bounds.size.y, bounds.size.z));
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(bounds.center, new Vector3(aggroTriggerRange, bounds.size.y, 0.0f));
    }

}

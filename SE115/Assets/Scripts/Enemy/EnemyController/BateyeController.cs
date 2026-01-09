using UnityEngine;

public class BateyeController : EnemyController
{
    [Header("Bateye Settings")]
    [SerializeField] private float recoveryDuration = 2.0f;
    [SerializeField] private float dashAttackDuration = 3.0f;
    [SerializeField] private SwordDamage damage;
    [SerializeField] private Hurtbox hurtbox;


    private float attackTimer = float.NegativeInfinity;
    private float recoveryTimer = 0.0f;
    public bool isRecovering = false;

    //For attacking calculator
    private float dashSpeed;
    private Vector2 dashDirection;
    private Vector2 aimOffset;
    private LayerMask obstacleLayer;
    private Transform target;

    protected override void Awake()
    {
        base.Awake();

        if (damage == null)
            damage = GetComponent<SwordDamage>();
        if (hurtbox == null)
            hurtbox = GetComponentInChildren<Hurtbox>();
    }
    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        attackTimer += Time.deltaTime;
        recoveryTimer -= Time.deltaTime;

        if (recoveryTimer <= 0.0f)
        {
            isRecovering = false;
        }

        if (isAttacking)
        {
            attackCooldownTimer += Time.deltaTime;
            stateManager.Update();

            return;
        }

        base.Update();
    }
    protected override void FixedUpdate()
    {
        if (isAttacking)
        {
            if (myCollider.IsTouchingLayers(obstacleLayer) || attackTimer >= dashAttackDuration)
            {
                FinishAttack();
            }
        }

        base.FixedUpdate();
    }
    #region Bateye Specification Function
    public void StartRecovering()
    {
        isRecovering = true;
        recoveryTimer = recoveryDuration;
    }
    public void SetupAttack(float speed, Vector2 offset, LayerMask platformer, Transform attackTarget)
    {
        dashSpeed = speed;
        aimOffset = offset;
        obstacleLayer = platformer;
        target = attackTarget;
    }
    public override void Attack()
    {
        if (!isAttacking) return;

        attackTimer = 0.0f;

        damage.ResetHitList();

        animator.SetBool("isAttacking", true);

        hurtbox.gameObject.SetActive(false);        //Bat cant be attacked when attack

        dashDirection = facingDirection;

        if (target != null)
        {
            Vector3 targetPos = target.position;
            Vector3 aimPos = targetPos + (Vector3)aimOffset;

            dashDirection = (aimPos - transform.position).normalized;
        }

        CheckFacingDirection(dashDirection);

        movement.MoveNoCheck(dashDirection, dashSpeed);
    }
    public override void FinishAttack()
    {
        isAttacking = false;

        attackTimer = float.NegativeInfinity;

        animator.SetBool("isAttacking", false);

        hurtbox.gameObject.SetActive(true);

        movement.StopMove();

        StartRecovering();
    }
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isAttacking) return;

        if ((obstacleLayer & (1 << collision.gameObject.layer)) > 0)
        {
            FinishAttack();
        }
    }
    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (!isAttacking) return;

        if ((obstacleLayer & (1 << collision.gameObject.layer)) > 0)
        {
            FinishAttack();
        }
    }
    #endregion
    public override void CheckAggroRange()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, aggroTriggerRange, playerLayer);

        if (hit != null)
        {
            playerTarget = hit.transform;
            isAggroed = true;
        }
        else
        {
            playerTarget = null;
            isAggroed = false;
        }
    }
    public override void CheckAttackRange()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);

        if (hit != null)
        {
            attackTarget = hit.transform;
            isAggroed = true;
        }
        else
        {
            attackTarget = null;
            isAggroed = false;
        }
    }
    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroTriggerRange);

        Gizmos.color = Color.orange;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
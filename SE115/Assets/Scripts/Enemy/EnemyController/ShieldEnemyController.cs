using UnityEngine;

public class ShieldEnemyController : EnemyController
{
    [Header("Signature for Enemy")]
    [SerializeField] private GameObject blockEffectPrefab;
    [SerializeField] private float shieldBlockTime = 2.0f;

    private float shieldBlockTimer = 0.0f;

    protected override void Awake()
    {
        base.Awake();

        health.checkBlockDirection += CheckShieldBlock;
    }
    protected override void Update()
    {
        shieldBlockTimer += Time.deltaTime;

        if(shieldBlockTimer >= shieldBlockTime)
        {
            FinishAttack();
        }

        stateManager.Update();
    }
    private bool CheckShieldBlock(Vector2 sourcePos)
    {

        if (isHurtStun) return false;

        float dirX = sourcePos.x - transform.position.x;

        if (isAttacking || dirX * facingDirection.x > 0)
        {
            HandleBlock(sourcePos);
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void Attack()
    {
        shieldBlockTimer = 0.0f;

        base.Attack();
    }
    private void HandleBlock(Vector2 sourcePos)
    {
        if (blockEffectPrefab != null)
        {
            Vector2 effectPos = (Vector2)transform.position;
            Instantiate(blockEffectPrefab, effectPos, Quaternion.identity);
        }

        FindPlayerTarget();

        if (!isAttacking)
        {
            attackCooldownTimer = attackCooldown + 1.0f;
        }
    }
    private void FindPlayerTarget()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, new Vector2(aggroTriggerRange, aggroTriggerRange), playerLayer);

        foreach (var hit in hits)
        {
            if (hit.GetComponent<PlayerController>() != null)
            {
                playerTarget = hit.transform;
                attackTarget = hit.transform;
                return;
            }
        }
    }
    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, aggroTriggerRange);


        Gizmos.color = Color.yellow;

        Vector3 topPoint = transform.position + Vector3.up * 3.0f;
        Vector3 bottomPoint = transform.position + Vector3.down * 1.0f;
        Gizmos.DrawLine(topPoint, bottomPoint);

        Vector3 arrowTip = transform.position + (Vector3)facingDirection * 2.0f;
        Gizmos.DrawLine(transform.position, arrowTip);

        Vector3 arrowWingUp = arrowTip - (Vector3)facingDirection * 0.5f + Vector3.up * 0.5f;
        Vector3 arrowWingDown = arrowTip - (Vector3)facingDirection * 0.5f + Vector3.down * 0.5f;
        Gizmos.DrawLine(arrowTip, arrowWingUp);
        Gizmos.DrawLine(arrowTip, arrowWingDown);
    }
}
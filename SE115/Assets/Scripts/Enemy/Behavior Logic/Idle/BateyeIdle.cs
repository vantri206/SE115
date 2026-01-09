using UnityEngine;

[CreateAssetMenu(fileName = "Idle-BateyeIdle", menuName = "Enemy Logic Behavior/Idle Logic/BateyeIdle")]
public class BateyeIdle : EnemyIdleSOBase
{
    private float ignoreTimer = 0.0f;
    private float freezeTimer = 0.0f;

    [Header("Flying Up Settings")]
    [SerializeField] private float hoverHeight = 3.5f;
    [SerializeField] private float maxHeight = 7.0f;
    [SerializeField] private float flyingUpSpeed = 3.0f; 
    [SerializeField] private LayerMask groundLayer;  

    [Header("Idle Settings")]
    [SerializeField] private float idleMoveSpeed = 2.0f;
    [SerializeField] private float idleMoveRange = 5.0f;
    [SerializeField] private float freezeTime = 1.0f;

    [Header("Recovery Settings")]
    [SerializeField] private float timeIgnoreChase = 2.0f;

    private bool shouldStopMove = false;
    private bool shouldChase = true;
    private bool isAtHoverHeight = false;

    private Vector2 targetPos;

    public override void Initalize(GameObject gameObject, EnemyController enemy)
    {
        base.Initalize(gameObject, enemy);
    }

    public override void HandleEnterState()
    {
        base.HandleEnterState();

        shouldChase = true;
        ignoreTimer = 0;
        freezeTimer = freezeTime;

        if (enemy is BateyeController bat && bat.isRecovering)
        {
            isAtHoverHeight = false;
            shouldStopMove = true;
            enemy.movement.StopMove();
        }
        else
        {
            CheckAltitude();
        }
    }

    public override void HandleExitState()
    {
        base.HandleExitState();
    }

    public override void HandleFixedUpdateState()
    {
        base.HandleFixedUpdateState();

        if (enemy is BateyeController bat && bat.isRecovering)
        {
            RaycastHit2D hit = Physics2D.Raycast(enemy.transform.position, Vector2.down, 1000.0f, groundLayer);
            float currentAltitude = (hit.collider != null) ? hit.distance : 1000.0f;

            if (currentAltitude < hoverHeight)
            {
                enemy.movement.Move(Vector2.up, flyingUpSpeed);
            }
            else if (currentAltitude > maxHeight)
            {
                enemy.movement.Move(Vector2.down, flyingUpSpeed);
            }
            else
            {
                enemy.movement.StopMove();
            }
            return;
        }

        if (shouldStopMove || freezeTimer < freezeTime)
        {
            enemy.movement.StopMove();
        }
        else
        {
            if (Vector2.Distance(enemy.transform.position, targetPos) < 0.25f)
            {
                freezeTimer = 0;
                enemy.movement.StopMove();
                SelectNewTargetPos();
            }
            else
            {
                if (isAtHoverHeight)
                {
                    enemy.movement.RunToTarget(targetPos, idleMoveSpeed);
                }
                else
                {
                    enemy.movement.RunToTarget(targetPos, flyingUpSpeed);
                }
            }
        }
        base.HandleFixedUpdateState();
    }
    public override void HandleUpdateState()
    {
        freezeTimer += Time.deltaTime;

        base.HandleUpdateState();

        if (stateManager.currentState != stateManager.EnemyIdleState)
            return;

        if (enemy is BateyeController bat && bat.isRecovering)
        {
            return;
        }
        else
        {
            shouldStopMove = false;
        }

        CheckAltitude();

        bool hasCeiling = enemy.movement.CheckCeilingAhead();

        if (enemy.attackTarget != null)
        {
            if (enemy.CanAttack())
                stateManager.ChangeState(stateManager.EnemeyAttackState);
            return;
        }
        if (enemy.isAggroed)
        {
            if (!shouldChase)
            {
                ignoreTimer += Time.deltaTime;
                if (ignoreTimer >= timeIgnoreChase)
                {
                    shouldChase = true;
                    ignoreTimer = 0;
                }
            }
            else
            {
                Vector2 dirToPlayer = enemy.playerTarget.position - enemy.transform.position;
                Vector2 currentDir = enemy.facingDirection;

                float signX = Mathf.Sign(dirToPlayer.x);
                if (signX != currentDir.x && signX != 0)
                {
                    enemy.CheckFacingDirection(new Vector2(signX, enemy.facingDirection.y));
                }
                if (enemy.movement.canMoveContinuous)
                {
                    stateManager.ChangeState(stateManager.EnemyChaseState);
                    return;
                }
                else
                {
                    shouldChase = false;
                }
            }
        }
        else
        {
            shouldChase = true;
            ignoreTimer = 0;
        }

        if (!enemy.movement.canMoveContinuous)
        {
            freezeTimer = 0;
            SelectNewTargetPos();
        }
    }
    private void CheckAltitude()
    {
        RaycastHit2D hit = Physics2D.Raycast(enemy.transform.position, Vector2.down, 1000.0f, groundLayer);

        float currentAltitude = (hit.collider != null) ? hit.distance : 1000.0f;
        bool hasCeiling = enemy.movement.CheckCeilingAhead();

        if (currentAltitude > maxHeight)
        {
            isAtHoverHeight = false;
            shouldStopMove = false;
            targetPos = new Vector2(enemy.transform.position.x, enemy.transform.position.y - 2.0f);
            return;
        }

        if (currentAltitude < hoverHeight && !hasCeiling)
        {
            isAtHoverHeight = false;
            shouldStopMove = false;
            targetPos = new Vector2(enemy.transform.position.x, enemy.transform.position.y + 2.0f);
            return;
        }

        if (!isAtHoverHeight)
        {
            SelectNewTargetPos(); 
        }
        isAtHoverHeight = true;
        shouldStopMove = false;
    }
    public void ResetTargetPos()
    {
        this.targetPos = enemy.startPosition;
    }
    private void SelectNewTargetPos()
    {
        float distance = Random.Range(0.0f, idleMoveRange);
        if (distance < 1.0f) distance += 1.0f;

        if (!enemy.movement.canMoveContinuous)
        {
            float currentDirX = enemy.facingDirection.x;
            float newDirX = (currentDirX > 0) ? -1.0f : 1.0f;

            targetPos = new Vector2(enemy.startPosition.x + distance * newDirX, enemy.transform.position.y);

            enemy.CheckFacingDirection(new Vector2(newDirX, enemy.facingDirection.y));
        }
        else
        {
            float directionX = Random.Range(0.0f, 1.0f) > 0.5f ? -1.0f : 1.0f;

            targetPos = new Vector2(enemy.startPosition.x + distance * directionX, enemy.transform.position.y);
        }
    }
}

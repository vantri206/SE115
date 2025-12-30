using UnityEngine;

[CreateAssetMenu(fileName = "Idle-Normal", menuName = "Enemy Logic Behavior/Idle Logic/IdleNormal")]
public class EnemyIdleNormal : EnemyIdleSOBase
{
    private float ignoreTimer = 0.0f;
    private float freezeTimer = 0.0f;

    [SerializeField] private float idleMoveSpeed = 2.0f;
    [SerializeField] private float idleMoveRange = 5.0f;
    [SerializeField] private float freezeTime = 1.0f;
    [SerializeField] private float timeIgnoreChase = 2.0f;

    private Vector2 targetPos;

    private bool shouldChase = true;

    public override void HandleEnterState()
    {
        base.HandleEnterState();

        shouldChase = true;
        ignoreTimer = 0;
        freezeTimer = freezeTime;
        ResetTargetPos();
        enemy.movement.StopMove();
    }

    public override void HandleExitState()
    {
        base.HandleExitState();
    }

    public override void HandleFixedUpdateState()
    {
        base.HandleFixedUpdateState();

        if (freezeTimer < freezeTime)
        {
            enemy.movement.StopMove();
            return;
        }

        if (Mathf.Abs(enemy.transform.position.x - targetPos.x) < 0.1f)
        {
            freezeTimer = 0;
            enemy.movement.StopMove();
            SelectNewTargetPos();
        }
        else
        {
            enemy.movement.RunToTarget(targetPos, idleMoveSpeed);
        }
    }

    public override void HandleUpdateState()
    {
        freezeTimer += Time.deltaTime;

        base.HandleUpdateState();

        if (stateManager.currentState != stateManager.EnemyIdleState)
            return;

        if (enemy.attackTarget != null)
        {
            if (enemy.CanAttack())
                stateManager.ChangeState(stateManager.EnemeyAttackState);
            return;
        }
        if (enemy.isAggroed)
        {
            if(!shouldChase)
            {
                ignoreTimer += Time.deltaTime;
                if(ignoreTimer >= timeIgnoreChase)
                {
                    shouldChase = true;
                    ignoreTimer = 0;
                }
            }
            else
            {
                float dirToPlayer = Mathf.Sign(enemy.playerTarget.position.x - enemy.transform.position.x);
                float currentDir = enemy.facingDirection.x;

                if (dirToPlayer != currentDir)
                {
                    enemy.CheckFacingDirection(new Vector2(dirToPlayer, enemy.facingDirection.y));
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
            SelectNewTargetPos();
            freezeTimer = 0;
        }
    }
    public override void Initalize(GameObject gameObject, EnemyController enemy)
    {
        base.Initalize(gameObject, enemy);
    }

    public override void ResetValue()
    {
        base.ResetValue();
    }
    public void ResetTargetPos()
    {
        this.targetPos = enemy.startPosition;
    }
    public void SelectNewTargetPos()
    {
        float distance = Random.Range(0.0f, idleMoveRange);
        if (distance < 1.0f) distance += 1.0f;

        if (!enemy.movement.canMoveContinuous)
        {
            float currentDirX = enemy.facingDirection.x;
            float newDirX = (currentDirX > 0) ? -1.0f : 1.0f;

            targetPos = new Vector2(enemy.startPosition.x + distance * newDirX, enemy.startPosition.y);

            enemy.CheckFacingDirection(new Vector2(newDirX, 0));
        }
        else
        {
            float directionX = Random.Range(0.0f, 1.0f) > 0.5f ? -1.0f : 1.0f; 

            targetPos = new Vector2(enemy.startPosition.x + distance * directionX, enemy.startPosition.y);
        }
    }
}

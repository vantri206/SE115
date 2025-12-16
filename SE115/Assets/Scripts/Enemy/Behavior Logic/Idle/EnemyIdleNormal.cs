using UnityEngine;

[CreateAssetMenu(fileName = "Idle-Normal", menuName = "Enemy Logic Behavior/Idle Logic/IdleNormal")]
public class EnemyIdleNormal : EnemyIdleSOBase
{
    private float freezeTimer = 0.0f;

    [SerializeField] private float idleMoveSpeed = 2.0f;
    [SerializeField] private float idleMoveRange = 5.0f;
    [SerializeField] private float freezeTime = 1.0f;

    private Vector2 targetPos;
    public override void HandleEnterState()
    {
        base.HandleEnterState();

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
        else if (enemy.isAggroed)
        {
            stateManager.ChangeState(stateManager.EnemyChaseState);
            return;
        }

        if (!enemy.movement.canMoveContinuous)
        {
            SelectNewTargetPos();
            freezeTimer = 0; 
            enemy.movement.StopMove();
            return; 
        }
        if (freezeTimer < freezeTime)
        {
            enemy.movement.StopMove();
        }
        else
        {
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

        if (!enemy.movement.canMoveContinous)
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

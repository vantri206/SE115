using UnityEngine;

[CreateAssetMenu(fileName = "Chase-RunDirect", menuName = "Enemy Logic Behavior/Chase Logic/ChaseRunDirect")]
public class EnemyChaseRunDirectToPlayer : EnemyChaseSOBase
{
    private float thresholds = 0.5f;
    [SerializeField] private float chaseMoveSpeed = 4.0f;
    public override void HandleEnterState()
    {
        base.HandleEnterState();
    }

    public override void HandleExitState()
    {
        base.HandleExitState();

        enemy.movement.StopMove();
    }

    public override void HandleFixedUpdateState()
    {
        base.HandleFixedUpdateState();

        if (enemy.playerTarget == null) return;

        float dir = Mathf.Sign(enemy.playerTarget.position.x - enemy.transform.position.x);
        float targetX = enemy.playerTarget.position.x - dir * enemy.attackRange / 3;
        Vector2 targetPos = new Vector2(targetX, enemy.transform.position.y);
        float distance = Mathf.Abs(targetX - enemy.transform.position.x);

        if (distance <= thresholds)
        {
            enemy.movement.StopMove();
            return;
        }

        bool isFacingTarget = Mathf.Sign(enemy.facingDirection.x) == dir;

        if (isFacingTarget && !enemy.movement.canMoveContinuous)
        {
            enemy.movement.StopMove();
            return;
        }

        enemy.movement.RunToTarget(targetPos, chaseMoveSpeed);
    }

    public override void HandleUpdateState()
    {
        base.HandleUpdateState();

        if (stateManager.currentState != stateManager.EnemyChaseState)
            return;

        if (enemy.attackTarget != null)
        {
            if (enemy.CanAttack())
            {
                stateManager.ChangeState(stateManager.EnemeyAttackState);
                return;
            }
        }
        if (enemy.isAggroed == false)
        {
            stateManager.ChangeState(stateManager.EnemyIdleState);
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
}

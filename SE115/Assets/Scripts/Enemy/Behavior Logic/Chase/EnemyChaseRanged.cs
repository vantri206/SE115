using UnityEngine;

[CreateAssetMenu(fileName = "Chase-Ranged", menuName = "Enemy Logic Behavior/Chase Logic/ChaseRanged")]
public class EnemyChaseRanged : EnemyChaseSOBase
{
    private float thresholds = 0.5f;
    [SerializeField] private float startRetreatDistance = 6.0f;
    [SerializeField] private float finishRetreatDistance = 8.0f;
    [SerializeField] private float chaseDistance = 10.0f;
    [SerializeField] private float chaseMoveSpeed = 6.0f;
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
        float targetX = enemy.playerTarget.position.x;

        Vector2 targetPos = new Vector2(targetX, enemy.transform.position.y);
        Vector2 targetRetreatPos = new Vector2(enemy.transform.position.x - dir * 2.0f, enemy.transform.position.y);


        float distance = Mathf.Abs(targetPos.x - enemy.transform.position.x);
        float distanceToRetreat = Mathf.Abs(targetRetreatPos.x - enemy.transform.position.x);

        bool isRetreating = (Mathf.Sign(enemy.myRigidbody.linearVelocity.x) != dir);

        float currentRetreatDistance = isRetreating? finishRetreatDistance : startRetreatDistance;
        if (distance < currentRetreatDistance)
        {
            if (distanceToRetreat > thresholds)
            {
                if (Mathf.Sign(enemy.facingDirection.x) != dir && !enemy.movement.canMoveContinuous)
                {
                    enemy.movement.StopMove();
                }
                else
                {
                    enemy.movement.RunToTarget(targetRetreatPos, chaseMoveSpeed);
                }
            }
            else
            {
                enemy.movement.StopMove();
            }
        }
        else if(distance > chaseDistance)
        {
            if (distance > thresholds)
            {
                if (Mathf.Sign(enemy.facingDirection.x) == dir && !enemy.movement.canMoveContinuous)
                {
                    enemy.movement.StopMove();
                }
                else
                {
                    enemy.movement.RunToTarget(targetPos, chaseMoveSpeed);
                }
            }
            else
            {
                enemy.movement.StopMove();
            }
        }
        else
        {
            enemy.movement.StopMove();
        }
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

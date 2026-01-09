using UnityEngine;

[CreateAssetMenu(fileName = "Chase-NoChase", menuName = "Enemy Logic Behavior/Chase Logic/No Chase")]
public class EnemyNoChase : EnemyChaseSOBase
{
    public override void HandleEnterState()
    {
        base.HandleEnterState();

        enemy.movement.StopMove();
    }

    public override void HandleExitState()
    {
        base.HandleExitState();

        enemy.movement.StopMove();
    }

    public override void HandleFixedUpdateState()
    {
        base.HandleFixedUpdateState();
    }

    public override void HandleUpdateState()
    {
        base.HandleUpdateState();

        if (stateManager.currentState != stateManager.EnemyChaseState)
            return;

        if (enemy.playerTarget == null)
        {
            stateManager.ChangeState(stateManager.EnemyIdleState);
            return;
        }

        if (enemy.attackTarget != null)
        {
            if (enemy.CanAttack())
            {
                stateManager.ChangeState(stateManager.EnemeyAttackState);
                return;
            }
        }
        else if (enemy.isAggroed == false)
        {
            stateManager.ChangeState(stateManager.EnemyIdleState);
            return;
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

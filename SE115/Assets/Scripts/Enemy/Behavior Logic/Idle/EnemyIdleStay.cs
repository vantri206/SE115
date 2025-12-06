using UnityEngine;

[CreateAssetMenu(fileName = "Idle-Stay", menuName = "Enemy Logic Behavior/Idle Logic/IdleStay")]
public class EnemyIdleStay : EnemyIdleSOBase
{
    public override void HandleEnterState()
    {
        base.HandleEnterState();

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
        base.HandleUpdateState();

        if (stateManager.currentState != stateManager.EnemyIdleState)
            return;

        if (enemy.CanAttack())
            stateManager.ChangeState(stateManager.EnemeyAttackState);
        else
            stateManager.ChangeState(stateManager.EnemyChaseState);
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

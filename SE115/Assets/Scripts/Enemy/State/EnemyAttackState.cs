using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    public EnemyAttackState(EnemyStateManager stateManager) : base(stateManager)
    {

    }

    public override void EnterState(StateManager stateManager)
    {
        base.EnterState(stateManager);

        enemy.enemyAttackBaseInstance.HandleEnterState();
    }

    public override void ExitState(StateManager stateManager)
    {
        base.ExitState(stateManager);

        enemy.enemyAttackBaseInstance.HandleExitState();
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        enemy.enemyAttackBaseInstance.HandleFixedUpdateState();
    }
    public override void UpdateState()
    {
        base.UpdateState();

        enemy.enemyAttackBaseInstance.HandleUpdateState();
    }
}

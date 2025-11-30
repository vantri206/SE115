using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    public EnemyIdleState(EnemyStateManager stateManager) : base(stateManager)
    {

    }

    public override void EnterState(StateManager stateManager)
    {
        base.EnterState(stateManager);

        enemy.enemyIdleBaseInstance.HandleEnterState();
    }

    public override void ExitState(StateManager stateManager)
    {
        base.ExitState(stateManager);

        enemy.enemyIdleBaseInstance.HandleExitState();
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        enemy.enemyIdleBaseInstance.HandleFixedUpdateState();
    }
    public override void UpdateState()
    {
        base.UpdateState();

        enemy.enemyIdleBaseInstance.HandleUpdateState();
    }
}

using UnityEngine;

public class EnemyDeadState : EnemyBaseState
{
    public EnemyDeadState(EnemyStateManager stateManager) : base(stateManager)
    {

    }

    public override void EnterState(StateManager stateManager)
    {
        base.EnterState(stateManager);

        enemy.enemyDeadBaseInstance.HandleEnterState();
    }

    public override void ExitState(StateManager stateManager)
    {
        base.ExitState(stateManager);

        enemy.enemyDeadBaseInstance.HandleExitState();
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        enemy.enemyDeadBaseInstance.HandleFixedUpdateState();
    }
    public override void UpdateState()
    {
        base.UpdateState();

        enemy.enemyDeadBaseInstance.HandleUpdateState();
    }
}

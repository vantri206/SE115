using UnityEngine;

public class EnemyChaseState : EnemyBaseState
{
    public EnemyChaseState(EnemyStateManager stateManager) : base(stateManager)
    {

    }

    public override void EnterState(StateManager stateManager)
    {
        base.EnterState(stateManager);

        enemy.enemyChaseBaseInstance.HandleEnterState();
    }

    public override void ExitState(StateManager stateManager)
    {
        base.ExitState(stateManager);

        enemy.enemyChaseBaseInstance.HandleExitState();
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        enemy.enemyChaseBaseInstance.HandleFixedUpdateState();
    }
    public override void UpdateState()
    {
        base.UpdateState();

        enemy.enemyChaseBaseInstance.HandleUpdateState();
    }
}

using UnityEngine;

public class EnemyHurtState : EnemyBaseState
{
    public EnemyHurtState(EnemyStateManager stateManager) : base(stateManager)
    {

    }

    public override void EnterState(StateManager stateManager)
    {
        base.EnterState(stateManager);

        enemy.enemyHurtBaseInstance.HandleEnterState();
    }

    public override void ExitState(StateManager stateManager)
    {
        base.ExitState(stateManager);

        enemy.enemyHurtBaseInstance.HandleExitState();
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        enemy.enemyHurtBaseInstance.HandleFixedUpdateState();
    }
    public override void UpdateState()
    {
        base.UpdateState();

        enemy.enemyHurtBaseInstance.HandleUpdateState();
    }
}

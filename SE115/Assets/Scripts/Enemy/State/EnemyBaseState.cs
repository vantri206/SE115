using Unity.VisualScripting;
using UnityEngine;

public abstract class EnemyBaseState : BaseState
{
    protected EnemyStateManager stateManager;
    protected EnemyController enemy;

    public EnemyBaseState(EnemyStateManager stateManager)
    {
        this.stateManager = stateManager;
        this.enemy = stateManager.enemy;
    }
    public override void EnterState(StateManager stateManager)
    {

    }

    public override void ExitState(StateManager stateManager)
    {

    }
    public override void UpdateState()
    {

    }
    public override void FixedUpdateState()
    {

    }
    public virtual void OnCollisionEnter(StateManager stateManager)
    {

    }

}

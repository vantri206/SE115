using JetBrains.Annotations;
using UnityEngine;

public class EnemyStateManager : StateManager
{
    public EnemyController enemy;

    public EnemyIdleState EnemyIdleState;    
    public EnemyChaseState EnemyChaseState;
    public EnemyHurtState EnemyHurtState;
    public EnemyDeadState EnemyDeadState;
    public EnemyAttackState EnemeyAttackState;

    public EnemyStateManager(EnemyController enemy)
    {
        this.enemy = enemy;

        EnemyIdleState = new EnemyIdleState(this);
        EnemyChaseState = new EnemyChaseState(this);
        EnemyHurtState = new EnemyHurtState(this);
        EnemyDeadState = new EnemyDeadState(this);
        EnemeyAttackState = new EnemyAttackState(this);
    }
    public void Initialize()
    {
        currentState = EnemyIdleState;
        currentState.EnterState(this);
    }
    public override void ChangeState(BaseState newState)
    {
        base.ChangeState(newState);
    }
    public override void Update()
    {
        base.Update();
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}

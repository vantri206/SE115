using UnityEngine;

public class PlayerHurtState : PlayerBaseState
{
    public PlayerHurtState(PlayerStateManager stateManager) : base(stateManager)
    {
        this.stateManager = stateManager;
        this.player = stateManager.player;
    }
    public override void EnterState(StateManager stateManager)
    {
        base.EnterState(stateManager);

        player.animator.SetTrigger("Hurt");

        player.movement.StopMoving();
        player.combat.FinishAttack();

        player.effect.PlayHurtEffect();
    }

    public override void ExitState(StateManager stateManager)
    {
        base.ExitState(stateManager);
    }
    public override void UpdateState()
    {
        if(!player.isHurting)
        {
            stateManager.ChangeState(stateManager.IdleState);
            return;
        }
        else if(player.isDead)
        {
            stateManager.ChangeState(stateManager.DeadState);
            return;
        }
    }
    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }
}

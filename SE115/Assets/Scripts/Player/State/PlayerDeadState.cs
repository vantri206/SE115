using UnityEngine;

public class PlayerDeadState : PlayerBaseState
{
    public PlayerDeadState(PlayerStateManager stateManager) : base(stateManager)
    {
        this.stateManager = stateManager;
        this.player = stateManager.player;
    }
    public override void EnterState(StateManager stateManager)
    {
        base.EnterState(stateManager);

        player.animator.SetTrigger("Dead");
    }

    public override void ExitState(StateManager stateManager)
    {
        base.ExitState(stateManager);
    }

    public override void FixedUpdateState()
    {

    }

    public override void UpdateState()
    {

    }
}

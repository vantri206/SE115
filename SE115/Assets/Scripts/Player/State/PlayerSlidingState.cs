using UnityEngine;

public class PlayerSlidingState : PlayerBaseState
{
    public PlayerSlidingState(PlayerStateManager stateManager) : base(stateManager)
    {
        this.stateManager = stateManager;
        this.player = stateManager.player;
    }
    public override void EnterState(StateManager stateManager)
    {
        base.EnterState(stateManager);

        player.movement.StopMoving();

        player.OnStartSliding();
    }
    public override void ExitState(StateManager stateManager)
    {
        base.ExitState(stateManager);

        player.OnEndSliding();
    }
    public override void UpdateState()
    {
        base.UpdateState();
        if (stateManager.currentState != this)
            return;

        if (player.lastPressedJumpTime > 0)
        {
            stateManager.ChangeState(stateManager.WallJumpState);
        }
        else if(player.CheckOnGround())
        {
            stateManager.ChangeState(stateManager.IdleState);
        }
        else if (!player.CheckWall())
        {
            stateManager.ChangeState(stateManager.FallingState);
        }
    }
    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }
}

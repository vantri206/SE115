using UnityEngine;

public class PlayerRunState : PlayerBaseState
{
    public PlayerRunState(PlayerStateManager stateManager) : base(stateManager)
    {
        this.stateManager = stateManager;
        this.player = stateManager.player;
    }
    public override void EnterState(StateManager stateManager)
    {
        base.EnterState(stateManager);

        player.animator.SetBool("isRunning", true);
    }
    public override void ExitState(StateManager stateManager)
    {
        base.ExitState(stateManager);

        player.animator.SetBool("isRunning", false);
    }
    public override void UpdateState()
    {
        base.UpdateState();

        if (player.input.isAttackPressed)
        {
            stateManager.ChangeState(stateManager.AttackState);
        }
        else if (player.lastPressedDashTime > 0 && player.CanDash())
        {
            stateManager.ChangeState(stateManager.DashState);
        }
        else if (player.lastPressedJumpTime > 0 && player.CanJump())
        {
            stateManager.ChangeState(stateManager.JumpUpState);
            return;
        }
        else if (player.IsFalling())
        {
            stateManager.ChangeState(stateManager.FallingState);
        }
        else if (player.input.moveInput.x == 0)
        {
            stateManager.ChangeState(stateManager.IdleState);
        }
        else if (player.input.moveInput.x != 0)
        {
            player.CheckFacingDirection(player.input.moveInput);
        }
    }
    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        player.movement.HorizonMoving(1.0f);
    }
}

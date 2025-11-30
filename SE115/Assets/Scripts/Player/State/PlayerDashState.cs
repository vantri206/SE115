using UnityEngine;

public class PlayerDashState : PlayerBaseState
{
    private Vector2 dashDirection;
    private float startTime;
    private bool inDash = false;
    public PlayerDashState(PlayerStateManager stateManager) : base(stateManager)
    {
        this.stateManager = stateManager;
        this.player = stateManager.player;
    }
    public override void EnterState(StateManager stateManager)
    {
        base.EnterState(stateManager);

        player.OnStartDash();
        player.movement.StopGravity();

        dashDirection = player.input.moveInput;
        if (dashDirection == Vector2.zero) dashDirection = player.facingDirection;

        inDash = true;

        startTime = Time.time;
    }

    public override void ExitState(StateManager stateManager)
    {
        base.ExitState(stateManager);

        player.FinishDash();
    }
    public override void UpdateState()
    {
        base.UpdateState();

        float timer = Time.time - startTime;

        if(timer <= player.data.dashTime)
        {
            player.movement.Dash();
        }
        else if(timer <= player.data.dashTime + player.data.dashEndTime && inDash)
        {
            inDash = false;
            player.movement.StopMoving();
        }
        else
        {
            stateManager.ChangeState(stateManager.IdleState);
        }
    }
    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }
}

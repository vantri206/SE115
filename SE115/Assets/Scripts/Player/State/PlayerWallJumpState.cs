using UnityEngine;

public class PlayerWallJumpState : PlayerBaseState
{
    public float wallJumpHorizonBlockTime = 0.2f;
    private float wallJumpHorizonBlockTimer = 0.0f;
    public PlayerWallJumpState(PlayerStateManager stateManager) : base(stateManager)
    {
        this.stateManager = stateManager;
        this.player = stateManager.player;
    }
    public override void EnterState(StateManager stateManager)
    {
        base.EnterState(stateManager);

        player.OnStartJump();
        player.movement.WallJump();

        wallJumpHorizonBlockTimer = 0.0f;
    }
    public override void ExitState(StateManager stateManager)
    {
        base.ExitState(stateManager);

        player.OnEndJump();
    }
    public override void UpdateState()
    {
        base.UpdateState();
        if (stateManager.currentState != this)
            return;

        wallJumpHorizonBlockTimer += Time.deltaTime;

        int skillIndex = player.input.CheckSkillPressed();
        if (skillIndex != -1)
        {
            if (player.skill.CanUseSkill(skillIndex))
            {
                player.skill.SetCurrentSkill(skillIndex);
                stateManager.ChangeState(stateManager.SkillState);
                return;
            }
        }

        if (player.lastPressedJumpTime > 0 && player.CanJump())
        {
            stateManager.ChangeState(stateManager.JumpUpState);
            return;
        }

        if (player.lastPressedDashTime > 0 && player.CanDash())
        {
            stateManager.ChangeState(stateManager.DashState);
        }
        else if (player.input.isAttackPressed && player.CanAttack())
        {
            stateManager.ChangeState(stateManager.AttackState);
        }
        else if (player.CheckOnGround())
        {
            stateManager.ChangeState(stateManager.IdleState);
        }
        else if (player.IsFalling())
        {
            stateManager.ChangeState(stateManager.FallingState);
            return;
        }
        else if (player.input.moveInput.x != 0)
        {
            player.CheckFacingDirection(player.input.moveInput);
        }
    }
    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        if(wallJumpHorizonBlockTimer >= wallJumpHorizonBlockTime)
            player.movement.HorizonMoving(1.0f);
    }
}

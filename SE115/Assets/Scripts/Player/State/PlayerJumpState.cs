using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateManager stateManager) : base(stateManager)
    {
        this.stateManager = stateManager;
        this.player = stateManager.player;
    }
    public override void EnterState(StateManager stateManager)
    {
        base.EnterState(stateManager);

        player.OnStartJump();
        player.movement.Jump();
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
            player.OnStartJump();
            player.movement.Jump();
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
        else if(player.CanWallSliding())
        {
            stateManager.ChangeState(stateManager.SlidingState);
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

        player.movement.HorizonMoving(1.0f);
    }
}

using UnityEngine;
using UnityEngine.Windows;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateManager stateManager) : base(stateManager)
    {
        this.stateManager = stateManager;
        this.player = stateManager.player;
    }

    public override void EnterState(StateManager stateManager)
    {
        base.EnterState(stateManager);

        player.movement.StopMovingHorzion();
    }
    public override void ExitState(StateManager stateManager)
    {
        base.ExitState(stateManager);
    }
    public override void UpdateState()
    {
        base.UpdateState();
        if (stateManager.currentState != this)
            return;

        int skillIndex = player.input.CheckSkillPressed();
        if (skillIndex != -1)
        {
            if(player.skill.CanUseSkill(skillIndex))
            {
                player.skill.SetCurrentSkill(skillIndex);
                stateManager.ChangeState(stateManager.SkillState);
                return;
            }
        }

        if (player.input.isAttackPressed && player.CanAttack())
        {
            stateManager.ChangeState(stateManager.AttackState);
        }
        else if(player.lastPressedDashTime > 0 && player.CanDash())
        {
            stateManager.ChangeState(stateManager.DashState);
        }
        else if (player.lastPressedJumpTime > 0 && player.CanJump())
        {
            stateManager.ChangeState(stateManager.JumpUpState);
        }
        else if (player.IsFalling())
        {
            stateManager.ChangeState(stateManager.FallingState);
        }
        else if (player.input.moveInput.x != 0)
        {
            stateManager.ChangeState(stateManager.RunState);
        }
    }
    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }
}

using UnityEngine;
using UnityEngine.Windows;

public class PlayerSkillState : PlayerBaseState
{
    public PlayerSkillState(PlayerStateManager stateManager) : base(stateManager)
    {
        this.stateManager = stateManager;
        this.player = stateManager.player;
    }

    public override void EnterState(StateManager stateManager)
    {
        base.EnterState(stateManager);

        player.input.ResetSkillPressed(player.skill.currentSkillIndex);

        player.movement.StopMoving();
        player.skill.StartSkill();
    }
    public override void ExitState(StateManager stateManager)
    {
        base.ExitState(stateManager);

        player.skill.FinishSkill();
    }
    public override void UpdateState()
    {
        base.UpdateState();

        if(player.skill.isAniSkillFinished)
        {
            stateManager.ChangeState(stateManager.IdleState);
            return;
        }
    }
    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }
}

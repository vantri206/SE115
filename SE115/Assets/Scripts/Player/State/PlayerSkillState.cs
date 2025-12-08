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

        player.input.ResetSkillPressed(player.combat.currentSkillIndex);

        player.movement.StopMoving();
        player.combat.StartSkill();
    }
    public override void ExitState(StateManager stateManager)
    {
        base.ExitState(stateManager);

        player.combat.FinishSkill();
    }
    public override void UpdateState()
    {
        base.UpdateState();

        if(player.combat.isAniSkillFinished)
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

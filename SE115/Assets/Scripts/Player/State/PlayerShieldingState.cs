using UnityEngine;

public class PlayerShieldingState : PlayerBaseState
{
    private float shieldDuration = 1.0f;
    private float shieldTimer = 0.0f;

    public PlayerShieldingState(PlayerStateManager stateManager) : base(stateManager)
    {
        this.stateManager = stateManager;
        this.player = stateManager.player;
    }

    public override void EnterState(StateManager stateManager)
    {
        base.EnterState(stateManager);

        shieldTimer = 0f;

        if (player.reflectShieldObj != null)
            player.reflectShieldObj.SetActive(true);

        player.movement.StopMovingHorzion();
    }

    public override void ExitState(StateManager stateManager)
    {
        base.ExitState(stateManager);

        if (player.reflectShieldObj != null)
            player.reflectShieldObj.SetActive(false);

        if (player.skill.currentSkill != null)
            player.skill.currentSkill.OnSkillEnd(player);

        player.isShielding = false;

        player.animator.SetBool("isShielding", false);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (stateManager.currentState != this) return;

        shieldTimer += Time.deltaTime;

        if (shieldTimer >= shieldDuration)
        {
            if (player.input.moveInput.x != 0)
                stateManager.ChangeState(stateManager.RunState);
            else
                stateManager.ChangeState(stateManager.IdleState);
            return;
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }
}
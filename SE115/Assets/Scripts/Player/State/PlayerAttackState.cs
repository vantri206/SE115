using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    public PlayerAttackState(PlayerStateManager stateManager) : base(stateManager)
    {
        this.stateManager = stateManager;
        this.player = stateManager.player;
    }
    public override void EnterState(StateManager stateManager)
    {
        base.EnterState(stateManager);

        player.input.ResetAttackPressed();

        player.animator.SetBool("isAttack", true);
        player.animator.SetInteger("comboCount", player.combat.comboCount);

        player.movement.StopMoving();
        player.combat.SwordAttack();
    }
    public override void ExitState(StateManager stateManager)
    {
        base.ExitState(stateManager);

        player.animator.SetBool("isAttack", false);

        player.combat.DisableSword();
    }
    public override void UpdateState()
    {
        base.UpdateState();
        if (stateManager.currentState != this)
            return;

        if (player.isAttacking == false)
            stateManager.ChangeState(stateManager.IdleState);
    }
    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }
}

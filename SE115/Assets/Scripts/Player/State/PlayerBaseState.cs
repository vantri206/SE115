using Unity.VisualScripting;
using UnityEngine;

public abstract class PlayerBaseState : BaseState
{
    protected PlayerStateManager stateManager;
    protected PlayerController player;

    public PlayerBaseState(PlayerStateManager stateManager)
    {
        this.stateManager = stateManager;
        this.player = stateManager.player;
    }
    public override void EnterState(StateManager stateManager)
    {

    }

    public override void ExitState(StateManager stateManager)
    {

    }
    public override void UpdateState()
    {
        if (player.isHurting)
        {
            stateManager.ChangeState(stateManager.HurtState);
            return;
        }
        else if(player.isDead)
        {
            stateManager.ChangeState(stateManager.DeadState);
            return;
        }
    }
    public override void FixedUpdateState()
    {

    }
}

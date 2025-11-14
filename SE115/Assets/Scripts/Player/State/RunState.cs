using UnityEngine;

public class RunState : BaseState
{
    public StateManager stateManager;
    public PlayerController player;
    public override void EnterState(StateManager stateManager)
    {
        stateManager.player.animator.SetBool("isRunning", true);

        player.movement.SetMultipleSpeed(1.0f);
    }
    public override void ExitState(StateManager stateManager)
    {
        stateManager.player.animator.SetBool("isRunning", false);
    }
    public override void OnCollisionEnter(StateManager stateManager) { }
    public override void UpdateState()
    {
        Vector2 moveInput = player.input.moveInput;
        if (moveInput.x != 0)
        {
            Vector2 direction = new Vector2(moveInput.x, 0);
            player.movement.SetDirection(direction);
            player.SetFacingDirection(direction);
        }
        else
        {
            stateManager.ChangeState(stateManager.IdleState);
        }
    }
    public override void FixedUpdateState() { }
}

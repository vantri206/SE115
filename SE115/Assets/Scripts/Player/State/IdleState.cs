using UnityEngine;
using UnityEngine.Windows;

public class IdleState : BaseState
{
    public StateManager stateManager;
    public PlayerController player;
    public override void EnterState(StateManager stateManager)
    {
        player.animator.SetBool("isRunning", false);

        player.movement.SetMultipleSpeed(0.0f);
    }
    public override void ExitState(StateManager stateManager) { }
    public override void OnCollisionEnter(StateManager stateManager) { }
    public override void UpdateState()
    {
        Vector2 moveInput = player.input.moveInput;
        if (moveInput.x != 0)
        {
            Vector2 direction = new Vector2(moveInput.x, 0);
            player.movement.SetDirection(direction);
            player.SetFacingDirection(direction);
            stateManager.ChangeState(stateManager.RunState);
        }
        else if (UnityEngine.Input.GetKeyDown(KeyCode.J))
        {
            stateManager.ChangeState(stateManager.AttackState);
        }
    }
    public override void FixedUpdateState() { }
}

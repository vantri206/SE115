using UnityEngine;

public class IdleState : BaseState
{
    public StateManager stateManager;
    public PlayerController player;
    public override void EnterState(StateManager stateManager)
    {
        this.stateManager = stateManager;
        this.player = stateManager.player;
        player.animator.SetBool("isRunning", false);
    }
    public override void ExitState(StateManager stateManager) { }
    public override void OnCollisionEnter(StateManager stateManager) { }
    public override void UpdateState()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            stateManager.ChangeState(stateManager.RunState);
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            stateManager.ChangeState(stateManager.AttackState);
        }
    }
    public override void FixedUpdateState() { }
}

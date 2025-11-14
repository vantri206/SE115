using UnityEngine;

public class RunState : BaseState
{
    public override void EnterState(StateManager stateManager)
    {
        stateManager.player.animator.SetBool("isRunning", true);
    }
    public override void ExitState(StateManager stateManager)
    {
        stateManager.player.animator.SetBool("isRunning", false);
    }
    public override void OnCollisionEnter(StateManager stateManager) { }
    public override void UpdateState() { }
    public override void FixedUpdateState() { }
}

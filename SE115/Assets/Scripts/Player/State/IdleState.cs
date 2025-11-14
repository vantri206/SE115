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
            //Debug.Log("true");
            stateManager.ChangeState(stateManager.RunState);
        }
    }
    public override void FixedUpdateState() { }
}

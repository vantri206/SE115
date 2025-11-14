using UnityEngine;

public class AttackState : BaseState
{
    public StateManager stateManager;
    public PlayerController player;
    float attackDuration = 0.25f;
    float attackTimer = 0f;
    public override void EnterState(StateManager stateManager)
    {
        this.stateManager = stateManager;
        this.player = stateManager.player;
        stateManager.player.animator.SetBool("isAttack", true);
        attackTimer = 0f;
        player.sword.gameObject.SetActive(true);
    }
    public override void ExitState(StateManager stateManager)
    {
        player.sword.gameObject.SetActive(false);
        player.animator.SetBool("isAttack", false);
    }
    public override void OnCollisionEnter(StateManager stateManager) { }
    public override void UpdateState()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer > attackDuration)
        {
            stateManager.ChangeState(stateManager.IdleState);
        }
    }
    public override void FixedUpdateState() { }
}

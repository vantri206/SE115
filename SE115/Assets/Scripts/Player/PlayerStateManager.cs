using JetBrains.Annotations;
using UnityEngine;

public class PlayerStateManager : StateManager
{
    public PlayerController player;

    public PlayerIdleState IdleState;
    public PlayerRunState RunState;
    public PlayerFallingState FallingState;
    public PlayerJumpState JumpUpState;
    public PlayerAttackState AttackState;
    public PlayerDashState DashState;
    public PlayerHurtState HurtState;
    public PlayerDeadState DeadState;
    public PlayerSkillState SkillState;
    public PlayerSlidingState SlidingState;
    public PlayerWallJumpState WallJumpState;

    public PlayerStateManager(PlayerController player)
    {
        this.player = player;

        IdleState = new PlayerIdleState(this);
        RunState = new PlayerRunState(this);
        FallingState = new PlayerFallingState(this);
        JumpUpState = new PlayerJumpState(this);
        AttackState = new PlayerAttackState(this);
        DashState = new PlayerDashState(this);
        HurtState = new PlayerHurtState(this);
        DeadState = new PlayerDeadState(this);
        SkillState = new PlayerSkillState(this);
        SlidingState = new PlayerSlidingState(this);
        WallJumpState = new PlayerWallJumpState(this);
    }
    public void Initialize()
    {
        currentState = IdleState;
        currentState.EnterState(this);
    }
    public override void ChangeState(BaseState newState)
    {
        base.ChangeState(newState);
    }
    public override void Update()
    {
        base.Update();

        Debug.Log(currentState.GetType().ToString());
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}

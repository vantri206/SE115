using UnityEngine;

public class StateManager : MonoBehaviour
{
    BaseState currentState;
    IdleState IdleState = new IdleState();
    RunState RunState = new RunState();
    JumpDownState JumpDownState = new JumpDownState();
    JumpUpState JumpUpState = new JumpUpState();
    AttackState AttackState = new AttackState();
    DashState DashState = new DashState();
    HurtState HurtState = new HurtState();
    DeadState DeadState = new DeadState();

    void Start()
    {
        currentState = IdleState;
        currentState.EnterState(this);
    }
}

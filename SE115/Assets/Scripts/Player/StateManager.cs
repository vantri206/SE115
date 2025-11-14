using UnityEngine;

public class StateManager : MonoBehaviour
{
    public PlayerController player;

    BaseState currentState;

    public IdleState IdleState = new IdleState();
    public RunState RunState = new RunState();
    public JumpDownState JumpDownState = new JumpDownState();
    public JumpUpState JumpUpState = new JumpUpState();
    public AttackState AttackState = new AttackState();
    public DashState DashState = new DashState();
    public HurtState HurtState = new HurtState();
    public DeadState DeadState = new DeadState();

    void Awake()
    {
        player = GetComponent<PlayerController>();

        IdleState.stateManager = this;
        IdleState.player = this.player;
        RunState.stateManager = this;
        RunState.player = this.player;
    }
    void Start()
    {
        currentState = IdleState;
        currentState.EnterState(this);
    }
    public void ChangeState(BaseState newState)
    {
        if (newState == null) return;
        if(currentState != newState)
        {
            currentState.ExitState(this);
        }
        currentState = newState;
        currentState.EnterState(this);
    }
    public void Update()
    {
        currentState.UpdateState();
    }
    public void FixedUpdate()
    {
        currentState.FixedUpdateState();
    }
}

using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseState
{
    public abstract void EnterState(StateManager stateManager);
    public abstract void ExitState(StateManager stateManager);
    public abstract void UpdateState();
    public abstract void FixedUpdateState();
}

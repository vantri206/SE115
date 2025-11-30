using UnityEngine;

[System.Serializable]
public class StateManager
{
    public BaseState currentState;
    public virtual void ChangeState(BaseState newState)
    {
        if (newState == null) return;
        if(currentState != newState)
        {
            if(currentState != null)
                currentState.ExitState(this);
        }
        currentState = newState;
        currentState.EnterState(this);
    }
    public virtual void Update()
    {
        if (currentState != null)
            currentState.UpdateState();
    }
    public virtual void FixedUpdate()
    {
        if (currentState != null)
            currentState.FixedUpdateState();
    }
}

using UnityEngine;

public class EnemyDeadSOBase : ScriptableObject
{
    protected EnemyController enemy;
    protected Transform transform;
    protected GameObject gameObject;
    protected EnemyStateManager stateManager;

    public virtual void Initalize(GameObject gameObject, EnemyController enemy)
    {
        this.gameObject = gameObject;
        transform = gameObject.transform;
        this.enemy = enemy;
        this.stateManager = enemy.stateManager; 
    }

    public virtual void HandleEnterState() { }
    public virtual void HandleExitState() { }
    public virtual void HandleUpdateState()
    {

    }
    public virtual void HandleFixedUpdateState() { }

    public virtual void ResetValue() { }
}

using UnityEngine;
public class EnemyChaseSOBase : ScriptableObject
{
    protected EnemyController enemy;
    protected Transform transform;
    protected GameObject gameObject;
    protected EnemyStateManager stateManager;

    [SerializeField] protected bool canInterruptByHit = true;
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
        if (enemy.health.isDead)
        {
            stateManager.ChangeState(stateManager.EnemyDeadState);
        }
        else if (enemy.isHurtStun && canInterruptByHit)
        {
            stateManager.ChangeState(stateManager.EnemyHurtState);
        }
    }
    public virtual void HandleFixedUpdateState() { }

    public virtual void ResetValue() { }
}

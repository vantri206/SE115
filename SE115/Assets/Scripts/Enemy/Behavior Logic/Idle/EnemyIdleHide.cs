using UnityEngine;

[CreateAssetMenu(fileName = "Idle-Hide", menuName = "Enemy Logic Behavior/Idle Logic/IdleHide")]
public class EnemyIdleHide : EnemyIdleSOBase
{
    private static readonly int HideAnimatorHash = Animator.StringToHash("isHide");
    [SerializeField] private float wakeUpRange = 8.0f;
    public override void HandleEnterState()
    {
        base.HandleEnterState();

        enemy.animator.SetBool(HideAnimatorHash, true);

        enemy.movement.StopMove();
    }

    public override void HandleExitState()
    {
        base.HandleExitState();

        enemy.animator.SetBool(HideAnimatorHash, false);

        enemy.movement.StopMove();
    }

    public override void HandleFixedUpdateState()
    {
        base.HandleFixedUpdateState();
    }

    public override void HandleUpdateState()
    {
        base.HandleUpdateState();

        if (stateManager.currentState != stateManager.EnemyIdleState)
            return;

        if (enemy.attackTarget != null)
        {
            float distance = Mathf.Abs(enemy.transform.position.x - enemy.attackTarget.position.x);
            if (distance <= wakeUpRange) 
            {
                if (enemy.CanAttack())
                    stateManager.ChangeState(stateManager.EnemeyAttackState);
            }
        }
        else if (enemy.isAggroed)
        {
            float distance = Mathf.Abs(enemy.transform.position.x - enemy.playerTarget.position.x);
            if (distance <= wakeUpRange)
            {
                    stateManager.ChangeState(stateManager.EnemyChaseState);
            }
        }
    }

    public override void Initalize(GameObject gameObject, EnemyController enemy)
    {
        base.Initalize(gameObject, enemy);
    }

    public override void ResetValue()
    {
        base.ResetValue();
    }
}

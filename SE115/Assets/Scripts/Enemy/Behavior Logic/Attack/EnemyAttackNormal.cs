using UnityEngine;

[CreateAssetMenu(fileName = "Attack-Normal", menuName = "Enemy Logic Behavior/Attack Logic/AttackNormal")]
public class EnemyAttackNormal : EnemyAttackSOBase
{
    [SerializeField] private bool attackFacingPlayer = true;
    public override void HandleEnterState()
    {
        base.HandleEnterState();

        enemy.movement.StopMove();

        if (enemy.attackTarget != null && attackFacingPlayer)
        {
            Vector2 dir = enemy.attackTarget.position.x - transform.position.x > 0 ? Vector2.right : Vector2.left;
            enemy.CheckFacingDirection(dir);
        }

        enemy.animator.SetTrigger("Attack");

        enemy.isAttacking = true;

        enemy.attackCooldownTimer = float.NegativeInfinity;
    }

    public override void HandleExitState()
    {
        base.HandleExitState();

        enemy.attackTarget = null;
        enemy.attackCooldownTimer = 0;

        enemy.AE_FinishAttack();
    }

    public override void HandleFixedUpdateState()
    {
        base.HandleFixedUpdateState();
    }

    public override void HandleUpdateState()
    {
        base.HandleUpdateState();

        if (stateManager.currentState != stateManager.EnemeyAttackState)
            return;

        if(!enemy.isAttacking)
        {
            stateManager.ChangeState(stateManager.EnemyChaseState);
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

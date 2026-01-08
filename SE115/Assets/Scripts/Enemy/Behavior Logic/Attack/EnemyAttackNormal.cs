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
            float playerDistance = enemy.playerTarget.position.x - enemy.transform.position.x;

            if (Mathf.Abs(playerDistance) > 0.1f)
            {
                float playerDirection = Mathf.Sign(playerDistance);
                if (playerDirection != enemy.facingDirection.x)
                {
                    enemy.CheckFacingDirection(new Vector2(playerDirection, enemy.facingDirection.y));
                }
            }
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

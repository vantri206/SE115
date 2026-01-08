using UnityEngine;

[CreateAssetMenu(fileName = "Attack-Bateye", menuName = "Enemy Logic Behavior/Attack Logic/BateyeAttack")]
public class BateyeAttack : EnemyAttackSOBase
{
    [Header("Attack Configuration")]
    [SerializeField] private float dashSpeed = 15.0f;
    [SerializeField] private LayerMask platformerLayer;
    [SerializeField] private Vector2 aimOffset = new Vector2(-3f, 0f);
    public override void Initalize(GameObject gameObject, EnemyController enemy)
    {
        base.Initalize(gameObject, enemy);
    }

    public override void HandleEnterState()
    {
        base.HandleEnterState();

        enemy.movement.StopMove();

        if (enemy is BateyeController bateye)
        {
            bateye.SetupAttack(dashSpeed, aimOffset, platformerLayer, enemy.attackTarget);
        }

        enemy.animator.SetTrigger("StartAttack");

        enemy.isAttacking = true;

        enemy.attackCooldownTimer = float.NegativeInfinity;
    }

    public override void HandleUpdateState()
    {
        base.HandleUpdateState();

        if (stateManager.currentState != stateManager.EnemeyAttackState)
            return;

        if (!enemy.isAttacking)
        {
            stateManager.ChangeState(stateManager.EnemyIdleState);
        }
    }
    public override void HandleExitState()
    {
        base.HandleExitState();

        enemy.movement.StopMove();

        enemy.attackTarget = null;
        enemy.attackCooldownTimer = 0;
    }
}
using UnityEngine;

[CreateAssetMenu(fileName = "Chase-Stay", menuName = "Enemy Logic Behavior/Chase Logic/Stay")]
public class EnemyChaseStay : EnemyChaseSOBase
{
    public override void HandleEnterState()
    {
        base.HandleEnterState();

        enemy.movement.StopMove();
    }

    public override void HandleExitState()
    {
        base.HandleExitState();

        enemy.movement.StopMove();
    }

    public override void HandleFixedUpdateState()
    {
        base.HandleFixedUpdateState();
    }

    public override void HandleUpdateState()
    {
        base.HandleUpdateState();

        if (stateManager.currentState != stateManager.EnemyChaseState)
            return;

        if (enemy.playerTarget == null)
        {
            stateManager.ChangeState(stateManager.EnemyIdleState);
            return;
        }

        if (enemy.CanAttack())
        {
            stateManager.ChangeState(stateManager.EnemeyAttackState);
            return;
        }
        else if (enemy.isAggroed == false)
        {
            stateManager.ChangeState(stateManager.EnemyIdleState);
            return;
        }

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

    public override void Initalize(GameObject gameObject, EnemyController enemy)
    {
        base.Initalize(gameObject, enemy);
    }

    public override void ResetValue()
    {
        base.ResetValue();
    }
}

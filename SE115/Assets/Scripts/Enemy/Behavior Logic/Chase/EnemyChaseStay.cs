using UnityEngine;

[CreateAssetMenu(fileName = "Chase-Stay", menuName = "Enemy Logic Behavior/Chase Logic/Stay")]
public class EnemyChaseStay : EnemyChaseSOBase
{
    public override void HandleEnterState()
    {
        base.HandleEnterState();
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

        if (enemy.CanAttack())
        {
            stateManager.ChangeState(stateManager.EnemeyAttackState);
            return;
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

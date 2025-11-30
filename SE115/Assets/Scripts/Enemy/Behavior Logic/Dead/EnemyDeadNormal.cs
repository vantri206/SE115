using UnityEngine;

[CreateAssetMenu(fileName = "Dead-Normal", menuName = "Enemy Logic Behavior/Dead Logic/DeadNormal")]
public class EnemyDeadNormal : EnemyDeadSOBase
{
    public override void HandleEnterState()
    {
        base.HandleEnterState();
    }

    public override void HandleExitState()
    {
        base.HandleExitState();
    }

    public override void HandleFixedUpdateState()
    {
        base.HandleFixedUpdateState();
    }

    public override void HandleUpdateState()
    {
        base.HandleUpdateState();
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

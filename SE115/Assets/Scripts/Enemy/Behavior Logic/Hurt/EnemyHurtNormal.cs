using UnityEngine;

[CreateAssetMenu(fileName = "Hurt-Normal", menuName = "Enemy Logic Behavior/Hurt Logic/HurtNormal")]
public class EnemyHurtNormal : EnemyHurtSOBase
{
    public float knockbackForce = 5.0f;
    public Vector2 knockbackDir = new Vector2(0.25f, 0.75f);
    public override void HandleEnterState()
    {
        base.HandleEnterState();

        enemy.animator.SetTrigger("Hurt");
        enemy.movement.StopMove();
        enemy.FinishAttack();

        ApplyKnockback();
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

    private void ApplyKnockback()
    {
        Vector2 sourcePos = enemy.takenDamageSourcePos;
        int dirX = (enemy.transform.position.x < sourcePos.x) ? -1 : 1;
        enemy.myRigidbody.linearVelocity = Vector2.zero;

        Vector2 direction = new Vector2(knockbackDir.x * dirX, knockbackDir.y).normalized;
        Vector2 force = direction *  new Vector2(knockbackForce, knockbackForce * 2);

        enemy.myRigidbody.AddForce(force, ForceMode2D.Impulse);
    }
}

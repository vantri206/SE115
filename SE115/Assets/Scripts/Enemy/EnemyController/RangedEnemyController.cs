using UnityEngine;

public class RangedEnemyController : EnemyController
{
    private int currentWeaponIndex = 0;

    public override void Attack()
    {
        if (weapons == null || weapons.Length == 0) return;

        weapons[currentWeaponIndex].PerformAttack();

        currentWeaponIndex++;
        if (currentWeaponIndex >= weapons.Length)
        {
            currentWeaponIndex = 0;
        }
    }
    public override void FinishAttack()
    {
        if (weapons == null || weapons.Length == 0) return;

        isAttacking = false;

        weapons[currentWeaponIndex].FinishAttack();
    }

    public override void CheckAggroRange()
    {
        Vector2 centerPoint = myCollider.bounds.center;

        RaycastHit2D hit = Physics2D.BoxCast(
            centerPoint,
            new Vector2(aggroTriggerRange, detectionHeightRange),
            0,
            facingDirection,
            0,
            playerLayer
        );

        if (hit.collider != null)
        {
            playerTarget = hit.collider.transform;
            this.isAggroed = true;
        }
        else
        {
            playerTarget = null;
            this.isAggroed = false;
        }
    }
    public override void OnDrawGizmos()
    {
        if (myCollider == null) return;

        Bounds bounds = myCollider.bounds;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(bounds.center, new Vector3(aggroTriggerRange, detectionHeightRange, 0.0f));

        Gizmos.color = Color.darkCyan;
        Gizmos.DrawWireCube(bounds.center, new Vector3(attackRange, attackHeightRange, 0.0f));
    }
}
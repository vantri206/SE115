using UnityEngine;

[CreateAssetMenu(fileName = "Chase-Burrow", menuName = "Enemy Logic Behavior/Chase Logic/ChaseBurrow")]
public class EnemyChaseBurrow : EnemyChaseSOBase
{
    [Header("Burrow Settings")]
    [SerializeField] private float moveSpeed = 8.0f;
    [SerializeField] private float spawnFireInterval = 0.2f;
    [SerializeField] private float emergeDistance = 3.0f;

    private EnemyFireTrailWeapon fireWeapon;

    private float fireTimer;
    private bool isUnderground = false;
    private bool isEmerging = false;

    public override void HandleEnterState()
    {
        base.HandleEnterState();
        isUnderground = false;
        isEmerging = false;
        fireTimer = 0;

        fireWeapon = enemy.GetComponent<EnemyFireTrailWeapon>();

        if (fireWeapon == null)
        {
            Debug.LogWarning("Boss chưa gắn script EnemyFireTrailWeapon!");
        }

        enemy.animator.SetTrigger("Hide");
        enemy.movement.StopMove();
        enemy.health.SetInvincible(true);
    }

    public override void HandleUpdateState()
    {
        base.HandleUpdateState();

        if (isEmerging) return;

        if (isUnderground)
        {
            MoveAndBurn();
        }
    }

    private void MoveAndBurn()
    {
        if (enemy.playerTarget == null) return;

        enemy.movement.RunToTarget(enemy.playerTarget.position, moveSpeed);

        fireTimer += Time.deltaTime;
        if (fireTimer >= spawnFireInterval)
        {
            SpawnFire();
            fireTimer = 0;
        }

        float distance = Vector2.Distance(enemy.transform.position, enemy.playerTarget.position);
        if (distance <= emergeDistance)
        {
            StartEmerge();
        }
    }

    private void SpawnFire()
    {
        if (fireWeapon != null)
        {
            fireWeapon.PerformAttack();
        }
    }

    private void StartEmerge()
    {
        isEmerging = true;
        enemy.movement.StopMove();


        enemy.animator.SetTrigger("Emerge");
    }

    public void AE_FinishHide()
    {
        isUnderground = true;
    }

    public void AE_FinishEmerge()
    {
        enemy.health.SetInvincible(false); 

        BossManager bossManager = enemy.GetComponent<BossManager>();

        if (bossManager != null)
        {
            bossManager.DecideAttack();
        }
        else
        {
            enemy.stateManager.ChangeState(enemy.stateManager.EnemeyAttackState);
        }
    }
}
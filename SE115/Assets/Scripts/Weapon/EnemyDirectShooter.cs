using UnityEngine;
using System.Collections; 

public class EnemyDirectShooter : EnemyWeapon
{
    [Header("General Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform FirePoint;

    [Header("Burst Fire Settings")]
    [SerializeField] private int burstCount = 1;

    [SerializeField] private float timeBetweenShots = 0.2f;

    [Header("Spread Settings")]
    [SerializeField] private float spreadAngle = 0f;

    private EnemyController enemyController;
    private Coroutine firingCoroutine;

    private void Awake()
    {
        enemyController = GetComponentInParent<EnemyController>();
    }

    public void SetBurstParameters(int newCount, float newTimeBetween, float newSpread)
    {
        burstCount = newCount;
        timeBetweenShots = newTimeBetween;
        spreadAngle = newSpread;
    }

    public override void PerformAttack()
    {
        if (projectilePrefab == null || FirePoint == null || enemyController.playerTarget == null)
            return;

        if (firingCoroutine != null) StopCoroutine(firingCoroutine);

        firingCoroutine = StartCoroutine(BurstFireRoutine());
    }

    private IEnumerator BurstFireRoutine()
    {
        for (int i = 0; i < burstCount; i++)
        {
            if (enemyController.playerTarget == null) yield break;

            ShootSingleBullet();

            if (i < burstCount - 1 && timeBetweenShots > 0)
            {
                yield return new WaitForSeconds(timeBetweenShots);
            }
        }
    }

    private void ShootSingleBullet()
    {
        Vector2 directionToPlayer = (enemyController.playerTarget.position - FirePoint.position).normalized;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;


        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        Instantiate(projectilePrefab, FirePoint.position, rotation);
    }
}
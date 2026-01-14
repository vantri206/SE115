using UnityEngine;

public class SatyrProjectileShooter : EnemyWeapon
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform shootPoint;

    public override void PerformAttack() { }
    public override void FinishAttack() { }

    public void ShootStraight(float directionX)
    {
        if (projectilePrefab && shootPoint)
        {
            GameObject obj = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

            SatyrProjectile projectile = obj.GetComponent<SatyrProjectile>();
            if (projectile != null)
            {
                Vector2 dir = new Vector2(directionX, 0);
                projectile.Launch(dir);
            }
        }
    }

    public void ShootAtTarget(Vector2 targetPos)
    {
        if (projectilePrefab && shootPoint)
        {
            GameObject obj = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

            SatyrProjectile projectile = obj.GetComponent<SatyrProjectile>();
            if (projectile != null)
            {
                Vector2 dir = (targetPos - (Vector2)shootPoint.position).normalized;
                projectile.Launch(dir);
            }
        }
    }
}
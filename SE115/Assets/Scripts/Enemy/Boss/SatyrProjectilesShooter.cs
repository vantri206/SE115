using UnityEngine;

public class SatyrProjectileShooter : EnemyWeapon
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform shootPoint;     
    public float projectileSpeed = 15f;

    public override void PerformAttack() { }
    public override void FinishAttack() { }

    public void ShootStraight(float directionX)
    {
        if (projectilePrefab && shootPoint)
        {
            GameObject b = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
            var rb = b.GetComponent<Rigidbody2D>();
            if (rb) rb.linearVelocity = new Vector2(directionX * projectileSpeed, 0);

            if (directionX < 0)
                b.transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    public void ShootAtTarget(Vector2 targetPos)
    {
        if (projectilePrefab && shootPoint)
        {
            GameObject b = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

            Vector2 dir = (targetPos - (Vector2)shootPoint.position).normalized;

            var rb = b.GetComponent<Rigidbody2D>();
            if (rb) rb.linearVelocity = dir * projectileSpeed;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            b.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}
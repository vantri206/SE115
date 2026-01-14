using UnityEngine;

public class EnemyFireTrailWeapon : EnemyWeapon
{
    [Header("Fire Trail Setup")]
    [SerializeField] private GameObject fireTrailPrefab;
    [SerializeField] private Transform firePoint; 

    [Header("Visual Settings")]
    [SerializeField] private Vector3 fireScale = new Vector3(2f, 2f, 1f); 

    public override void PerformAttack()
    {
        if (fireTrailPrefab == null) return;

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;

        GameObject fireObj = Instantiate(fireTrailPrefab, spawnPos, Quaternion.identity);

        fireObj.transform.localScale = fireScale;
    }
}
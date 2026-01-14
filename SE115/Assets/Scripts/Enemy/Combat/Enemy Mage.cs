using UnityEngine;

public class EnemyMageWeapon : EnemyWeapon
{
    [Header("References")]
    public EnemyController enemy;

    [Header("Magic Orb Settings")]
    [SerializeField] private GameObject magicOrbPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform projectilesPool;

    [Header("Spawn Settings")]
    [SerializeField] private int orbsPerCast = 1;
    [SerializeField] private float orbSpacing = 0.3f; 
    [SerializeField] private float spawnDelay = 0.1f; 


    private void Awake()
    {
        if (enemy == null)
            enemy = transform.parent.GetComponent<EnemyController>();

        if (spawnPoint == null)
            spawnPoint = transform;
    }

    public override void PerformAttack()
    {
        if (orbsPerCast == 1)
        {
            SpawnSingleOrb();
        }
        else
        {
            StartCoroutine(SpawnMultipleOrbs());
        }
    }

    private void SpawnSingleOrb()
    {
        if (magicOrbPrefab == null)
        {
            Debug.LogWarning("Magic Orb Prefab is not assigned!");
            return;
        }

        Vector3 spawnPos = spawnPoint.position;
        Quaternion spawnRot = Quaternion.identity;

        GameObject orb = Instantiate(
            magicOrbPrefab,
            spawnPos,
            spawnRot,
            projectilesPool
        );

        orb.layer = gameObject.layer;
    }

    private System.Collections.IEnumerator SpawnMultipleOrbs()
    {
        for (int i = 0; i < orbsPerCast; i++)
        {
            float offsetY = (i - (orbsPerCast - 1) / 2f) * orbSpacing;
            Vector3 spawnPos = spawnPoint.position + new Vector3(0, offsetY, 0);

            GameObject orb = Instantiate(
                magicOrbPrefab,
                spawnPos,
                Quaternion.identity,
                projectilesPool
            );

            orb.layer = gameObject.layer;

            if (i < orbsPerCast - 1)
            {
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }

    public override void FinishAttack()
    {
        base.FinishAttack();
    }
}
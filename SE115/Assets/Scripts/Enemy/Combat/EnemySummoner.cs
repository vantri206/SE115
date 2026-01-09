using System.Collections.Generic;
using UnityEngine;

public class EnemySummonWeapon : EnemyWeapon
{
    [Header("Summon Settings")]
    [SerializeField] private GameObject minionPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private int maxMinionsAlive = 2;

    [Header("VFX Settings")]
    [SerializeField] private GameObject smokeEffectPrefab;

    private List<GameObject> activeMinions = new List<GameObject>();

    private GameObject newMinion;
    public override void PerformAttack()
    {
        SpawnMinion();
    }

    public override void FinishAttack()
    {
        ActiveNewMinion();
        base.FinishAttack();
    }

    public override bool CanUseWeapon()
    {
        activeMinions.RemoveAll(minion => minion == null);

        if (activeMinions.Count >= maxMinionsAlive)
        {
            return false;
        }

        return true;
    }

    private void SpawnMinion()
    {
        if (spawnPoints.Length > 0 && minionPrefab != null)
        {
            Transform selectedPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            if (smokeEffectPrefab != null)
            {
                Instantiate(smokeEffectPrefab, selectedPoint.position, Quaternion.identity);
            }

            newMinion = Instantiate(minionPrefab, selectedPoint.position, Quaternion.identity);

            EnemyController minionController = newMinion.GetComponent<EnemyController>();
            if (minionController != null)
            {
                minionController.ResetEnemyState();
            }
        }
    }

    private void ActiveNewMinion()
    {
        if (newMinion != null)
        {
            EnemyController minionController = newMinion.GetComponent<EnemyController>();

            if (minionController != null)
            {
                minionController.ActivateEnemyAI();

                EnemyController master = GetComponentInParent<EnemyController>();
                if (master != null)
                {
                    if (master.playerTarget != null)
                    {
                        minionController.playerTarget = master.playerTarget;
                        minionController.isAggroed = true;
                    }
                    minionController.attackTarget = master.attackTarget;
                }
            }

            activeMinions.Add(newMinion);
            newMinion = null;
        }
    }
}
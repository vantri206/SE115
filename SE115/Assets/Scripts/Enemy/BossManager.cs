using UnityEngine;

public class BossManager : MonoBehaviour
{
    private EnemyController enemy;

    [Header("Phase Settings")]
    [SerializeField] private float phase2Threshold = 0.5f;

    [Header("Phase 2 Weapon Configurations")]
    [SerializeField] private int p2BurstCount = 3;

    [SerializeField] private float p2TimeBetweenShots = 0.15f;

    [SerializeField] private float p2SpreadAngle = 15f;

    [Header("Attack Logics")]
    [SerializeField] private EnemyAttackSOBase normalAttackSO; 
    [SerializeField] private EnemyAttackSOBase summonAttackSO; 

    private EnemyAttackSOBase normalAttackInstance;
    private EnemyAttackSOBase summonAttackInstance;

    private bool hasSwitchedToPhase2 = false;

    void Start()
    {
        enemy = GetComponent<EnemyController>();

        if (enemy.health != null)
        {
            enemy.health.onHealthChanged += CheckPhase;
        }

        normalAttackInstance = Instantiate(normalAttackSO);
        normalAttackInstance.Initalize(gameObject, enemy);

        summonAttackInstance = Instantiate(summonAttackSO);
        summonAttackInstance.Initalize(gameObject, enemy);

        enemy.enemyAttackBaseInstance = normalAttackInstance;
    }

    void CheckPhase()
    {
        if (hasSwitchedToPhase2) return;

        float hpPercent = (float)enemy.health.currentHealth / enemy.health.maxHealth;


        if (hpPercent <= phase2Threshold)
        {
            EnterPhase2();
        }
    }

    void EnterPhase2()
    {
        hasSwitchedToPhase2 = true;
        enemy.isPhase2 = true;
        Debug.Log("BOSS ENTER PHASE 2!");

        EnemyDirectShooter shooter = GetComponentInChildren<EnemyDirectShooter>();
        if (shooter != null)
        {
            shooter.SetBurstParameters(p2BurstCount, p2TimeBetweenShots, p2SpreadAngle);
        }
    }

    public void DecideAttack()
    {
        if (enemy.isPhase2)
        {
            if (Random.value < 0.4f)
            {
                enemy.enemyAttackBaseInstance = summonAttackInstance;
            }
            else
            {
                enemy.enemyAttackBaseInstance = normalAttackInstance;
            }
        }
        else
        {
            enemy.enemyAttackBaseInstance = normalAttackInstance;
        }
        enemy.stateManager.ChangeState(enemy.stateManager.EnemeyAttackState);
    }

    private void OnDestroy()
    {
        if (enemy != null && enemy.health != null)
            enemy.health.onHealthChanged -= CheckPhase;
    }
}
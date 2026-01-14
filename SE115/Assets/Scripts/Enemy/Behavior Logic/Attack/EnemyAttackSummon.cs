using UnityEngine;

[CreateAssetMenu(fileName = "Attack-Summon", menuName = "Enemy Logic Behavior/Attack Logic/AttackSummon")]
public class EnemyAttackSummon : EnemyAttackSOBase
{
    [SerializeField] private GameObject minionPrefab;
    [SerializeField] private int minionCount = 2;
    [SerializeField] private float summonRadius = 3.0f;

    public override void HandleEnterState()
    {
        base.HandleEnterState();
        enemy.movement.StopMove();
        enemy.animator.SetTrigger("Summon"); 
        enemy.isAttacking = true;
    }

    public override void HandleUpdateState()
    {
        base.HandleUpdateState();
        if (!enemy.isAttacking)
        {
            enemy.stateManager.ChangeState(enemy.stateManager.EnemyChaseState);
        }
    }

    public void SummonMinions()
    {
        for (int i = 0; i < minionCount; i++)
        {
            Vector2 randomPos = (Vector2)enemy.transform.position + Random.insideUnitCircle * summonRadius;
            Instantiate(minionPrefab, randomPos, Quaternion.identity);
        }
    }

}
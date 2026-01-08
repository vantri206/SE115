using UnityEngine;
using UnityEngine.Rendering.Universal;
public class EnemyProjectilesShooter : EnemyWeapon
{
    public EnemyController enemy;
    [SerializeField] private GameObject projectilesPrefab;
    private Transform currentProjectiles;
    private void Awake()
    {
        if (enemy == null)
            enemy = transform.parent.GetComponent<EnemyController>();
    }
    public override void PerformAttack()
    {
        GameObject projectiles = Instantiate(projectilesPrefab, this.transform.position, this.transform.rotation, ProjectilesPool.Instance);
        if (projectiles != null)
        {
            currentProjectiles = projectiles.transform;
            if (currentProjectiles.TryGetComponent<LinearProjectiles>(out LinearProjectiles linearProjectiles))
            {
                linearProjectiles.SetDirection(enemy.facingDirection);
            }
            else if (currentProjectiles.TryGetComponent<MagicOrbProjectiles>(out MagicOrbProjectiles magicProjectiles))
            {
                magicProjectiles.Initialize((Vector2)this.transform.position, enemy.attackTarget);
            }
        }
    }
    public override void FinishAttack()
    {
        if (currentProjectiles == null) return;
        if (currentProjectiles.TryGetComponent<MagicOrbProjectiles>(out MagicOrbProjectiles magicProjectiles))
        {
            magicProjectiles.Fire(enemy.facingDirection);
        }
    }
}

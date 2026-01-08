using UnityEngine;

public class EnemyBoomShooter : EnemyWeapon
{
    public EnemyController enemy;
    [SerializeField] private GameObject boomPrefab;
    [SerializeField] private Vector2 throwForce;
    private void Awake()
    {
        if (enemy == null)
            enemy = transform.parent.GetComponent<EnemyController>();
    }
    public override void PerformAttack()
    {
        GameObject boom = Instantiate(boomPrefab, this.transform.position, this.transform.rotation, ProjectilesPool.Instance);
        if(boom.GetComponent<Boom>() != null)
            boom.GetComponent<Boom>().Throw(throwForce * enemy.facingDirection);
    }
}

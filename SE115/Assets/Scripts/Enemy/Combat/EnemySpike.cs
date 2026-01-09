using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class EnemySpike : EnemyWeapon
{
    public OverTimeDamage damage;
    public Hitbox hitbox;
    private void Awake()
    {
        hitbox.gameObject.SetActive(true);
    }
    public override void PerformAttack()
    {
        damage.ResetHitList();
    }
}

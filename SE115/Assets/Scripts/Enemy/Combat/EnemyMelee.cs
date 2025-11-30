using UnityEngine;

public class EnemyMelee : EnemyWeapon
{
    public SwordDamage damage;
    public Hitbox hitbox;
    private void Awake()
    {
        hitbox.gameObject.SetActive(false);
    }
    public override void PerformAttack()
    {
        damage.ResetHitList();
        hitbox.gameObject.SetActive(true);
    }
    public override void FinishAttack()
    {
        hitbox.gameObject.SetActive(false);
    }
}

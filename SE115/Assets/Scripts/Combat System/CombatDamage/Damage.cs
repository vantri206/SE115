using Unity.VisualScripting;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public DamageTeam attackerTeam = DamageTeam.Neutral;
    [SerializeField] protected bool isDestroyAfterDamage = false;
    [SerializeField] protected float damage = 0;
    public void SetDamageAmount(float amount)
    {
        this.damage = amount;
    }
    public virtual void DealDamage(IDamageable targetHealth)
    {
        if (targetHealth == null) return;

        DamageTeam targetTeam = targetHealth.team;

        if (CombatManager.CanDamage(attackerTeam, targetTeam))
        {
            Transform ownerTransform = this.transform.parent != null ? this.transform.parent : this.transform;
            Vector2 sourcePos = ownerTransform.position;
            targetHealth.TakeDamage(damage, sourcePos);
        }

        if (isDestroyAfterDamage)
            Destroy(gameObject);
    }
}

using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SwordDamage : Damage
{
    private List<IDamageable> hitList = new List<IDamageable>();
    public void ResetHitList()
    {
        hitList.Clear();
    }
    public override void DealDamage(IDamageable targetHealth)
    {
        if (targetHealth == null) return;

        DamageTeam targetTeam = targetHealth.team;

        if (CombatManager.CanDamage(attackerTeam, targetTeam) && !hitList.Contains(targetHealth))
        {
            Transform ownerTransform = this.transform.parent != null ? this.transform.parent : this.transform;
            Vector2 sourcePos = ownerTransform.position;
            targetHealth.TakeDamage(damage, sourcePos);
            hitList.Add(targetHealth);

            if (isDestroyAfterDamage)
                Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        ResetHitList();
    }
}

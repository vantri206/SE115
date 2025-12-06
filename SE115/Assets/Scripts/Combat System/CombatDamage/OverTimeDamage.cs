using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class OverTimeDamage : Damage
{
    [SerializeField] float overTime = 0.5f;
    private float overTimer = 0.0f;

    private List<IDamageable> hitList = new List<IDamageable>();
    public void ResetHitList()
    {
        hitList.Clear();
    }
    private void Update()
    {
        overTimer += Time.deltaTime;
        if (overTimer >= overTime)
        {
            ResetHitList();
            overTimer = 0.0f;
        }
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
        }
    }
    private void OnEnable()
    {
        ResetHitList();
    }
}

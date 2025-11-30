using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage, Vector2 sourcePos);
    bool isDead { get; }
    DamageTeam team { get; }
}

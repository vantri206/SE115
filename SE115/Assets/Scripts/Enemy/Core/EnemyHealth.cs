using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public Rigidbody2D myRigidbody;

    [Header("Interface setting")]
    public float currentHealth;
    public float maxHealth;
    public bool isDead => currentHealth <= 0;
    public DamageTeam team => damageTeam;

    [Header("Team Setting")]
    public DamageTeam damageTeam = DamageTeam.Enemy;

    private bool isInvincible;

    public Action<Vector2> onTakeDamage;
    public Action onDead;

    public Func<bool> checkBlock;
    public Func<Vector2, bool> checkBlockDirection;

    public void Start()
    {
        if (myRigidbody == null)
            myRigidbody = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;
    }
    public void TakeDamage(float damage, Vector2 sourcePos)
    {
        if (isInvincible) return;

        if (checkBlock != null && checkBlock.Invoke())
        {
            return;
        }

        if (checkBlockDirection != null && checkBlockDirection.Invoke(sourcePos))
        {
            return;
        }

        currentHealth -= damage;
<<<<<<< Updated upstream:SE115/Assets/Scripts/Enemy/EnemyHealth.cs

        onTakeDamage?.Invoke(sourcePos);
        if (isDead) onDead?.Invoke();
=======
        onHealthChanged?.Invoke();
        onTakeDamage.Invoke(sourcePos);
        if (isDead) onDead.Invoke();
>>>>>>> Stashed changes:SE115/Assets/Scripts/Enemy/Core/EnemyHealth.cs
    }
    public void SetInvincible(bool invincible)
    {
        isInvincible = invincible;
    }
}

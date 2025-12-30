using System;
using System.Linq;
using UnityEngine;

public class BossEnemyHealth : MonoBehaviour, IDamageable
{
    public Rigidbody2D myRigidbody;

    [Header("Interface setting")]

    public float currentHealth;
    public float maxHealth;
    public bool isDead => currentHealth <= 0;
    public DamageTeam team => damageTeam;

    [Header("Team Setting")]

    public DamageTeam damageTeam = DamageTeam.Enemy;

    public bool isInvincible = false;

    public Action onTakeDamage;
    public Action onDead;
    public Action<float, float, float> onHealthChanged;

    public void Awake()
    {
        if (myRigidbody == null)
            myRigidbody = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;
        onHealthChanged?.Invoke(currentHealth, currentHealth, maxHealth);
    }
    public void TakeDamage(float damage, Vector2 sourcePos)
    {
        float healthBefore = currentHealth;

        if (isInvincible) return;
        currentHealth -= damage;

        float healthAfter = currentHealth;

        onHealthChanged?.Invoke(healthBefore, healthAfter, maxHealth);

        onTakeDamage?.Invoke();
        if (isDead)
            onDead?.Invoke();
    }
    public void ReceiveHeal(float healAmount)
    {
        float healthBefore = currentHealth;

        currentHealth += healAmount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        float healthAfter = currentHealth;

        onHealthChanged?.Invoke(healthBefore, healthAfter, maxHealth);
    }
    public void SetInvincible(bool invincible)
    {
        isInvincible = invincible;
    }
}

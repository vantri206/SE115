using System;
using System.Linq;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public Rigidbody2D myRigidbody;

    [Header("Interface setting")]

    public float currentHealth;
    public float maxHealth;
    public bool isDead => currentHealth <= 0;
    public DamageTeam team => damageTeam;

    [Header("Team Setting")]

    public DamageTeam damageTeam = DamageTeam.Player;

    public bool isInvincible = false; 

    public Action onTakeDamage;
    public Action onDead;
    public Action<float, float> onHealthChanged;

    public void Awake()
    {
        if(myRigidbody == null)
            myRigidbody = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    public void TakeDamage(float damage, Vector2 sourcePos)
    {
        if (isInvincible) return;
        currentHealth -= damage;

        onHealthChanged?.Invoke(currentHealth, maxHealth);

        onTakeDamage?.Invoke();
        if (isDead) 
            onDead?.Invoke();
    }
    public void ReceiveHeal(float healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth) 
            currentHealth = maxHealth;

        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    public void SetInvincible(bool invincible)
    {
        isInvincible = invincible;
    }
}

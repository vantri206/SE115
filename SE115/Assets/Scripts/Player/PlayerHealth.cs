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
    public Action<float, float, float> onHealthChanged;

    public void Awake()
    {
        if(myRigidbody == null)
            myRigidbody = GetComponent<Rigidbody2D>();

        onHealthChanged?.Invoke(currentHealth, currentHealth, maxHealth);
    }
    public void TakeDamage(float damage, Vector2 sourcePos)
    {
        float healthBefore = currentHealth;

        if (isInvincible) return;
        currentHealth -= damage;

        currentHealth = Mathf.Max(0.0f, currentHealth);

        onHealthChanged?.Invoke(healthBefore, currentHealth, maxHealth);

        onTakeDamage?.Invoke();

        if (isDead)
        {
            if(!CheckAstralSkill())
                onDead?.Invoke();
        } 
        
    }
    public void ReceiveHeal(float healAmount)
    {
        float healthBefore = currentHealth;
       
        currentHealth += healAmount;

        if (currentHealth > maxHealth) 
            currentHealth = maxHealth;

        onHealthChanged?.Invoke(healthBefore, currentHealth, maxHealth);
    }
    public void SetCurrentHeal(float amount)
    {
        float healthBefore = currentHealth;

        amount = Mathf.Min(amount, maxHealth);
        currentHealth = amount;

        onHealthChanged?.Invoke(healthBefore, currentHealth, maxHealth);
    }
    public void SetMaxHeal(float amount)
    {
        maxHealth = amount;

        currentHealth = Mathf.Min(currentHealth, maxHealth);

        onHealthChanged?.Invoke(currentHealth, currentHealth, maxHealth);
    }
    public void SetInvincible(bool invincible)
    {
        isInvincible = invincible;
    }
    public bool CheckAstralSkill()
    {
        AstralTimeRunner astralRunner = GetComponent<AstralTimeRunner>();

        if (astralRunner != null)
        {
            astralRunner.AstralReturn();
            return true;
        }

        return false;
    }
}

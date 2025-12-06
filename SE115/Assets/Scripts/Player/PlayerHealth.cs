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

    public System.Action onTakeDamage;
    public System.Action onDead;

    public void Start()
    {
        if(myRigidbody == null)
            myRigidbody = GetComponent<Rigidbody2D>();
    }
    public void TakeDamage(float damage, Vector2 sourcePos)
    {
        if (isInvincible) return;
        currentHealth -= damage;
        onTakeDamage.Invoke();
        if (isDead) onDead.Invoke();
    }
    public void ReceiveHeal(float healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }
    public void SetInvincible(bool invincible)
    {
        isInvincible = invincible;
    }
}

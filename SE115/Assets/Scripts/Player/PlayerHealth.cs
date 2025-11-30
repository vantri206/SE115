using System.Linq;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Interface setting")]
    public float currentHealth;
    public float maxHealth;
    public bool isDead => currentHealth <= 0;
    public DamageTeam team => damageTeam;
    [Header("Team Setting")]
    public DamageTeam damageTeam = DamageTeam.Player;
    public Rigidbody2D myRigidbody;

    public System.Action onTakeDamage;
    public System.Action onDead;

    public void Start()
    {
        if(myRigidbody == null)
            myRigidbody = GetComponent<Rigidbody2D>();
    }
    public void TakeDamage(float damage, Vector2 sourcePos)
    {
        currentHealth -= damage;
        onTakeDamage.Invoke();
        if (isDead) onDead.Invoke();
    }
    public void ReceiveHeal(float healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }
}

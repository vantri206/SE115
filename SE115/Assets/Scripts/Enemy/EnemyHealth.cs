using System.Linq;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Interface setting")]
    public float currentHealth;
    public float maxHealth;
    public bool isDead => currentHealth <= 0;
    public DamageTeam team => damageTeam;

    [Header("Team Setting")]
    public DamageTeam damageTeam = DamageTeam.Enemy;

    public Rigidbody2D myRigidbody;

    public System.Action<Vector2> onTakeDamage;
    public System.Action onDead;

    public void Start()
    {
        if (myRigidbody == null)
            myRigidbody = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;
    }
    public void TakeDamage(float damage, Vector2 sourcePos)
    {
        currentHealth -= damage;
        onTakeDamage.Invoke(sourcePos);
        if (isDead) onDead.Invoke();
    }
}

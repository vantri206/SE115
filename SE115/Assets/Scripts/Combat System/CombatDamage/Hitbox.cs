using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] private Damage owner;
    private void Awake()
    {
        if (GetComponent<Damage>() != null)
            owner = GetComponent<Damage>();
        else owner = transform.parent.GetComponent<Damage>();
    }
    private void OnTriggerStay2D(Collider2D collider)
    {
        HandleCollision(collider);
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        HandleCollision(collider);
    }
    private void HandleCollision(Collider2D collider)
    {
        if (collider.gameObject.layer != LayerMask.NameToLayer("Hurtbox")) return;

        IDamageable targetHealth = null;
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();

        if (hurtbox != null)
            targetHealth = hurtbox.health;
        if (targetHealth != null)
        {
            owner.DealDamage(targetHealth);
        }
    }
}
